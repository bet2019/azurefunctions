using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Configuration;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp4ASA
{
    public static class function4asa
    {
        [FunctionName("function4asa")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            try
            {
                var redisConnectString = ConnectionMultiplexer.Connect("danielshrc.redis.cache.windows.net:6380,password=fBEKVW0gdlGMC8YkeaiE1bbldy3+3qLtbmk55mK7pak=,ssl=True,abortConnect=False");
                log.LogInformation($"Connection string.. {redisConnectString}");

                IDatabase db = redisConnectString.GetDatabase();
                log.LogInformation($"Created database {db}");

                string time = data.time;
                string callingnum = data.callingnum;
                string key = time + '-' + callingnum;
                db.StringSet(key, data.ToString());

                string responseMessage = "This HTTP triggered function executed successfully.";
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                return new OkObjectResult("bad");
            }
        }
    }
}
