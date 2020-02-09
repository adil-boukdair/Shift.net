using System.Web.Mvc;

namespace ExpensTrackerAPI.Controllers
{
	public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Corporate()
        {
            ViewBag.Title = "Corporate Main";

            return View();
        }
       
        
    }
}
