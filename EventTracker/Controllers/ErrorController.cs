using Microsoft.AspNetCore.Mvc;

namespace EventTracker.BLL.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = NotFound().StatusCode;
            return View();
        }

        [Route("error/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }

        public IActionResult EventNotFound()
        {
            Response.StatusCode = BadRequest().StatusCode;
            return View();
        }

        public IActionResult UserProfileNotFound()
        {
            Response.StatusCode = BadRequest().StatusCode;
            return View();
        }
    }
}