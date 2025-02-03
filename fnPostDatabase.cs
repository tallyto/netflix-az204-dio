using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace netflix_az204_dio
{
    public class FnPostDatabase
    {
        private readonly ILogger<FnPostDatabase> _logger;

        public FnPostDatabase(ILogger<FnPostDatabase> logger)
        {
            _logger = logger;
        }

        [Function("fnPostDatabase")]
        [CosmosDBOutput(
            "%DatabaseName%", "movies", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "/id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
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
