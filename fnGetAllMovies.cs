using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace lyto.dioflix
{
    public class fnGetAllMovies
    {
        private readonly ILogger<fnGetAllMovies> _logger;

        public fnGetAllMovies(ILogger<fnGetAllMovies> logger)
        {
            _logger = logger;
        }

        [Function("fnGetAllMovies")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
