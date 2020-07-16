using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CheckOutDemoApp.Controllers
{
    public class ChargeController : Controller
    {
        [HttpGet]
        public IActionResult CreateCharge()
        {
            return View();
        }
    }
}
