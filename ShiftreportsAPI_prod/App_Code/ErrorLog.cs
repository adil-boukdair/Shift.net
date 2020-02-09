using System;
using System.Text;

using shiftreportapp.data;

namespace LinkedliveWebLib.Error
{
	public class ErrorLog
    {
        public static void WriteToErrorLog(String message, Exception ex)
        {

            AppModel Context = new AppModel();
            StringBuilder Str = new StringBuilder();
            if(ex.InnerException!=null)
            foreach (var k in ex.StackTrace)
            {
                Str.Append(k.ToString());
                
            }
         //   Context.Database.ExecuteSqlCommand("Insert into program_error_log (err_message,err_stacktrace,dateandtime) values('"+message+"','"+Str.ToString()+"',getdate())");
        }
        public static void WriteToErrorLog( Exception ex)
        {
            String message = ex.Source;
            AppModel Context = new AppModel();
            StringBuilder Str = new StringBuilder();
            if (ex.InnerException != null)
                foreach (var k in ex.StackTrace)
                {
                    Str.Append(k.ToString());

                }
          //  Context.Database.ExecuteSqlCommand("Insert into program_error_log (err_message,err_stacktrace,dateandtime) values('" + message + "','" + Str.ToString() + "',getdate())");
        }
        public static void WriteToErrorLog(string p)
        {
            AppModel Context = new AppModel();
            Context.Database.ExecuteSqlCommand("Insert into program_error_log (err_message,err_stacktrace,dateandtime) values('info','" + p.ToString() + "',getdate())");
        }
    }
}
