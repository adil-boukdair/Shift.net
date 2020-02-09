using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiftreportapp.Rest
{
    public class AddnewShiftDM
    {
        public int store_id { set; get; }
        public string username { get; set; }
        public string password { set; get; }

    }
    public class UpdatenewShiftDM
    {
        public string username { get; set; }
        public string password { set; get; }
    }
}
