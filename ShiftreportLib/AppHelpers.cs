using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

using shiftreportapp.data;
using ShiftreportLib;

namespace ShiftreportLib.Helpers
{
    public class AppHelpers
    { 
        public static String GenerateRacknames(int tray,int rows,int rackno,string prefix){
            string[] trayletters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l","m","n" };
            StringBuilder b = new StringBuilder();
            for (int j = 0; j < rows;j++)
            {
                for (int i = 0; i < tray; i++)
                {
                    var name = "rack" + rackno + "_" + prefix + "_" + trayletters[i].ToString() + (j+1).ToString();
                    Console.WriteLine(name);
                    b.Append(","+name);
                    
                }
            }
            b.Remove(0, 1);    
            return b.ToString();
         }
        public static String GenerateBinnames(int totalbins, string prefix)
        {
            
            StringBuilder b = new StringBuilder();

            for (int i = 1; i < totalbins; i++)
                {
                    var name = "bin" + i.ToString() + "_" + prefix;
                    b.Append("," + name);

                }
          
            b.Remove(0, 1);
            return b.ToString();
        }
        public static BusinessInfoDM CreateBuismesinfo(int store_id)
        {
            AppModel Context = new AppModel();
            var store = Context.store_profile_mst2.Find(store_id);
            var cigrack = Context.store_cig_rack_dtl2.Where(r => r.store_id==store_id).ToArray();
            var lottorack = Context.store_lotto_rack_dtl2.Where(r => r.store_id == store_id).ToArray();
            if (store == null)
            {
                throw new AppException("No Shift found for this cashier", "-1");
            }
            else
            {

            }

            string shift1_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_1_time_begins));
            string shift1_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_1_time_ends));
            string shift2_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_2_time_begins));
            string shift2_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_2_time_ends));
            string shift3_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_3_time_begins));
            string shift3_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_3_time_ends));

            string total_cigracks = cigrack.Length.ToString();
            string total_lottoracks = lottorack.Length.ToString();
			string store_plan_id = Context.Database.SqlQuery<string>("select Convert(nvarchar(10),c.plan_id) as plan_id from corporate_mst c inner join store_profile_mst s on c.id=s.corp_id and s.id="+store_id).FirstOrDefault();


			StringBuilder strcig = new StringBuilder();
            for (int i = 0; i < cigrack.Length; i++)
            {
                strcig.Append(cigrack[i].cigs_num_rack + "," + cigrack[i].cigs_rack_rows + "," + cigrack[i].cigs_rack_columns+","+cigrack[i].rack_name);
                if (i < cigrack.Length-1)
                    strcig.Append("|");
            }

            StringBuilder strlotto = new StringBuilder();
            for (int i = 0; i < lottorack.Length; i++)
            {
                strlotto.Append(lottorack[i].lotto_num_racks + "," + lottorack[i].lotto_rack_rows + "," + lottorack[i].lotto_rack_columns + "," + lottorack[i].lotto_rack_name);
                if (i < lottorack.Length-1)
                    strlotto.Append("|");
            }

			var chk = (new DB_shift_checklist_store_mst()).GetCheckList(store_id);
			DB_shift_store_times shifts = new DB_shift_store_times();
			var sh=shifts.Getshift_store_times(store_id);
			shiftreportapp.data.store_racks_array racksets = GetracksetsCollection(store_id);

				return new BusinessInfoDM()
                {
					store_plan= store_plan_id,
					shift_1_time_begins = shift1_start_text,
                    shift_1_time_ends = shift1_end_text,
                    shift_2_time_begins = shift2_start_text,
                    shift_2_time_ends = shift2_end_text,
                    shift_3_time_begins = shift3_start_text,
                    shift_3_time_ends = shift3_end_text,
                    cigs_num_rack = total_cigracks,
                    cigs_rack_columns = "",
                    cigs_rack_rows = "",
                    lotto_num_racks = total_lottoracks,
                    lotto_rack_columns = "",
                    lotto_rack_rows = "",
                    cig_rack_layout = strcig.ToString(),
                    lotto_bin_layout = strlotto.ToString(),
                    store_cat=Convert.ToInt32(store.Store_Catagory),
					shifts=sh,
					check=chk,
					store_racks_array=racksets
				};
        }


		public static BusinessInfoDM VcCreateBuismesinfo(int store_id,int is_mobile_register)
		{
			AppModel Context = new AppModel();
			var store = Context.store_profile_mst2.Find(store_id);
			var cigrack = Context.store_cig_rack_dtl2.Where(r => r.store_id == store_id).ToArray();
			var lottorack = Context.store_lotto_rack_dtl2.Where(r => r.store_id == store_id).ToArray();
			if (store == null)
			{
				throw new AppException("No Shift found for this cashier", "-1");
			}
			else
			{

			}

			string shift1_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_1_time_begins));
			string shift1_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_1_time_ends));
			string shift2_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_2_time_begins));
			string shift2_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_2_time_ends));
			string shift3_start_text = MilitaryToDateTime(Convert.ToInt32(store.shift_3_time_begins));
			string shift3_end_text = MilitaryToDateTime(Convert.ToInt32(store.shift_3_time_ends));

			string total_cigracks = cigrack.Length.ToString();
			string total_lottoracks = lottorack.Length.ToString();
			string store_plan_id = Context.Database.SqlQuery<string>("select Convert(nvarchar(10),c.plan_id) as plan_id from corporate_mst c inner join store_profile_mst s on c.id=s.corp_id and s.id=" + store_id).FirstOrDefault();


			StringBuilder strcig = new StringBuilder();
			for (int i = 0; i < cigrack.Length; i++)
			{
				strcig.Append(cigrack[i].cigs_num_rack + "," + cigrack[i].cigs_rack_rows + "," + cigrack[i].cigs_rack_columns + "," + cigrack[i].rack_name);
				if (i < cigrack.Length - 1)
					strcig.Append("|");
			}

			StringBuilder strlotto = new StringBuilder();
			for (int i = 0; i < lottorack.Length; i++)
			{
				strlotto.Append(lottorack[i].lotto_num_racks + "," + lottorack[i].lotto_rack_rows + "," + lottorack[i].lotto_rack_columns + "," + lottorack[i].lotto_rack_name);
				if (i < lottorack.Length - 1)
					strlotto.Append("|");
			}
			
			var chk = (new DB_shift_checklist_store_mst()).GetCheckList(store_id);
			DB_shift_store_times shifts = new DB_shift_store_times();
			var sh = shifts.Getshift_store_times(store_id);
			shiftreportapp.data.store_racks_array racksets = GetracksetsCollection(store_id);
			List<vc_StoreRegisters> vc = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id && r.IsGasPump==0).ToList();
			return new BusinessInfoDM()
			{
				store_plan = store_plan_id,
				shift_1_time_begins = shift1_start_text,
				shift_1_time_ends = shift1_end_text,
				shift_2_time_begins = shift2_start_text,
				shift_2_time_ends = shift2_end_text,
				shift_3_time_begins = shift3_start_text,
				shift_3_time_ends = shift3_end_text,
				cigs_num_rack = total_cigracks,
				cigs_rack_columns = "",
				cigs_rack_rows = "",
				lotto_num_racks = total_lottoracks,
				lotto_rack_columns = "",
				lotto_rack_rows = "",
				cig_rack_layout = strcig.ToString(),
				lotto_bin_layout = strlotto.ToString(),
				store_cat = Convert.ToInt32(store.Store_Catagory),
				shifts = sh,
				check = chk,
				store_racks_array = racksets,
				vc_StoreRegisters = vc,
                is_mobile_register = is_mobile_register
            };
		}


		public static store_racks_array GetracksetsCollectionAll()
		{
			store_racks_array D = new store_racks_array();
			#region Helpers
			DB_rackset_mst p = new DB_rackset_mst();
			DB_section_mst sec = new DB_section_mst();
			DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
			DB_rack_mst ra = new DB_rack_mst();
			DB_rack_names_mst ns = new DB_rack_names_mst();


			#endregion

			var rackset = p.Getrackset_mstByStoreID(0);
			D.store_racks = new List<store_racks>();
			for (int i = 0; i < rackset.Length; i++)
			{
				store_racks str_rac = new store_racks();
				str_rac.id = rackset[i].id;
				str_rac.title = rackset[i].rack_title;

				str_rac.header = new header();
				str_rac.footer = new footer();
				str_rac.rack = new List<racks>();

				str_rac.header.Sections = new List<Sections>();
				str_rac.footer.Sections = new List<Sections>();



				var header = sec.GetHeaderSections(rackset[i].id);
				var footer = sec.GetFooterSections(rackset[i].id);
				var racks = ra.GetAllRacksFoRackSet(rackset[i].id);

				#region Header
				for (int k = 0; k < header.Length; k++)
				{
					var S = header[k];
					var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
					var SS = new Sections(); // The response section element
					SS.id = S.Id;
					SS.elemnts = new List<elements>();
					for (int j = 0; j < sec_elem.Length; j++)
					{
						SS.elemnts.Add(new elements()
						{
							id = sec_elem[j].Id,
							title = sec_elem[j].elem_lable,
							uuid = sec_elem[j].elem_uuid,
							def = ""
						});
					}
					SS.show_border = S.show_boarder;
					SS.Title = S.section_title;
					str_rac.header.Sections.Add(SS);
				}
				#endregion
				#region Footer

				for (int k = 0; k < footer.Length; k++)
				{
					var S = footer[k];
					var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
					var SS = new Sections(); // The response section element
					SS.id = S.Id;
					SS.elemnts = new List<elements>();
					for (int jj = 0; jj < sec_elem.Length; jj++)
					{
						SS.elemnts.Add(new elements()
						{
							id = sec_elem[jj].Id,
							title = sec_elem[jj].elem_lable,
							uuid = sec_elem[jj].elem_uuid,
							def = ""
						});
					}
					SS.show_border = S.show_boarder;
					SS.Title = S.section_title;
					str_rac.footer.Sections.Add(SS);



				}
				#endregion
				#region Racks
				str_rac.rack = new List<racks>();
				for (int k = 0; k < racks.Length; k++)
				{
					var rack = racks[k];
					var r = ns.Getrack_names_mst(rack.Id, rack.rack_no);


					if (r != null)
						str_rac.rack.Add(new racks()
						{
							id = rack.Id,
							rack_name = rack.rack_name,
							rack_no = rack.rack_no,
							rack_row = rack.rack_row,
							rack_col = rack.rock_col,
							show_added = rack.show_added,
							show_ended = rack.show_ended,
							show_started = rack.show_started,
							product_label = r,
							show_product_label = rack.show_product_label
						});
					
				}

				D.store_racks.Add(str_rac);
				#endregion

			}



			return D;
		}

		public static store_racks_array GetracksetsCollection(int store_id)
		{
			store_racks_array d = new store_racks_array();
			#region Helpers
			DB_rackset_mst p = new DB_rackset_mst();
			DB_section_mst sec = new DB_section_mst();
			DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
			DB_rack_mst ra = new DB_rack_mst();
			DB_rack_names_mst ns = new DB_rack_names_mst();
			

			#endregion

			var rackset = p.Getrackset_mstByStoreID(store_id);
			d.store_racks = new List<store_racks>();
			for (int i = 0; i < rackset.Length; i++)
			{
				store_racks str_rac = new store_racks();
				str_rac.id = rackset[i].id;
				str_rac.title = rackset[i].rack_title;

				str_rac.header = new header();
				str_rac.footer = new footer();
				str_rac.rack = new List<racks>();

				str_rac.header.Sections = new List<Sections>();
				str_rac.footer.Sections = new List<Sections>();

				

				var header = sec.GetHeaderSections(rackset[i].id);
				var footer = sec.GetFooterSections(rackset[i].id);
				var racks = ra.GetAllRacksFoRackSet(rackset[i].id);

				#region Header
				for(int k = 0; k < header.Length; k++)
				{
					var S = header[k];
					var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
					var SS = new Sections(); // The response section element
					SS.id = S.Id;
					SS.elemnts = new List<elements>();
					for (int j=0;j< sec_elem.Length; j++)
					{
						SS.elemnts.Add(new elements()
						{
							id = sec_elem[j].Id,
							title = sec_elem[j].elem_lable,
							uuid = sec_elem[j].elem_uuid,
							def = ""
						});
					}
					SS.show_border = S.show_boarder;
					SS.Title = S.section_title;
					str_rac.header.Sections.Add(SS);
				}
				#endregion
				#region Footer

				for (int k = 0; k < footer.Length; k++)
				{
					var S = footer[k];
					var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
					var SS = new Sections(); // The response section element
					SS.id = S.Id;
					SS.elemnts = new List<elements>();
					for (int jj=0;jj < sec_elem.Length; jj++)
					{
						SS.elemnts.Add(new elements() {
							id = sec_elem[jj].Id,
							title=sec_elem[jj].elem_lable,
							uuid=sec_elem[jj].elem_uuid,
							def=""
						});
					}
					SS.show_border = S.show_boarder;
					SS.Title = S.section_title;
					str_rac.footer.Sections.Add(SS);



				}
				#endregion
				#region Racks
				str_rac.rack = new List<racks>();
				for (int k = 0; k < racks.Length; k++)
				{
					var rack = racks[k];
					var r =ns.Getrack_names_mst(rack.Id, rack.rack_no);
					

					if(r!=null)
					str_rac.rack.Add(new racks() {
						id=rack.Id,
						rack_name=rack.rack_name,
						rack_no=rack.rack_no,
						rack_row=rack.rack_row,
						rack_col=rack.rock_col,
						show_added=rack.show_added,
						show_ended=rack.show_ended,
						show_started=rack.show_started,
						product_label = r,
						show_product_label=rack.show_product_label
					});
					
				}
				d.store_racks.Add(str_rac);
				
				#endregion
				
			}
			
			
			
			return d;
		}




		public static List<store_racks> GetracksetsCollection()
		{
			List<store_racks> d = new List<store_racks>();
			#region Helpers
			DB_rackset_mst p = new DB_rackset_mst();
			DB_section_mst sec = new DB_section_mst();
			DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
			DB_rack_mst ra = new DB_rack_mst();
			DB_rack_names_mst ns = new DB_rack_names_mst();
			DB_rack_template_mst racktmp = new DB_rack_template_mst();
			#endregion
			var x = racktmp.GetAll();
			
			for(int ii=0;ii<x.Length;ii++)
			{
					var rackset = p.Getrackset_mst(x[ii]);
				if (rackset == null)
					continue;
					store_racks str_rac = new store_racks();
					str_rac.id = rackset.id;
				    str_rac.title = rackset.rack_title;
					str_rac.header = new header();
					str_rac.footer = new footer();
					str_rac.rack = new List<racks>();
					str_rac.header.Sections = new List<Sections>();
					str_rac.footer.Sections = new List<Sections>();
					var header = sec.GetHeaderSections(rackset.id);
					var footer = sec.GetFooterSections(rackset.id);
					var racks = ra.GetAllRacksFoRackSet(rackset.id);

					#region Header
					for (int k = 0; k < header.Length; k++)
					{
						var S = header[k];
						var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
						var SS = new Sections(); // The response section element
						SS.id = S.Id;
						SS.elemnts = new List<elements>();
						for (int j = 0; j < sec_elem.Length; j++)
						{
							SS.elemnts.Add(new elements()
							{
								id = sec_elem[j].Id,
								title = sec_elem[j].elem_lable,
								uuid = sec_elem[j].elem_uuid,
								def = ""
							});
						}
						SS.show_border = S.show_boarder;
						SS.Title = S.section_title;
						str_rac.header.Sections.Add(SS);
					}
					#endregion
					#region Footer

					for (int k = 0; k < footer.Length; k++)
					{
						var S = footer[k];
						var sec_elem = e.GetElementsForSection(S.Id); // The database section Elemnt
						var SS = new Sections(); // The response section element
						SS.elemnts = new List<elements>();
						for (int jj = 0; jj < sec_elem.Length; jj++)
						{
							SS.elemnts.Add(new elements()
							{
								id = sec_elem[jj].Id,
								title = sec_elem[jj].elem_lable,
								uuid = sec_elem[jj].elem_uuid,
								def = ""
							});
						}
						SS.show_border = S.show_boarder;
						SS.Title = S.section_title;
						str_rac.footer.Sections.Add(SS);



					}
					#endregion
					#region Racks
					str_rac.rack = new List<shiftreportapp.data.racks>();
					for (int k = 0; k < racks.Length; k++)
					{
						var rack = racks[k];
						var r = ns.Getrack_names_mst(rackset.id, rack.rack_no);
						if (r != null)
							str_rac.rack.Add(new racks()
							{
								rack_name = rack.rack_name,
								rack_no = rack.rack_no,
								rack_row = rack.rack_row,
								rack_col = rack.rock_col,
								show_added = rack.show_added,
								show_ended = rack.show_ended,
								show_started = rack.show_ended,
								product_label = r
							});


					
					#endregion

					d.Add(str_rac);
				}
			}
			return d;
		}
		public static void AddnewRackStoreCollection(store_racks_array_add data)
		{
			DB_rackset_mst p = new DB_rackset_mst();
			DB_section_mst sec = new DB_section_mst();
			DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
			DB_rack_mst ra = new DB_rack_mst();
			DB_rack_names_mst ns = new DB_rack_names_mst();
			DB_rack_inventory_level_mst inv = new DB_rack_inventory_level_mst();

			for (int i = 0;i< data.insert_store_racks.Count; i++)
			{
				int rackset_id=p.Addrackset_mst(new AppModel.rackset_mst() {
					store_id=data.store_id,
					rack_name=data.insert_store_racks[i].title,
					rack_title=data.insert_store_racks[i].title
				});

				for(int k = 0; k < data.insert_store_racks[i].header.Sections.Count; k++)
				{
					int section_id = sec.Addsection_mst(new AppModel.section_mst() {
						section_name = data.insert_store_racks[i].header.Sections[k].Title,
						section_title =data.insert_store_racks[i].header.Sections[k].Title,
						section_type="header",
						show_boarder=data.insert_store_racks[i].show_boarder,
						rackset_id=rackset_id
					});
					for(int k2 = 0; k2 < data.insert_store_racks[i].header.Sections[k].elemnts.Count; k2++)
					{
						e.Addsection_elemnts_dtls(new AppModel.section_elemnts_dtls() {
							elem_uuid=data.insert_store_racks[i].header.Sections[k].elemnts[k2].uuid,
							elem_lable=data.insert_store_racks[i].header.Sections[k].elemnts[k2].title,
							elem_name=data.insert_store_racks[i].header.Sections[k].elemnts[k2].title,
							section_id=section_id
						});
					}

				}

				if(data.insert_store_racks[i].footer!=null)
				for (int z = 0; z < data.insert_store_racks[i].footer.Sections.Count; z++)
				{
					int section_id = sec.Addsection_mst(new AppModel.section_mst()
					{
						section_name = data.insert_store_racks[i].footer.Sections[z].Title,
						section_title = data.insert_store_racks[i].footer.Sections[z].Title,
						section_type = "footer",
						show_boarder = data.insert_store_racks[i].show_boarder,
						rackset_id = rackset_id
					});
					for (int z2 = 0; z2 < data.insert_store_racks[i].footer.Sections[z].elemnts.Count; z2++)
					{
						e.Addsection_elemnts_dtls(new AppModel.section_elemnts_dtls()
						{
							elem_uuid = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].uuid,
							elem_lable = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].title,
							elem_name = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].title,
							section_id = section_id
						});
						
					}
				}

				for (int x = 0; x < data.insert_store_racks[i].rack.Count; x++)
				{
					ra.Addrack_mst(new AppModel.rack_mst() {
						rack_name=data.insert_store_racks[i].rack[x].rack_name,
						rack_no= data.insert_store_racks[i].rack[x].rack_no,
						rack_row = data.insert_store_racks[i].rack[x].rack_row,
						rock_col = data.insert_store_racks[i].rack[x].rack_col,
						show_added= data.insert_store_racks[i].rack[x].show_added,
						show_started = data.insert_store_racks[i].rack[x].show_started,
						show_ended=data.insert_store_racks[i].rack[x].show_ended,
						show_product_label= data.insert_store_racks[i].rack[x].show_product_label,
						rackset_id =rackset_id

					});

					
						for (int X = 0; X < data.insert_store_racks[i].rack[x].rack_no; X++)
						{
							for (int XX = 0; XX < data.insert_store_racks[i].rack[x].rack_row; XX++)
							{
								for (int XXX = 0; XXX < data.insert_store_racks[i].rack[x].rack_col; XXX++)
								{
									inv.Addrack_inventory_level_mst(new AppModel.rack_inventory_level_mst()
									{
										rack_id = rackset_id,// data.insert_store_racks[i].id,
										store_id = data.store_id,
										rack_no = X,
										row_no = XX,
										col_no = XXX,
										inventory_level_amt = "",
										product_name = ""
									});
								}
							}
						}
					

					for (int z3 = 0; z3 < data.insert_store_racks[i].rack[x].product_label.Length; z3++)
					{
						ns.Addrack_names_mst(new AppModel.rack_names_mst() {
							rack_id=rackset_id,
							rack_no= data.insert_store_racks[i].rack[x].rack_no,
							col_no= data.insert_store_racks[i].rack[x].product_label[z3].col_no,
							row_no= data.insert_store_racks[i].rack[x].product_label[z3].row_no,
							product_name= data.insert_store_racks[i].rack[x].product_label[z3].product_name

						});
						inv.UpdateLabelname(rackset_id, data.store_id, data.insert_store_racks[i].rack[x].rack_no, data.insert_store_racks[i].rack[x].product_label[z3].row_no, data.insert_store_racks[i].rack[x].product_label[z3].col_no, "", data.insert_store_racks[i].rack[x].product_label[z3].product_name);
					


					}
				}
				/*
				 * 
				 * inv.Addrack_inventory_level_mst(new AppModel.rack_inventory_level_mst()
						{
							rack_id = rackset_id,// data.insert_store_racks[i].id,
							store_id = data.store_id,
							rack_no = data.insert_store_racks[i].rack[x].rack_no,
							row_no = data.insert_store_racks[i].rack[x].product_label[z3].row_no,
							col_no = data.insert_store_racks[i].rack[x].product_label[z3].col_no,
							inventory_level_amt = "",
							product_name = data.insert_store_racks[i].rack[x].product_label[z3].product_name
						});
				 * 
				 * 
				 * 
				 * 
				 * */

			}
		}
		public static void UpdateRackStoreCollection(store_racks_array_update data)
		{
			DB_rackset_mst p = new DB_rackset_mst();
			DB_section_mst sec = new DB_section_mst();
			DB_section_elemnts_dtls e = new DB_section_elemnts_dtls();
			DB_rack_mst ra = new DB_rack_mst();
			DB_rack_names_mst ns = new DB_rack_names_mst();
			DB_rack_inventory_level_mst inv = new DB_rack_inventory_level_mst();
			#region add 
			if(data.insert_store_racks!=null)
			for (int i = 0; i < data.insert_store_racks.Count; i++)
			{
				int rackset_id = p.Addrackset_mst(new AppModel.rackset_mst()
				{
					store_id = data.store_id,
					rack_name = data.insert_store_racks[i].title,
					rack_title = data.insert_store_racks[i].title
				});

				for (int k = 0; k < data.insert_store_racks[i].header.Sections.Count; k++)
				{
					int section_id = sec.Addsection_mst(new AppModel.section_mst()
					{
						section_name = data.insert_store_racks[i].header.Sections[k].Title,
						section_title = data.insert_store_racks[i].header.Sections[k].Title,
						section_type = "header",
						show_boarder = data.insert_store_racks[i].show_boarder,
						rackset_id = rackset_id
					});
					for (int k2 = 0; k2 < data.insert_store_racks[i].header.Sections[k].elemnts.Count; k2++)
					{
						e.Addsection_elemnts_dtls(new AppModel.section_elemnts_dtls()
						{
							elem_uuid = data.insert_store_racks[i].header.Sections[k].elemnts[k2].uuid,
							elem_lable = data.insert_store_racks[i].header.Sections[k].elemnts[k2].title,
							elem_name = data.insert_store_racks[i].header.Sections[k].elemnts[k2].title,
							section_id = section_id
						});
					}
				}

				if (data.insert_store_racks[i].footer != null)
					for (int z = 0; z < data.insert_store_racks[i].footer.Sections.Count; z++)
					{
						int section_id = sec.Addsection_mst(new AppModel.section_mst()
						{
							section_name = data.insert_store_racks[i].footer.Sections[z].Title,
							section_title = data.insert_store_racks[i].footer.Sections[z].Title,
							section_type = "footer",
							show_boarder = data.insert_store_racks[i].show_boarder,
							rackset_id = rackset_id
						});
						for (int z2 = 0; z2 < data.insert_store_racks[i].footer.Sections[z].elemnts.Count; z2++)
						{
							e.Addsection_elemnts_dtls(new AppModel.section_elemnts_dtls()
							{
								elem_uuid = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].uuid,
								elem_lable = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].title,
								elem_name = data.insert_store_racks[i].footer.Sections[z].elemnts[z2].title,
								section_id = section_id
							});
						}
					}

				for (int x = 0; x < data.insert_store_racks[i].rack.Count; x++)
				{
					int ID=ra.Addrack_mst(new AppModel.rack_mst()
					{
						rack_name = data.insert_store_racks[i].rack[x].rack_name,
						rack_no = data.insert_store_racks[i].rack[x].rack_no,
						rack_row = data.insert_store_racks[i].rack[x].rack_row,
						rock_col = data.insert_store_racks[i].rack[x].rack_col,
						show_added = data.insert_store_racks[i].rack[x].show_added,
						show_started = data.insert_store_racks[i].rack[x].show_started,
						show_ended = data.insert_store_racks[i].rack[x].show_ended,
						rackset_id = rackset_id,
						show_product_label= data.insert_store_racks[i].rack[x].show_product_label

					});

					for (int z3 = 0; z3 < data.insert_store_racks[i].rack[x].product_label.Length; z3++)
					{
						ns.Addrack_names_mst(new AppModel.rack_names_mst()
						{
							rack_id = ID,
							rack_no = data.insert_store_racks[i].rack[x].rack_no,
							col_no = data.insert_store_racks[i].rack[x].product_label[z3].col_no,
							row_no = data.insert_store_racks[i].rack[x].product_label[z3].row_no,
							product_name = data.insert_store_racks[i].rack[x].product_label[z3].product_name

						});
						

					}
					for(int k=0;k< data.insert_store_racks[i].rack[x].rack_no; k++)
					{
						for (int kk = 0; kk < data.insert_store_racks[i].rack[x].rack_no; kk++)
						{
							for (int kkk = 0; kkk < data.insert_store_racks[i].rack[x].rack_no; kkk++)
							{
								inv.Addrack_inventory_level_mst(new AppModel.rack_inventory_level_mst() {
									rack_id=rackset_id,
									rack_no=k,
									row_no=kk,
									col_no=kkk,
									store_id=data.store_id
								});
							}
						}
					}
				}
			}
			#endregion
			#region update
			if (data.update_store_racks != null)
				for (int i = 0; i < data.update_store_racks.Count; i++)
				{

					int rackset_id = data.update_store_racks[i].id;

					p.Saverackset_mst(data.update_store_racks[i].id, data.update_store_racks[i].title);

					for (int k = 0; k < data.update_store_racks[i].header.Sections.Count; k++)
					{
						int id=sec.Savesection_mst(data.update_store_racks[i].header.Sections[k].id,data.update_store_racks[i].id,
						data.update_store_racks[i].header.Sections[k],"header"
					   );
						for (int k2 = 0; k2 < data.update_store_racks[i].header.Sections[k].elemnts.Count; k2++)
						{
							e.Savesection_elemnts_dtls(data.update_store_racks[i].header.Sections[k].elemnts[k2].id,
								id,
								data.update_store_racks[i].header.Sections[k].elemnts[k2].uuid, data.update_store_racks[i].header.Sections[k].elemnts[k2].title
							);
						}

					}

					if (data.update_store_racks[i].footer != null)
						for (int z = 0; z < data.update_store_racks[i].footer.Sections.Count; z++)
						{
							sec.Savesection_mst(data.update_store_racks[i].footer.Sections[z].id,
								data.update_store_racks[i].id,
							   data.update_store_racks[i].footer.Sections[z], "footer"
						   );
							for (int z2 = 0; z2 < data.update_store_racks[i].footer.Sections[z].elemnts.Count; z2++)
							{
								e.Savesection_elemnts_dtls(data.update_store_racks[i].footer.Sections[z].elemnts[z2].id,
									data.update_store_racks[i].footer.Sections[z].id,
									data.update_store_racks[i].footer.Sections[z].elemnts[z2].uuid,
									data.update_store_racks[i].footer.Sections[z].elemnts[z2].title);

							}

						}


					for (int x = 0; x < data.update_store_racks[i].rack.Count; x++)
					{
						int ID=ra.Saverack_mst(data.update_store_racks[i].rack[x].id, new AppModel.rack_mst()
						{
							rackset_id=rackset_id,
							rack_name = data.update_store_racks[i].rack[x].rack_name,
							rack_no = data.update_store_racks[i].rack[x].rack_no,
							rack_row = data.update_store_racks[i].rack[x].rack_row,
							rock_col = data.update_store_racks[i].rack[x].rack_col,
							show_added = data.update_store_racks[i].rack[x].show_added,
							show_started = data.update_store_racks[i].rack[x].show_started,
							show_ended = data.update_store_racks[i].rack[x].show_ended,
							show_product_label = data.update_store_racks[i].rack[x].show_product_label
						});

						for (int z3 = 0; z3 < data.update_store_racks[i].rack[x].product_label.Length; z3++)
						{
							//ns.Saverack_names_mst(rackset_id, data.update_store_racks[i].rack[x].rack_no, data.update_store_racks[i].rack[x].product_label[z3].col_no, data.update_store_racks[i].rack[x].product_label[z3].row_no, data.update_store_racks[i].rack[x].product_label[z3].product_name,data.store_id);

							int row = data.update_store_racks[i].rack[x].product_label[z3].row_no;
							int col = data.update_store_racks[i].rack[x].product_label[z3].col_no;
							int rack_no = data.update_store_racks[i].rack[x].rack_no;
							AppModel Context = new AppModel();
							var DN = (from xxx in Context.rack_names_mst2
									  where xxx.rack_id == rackset_id && xxx.rack_no == rack_no && xxx.row_no ==row  && xxx.col_no == col
									  select xxx).FirstOrDefault();
							if (DN != null)
							{
								DN.product_name = data.update_store_racks[i].rack[x].product_label[z3].product_name;

								Context.Entry(DN).State = System.Data.Entity.EntityState.Modified;
								Context.SaveChanges();
								Context.Dispose();
							}
							else
							{
								DN = new AppModel.rack_names_mst()
								{
									rack_id = ID,
									row_no = data.update_store_racks[i].rack[x].product_label[z3].row_no,
									col_no = data.update_store_racks[i].rack[x].product_label[z3].col_no,
									rack_no = data.update_store_racks[i].rack[x].rack_no,
									product_name = data.update_store_racks[i].rack[x].product_label[z3].product_name
								};
								Context.rack_names_mst2.Add(DN);
								Context.SaveChanges();



								inv.Addrack_inventory_level_mst(new AppModel.rack_inventory_level_mst()
								{
									rack_id = rackset_id,
									rack_no = DN.rack_no,
									row_no = DN.row_no,
									col_no = DN.col_no,
									store_id = data.store_id
								});
							}
						}
					}
					#endregion
			#region delete

					if (data.delete_store_racks != null)
						for (int f = 0; f < data.delete_store_racks.Count; f++)
						{

							int racksetid = data.delete_store_racks[f].id;
							p.Deleterackset_mst(racksetid);
						}
				}



			#endregion
		}
		public static shiftreportapp.data.AppModel.ShiftResult CreateNewShiftForStoreid(int casheir_id, int shift_no)
        {
            AppModel Context = new AppModel();


            var shift = new shiftreportapp.data.AppModel.shift_details_mst();
            int shift_start = 700, shift_end = 1500;
            string shift_start_text = "7am", shift_end_text = "3pm";
            
           
            var caheir = Context.cashier_mst2.Find(casheir_id);
            var store = Context.store_profile_mst2.Find(caheir.store_id);

			
            switch (shift_no)
            {
                case (1):
                    shift_start = Convert.ToInt32(store.shift_1_time_begins);
                    shift_end = Convert.ToInt32(store.shift_1_time_ends);

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text = MilitaryToDateTime(Convert.ToInt32(shift_end));
                    break;
                case (2):
                     shift_start = Convert.ToInt32(store.shift_2_time_begins);
                    shift_end = Convert.ToInt32(store.shift_2_time_ends);

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text =MilitaryToDateTime(Convert.ToInt32(shift_end));
                    break;
                case (3):
                    shift_start = Convert.ToInt32(store.shift_3_time_begins);
                    shift_end = Convert.ToInt32(store.shift_3_time_ends);

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text = MilitaryToDateTime(Convert.ToInt32(shift_end));

                    break;

            }

			// 
            shift.store_id = store.id;
            shift.shift_date = DateTime.Now.ToShortDateString();
            shift.shift_scheduled_open = shift_start;
            shift.shift_scheduled_close = shift_end;
            shift.shift_num = shift_no;
            shift.cashier_id = casheir_id;
			shift.cashier_name = caheir.cashier_name;
		   // shift.shift_session_started = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
			shift.shift_status = "P";
           
            Context.shift_details_mst2.Add(shift);
            Context.SaveChanges();
			
            int shiftstatus = 1;
            string shiftxt = (shift_start_text + " - " + shift_end_text + " Shift");


            return new shiftreportapp.data.AppModel.ShiftResult (){ shiftkey = shift.id, shiftstatus = shiftstatus, shift_time = shiftxt,store_id=Convert.ToInt32(shift.store_id)};

        }
		public static shiftreportapp.data.AppModel.ShiftResult CreateNewShiftForStoreid_cashapp(int casheir_id, int shift_no)
		{
			AppModel Context = new AppModel();


			var shift = new shiftreportapp.data.AppModel.shift_details_mst();
			int shift_start = 700, shift_end = 1500;
			string shift_start_text = "7am", shift_end_text = "3pm";


			var caheir = Context.cashier_mst2.Find(casheir_id);
			var store = Context.store_profile_mst2.Find(caheir.store_id);

			var shifttimes = Context.Database.SqlQuery<store_times>("exec usp_sr_getshifttimes @store_id=" + store.id + ",@shift_no=" + shift_no, new Object[] { }).FirstOrDefault();

			shift_start = Convert.ToInt32(shifttimes.start_time);
			shift_end = Convert.ToInt32(shifttimes.end_time);
			// 
			shift.store_id = store.id;
			shift.shift_date = DateTime.Now.ToShortDateString();
			shift.shift_scheduled_open = shift_start;
			shift.shift_scheduled_close = shift_end;
			shift.shift_num = shift_no;
			shift.cashier_id = casheir_id;
			// shift.shift_session_started = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
			shift.shift_status = "P";

			Context.shift_details_mst2.Add(shift);
			Context.SaveChanges();

			int shiftstatus = 1;
			string shiftxt = (shift_start_text + " - " + shift_end_text + " Shift");


			return new shiftreportapp.data.AppModel.ShiftResult() { shiftkey = shift.id, shiftstatus = shiftstatus, shift_time = shiftxt };

		}
 		public static shiftreportapp.data.AppModel.ShiftResult GetShiftForStoreid(int shiftid)
        {
            AppModel Context = new AppModel();
          
            int shift_start = 700, shift_end = 1500;
            string shift_start_text = "7am", shift_end_text = "3pm";


            var sh = Context.shift_details_mst2.Find(shiftid);
            var store = Context.store_profile_mst2.Find(sh.store_id);

            switch (sh.shift_num)
            {
                case (1):
                    shift_start = Convert.ToInt32(store.shift_1_time_begins);
                    shift_end = Convert.ToInt32((store.shift_1_time_ends));

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text = MilitaryToDateTime(Convert.ToInt32(shift_end));
                    break;
                case (2):
                    shift_start = Convert.ToInt32(store.shift_2_time_begins);
                    shift_end = Convert.ToInt32(store.shift_2_time_ends);

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text = MilitaryToDateTime(Convert.ToInt32(shift_end));
                    break;
                case (3):
                    shift_start = Convert.ToInt32(store.shift_3_time_begins);
                    shift_end = Convert.ToInt32(store.shift_3_time_ends);

                    shift_start_text = MilitaryToDateTime(Convert.ToInt32(shift_start));
                    shift_end_text = MilitaryToDateTime(Convert.ToInt32(shift_end));

                    break;

            }
          
            int shiftstatus = 0;
            string shiftxt = (shift_start_text + " - " + shift_end_text + "Shift");


            return new shiftreportapp.data.AppModel.ShiftResult() { shiftkey = sh.id, shiftstatus = shiftstatus, shift_time = shiftxt };

        }
		public static shiftreportapp.data.AppModel.ShiftResult GetShiftForStoreid_cashapp(int shiftid)
		{
			AppModel Context = new AppModel();

			
			string shift_start_text = "7am", shift_end_text = "3pm";


			var sh = Context.shift_details_mst2.Find(shiftid);
			var store = Context.store_profile_mst2.Find(sh.store_id);

			var shifttimes = Context.Database.SqlQuery<store_times>("exec usp_sr_getshifttimes @store_id=" + store.id + ",@shift_no=" + sh.shift_num, new Object[] { }).FirstOrDefault();

			shift_start_text = FormatShiftTime(shifttimes.start_time);
			shift_start_text = FormatShiftTime(shifttimes.end_time);
			
			int shiftstatus = 0;
			string shiftxt = (shift_start_text + " - " + shift_end_text + "Shift");


			return new shiftreportapp.data.AppModel.ShiftResult() { shiftkey = sh.id, shiftstatus = shiftstatus, shift_time = shiftxt };

		}
		public static string MilitaryToDateTime(int Military)
        {
            int Hours = Military / 100;
            int Minutes = Military - Hours * 100;

            if (Hours < 12)
                return Hours.ToString() + "AM";
            else
                return (Hours-12).ToString() + "PM";

            
        }
		public static string FormatShiftTime(string time)
		{
			if (time.Length > 1)
			{
				time = time.Substring(0, 2);
			}
			else
			{
				time = "00";
			}
			int x = Int32.Parse(time);
			TimeSpan span = new TimeSpan(x, 00, 00);
			DateTime xtime = DateTime.Today.Add(span);

			return xtime.ToString("hh:mm tt");
		}
		public static int IsCheklistValid(Object data)
        {
            return 0;
        }
	
		public static string GetLastFourDigits(String cardnumber) 
		{
			return cardnumber.Substring(cardnumber.Length - 4, 4);
		}
		

	}
    public class shift_cigarit_dtl_helper
    {
        public void Addshift_cigarit_dtl(shiftreportapp.data.AppModel.shift_cigarit_dtl data)
        {
            AppModel Context = new AppModel();
            Context.shift_cigarit_dtl2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }
        public shiftreportapp.data.AppModel.shift_cigarit_dtl Getshift_cigarit_dtl(int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.shift_cigarit_dtl2
                where x.Id==id
                select x).FirstOrDefault();
        }
        public void Saveshift_cigarit_dtl(int id, shiftreportapp.data.AppModel.shift_cigarit_dtl data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }
        public void Deleteshift_cigarit_dtl(shiftreportapp.data.AppModel.shift_cigarit_dtl data)
        {
            AppModel Context = new AppModel();
            Context.shift_cigarit_dtl2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
    }
    public class program_error_log_helper
    {

        // DbContext
        AppModel Context = new AppModel();

        public void Addprogram_error_log(shiftreportapp.data.AppModel.program_error_log data)
        {
            Context.program_error_log2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.program_error_log Getprogram_error_log(int id)
        {
        return (from x in Context.program_error_log2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Saveprogram_error_log(int id, shiftreportapp.data.AppModel.program_error_log data)
        {
            Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deleteprogram_error_log(shiftreportapp.data.AppModel.program_error_log data)
        {
            Context.program_error_log2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
    }
    public class lotto_bin_dtls_helper{

        // DbContext
        

        public void Addlotto_bin_dtls (shiftreportapp.data.AppModel.lotto_bin_dtls data)
        {
            AppModel Context = new AppModel();
            Context.lotto_bin_dtls2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.lotto_bin_dtls Getlotto_bin_dtls (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.lotto_bin_dtls2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savelotto_bin_dtls (int id,shiftreportapp.data.AppModel.lotto_bin_dtls data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletelotto_bin_dtls (shiftreportapp.data.AppModel.lotto_bin_dtls data)
        {
            AppModel Context = new AppModel();
            Context.lotto_bin_dtls2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class drawer_open_mst_helper{

// DbContext
        

        public void Adddrawer_open_mst (shiftreportapp.data.AppModel.drawer_open_mst data)
        {
            AppModel Context = new AppModel();
            Context.drawer_open_mst2 .Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.drawer_open_mst Getdrawer_open_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.drawer_open_mst2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savedrawer_open_mst (int id,shiftreportapp.data.AppModel.drawer_open_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletedrawer_open_mst (shiftreportapp.data.AppModel.drawer_open_mst data)
        {
            AppModel Context = new AppModel();
            Context.drawer_open_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class phone_cards_helper{


        

        public void Addphone_cards (shiftreportapp.data.AppModel.phone_cards data)
        {
            AppModel Context = new AppModel();
            Context.phone_cards2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.phone_cards Getphone_cards (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.phone_cards2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savephone_cards (int id,shiftreportapp.data.AppModel.phone_cards data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletephone_cards (shiftreportapp.data.AppModel.phone_cards data)
        {
            AppModel Context = new AppModel();
            Context.phone_cards2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class cartons_of_cigarrets_helper{

// DbContext
        

        public void Addcartons_of_cigarrets (shiftreportapp.data.AppModel.cartons_of_cigarrets data)
        {
            AppModel Context = new AppModel();
            Context.cartons_of_cigarrets2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.cartons_of_cigarrets Getcartons_of_cigarrets (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.cartons_of_cigarrets2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savecartons_of_cigarrets (int id,shiftreportapp.data.AppModel.cartons_of_cigarrets data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletecartons_of_cigarrets (shiftreportapp.data.AppModel.cartons_of_cigarrets data)
        {
            AppModel Context = new AppModel();
            Context.cartons_of_cigarrets2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class shift_checklist_mst_helper{

// DbContext
        

        public void Addshift_checklist_mst (shiftreportapp.data.AppModel.shift_checklist_mst data)
        {
            AppModel Context = new AppModel();
            Context.shift_checklist_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.shift_checklist_mst Getshift_checklist_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.shift_checklist_mst2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Saveshift_checklist_mst (int id,shiftreportapp.data.AppModel.shift_checklist_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deleteshift_checklist_mst (shiftreportapp.data.AppModel.shift_checklist_mst data)
        {
            AppModel Context = new AppModel();
            Context.shift_checklist_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class cigs_sold_mst_helper{

// DbContext
       

        public void Addcigs_sold_mst (shiftreportapp.data.AppModel.cigs_sold_mst data)
        {
            AppModel Context = new AppModel();
            Context.cigs_sold_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.cigs_sold_mst Getcigs_sold_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.cigs_sold_mst2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savecigs_sold_mst (int id,shiftreportapp.data.AppModel.cigs_sold_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletecigs_sold_mst (shiftreportapp.data.AppModel.cigs_sold_mst data)
        {
            AppModel Context = new AppModel();
            Context.cigs_sold_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class drawer_close_mst_helper{

// DbContext
        AppModel Context = new AppModel();

        public void Adddrawer_close_mst (shiftreportapp.data.AppModel.drawer_close_mst data)
        {
            AppModel Context = new AppModel();
            Context.drawer_close_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.drawer_close_mst Getdrawer_close_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.drawer_close_mst2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savedrawer_close_mst (int id,shiftreportapp.data.AppModel.drawer_close_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletedrawer_close_mst (shiftreportapp.data.AppModel.drawer_close_mst data)
        {
            AppModel Context = new AppModel();
            Context.drawer_close_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class promoters_mst_helper{

// DbContext
       
/*
        public void Addpromoters_mst (shiftreportapp.data.AppModel.promoters_mst data)
        {
            AppModel Context = new AppModel();
            Context.promoters_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.promoters_mst Getpromoters_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.promoters_mst2
                where x.id==id
                select x).FirstOrDefault();
        }

        public void Savepromoters_mst (int id,shiftreportapp.data.AppModel.promoters_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletepromoters_mst (shiftreportapp.data.AppModel.promoters_mst data)
        {
            AppModel Context = new AppModel();
            Context.promoters_mst2 .Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
        */
}
    public class  store_profile_mst_helper{

// DbContext

        
        public void Addstore_profile_mst (shiftreportapp.data.AppModel.store_profile_mst data)
        {
            AppModel Context = new AppModel();
            Context.store_profile_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.store_profile_mst Getstore_profile_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.store_profile_mst2
                where x.id==id
                select x).FirstOrDefault();
        }

        public void Savestore_profile_mst (int id,shiftreportapp.data.AppModel.store_profile_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletestore_profile_mst (shiftreportapp.data.AppModel.store_profile_mst data)
        {
            AppModel Context = new AppModel();
            Context.store_profile_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class manager_report_mst_helper{

// DbContext

       
        public void Addmanager_report_mst (shiftreportapp.data.AppModel.manager_report_mst data)
        {
            AppModel Context = new AppModel();
            Context.manager_report_mst2 .Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.manager_report_mst Getmanager_report_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.manager_report_mst2
                where x.id==id
                select x).FirstOrDefault();
        }

        public void Savemanager_report_mst (int id,shiftreportapp.data.AppModel.manager_report_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletemanager_report_mst (shiftreportapp.data.AppModel.manager_report_mst data)
        {
            AppModel Context = new AppModel();
            Context.manager_report_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }
}
    public class corporate_mst_helper{

// DbContext

        
        public int Addcorporate_mst (shiftreportapp.data.AppModel.corporate_mst data)
        {
            AppModel Context = new AppModel();
            Context.corporate_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
            return data.Id;
        }


        public shiftreportapp.data.AppModel.corporate_mst Getcorporate_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.corporate_mst2
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savecorporate_mst (int id,shiftreportapp.data.AppModel.corporate_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
        }

        public void Deletecorporate_mst (shiftreportapp.data.AppModel.corporate_mst data)
        {
            AppModel Context = new AppModel();
            Context.corporate_mst2.Remove(data);
            Context.SaveChanges();
        }
}
    public class  managers_mst_helper{

// DbContext

        
        public void Addmanagers_mst(shiftreportapp.data.AppModel.managers_mst data)
        {
            AppModel Context = new AppModel();
            Context.managers_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.managers_mst Getmanagers_mst(int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.managers_mst2	
                where x.Id==id
                select x).FirstOrDefault();
        }

        public void Savemanagers_mst(int id,shiftreportapp.data.AppModel.managers_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }

        public void Deletemanagers_mst(shiftreportapp.data.AppModel.managers_mst	 data)
        {
            AppModel Context = new AppModel();
            Context.managers_mst2.Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }

		public void Savemanagers_mst(string corpID,string manager_id, string name, string password, string email, string manager_cell_phone)
		{
			int mid = Convert.ToInt32(manager_id);
			AppModel Context = new AppModel();
			var data = Context.managers_mst2.Find(mid);
			if(data!=null)
			{
				data.corp_id = Convert.ToInt32(corpID);
				data.manager_name = name;
				data.manager_pw = password;
				data.manager_email = email;
				Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
				Context.Dispose();
			}
			
		}
	}
    public class cashier_mst_helper{

// DbContext

        
        public void Addcashier_mst (shiftreportapp.data.AppModel.cashier_mst data)
        {
            AppModel Context = new AppModel();
            Context.cashier_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }


        public shiftreportapp.data.AppModel.cashier_mst Getcashier_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.cashier_mst2
                where x.id==id
                select x).FirstOrDefault();
        }

        public void Savecashier_mst (int id,shiftreportapp.data.AppModel.cashier_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
        }

        public void Deletecashier_mst (shiftreportapp.data.AppModel.cashier_mst data)
        {
            AppModel Context = new AppModel();
            Context.cashier_mst2 .Remove(data);
            Context.SaveChanges();
            Context.Dispose();
        }

		public int GetCasherStorePlanNo(int id)
		{
			AppModel Context = new AppModel();
			var c= (from x in Context.cashier_mst2
					where x.id == id
					select x).FirstOrDefault();
			var corpID = Context.store_profile_mst2.Find(c.store_id).corp_id;
			var plan_no = Context.corporate_mst2.Find(corpID).plan_id;
			//var plan_id = Context.plan_major_minor_mst2.Where(r => r.minor_plan_id == plan_no).FirstOrDefault().major_plan_id;
			return Convert.ToInt32(plan_no);
		}
	}
    public class shift_details_mst_helper{

        public void Addshift_details_mst (shiftreportapp.data.AppModel.shift_details_mst data)
        {
            AppModel Context = new AppModel();
            Context.shift_details_mst2.Add(data);
            Context.SaveChanges();
            Context.Dispose();
        }
        public shiftreportapp.data.AppModel.shift_details_mst Getshift_details_mst (int id)
        {
            AppModel Context = new AppModel();
        return (from x in Context.shift_details_mst2
                where x.id==id
                select x).FirstOrDefault();
        }
        public void setShiftDone(int shift_id){
            AppModel Context = new AppModel();
            var shiftm = Context.shift_details_mst2.Find(shift_id);
            shiftm.cash_counted = 0;
            shiftm.shift_status = "D";
            shiftm.Tip_Payout_Status = "PAYOUT_PENDING"; // Added By Adil
			//shiftm.shift_session_ended = MilitaryToDateTime(DateTime.Now);// Convert.ToInt32(DateTime.Now.ToString("hhmm"));
            Saveshift_details_mst(shiftm);
            var shiftp = Context.cashier_shift_pending_mst2.Where(r=>r.shift_id==shift_id).FirstOrDefault();
            shiftp.shift_status = "D";
            Context.Entry(shiftp).State = System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }
		public String getShiftStatus(int shift_id)
		{
			AppModel Context = new AppModel();
			return  Context.shift_details_mst2.Find(shift_id).shift_status;
			 
		}
        public void Saveshift_details_mst( shiftreportapp.data.AppModel.shift_details_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }
        public void Saveshift_details_mst (int id,shiftreportapp.data.AppModel.shift_details_mst data)
        {
            AppModel Context = new AppModel();
            Context.Entry(data).State=System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
            Context.Dispose();
        }
        public void Deleteshift_details_mst (shiftreportapp.data.AppModel.shift_details_mst data)
        {
            AppModel Context = new AppModel();
            Context.shift_details_mst2.Remove(data);
            Context.SaveChanges();
        }
        public void DoPostSubmit(string p)
        {
            AppModel Context = new AppModel();
            Context.Database.ExecuteSqlCommand("exec usp_sr_dopostshiftsubmit_V1 @shift_id="+p);
            Context.Dispose();
        }
    }
 	public class shift_store_times_Context 
		: DbContext
	{
		public shift_store_times_Context()
			: base("name=shiftreport")
		{
		}

		public virtual DbSet<shiftreportapp.data.AppModel.shift_store_times> shift_store_times { set; get; }
	}
	public class DB_shift_checklist_store_mst
	{




		public List<shiftreportapp.data.checklist_resulst> GetCheckList(int store_id)
		{
			AppModel Context = new AppModel();
			return(List<shiftreportapp.data.checklist_resulst>) Context.Database.SqlQuery<shiftreportapp.data.checklist_resulst>("exec usp_sr_getshiftid_checklist_full_bystore_V4 @store_id=" + store_id.ToString(), new Object[]{}).ToList();
		}
	}
	public class DB_shift_store_times
	{


		public virtual DbSet<shiftreportapp.data.AppModel.shift_store_times> shift_store_times { set; get; }

		public void Addshift_store_times(shiftreportapp.data.AppModel.shift_store_times data)
		{
			shift_store_times_Context Context = new shift_store_times_Context();
			Context.shift_store_times.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}


		public List<shiftreportapp.data.AppModel.shift_store_times> Getshift_store_times(int id)
        {
			shift_store_times_Context Context = new shift_store_times_Context();
            return (from x in Context.shift_store_times
                    where x.store_id==id
                    select x).ToList();
        }

		public void Saveshift_store_times(int id, shiftreportapp.data.AppModel.shift_store_times data)
		{
			shift_store_times_Context Context = new shift_store_times_Context();
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}

		public void Deleteshift_store_times(shiftreportapp.data.AppModel.shift_store_times data)
		{
			shift_store_times_Context Context = new shift_store_times_Context();
			Context.shift_store_times.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

	}
    public class DB_rackset_mst
	{


		

		public int Addrackset_mst(AppModel.rackset_mst data)
		{
			AppModel Context = new AppModel();
			Context.rackset_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
			return data.id;
		}


		public AppModel.rackset_mst[] Getrackset_mstByStoreID(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rackset_mst2
					where x.store_id == id 
					select x).ToArray();
		}

	


		public AppModel.rackset_mst Getrackset_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rackset_mst2
					where x.id == id
					select x).FirstOrDefault();
		}
		public void Saverackset_mst(int id, string title)
		{
			AppModel Context = new AppModel();
			var data = Context.rackset_mst2.Find(id);
			if (data == null)
				return;
			data.rack_title = data.rack_name = title;
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}



		


		public void Deleterackset_mst(AppModel.rackset_mst data)
		{
			AppModel Context = new AppModel();
			Context.rackset_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public void Deleterackset_mst(int id)
		{
			AppModel Context = new AppModel();
			var data = Context.rackset_mst2.Find(id);
			if (data == null)
				return;
			Context.rackset_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		
	}
	public class DB_section_mst
	{
		
		public int Addsection_mst(AppModel.section_mst data)
		{
			AppModel Context = new AppModel();
			Context.section_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
			return data.Id;
		}


		public AppModel.section_mst Getsection_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}

		public int Savesection_mst(int id,int rackset_id, Sections section_data,string section_type)
		{
			AppModel Context = new AppModel();
			var data = Context.section_mst2.Find(id);
			if (data != null)
			{
				data.section_title = data.section_name = section_data.Title;
				Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
				Context.Dispose();
			}
			else
			{
				if(id==-1)
				{
					AppModel.section_mst s = new AppModel.section_mst()
					{
						rackset_id=rackset_id,
						section_title = section_data.Title,
						section_name = section_data.Title,
						section_type = section_type,
						show_boarder = true
					};
					Context.section_mst2.Add(s);
					Context.SaveChanges();
					Context.Dispose();
					return s.Id;
				}
				else
				{
					int ID = Convert.ToInt32(section_data.Title);
					Deletesection_mst(ID);
					return 0;

				}
				

			}
			return data.Id;
		}

		public void Deletesection_mst(AppModel.section_mst data)
		{
			AppModel Context = new AppModel();
			if (data == null)
				return;
			Context.section_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public AppModel.section_mst[] GetAllsection_mst(int header_id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_mst2
					where x.Id == header_id
					select x).ToArray();
		}

		public AppModel.section_mst[] GetHeaderSections(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_mst2
					where x.rackset_id == id && (x.section_type.Equals("header") || x.section_type.Equals("top"))
					select x).ToArray();
		}

		public AppModel.section_mst[] GetFooterSections(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_mst2
					where x.rackset_id == id && ( x.section_type.Equals("footer") || x.section_type.Equals("buttom"))
					select x).ToArray();
		}

		public void Deletesection_mst(int id)
		{
			AppModel Context = new AppModel();
			var data = Context.section_mst2.Find(id);
			if (data == null)
				return;
			Context.section_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}
	}
	public class DB_section_elemnts_dtls
	{
		public void Addsection_elemnts_dtls(AppModel.section_elemnts_dtls data)
		{
			AppModel Context = new AppModel();
			Context.section_elemnts_dtls2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}


		public AppModel.section_elemnts_dtls Getsection_elemnts_dtls(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_elemnts_dtls2
					where x.Id == id
					select x).FirstOrDefault();
		}

		public void Savesection_elemnts_dtls(int id,int section_id, string uuid,string label)
		{

		

			AppModel Context = new AppModel();
			var data = Context.section_elemnts_dtls2.Find(id);
			if (data == null)
			{
				if(id==-1)
				{

					if (section_id == 0)
						return;

					AppModel.section_elemnts_dtls d = new AppModel.section_elemnts_dtls()
					{
						section_id=section_id,
						elem_uuid = uuid,
						elem_lable = label,
						elem_name = label
					};
					Context.section_elemnts_dtls2.Add(d);
					Context.SaveChanges();
					Context.Dispose();
				}
				else
				{
					int ID = Convert.ToInt32(label);
					Deletesection_elemnts_dtls(ID);

				}
				
			}
			else
			{
				data.elem_uuid = uuid;
				data.elem_name = data.elem_lable = label;
				Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
			}
		
			
		}

		public void Deletesection_elemnts_dtls(AppModel.section_elemnts_dtls data)
		{
			AppModel Context = new AppModel();
			Context.section_elemnts_dtls2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public AppModel.section_elemnts_dtls[] GetElementsForSection(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_elemnts_dtls2
					where x.section_id == id
					select x).ToArray();
		}

		public void Deletesection_elemnts_dtls(int id)
		{
			AppModel Context = new AppModel();
			var data = Context.section_elemnts_dtls2.Find(id);
			Context.section_elemnts_dtls2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}
	}
	public class DB_rack_names_mst
	{
		public void Addrack_names_mst(AppModel.rack_names_mst data)
		{
			AppModel Context = new AppModel();
			Context.rack_names_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}
		public AppModel.rack_names_mst Getrack_names_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_names_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}
		public AppModel.rack_names_mst Getrack_names_mst(int rackset_id,int rack_no,int row_no,int col_no)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_names_mst2
					where x.rack_id==rackset_id && x.rack_no==rack_no && x.row_no==row_no && x.col_no==col_no
					select x).FirstOrDefault();
		}
		public void Saverack_names_mst(int id, AppModel.rack_names_mst data)
		{
			AppModel Context = new AppModel();
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}
		public void Deleterack_names_mst(AppModel.rack_names_mst data)
		{
			AppModel Context = new AppModel();
			Context.rack_names_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}
		public product_label_dm[] Getrack_names_mst(int id, int rack_no)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_names_mst2
					where x.rack_id == id && x.rack_no == rack_no
					select new product_label_dm() { row_no=x.row_no,col_no=x.col_no,product_name=x.product_name }).ToArray();
		}
		public void Saverack_names_mst(int rackset_id, int rack_no, int col_no,int row_no, string product_name,int store_id)
		{
			AppModel Context = new AppModel();
			var data = (from x in Context.rack_names_mst2
						where x.rack_id== rackset_id && x.rack_no==rack_no && x.row_no==row_no && x.col_no==col_no
						select x).FirstOrDefault();
			if (data != null)
			{
				data.product_name = product_name;
				
				Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
				Context.Dispose();
			}
			else
			{
				data = new AppModel.rack_names_mst() {
					rack_id=rackset_id,
					row_no=row_no,
					col_no=col_no,
					rack_no=rack_no,
					product_name=product_name
				};
				Context.rack_names_mst2.Add(data);
				Context.SaveChanges();

				
				DB_rack_inventory_level_mst inv = new DB_rack_inventory_level_mst();
				inv.Addrack_inventory_level_mst(new AppModel.rack_inventory_level_mst()
				{
					rack_id = rackset_id,
					rack_no = rack_no,
					row_no = row_no,
					col_no = col_no,
					store_id = store_id
				});
			}
		}
		

		public void Deleterack_names_mst(int rackset_id, int rack_no, int col_no, int row_no)
		{
			AppModel Context = new AppModel();
			var data = (from x in Context.rack_names_mst2
						where x.rack_id == rackset_id && x.rack_no == rack_no && x.row_no == row_no && x.col_no == col_no
						select x).FirstOrDefault();
			
			Context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
			Context.SaveChanges();
			Context.Dispose();
		}
	}
	public class DB_section_elem_ans_mst
	{
		public void Addsection_elem_ans_mst(AppModel.section_elem_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.section_elem_ans_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public AppModel.section_elem_ans_mst Getsection_elem_ans_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.section_elem_ans_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}

		public void Savesection_elem_ans_mst(int id, AppModel.section_elem_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}

		public void Deletesection_elem_ans_mst(AppModel.section_elem_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.section_elem_ans_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public string Getsection_elem_ans_mst(int shift_id, int id)
		{
			AppModel Context = new AppModel();
			var data = (from x in Context.section_elem_ans_mst2
						where x.elem_id == id && x.shift_id == shift_id
						select x).FirstOrDefault();
			if (data != null)
				return data.elem_val;
			else
				return "";
		}
	}
	public class DB_rack_mst
	{
		public int Addrack_mst(AppModel.rack_mst data)
		{
			AppModel Context = new AppModel();
			Context.rack_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();

			return data.Id;
		}


		public AppModel.rack_mst Getrack_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}

		public int Saverack_mst(int id, AppModel.rack_mst D)
		{
			AppModel Context = new AppModel();
			var data = Context.rack_mst2.Find(id);
			if (data == null)

			{
				if(id==-1)
				{
					data = new AppModel.rack_mst()
					{
						rackset_id = D.rackset_id,
						rack_row = D.rack_row,
						rock_col = D.rock_col,
						rack_name = D.rack_name,
						show_added = D.show_added,
						show_ended = D.show_ended,
						show_started = D.show_started,
						show_product_label = D.show_product_label,
					};
					Context.rack_mst2.Add(data);
					Context.SaveChanges();
					Context.Dispose();

					return data.Id;
				}

				else
				{
					int ID = Convert.ToInt32(D.rack_name);
					Deleterack_mst(ID);
					return -2;
				}
				
				
			
			}
			else
			{
				data.rack_row = D.rack_row;
				data.rock_col = D.rock_col;
				data.rack_name = D.rack_name;
				data.show_added = D.show_added;
				data.show_ended = D.show_ended;
				data.show_started = D.show_started;
				data.show_product_label = D.show_product_label;
				Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
				Context.Dispose();
			}
			return data.Id;
		}

		public void Deleterack_mst(int id)
		{
			AppModel Context = new AppModel();
			AppModel.rack_mst data = Context.rack_mst2.Find(id);
			if (data == null)
				return;
			Context.rack_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public AppModel.rack_mst[] GetAllRacks(int racks_id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_mst2
					where x.Id == racks_id
					select x).ToArray();
		}

		public AppModel.rack_mst[] GetAllRacksFoRackSet(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_mst2
					where x.rackset_id == id
					select x).ToArray();
		}

		internal string getProcuctLabel(int id, int rack_no, int rack_col, int rack_row)
		{
			AppModel Context = new AppModel();
			var d=(from x in Context.rack_mst2
					where  x.rackset_id == id && x.rack_no==rack_no && x.rack_row==rack_row && x.rock_col==rack_col
					select x).FirstOrDefault();
			return d==null?"":d.rack_name;
		}
	}
	public class DB_rack_ans_mst
	{
		public void Addrack_ans_mst(AppModel.rack_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.rack_ans_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}
		public AppModel.rack_ans_mst Getrack_ans_mst(int id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_ans_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}
		public void Saverack_ans_mst(int id, AppModel.rack_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}
		public void Deleterack_ans_mst(AppModel.rack_ans_mst data)
		{
			AppModel Context = new AppModel();
			Context.rack_ans_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}
		public AppModel.rack_ans_mst Getrack_ans_mst(int rackset_id,int rack_no,int row_no,int col_no,int shift_id)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_ans_mst2
					where x.shift_id==shift_id && x.rackset_id==rackset_id && x.rack_no==rack_no && x.row_no==row_no && x.col_no==col_no
					select x).FirstOrDefault();
		}

		public List<AppModel.rack_ans_mst_resp> Getrack_ans_mstByRackID(int id)
		{
			AppModel Context = new AppModel();

			var y=(from x in Context.rack_ans_mst2
					where x.rackset_id == id
					select new AppModel.rack_ans_mst_resp() {
						row_no=x.row_no,
						col_no=x.col_no,
						added_value=x.rack_added_value,
						ended_value=x.rack_ended_value,
						started_value=x.rack_started_value,
						product_label=""
					}).ToList();

			return y.ToList();
		}

		public List<AppModel.rack_ans_mst> Getrack_ans_mstByShiftId(int shift_id,int rack_no)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_ans_mst2
					where x.shift_id == shift_id && x.rack_no == rack_no
					select x).ToList();
		}

		
	}
	public class DB_rack_template_mst
	{


		AppModel Context = new AppModel();

		public void Addrack_template_mst(rack_template_mst data)
		{
			
			Context.rack_template_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}


		public rack_template_mst Getrack_template_mst(int id)
		{
			
			return (from x in Context.rack_template_mst2
					where x.Id == id
					select x).FirstOrDefault();
		}

		public void Saverack_template_mst(int id, rack_template_mst data)
		{
			
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}

		public void Deleterack_template_mst(rack_template_mst data)
		{
			
			Context.rack_template_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public int[] GetAll()
		{
			return (from x in Context.rack_template_mst2
					select x.Id).ToArray();
		}
	}
	public class DB_rackset_diff_mst
	{

		AppModel Context = new AppModel();
		public void Addrackset_diff_mst(AppModel.rackset_diff_mst data)
		{
			
			
			Context.rackset_diff_mst2.Add(data);
			Context.SaveChanges();
			Context.Dispose();
		}


		public AppModel.rackset_diff_mst Getrackset_diff_mst(int id)
		{
			
			return (from x in Context.rackset_diff_mst2
					where x.rackset_id == id
					select x).FirstOrDefault();
		}

		public void Saverackset_diff_mst(int id, AppModel.rackset_diff_mst data)
		{
			
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}

		public void Deleterackset_diff_mst(AppModel.rackset_diff_mst data)
		{
			
			Context.rackset_diff_mst2.Remove(data);
			Context.SaveChanges();
			Context.Dispose();
		}

		public AppModel.rackset_diff_mst[] Getrackset_diff_mstForStore(int id)
		{
			return (from x in Context.rackset_diff_mst2
					where x.store_id == id
					select x).ToArray();
		}
	}
	public class DB_rack_inventory_level_mst
{

	public void Addrack_inventory_level_mst(AppModel.rack_inventory_level_mst data)
	{
		AppModel Context = new AppModel();
		Context.rack_inventory_level_mst2.Add(data);
		Context.SaveChanges();
		Context.Dispose();
	}


	public AppModel.rack_inventory_level_mst Getrack_inventory_level_mst(int id)
	{
		AppModel Context = new AppModel();
		return (from x in Context.rack_inventory_level_mst2
				where x.Id == id
				select x).FirstOrDefault();
	}

		public AppModel.rack_inventory_level_mst Getrack_inventory_level_mst(int rack_id,int rack_no,int row_no,int col_no)
		{
			AppModel Context = new AppModel();
			return (from x in Context.rack_inventory_level_mst2
					where x.rack_id == rack_id && x.rack_no== rack_no && x.row_no == row_no && x.col_no== col_no
					select x).FirstOrDefault();
		}

		public void Saverack_inventory_level_mst(int id, AppModel.rack_inventory_level_mst data)
	{
		AppModel Context = new AppModel();
		Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
		Context.SaveChanges();
		Context.Dispose();
	}
	public void Deleterack_inventory_level_mst(AppModel.rack_inventory_level_mst data)
	{
		AppModel Context = new AppModel();
		Context.rack_inventory_level_mst2.Remove(data);
		Context.SaveChanges();
		Context.Dispose();
	}
	public List<AppModel.rack_inventory_level_mst> GetAllForRack_id(int id)
	{
		AppModel Context = new AppModel();
		return (from x in Context.rack_inventory_level_mst2
				where x.rack_id == id
				select x).ToList();
	}
	public void Saverack_inventory_level_mst(add_new_rack_store_collection_dm_items D)
	{
		AppModel Context = new AppModel();
		var data = (from x in Context.rack_inventory_level_mst2
					where x.rack_id == D.rack_id && 
					x.rack_no == D.rack_no && 
					x.row_no == D.row_no && 
					x.col_no == D.col_no &&
					x.store_id == D.store_Id
					select x).FirstOrDefault();
		if (data != null)
		{
			data.inventory_level_amt = D.inventory_level_amt;
			Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
			Context.SaveChanges();
			Context.Dispose();
		}
			else
			{
				data = new AppModel.rack_inventory_level_mst()
				{
					rack_id = D.rack_id,
					rack_no = D.rack_no,
					row_no = D.row_no,
					col_no = D.col_no,
					store_id = D.store_Id,
					inventory_level_amt=D.inventory_level_amt
					
				};

				Context.rack_inventory_level_mst2.Add(data);
				Context.SaveChanges();
				Context.Dispose();

			}

		}

	internal void SaveNewrack_inventory_level_mst(add_new_rack_store_collection_dm_items D)
	{
		AppModel Context = new AppModel();
		AppModel.rack_inventory_level_mst d = new AppModel.rack_inventory_level_mst();
		d.col_no = D.col_no;
		d.row_no = D.row_no;
		d.store_id = D.row_no;
		d.inventory_level_amt = D.inventory_level_amt;
		d.product_name = D.product_name;
		d.store_id = D.store_Id;
		Context.rack_inventory_level_mst2.Add(d);
		Context.SaveChanges();
	}

	internal void SaveNewrack_inventory_level_mst(int rack_id, int rack_no, int col_no, int row_no, string product_name, int store_id)
	{
		AppModel Context = new AppModel();
		AppModel.rack_inventory_level_mst d = new AppModel.rack_inventory_level_mst();
		d.col_no = col_no;
		d.row_no = row_no;
		d.rack_id = rack_id;
		d.rack_no = rack_no;
		d.product_name = product_name;
		d.store_id = store_id;
		Context.rack_inventory_level_mst2.Add(d);
		Context.SaveChanges();
	}

	internal void UpdateLabelname(int rackset_id, int store_id, int rack_no, int row_no, int col_no, string v, string product_name)
	{
		AppModel Context = new AppModel();
		var x = (from d in Context.rack_inventory_level_mst2
				 where
					d.col_no == col_no &&
					d.row_no == row_no &&
					d.rack_id == rackset_id &&
					d.rack_no == rack_no &&
					d.product_name == product_name &&
					d.store_id == store_id
				 select d).FirstOrDefault();

		x.product_name = x.product_name;
		Context.Entry(x).State = System.Data.Entity.EntityState.Modified;
		Context.SaveChanges();
	}


}

}
 