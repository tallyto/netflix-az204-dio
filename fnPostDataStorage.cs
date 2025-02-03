using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace netflix_az204_dio
{
    public class FnPostDataStorage
    {
        private readonly ILogger<FnPostDataStorage> _logger;

        public FnPostDataStorage(ILogger<FnPostDataStorage> logger)
        {
            _logger = logger;
        }

        [Function("fnPostDataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando a Imagem no Storage");

            try
            {
                if (!req.Headers.TryGetValue("file-type", out var fileTyeHeader))
                {
                    return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório");
                }

                var fileType = fileTyeHeader.ToString();

                var form = await req.ReadFormAsync();

                var file = form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("O arquivo não foi enviado");
                }

                string? connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string containerName = fileType;

                BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);

                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

                await containerClient.CreateIfNotExistsAsync();

                await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

                await blobClient.UploadAsync(file.OpenReadStream(), true);

                string blobName = file.FileName;

                var blob = containerClient.GetBlobClient(blobName);

                _logger.LogInformation($"Arquivo {blobName} enviado com sucesso");

                return new OkObjectResult(
                    new
                    {
                        Message = $"Arquivo {blobName} enviado com sucesso",
                        BlobUri = blob.Uri
                    }
                );


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a imagem no Storage");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


        }
    }
}
