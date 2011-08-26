using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileVikings_Wrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            //Example usage
            var mainModel= new MobVikDataModel("username", "password");

            //First ask list of all telephonenumber associated to user
            List<string> telephones= mainModel.GetTelePhoneList();

            //Ask simbalance of first telephoneno in list
            if (telephones.Count > 0)
            {
                SimBalance myBalance = mainModel.GetSimBalance(telephones[0]);
                //Show simbalance
                Console.WriteLine("{0},{1},{2}", myBalance.Credits, myBalance.Data, myBalance.PricePlan);
            }


        }
    }
}
