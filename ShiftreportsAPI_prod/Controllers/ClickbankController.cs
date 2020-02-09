using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiomedicaLib.Net;
using ExpensTrackerAPI.Controllers;
using ShiftReportApi.App_Code;
using shiftreportapp.data;
using ShiftreportLib;

namespace ShiftReportApi.Controllers
{
	public class ClickbankController : ApiController
	{
		String html_root_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_root_path"];
			}
		}


		String html_folder_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_folder_path"];
			}
		}

		String S3_path
		{
			get
			{
				return html_root_path + html_folder_path;
			}
		}



		String clickbankkey
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["clickbankkey"];
			}
		}



		[HttpGet]
		public HttpResponseMessage get_referral_checkout_information([FromUri] int corp_id)
		{
			AppModel Context = new AppModel();
			try
			{
				var corp = Context.corporate_mst2.Find(corp_id);
				var mngr = Context.managers_mst2.Find(corp.acct_admin_id);
				var r   = Context.Database.SqlQuery<String>("select  top 1 txn_id from payment_transactions_mst where corp_id="+corp_id+" order by transaction_date_time desc").FirstOrDefault();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					receipt=r,
					manager_mst = new
					{
						manager_id = mngr.Id,
						manager_pw = mngr.manager_pw
					}

				});
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage notify(clickbank_notify_req_dm data)
		{
			AppModel Context = new AppModel();
			try
			{
				//System.IO.File.WriteAllText(@"C:\\Tmp\" + data.iv + ".log", data.notification);

				var D = AES.Decrypt(data.notification, clickbankkey, data.iv);
				return Request.CreateResponse(HttpStatusCode.Accepted, "");
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}

		}
	}
}
