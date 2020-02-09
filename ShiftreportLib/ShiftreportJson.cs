using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using shiftreportapp.data;
using ShiftreportLib;

public class ShiftDetails
{

    

    private  Object[] lotto_rack_dtls { get; set; }
    private Object[] cig_rack_dtl { get; set; }
    private Object[] shift_checklist_mst { get; set; }
    private Object[] phone_cards { get; set; }
    private Object[] lotto_bin_dtls { get; set; }
    private Object[] drawer_open_mst { get; set; }
    private Object[] drawer_close_mst { get; set; }
    private Object[] cig_rackrowtray_open_mst { get; set; }
    private Object[] cig_rackrowtray_close_mst { get; set; }
    private Object[] cartons_of_cigarrets { get; set; }
    public void ParseData(int shift_id,string json,string connectionString)
    {
        StringBuilder str = new StringBuilder();
        var db = new AppModel().Database;
		

         System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
		
    dynamic data = jss.DeserializeObject(json);
    try
    {
		db.ExecuteSqlCommand("DELETE FROM shift_checklist_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cartons_of_cigarrets WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM phone_cards WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_bin_dtls WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_rack_dtls WHERE shift_id=" + shift_id.ToString());//cig_rack_dtl
		db.ExecuteSqlCommand("DELETE FROM cig_rack_dtl WHERE shift_id=" + shift_id.ToString());

		for (int i = 0; i < data.Length; i++)
		{
			Dictionary<string, Object> L1 = data[i];
			String Name = L1.Keys.FirstOrDefault();
			dynamic Body = L1.Values.FirstOrDefault();
			
			

			if (Body.Length > 0)
			{
				var D2 = Body;
				switch (Name)
				{
					case ("lotto_rack_dtls"):
						lotto_rack_dtls = D2;
						break;
					case ("cig_rack_dtl"):
						cig_rack_dtl = D2;
						break;
					case ("shift_checklist_mst"):
						shift_checklist_mst = D2;
						break;
					case ("phone_cards"):
						phone_cards = D2;
						break;
					case ("lotto_bin_dtls"):
						lotto_bin_dtls = D2;
						break;
					case ("drawer_open_mst"):
						drawer_open_mst = D2;
						break;
					case ("drawer_close_mst"):
						drawer_close_mst = D2;
						break;
					case ("cig_rackrowtray_open_mst"):
						cig_rackrowtray_open_mst = D2;
						break;
					case ("cig_rackrowtray_close_mst"):
						cig_rackrowtray_close_mst = D2;
						break;
					case ("cartons_of_cigarrets"):
						cartons_of_cigarrets = D2;
						break;
				}
			}
		}
			Object[] x = shift_checklist_mst.ToArray();
			var shift_checklist = (from Q in
									   (from Dictionary<string, Object> S in shift_checklist_mst
										select S)
								   select Q);


			foreach (var t0 in shift_checklist)
			{
					var q_uuid = t0["q_uuid"];
					var q_answer = t0["q_answer"];
					var shift_no = t0["shift_no"];
					var sql = "INSERT INTO shift_checklist_mst(shift_id,q_uuid,q_answer,shift_no) VALUES (" + shift_id.ToString() + ",'" + q_uuid + "','" + q_answer + "'," + shift_no.ToString() + ");";
				try
				{
				
					db.ExecuteSqlCommand(sql);
				}
				catch(Exception err)
				{
					throw new Exception("Error while parsing the cigaret racks at ( " + q_uuid+" )");
				}
			}



			var open_drawer = (from Q in
								   (from Dictionary<string, Object> S in drawer_open_mst
									select S)
							   select Q);
			foreach (var t1 in open_drawer)
			{

				var fifties_open = t1["fifties_open"];
				var twenties_open = t1["twenties_open"];
				var tens_open = t1["tens_open"];
				var fives_open = t1["fives_open"];
				var singles_open = t1["singles_open"];
				var dollars_open = t1["dollars_open"];
				var quarters_open = t1["quarters_open"];
				var dimes_open = t1["dimes_open"];
				var nickels_open = t1["nickels_open"];
				var rolled_quarters_open = t1["rolled_quarters_open"];
				var rolled_dimes_open = t1["rolled_dimes_open"];
				var rolled_nickels_open = t1["rolled_nickels_open"];
				var rolled_pennies_open = t1["rolled_pennies_open"];
				var pennies_open = t1["pennies_open"];

				var sql = "INSERT INTO drawer_open_mst(shift_id,fifties_open,twenties_open,tens_open,fives_open,singles_open,dollars_open,quarters_open,dimes_open,nickels_open,rolled_quarters_open,rolled_dimes_open,rolled_nickels_open,rolled_pennies_open,pennies_open) VALUES (" + shift_id.ToString() + "," + fifties_open.ToString() + "," + twenties_open.ToString() + "," + tens_open.ToString() + "," + fives_open.ToString() + "," + singles_open.ToString() + "," + dollars_open.ToString() + "," + quarters_open.ToString() + "," + dimes_open.ToString() + "," + nickels_open.ToString() + "," + rolled_quarters_open.ToString() + "," + rolled_dimes_open.ToString() + "," + rolled_nickels_open.ToString() + "," + rolled_pennies_open.ToString() + "," + pennies_open.ToString() + ");";
				try
				{
						db.ExecuteSqlCommand(sql);
				}
				catch(Exception err)
				{
					throw new Exception("Error while updating open drawers values ");
				}
				

			}

			var close_drawer = (from Q in
									(from Dictionary<string, Object> S in drawer_close_mst
									 select S)
								select Q);
			foreach (var t2 in close_drawer)
			{


				var tens_close = t2["tens_close"];
				var fives_close = t2["fives_close"];
				var singles_close = t2["singles_close"];
				var dollars_close = t2["dollars_close"];
				var quarters_close = t2["quarters_close"];
				var dimes_close = t2["dimes_close"];
				var nickels_close = t2["nickels_close"];
				var pennies_close = t2["pennies_close"];
				var rolled_quarters_close = t2["rolled_quarters_close"];
				var rolled_dimes_close = t2["rolled_dimes_close"];
				var rolled_nickels_close = t2["rolled_nickels_close"];
				var rolled_pennies_close = t2["rolled_pennies_close"];
				var cash_drawer_close = t2["cash_drawer_close"];
				var fifties_close = t2["fifties_close"];
				var twenties_close = t2["twenties_close"];
				var sql = "INSERT INTO drawer_close_mst(shift_id,tens_close,fives_close,singles_close,dollars_close,quarters_close,dimes_close,nickels_close,pennies_close,rolled_quarters_close,rolled_dimes_close,rolled_nickels_close,rolled_pennies_close,cash_drawer_close,fifties_close,twenties_close) VALUES (" + shift_id.ToString() + "," + tens_close.ToString() + "," + fives_close.ToString() + "," + singles_close.ToString() + "," + dollars_close.ToString() + "," + quarters_close.ToString() + "," + dimes_close.ToString() + "," + nickels_close.ToString() + "," + pennies_close.ToString() + "," + rolled_quarters_close.ToString() + "," + rolled_dimes_close.ToString() + "," + rolled_nickels_close.ToString() + "," + rolled_pennies_close.ToString() + "," + cash_drawer_close.ToString() + "," + fifties_close.ToString() + "," + twenties_close.ToString() + ");";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating close drawers values ");
				}
			}

			var cig_cartons = (from Q in
								   (from Dictionary<string, Object> S in cartons_of_cigarrets
									select S)
							   select Q);
			foreach (var t3 in cig_cartons)
			{

				var carton_begin = t3["carton_begin"];
				var carton_added = t3["carton_added"];
				var carton_ended = t3["carton_ended"];
				var sql = "INSERT INTO cartons_of_cigarrets(shift_id,carton_begin,carton_added,carton_ended) VALUES (" + shift_id.ToString() + "," + carton_begin.ToString() + "," + carton_added.ToString() + "," + carton_ended.ToString() + ")";

				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating cartons of cigarrets ");
				}

			}

			var phone_cards_col = (from Q in
									   (from Dictionary<string, Object> S in phone_cards
										select S)
								   select Q);
			foreach (var t4 in phone_cards_col)
			{

				var phone_begin_1 = t4["phone_begin_1"];
				var phone_begin_2 = t4["phone_begin_2"];
				var phone_begin_3 = t4["phone_begin_3"];
				var phone_begin_4 = t4["phone_begin_4"];
				var phone_added_5 = t4["phone_added_5"];
				var phone_added_6 = t4["phone_added_6"];
				var phone_added_7 = t4["phone_added_7"];
				var phone_added_8 = t4["phone_added_8"];
				var phone_ended_9 = t4["hone_ended_9"];
				var phone_ended_10 = t4["phone_ended_10"];
				var phone_ended_11 = t4["phone_ended_11"];
				var phone_ended_12 = t4["phone_ended_12"];

				var sql = "INSERT INTO phone_cards(phone_begin_1,phone_begin_2,phone_begin_3,phone_begin_4,phone_added_5,phone_added_6,phone_added_7,phone_added_8,hone_ended_9,phone_ended_10,phone_ended_11,phone_ended_12,shift_id) VALUES (" + phone_begin_1.ToString() + "," + phone_begin_2.ToString() + "," + phone_begin_3.ToString() + "," + phone_begin_4.ToString() + "," + phone_added_5.ToString() + "," + phone_added_6.ToString() + "," + phone_added_7.ToString() + "," + phone_added_8.ToString() + "," + phone_ended_9.ToString() + "," + phone_ended_10.ToString() + "," + phone_ended_11.ToString() + "," + phone_ended_12.ToString() + "," + shift_id.ToString() + ");";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating phone cards values");
				}


			}

			var cig_rackrowtray_open = (from Q in
											(from Dictionary<string, Object> S in cig_rackrowtray_open_mst
											 select S)
										select Q);
			foreach (var t5 in cig_rackrowtray_open)
			{

				var rack_no = t5["rack_no"];
				var uuid = t5["uuid"];

				var sql = "INSERT INTO cig_rackrowtray_open_mst(rack_no,uuid,shift_id) VALUES (" + rack_no.ToString() + ",'" + uuid + "'," + shift_id.ToString() + ");";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating the cigarette tray open values at uuid .");
				}

			}

			var cig_rackrowtray_close = (from Q in
											 (from Dictionary<string, Object> S in cig_rackrowtray_close_mst
											  select S)
										 select Q);
			foreach (var t6 in cig_rackrowtray_close)
			{

				var rack_no = t6["rack_no"];
				var uuid = t6["uuid"];

				var sql = "INSERT INTO cig_rackrowtray_close_mst(rack_no,uuid,shift_id) VALUES (" + rack_no.ToString() + ",'" + uuid + "'," + shift_id.ToString() + ");";

				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating the cigarette tray close values at uuid .");
				}

			}

			var lotto_bin = (from Q in
								 (from Dictionary<string, Object> S in lotto_bin_dtls
								  select S)
							 select Q);

			foreach (var t7 in lotto_bin)
			{
				
					var _id = t7["_id"];
					var open_close_flag = t7["open_close_flag"];
					var uuid = t7["uuid"];
					var bin_amnt = t7["bin_amnt"];

					var sql = "INSERT INTO lotto_bin_dtls (shift_id,open_close_flag,uuid,bin_amnt) VALUES ( " + shift_id + ",'" + open_close_flag + "','" + uuid + "'," + bin_amnt + " );";
					try
					{
						db.ExecuteSqlCommand(sql);
					}
					catch (Exception err)
					{
						if(open_close_flag=="o")
						{
							throw new Exception("Error while updating the lotto open tray  values at uuid ( " + uuid + " )");
						}
						else
						{
							throw new Exception("Error while updating the lotto close tray  values at uuid ( " + uuid + " )");
						}
						
					}
				

			}


			var lotto_rack = (from Q in
								  (from Dictionary<string, Object> S in lotto_rack_dtls
								   select S)
							  select Q);
			foreach (var t8 in lotto_rack)
			{

				var rack_no = t8["rack_no"];
				var rack_row = t8["rack_row"];
				var rack_col = t8["rack_col"];
				var open_amnt = t8["open_amnt"];
				var added_amnt = t8["added_amnt"];
				var close_amnt = t8["close_amnt"];
				var sql = "INSERT INTO lotto_rack_dtls(shift_id,rack_no,rack_row,rack_col,open_amnt,added_amnt,close_amnt) VALUES (" + shift_id.ToString() + "," + rack_no.ToString() + "," + rack_row.ToString() + "," + rack_col.ToString() + "," + open_amnt + "," + added_amnt + "," + close_amnt + ");";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{

					throw new Exception("Error while updating the lotto open bins  values at rack no=" + rack_no + " and row_no=" + rack_row + " and col_no=" + rack_col + " ");
					

				}


			}

			var cig_rack = (from Q in
								(from Dictionary<string, Object> S in cig_rack_dtl
								 select S)
							select Q);
			foreach (var t9 in cig_rack)
			{
				var rack_no = t9["rack_no"];
				var row_no = t9["row_no"];
				var tray_no = t9["tray_no"];
				var open_value = Convert.ToDouble(t9["open_value"]);
				var close_value = Convert.ToDouble(t9["close_value"]);
				var sql = "INSERT INTO cig_rack_dtl(shift_id,rack_no,row_no,tray_no,open_value,close_value) VALUES (" + shift_id.ToString() + "," + rack_no.ToString() + "," + row_no.ToString() + "," + tray_no.ToString() + "," + open_value.ToString() + "," + close_value.ToString() + ")";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{

					throw new Exception("Error while updating the lotto open bins  values at rack no=" + rack_no + " and row_no=" + row_no + " and col_no=" + tray_no + " ");


				}

			
		}
	

    }
    catch(Exception err)
    {
		db.ExecuteSqlCommand("DELETE FROM shift_checklist_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cartons_of_cigarrets WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM phone_cards WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_bin_dtls WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_rack_dtls WHERE shift_id=" + shift_id.ToString());//cig_rack_dtl
		db.ExecuteSqlCommand("DELETE FROM cig_rack_dtl WHERE shift_id=" + shift_id.ToString());

		throw new AppErrorException (98,err.Message+" , Please contact your manager for assistance .");  
    }
  
    }
	public void ParseData_cash_app(int shift_id, string json, string connectionString)
	{
		StringBuilder str = new StringBuilder();
		var db = new AppModel().Database;
		System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
		dynamic data = jss.DeserializeObject(json);
		
		db.ExecuteSqlCommand("DELETE FROM shift_checklist_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM drawer_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cartons_of_cigarrets WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM phone_cards WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_open_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM cig_rackrowtray_close_mst WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_bin_dtls WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM lotto_rack_dtls WHERE shift_id=" + shift_id.ToString());//cig_rack_dtl
		db.ExecuteSqlCommand("DELETE FROM cig_rack_dtl WHERE shift_id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM  section_elem_ans_mst WHERE Id=" + shift_id.ToString());
		db.ExecuteSqlCommand("DELETE FROM  rack_ans_mst WHERE Id=" + shift_id.ToString());

		var main=data[0];
		var shift_info = (main["shift_info"])[0];
		var drawer_open_mst = (main["drawer_open_mst"])[0];
		var drawer_close_mst = (main["drawer_close_mst"])[0];
		var safe_drops = main["safe_drops"];
		var mop_sales = main["mop_sales"];
		var shift_checklist_mst = main["shift_checklist_mst"];
		var shift_details_mst = main["shift_details_mst"];
		var rackset = main["rack_set"]; // The collection of all the rack sets
		
		var store_id = db.SqlQuery<int>("Select store_id from shift_details_mst where id=" + shift_id).FirstOrDefault();

		var sql = "INSERT INTO drawer_open_mst(shift_id,fifties_open,twenties_open,tens_open,fives_open,singles_open,dollars_open,quarters_open,dimes_open,nickels_open,pennies_open,rolled_quarters_open,rolled_dimes_open,rolled_nickels_open,rolled_pennies_open) VALUES (" + shift_id.ToString() + "," + drawer_open_mst["fifties"].ToString() + "," + drawer_open_mst["twenties"].ToString() + "," + drawer_open_mst["tens"].ToString() + "," + drawer_open_mst["fives"].ToString() + "," + drawer_open_mst["singles"].ToString() + "," + drawer_open_mst["dollars"].ToString() + "," + drawer_open_mst["quarters"].ToString() + "," + drawer_open_mst["dimes"].ToString() + "," + drawer_open_mst["nickels"].ToString() + "," + drawer_open_mst["pennies"].ToString() + "," + drawer_open_mst["rolled_quarters"].ToString() + "," + drawer_open_mst["rolled_dimes"].ToString() + "," + drawer_open_mst["rolled_nickels"].ToString() + "," + drawer_open_mst["rolled_pennies"].ToString() + ");";
		try
		{
			db.ExecuteSqlCommand(sql);
		}
		catch (Exception err)
		{
			throw new Exception("Error while updating open drawers values ");
		}
		 sql = "INSERT INTO drawer_close_mst(shift_id,fifties_close,twenties_close,tens_close,fives_close,singles_close,dollars_close,quarters_close,dimes_close,nickels_close,pennies_close,rolled_quarters_close,rolled_dimes_close,rolled_nickels_close,rolled_pennies_close) VALUES (" + shift_id.ToString() + "," + drawer_close_mst["fifties"].ToString() + "," + drawer_close_mst["twenties"].ToString() + "," + drawer_close_mst["tens"].ToString() + "," + drawer_close_mst["fives"].ToString() + "," + drawer_close_mst["singles"].ToString() + "," + drawer_close_mst["dollars"].ToString() + "," + drawer_close_mst["quarters"].ToString() + "," + drawer_close_mst["dimes"].ToString() + "," + drawer_close_mst["nickels"].ToString() + "," + drawer_close_mst["pennies"].ToString() + "," + drawer_close_mst["rolled_quarters"].ToString() + "," + drawer_close_mst["rolled_dimes"].ToString() + "," + drawer_close_mst["rolled_nickels"].ToString() + "," + drawer_close_mst["rolled_pennies"].ToString() + ");";
		 try
		 {
			 db.ExecuteSqlCommand(sql);
		 }
		 catch (Exception err)
		 {
			 throw new Exception("Error while updating close drawers values ");
		 }

		 for (int i = 0; i < ((object[])safe_drops).Length; i++)
		 {
			 object sd = ((object[])safe_drops)[i];
			 var dat = ((Dictionary<String, Object>)sd);
			 sql = "INSERT INTO safe_drops_mst(shift_id,safedrop_num,safedrop_amnt) VALUES("+shift_id+","+dat["_id"]+","+dat["value"]+")  ";
			 try
			 {
				 db.ExecuteSqlCommand(sql);
			 }
			 catch (Exception err)
			 {
				 throw new Exception("Error while updating safe drops values ");
			 }
		 }
		 Double reg1_debit = 0.0, reg2_debit = 0.0, all_debit = 0.0, reg1_credit = 0.0, reg2_credit = 0.0, all_credit = 0.0, reg1_cash = 0.0, reg2_cash = 0.0, all_cash = 0.0;
		 for (int i = 0; i < ((object[])mop_sales).Length; i++)
		 {
			 object sd = ((object[])mop_sales)[i];
			 var dat = ((Dictionary<String, Object>)sd);
			
			 switch (dat["label"].ToString())
			 {
				 case ("REGISTER 1"):
					 reg1_debit = Convert.ToDouble(dat["debit"]);
					 reg1_credit = Convert.ToDouble(dat["credit"]);
					 reg1_cash = Convert.ToDouble(dat["cash"]);
					 break;
				 case ("REGISTER 2"):
					  reg2_debit = Convert.ToDouble(dat["debit"]);
					 reg2_credit = Convert.ToDouble(dat["credit"]);
					 reg2_cash = Convert.ToDouble(dat["cash"]);
					 break;
				 case ("ALL REGISTERS"):
					  all_debit = Convert.ToDouble(dat["debit"]);
					 all_credit = Convert.ToDouble(dat["credit"]);
					 all_cash = Convert.ToDouble(dat["cash"]);
					 break;
			 }
		 }
		 sql = "Update a Set a.credit_sales_reg1=" + reg1_credit + ",a.credit_sales_reg2=" + reg2_credit + ",a.credit_sales_all_registers=" + all_credit + ",a.debit_sales_reg1=" + reg1_debit + ",a.debit_sales_reg2=" + reg2_debit + ",a.debit_sales_all_registers=" + all_debit + ",a.cash_sales_reg1=" + reg1_cash + ",a.cash_sales_reg2=" + reg2_cash + ",a.cash_sales_all_registers=" + all_cash + ",shift_open_time=" + shift_info["shift_opened"] + ",a.shift_close_time=" + shift_info["shift_closed"] + " from shift_details_mst a where id=" + shift_id;
		 try
		 {
			 db.ExecuteSqlCommand(sql);
		 }
		 catch (Exception err)
		 {
			 throw new Exception("Error while updating close mob values ");
		 }


		 for (int i = 0; i < ((object[])shift_checklist_mst).Length; i++)
		 {
			 object sd = ((object[])shift_checklist_mst)[i];
			 var dat = ((Dictionary<String, Object>)sd);

			 var q_id = dat["q_id"];
			 var q_uuid = dat["q_uuid"];
			 var shift_no = shift_info["shift_no"];
			 if (q_id == null)
				 q_id = 0;
			 var q_answer = dat["q_answer"];
			// sql = "Update a set a.q_answer='" + q_answer + "' from shift_checklist_store_mst a where id=" + q_id;
			  sql = "INSERT INTO shift_checklist_mst(shift_id,q_uuid,q_answer,shift_no,question_id,store_id) VALUES (" + shift_id.ToString() + ",'" + q_uuid + "','" + q_answer + "'," + shift_no.ToString() +","+q_id+ ","+ store_id +");";
			 try
			 {
				 db.ExecuteSqlCommand(sql);
			 }
			 catch (Exception err)
			 {
				 throw new Exception("Error while updating close checklist values ");
			 }

			
			var shift_session_ended = Convert.ToInt32(shift_details_mst["shift_session_ended"]);
			
			sql = "Update a Set a.shift_session_ended='"+ shift_session_ended + "' from shift_details_mst a where id=" + shift_id;

			try
			{
				db.ExecuteSqlCommand(sql);
			}
			catch (Exception err)
			{
				throw new Exception("Error while updating session date and time ");
			}

			

		}

		#region parse rack section elemnts

		for (int i = 0; i < ((object[])rackset).Length; i++)
		{
			object rs = ((object[])rackset)[i];
			var dat = ((Dictionary<String, Object>)rs);
			var id = dat["rackset_id"];
			var sec_elem = dat["racks_vals"];
			var racks = dat["shift_racks"];

			#region section elem
			for (int k = 0; k < ((object[])sec_elem).Length; k++)
			{
				object se = ((object[])sec_elem)[k];
				var dat_sec_elem = ((Dictionary<String, Object>)se);

				var elem_id = dat_sec_elem["elem_id"];
				var elem_uuid = dat_sec_elem["elem_uuid"];
				var val = dat_sec_elem["value"];

				sql = "INSERT INTO section_elem_ans_mst (elem_id,elem_uuid,elem_val,shift_id) VALUES (" + elem_id + ",'" + elem_uuid + "','" + val + "'," + shift_id + ") ";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating rackset section elmnst ");
				}
			}
			#endregion

			for (int k = 0; k < ((object[])racks).Length; k++)
			{
				object rk = ((object[])racks)[k];
				var dat_racks = ((Dictionary<String, Object>)rk);
				var rack_no = dat_racks["rack_no"];
				var row_no = dat_racks["row_no"];
				var col_no = dat_racks["col_no"];
				var started = dat_racks["started_val"];
				var added = dat_racks["added_val"];
				var ended = dat_racks["ended_val"];

				sql = "INSERT INTO rack_ans_mst (shift_id,rack_started_value,rack_added_value,rack_ended_value,rackset_id,rack_no,row_no,col_no) VALUES (" + shift_id + ",'" + started + "','" + added + "','" + ended + "'," + id + "," + rack_no + "," + row_no + "," + col_no + ") ";
				try
				{
					db.ExecuteSqlCommand(sql);
				}
				catch (Exception err)
				{
					throw new Exception("Error while updating rackset section elmnst ");
				}
			}
		}



		#endregion


	}

}
public class TimeHelpers
{
    public static string MilitaryToDateTime(int Military)
    {
        int Hours = Military / 100;
        int Minutes = Military - Hours * 100;

        if (Hours < 12)
            return Hours.ToString() + "AM";
        else
            return (Hours - 12).ToString() + "PM";


    }
}
