using System;
using System.Collections.Generic;
using System.IO;
using BiomedicaLib.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftreportLib.Helpers;

namespace ShiftreportLib
{
    public class VenueCashEmails
    {


        public static string newAccountBusiness(string email,string username, string companyName, string userID, string password)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/newAccountBusiness.html");




            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", username);
                body = body.Replace("*[company]*", companyName);
                body = body.Replace("*[id]*", userID);
                body = body.Replace("*[pw]*", password);



                MailLogService.SendMail("support@shiftreports.com", email, "You're in! Let's get started.", body);

                return body;
            }

         
        }



        public static string newStoreBusiness(string email, string username, string companyName, string userID, string password,string storeName)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/newStoreBusiness.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", username);
                body = body.Replace("*[company]*", companyName);
                body = body.Replace("*[store name]*", storeName);
                body = body.Replace("*[id]*", userID);
                body = body.Replace("*[pw]*", password);



                MailLogService.SendMail("support@shiftreports.com", email, "You're in! Let's get started.", body);

                return body;
            }


        }

        public static string newManagerBusiness(string email, string username, string companyName, string userID, string password,string storeName, string access_level)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/newManagerBusiness.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();


    
                //*[access_level_txt]*
                if (access_level=="ASM" )
                {//
                    body = body.Replace("*[access_level_txt]*", "You where added as assistant manager ");
                    body = body.Replace("*[css_block_1]*", "none");
                }
                else
                {
                    body = body.Replace("*[access_level_txt]*", "You where added as manager");
                    body = body.Replace("*[css_block_1]*", "block");
                }
                // store name 
                body = body.Replace("*[store name]*", storeName);


                body = body.Replace("*[user first name]*", username);
                body = body.Replace("*[company]*", companyName);
        
                body = body.Replace("*[id]*", userID);
                body = body.Replace("*[pw]*", password);



                MailLogService.SendMail("support@shiftreports.com", email, "You're in! Let's get started.", body);

                return body;
            }


        }


        public static string newCashierEmployee(string email, string username, string companyName, string userID, string password, string storeName)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/newCashierEmployee.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", username);
                body = body.Replace("*[company]*", companyName);
                body = body.Replace("*[store name]*", storeName);
                body = body.Replace("*[id]*", userID);
                body = body.Replace("*[pw]*", password);



                MailLogService.SendMail("support@shiftreports.com", email, "You're in! Let's get started.", body);

                return body;
            }


        }


        public static string newCustomer(string email, string name)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/newCustomer.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);
 



                MailLogService.SendMail("support@shiftreports.com", email, "You're in! Let's get started.", body);

                return body;
            }


        }


        public static string salesOrderConfirmationCustomer(string email, string firstName,string store_name,string store_address, vc_CustomerTransactions transaction, vc_CustomerCreditCards creditCard)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/saleConfirmationCustomer.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", firstName);
                body = body.Replace("*[business name]*", store_name);
                body = body.Replace("*[business address]*", store_address);
                // transaction
                body = body.Replace("*[order#]*", transaction.Id.ToString());
                body = body.Replace("*[mm/dd/yy]*", String.Format("{0:MM/dd/yyyy}", transaction.Date));
                body = body.Replace("*[Sale amount]*", String.Format("{0:C}", transaction.Sub_TotalAmount));
                // other financing
                if (transaction.OtherFinancingAmount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">Other Financing Amount:</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.OtherFinancingAmount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[Other Financing Amount]*", html);
                }
                else {
                    body = body.Replace("*[Other Financing Amount]*", "");
                }
                // tips
                if (transaction.Tip_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">Tip:</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.Tip_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[tip amount]*", html);
                }
                else
                {
                    body = body.Replace("*[tip amount]*", "");
                }
                //Total Order Amount
                body = body.Replace("*[order amount]*", String.Format("{0:C}", transaction.TotalAmount + transaction.OtherFinancingAmount));
                //payments methods
                //*[paid_cc]*
                if (transaction.Paid_Credit_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">" + creditCard.CardType + " xxxx-" + AppHelpers.GetLastFourDigits(creditCard.CardNumber).ToString() + "</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.Paid_Credit_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_cc]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_cc]*", "");
                }
                //*[paid_vc]*
                if (transaction.Paid_VenueCash_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\" class=\"vc-money\">VenueCash</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"vc-money\"> " + String.Format("{0:C}", transaction.Paid_VenueCash_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_vc]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_vc]*", "");
                }
                //*[paid_ac]*
                if (transaction.Paid_AllCash_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\" class=\"ac-money\">AllCash</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"ac-money\"> " + String.Format("{0:C}", transaction.Paid_AllCash_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_ac]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_ac]*", "");
                }
                //*[vc_earned]*
                if (transaction.VenueCashEarned_Total > 0)
                {
                    string html = "<tr style=\"border-top:solid 1px #A9A9A9;\">" +
                                  "<td style=\"width:50%\" class=\"vc-color\">VenueCash Earned</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"vc-color\"> " + String.Format("{0:C}", transaction.VenueCashEarned_Total) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[vc_earned]*", html);
                }
                else
                {
                    body = body.Replace("*[vc_earned]*", "");
                }






                // VenueCash earned
                body = body.Replace("*[VC amount earned]*", String.Format("{0:C}", transaction.VenueCashEarned_Total));

                if (transaction.VenueCashEarned_Total > 0) { 
                MailLogService.SendMail("support@shiftreports.com", email, "Yesss! You earned " + String.Format("{0:C}", transaction.VenueCashEarned_Total), body);
                }
                else
                {
                    MailLogService.SendMail("support@shiftreports.com", email, "Thank you for your Order!", body);
                }

                return body;
            }

        }



        public static string refundOrderConfirmationCustomer(string email, string firstName, string store_name, string store_address, vc_CustomerTransactions transaction, vc_CustomerCreditCards creditCard,List<vc_refunds> refunds)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/refundOrderConfirmationCustomer.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", firstName);
                body = body.Replace("*[business name]*", store_name);
                body = body.Replace("*[business address]*", store_address);
                // transaction
                body = body.Replace("*[order#]*", transaction.Id.ToString());
                body = body.Replace("*[mm/dd/yy]*", String.Format("{0:MM/dd/yyyy}", transaction.Date));
                body = body.Replace("*[Sale amount]*", String.Format("{0:C}", transaction.Sub_TotalAmount));
                // other financing
                if (transaction.OtherFinancingAmount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">Other Financing Amount:</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.OtherFinancingAmount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[Other Financing Amount]*", html);
                }
                else
                {
                    body = body.Replace("*[Other Financing Amount]*", "");
                }
                // Tips
                if (transaction.Tip_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">Tips:</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.Tip_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[tips]*", html);
                }
                else
                {
                    body = body.Replace("*[tips]*", "");
                }

                //Total Order Amount
                body = body.Replace("*[order amount]*", String.Format("{0:C}", transaction.TotalAmount +transaction.OtherFinancingAmount));
                //payments methods
                //*[paid_cc]*
                if (transaction.Paid_Credit_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\">"+ creditCard.CardType +" xxxx-"+ AppHelpers.GetLastFourDigits(creditCard.CardNumber).ToString()+"</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", transaction.Paid_Credit_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_cc]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_cc]*", "");
                }
                //*[paid_vc]*
                if (transaction.Paid_VenueCash_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\" class=\"vc-money\">VenueCash</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"vc-money\"> " + String.Format("{0:C}", transaction.Paid_VenueCash_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_vc]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_vc]*", "");
                }
                //*[paid_ac]*
                if (transaction.Paid_AllCash_Amount > 0)
                {
                    string html = "<tr>" +
                                  "<td style=\"width:50%\" class=\"ac-money\">AllCash</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"ac-money\"> " + String.Format("{0:C}", transaction.Paid_AllCash_Amount) + "</td>"
                                  + "<tr>";

                    body = body.Replace("*[paid_ac]*", html);
                }
                else
                {
                    body = body.Replace("*[paid_ac]*", "");
                }






                // Refunds transactions
                var refund_html = "";
                double total_cc_refunded = 0;
                double total_rewards_returned = 0;
                foreach(var refund in refunds)
                {


                     refund_html = refund_html+ "<tr style=\"width:50%;border-top:solid 2px #A9A9A9;\"><td>Returned " + String.Format("{0:MM/dd/yyyy}", refund.date_of_refund) + "</td><td></td></tr><tr>" +
                                  "<td style=\"width:50%\">"+ creditCard.CardType + " xxxx-" + AppHelpers.GetLastFourDigits(creditCard.CardNumber).ToString() + "</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" > " + String.Format("{0:C}", refund.cc_refunded_amount) + "</td>"
                                  + "<tr>";


                    // VenueCash refunded
                    if(refund.vc_refunded_amount > 0) { 
                      refund_html = refund_html+ "<tr>" +
                                  "<td style=\"width:50%\" class=\"vc-money\">VenueCash</td>"
                                  + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"vc-money\"> " + String.Format("{0:C}", refund.vc_refunded_amount) + "</td>"
                                  + "<tr>";
                    }

                    // AllCash refunded
                    if (refund.ac_refunded_amount > 0)
                    {
                        refund_html = refund_html + "<tr>" +
                                    "<td style=\"width:50%\" class=\"ac-money\">AllCash</td>"
                                    + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"ac-money\"> " + String.Format("{0:C}", refund.ac_refunded_amount) + "</td>"
                                    + "<tr>";
                    }

                
                    //Total Refunded
                    refund_html = refund_html + "<tr style=\"width:50%;border-top:solid 1px #A9A9A9;\">" +
                                "<td style=\"width:50%\"><b>Total Refunded</b></td>"
                                + "<td style=\"width:50%;text-align:right; padding-right:50px;\"> " + String.Format("{0:C}", refund.cashier_refunded_amount+ refund.VenueCash_Deficient_Charge_Amount) + "</td>"
                                + "<tr>";
                    //Rewards Returned
                    if (refund.vc_earned_returned_amount > 0) { 
                    refund_html = refund_html + "<tr style=\"width:50%;border-top:solid 1px #A9A9A9;\">" +
                                "<td style=\"width:50%\" class=\"blue-text\"><b>Rewards Returned</b></td>"
                                + "<td style=\"width:50%;text-align:right; padding-right:50px;\" class=\"blue-text\"> " + String.Format("{0:C}", refund.vc_earned_returned_amount ) + "</td>"
                                + "<tr>";
                    }

                
                }// refund loop
                body = body.Replace("*[refunds_loop]*", refund_html);
 

                MailLogService.SendMail("support@shiftreports.com", email, "You got a refund!" , body);

                return body;
            }

        }



        public static string conversionToAllCashCustomer(string email, string name,double venueCash, double allCash)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/conversionToAllCashCustomer.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);
                body = body.Replace("*[VenueCash amount]*", String.Format("{0:C}", venueCash));
                body = body.Replace("*[AllCash amount]*", String.Format("{0:C}", allCash));




                MailLogService.SendMail("support@shiftreports.com", email, "You have AllCash!", body);

                return body;
            }


        }

        public static string bankAccountAddedBusiness(string email, string name)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/bankAccountAddedBusiness.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);





                MailLogService.SendMail("support@shiftreports.com", email, "Your bank information has been updated!", body);

                return body;
            }


        }

        public static string payoutSentBusiness(string email, string name)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/payoutSentBusiness.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);





                MailLogService.SendMail("support@shiftreports.com", email, "Your payout is on the way!", body);

                return body;
            }


        }

        public static string forgotPassword(string email, string name,string password)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/forgotPassword.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);
                body = body.Replace("*[pw]*", password);




                MailLogService.SendMail("support@shiftreports.com", email, "Forgot Password", body);

                return body;
            }


        }

        public static string couponRedeemedCustomer(string email, string name, double couponAmount,string store_name)
        {

            var webRequest = System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["venuecash_email_url"] + "/couponRedeemedCustomer.html");


            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var body = reader.ReadToEnd();

                body = body.Replace("*[user first name]*", name);
                body = body.Replace("*[coupon amount]*", String.Format("{0:C}", couponAmount));
                body = body.Replace("*[business name]*", store_name);




                MailLogService.SendMail("support@shiftreports.com", email, "Yesss! You got "+ String.Format("{0:C}", couponAmount)+"!", body);

                return body;
            }


        }



    }

}
