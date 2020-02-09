using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BiomedicaLib.Net;
using ExpensTrackerAPI.App_Code;
using ExpensTrackerAPI.Controllers;
using ShiftReportApi.App_Code;
using shiftreportapp.data;
using ShiftreportLib;
using ShiftreportLib.Helpers;
using SRYelpAPI;
using static ShiftreportLib.internal_restful_dm;
using PayPal.Api;
using System.Data.Entity.SqlServer;

namespace ShiftReportApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerController : ApiController
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


        // Added by ADIL
        [HttpGet]
        public HttpResponseMessage searchBusinesses([FromUri] int customerId, [FromUri] float latitude, [FromUri] float longitude, [FromUri] string term, [FromUri] String location, [FromUri] String categories, [FromUri] int sort_by, [FromUri] int offset)
        {
            try
            {
                //sort_by 0=rewards,1=distance,2=venueCash

                    var yelpBusinesses = YelpV3.Search(customerId, latitude, longitude, term, location, categories, sort_by, offset);
 

                List<SearchBusinessBindingModel> businessesList = new List<SearchBusinessBindingModel>();

                
                // Getting VC business from DB
                AppModel Context2 = new AppModel();

  
                var venueCashBusinesses = (from x in Context.vc_store_profile_mst2
                                           from y in Context.store_profile_mst2
                                           where x.store_id == y.id
                                           select new { x, y }).AsQueryable();


                // Filter by Term
                if (term != null)
                {
                    venueCashBusinesses = venueCashBusinesses.Where(o =>  SqlFunctions.PatIndex("%"+term+"%", o.y.store_name) > 0);
                }
                // Filter by Category name
                if (categories != null)
                {
                    venueCashBusinesses = venueCashBusinesses.Where(o => SqlFunctions.PatIndex("%" + categories + "%", o.x.Category_Name) > 0);
                }
                // Filter by Location
                if (location != null)
                {
                    venueCashBusinesses = venueCashBusinesses.Where(o => SqlFunctions.PatIndex("%" + location + "%", o.y.store_city) > 0);
                }



                // VenueCash Businesses
                foreach (var venueCashBusiness in venueCashBusinesses)
                {

                    SearchBusinessBindingModel business = new SearchBusinessBindingModel();
                     
                    business.distance = YelpV3.DistanceTo(latitude,longitude, (double)venueCashBusiness.x.latitude, (double)venueCashBusiness.x.longitude);
                    business.rating = venueCashBusiness.x.Rating;
                    business.image_url = venueCashBusiness.x.image_url;
                    business.is_closed = false;
                    business.latitude = (double)venueCashBusiness.x.latitude;
                    business.longitude = (double)venueCashBusiness.x.longitude;
                    business.review_count = (venueCashBusiness.x.review_count==null) ? 0:(int)venueCashBusiness.x.review_count;
                    business.store_address1 = venueCashBusiness.y.store_address1;
                    business.store_city = venueCashBusiness.y.store_city;
                    business.store_country = venueCashBusiness.y.store_country;
                    business.price = venueCashBusiness.x.price;
                    business.categories = venueCashBusiness.x.Category_Name;
                    business.store_name = venueCashBusiness.y.store_name;
                    business.store_phone_no = venueCashBusiness.y.store_phone_no;
                    business.store_state = venueCashBusiness.y.store_state;
                    business.store_zip = venueCashBusiness.y.store_zip.ToString();
                    business.Website = venueCashBusiness.x.website;
                    if (venueCashBusiness.x.yelp_store_id == null)
                    {
                        business.yelp_store_id = venueCashBusiness.x.Id.ToString();
                    }
                    else
                    {
                        business.yelp_store_id = venueCashBusiness.x.yelp_store_id;
                    }
                    
                    business.store_id = venueCashBusiness.x.store_id;

                    // data Related to VC 
                    business.id = -1;
                    business.GasCashBackRate = (double)venueCashBusiness.x.gas_back_rate;
                    business.OtherCashBackRate = (double)venueCashBusiness.x.cash_back_rate;
                    business.IsGasStation = (bool)venueCashBusiness.x.is_gas_station;
                    business.IsVenueCashBusiness = true;


                    // get rewards if CustomerID !=null
                    if (customerId !=-1)
                    {
                     var customerRewards = Context2.vc_Customer_Rewards_Balances2.Where(o => o.store_id == venueCashBusiness.x.store_id && o.CustomerId==customerId).FirstOrDefault();
                        if (customerRewards == null)
                            business.VenueCash__Available_balance =0 ;
                        else
                            business.VenueCash__Available_balance = (double)customerRewards.VenueCash__Available_balance;
                    }
                    else
                    {
                        business.VenueCash__Available_balance = -1;
                    }

                    // add venueCashBusiness to list only if offset equal to 1 // this need to be fixed when we will have more venueCash business
                    if(offset==1)businessesList.Add(business);

                }



                // Yelp Businesses
                foreach (var yelpBusiness in yelpBusinesses.businesses)
                {
                    // Skip existing Yelp business implemented as VC businesses
                    if(venueCashBusinesses.Any(o=>o.x.yelp_store_id== yelpBusiness.id))
                    {
                        continue;
                    }


                    SearchBusinessBindingModel business = new SearchBusinessBindingModel();

                    business.distance = yelpBusiness.distance / 1609.34;// convert to miles
                    business.rating = yelpBusiness.rating;
                    business.image_url = yelpBusiness.image_url;
                    business.is_closed = yelpBusiness.is_closed;
                    business.latitude = yelpBusiness.coordinates.latitude ?? default(double);
                    business.longitude = yelpBusiness.coordinates.longitude ?? default(double);
                    business.review_count = yelpBusiness.review_count;
                    business.store_address1 = yelpBusiness.location.address1;
                    business.store_city = yelpBusiness.location.city;
                    business.store_country = yelpBusiness.location.country;
                    business.price = yelpBusiness.price;
                    business.categories = yelpBusiness.categories[0].title;
                    business.store_name = yelpBusiness.name;
                    business.store_phone_no = yelpBusiness.phone;
                    business.store_state = yelpBusiness.location.state;
                    business.store_zip = yelpBusiness.location.zip_code;
                    business.Website = yelpBusiness.url;
                    business.yelp_store_id = yelpBusiness.id;

                    // data Related to VC
                    business.id = -1;
                    business.GasCashBackRate = -1;
                    business.OtherCashBackRate = -1;
                    business.IsGasStation = false;
                    business.IsVenueCashBusiness = false;
                    business.store_id = -1;
                    business.VenueCash__Available_balance = -1;

                    

                    // add Yelp Businesses to list
                    businessesList.Add(business);
                }



                // sort only if term==null
                if (term == null) { 
                // Sorting
                if (sort_by == 0)// By Rewards
                {
                    businessesList = businessesList.OrderByDescending(o => o.OtherCashBackRate).ToList();
                }
                else if(sort_by==1)// by distance
                {
                    businessesList = businessesList.OrderBy(o => o.distance).ToList();
                }
                else if (sort_by == 2) // By VenueCash
                {
                    businessesList = businessesList.OrderByDescending(o => o.VenueCash__Available_balance).ToList();
                }
                }






                return Request.CreateResponse(HttpStatusCode.OK, businessesList);
            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
            }
            catch (Exception err)
            {
                if (err.InnerException == null)
                {
                  //  MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
                }
                else
                {
                   // MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage VCBusinessProfile([FromUri] int store_id, [FromUri]  int customerID, [FromUri] double customerLat, [FromUri] double customerLong)
        {


            var venueCashBusiness = (from x in Context.vc_store_profile_mst2
                                       from y in Context.store_profile_mst2
                                       where x.store_id == y.id
                                       where x.store_id== store_id
                                     select new { x, y }).FirstOrDefault();

            SearchBusinessBindingModel business = new SearchBusinessBindingModel();

            business.distance = YelpV3.DistanceTo(customerLat, customerLong, (double)venueCashBusiness.x.latitude, (double)venueCashBusiness.x.longitude);
            business.rating = venueCashBusiness.x.Rating;
            business.image_url = venueCashBusiness.x.image_url;
            business.is_closed = false;
            business.latitude = (double)venueCashBusiness.x.latitude;
            business.longitude = (double)venueCashBusiness.x.longitude;
            business.review_count = (venueCashBusiness.x.review_count == null) ? 0 : (int)venueCashBusiness.x.review_count;
            business.store_address1 = venueCashBusiness.y.store_address1;
            business.store_city = venueCashBusiness.y.store_city;
            business.store_country = venueCashBusiness.y.store_country;
            business.price = venueCashBusiness.x.price;
            business.categories = venueCashBusiness.x.Category_Name;
            business.store_name = venueCashBusiness.y.store_name;
            business.store_phone_no = venueCashBusiness.y.store_phone_no;
            business.store_state = venueCashBusiness.y.store_state;
            business.store_zip = venueCashBusiness.y.store_zip.ToString();
            business.Website = venueCashBusiness.x.website;
            if (venueCashBusiness.x.yelp_store_id == null)
            {
                business.yelp_store_id = venueCashBusiness.x.Id.ToString();
            }
            else
            {
                business.yelp_store_id = venueCashBusiness.x.yelp_store_id;
            }

            business.store_id = venueCashBusiness.x.store_id;

            // data Related to VC 
            business.id = -1;
            business.GasCashBackRate = (double)venueCashBusiness.x.gas_back_rate;
            business.OtherCashBackRate = (double)venueCashBusiness.x.cash_back_rate;
            business.IsGasStation = false;
            business.IsVenueCashBusiness = true;


            // yelp data
            business.yelp_store_id = venueCashBusiness.x.yelp_store_id;


            // get rewards if CustomerID !=null
            if (customerID != -1)
            {
                var customerRewards = Context.vc_Customer_Rewards_Balances2.Where(o => o.store_id == venueCashBusiness.x.store_id && o.CustomerId == customerID).FirstOrDefault();
                if (customerRewards == null)
                    business.VenueCash__Available_balance = 0;
                else
                    business.VenueCash__Available_balance = (double)customerRewards.VenueCash__Available_balance;
            }
            else
            {
                business.VenueCash__Available_balance = -1;
            }


            return Request.CreateResponse(HttpStatusCode.OK, business);

        }


        [HttpGet]
        public HttpResponseMessage isVenueCashBusiness([FromUri] string yelp_id,[FromUri] int customerID ,[FromUri] double yelpLat, [FromUri] double yelpLong, [FromUri] double customerLat, [FromUri] double customerLong)
        {
            try
            {

                var vcStoreProfile = Context.vc_store_profile_mst2.Where(o => o.yelp_store_id == yelp_id).FirstOrDefault();

                if (vcStoreProfile != null)
                {

                double VenueCash__Available_balance = 0;

                    // get rewards if CustomerID !=null
                if (customerID != -1)
                    {
                        var customerRewards = Context.vc_Customer_Rewards_Balances2.Where(o => o.store_id == vcStoreProfile.store_id && o.CustomerId == customerID).FirstOrDefault();
                        if (customerRewards != null)
                            { 
                        VenueCash__Available_balance = (double)customerRewards.VenueCash__Available_balance;
                             }
                     }
                    else
                    {
                        VenueCash__Available_balance = -1;
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, new {
                        VenueCash__Available_balance = VenueCash__Available_balance ,
                        GasCashBackRate= (double)vcStoreProfile.gas_back_rate,
                        OtherCashBackRate = (double)vcStoreProfile.cash_back_rate,
                        IsGasStation = (bool)vcStoreProfile.is_gas_station,
                        IsVenueCashBusiness=true,
                        store_id =vcStoreProfile.store_id,
                        distance = YelpV3.DistanceTo(yelpLat, yelpLong, customerLat, customerLong)
                });

                }
                else
                {
                    // not a venueCash business return IsVenueCashBusiness false
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        IsVenueCashBusiness = false,
                        distance = YelpV3.DistanceTo(yelpLat, yelpLong, customerLat, customerLong)
                    });
                }


               
            }



            catch (AppException ex)
            {


                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }

        }



        /* Disabled by Adil
		[HttpGet]
		public  HttpResponseMessage searchBusinesses ([FromUri] String customerId,[FromUri] float latitude,[FromUri] float longitude,[FromUri] string term,[FromUri] String location, [FromUri] String categories,[FromUri] int sort_by = 0)
		{
			try
			{

				YelpHelper yh = new YelpHelper();
				List<SearchBusinessBindingModel> searchBusinesses = new List<SearchBusinessBindingModel>();
				 searchBusinesses = yh.SearchForBusinessNearme(latitude, longitude,term,location,0, categories, sort_by);

				

				var vc_store_profile_mst = (from x in Context.vc_store_profile_mst2
											from y in Context.store_profile_mst2
											where x.store_id == y.id
											select x).Take(10);
				foreach (var storeProfile in vc_store_profile_mst)
				{
					SearchBusinessBindingModel business = new SearchBusinessBindingModel();

					AppModel Context2 = new AppModel();
					var store_profile_mst = Context2.store_profile_mst2.Find(storeProfile.store_id);
					if (store_profile_mst != null) // dont return any result if informations doesnt exist in store_profile_mst too
					{
						// Set vc_store_profile_mst data
						business.id = storeProfile.Id;
						business.IsGasStation = Convert.ToBoolean(storeProfile.is_gas_station);
						business.OtherCashBackRate = storeProfile.cash_back_rate.ToString();
						business.GasCashBackRate = storeProfile.gas_back_rate.ToString();
						business.Rating = storeProfile.Rating;
						business.Website = storeProfile.website;
						business.SubCategorieId = storeProfile.SubCategorieId.ToString();
						business.CategorieId = storeProfile.CategorieId;
						business.store_id = storeProfile.store_id;
						business.yelp_store_id = storeProfile.yelp_store_id.ToString();
						// Set store_profile_mst data
						business.store_name = store_profile_mst.store_name;
						business.store_address1 = store_profile_mst.store_address1;
						business.store_phone_no = store_profile_mst.store_phone_no;
						business.store_city = store_profile_mst.store_city;
						business.store_state = store_profile_mst.store_state;
						business.store_zip = store_profile_mst.store_zip.ToString();
						business.latitude = storeProfile.latitude;
						business.longitude = storeProfile.latitude;
						business.price = storeProfile.price;
						business.store_country = store_profile_mst.store_country;
						business.is_closed = Convert.ToBoolean(storeProfile.is_closed);
						using(AppModel Context4=new AppModel())
						{
							var c = (from r in Context4.vc_business_catagory_detail2
									 where r.store_id == business.store_id
									 select new {
										title= r.title,
										 alias=r.alias
									 }).ToList();
							if(c!=null)
								business.categories = c;

							Context4.Dispose();

						}
						//business.categories=
						//business.price = storeProfile.p
						// calculate distance 
						// SQL parameters 
						
						using (AppModel Context3 = new AppModel())
						{
							SqlParameter customerLatitude = new SqlParameter("@customerLatitude", latitude);
							SqlParameter customerLongitude = new SqlParameter("@customerLongitude", longitude);
							SqlParameter storeLatitude = new SqlParameter("@storeLatitude", storeProfile.latitude);
							SqlParameter storeLongitude = new SqlParameter("@storeLongitude", storeProfile.longitude);
							// run procedure to calculate distance
							double calculatedDistance = Context3.Database.SqlQuery<double>("exec vc_calculateDistanceUsingLatAndLong @customerLatitude=" + latitude + ",@customerLongitude=" + longitude + ",@storeLatitude=" + storeProfile.latitude + ",@storeLongitude=" + storeProfile.longitude).FirstOrDefault();
							var distance = (double)calculatedDistance;
							business.distance = (double) (distance / 1000.0f) * 0.621371f; // get distance in miles
							business.is_closed = Convert.ToBoolean(storeProfile.is_closed);
							//business.VenueCash__Available_balance=
							Context3.Dispose();
						}
						
						using(AppModel Context6=new AppModel())
						{
							var c = Context6.vc_Customer_Rewards_Balances2.Where(r => r.store_id == business.store_id).FirstOrDefault();
							if(c!=null)
								business.VenueCash__Available_balance = c.VenueCash__Available_balance;
							Context6.Dispose();
						}

						business.IsVenueCashBusiness = true;
						//add to list
						searchBusinesses.Add(business);
						Context2.Dispose();
					}

				}
				return Request.CreateResponse(HttpStatusCode.OK, searchBusinesses.OrderBy(o => o.distance).ToList());
			}
			catch(AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch(Exception err)
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

        */
        [HttpGet]
		public HttpResponseMessage OrderHistory([FromUri] int customerId)
		{
			try
			{

                // new Fast Request
                var Orders = (from t in Context.vc_CustomerTransactions2
                                           from s in Context.store_profile_mst2
                                           from vs in Context.vc_store_profile_mst2
                                           where t.StoreId==s.id && s.id==vs.store_id
                                           where t.CustomerId==customerId
                                           where t.Status=="APPROVED" || t.Status == "REFUNDED" || t.Status == "PARTIAL REFUND"
                                           

                                           select new orderHistoryOutput
                                           {
                                               CustomerTransactionId=t.Id,
                                               Date=(DateTime)t.Date,
                                               TotalAmount = (double) t.TotalAmount,
                                               VenueCashEarned = (double) t.VenueCashEarned_Other,
                                               VenueCashEarned_Total = (double)t.VenueCashEarned_Total,
                                               VenueCashEarnedGas = (double)t.VenueCashEarnedGas,
                                               Status = t.Status,
                                               Paid_VenueCash_Amount= (double)t.Paid_VenueCash_Amount,
                                               Paid_AllCash_Amount= (double)t.Paid_AllCash_Amount,
                                               Paid_Credit_Amount= (double)t.Paid_Credit_Amount,
                                               Cardnumber ="",
                                               store_name = s.store_name,
                                               store_address1 = s.store_address1,
                                               store_address2 = s.store_address2,
                                               store_city =s.store_city,
                                               store_state = s.store_state,
                                               store_zip =(int) s.store_zip,
                                               cash_back_rate= (double)vs.cash_back_rate,
                                               gas_back_rate= (double)vs.gas_back_rate,
                                               modify_dt=(DateTime) t.modify_dt,
                                               newTotal =(double) t.TotalAmount - (Context.vc_refunds2.Where(o=>o.customer_transaction_id==t.Id).Sum(o=> (double?)o.cashier_refunded_amount) ?? 0),
                                               OtherFinancingAmount=t.OtherFinancingAmount
                                           }
                                     ).ToList();


                // Get conversions

                // customerId to string :/ can't fix this no time
                var Conversions = (from t in Context.vc_VenueCash_Conversions2
                                  from s in Context.store_profile_mst2
                                  where t.store_id == s.id
                                  where t.CustomerId == customerId

                                   select new 
                                  {
                                       modify_dt=t.modify_dt,
                                       VenueCashAmountConverted=t.VenueCashAmountConverted,
                                       store_name = s.store_name,
                                       Status = "conversion",
                                       id= t.Id

                                   }
                                     ).ToList().ToList().OrderByDescending(o => o.modify_dt);


                // Adding to orders
                foreach(var conversion in Conversions)
                {
                    orderHistoryOutput order = new orderHistoryOutput();

                    order.modify_dt = conversion.modify_dt;
                    order.VenueCashEarned_Total = conversion.VenueCashAmountConverted/2;
                    order.store_name = conversion.store_name;
                    order.Status = conversion.Status;
                    order.CustomerTransactionId = conversion.id;
                    order.Date = conversion.modify_dt;

                    Orders.Add(order);
                }


                // Get Coupon Code Transactions
                var couponCodeTransactions = (from cT in Context.coupon_code_transactions2
                                              from p in Context.promoters_mst2
                                              from s in Context.store_profile_mst2
                                              where cT.promoter_userid == p.promoters_userid
                                              where s.id == p.Store_ID
                                              where cT.customer_ID == customerId
                                              select new
                                              {
                                                  modify_dt=cT.Date_Redeemed_Server,
                                                  VenueCashEarned = cT.VenueCash_Credit_Amt,
                                                  store_name = s.store_name,
                                                  Status ="coupon",
                                                  DeviceDate = cT.Date_Redeemed_Customer,
                                                  id = cT.coupon_code_transaction_ID,
                                              }
                                              ).ToList();

                // Adding to orders
                foreach (var coupon in couponCodeTransactions)
                {
                    orderHistoryOutput order = new orderHistoryOutput();

                    order.modify_dt = coupon.modify_dt;
                    order.VenueCashEarned_Total = coupon.VenueCashEarned;
                    order.store_name = coupon.store_name;
                    order.Status = coupon.Status;
                    order.CustomerTransactionId = coupon.id;
                    order.Date = coupon.modify_dt;
                    Orders.Add(order);
                }






                var combinedOrder = Orders.ToList().OrderByDescending(o => o.modify_dt);





                return Request.CreateResponse(HttpStatusCode.OK, combinedOrder);
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
		public HttpResponseMessage OrderDetails([FromUri] int CustomerTransactionId)
		{
			try
			{
                // get Transaction
                var transaction = Context.vc_CustomerTransactions2.Find(CustomerTransactionId);

                // Get credit card info if credit card is used in transaction
                var Cardnumber = "";
                var CardType = "";
                var cardID = 0;
                if (transaction.CardId != null)
                {
                    var creditCard = Context.vc_CustomerCreditCards2.Find(transaction.CardId);
                    if(creditCard != null) { // prevent API from crashing in case the credit card was deleted
                    Cardnumber = AppHelpers.GetLastFourDigits(creditCard.CardNumber);
                    CardType = creditCard.CardType;
                    cardID = creditCard.Id;
                    }
                }
                
                



                //Faster query
                var OrderDetails = (from t in Context.vc_CustomerTransactions2
                                    from s in Context.store_profile_mst2
                                    from employee in Context.cashier_mst2.Where(e=>e.id==t.cashier_id)
                                    from customer in Context.vc_customers2.Where(c=>c.id==t.CustomerId)
                              where t.StoreId == s.id
                              where t.Id == CustomerTransactionId
                              where t.Status == "APPROVED" || t.Status == "REFUNDED" || t.Status == "PARTIAL REFUND"

                              select new
                              {
                                  CustomerTransactionId = t.Id,
                                  Date = t.Date,
                                  TotalAmount = t.TotalAmount,
                                  VenueCashEarned = t.VenueCashEarned,
                                  GasAmount =t.GasAmount,
                                  OtherAmount=t.OtherAmount,
                                  VenueCashEarned_Total = t.VenueCashEarned_Total,
                                  VenueCashEarnedGas = t.VenueCashEarnedGas,
                                  VenueCashEarned_Other =t.VenueCashEarned_Other,
                                  Status = t.Status,
                                  Paid_VenueCash_Amount = t.Paid_VenueCash_Amount,
                                  Paid_AllCash_Amount = t.Paid_AllCash_Amount,
                                  Paid_Credit_Amount = t.Paid_Credit_Amount,
                                  Tip_Amount =t.Tip_Amount,
                                  OtherFinancingAmount=t.OtherFinancingAmount,

                                  cardID = cardID,
                                  CardType = CardType,
                                  Cardnumber = Cardnumber,

                                  store_name = s.store_name,
                                  store_address1 = s.store_address1,
                                  store_address2 = s.store_address2,
                                  store_city = s.store_city,
                                  store_state = s.store_state,
                                  store_zip = s.store_zip,

                                  customerFirstName= customer.FirstName,
                                  customerLastName= customer.LastName,
                                  employeeName = employee.cashier_name,


                                  vc_refunds = Context.vc_refunds2.Where(r => r.customer_transaction_id == t.Id).ToList()
                              }
                                   ).FirstOrDefault();

 
				
				return Request.CreateResponse(HttpStatusCode.OK, OrderDetails);
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
        public HttpResponseMessage ConversionDetails([FromUri] int id)
        {
            try
            {

                //Faster query
                var ConversionDetails = (from t in Context.vc_VenueCash_Conversions2
                                    from s in Context.store_profile_mst2
                                    from customer in Context.vc_customers2.Where(c => c.id == t.CustomerId)
                                    where t.store_id == s.id
                                    where t.Id == id
                                    select new
                                    {
                                        store_name=s.store_name,
                                        store_address1 = s.store_address1,
                                        conversionID = t.Id,
                                        Date = t.create_dt,
                                        VenueCashAmountConverted=t.VenueCashAmountConverted,
                                        allCashCredited= t.VenueCashAmountConverted/2,
                                        AllCashBalance = customer.TotalAllCash
                                    }
                                   ).FirstOrDefault();


                return Request.CreateResponse(HttpStatusCode.OK, ConversionDetails);
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
        public HttpResponseMessage CouponDetails([FromUri] int id)
        {
            try
            {
                // Get Coupon Code Transactions
                var couponCodeDetails = (from cT in Context.coupon_code_transactions2
                                              from p in Context.promoters_mst2
                                              from s in Context.store_profile_mst2
                                              from c in Context.vc_customers2
                                              where cT.promoter_userid == p.promoters_userid
                                              where s.id == p.Store_ID
                                              where cT.customer_ID == c.id
                                              where cT.coupon_code_transaction_ID==id
                                              select new
                                              {
                                                  store_name = s.store_name,
                                                  store_address1 = s.store_address1,
                                                  coupon_code_transaction_ID=cT.coupon_code_transaction_ID,
                                                  Date = cT.Date_Redeemed_Customer,
                                                  VenueCashAmountConverted = cT.VenueCash_Credit_Amt,
                                                  VenueCashBalance = c.TotalVenueCash

                                              }
                                              ).FirstOrDefault();




                return Request.CreateResponse(HttpStatusCode.OK, couponCodeDetails);
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
		public HttpResponseMessage AddCreditCard([FromBody]vc_CustomerCreditCards_post_req req)
		{
			try
			{
    
				AppModel Context2 = new AppModel();
				var data = new vc_CustomerCreditCards()
				{
					
						CustomerId=req.CustomerId,
						NameOnCard=req.NameOnCard,
						BillingAddress1=req.BillingAddress1,
						BillingAddress2=req.BillingAddress2,
						City=req.City,
						State=req.State,
						CardType=req.CardType,
						CardNumber=req.CardNumber,
						SecurityCode=req.SecurityCode,
						ExpirationDate=req.ExpirationDate,
						IsDefault=req.IsDefault,
						create_dt=DateTime.Now,
						modify_dt=DateTime.Now,
						created_userid="system"

					
				};
                if (data.IsDefault)
                {

                    var creditCards = Context.vc_CustomerCreditCards2.Where(o => o.CustomerId == req.CustomerId).ToList();


                    foreach ( var creditCard in creditCards)
                    {
                        creditCard.IsDefault = false;
                        Context.Entry(creditCard).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else
                {
                    var creditCards = Context.vc_CustomerCreditCards2.Where(o => o.CustomerId == req.CustomerId).ToList();
                    if (creditCards.Count == 0)
                    {
                        data.IsDefault = true;
                    }


                }



                Context.vc_CustomerCreditCards2.Add(data);
                Context.SaveChanges();
                
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
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
		public HttpResponseMessage ModifyCreditCard(vc_CustomerCreditCards_put_req req)
		{
			try
			{
				AppModel Context2 = new AppModel();
				var c = Context.vc_CustomerCreditCards2.Find(req.id);
				if (c == null)
					throw new Exception("Invalid card");
				c.CustomerId = req.CustomerId;
				c.NameOnCard = req.NameOnCard;
				c.BillingAddress1 = req.BillingAddress1;
				c.BillingAddress2 = req.BillingAddress2;
				c.City = req.City;
				c.State = req.State;
				c.CardType = req.CardType;
				c.CardNumber = req.CardNumber;
				c.SecurityCode = req.SecurityCode;
				c.ExpirationDate = req.ExpirationDate;
				c.IsDefault = req.IsDefault;
				Context.Entry(c).State = System.Data.Entity.EntityState.Modified;
				Context.SaveChanges();

                if (c.IsDefault) // only if the credit card is default
                {
				    var cc = Context2.vc_CustomerCreditCards2.Where(r => r.CustomerId == c.CustomerId);
				    foreach (var x in cc)
				    {
					    x.IsDefault = false;

					    if (x.Id == c.Id)
						    x.IsDefault = true;

					    x.modify_dt = DateTime.Now;
					    x.Modify_usrid = "system";
					    Context2.Entry(x).State = System.Data.Entity.EntityState.Modified;
					
				    }
				    Context2.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
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
		public HttpResponseMessage CardsOnFile([FromUri] int CustomerID)
		{
			try
			{
				var x = Context.vc_CustomerCreditCards2.Where(r => r.CustomerId==CustomerID).ToArray();
				for (int i = 0; i < x.Length; i++)
					x[i].CardNumber = AppHelpers.GetLastFourDigits(x[i].CardNumber);

				return Request.CreateResponse(HttpStatusCode.OK, x);
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
		public HttpResponseMessage DeleteCustomerCreditCard([FromUri] int id)
		{
			try
			{
				var customerCreditCard = Context.vc_CustomerCreditCards2.Find(id);
				if (customerCreditCard == null)
				{
					throw new AppException(-1, "Not Found");
				}

				Context.vc_CustomerCreditCards2.Remove(customerCreditCard);
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


		

		[HttpGet]
		public HttpResponseMessage MyWallet([FromUri] String customerId)
		{
			try
			{
				var id = Convert.ToInt32(customerId);
				var ls_my_walletObj = new my_walletObj();
				var c = Context.vc_customers2.Find(id);

				if (c == null)
					throw new AppException(-1, "Customer not found");

				ls_my_walletObj.TotalAllCash =(double)c.TotalAllCash;
				ls_my_walletObj.TotalVenueCash = (double)c.TotalVenueCash;

				Object rwd = (from x in Context.vc_Customer_Rewards_Balances2
						   from y in Context.store_profile_mst2
                           from z in Context.vc_store_profile_mst2
						   where x.store_id == y.id && x.CustomerId==id && y.id== z.store_id

						   select new {
							    VenueCashBalance=x.VenueCashBalance,
								store_id=y.id,
							    store_name = y.store_name,
								store_address = y.store_address1,
								store_city =y.store_city,
								store_state = y.store_state,
                                image_url = z.image_url
						   }).ToArray();

				ls_my_walletObj.vc_Customer_Rewards_Balances = rwd;


				return Request.CreateResponse(HttpStatusCode.OK, ls_my_walletObj);
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
		public HttpResponseMessage ConvertAllVenueCashToAllCash(ConvertAllVenueCashToAllCash_post_req_2 data)
		{
			
			try
			{
				
				ConvertAllVenueCashToAllCashConvertAll_post_resp rep = new ConvertAllVenueCashToAllCashConvertAll_post_resp();
				var stores = Context.vc_Customer_Rewards_Balances2.Where(r => r.CustomerId == data.CustomerId && r.VenueCashBalance >0 );

				foreach (var s in stores)
				{
					AppModel Context2 = new AppModel();
					

					// Step 1,2
					vc_VenueCash_Conversions f = new vc_VenueCash_Conversions() {
						CustomerId=data.CustomerId,
						store_id=Convert.ToInt32(s.store_id),
						modify_dt=DateTime.Now,
						create_dt=DateTime.Now,
						Modify_usrid="system",
						created_userid="system",
						VenueCashAmountConverted=(float)s.VenueCashBalance
					};
					Context2.vc_VenueCash_Conversions2.Add(f);
					// Step 3
					s.VenueCashBalance = 0;
                    s.VenueCash__Available_balance = 0;
					Context2.Entry(s).State = EntityState.Modified;
					Context2.SaveChanges();
					Context2.Dispose();
					
				}
				
              

				// Step 4
				using (var Context3=new AppModel())
				{
					var c = Context3.vc_customers2.Find(data.CustomerId);
                    double venueCashConverted = c.TotalVenueCash;
					c.TotalAllCash = c.TotalAllCash + (c.TotalVenueCash * 0.5);
                    // Round up to two digits after floating point
                    c.TotalAllCash = VCBalanceHelper.nearToZero((double)c.TotalAllCash);
                    // Step 51
                    c.TotalVenueCash = 0;
					Context3.Entry(c).State = EntityState.Modified;
					Context3.SaveChanges();
					rep.TotalVenueCash = c.TotalVenueCash;
					rep.TotalAllCash = c.TotalAllCash;


                    // Send VenueCash emails
                    VenueCashEmails.conversionToAllCashCustomer(c.Email,c.FirstName,venueCashConverted,(venueCashConverted*0.5));

                }




                return Request.CreateResponse(HttpStatusCode.OK, rep);
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
		public HttpResponseMessage ConvertStoreVenueCashToAllCash(ConvertAllVenueCashToAllCash_post_req data)
		{
			
			try
			{


                var customerReward = Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == data.CustomerId && o.VenueCashBalance > 0 && o.store_id == data.store_id).FirstOrDefault();

                if (customerReward != null)
                { // if balance > 0 for that store we do the convertion

                    // check if VenueCashToConvert greater that VCbalance
                    if (data.VenueCashToConvert > customerReward.VenueCashBalance)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -2); // convertion Not Possible
                    }


                    double leftInBalance = VCBalanceHelper.preventSingedBalance((double)customerReward.VenueCashBalance - data.VenueCashToConvert);

                    // set new balance
                    customerReward.VenueCashBalance = leftInBalance;
                    customerReward.VenueCash__Available_balance = customerReward.VenueCashBalance;
                    // set modified date
                    customerReward.modify_dt = DateTime.Now;

                    // Calculate All cash Conversion
                    double newAllCash = data.VenueCashToConvert * 0.5;


                    //Update Vc_customer Update VenueCash and all Cash
                    var vcCustomer = Context.vc_customers2.Find(data.CustomerId); 
                    vcCustomer.TotalVenueCash = VCBalanceHelper.preventSingedBalance(vcCustomer.TotalVenueCash - data.VenueCashToConvert);
                    vcCustomer.TotalAllCash = vcCustomer.TotalAllCash + newAllCash;


                    // Keep a record of the conversion
                    vc_VenueCash_Conversions venueCashConvertion = new vc_VenueCash_Conversions();
                    venueCashConvertion.CustomerId = data.CustomerId;
                    venueCashConvertion.store_id = data.store_id;
                    venueCashConvertion.modify_dt = DateTime.Now;
                    venueCashConvertion.create_dt = DateTime.Now;
                    venueCashConvertion.Modify_usrid = "system";
                    venueCashConvertion.created_userid = "system";
                    venueCashConvertion.VenueCashAmountConverted = data.VenueCashToConvert;

                    Context.vc_VenueCash_Conversions2.Add(venueCashConvertion);

                    Context.SaveChanges();


                    // Send VenueCash emails
                    VenueCashEmails.conversionToAllCashCustomer(vcCustomer.Email, vcCustomer.FirstName, data.VenueCashToConvert, newAllCash);

                    return Request.CreateResponse(HttpStatusCode.OK, 0);// All is good

                }
                else // if no balance no convertion
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// -1 no Reward balance found convertion not done

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
		public HttpResponseMessage DeactivateAccount(CAccount_Deactivated_post_req data)
		{
			try
			{
				int id =  data.CustomerId;
				var x = Context.vc_customers2.Find(id);
				x.Account_Deactivated = 1;
				Context.Entry(x).State = EntityState.Modified;
				Context.SaveChanges();
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
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
		public HttpResponseMessage ScanQRCode(ScanQRCodeBindingModel_post_req_dm data)
		{
			try
			{


				//var cashier_id = Context.vc_StoreRegisters2.Where(r => r.RegisterId == data.register_id && r.store_id == data.store_id);
				//vc_CustomerTransactions customerTransaction = new vc_CustomerTransactions();

				var storeProfile = Context.vc_store_profile_mst2.Where(o => o.store_id == data.store_id).FirstOrDefault();
				if (storeProfile == null)
                    return Request.CreateResponse(HttpStatusCode.OK, -2);// store not found in vc_store_profile_mst

                // allow transaction without credit card . possible venuecash or all cash money
                var cardID = 0;
                var cc = Context.vc_CustomerCreditCards2.Where(r => r.CustomerId == data.CustomerId && r.IsDefault).FirstOrDefault();
                if (cc != null)
                {
                    cardID = cc.Id;
                }



                // check Register is gas Pump
                int shift_id = -1;
                int Cashier_ID_on_duty = -1;
                bool isGasPump = false;
                bool authorizePumping = false;
                string authorizationID = null;
                var storeRegister = Context.vc_StoreRegisters2.Find(data.register_id);
                if (storeRegister == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -3);// Register Not found
                }
                else if (storeRegister.IsGasPump==1) // gas pump register
                {
                    isGasPump = true;
                    // check if has $50 on venuecash or allcash or default credit card
                    // get Venuecash balance for that store and all cash balance
                    var customerRewardsBalance = Context.vc_Customer_Rewards_Balances2.Where(r => r.CustomerId == data.CustomerId && r.store_id == data.store_id).FirstOrDefault();
                    var customer = Context.vc_customers2.Find(data.CustomerId);

                    if (customerRewardsBalance != null)
                    {
                        if (customerRewardsBalance.VenueCashBalance + customer.TotalAllCash >= 50) // check VenueCash Balance
                        {
                            authorizePumping = true;
                        }

                    }

                    if (customer.TotalAllCash>=50) // check AllCash Balance
                    {
                        authorizePumping = true;
                    }
                    else if(!authorizePumping) // Authorize $50 on credit card
                    {
                        PayPal.Api.CreditCard creditCard = new PayPal.Api.CreditCard();
                        creditCard.type = cc.CardType.ToLower();
                        creditCard.number = cc.CardNumber;
                        creditCard.expire_month = Convert.ToInt32(cc.ExpirationDate.Split('/')[0]);
                        creditCard.expire_year = Convert.ToInt32(cc.ExpirationDate.Split('/')[1]) + 2000;
                        creditCard.first_name = cc.NameOnCard.Split(' ')[0];
                        creditCard.last_name = cc.NameOnCard.Split(' ')[1];

                        var paymentGateWayResponse = PayPalProcessor.Authorize(creditCard, 50); // Authorize $50 on customer credit card
                        if(paymentGateWayResponse.state.ToLower()== "approved")
                        {
                            authorizationID = paymentGateWayResponse.transactions[0].related_resources[0].authorization.id;
                            authorizePumping = true;
                        }
                    }

                    // check if authorized to pump
                    if (!authorizePumping)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -4);// Not authorized to pump
                    }

                }
                else // regular register
                {
                    shift_id = storeRegister.shift_id;
                    // getCashier on duty for that register
                    Cashier_ID_on_duty = (int)storeRegister.Cashier_ID_on_duty;
                }


                //Get Next order_number
                int order_number=usefullFunctions.getNextOrderNumber(data.store_id, (int)storeProfile.business_type);



                int reg_id = data.register_id;

                var trans = new vc_CustomerTransactions() {
                    CustomerId = data.CustomerId,
                    StoreId = data.store_id,
                    created_userid = "system",
                    Modify_usrid = "system",
                    modify_dt = DateTime.Now,
                    create_dt = DateTime.Now,
                    RegisterId = reg_id,
                    Date = Convert.ToDateTime(data.Date),
                    other_cash_back_rate = storeProfile.cash_back_rate,
                    gas_back_rate = storeProfile.gas_back_rate,
                    cash_back_rate = storeProfile.cash_back_rate,
                    Amount = 0,
                    GasAmount = 0,
                    VenueCashEarnedGas = 0,
                    PaidOut = false,
                    OtherAmount = 0,
                    TotalAmount = 0,
                    Paid_VenueCash_Amount = 0,
                    Paid_AllCash_Amount = 0,
                    Paid_Credit_Amount = 0,
                    TransactionType = 0,
                    VenueCashEarned = 0,
                    cashier_id = Cashier_ID_on_duty,
                    VenueCashEarned_Other = 0,
                    Status = "Pending_Cashier_Request",
                    CardId = cardID,
                    Sub_TotalAmount = 0,
                    Tip_Amount = 0,
                    VenueCashEarned_Total = 0,
                    Gross_Refund_Amount = 0,
                    shift_id = shift_id,
                    OtherFinancingAmount = 0,
                    PayoutAmount = 0,
                    AuthorizationID=authorizationID,
                    order_number= order_number

                };
				Context.vc_CustomerTransactions2.Add(trans);
				Context.SaveChanges();
					
				 
				return Request.CreateResponse(HttpStatusCode.OK,new {
                    transactionID = trans.Id,
                    isGasPump = isGasPump,
                    business_type= storeProfile.business_type,
                    order_number = order_number
                });

			}
			catch(AppException ex)
			{
                //return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
                return Request.CreateResponse(HttpStatusCode.OK, -4);// Try new credit card
            }
			catch(Exception err)
			{
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
                return Request.CreateResponse(HttpStatusCode.OK, -4);// Try new Credit card
            }

		}

		[HttpGet]
		public HttpResponseMessage ReceivePaymentRequest([FromUri] int customerTransactionId)
		{
			try
			{
				
				ReceivePaymentRequestResponseBindingModel paymentResponse = new ReceivePaymentRequestResponseBindingModel();

                paymentResponse.canPay = true;

                var customerTransaction = Context.vc_CustomerTransactions2.Find(customerTransactionId);

				if (customerTransaction == null)
					throw new AppException(-1, "CustomerTransactions retuen null");

				int cid = Convert.ToInt32(customerTransaction.CustomerId);
				var storeProfile = Context.vc_store_profile_mst2.Where(o => o.store_id == customerTransaction.StoreId).FirstOrDefault();
				
				var customerRewardsBalance = Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == cid && o.store_id == customerTransaction.StoreId).FirstOrDefault();
				

				var customer = Context.vc_customers2.Where(r => r.id == customerTransaction.CustomerId).FirstOrDefault();
                

                paymentResponse.OtherAmount = (float)customerTransaction.OtherAmount;
				paymentResponse.Date = customerTransaction.Date;
				paymentResponse.GasAmount = (float)customerTransaction.GasAmount;
				paymentResponse.Status = customerTransaction.Status;
                paymentResponse.OtherFinancingAmount = customerTransaction.OtherFinancingAmount;


                paymentResponse.OtherCashBackRate = (float)storeProfile.cash_back_rate;
				paymentResponse.GasCashBackRate = (float)storeProfile.gas_back_rate;
				if (customerRewardsBalance == null)
				{
                    // set TotalVenueCash to Zero if no balance exist for that store in RewardsBalance table
					paymentResponse.TotalVenueCash = 0;
				}
				else
				{
					paymentResponse.TotalVenueCash = (float)customerRewardsBalance.VenueCashBalance;

				}
                // AllCash Balance
                paymentResponse.TotalAllCash = (float)customer.TotalAllCash;
                paymentResponse.TotalAmount = (float)customerTransaction.TotalAmount;

                // get credit card info
                if (customerTransaction.CardId != 0)
                {
                    var cc = Context.vc_CustomerCreditCards2.Where(r => r.IsDefault && r.CustomerId == customerTransaction.CustomerId).FirstOrDefault();

                    paymentResponse.CardId = cc.Id;
                    paymentResponse.card4digits = AppHelpers.GetLastFourDigits(cc.CardNumber);
                    paymentResponse.CardType = cc.CardType;
                }
                else // Customer have no credit card check vc and ac balance if it can cover payment
                {
                    if(paymentResponse.TotalAllCash + paymentResponse.TotalVenueCash <= paymentResponse.TotalAmount)
                    {
                        paymentResponse.canPay = false;
                    }

                }




                // Controle Tips
                var vcStoreProfile = Context.vc_store_profile_mst2.Where(s => s.store_id == customerTransaction.StoreId).FirstOrDefault();
                paymentResponse.isTips = vcStoreProfile.is_tips;


                return Request.CreateResponse(HttpStatusCode.OK, paymentResponse);
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


        //ADD By ADIL
        [HttpPost]
        public HttpResponseMessage SendPayment_for_Approval(SendPaymentRequestBModel data)
        {
            try
            {

                // Get the transaction by ID
                var transaction = Context.vc_CustomerTransactions2.Where(o => o.Id == data.CustomerTransactionId).FirstOrDefault();
                var vcStoreProfileMst = Context.vc_store_profile_mst2.Where( o => o.store_id == transaction.StoreId).FirstOrDefault();
                if (transaction == null)
                {
                    throw new AppException(-1, "Not Found");
                }

                // set Data from request
                transaction.Paid_Credit_Amount = VCBalanceHelper.nearToZero((double)data.Paid_Credit_Amount);
                transaction.Paid_VenueCash_Amount = VCBalanceHelper.nearToZero((double)data.Paid_VenueCash_Amount);
                transaction.Paid_AllCash_Amount = VCBalanceHelper.nearToZero((double)data.Paid_AllCash_Amount);
                transaction.Tip_Amount = VCBalanceHelper.nearToZero((double)data.Tip_Amount);
                transaction.TotalAmount = VCBalanceHelper.nearToZero((double)transaction.Sub_TotalAmount + (double)transaction.Tip_Amount);

                // Set back_rate from vcStoreProfileMst
                transaction.other_cash_back_rate = vcStoreProfileMst.cash_back_rate;
                transaction.cash_back_rate = vcStoreProfileMst.cash_back_rate;
                transaction.gas_back_rate = vcStoreProfileMst.gas_back_rate;

                //## Step 1 update transaction status to "Customer_Submitted"
                transaction.Status = "Customer_Submitted";

                Context.SaveChanges(); // we need to save the Status in case something happend with the Paypal API

                // convert customer ID
                var customerID =  transaction.CustomerId;
                // get Customer details
                vc_customers vcCustomer = Context.vc_customers2.Where(o => o.id == customerID).FirstOrDefault();
                
                // Get CreditCard Info and customer info to send to paypal
                var vcCustomerCreditCard = Context.vc_CustomerCreditCards2.Where(o=>o.CustomerId== transaction.CustomerId &&  o.IsDefault==true).FirstOrDefault();

                // Call Paypal API if Paid_Credit_Amount> 0
                payment paymentInfo = new payment();
                if (transaction.Paid_Credit_Amount > 0)
                {
                    // create paypal credit card
                    CreditCard creditCard = new CreditCard();
                    creditCard.type = vcCustomerCreditCard.CardType.ToLower();
                    creditCard.number = vcCustomerCreditCard.CardNumber;
                    creditCard.expire_month = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[0]);
                    creditCard.expire_year = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[1]) + 2000;
                    creditCard.first_name = vcCustomerCreditCard.NameOnCard.Split(' ')[0];
                    creditCard.last_name = vcCustomerCreditCard.NameOnCard.Split(' ')[1];

                    var paymentGateWayResponse = PayPalProcessor.Pay(creditCard, data.Paid_Credit_Amount);
                    paymentInfo.state = paymentGateWayResponse.state;
                    paymentInfo.id = paymentGateWayResponse.id;
                    paymentInfo.sale_id = paymentGateWayResponse.transactions[0].related_resources[0].sale.id;
                }
                else
                {
                    if (transaction.Paid_VenueCash_Amount > 0)
                    {
                        // check if venueCash Balance is covering the payment
                        var venueCashBalance = Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == transaction.CustomerId && o.store_id == transaction.StoreId).FirstOrDefault();
                        if (venueCashBalance.VenueCashBalance >= transaction.Paid_VenueCash_Amount)
                        {
                            paymentInfo.state = "approved";
                            paymentInfo.id = transaction.Id.ToString();
                            paymentInfo.sale_id = "";
                        }
                        else
                        {
                            paymentInfo.state = "denied";
                        }
                    }
                    if (transaction.Paid_AllCash_Amount > 0)
                    {
                        // check if AllCash Balance is covering the payment
                        var customer = Context.vc_customers2.Find(transaction.CustomerId);
                        if (customer.TotalAllCash >= transaction.Paid_AllCash_Amount)
                        {
                            paymentInfo.state = "approved";
                            paymentInfo.id = transaction.Id.ToString();
                            paymentInfo.sale_id = "";
                        }
                        else
                        {
                            paymentInfo.state = "denied";
                        }
                    }
                }
                
                float venueCashEarned = 0;
                // Payment Approved
                if (paymentInfo.state == "approved")
                {
                    if (transaction.Paid_Credit_Amount > 0 || transaction.OtherFinancingAmount>0)
                    {
                        // Calculating VenueCash
                        //## Step 2 Set VenueCashEarned_Other in the vc_CustomerTransactions table

                        // Tested
                        //transaction.VenueCashEarned_Other = (transaction.other_cash_back_rate * (data.Paid_Credit_Amount+ transaction.OtherFinancingAmount) / 100);
                        // Other Product Use Case
                        if (transaction.GasAmount <= 0)


                        
                        // 1 - when CC Paid => Sub Total; use the Sub total
                        // 2 - when CC Paid < Sub Total; use the CC Paid


                        {
                            // other   
                            if (transaction.Paid_Credit_Amount >= transaction.Sub_TotalAmount)
                                 
                            
                            {
                                transaction.VenueCashEarned_Other = (transaction.other_cash_back_rate * ((transaction.Sub_TotalAmount)
                                + transaction.OtherFinancingAmount) / 100);

                            }

                            else

                            {
                                
                                transaction.VenueCashEarned_Other = (transaction.other_cash_back_rate * ((transaction.Paid_Credit_Amount)
                                + transaction.OtherFinancingAmount) / 100);

                            }
                                
                           
                            // Gas
                            transaction.VenueCashEarnedGas = 0;

                        }

                        else
                        {
                            // other

                            if (transaction.Paid_Credit_Amount >= transaction.Sub_TotalAmount)


                            {
                                transaction.VenueCashEarned_Other = ((transaction.other_cash_back_rate *
                               ((transaction.OtherAmount / transaction.TotalAmount) * transaction.Sub_TotalAmount
                               )) + (transaction.other_cash_back_rate * transaction.OtherFinancingAmount)) / 100;

                            }

                            else

                            {
                                
                                transaction.VenueCashEarned_Other = ((transaction.other_cash_back_rate *
                               ((transaction.OtherAmount / transaction.TotalAmount) * transaction.Paid_Credit_Amount
                               )) + (transaction.other_cash_back_rate * transaction.OtherFinancingAmount)) / 100;
                            }

                            
                            // GAs

                            transaction.VenueCashEarnedGas = ((transaction.gas_back_rate *
                                ((transaction.GasAmount / transaction.TotalAmount) * transaction.Paid_Credit_Amount
                                ))) / 100;

                        }
                        


                            // Total earned
                            transaction.VenueCashEarned_Total = transaction.VenueCashEarnedGas + transaction.VenueCashEarned_Other;
 

                        // Round up VenueCash Earned two digits after floating point
                        transaction.VenueCashEarned_Other = VCBalanceHelper.nearToZero((double)transaction.VenueCashEarned_Other);
                        transaction.VenueCashEarnedGas = VCBalanceHelper.nearToZero((double)transaction.VenueCashEarnedGas);
                        transaction.VenueCashEarned_Total = VCBalanceHelper.nearToZero((double)transaction.VenueCashEarned_Total);

                        //## Step 3 
                        // transaction.VenueCashEarned_Total = transaction.VenueCashEarned_Other;
                        venueCashEarned = (float)transaction.VenueCashEarned_Total;
 
                    }// end if Paid_VenueCash_Amount >0

                    // Update VenueCash Balance

                    //## Step 4 update "VenueCashBalance" if venueCash is not used
                    // Find reward balance if it exist
                    var vcCustomerRewardsBalance = Context.vc_Customer_Rewards_Balances2.Where(o => o.store_id == transaction.StoreId && o.CustomerId == customerID).FirstOrDefault();
                    // if reward balance doesnt exist for that store create one
                    if (vcCustomerRewardsBalance == null)
                    {
                        vc_Customer_Rewards_Balances vcCustomerRewardsBalanceNew = new vc_Customer_Rewards_Balances();
                        vcCustomerRewardsBalanceNew.create_dt = DateTime.Now;
                        vcCustomerRewardsBalanceNew.modify_dt = DateTime.Now;
                        vcCustomerRewardsBalanceNew.store_id = transaction.StoreId;
                        vcCustomerRewardsBalanceNew.CustomerId = transaction.CustomerId;
                        vcCustomerRewardsBalanceNew.VenueCash_Gas_Awarded = transaction.VenueCashEarnedGas;
                        vcCustomerRewardsBalanceNew.VenueCash_Other_Awarded = transaction.VenueCashEarned_Other;
                        vcCustomerRewardsBalanceNew.VenueCash_Awarded = transaction.VenueCashEarned;
                        vcCustomerRewardsBalanceNew.VenueCashEarned_Total = transaction.VenueCashEarned_Total;
                        vcCustomerRewardsBalanceNew.VenueCashBalance = transaction.VenueCashEarned_Total;
                        vcCustomerRewardsBalanceNew.VenueCash__Available_balance = vcCustomerRewardsBalanceNew.VenueCashBalance;

                        vcCustomerRewardsBalanceNew.AllCash__Available_balance = 0; // this parameter should'nt be in this table

                        Context.vc_Customer_Rewards_Balances2.Add(vcCustomerRewardsBalanceNew); // insert to table
                    }
                    else // Update existing record in vc_customer_rewards_balance table
                    {
                        //## Step 4 update "VenueCashBalance" if venueCash is used
                        vcCustomerRewardsBalance.modify_dt = DateTime.Now;
                        
                        vcCustomerRewardsBalance.VenueCashBalance = vcCustomerRewardsBalance.VenueCashBalance + transaction.VenueCashEarned_Total - data.Paid_VenueCash_Amount;
                        vcCustomerRewardsBalance.VenueCash__Available_balance = vcCustomerRewardsBalance.VenueCashBalance;
                        vcCustomerRewardsBalance.VenueCash_Gas_Awarded = vcCustomerRewardsBalance.VenueCash_Gas_Awarded + transaction.VenueCashEarnedGas;
                        vcCustomerRewardsBalance.VenueCash_Other_Awarded = vcCustomerRewardsBalance.VenueCash_Other_Awarded + transaction.VenueCashEarned_Other;
                        vcCustomerRewardsBalance.VenueCash_Awarded = vcCustomerRewardsBalance.VenueCash_Awarded + transaction.VenueCashEarned;
                        vcCustomerRewardsBalance.VenueCashEarned_Total = vcCustomerRewardsBalance.VenueCashEarned_Total + transaction.VenueCashEarned_Total;
                        vcCustomerRewardsBalance.AllCash__Available_balance = 0; // this parameter should'nt be in this table
                    }

                    // Step 5 Update TotalVenueCash if venueCash not used
                    vcCustomer.TotalVenueCash = vcCustomer.TotalVenueCash + (double)transaction.VenueCashEarned_Total - (double)data.Paid_VenueCash_Amount;
                    if (vcCustomer.TotalVenueCash < 0) { vcCustomer.TotalVenueCash = 0; }
                    //

                    // Step 6 update total all cash if Paid_AllCash_Amount> 0
                    if (data.Paid_AllCash_Amount > 0)
                    {
                        vcCustomer.TotalAllCash = vcCustomer.TotalAllCash - (double)transaction.Paid_AllCash_Amount;
                        // prevent inferior to zero value caused by the floating point round up
                        vcCustomer.TotalVenueCash = VCBalanceHelper.nearToZero((double)vcCustomer.TotalVenueCash);
                       // if (vcCustomer.TotalAllCash < 0) { vcCustomer.TotalAllCash = 0; }
                    }

                    // Step 7 Set Tip_Payout_Amount in the shift_details_mst table it data.Tip_Amount > 0 
                    if (data.Tip_Amount > 0)
                    {

                        var shiftDetailsMst = Context.shift_details_mst2.Find(transaction.shift_id);

                        if (shiftDetailsMst.Tip_Payout_Amount == null)
                        {
                            shiftDetailsMst.Tip_Payout_Amount = data.Tip_Amount;
                        }
                        else
                        {
                            shiftDetailsMst.Tip_Payout_Amount = shiftDetailsMst.Tip_Payout_Amount + data.Tip_Amount; 
                        }
                    }



                    transaction.ProcessorConfirmationId = paymentInfo.id;
                    transaction.sale_id = paymentInfo.sale_id;
                    transaction.Status = "approved";


                    // Calculating PayoutAmount for this transaction
                    double CCFees = 0;
                    if (transaction.Paid_Credit_Amount > 0)
                    {
                        CCFees= VCBalanceHelper.getCCFees((double)transaction.Paid_Credit_Amount, vcCustomerCreditCard.CardType);
                    }
                    transaction.PayoutAmount = (double)transaction.TotalAmount - (double)transaction.VenueCashEarned_Total - CCFees;
                    //

                    Context.SaveChanges();



                    // get Store info
                    var store_profile_mst = Context.store_profile_mst2.Find(vcStoreProfileMst.store_id);

                    Send_payment_post_resp response = new Send_payment_post_resp();
                    response.CustomerTransactionId = transaction.Id;
                    response.ProcessorConfirmationId = transaction.ProcessorConfirmationId;
                    response.Status = transaction.Status;
                    response.VenueCashEarned_Total = (float)venueCashEarned;// this conversion may create a problem
                    response.Sub_TotalAmount = (double)transaction.Sub_TotalAmount;
                    response.TotalAmount = (double)transaction.TotalAmount;
                    response.Tip_Amount = (double)transaction.Tip_Amount;
                    response.TotalAllCash = vcCustomer.TotalAllCash;
                    response.TotalVenueCash = vcCustomer.TotalVenueCash;
                    response.totalEarned = (double)Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == vcCustomer.id).Sum(o => o.VenueCashEarned_Total);

                    // store details
                    response.store_address1 = store_profile_mst.store_address1;
                    response.store_name = store_profile_mst.store_name;
                    response.image_url = vcStoreProfileMst.image_url;

                    // Send email
                    // get customer info
                    var customer = Context.vc_customers2.Find(customerID);
                    var store = Context.store_profile_mst2.Find(transaction.StoreId);
                    VenueCashEmails.salesOrderConfirmationCustomer(customer.Email,customer.FirstName,store.store_name,store.store_address1,transaction, vcCustomerCreditCard);
                    // salesOrderConfirmationCustomer(string email, string firstName,string store_name,string store_address, vc_CustomerTransactions transaction, vc_CustomerCreditCards creditCard)

                    return Request.CreateResponse(HttpStatusCode.OK, response);

                } // end if payment approved
                else
                {
                    Send_payment_post_resp response = new Send_payment_post_resp();
                    response.CustomerTransactionId = 0;
                    response.ProcessorConfirmationId = null;
                    response.Status = "denied";
                    response.VenueCashEarned_Total = 0;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                
               
            }
            catch (PayPal.PayPalException ex)
            {
                Send_payment_post_resp response = new Send_payment_post_resp();
                response.CustomerTransactionId = 0;
                response.ProcessorConfirmationId = null;
                response.Status = "denied";
                response.VenueCashEarned_Total = 0; 

                return Request.CreateResponse(HttpStatusCode.OK, response);


            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception err)
            {
                if (err.InnerException == null)
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "adil.boukdair@gmail.com", "Erorr while Casheir Loged in", err.Message);
                }
                else
                {
                    MailLogService.SendMail("shiftmanager@shiftReports.com", "adil.boukdair@gmail.com", "Erorr while Casheir Loged in", err.InnerException.Message);
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }


        /*
        // Need an other look with john
        [HttpPost]
		public HttpResponseMessage SendPayment_for_Approval(SendPaymentRequestBModel data)
		{
			try
			{
				Send_payment_post_resp resp = new Send_payment_post_resp();
				//////////////////////////
				var customerTransaction = Context.vc_CustomerTransactions2.Where(o => o.Id == data.CustomerTransactionId).FirstOrDefault();// add it affter test  && o.CustomerId==CustomerId

				if (customerTransaction == null)
				{
					throw new AppException(-1, "Not Found");
				}
				else
				{
					// Step 1
					customerTransaction.Status = "Customer_Submitted";
					// Step 2,3
					using (AppModel Context2=new AppModel())
					{
						var c1 = Context2.vc_store_profile_mst2.Where(r => r.store_id == customerTransaction.StoreId).FirstOrDefault();
						customerTransaction.VenueCashEarned_Other = c1.cash_back_rate * data.Paid_VenueCash_Amount;
						customerTransaction.VenueCashEarned_Total = customerTransaction.VenueCashEarned_Other;

						Context2.Dispose();

					}
					// Step 4
					Double new_VenueCashBalance = 0;
					using (AppModel Context3=new AppModel())
					{
						var c2 = Context3.vc_Customer_Rewards_Balances2.Where(r => r.store_id == customerTransaction.StoreId && r.CustomerId==data.CustomerId).FirstOrDefault();
						 new_VenueCashBalance =Convert.ToDouble( c2.VenueCashBalance + customerTransaction.VenueCashEarned_Total - data.Paid_VenueCash_Amount);

						Context3.Entry(c2).State = EntityState.Modified;
						Context3.SaveChanges();
						Context3.Dispose();

					}
					// Step 5,6
					using(AppModel Context4=new AppModel())
					{
						var c3 = Context4.vc_customers2.Find(data.CustomerId);
						c3.TotalVenueCash = new_VenueCashBalance;
						c3.TotalAllCash = c3.TotalAllCash - data.Paid_AllCash_Amount;
						Context4.Entry(c3).State = EntityState.Modified;
						Context4.SaveChanges();
						Context4.Dispose();
					}
					int tip = 0;
					using(AppModel Context5=new AppModel())
					{
						tip =Convert.ToInt32( (from x in Context5.vc_StoreRegisters2
								from y in Context5.shift_details_mst2
								where y.id == x.shift_id && x.Cashier_ID_on_duty == customerTransaction.cashier_id && x.store_id == customerTransaction.StoreId
								select y.Tip_Payout_Amount).FirstOrDefault());

						
					}
					using(AppModel Context6 = new AppModel())
					{
					
					}

					////if (true) // Give Reward only if VenueCash is not used
					//{
					//	// update status to Customer_Submitted before calling the paypal API and processing the card
					//	customerTransaction.Status = "Customer_Submitted";
					//	// VenueCash and allCash Amount
					//	customerTransaction.Paid_VenueCash_Amount = data.Paid_VenueCash_Amount;
					//	customerTransaction.Paid_AllCash_Amount = data.Paid_AllCash_Amount;
					//	customerTransaction.Paid_Credit_Amount = (customerTransaction.OtherAmount + customerTransaction.GasAmount) - (customerTransaction.Paid_VenueCash_Amount + customerTransaction.Paid_AllCash_Amount);
					//	var storeProfile = Context.vc_store_profile_mst2.Where(o => o.store_id == customerTransaction.StoreId).FirstOrDefault();

					//	var VenueCash_Other_Awarded = (customerTransaction.Paid_Credit_Amount * storeProfile.cash_back_rate) / 100;
					//	//var VenueCash_Gas_Awarded = (customerTransaction.GasAmount * storeProfile.gas_back_rate) / 100;


					//	//VenueCashEarned_Other = other_cash_back_rate x Paid_Credit_Amount

					//	var id = Convert.ToInt32(customerTransaction.CustomerId);
					//	var rewardBalance = Context.vc_Customer_Rewards_Balances2.Where(o => o.store_id == customerTransaction.StoreId && o.CustomerId == id).FirstOrDefault();
					//	if (rewardBalance != null) // if record already exist update VenueCash balance
					//	{
					//		rewardBalance.VenueCash_Other_Awarded = (int)(rewardBalance.VenueCash_Other_Awarded + rewardBalance.VenueCash_Other_Awarded + (int)VenueCash_Other_Awarded);
					//		rewardBalance.VenueCash_Gas_Awarded = (int)(rewardBalance.VenueCash_Gas_Awarded + rewardBalance.VenueCash_Gas_Awarded);// + VenueCash_Gas_Awarded);
					//		rewardBalance.VenueCashBalance = (int)(rewardBalance.VenueCashBalance + rewardBalance.VenueCash_Other_Awarded + rewardBalance.VenueCash_Gas_Awarded);
					//		Context.Entry(rewardBalance).State = EntityState.Modified; // update record


					//	}
					//	else
					//	{
					//		var createRewardBalance = new vc_Customer_Rewards_Balances();
					//		createRewardBalance.store_id = (int)customerTransaction.StoreId;
					//		createRewardBalance.CustomerId = Convert.ToInt32(customerTransaction.CustomerId);
					//		createRewardBalance.VenueCash_Other_Awarded = (int)VenueCash_Other_Awarded;
					//		//	createRewardBalance.VenueCash_Gas_Awarded = (int)VenueCash_Gas_Awarded;
					//		//	createRewardBalance.VenueCashBalance = (float)(VenueCash_Other_Awarded + VenueCash_Gas_Awarded);
					//		createRewardBalance.VenueCashBalance = (float)(VenueCash_Other_Awarded);
					//		Context.vc_Customer_Rewards_Balances2.Add(createRewardBalance);
					//	}

					//	customerTransaction.VenueCashEarned_Other = (float)VenueCash_Other_Awarded;
					//	//customerTransaction.VenueCashEarnedGas = VenueCash_Gas_Awarded;

					//	var currentUser = Context.vc_customers2.Where(r => r.id == data.CustomerId).FirstOrDefault();
					//	currentUser.TotalVenueCash = currentUser.TotalVenueCash + VenueCash_Other_Awarded;// + VenueCash_Gas_Awarded;
					//	Context.Entry(currentUser).State = EntityState.Modified;
						

						//#region submit payment
						//// Add Paypal Transactions here
						////PaypalHelper.SendTransaction()
						//var cc = (from x in Context.vc_CustomerTransactions2
						//		  from y in Context.vc_CustomerCreditCards2
						//		  where x.CardId == y.Id && x.Id == data.CustomerTransactionId
						//		  select y).FirstOrDefault();

						//if (cc == null)
						//	throw new AppException(data.CustomerId, "No crdit card on file");

						//var C = Context.vc_customers2.Find(data.CustomerId);
						//try
						//{
						//	PaypalHelper.SendTransaction(cc.CardNumber, cc.ExpirationDate, C.FirstName, C.LastName, currentUser.TotalVenueCash.ToString(), cc.CardType, "USD");
						//	Context.SaveChanges();
						//}
						//catch(PayPal.PaymentsException pex)
						//{
						//	MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", pex.Message);
						//	return Request.CreateResponse(HttpStatusCode.OK, new { error = 9 });
						//}
						//catch(PayPal.PayPalException ppex)
						//{
						//	MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", ppex.Message);
						//	return Request.CreateResponse(HttpStatusCode.OK, new { error=9});
						//}
						//#endregion


						

						resp.Status = customerTransaction.Status;
						resp.VenueCashEarned_Total = Convert.ToInt32(customerTransaction.VenueCashEarned_Total);
						resp.ProcessorConfirmationId = customerTransaction.ProcessorConfirmationId;
						resp.CustomerTransactionId = customerTransaction.Id;

					//}



				}
				return Request.CreateResponse(HttpStatusCode.OK, resp);
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

    */
		
		[HttpGet]
		public HttpResponseMessage GetCreditCard([FromUri] int card_id )
		{
			try
			{


				var x = (from n in Context.vc_CustomerCreditCards2
						 where n.Id==card_id
						select new {
							id =n.Id,
							CustomerId = n.CustomerId,
							NameOnCard = n.NameOnCard,
							CardType = n.CardType,
							CardNumber = n.CardNumber,
							SecurityCode = n.SecurityCode,
							ExpirationDate = n.ExpirationDate,
							IsDefault = n.IsDefault

						}).FirstOrDefault();
				return Request.CreateResponse(HttpStatusCode.OK, x);
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

		// Ask john
		[HttpGet]
		public HttpResponseMessage yelp_business_search()
		{
			try
			{
				

				
				return Request.CreateResponse(HttpStatusCode.OK, new {

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

		[HttpGet]
		public HttpResponseMessage yelp_business_profile([FromUri] string id)
		{
			try
			{
				YelpHelper Y = new YelpHelper();
				//var yelp = Y.GetYelpStore(id);
				return Request.CreateResponse(HttpStatusCode.OK, new { });
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
		public HttpResponseMessage vc_business_profile([FromUri] int id)
		{
			try
			{

				var s = Context.store_profile_mst2.Find(id);
				if (s == null)
					throw new AppException(id, "This store is not in the store_profile_mst ");
				var b1 = (from r in Context.vc_business_hourse_mst2
						  where r.store_id == id
						  select new
						  {
							  is_open_now = r.is_open_now,
							  open_time = r.open_time,
							  close_time = r.close_time,
							  day = r.day,
						  }).ToList();
				var b2 = Context.vc_store_profile_mst2.Where(r => r.store_id == id).FirstOrDefault();
				if (b2 == null)
					throw new AppException(id, "This store is not in the vc_store_profile_ms");

				var b3 = Context.business_details_mst2.Where(r => r.store_id == id).FirstOrDefault();
				if (b3 == null)
					throw new AppException(id, "This store is not in the business_details_mst");
				var b4 = Context.vc_BusinessCategories2.Where(r => r.store_id == id).ToList();
				if (b4 == null)
					throw new AppException(id, "This store is not in the vc_BusinessCategories");
				var rv_count = Context.vc_business_review_mst2.Where(r => r.store_id == id).ToArray().Length;

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					is_gas_station = b2.is_gas_station,
					other_cash_back_rate =b2.cash_back_rate, // talk with john about it
					gas_back_rate = 0, // talk with john about it
					cash_back_rate =b2.cash_back_rate, 
					latitude = b2.latitude  ,
					longitude = b2.longitude,
					Rating = b2.Rating ,
					Website = b2.website,
					Category_Name = "",
					image_url = s.Store_images,
					price = b3.price,
					review_count = rv_count,
					photos = "",//  talk with john
					hours = b1,
					categories = b4,
					refund_policy ="", // talk with john
					store_name=s.store_name, // from store_profile_mst
					store_address2=s.store_address2,
					store_address1=s.store_address2,
					store_city=s.store_city,
					store_state=s.store_city,
					store_zip=s.store_zip,
					store_phone_no=s.store_phone_no,
					store_email=s.store_email,
					store_wifi_pw=s.store_wifi_name,
					store_wifi_name=s.store_wifi_pw,
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
		public HttpResponseMessage update_transaction_status(customer_update_transaction_post_req data)
		{
			try
			{
				ConvertAllVenueCashToAllCashConvertAll_post_resp rep = new ConvertAllVenueCashToAllCashConvertAll_post_resp();
				var t = Context.vc_CustomerTransactions2.Where(r => r.Id == data.CustomerTransactionId).FirstOrDefault();
				if(t!=null)
				{
					t.Status = data.Status;
					t.modify_dt = DateTime.Now;
					t.Modify_usrid = "system";
					Context.Entry(t).State = EntityState.Modified;
					Context.SaveChanges();
				}
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
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
		public HttpResponseMessage customer_cancel_transaction(customer_cancel_transaction_post_req data)
		{
			try
			{
				var transaction = Context.vc_CustomerTransactions2.Where(r => r.Id == data.CustomerTransactionId).FirstOrDefault();
				if (transaction != null)
				{
                    if (transaction.Status.ToUpper() == "PENDING_CUSTOMER_APPROVAL" || transaction.Status.ToUpper() == "PENDING_CASHIER_REQUEST" || transaction.Status.ToUpper() == "CUSTOMER_SUBMITTED" || transaction.Status.ToUpper() == "READY_TO_PUMP")
                    {
                    // check if GasPump
                    var register = Context.vc_StoreRegisters2.Find(transaction.RegisterId);
                    if (register.IsGasPump==1)
                    {
                            if (transaction.AuthorizationID != null)
                            {
                                var paymentGateWayResponse = PayPalProcessor.VoidAuthorize(transaction.AuthorizationID, 50);
                            }
                    }

                    transaction.Status = "CANCELLED";
                    transaction.modify_dt = DateTime.Now;
                    transaction.Modify_usrid = "system";
					Context.Entry(transaction).State = EntityState.Modified;
					Context.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, 0);// OK
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -1);// can't cancel this transaction
                    }
                    
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -3);// not transaction found
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
        public HttpResponseMessage setPin(setPinInput  data)
        {
            try
            {

                var customer = Context.vc_customers2.Find(data.customer_id);

                customer.Pin = data.pin;

                Context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, 0);// OK

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
        public HttpResponseMessage checkPin(setPinInput data)
        {
            try
            {

                var customer = Context.vc_customers2.Find(data.customer_id);

                if (customer.Pin == data.pin)
                {
                    customer.AccessFailedCount = 0;
                    Context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, 0);// OK
                }
                else
                {

                    // check number of attemps

                    if (customer.AccessFailedCount >= 3)
                    {
                     //   customer.LockoutEndDateUtc = DateTime.Now.AddMinutes(2);
                        Context.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, -2);// Wait for 2 minutes before try
                    }
                    else
                    {
                        customer.AccessFailedCount = customer.AccessFailedCount + 1;
                        return Request.CreateResponse(HttpStatusCode.OK, -1);// pin not correct
                    }

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



        /* ################# Customer Coupon API  ######################################*/

        [HttpPost]
        public HttpResponseMessage redeemCoupon(redeemCouponInput data)
        {
            try
            {

                //Get coupon Code by id
                var coupon = Context.vc_coupon_code_mst2.Find(data.Coupon_Code_ID);
                // check if coupon exist on db
                if (coupon == null) {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// No coupon found
                }

                //check coupon Code Type

                //##### Coupon_Type 3 ##############################################################################################################################
                if (coupon.Code_Type == 3)
                {
                    // 1 - check the 'Code_Redeemed' value 
                    if (coupon.Code_Redeemed)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -2);// Coupon already redeemed
                    }
                     if ((coupon.Coupon_Code_Start_Date <= data.Date_Redeemed_Customer) && (coupon.Coupon_Code_Expire_Date >= data.Date_Redeemed_Customer))
                    {   // Security issue if the customer is manipulating his device date he will be able to use expired coupons

                        // Processing Coupon type 3
                        //2- Update the VenueCashBalance in the vc_CustomerRewardsBalance 
                        //2.1 get the promoter store_id
                        var promoter = Context.promoters_mst2.Find(coupon.promoter_userid);
                        //2.2 updating venuecashBalance in vc_customer_rewards_balance table
                        var customerRewardsBalance = Context.vc_Customer_Rewards_Balances2.Where(b => b.CustomerId == data.customer_id && b.store_id == promoter.Store_ID).FirstOrDefault();
                        if (customerRewardsBalance != null) // Balance already exist for that store
                        {
                            customerRewardsBalance.VenueCashBalance = customerRewardsBalance.VenueCashBalance + coupon.VenueCash_Credit_Amt;
                            customerRewardsBalance.VenueCash__Available_balance = customerRewardsBalance.VenueCashBalance;
                            customerRewardsBalance.VenueCashEarned_Total = customerRewardsBalance.VenueCashEarned_Total + coupon.VenueCash_Credit_Amt;
                            customerRewardsBalance.VenueCash_Awarded = customerRewardsBalance.VenueCashEarned_Total;
                        }
                        else
                        {//2.3 Balance does'nt exist for that store create one // USE CASE CAUSING ISSUE 
                            vc_Customer_Rewards_Balances newRewardsBalance = new vc_Customer_Rewards_Balances();
                            newRewardsBalance.create_dt = DateTime.Now;
                            newRewardsBalance.created_userid = "System";
                            newRewardsBalance.modify_dt = DateTime.Now;
                            newRewardsBalance.Modify_usrid = "System";
                            newRewardsBalance.store_id = promoter.Store_ID;
                            newRewardsBalance.CustomerId = data.customer_id;
                            newRewardsBalance.VenueCashBalance = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.VenueCash__Available_balance = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.AllCash__Available_balance = 0;
                            newRewardsBalance.VenueCash_Awarded = 0;
                            newRewardsBalance.VenueCash_Other_Awarded = 0;
                            newRewardsBalance.VenueCash_Gas_Awarded = 0;
                            newRewardsBalance.VenueCash_Awarded = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.VenueCashEarned_Total = coupon.VenueCash_Credit_Amt;
                            Context.vc_Customer_Rewards_Balances2.Add(newRewardsBalance);
                        }

                        //2.4 updating balance in vc_customer table
                        var customer = Context.vc_customers2.Find(data.customer_id);
                        customer.TotalVenueCash = customer.TotalVenueCash + coupon.VenueCash_Credit_Amt;

                        //3 update code redeemed to true
                        coupon.Code_Redeemed = true;

                        //4 Add new record in coupon_transactions_mst
                        coupon_code_transactions couponCodeTransaction = new coupon_code_transactions();

                        couponCodeTransaction.Coupon_Code_ID = coupon.Coupon_Code_ID;
                        couponCodeTransaction.VenueCash_Credit_Amt = coupon.VenueCash_Credit_Amt;
                        couponCodeTransaction.SignUP_VenueCash_Credit_Amt = coupon.SignUP_VenueCash_Credit_Amt;
                        couponCodeTransaction.SignUP_VC_Credit_Issued_by_VenueCash_Amt = coupon.SignUP_VC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.SignUP_AC_Credit_Issued_by_VenueCash_Amt = coupon.SignUP_AC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.VC_Credit_Issued_by_VenueCash_Amt = coupon.VC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.AC_Credit_Issued_by_VenueCash_Amt = coupon.AC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.Promoter_business_Commission_Sign_Up_Amt = coupon.Promoter_business_Commission_Sign_Up_Amt;
                        couponCodeTransaction.Promoter_customer_Commission_Sign_Up_Amt = coupon.Promoter_customer_Commission_Sign_Up_Amt;
                        couponCodeTransaction.customer_ID = data.customer_id;
                        couponCodeTransaction.promoter_userid = promoter.promoters_userid;
                        couponCodeTransaction.Date_Redeemed_Server = DateTime.Now;
                        couponCodeTransaction.Date_Redeemed_Customer = data.Date_Redeemed_Customer;
                        couponCodeTransaction.PaidOut = false;

                        Context.coupon_code_transactions2.Add(couponCodeTransaction);
                        Context.SaveChanges();

                        // Create Response
                        var storeProfile = (from s in Context.store_profile_mst2
                                            from vc_s in Context.vc_store_profile_mst2
                                            where s.id == vc_s.store_id
                                            where s.id == promoter.Store_ID
                                            select new
                                            {
                                                store_name = s.store_name,
                                                store_address1 = s.store_address1,
                                                vc_s.image_url,
                                            }
                                             ).FirstOrDefault();


                        // Send VC email
                        //couponRedeemedCustomer(string email, string name, double couponAmount,string store_name)
                        VenueCashEmails.couponRedeemedCustomer(customer.Email,customer.FirstName, coupon.VenueCash_Credit_Amt, storeProfile.store_name);

                        return Request.CreateResponse(HttpStatusCode.OK,
                            new {
                                store_name = storeProfile.store_name,
                                store_address1 = storeProfile.store_address1,
                                image_url = storeProfile.image_url,
                                venueCashEarned = coupon.VenueCash_Credit_Amt,
                                TotalVenueCash = customer.TotalVenueCash,
                                TotalAllCash = customer.TotalAllCash
                            });// Coupon already redeemed
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -3);// Coupon expired                       
                    }
                }
                //##### Coupon_Type 4 ##############################################################################################################################

                if (coupon.Code_Type == 4)
                {
                    //1 check if coupon code valid : already used by the customer
                    var couponCodeTransactionVerification = Context.coupon_code_transactions2.Where(c=>c.customer_ID==data.customer_id && c.Coupon_Code_ID==data.Coupon_Code_ID).FirstOrDefault();
                    if (couponCodeTransactionVerification != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -2);// Coupon already redeemed
                    }
                    if ((coupon.Coupon_Code_Start_Date <= data.Date_Redeemed_Customer) && (coupon.Coupon_Code_Expire_Date >= data.Date_Redeemed_Customer))
                    {   // Security issue if the customer is manipulating his device date he will be able to use expired coupons

                        // Processing Coupon type 4
                        //2- Update the VenueCashBalance in the vc_CustomerRewardsBalance 
                        //2.1 get the promoter store_id
                        var promoter = Context.promoters_mst2.Find(coupon.promoter_userid);
                        //2.2 updating venuecashBalance in vc_customer_rewards_balance table
                        var customerRewardsBalance = Context.vc_Customer_Rewards_Balances2.Where(b => b.CustomerId == data.customer_id && b.store_id == promoter.Store_ID).FirstOrDefault();
                        if (customerRewardsBalance != null) // Balance already exist for that store
                        {
                            customerRewardsBalance.VenueCashBalance = customerRewardsBalance.VenueCashBalance + coupon.VenueCash_Credit_Amt;
                            customerRewardsBalance.VenueCash__Available_balance = customerRewardsBalance.VenueCashBalance;
                            customerRewardsBalance.VenueCashEarned_Total = customerRewardsBalance.VenueCashEarned_Total + coupon.VenueCash_Credit_Amt;
                            customerRewardsBalance.VenueCash_Awarded = customerRewardsBalance.VenueCashEarned_Total;
                        }
                        else
                        {//2.3 Balance does'nt exist for that store create one // USE CASE CAUSING ISSUE 
                            vc_Customer_Rewards_Balances newRewardsBalance = new vc_Customer_Rewards_Balances();
                            newRewardsBalance.create_dt = DateTime.Now;
                            newRewardsBalance.created_userid = "System";
                            newRewardsBalance.modify_dt = DateTime.Now;
                            newRewardsBalance.Modify_usrid = "System";
                            newRewardsBalance.store_id = promoter.Store_ID;
                            newRewardsBalance.CustomerId = data.customer_id;
                            newRewardsBalance.VenueCashBalance = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.VenueCash__Available_balance = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.AllCash__Available_balance = 0;
                            newRewardsBalance.VenueCash_Awarded = 0;
                            newRewardsBalance.VenueCash_Other_Awarded = 0;
                            newRewardsBalance.VenueCash_Gas_Awarded = 0;
                            newRewardsBalance.VenueCash_Awarded = coupon.VenueCash_Credit_Amt;
                            newRewardsBalance.VenueCashEarned_Total = coupon.VenueCash_Credit_Amt;
                            Context.vc_Customer_Rewards_Balances2.Add(newRewardsBalance);
                        }
                        //2.3 updating balance in vc_customer table
                        var customer = Context.vc_customers2.Find(data.customer_id);
                        customer.TotalVenueCash = customer.TotalVenueCash + coupon.VenueCash_Credit_Amt;

                        //3 update code redeemed to true
                        coupon.Code_Redeemed = true;
                        //4 Add new record in coupon_transactions_mst
                        coupon_code_transactions couponCodeTransaction = new coupon_code_transactions();

                        couponCodeTransaction.Coupon_Code_ID = coupon.Coupon_Code_ID;
                        couponCodeTransaction.VenueCash_Credit_Amt = coupon.VenueCash_Credit_Amt;
                        couponCodeTransaction.SignUP_VenueCash_Credit_Amt = coupon.SignUP_VenueCash_Credit_Amt;
                        couponCodeTransaction.SignUP_VC_Credit_Issued_by_VenueCash_Amt = coupon.SignUP_VC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.SignUP_AC_Credit_Issued_by_VenueCash_Amt = coupon.SignUP_AC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.VC_Credit_Issued_by_VenueCash_Amt = coupon.VC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.AC_Credit_Issued_by_VenueCash_Amt = coupon.AC_Credit_Issued_by_VenueCash_Amt;
                        couponCodeTransaction.Promoter_business_Commission_Sign_Up_Amt = coupon.Promoter_business_Commission_Sign_Up_Amt;
                        couponCodeTransaction.Promoter_customer_Commission_Sign_Up_Amt = coupon.Promoter_customer_Commission_Sign_Up_Amt;
                        couponCodeTransaction.customer_ID = data.customer_id;
                        couponCodeTransaction.promoter_userid = promoter.promoters_userid;
                        couponCodeTransaction.Date_Redeemed_Server = DateTime.Now;
                        couponCodeTransaction.Date_Redeemed_Customer = data.Date_Redeemed_Customer;
                        couponCodeTransaction.PaidOut = false;

                        Context.coupon_code_transactions2.Add(couponCodeTransaction);
                        Context.SaveChanges();
                        // Create Response
                        var storeProfile = (from s in Context.store_profile_mst2
                                            from vc_s in Context.vc_store_profile_mst2
                                            where s.id == vc_s.store_id
                                            where s.id == promoter.Store_ID
                                            select new
                                            {
                                                store_name = s.store_name,
                                                store_address1 = s.store_address1,
                                                vc_s.image_url,
                                            }
                                             ).FirstOrDefault();

                        // Send VC email
                        //couponRedeemedCustomer(string email, string name, double couponAmount,string store_name)
                        VenueCashEmails.couponRedeemedCustomer(customer.Email, customer.FirstName, coupon.VenueCash_Credit_Amt, storeProfile.store_name);

                        return Request.CreateResponse(HttpStatusCode.OK,
                            new
                            {
                                store_name = storeProfile.store_name,
                                store_address1 = storeProfile.store_address1,
                                image_url = storeProfile.image_url,
                                venueCashEarned = coupon.VenueCash_Credit_Amt,
                                TotalVenueCash=customer.TotalVenueCash,
                                TotalAllCash=customer.TotalAllCash
                            });// Coupon already redeemed

                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, -3);// Coupon expired      
                    }




                }


 


                    return Request.CreateResponse(HttpStatusCode.OK, -1); 

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
        public HttpResponseMessage forgotPassword([FromUri] string email)
        {
            var customer = Context.vc_customers2.Where(c => c.Email == email).FirstOrDefault();
            
            if(customer != null)
            {

                VenueCashEmails.forgotPassword(customer.Email, customer.FirstName, customer.PasswordHash);

                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,-1);
            }
           


        }

    }
}
