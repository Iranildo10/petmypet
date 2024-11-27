using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace petmypet.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        [Area("Admin")]
        [Authorize("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
