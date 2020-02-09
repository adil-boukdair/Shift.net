using System;
using System.Collections.Generic;
using PayPal.Api;
using static SRYelpAPI.YelpSearchAgent.SearchResponse;
using SRYelpAPI;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Expressions;
using shiftreportapp.data;

namespace ShiftreportLib
{
    public class PayPalProcessor
    {

    
        public static String mode = System.Configuration.ConfigurationManager.AppSettings["Paypal_mode"];//sandbox
        /* Live clientID and Secret */
        public static String clientID = System.Configuration.ConfigurationManager.AppSettings["Paypal_ClientID"];
        public static String secret = System.Configuration.ConfigurationManager.AppSettings["Paypal_Secret"];
        
        /* SandBox clientID and Secret 
        public static String clientID = "Aa3fQicf6E747E5Snc9I2XgEmGbOIyKBTkmVHDfU6BfjD6mSn-7goLwYSSrOYuUrbD6Ifb5CzQa7R_BH";
        public static String secret = "EE4BJAi8KirBrsu11CDWhKzhIxmoYxwuw0ZL7StRcSrvxC524vA3-KJndFQgDJxoyuBu-Q7byo9YiLMW";
       */
        public static String currency = "USD";


        public static Payment Pay(CreditCard creditCard, float amount)
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

            sdkConfig.Add("mode", mode);
            string accessToken = new OAuthTokenCredential(clientID, secret, sdkConfig).GetAccessToken();

            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = sdkConfig;


            // Adding Credit card
            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = creditCard;

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);


            // Create Payment details
            Payer payer = new Payer();
            payer.funding_instruments = fundingInstrumentList;
            payer.payment_method = "credit_card";

            Amount amnt = new Amount();
            amnt.currency = currency;
            amnt.total = amount.ToString("F");

            Transaction tran = new Transaction();
            tran.description = "VenueCash Payment";
            tran.amount = amnt;


            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            Payment payment = new Payment();
            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions = transactions;

         
                Payment createdPayment = payment.Create(apiContext);

                return createdPayment;
            
        }


        public static Refund refund(string sale_id,string amount)
        {

            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

            sdkConfig.Add("mode", mode);
            string accessToken = new OAuthTokenCredential(clientID, secret, sdkConfig).GetAccessToken();

            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = sdkConfig;
 

            var refund = new Refund()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    total = amount
                }
            };

            Sale refundSale = new Sale()
            {
                id = sale_id
            };


            Refund refundResponse = refundSale.Refund(apiContext, refund);

 
            return refundResponse;
        }


        // Authorize API
        public static Payment Authorize(CreditCard creditCard, float amount)
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

            sdkConfig.Add("mode", mode);
            string accessToken = new OAuthTokenCredential(clientID, secret, sdkConfig).GetAccessToken();

            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = sdkConfig;


            // Adding Credit card
            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = creditCard;

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);


            // Create Payment details
            Payer payer = new Payer();
            payer.funding_instruments = fundingInstrumentList;
            payer.payment_method = "credit_card";

            Amount amnt = new Amount();
            amnt.currency = currency;
            amnt.total = amount.ToString("F");

            Transaction tran = new Transaction();
            tran.description = "VenueCash Payment";
            tran.amount = amnt;


            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            Payment payment = new Payment();
            payment.intent = "authorize";
            payment.payer = payer;
            payment.transactions = transactions;


            Payment createdPayment = payment.Create(apiContext);

            return createdPayment;

        }
        //VoidAuthorization
        public static Authorization VoidAuthorize(string authorizationID, float amount)
        {
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

            sdkConfig.Add("mode", mode);
            string accessToken = new OAuthTokenCredential(clientID, secret, sdkConfig).GetAccessToken();

            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = sdkConfig;

            var auth = new Authorization();
            auth.id = authorizationID;//authorization ID
            Amount amnt = new Amount();
            amnt.currency = currency;
            amnt.total = amount.ToString("F");
            auth.amount = amnt;

            var authResponse = auth.Void(apiContext);

            return authResponse;
        }

    }// end PayPalProcessor calss






    // YELP API V3
    public class YelpV3
    {


        static String CONSUMER_KEY {  get  {  return System.Configuration.ConfigurationSettings.AppSettings["yelp_Consumer_Key"];   }  }
        static String CONSUMER_SECRET  { get { return System.Configuration.ConfigurationSettings.AppSettings["yelp_Consumer_Secret"]; } }
        static String TOKEN { get { return System.Configuration.ConfigurationSettings.AppSettings["yelp_Token"]; } }
        static String TOKEN_SECRET  {get{ return System.Configuration.ConfigurationSettings.AppSettings["yelp_Token_Secret"]; }}


        public static RootObject Search(int customerId, float latitude,  float longitude, string term, String location,String categories, int sort_by, int offset)
        {

            if (sort_by == 2) { sort_by = 0; }

            YelpContext cont = new YelpContext(TOKEN, TOKEN_SECRET, CONSUMER_KEY, CONSUMER_SECRET);
            SRYelpAPI.YelpSearchAgent search = new SRYelpAPI.YelpSearchAgent();

            

            // set location
            if (location != null)
            {
                cont.Location = location;
            }
            else
            {
                if(latitude!=-1 && longitude != -1)
                {
                    cont.setGelocation2(latitude, longitude);
                }
            }


            // set categories
            if(categories!=null)cont.categoryFilter = categories;
            //set Term
            if(term!=null)cont.Term = term;
            // sortby 
            cont.Sort = "distance";
            cont.Offset = offset;

            var searchResults = search.Send(cont);

            var searchResultObject = JsonConvert.DeserializeObject<RootObject>(searchResults);

            
            return searchResultObject;




        }

        public static Business getBusiness(string ID)
        {

            YelpContext cont = new YelpContext(TOKEN, TOKEN_SECRET, CONSUMER_KEY, CONSUMER_SECRET);
            SRYelpAPI.YelpBusinessAgent search = new SRYelpAPI.YelpBusinessAgent();


            cont.setID(ID);

            var business = search.getBusiness(cont,ID);

            var businessObject = JsonConvert.DeserializeObject<Business>(business);


            return businessObject;
        }



        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'M')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }





    }
     


    public static class VCBalanceHelper
    {

        public static double nearToZero(double value)
        {
            if(value < 0.01 || value < 0)
            {
                return 0;
            }
            else
            {
                return Math.Round(value,2);
            }
        }

        public static double preventSingedBalance (double value)
        {// this prevent from having balance inferior to zero or to 0.01
            if (value < 0.01 || value < 0)
            {
                return 0;
            }
            else
            {
                return value;
            }


        }

        public static double getCCFees (double cc_amount,string cc_type)
        {
            if (cc_type.ToLower() == "amex") // fee for american express cards
            {
                return nearToZero( cc_amount -  ((cc_amount * 0.96) - 0.30  ));
            }
            else // fees for Visa/masterCard/discover
            {
                return nearToZero(cc_amount - ((cc_amount*0.965) - 0.30 ));
            }
        }



    }

    public class usefullFunctions
    {
        
        public static int getNextOrderNumber(int store_id,int business_type)
        {
            if (business_type == 2) { 
            AppModel Context = new AppModel();

            var transaction = Context.vc_CustomerTransactions2.Where(t=> t.StoreId==store_id).OrderByDescending(t=>t.Id).FirstOrDefault();

                // Case if no transaction was emitted by that store
                if (transaction == null) return 1;
                // Case if order_number equal to null
                if (transaction.order_number == null) return 1;

                // check if the date of the transaction is the same date as today if false reset order_number return 0
                var nowDate = DateTime.Now.ToString("MMMM dd, yyyy");
                var transactionDate = transaction.Date.Value.ToString("MMMM dd, yyyy");

                if (nowDate == transactionDate) //increment order_number
                {
                    return ((int)transaction.order_number) + 1;
                }
                else
                {
                    return 1; // reset order_number for the next day
                }

            }
            else return -1;
             



            
        }

    }

}