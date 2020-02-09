using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayPal.Api;

namespace ShiftreportLib
{
	public class PaypalHelper
	{

		/// <summary>
		/// Post a Transaction to paypal
		/// </summary>
		/// <param name="card_number">The catd number</param>
		/// <param name="card_exp">Card Expiration date</param>
		/// <param name="firstname">First name on card</param>
		/// <param name="lastname">Last name on card</param>
		/// <param name="amount">The ammoun</param>
		/// <param name="type">Card type</param>
		/// <param name="currancy">The currancy symbol</param>
		/// <returns>true if it was posted false other wise</returns>
		public static bool SendTransaction(
			string card_number, 
			string card_exp, 
			string firstname, 
			string lastname,
			string amount, 
			string type,
			string currancy
			)
		{
			APIContext apiContext;
			Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
			sdkConfig.Add("mode", "sandbox");
			string accessToken = new OAuthTokenCredential("Aa3fQicf6E747E5Snc9I2XgEmGbOIyKBTkmVHDfU6BfjD6mSn-7goLwYSSrOYuUrbD6Ifb5CzQa7R_BH", "EE4BJAi8KirBrsu11CDWhKzhIxmoYxwuw0ZL7StRcSrvxC524vA3-KJndFQgDJxoyuBu-Q7byo9YiLMW", sdkConfig).GetAccessToken();
			apiContext = new APIContext(accessToken);
			apiContext.Config = sdkConfig;

			CreditCard credtCard = new CreditCard();

			switch (type)
			{
				case ("1"):
					credtCard.type = "mastercard";
					break;
				case ("2"):
					credtCard.type = "visa";
					break;
				case ("3"):
					credtCard.type = "amercan express";
					break;
				case ("4"):
					credtCard.type = "discover";
					break;
				default:
					credtCard.type = "visa";
					break;
			}


			credtCard.number = card_number;
			credtCard.expire_month = Convert.ToInt32(card_exp.Split('/')[0]);
			credtCard.expire_year =Convert.ToInt32("20"+card_exp.Split('/')[1]);
			credtCard.first_name = firstname;
			credtCard.last_name = lastname;
			string last4digits = card_number.Substring(card_number.Length - 4);


			FundingInstrument fundInstrument = new FundingInstrument();
			fundInstrument.credit_card = credtCard;

			List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
			fundingInstrumentList.Add(fundInstrument);

			Payer payr = new Payer();
			payr.funding_instruments = fundingInstrumentList;
			payr.payment_method = "credit_card";


		

			Amount amnt = new Amount();
			amnt.currency = currancy;
			var a=Math.Round(Convert.ToDecimal(amount), 2);
			amnt.total = a.ToString("0.00"); 

			Transaction tran = new Transaction();
			tran.description = "creating a direct payment with credit card";
			tran.amount = amnt;


			List<Transaction> transactions = new List<Transaction>();
			transactions.Add(tran);

			Payment pymnt = new Payment();
			pymnt.intent = "sale";
			pymnt.payer = payr;
			pymnt.transactions = transactions;


			Payment createdPayment = pymnt.Create(apiContext);


			return createdPayment.state == "approved";
		}

		


	}
}
