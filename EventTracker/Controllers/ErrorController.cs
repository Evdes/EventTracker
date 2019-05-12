using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EventTracker.BLL.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public IActionResult EventNotFound()
        {
            Response.StatusCode = 400;
            return View();
        }

        public IActionResult UserProfileNotFound()
        {
            Response.StatusCode = 400;
            return View();
        }
    }
}