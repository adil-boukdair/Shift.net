using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiomedicaLib.Net;
using ExpensTrackerAPI.App_Code;
using ExpensTrackerAPI.Controllers;
using shiftreportapp.data;
using ShiftreportLib;
using ShiftreportLib.Helpers;

namespace ShiftReportApi.Controllers
{
	public class RacksController : ApiController
	{

		String ConnectionString
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];
			}
		}
		String NotificationEmail
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["NotificationEmail"];
			}
		}
		String PortalUrl
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["PortalUrl"];
			}
		}
		String ReportPath
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["ReportsPath"];
			}
		}
		String CashReportPath
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["CashReportsPath"];
			}
		}
		AppModel Context = new AppModel();
		#region Helpers
		DB_rackset_mst p = new DB_rackset_mst();
		DB_section_mst sec = new DB_section_mst();
		DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
		DB_rack_mst ra = new DB_rack_mst();
		DB_rack_names_mst ns = new DB_rack_names_mst();
		DB_rack_template_mst racktmp = new DB_rack_template_mst();
		DB_rack_ans_mst ram = new DB_rack_ans_mst();
		DB_section_elem_ans_mst ram_sec = new DB_section_elem_ans_mst();
		DB_rack_inventory_level_mst inv = new DB_rack_inventory_level_mst();
		#endregion

		[HttpGet]
		public HttpResponseMessage get_shift_rack_details([FromUri] int shift_id)
		{
			try
			{


				var store_id = Convert.ToInt32(Context.shift_details_mst2.Find(shift_id).store_id);
				var data = AppHelpers.GetracksetsCollection(store_id);
				
				for (int i = 0; i < data.store_racks.Count; i++)
				{
					
					for (int k = 0; k < data.store_racks[i].rack.Count; k++)
					{

						var shift_data= ram.Getrack_ans_mstByShiftId(shift_id, data.store_racks[i].rack[k].rack_no);
						data.store_racks[i].rack[k].cell = new List<AppModel.rack_ans_mst_resp>();
						for (int kw=0;kw< shift_data.Count; kw++)
						{
							var c = (from l in data.store_racks[i].rack[k].product_label
									 where l.rack_no == shift_data[kw].rack_no-1 &&
											l.row_no== shift_data[kw].row_no &&
											l.col_no == shift_data[kw].col_no
									 select l.product_name).FirstOrDefault();
							if (c == null)
								c = "";
							data.store_racks[i].rack[k].cell.Add(new AppModel.rack_ans_mst_resp()
							{
								shift_id = shift_id,
								row_no = shift_data[kw].row_no,
								col_no = shift_data[kw].col_no,
								added_value = shift_data[kw].rack_added_value,
								started_value = shift_data[kw].rack_started_value,
								ended_value = shift_data[kw].rack_ended_value,
								product_label = c
							});
						}
						
					//data.store_racks[i].rack[k].product_label = null;

					}
					
					var racH = data.store_racks[i].header.Sections;
					for (int x = 0; x < racH.Count; x++)
					{
						var elem = data.store_racks[i].header.Sections[x].elemnts;
						for (int y = 0; y < elem.Count; y++)
						{
							data.store_racks[i].header.Sections[x].elemnts[y].val = ram_sec.Getsection_elem_ans_mst(shift_id, data.store_racks[i].header.Sections[x].elemnts[y].id);
						}
					}

					var racF = data.store_racks[i].footer.Sections;
					for (int x = 0; x < racF.Count; x++)
					{
						var elem = data.store_racks[i].footer.Sections[x].elemnts;
						for (int y = 0; y < elem.Count; y++)
						{
							data.store_racks[i].footer.Sections[x].elemnts[y].val = ram_sec.Getsection_elem_ans_mst(shift_id, data.store_racks[i].footer.Sections[x].elemnts[y].id);
						}
					}

					
				}
				return Request.CreateResponse(HttpStatusCode.OK, data);

			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}



		[HttpGet]
		public HttpResponseMessage get_store_racks_profile([FromUri] int store_id)
		{
			try
			{
				var data = AppHelpers.GetracksetsCollection(store_id);
				var x = AppHelpers.GetracksetsCollectionAll();
				return Request.CreateResponse(HttpStatusCode.OK, new { store_racks = data.store_racks, racks_template = x.store_racks } );
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}

		}


		[HttpPost]
		public HttpResponseMessage update_store_racks_profile(store_racks_array_update data)
		{
			try
			{

				AppHelpers.UpdateRackStoreCollection(data);
				return Request.CreateResponse(HttpStatusCode.OK, new {success=1 } );
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);


				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_wizard_racks()
		{
			try
			{
				var x = AppHelpers.GetracksetsCollection(0);
				return Request.CreateResponse(HttpStatusCode.OK, new {
					racks_template = x.store_racks
				});
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err.Message);


				return Request.CreateResponse(HttpStatusCode.InternalServerError, err);
			}

		}


		[HttpPost]
		public HttpResponseMessage save_wizard_racks(store_racks_array_update data)
		{
			try
			{
				AppHelpers.UpdateRackStoreCollection(data);
				return Request.CreateResponse(HttpStatusCode.OK, new { success="1"});
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		public HttpResponseMessage get_store_report_rack_summary([FromUri] int shift_id)
		{
			try
			{


				var store_id = Convert.ToInt32(Context.shift_details_mst2.Find(shift_id).store_id);
				var data = AppHelpers.GetracksetsCollection(store_id);
				for (int i = 0; i < data.store_racks.Count; i++)
				{
					var racS = data.store_racks[i].rack;
					for (int k = 0; k < racS.Count; k++)
					{
						var vals = ram.Getrack_ans_mst(data.store_racks[i].id, racS[k].rack_no, racS[k].rack_col, racS[k].rack_col, shift_id);
						if (vals != null)
						{
							data.store_racks[i].rack[k].added_val = vals.rack_added_value;
							data.store_racks[i].rack[k].started_val = vals.rack_started_value;
							data.store_racks[i].rack[k].ended_val = vals.rack_ended_value;
						}
						else
						{
							data.store_racks[i].rack[k].added_val = "";
							data.store_racks[i].rack[k].started_val = "";
							data.store_racks[i].rack[k].ended_val = "";
						}

					}
					var racH = data.store_racks[i].header.Sections;
					for (int x = 0; x < racH.Count; x++)
					{
						var elem = data.store_racks[i].header.Sections[x].elemnts;
						for (int y = 0; y < elem.Count; y++)
						{
							data.store_racks[i].header.Sections[x].elemnts[y].val = ram_sec.Getsection_elem_ans_mst(shift_id, data.store_racks[i].header.Sections[x].elemnts[y].id);
						}
					}

					var racF = data.store_racks[i].footer.Sections;
					for (int x = 0; x < racF.Count; x++)
					{
						var elem = data.store_racks[i].footer.Sections[x].elemnts;
						for (int y = 0; y < elem.Count; y++)
						{
							data.store_racks[i].footer.Sections[x].elemnts[y].val = ram_sec.Getsection_elem_ans_mst(shift_id, data.store_racks[i].footer.Sections[x].elemnts[y].id);
						}
					}
				}
				return Request.CreateResponse(HttpStatusCode.OK, data);

			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage update_rack_inventory_levels(add_new_rack_store_collection_dm data)
		{
			try
			{
				#region Add
				for(int i=0;i<data.store_racks.Count;i++)
				{
					inv.Saverack_inventory_level_mst(data.store_racks[i]);
					
				}
				#endregion
				return Request.CreateResponse(HttpStatusCode.OK, new { success=1 });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_rack_sets_canvas([FromUri] int cashier_id)
		{
			try
			{
				var store_id = Context.cashier_mst2.Find(cashier_id).store_id;
				var d = AppHelpers.GetracksetsCollection(store_id);
				

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					rackse = d
				});
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}

		}

		[HttpGet]
		public HttpResponseMessage get_rack_sets([FromUri]int store_id)
		{
			try
			{
				
				var d= (from x in Context.rackset_mst2
						where x.store_id == store_id
						select new {
							id=x.id,
							title=x.rack_title
						}).ToArray();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					rackse=d
				});
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_rack_inventory_levels([FromUri]int rackset_id,[FromUri] int store_id)
		{
			try
			{

				var d = AppHelpers.GetracksetsCollection(store_id);
				get_rack_inventory_levels_get__resp resp = new get_rack_inventory_levels_get__resp();
				resp.store_racks = new List<get_rack_inventory_levels_get_resp_items_racks>();
				for(int i = 0; i < d.store_racks.Count; i++)
				{
					List<get_rack_inventory_levels_get_resp_items_racks_item> x2 = new List<get_rack_inventory_levels_get_resp_items_racks_item>();

					for (int j = 0; j < d.store_racks[i].rack.Count; j++)
					{
						get_rack_inventory_levels_get_resp_items_racks_item x3 = new get_rack_inventory_levels_get_resp_items_racks_item();
						x3.rack_name = d.store_racks[i].rack[j].rack_name;
						x3.rack_id = d.store_racks[i].rack[j].id;
						x3.rack_no = d.store_racks[i].rack[j].rack_no;
						x3.rack_col= d.store_racks[i].rack[j].rack_col;
						x3.rack_rows = d.store_racks[i].rack[j].rack_row;
						x3.show_product_label = d.store_racks[i].rack[j].show_product_label;
						x3.cell = new List<get_rack_inventory_levels_get_resp_items_rack_cell>();

						for(int kx = 0; kx < d.store_racks[i].rack[j].product_label.Length; kx++)
						{

							var INV = inv.Getrack_inventory_level_mst(x3.rack_id, d.store_racks[i].rack[j].rack_no, d.store_racks[i].rack[j].product_label[kx].row_no, d.store_racks[i].rack[j].product_label[kx].col_no);


							String invamnt ="";
							if (INV != null)
								invamnt = INV.inventory_level_amt;
							x3.cell.Add(new get_rack_inventory_levels_get_resp_items_rack_cell()
							{
								inventory_level_amt = invamnt,
								rack_no = x3.rack_no,
								row_no = d.store_racks[i].rack[j].product_label[kx].row_no,
								col_no = d.store_racks[i].rack[j].product_label[kx].col_no,
								product_name = d.store_racks[i].rack[j].product_label[kx].product_name
							});
						}

						

							
						
						x2.Add(x3);
					}
					resp.store_racks.Add(new get_rack_inventory_levels_get_resp_items_racks()
					{
						id = d.store_racks[i].id,
						title = d.store_racks[i].title,
						rack = x2

					});
				}
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_racks = resp.store_racks
				});
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.OK, ex);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", err);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}


	}
}
