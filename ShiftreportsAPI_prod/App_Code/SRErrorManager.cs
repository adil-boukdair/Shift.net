using System;
using System.Linq;
using  shiftreportapp.data;


public class SRErrorManager
	{
		public static String GetErrorFromMessage(String errorCode)
		{
		
			AppModel Context = new AppModel();
			string err_html = Context.error_code_mst2.Where(r => r.err_code.Equals(errorCode)).FirstOrDefault().err_html;
			return err_html;

		}
	}
