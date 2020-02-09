using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpensTrackerAPI.App_Code
{
    public class MapShiftToTime
    {
        public static void MapShiftNoToText(int shift_no, out string start_shift, out string close_shift)
        {

            start_shift = "";
            close_shift = "";
            switch (shift_no)
            {
                case (1):
                    start_shift = "7am";
                    close_shift = "3pm";
                    break;
                case (2):
                    start_shift = "3pm";
                    close_shift = "11pm";
                    break;
                case (3):
                    start_shift = "11pm";
                    close_shift = "7am";
                    break;
            }



        }
    }

  
}