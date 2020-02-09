using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExpenseTrackerLib.Rest
{
   
	public class ShiftWebsqlDtl
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int cigs_num_rack { get; set; }
		public int cigs_rack_columns { get; set; }
		public int cigs_rack_rows { get; set; }
		public int lotto_num_racks { get; set; }
		public int lotto_rack_columns { get; set; }
		public int lotto_rack_rows { get; set; }
		public int shift_no { get; set; }
		public float shift_opened { get; set; }
		public float shift_closed { get; set; }
	}

	public class LottoRackDtl
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int rack_no { get; set; }
		public int rack_row { get; set; }
		public int rack_col { get; set; }
		public int open_amnt { get; set; }
		public int added_amnt { get; set; }
		public int close_amnt { get; set; }
	}

	public class CigRackDtl
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int rack_no { get; set; }
		public int row_no { get; set; }
		public int tray_no { get; set; }
		public int open_value { get; set; }
		public int close_value { get; set; }
	}

	public class ShiftChecklistMst
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public string q_uuid { get; set; }
		public string q_answer { get; set; }
		public int shift_no { get; set; }
	}

	public class PhoneCard
	{
		public int _id { get; set; }
		public int phone_begin_1 { get; set; }
		public int phone_begin_2 { get; set; }
		public int phone_begin_3 { get; set; }
		public int phone_begin_4 { get; set; }
		public int phone_added_5 { get; set; }
		public int phone_added_6 { get; set; }
		public int phone_added_7 { get; set; }
		public int phone_added_8 { get; set; }
		public int hone_ended_9 { get; set; }
		public int phone_ended_10 { get; set; }
		public int phone_ended_11 { get; set; }
		public int phone_ended_12 { get; set; }
		public int shift_id { get; set; }
	}

	public class LottoBinDtl
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public string open_close_flag { get; set; }
		public string uuid { get; set; }
		public double bin_amnt { get; set; }
	}

	public class DrawerOpenMst
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int fifties_open { get; set; }
		public int twenties_open { get; set; }
		public int tens_open { get; set; }
		public int fives_open { get; set; }
		public int singles_open { get; set; }
		public int dollars_open { get; set; }
		public int quarters_open { get; set; }
		public int dimes_open { get; set; }
		public int nickels_open { get; set; }
		public int rolled_quarters_open { get; set; }
		public int rolled_dimes_open { get; set; }
		public int rolled_nickels_open { get; set; }
		public int rolled_pennies_open { get; set; }
		public int pennies_open { get; set; }
	}

	public class DrawerCloseMst
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int tens_close { get; set; }
		public int fives_close { get; set; }
		public int singles_close { get; set; }
		public int dollars_close { get; set; }
		public int quarters_close { get; set; }
		public int dimes_close { get; set; }
		public int nickels_close { get; set; }
		public int pennies_close { get; set; }
		public int rolled_quarters_close { get; set; }
		public int rolled_dimes_close { get; set; }
		public int rolled_nickels_close { get; set; }
		public int rolled_pennies_close { get; set; }
		public int cash_drawer_close { get; set; }
		public int fifties_close { get; set; }
		public int twenties_close { get; set; }
	}

	public class CigRackrowtrayOpenMst
	{
		public int _id { get; set; }
		public int rack_no { get; set; }
		public int shift_id { get; set; }
		public string uuid { get; set; }
	}

	public class CigRackrowtrayCloseMst
	{
		public int _id { get; set; }
		public int rack_no { get; set; }
		public string uuid { get; set; }
		public int shift_id { get; set; }
	}

	public class CartonsOfCigarret
	{
		public int _id { get; set; }
		public int shift_id { get; set; }
		public int carton_begin { get; set; }
		public int carton_added { get; set; }
		public int carton_ended { get; set; }
	}

	public class JSONReport
	{
		
		public List<ShiftWebsqlDtl> shift_websql_dtls { get; set; }
		
		public List<object> shift_cigarit_dtl { get; set; }
		
		public List<LottoRackDtl> lotto_rack_dtls { get; set; }
		public List<CigRackDtl> cig_rack_dtl { get; set; }
		public List<ShiftChecklistMst> shift_checklist_mst { get; set; }
		public List<PhoneCard> phone_cards { get; set; }
		public List<LottoBinDtl> lotto_bin_dtls { get; set; }
		public List<DrawerOpenMst> drawer_open_mst { get; set; }
		public List<DrawerCloseMst> drawer_close_mst { get; set; }
		public List<CigRackrowtrayOpenMst> cig_rackrowtray_open_mst { get; set; }
		public List<CigRackrowtrayCloseMst> cig_rackrowtray_close_mst { get; set; }
		public List<CartonsOfCigarret> cartons_of_cigarrets { get; set; }
	}




   
}
