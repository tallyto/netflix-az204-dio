using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace lyto.dioflix
{
    public class fnPostDatabase
    {
        private readonly ILogger<fnPostDatabase> _logger;

        public fnPostDatabase(ILogger<fnPostDatabase> logger)
        {
            _logger = logger;
        }

        [Function("fnPostDatabase")]
        [CosmosDBOutput(
            "%DatabaseName%", "movies", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "/id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {

            MovieRequest? movie = null;

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                movie = JsonConvert.DeserializeObject<MovieRequest>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deserializar o objeto");
                return new BadRequestObjectResult("Erro ao deserializar o objeto");
            }


            return JsonConvert.SerializeObject(movie);
        }
    }
}
