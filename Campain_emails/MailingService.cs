using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net.Mime;
using System.Net;



namespace BiomedicaLib.Net
{
    public class MailLogService
    {

        /// <summary>
        /// Send a simple Mail to a recipiant
        /// </summary>
        /// <param name="subjectmessage">Subject of the message</param>
        /// <param name="bodymessage">The body of the message</param>
        public static void SendMail(string senderemail,string reciveremail,string subjectmessage, string bodymessage)
        {
			if (reciveremail.Equals("") && senderemail.Equals(""))
				throw new Exception(" The Mail Source and Distination is not set.");

			MailMessage theMail = new System.Net.Mail.MailMessage(senderemail, reciveremail);

			theMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

			theMail.Subject = subjectmessage;
			theMail.Body = bodymessage;
			theMail.IsBodyHtml = true;

		
			SmtpClient client = new SmtpClient();
			client.Host = System.Configuration.ConfigurationSettings.AppSettings["emailhost"];// "172.31.29.141";
			
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.Port = 25;

			
			
			client.Send(theMail);
		}

       

        



       

    }
}
