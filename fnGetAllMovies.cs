using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace netflix_az204_dio
{
    public class FnGetAllMovies(ILogger<FnGetAllMovies> logger, CosmosClient cosmosClient)
    {
        private readonly ILogger<FnGetAllMovies> _logger = logger;

        private readonly CosmosClient _cosmosClient = cosmosClient;

        [Function("fnGetAllMovies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {

            var container = _cosmosClient.GetContainer("dioflixdb", "movies");

            var query = new QueryDefinition("SELECT * FROM c");

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
            await responseMessage.WriteAsJsonAsync(results);

            return responseMessage;
        }
    }
}
