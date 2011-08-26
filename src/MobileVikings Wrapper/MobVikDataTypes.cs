using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MobileVikings_Wrapper
{
    public class VikPointsRef
    {
        public string Status { get; set; }
        public string MethodStr { get; set; }
        public uint Amount { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string Name { get; set; }
    }

    public class VikPointsStats
    {
        public uint UsedPoints { get; set; }
        public uint UnUsedPoints { get; set; }
        public uint WaitingPoints { get; set; }
        public uint TopupUsed { get; set; }
        public uint EarnedPoints { get; set; }
    }

    public class VikPointsLink
    {
        public string Alias { get; set; }
        public Uri Link { get; set; }
    }

    public class SimCardInfo
    {
        public string Telephoneno { get; set; }
        public long CardNumber { get; set; }
        public uint Pin2 { get; set; }
        public uint Pin1 { get; set; }
        public uint Puk2 { get; set; }
        public uint Puk1 { get; set; }
        public long Imsi { get; set; }
    }

    public class UsageHistory
    {
        public bool IsData { get; set; }
        public DateTime StartTimestamp { get; set; }
        public double Balance { get; set; }
        public uint DurationCall { get; set; }
        public string To { get; set; }
        public bool IsSms { get; set; }
        public string Timestamp { get; set; }
        public double Price { get; set; }
        public uint DuractionConnection { get; set; }
        public string DurationHuman { get; set; }
        public string PricePlan { get; set; }
        public bool IsIncoming { get; set; }
        public bool IsVoice { get; set; }
        public bool IsMms { get; set; }
        public DateTime EndTimestamp { get; set; }
    }

    public class Topuphistory
    {
        public double Amount { get; set; }
        public double AmountExVat { get; set; } //zonder btw
        public DateTime ExecutedOn { get; set; }
        public string Paymethod { get; set; }
        public DateTime PaymentReceivedOn { get; set; }
        public string Status { get; set; }
    }

    public class SimBalance
    {
        public DateTime ValidUntil { get; set; }
        public Double Credits { get; set; }
        public bool IsExpired { get; set; }
        public long Data { get; set; }
        public uint Sms { get; set; }
        public uint SmsSuperOnNet { get; set; }
        //Optional price plan
        public string PricePlan { get; set; }

    }

    public class PricePlan
    {
        public string Name { get; set; }
        public double TopUpAmount { get; set; }
        public List<Price> Prices { get; set; }
        public List<Bundle> Bundles { get; set; }

        public PricePlan()
        {
            Prices = new List<Price>();
            Bundles = new List<Bundle>();
        }
    }

    public class Price
    {
        public enum TrafficTypes { Voice = 1, Data = 2, Sms = 5, Mms = 7, SmsSuper = 15 }

        public double Amount { get; set; }
        public TrafficTypes TrafficType { get; set; }

        public void SetType(int element)
        {
            TrafficType = (TrafficTypes) element;
        }
    }

    public class Bundle : Price
    {
    }
}
