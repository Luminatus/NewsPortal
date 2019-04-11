using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class StatusController : Controller
    {
        [HttpGet("/error/{code?}")]
        public IActionResult Index(int code = 500)
        {
            ViewData["Status"] = code;
            switch (code)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View();
            }
        }
    }
}