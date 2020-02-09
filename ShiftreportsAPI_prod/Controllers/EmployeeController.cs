using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using ExpensTrackerAPI.App_Code;
using System.Web.Http;
using System.Web.Http.Cors;
using PayPal.Api;
using System.IO;
using BiomedicaLib.Net;
using System.Data.Entity;
using shiftreportapp.data;
using ExpensTrackerAPI.Controllers;
using ShiftreportLib;

namespace ShiftReportApi.Controllers
{
	public class EmployeeController : ApiController
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


		AppModel Context = new AppModel();

		[HttpGet]
		public HttpResponseMessage get_corporate_employee_hours([FromUri] int corp_id)
		{
			try
			{
				var D = Context.corporate_employee_hours_report_mst2.Where(r => r.corp_id == corp_id).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, D);
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}


		[HttpPost]
		public HttpResponseMessage update_shiftdetails(update_shift_details_post_req data)
		{
			try
			{
				int shiftid = data.shift_id;
				var sh = Context.shift_details_mst2.Find(shiftid);
				if(sh==null)
				{
					throw new AppException(data.shift_id,"Now Shift Found");
				}

				// Shift Master Table
				sh.shift_open_time = data.update_shift_shift.shift_open_time;
				sh.shift_close_time = data.update_shift_shift.shift_close_time;
				sh.cash_counted = data.update_shift_shift.cash_counted;
				sh.cash_sales_all_registers = data.update_shift_shift.cash_sales_all_registers;
				sh.credit_sales_all_registers = data.update_shift_shift.credit_sales_all_registers;
				sh.debit_sales_all_registers = data.update_shift_shift.debit_sales_all_registers;
				sh.cash_sales_reg1 = data.update_shift_shift.cash_sales_reg1;
				sh.credit_sales_reg1 = data.update_shift_shift.credit_sales_reg1;
				sh.debit_sales_reg1 = data.update_shift_shift.debit_sales_reg1;
				sh.cash_sales_reg2 = data.update_shift_shift.cash_sales_reg2;
				sh.credit_sales_reg2 = data.update_shift_shift.credit_sales_reg2;
				sh.debit_sales_reg2 = data.update_shift_shift.debit_sales_reg2;
				Context.Entry(sh).State = EntityState.Modified;

				// Open Drower
				
				var od = Context.drawer_open_mst2.Where(r => r.shift_id == shiftid).FirstOrDefault();
				od.fifties_open = data.drawer_open_mst.fifties_open;
				od.twenties_open = data.drawer_open_mst.twenties_open;
				od.tens_open = data.drawer_open_mst.tens_open;
				od.fives_open = data.drawer_open_mst.fives_open;
				od.singles_open = data.drawer_open_mst.singles_open;
				od.dollars_open = data.drawer_open_mst.dollars_open;
				od.quarters_open = data.drawer_open_mst.quarters_open;
				od.dimes_open = data.drawer_open_mst.dimes_open;
				od.nickels_open = data.drawer_open_mst.nickels_open;
				od.rolled_quarters_open = data.drawer_open_mst.rolled_quarters_open;
				od.rolled_dimes_open = data.drawer_open_mst.rolled_dimes_open;
				od.rolled_nickels_open = data.drawer_open_mst.rolled_nickels_open;
				od.rolled_pennies_open = data.drawer_open_mst.rolled_pennies_open;
				od.pennies_open = data.drawer_open_mst.pennies_open;
				od.cash_drawer_open=data.drawer_open_mst.cash_drawer_open;
				Context.Entry(od).State = EntityState.Modified;

				var oc = Context.drawer_close_mst2.Where(r => r.shift_id == shiftid).FirstOrDefault();
				oc.fifties_close = data.drawer_close_mst.fifties_close;
				oc.twenties_close = data.drawer_close_mst.twenties_close;
				oc.tens_close = data.drawer_close_mst.tens_close;
				oc.fives_close = data.drawer_close_mst.fives_close;
				oc.singles_close = data.drawer_close_mst.singles_close;
				oc.dollars_close = data.drawer_close_mst.dollars_close;
				oc.quarters_close = data.drawer_close_mst.quarters_close;
				oc.dimes_close = data.drawer_close_mst.dimes_close;
				oc.nickels_close = data.drawer_close_mst.nickels_close;
				oc.rolled_quarters_close = data.drawer_close_mst.rolled_quarters_close;
				oc.rolled_dimes_close = data.drawer_close_mst.rolled_dimes_close;
				oc.rolled_nickels_close = data.drawer_close_mst.rolled_nickels_close;
				oc.rolled_pennies_close = data.drawer_close_mst.rolled_pennies_close;
				oc.pennies_close = data.drawer_close_mst.pennies_close;
				oc.cash_drawer_close = data.drawer_close_mst.cash_drawer_close;
				Context.Entry(oc).State = EntityState.Modified;

				for(int i = 0; i < data.safe_drops_mst.Count; i++)
				{
					var sd = (from m in Context.safe_drops_mst2
							 where m.shift_id == shiftid && m.safedrop_num == data.safe_drops_mst[i].safedrop_num
							 select m).FirstOrDefault();
					sd.safedrop_amnt = data.safe_drops_mst[i].safedrop_amnt;

					Context.Entry(sd).State = EntityState.Modified;
				}
				for(int i = 0; i < data.shift_racks.Count; i++)
				{
					var r = (from m in Context.rack_ans_mst2
							 where m.shift_id==shiftid && 
								 m.rackset_id==data.shift_racks[i].rackset_id &&
								 m.rack_no==data.shift_racks[i].rack_no &&
								 m.row_no==data.shift_racks[i].row_no &&
								 m.col_no==data.shift_racks[i].col_no
							select m).FirstOrDefault();
					if(r!=null)
					{
						r.rack_added_value = data.shift_racks[i].added_value.ToString();
						r.rack_started_value = data.shift_racks[i].started_value.ToString();
						r.rack_ended_value = data.shift_racks[i].ended_value.ToString();
						Context.Entry(r).State = EntityState.Modified;
					}
					
				}
				for(int i=0;i<data.section_elem_ans_mst.Count;i++)
				{
					var e = (from m in Context.section_elem_ans_mst2
							 where m.shift_id==shiftid && m.elem_uuid==data.section_elem_ans_mst[i].elem_uuid
							 select m).FirstOrDefault();
					if(e!=null)
					{
						e.elem_val = data.section_elem_ans_mst[i].elem_val;
						Context.Entry(e).State = EntityState.Modified;
					}

				} 
				Context.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}


		[HttpPost]
		public HttpResponseMessage update_free_plan(update_free_plan_post_req data)
		{
			try
			{

				var c = Context.corporate_mst2.Find(data.corp_id);
				c.plan_id = data.plan_type;
				Context.Entry(c).State = EntityState.Modified;
				Context.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}



	}
}
