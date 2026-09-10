using Microsoft.AspNetCore.Mvc;

namespace HospitalClinicMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Error() => View();
    }
}
