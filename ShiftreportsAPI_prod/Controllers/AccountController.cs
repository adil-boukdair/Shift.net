using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiomedicaLib.Net;
using ExpensTrackerAPI.Controllers;
using shiftreportapp.data;
using ShiftreportLib;
using static ShiftreportLib.internal_restful_dm;

namespace ShiftReportApi.Controllers
{
    public class AccountController : ApiController
    {
		String html_root_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_root_path"];
			}
		}


		String html_folder_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_folder_path"];
			}
		}

		String S3_path
		{
			get
			{
				return html_root_path + html_folder_path;
			}
		}

		AppModel Context = new AppModel();
		private object externalLogin;

		[HttpGet]
		public HttpResponseMessage getUserNameId([FromUri] String Email)
		{
			try
			{

				var c = Context.vc_customers2.Where(r => r.UserName.Equals(Email)).FirstOrDefault();
				if (c == null)
					throw new AppException(0, "Now user with that username");
					

				
				return Request.CreateResponse(HttpStatusCode.OK, c.id);
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage GetUserInfo([FromUri] string cutomer_id)
		{
			try
			{
				int id = Convert.ToInt32(cutomer_id);
				var currentUser = Context.vc_customers2.Find(id);
                return Request.CreateResponse(HttpStatusCode.OK, new UserInfoViewModel
                {
                    Email = currentUser.Email,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    TotalVenueCash = currentUser.TotalVenueCash,
                    TotalAllCash = currentUser.TotalAllCash,
                    totalEarned = Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == id).Sum(o => o.VenueCashEarned_Total) ?? default(double),
                    Customer_Profile_Pic = currentUser.Customer_Profile_Pic,
                    customerTransactionsCount = Context.vc_CustomerTransactions2.Where(t => t.CustomerId == id).Count()
                });
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage Signup(RegisterBindingModel model)
		{
			try
			{
				var v = Context.vc_customers2.Where(r => r.Email.Equals(model.Email)).FirstOrDefault();
				if(v!=null)
				{
					return Request.CreateResponse(HttpStatusCode.OK, -1);
				}

				var c = new vc_customers()
				{
					create_dt = DateTime.Now,
					modify_dt = DateTime.Now,
					created_userid = "system",
					Modify_usrid = "system",
					UserName = model.Email,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Pin = model.Pin,
					PasswordHash=model.Password,
					TotalAllCash = 0,
					TotalVenueCash = 0,
                    Customer_Profile_Pic=model.Customer_Profile_Pic
				};
				Context.vc_customers2.Add(c);
				Context.SaveChanges();

                // Send Vc email
                VenueCashEmails.newCustomer(c.Email,c.FirstName);

                
				return Request.CreateResponse(HttpStatusCode.OK, new { customer_id = c.id });
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}
        public HttpResponseMessage SocialSignup(RegisterBindingModel model)
        {
            try
            {
                var v = Context.vc_customers2.Where(r => r.Email.Equals(model.Email)).FirstOrDefault();
                if (v != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }

                var c = new vc_customers()
                {
                    create_dt = DateTime.Now,
                    modify_dt = DateTime.Now,
                    created_userid = "system",
                    Modify_usrid = "system",
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Pin = model.Pin,
                    SecurityStamp = model.Password,
                    TotalAllCash = 0,
                    TotalVenueCash = 0,
                    Customer_Profile_Pic = model.Customer_Profile_Pic
                };
                Context.vc_customers2.Add(c);
                Context.SaveChanges();

                // Send Vc email
                VenueCashEmails.newCustomer(c.Email, c.FirstName);


                return Request.CreateResponse(HttpStatusCode.OK, new { customer_id = c.id });
            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
            }
            catch (Exception err)
            {
                if (err.InnerException == null)
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
                }
                else
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPost]
		public HttpResponseMessage login(LoginBindingModel model)
		{
			try
			{
				var c = Context.vc_customers2.Where(r => r.UserName.Equals(model.UserName) && r.PasswordHash.Equals(model.Password) && r.Account_Deactivated!=1).FirstOrDefault();
				if(c!=null)
				{
                    bool isPinSet = false;
                    if (String.IsNullOrEmpty(c.Pin))
                    {
                        isPinSet = false;
                    }
                    else
                    {
                        isPinSet = true;
                    }
                   
                    return Request.CreateResponse(HttpStatusCode.OK, new { customer_id = c.id ,
                        isPinSet = isPinSet,
                        Customer_Profile_Pic =c.Customer_Profile_Pic,
                        FirstName=c.FirstName,
                        LastName=c.LastName,
                        Email=c.Email});
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, "-1");
				}
				
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}


        [HttpPost]
        public HttpResponseMessage SocialLogin(LoginBindingModel model)
        {
            try
            {
                var c = Context.vc_customers2.Where(r => r.UserName.Equals(model.UserName) && r.SecurityStamp.Equals(model.Password) && r.Account_Deactivated != 1).FirstOrDefault();
                if (c != null)
                {
                    bool isPinSet = false;
                    if (String.IsNullOrEmpty(c.Pin))
                    {
                        isPinSet = false;
                    }
                    else
                    {
                        isPinSet = true;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        customer_id = c.id,
                        isPinSet = isPinSet,
                        Customer_Profile_Pic = c.Customer_Profile_Pic,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "-1");
                }

            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
            }
            catch (Exception err)
            {
                if (err.InnerException == null)
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
                }
                else
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage isPasswordSet( [FromUri] int customerID)
        {
            try
            {

                var customer = Context.vc_customers2.Find(customerID);

                if (customer.PasswordHash != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, true);

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }


            }



            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }

        }


        [HttpPost]
		public HttpResponseMessage ChangeFirstNameLastName(ChangeFirstNameLastNameModel model)
		{
			try
			{
				var id = Convert.ToInt32(model.Customer_id);
				var c = Context.vc_customers2.Find(id);
				c.FirstName = model.FirstName;
				c.LastName = model.LastName;
				Context.Entry(c).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, new { success="1"});
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage ChangeEmail(ChangeEmailModel model)
		{
			try
			{

                var customer = Context.vc_customers2.Find(model.Customer_id);

                // check if new email doesnt exist in db
                var emails = Context.vc_customers2.Where(e => e.Email == model.Email).FirstOrDefault();
                if (emails != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }
                else // doesnt exist modify email and username
                {
                    customer.Email = model.Email;
                    Context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK,0);
                }

			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage ChangePassword(ChangePasswordModel model)
		{
			try
			{

                var customer = Context.vc_customers2.Find(model.Customer_id);

         
                    if(customer.PasswordHash != model.currentPassword)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -1);
                    }
                else
                {
                    customer.PasswordHash = model.Password;
                    Context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, 0);

                }
       
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

 
		[HttpPost]
		public HttpResponseMessage ChangePin(ChangePinModel model)
		{
			try
			{
                var customer = Context.vc_customers2.Find(model.Customer_id);

                if (customer.Pin != model.OldPin)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }
                else
                {
                    customer.Pin = model.Pin;
                    Context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK,0);
                }

			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}


	}
}
