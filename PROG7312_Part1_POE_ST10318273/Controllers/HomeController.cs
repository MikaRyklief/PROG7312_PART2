using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Models;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
