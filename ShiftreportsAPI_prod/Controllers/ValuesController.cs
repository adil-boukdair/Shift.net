using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiomedicaLib.Net;

namespace ExpensTrackerAPI.Controllers
{
	public class ValuesController : ApiController
	{

		[HttpPost]
		public HttpResponseMessage forgot_cashier_password([FromBody]  EmailSendDM d)
		{
			try
			{
				MailLogService.SendMail("support@shiftreports.com", d.email, d.subject, d.body);
				return Request.CreateResponse(HttpStatusCode.OK, new { success=1});
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
	public class EmailSendDM
	{
		public String email { set; get; }
		public String body { set; get; }
		public String subject { set; get; }
	}
}
