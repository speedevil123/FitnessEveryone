using Microsoft.AspNetCore.Mvc;

namespace FitnessEveryone.Controllers
{
    public class KaloriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
