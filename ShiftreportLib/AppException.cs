using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShiftreportLib
{
    public class AppException : Exception
    {
        public AppException(int userid,String msg)
        {
            this.ID = userid;
            this.Msg = msg;
        }
        public AppException(string userid, String msg)
        {
            this.UN = userid;
            this.Msg = msg;
        }

        public int ID { get; set; }

        public string Msg { get; set; }

        public string UN { get; set; }
    }
	public class AppErrorException : Exception
	{
		public int ErrorId { set; get; }
		public String ErrorDisc { set; get; }
		public AppErrorException(int id, string dis)
		{
			ErrorId = id;
			ErrorDisc = dis;
		}
	}
}
