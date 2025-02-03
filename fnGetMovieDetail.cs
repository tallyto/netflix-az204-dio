using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace netflix_az204_dio
{
    public class FnGetMovieDetail
    {
        private readonly ILogger<FnGetMovieDetail> _logger;
        private readonly CosmosClient _cosmosClient;

        public FnGetMovieDetail(ILogger<FnGetMovieDetail> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("fnGetMovieDetail")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("dioflixdb", "movies");

            var id = req.Query["id"];

            var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);

            var result = container.GetItemQueryIterator<MovieResult>(query);

            var results = new List<MovieResult>();

            while (result.HasMoreResults)
            {
              foreach (var item in await result.ReadNextAsync())
              {
                  results.Add(item);
              }
            }

            var responseMessage = req.CreateResponse(HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());

            return responseMessage;
        }
    }
}
