using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace MobileVikings_Wrapper
{
    //https://mobilevikings.com/api/2.0/doc/#basic-authentication

    public class MobVikDataModel
    {
        private readonly WebClient _webclient;

        public MobVikDataModel(string username, string password)
        {
            _webclient = new WebClient();
            //Gallantly copied from: //http://mobilevikingsdata.codeplex.com/SourceControl/changeset/view/56993#1076285
            string base64Creds =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)));
            _webclient.Headers["Authorization"] = "Basic " + base64Creds;
        }

        public List<string> GetTelePhoneList()
        {
            string reqresult =
                (_webclient.DownloadString(new Uri("https://mobilevikings.com/api/2.0/basic/msisdn_list.xml")));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?
            var result = new List<string>();
            if (resp != null)
            {
                result.AddRange(resp.Elements("resource").Select(element => element.Value));
            }
            return result;
        }

        public PricePlan GetPricePlanDetails(string telephoneno)
        {
            var result = new PricePlan();

            string reqresult =
                (_webclient.DownloadString(new Uri("https://mobilevikings.com/api/2.0/basic/price_plan_details.xml")));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response");

            if (resp != null)
            {
                XElement prices = resp.Element("prices");
                if (prices != null)
                    foreach (XElement price in prices.Elements("resource"))
                    {
                        var xElement = price.Element("amount");
                        if (xElement != null)
                        {
                            var pricetemp = new Price
                                                {
                                                    Amount =
                                                        double.Parse(xElement.Value,
                                                                     CultureInfo.InvariantCulture)
                                                };
                            var element = price.Element("type_id");
                            if (element != null)
                                pricetemp.SetType(int.Parse(element.Value, CultureInfo.InvariantCulture));
                            result.Prices.Add(pricetemp);
                        }
                    }
                XElement bundles = resp.Element("bundles");
                if (bundles != null)
                    foreach (XElement bundle in bundles.Elements("resource"))
                    {
                        var xElement = bundle.Element("amount");
                        if (xElement != null)
                        {
                            var bundletemp = new Bundle
                                                 {
                                                     Amount =
                                                         double.Parse(xElement.Value,
                                                                      CultureInfo.InvariantCulture)
                                                 };
                            var element = bundle.Element("type_id");
                            if (element != null)
                                bundletemp.SetType(int.Parse(element.Value, CultureInfo.InvariantCulture));
                            result.Bundles.Add(bundletemp);
                        }
                    }

                var xElement1 = resp.Element("name");
                if (xElement1 != null) result.Name = xElement1.Value;
                var element1 = resp.Element("top_up_amount");
                if (element1 != null)
                    result.TopUpAmount = double.Parse(element1.Value, CultureInfo.InvariantCulture);
            }
            return result;
        }


        public SimBalance GetSimBalance(string telephoneno, bool addPricePlan = false)
        {
            string reqresult;
            if (!addPricePlan)
                reqresult =
                    (_webclient.DownloadString(
                        new Uri("https://mobilevikings.com/api/2.0/basic/sim_balance.xml?msisdn=" + telephoneno)));
            else
            {
                reqresult =
                    (_webclient.DownloadString(
                        new Uri("https://mobilevikings.com/api/2.0/basic/sim_balance.xml?msisdn=" + telephoneno +
                                "&add_price_plan=1")));
            }

            var result = new SimBalance();

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?


            if (resp != null)
            {
                var xElement = resp.Element("is_expired");
                if (xElement != null) result.IsExpired = bool.Parse(xElement.Value);
                var element = resp.Element("data");
                if (element != null)
                    result.Data = long.Parse(element.Value, CultureInfo.InvariantCulture);
                var xElement1 = resp.Element("valid_until");
                if (xElement1 != null)
                    result.ValidUntil = DateTime.Parse(xElement1.Value);
                var element1 = resp.Element("credits");
                if (element1 != null)
                    result.Credits = Double.Parse(element1.Value, CultureInfo.InvariantCulture);
                var xElement2 = resp.Element("sms");
                if (xElement2 != null)
                    result.Sms = uint.Parse(xElement2.Value, CultureInfo.InvariantCulture);
                var element2 = resp.Element("sms_super_on_net");
                if (element2 != null)
                    result.SmsSuperOnNet = uint.Parse(element2.Value, CultureInfo.InvariantCulture);

                if (addPricePlan)
                {
                    var xElement3 = resp.Element("price_plan");
                    if (xElement3 != null) result.PricePlan = xElement3.Value;
                }
            }

            return result;
        }


        public List<Topuphistory> GetTopUpHistory(string telephoneno, DateTime? fromDate = null,
                                                  DateTime? untilDate = null, uint pageSize = 25, uint pageNo = 1)

        {
            /*
             * "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n
             * <response>
             * <resource>
             * <status>Top-up done</status>
             * <amount>15.00</amount>
             * <amount_ex_vat>12.40</amount_ex_vat>
             * <executed_on>2011-07-30 16:59:02</executed_on>
             * <method>PayPal</method>
             * <payment_received_on>2011-07-30 16:58:34</payment_received_on>
             * </resource>
             * <resource><status>Top-up done</status><amount>15.00</amount><amount_ex_vat>12.40</amount_ex_vat><executed_on>2011-07-09 11:38:42</executed_on><method>150 Viking Points</method><payment_received_on>2011-07-09 11:38:31</payment_received_on></resource><resource><status>Top-up done</status><amount>15.00</amount><amount_ex_vat>12.40</amount_ex_vat><executed_on>2011-06-07 17:03:00</executed_on><method>PayPal</method><payment_received_on>2011-06-07 17:02:46</payment_received_on></resource><resource><status>Top-up done</status><amount>15.00</amount><amount_ex_vat>12.40</amount_ex_vat><executed_on>2011-05-06 10:51:30</executed_on><method>PayPal</method><payment_received_on>2011-05-05 22:27:22</payment_received_on></resource></response>"
             * */
            var result = new List<Topuphistory>();

            string mainquerystring = "https://mobilevikings.com/api/2.0/basic/top_up_history.xml?msisdn=" + telephoneno +
                                     "&page_size=" + pageSize + "&page=" + pageNo;
            if (fromDate.HasValue)
                mainquerystring += "&from_date=" + fromDate.Value.ToString("YYYY-MM-DDTHH:MM:SS");
            if (untilDate.HasValue)
                mainquerystring += "&until_date=" + untilDate.Value.ToString("YYYY-MM-DDTHH:MM:SS");

            string reqresult = (_webclient.DownloadString(new Uri(mainquerystring)));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                result.AddRange(resp.Elements("resource").Select(history => new Topuphistory
                                                                                {
                                                                                    Amount =
                                                                                        double.Parse(
                                                                                            history.Element("amount").
                                                                                                Value,
                                                                                            CultureInfo.InvariantCulture),
                                                                                    AmountExVat =
                                                                                        double.Parse(
                                                                                            history.Element(
                                                                                                "amount_ex_vat").Value,
                                                                                            CultureInfo.InvariantCulture),
                                                                                    ExecutedOn =
                                                                                        DateTime.Parse(
                                                                                            history.Element(
                                                                                                "executed_on").Value),
                                                                                    PaymentReceivedOn =
                                                                                        DateTime.Parse(
                                                                                            history.Element(
                                                                                                "payment_received_on").
                                                                                                Value),
                                                                                    Paymethod =
                                                                                        history.Element("method").Value,
                                                                                    Status =
                                                                                        history.Element("status").Value
                                                                                }));
            }

            return result;
        }

        public List<UsageHistory> GetUsagHistory(string telephoneno, DateTime? fromDate = null,
                                                 DateTime? untilDate = null, uint pageSize = 25, uint pageNo = 1,
                                                 bool addPricePlan = false)
        {
            /*<response>
                  <resource>
                    <is_data>False</is_data>
                    <start_timestamp>2011-08-25 11:44:22</start_timestamp>
                    <balance>0.00</balance>
                    <duration_call>1</duration_call>
                    <to>0477313639</to>
                    <is_sms>True</is_sms>
                    <timestamp>1314265462</timestamp>
                    <price>0.00</price>
                    <duration_connection>1</duration_connection>
                    <duration_human>n/a</duration_human>
                    <is_incoming>True</is_incoming>
                    <is_voice>False</is_voice>
                    <is_mms>False</is_mms>
                    <end_timestamp>2011-08-25 11:44:23</end_timestamp>
                  </resource>
                  <resource>
                    <is_data>False</is_data>
                    <start_timestamp>2011-08-25 11:06:36</start_timestamp>
                    <balance>8.28</balance>
                    <duration_call>2</duration_call>
                    <to>0477313639</to>
                    <is_sms>True</is_sms>
                    <timestamp>1314263196</timestamp>
                    <price>0.00</price>
                    <duration_connection>1</duration_connection>
                    <duration_human>n/a</duration_human>
                    <is_incoming>False</is_incoming>
                    <is_voice>False</is_voice>
                    <is_mms>False</is_mms>
                    <end_timestamp>2011-08-25 11:06:38</end_timestamp>
                  </resource>
                </response>
*/
            var result = new List<UsageHistory>();

            string mainquerystring = "https://mobilevikings.com/api/2.0/basic/usage.xml?msisdn=" + telephoneno +
                                     "&page_size=" + pageSize + "&page=" + pageNo;
            if (fromDate.HasValue)
                mainquerystring += "&from_date=" + fromDate.Value.ToString("YYYY-MM-DDTHH:MM:SS");
            if (untilDate.HasValue)
                mainquerystring += "&until_date=" + untilDate.Value.ToString("YYYY-MM-DDTHH:MM:SS");
            if (addPricePlan)
                mainquerystring += "&add_price_plan=1";
            string reqresult = (_webclient.DownloadString(new Uri(mainquerystring)));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                foreach (XElement history in resp.Elements("resource"))
                {
                    var tempusage = new UsageHistory
                                        {
                                            IsData = bool.Parse(history.Element("is_data").Value),
                                            StartTimestamp =
                                                DateTime.Parse(history.Element("start_timestamp").Value),
                                            Balance =
                                                double.Parse(history.Element("balance").Value,
                                                             CultureInfo.InvariantCulture),
                                            DurationCall =
                                                uint.Parse(history.Element("duration_call").Value,
                                                           CultureInfo.InvariantCulture),
                                            To = history.Element("to").Value,
                                            IsSms = bool.Parse(history.Element("is_sms").Value),
                                            Timestamp = history.Element("timestamp").Value,
                                            Price =
                                                double.Parse(history.Element("price").Value,
                                                             CultureInfo.InvariantCulture),
                                            DuractionConnection =
                                                uint.Parse(history.Element("duration_connection").Value,
                                                           CultureInfo.InvariantCulture),
                                            DurationHuman = history.Element("duration_human").Value,
                                            IsIncoming = bool.Parse(history.Element("is_incoming").Value),
                                            IsMms = bool.Parse(history.Element("is_mms").Value),
                                            EndTimestamp =
                                                DateTime.Parse(history.Element("end_timestamp").Value)
                                        };

                    if (addPricePlan)
                        tempusage.PricePlan = resp.Element("price_plan").Value;
                    result.Add(tempusage);
                }
            }
            return result;
        }

//done

        public SimCardInfo GetSimCardInfo(string telephoneno)
        {
            var result = new SimCardInfo();

            string reqresult =
                (_webclient.DownloadString(
                    new Uri("https://mobilevikings.com/api/2.0/basic/sim_info.xml?msisdn=" + telephoneno)));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                var xElement = resp.Element("cardnumber");
                if (xElement != null)
                    result.CardNumber = long.Parse(xElement.Value, CultureInfo.InvariantCulture);
                var element = resp.Element("imsi");
                if (element != null)
                    result.Imsi = long.Parse(element.Value, CultureInfo.InvariantCulture);
                var xElement1 = resp.Element("msisdn");
                if (xElement1 != null) result.Telephoneno = xElement1.Value;
                var element1 = resp.Element("pin1");
                if (element1 != null)
                    result.Pin1 = uint.Parse(element1.Value, CultureInfo.InvariantCulture);
                var xElement2 = resp.Element("pin2");
                if (xElement2 != null)
                    result.Pin2 = uint.Parse(xElement2.Value, CultureInfo.InvariantCulture);
                var element2 = resp.Element("puk1");
                if (element2 != null)
                    result.Puk1 = uint.Parse(element2.Value, CultureInfo.InvariantCulture);
                var xElement3 = resp.Element("puk2");
                if (xElement3 != null)
                    result.Puk2 = uint.Parse(xElement3.Value, CultureInfo.InvariantCulture);
            }
            return result;
        }

        //done

        public List<VikPointsLink> GetVikingsPointsLinks()
        {
            /*
             * <response>
                  <resource>
                    <alias>+32486181475</alias>
                    <link>http://mobilevikings.com/referral/xMqZNINKilfkaNIBYpLOOrdmsxEwEiEx/</link>
                  </resource>
                </response>
             * */
            var result = new List<VikPointsLink>();

            string reqresult =
                (_webclient.DownloadString(
                    new Uri("https://mobilevikings.com/api/2.0/basic/points/links.xml")));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                result.AddRange(resp.Elements("resource").Select(link => new VikPointsLink
                                                                             {
                                                                                 Alias = link.Element("alias").Value,
                                                                                 Link =
                                                                                     new Uri(link.Element("link").Value)
                                                                             }));
            }
            return result;
        }

        //done

        public VikPointsStats GetVikingsPointsStatistics()
        {
            /*

            <response>
                  <used_points>150</used_points>
                  <unused_points>150</unused_points>
                  <waiting_points>0</waiting_points>
                  <topups_used>1</topups_used>
                  <earned_points>300</earned_points>
                </response>
             * */
            var result = new VikPointsStats();

            string reqresult =
                (_webclient.DownloadString(
                    new Uri("https://mobilevikings.com/api/2.0/basic/points/stats.xml")));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                var xElement = resp.Element("used_points");
                if (xElement != null)
                    result.UsedPoints = uint.Parse(xElement.Value, CultureInfo.InvariantCulture);
                var element = resp.Element("unused_points");
                if (element != null)
                    result.UnUsedPoints = uint.Parse(element.Value, CultureInfo.InvariantCulture);
                var xElement1 = resp.Element("waiting_points");
                if (xElement1 != null)
                    result.WaitingPoints = uint.Parse(xElement1.Value, CultureInfo.InvariantCulture);
                var element1 = resp.Element("topups_used");
                if (element1 != null)
                    result.TopupUsed = uint.Parse(element1.Value, CultureInfo.InvariantCulture);
                var xElement2 = resp.Element("earned_points");
                if (xElement2 != null)
                    result.EarnedPoints = uint.Parse(xElement2.Value, CultureInfo.InvariantCulture);
            }
            return result;
        }

        public List<VikPointsRef> GetVikingsPointsReferals(uint pageSize = 25, uint pageNo = 1)
        {
            var result = new List<VikPointsRef>();

            string mainquerystring = "https://mobilevikings.com/api/2.0/basic/points/referrals.xml?page_size=" +
                                     pageSize + "&page=" + pageNo;


            string reqresult = (_webclient.DownloadString(new Uri(mainquerystring)));

            XDocument xDocument = XDocument.Parse(reqresult);
            XElement resp = xDocument.Element("response"); //Else fout?

            if (resp != null)
            {
                result.AddRange(resp.Elements("resource").Select(refs => new VikPointsRef
                                                                             {
                                                                                 Status = refs.Element("status").Value,
                                                                                 Amount =
                                                                                     uint.Parse(
                                                                                         refs.Element("amount").Value,
                                                                                         CultureInfo.InvariantCulture),
                                                                                 Date =
                                                                                     DateTime.Parse(
                                                                                         refs.Element("date").Value,
                                                                                         CultureInfo.InvariantCulture),
                                                                                 Method = refs.Element("method").Value,
                                                                                 Name = refs.Element("name").Value,
                                                                                 MethodStr =
                                                                                     refs.Element("method_str").Value
                                                                             }));
            }
            return result;
        }
    }
}