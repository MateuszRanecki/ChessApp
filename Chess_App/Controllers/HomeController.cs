using Chess_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Chess_App.Controllers
{
    [Authorize]
    public class HomeController : Controller    {
        
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Multi() 
        {
            return View();
        }

        public IActionResult History() 
        {
            return View();
        }
        public IActionResult ViewPostion() 
        {
            return PartialView();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult PlayComputer()
        {
            return View();
        }

        public IActionResult SetPosition()
        {
            return View();
        }

        public IActionResult Tutorial()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
