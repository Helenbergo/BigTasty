using Microsoft.AspNetCore.Mvc;
using App.Filters;

namespace Produtos_Com_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]

    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Banner()
        {
            return View();
        }
    }
}