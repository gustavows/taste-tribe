using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using tastetribe.Models;

namespace tastetribe.Controllers
{
    public class HomeController : Controller
    {
        // Home Page
        public IActionResult Index()
        {
            return View();
        }

        // Data Visualization Page
        public IActionResult Data()
        {
            return View();
        }

        // About Page
        public IActionResult About()
        {
            return View();
        }

        // CRUD Pages
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Read()
        {
            return View();
        }

        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

        // Default
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
