using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace StreamAnalyticsApp
{
    public static class CreateOrUpdateTracking
    {
        [FunctionName("CreateOrUpdateTracking")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Sql("Tracking", ConnectionStringSetting = "SqlConnectionString")] IAsyncCollector<Tracking> trackings)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody)) {
                return new StatusCodeResult(204);
            }

            var incomingTrackings = JsonConvert.DeserializeObject<List<Tracking>>(requestBody);

            foreach (var tracking in incomingTrackings)
            {
                await trackings.AddAsync(tracking);
            }

            await trackings.FlushAsync();

            return new OkResult();
        }
    }
}
