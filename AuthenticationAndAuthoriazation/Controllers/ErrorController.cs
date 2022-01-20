using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAndAuthoriazation.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        [HttpGet]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:

                    ViewBag.ErrorMessage = "Sorry, the resource you request cannot be found";
                    _logger.LogWarning($"404 Error occured. Path is {statusCodeResult.OriginalPath} and querystring is {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }

        [HttpGet]
        [Route("Error")]
        public IActionResult CustomErrorPage()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");
            return View();
        }
    }
}
