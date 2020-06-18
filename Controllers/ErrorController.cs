using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace RoomMonitor.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/api/error-local-development")]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/api/error")]
        public IActionResult Error()
        {
            // var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            // if (context.Error.GetType() == typeof(SqlException))
            // {
            //     SqlException sqlException = context.Error as SqlException;
            //     SqlError error = sqlException.Errors[0];
            //     // Custom sql server errors are returned with code 50000
            //     if (error.Number == 50000)
            //     {
            //         return ValidationProblem(
            //             title: error.Message
            //             );
            //     }
            // }
            return Problem();
        }
    }
}