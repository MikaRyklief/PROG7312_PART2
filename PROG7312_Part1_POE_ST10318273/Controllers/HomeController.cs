using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG7312_Part1_POE_ST10318273.Models;

namespace PROG7312_Part1_POE_ST10318273.Controllers
{
    /// <summary>
    /// Controller responsible for handling home page and privacy policy requests.
    /// This controller manages the main landing page and privacy information display.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Displays the main landing page with municipal services overview.
        /// This action renders the Index view which contains the hero section
        /// and service cards for issue reporting and other municipal services.
        /// </summary>
        /// <returns>Index view with municipal services information</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// This action renders the Privacy view which contains information
        /// about data collection, usage, and user privacy rights.
        /// </summary>
        /// <returns>Privacy view with policy information</returns>
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
