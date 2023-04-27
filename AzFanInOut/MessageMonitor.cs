using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace AzFanInOut
{
    public static class MessageMonitor
    {
        [FunctionName("MessageMonitor")]
        public static async Task  RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // 1. Set the End-Time
            DateTime endTime = context.CurrentUtcDateTime.AddMinutes(1);

            //2. Perform Some Operations
            while (context.CurrentUtcDateTime < endTime)
            {
                // 3. Keep Reading Messages from Queue
                var message = await context.CallActivityAsync<string>("MessageReader", "myq");

            }

        }


        [FunctionName("MessageReader")]
        public static string MessageReader([ActivityTrigger] string queueName, ILogger log)
        {
            PeekedMessage[] peekedMessage = null;
            // Get the connection string from app settings
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Instantiate a QueueClient which will be used to manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            if (queueClient.Exists())
            {
                // Peek at the next message
                  peekedMessage = queueClient.PeekMessages();

                // Display the message
                Console.WriteLine($"Peeked message: '{peekedMessage[0].Body}'");
            }
            return peekedMessage[0].Body.ToString();
        }



        [FunctionName("MessageMonitor_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("MessageMonitor", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}