using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiomedicaLib.Net;
using ExpensTrackerAPI.Controllers;
using shiftreportapp.data;
using ShiftreportLib;
using static shiftreportapp.data.AppModel;

namespace ShiftReportApi.Controllers
{
	public class SchedulerController : ApiController
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


		[HttpGet]
		public HttpResponseMessage get_scheduler([FromUri] int store_id, [FromUri] DateTime date_range_start, [FromUri] DateTime date_range_end)
		{
			AppModel Context = new AppModel();
			try
			{
				var cashiers = Context.Database.SqlQuery<cashier_mst_get_sheduler>("Select id as cashier_id,cashier_name from  cashier_mst Where store_id ="+ store_id).ToList();

				var store_times = Context.Database.SqlQuery<shift_store_times>("select shift_no,start_time,end_time from shift_store_times where store_id="+store_id).ToList();

				var data = Context.Database.SqlQuery<scheduler_ms_get_scheduler>("select a.id,a.cashier_id,c.cashier_name,a.shift_no,a.status,a.assignment_date,b.reason as decline_reson from scheduler_mst a left outer  join scheduler_decline_reasons_dtls b on a.decline_reason=b.id inner join cashier_mst c on a.cashier_id=c.id where a.store_id=" + store_id + " and assignment_date between '"+ date_range_start + "' and '"+ date_range_end + "' ").ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new { cashier_mst= cashiers,
																	shift_store_times= store_times,
																	scheduler_mst = data });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpPost]
		public HttpResponseMessage assigne_shift(assigne_shift_dmt_list data)
		{
			try
			{
				AppModel Context = new AppModel();
				List<assign_shift_response_dm> dl = new List<assign_shift_response_dm>();
				for (int i = 0; i < data.scheduler_mst.Count; i++)
				{
					var sh = new scheduler_mst();
					sh.store_id = data.scheduler_mst[i].store_id;
					sh.cashier_id = data.scheduler_mst[i].cashier_id;
					sh.shift_no = data.scheduler_mst[i].shift_no;
					sh.status = data.scheduler_mst[i].status;
					sh.assignment_date = data.scheduler_mst[i].assignment_date;
					sh.decline_reason = data.scheduler_mst[i].decline_reason;
					Context.scheduler_mst2.Add(sh);
					Context.SaveChanges();
					dl.Add(new assign_shift_response_dm() { id = sh.id, cashier_id = data.scheduler_mst[i].cashier_id });
				}
				
				return Request.CreateResponse(HttpStatusCode.OK, new { scheduler_mst = dl });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage delete_assigned_shift(delete_assigned_shift_req_dm data)
		{
			AppModel Context = new AppModel();
			try
			{
				for(int i=0; i < data.ids.Count; i++)
				{
					int id = data.ids[i];
				   Context.Database.ExecuteSqlCommand("Delete from scheduler_mst where id="+id);
				}
				
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_scheduler_by_cashier([FromUri] int cashier_id, [FromUri] DateTime date_range_start, [FromUri] DateTime date_range_end)
		{
			AppModel Context = new AppModel();
			try
			{
				var cashiers = Context.Database.SqlQuery<cashier_mst_get_sheduler>("Select id as cashier_id,cashier_name from  cashier_mst Where id =" + cashier_id.ToString()).ToList();

				var storeid = Context.cashier_mst2.Find(cashier_id).store_id;

				var store_times = Context.Database.SqlQuery<shift_store_times>("select shift_no,start_time,end_time from shift_store_times where store_id=" + storeid).ToList();

				var data = Context.Database.SqlQuery<scheduler_ms_get_scheduler>("select a.id,a.cashier_id,c.cashier_name,a.shift_no,a.status,a.assignment_date,a.decline_reason  from scheduler_mst a inner join cashier_mst c on a.cashier_id=c.id where a.cashier_id=" + cashier_id + " and assignment_date between '" + date_range_start + "' and '" + date_range_end + "' ").ToList();
				var schedulerdeclinereasonsdtls = Context.scheduler_decline_reasons_dtls2.ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					cashier_mst = cashiers,
					shift_store_times = store_times,
					scheduler_decline_reasons_dtls= schedulerdeclinereasonsdtls,
					scheduler_mst = data
				});
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpPost]
		public HttpResponseMessage modify_schedule(modify_schediualer_post_dm data)
		{
			AppModel Context = new AppModel();
			try
			{
				var sh = Context.scheduler_mst2.Find(data.id);
				sh.status = data.status;
				sh.decline_reason = data.decline_reason;
				Context.Entry(sh).State = EntityState.Modified;
				Context.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}




	}
}
