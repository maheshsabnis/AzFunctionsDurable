using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FnDurable
{
    public static class MyDurableFunction
    {
        [FunctionName("MyDurableFunction")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.

            var msessgeq1 = await context.CallActivityAsync<string>(nameof(QueueOne), "Tokyo");

            await Console.Out.WriteLineAsync($"DOne with the First = {msessgeq1}");

            var msessgeq2 = await context.CallActivityAsync<string>(nameof(QueueTwo), "Seattle");
            await Console.Out.WriteLineAsync($"DOne with the Second = {msessgeq2}");

            var msessgeq3 = await context.CallActivityAsync<string>(nameof(QueueThree), "London");
            await Console.Out.WriteLineAsync($"DOne with the Third = {msessgeq3}");



            //outputs.Add(await context.CallActivityAsync<string>(nameof(QueueOne), "Tokyo"));

            //outputs.Add(await context.CallActivityAsync<string>(nameof(QueueTwo), "Seattle"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(QueueThree), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        //[FunctionName(nameof(SayHello))]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation("Saying hello to {name}.", name);
        //    return $"Hello {name}!";
        //}

        [FunctionName(nameof(QueueOne))]
        [return: Queue("queue-one", Connection = "MyDemo")]
        public static string QueueOne([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to Queue 2 {name}.", name);
            return $"Hello From Queue 1 {name}!";
        }


        [FunctionName(nameof(QueueTwo))]
        [return: Queue("queue-two", Connection = "MyDemo")]
        public static string QueueTwo([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to Queue 2 {name}.", name);
            return $"Hello From Queue 2 {name}!";
        }

        [FunctionName(nameof(QueueThree))]
        [return: Queue("queue-three", Connection = "MyDemo")]
        public static string QueueThree([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to Queue 3 {name}.", name);
            return $"Hello From Queue 3 {name}!";
        }


        [FunctionName("MyDurableFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("MyDurableFunction", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
 
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}