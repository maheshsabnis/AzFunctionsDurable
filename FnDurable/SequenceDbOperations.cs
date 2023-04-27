using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FnDurable.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace FnDurable
{
    public static class SequenceDbOperations
    {

        static List<ToDoItem> toDoItemList = new List<ToDoItem>();
        static ClsMessaging messaging = new ClsMessaging();
        [FunctionName("SequenceDbOperations")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));
            ToDoItem todoItem = new ToDoItem()
            {
                
                task = "Define Archirecture"
            };
            toDoItemList.Add(todoItem);
            var data = await context.CallActivityAsync<List<ToDoItem>>(nameof(AddToDb), todoItem);
           
             await context.CallActivityAsync<string>(nameof(AddToQueue), data);

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
            //jjj
        }

        //[FunctionName(nameof(SayHello))]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation("Saying hello to {name}.", name);
        //    return $"Hello {name}!";
        //}


        [FunctionName(nameof(AddToDb))]
        public static List<ToDoItem> AddToDb([ActivityTrigger] ToDoItem toDoItem, ILogger log)
        {
            List<ToDoItem> todos = new List<ToDoItem>();
            try
            {
                SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=Company;Integrated Security=SSPI;TrustServerCertificate=Yes");
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"Insert into ToDo values ('{toDoItem.task}')";
                cmd.ExecuteNonQuery();
                //conn.Close();

                cmd.CommandText = "Select * from ToDo";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    todos.Add(new ToDoItem() {Id = Convert.ToInt32(reader["id"]), task = reader["task"].ToString() });
                }
                reader.Close(); 
                Debug.WriteLine($"Done Dana Done {JsonSerializer.Serialize(todos)}");
                return todos;
            }
            catch (Exception ex)
            {
                 
            }
            return null;
        }


        [FunctionName(nameof(AddToQueue))]
        public static async Task<string> AddToQueue([ActivityTrigger] List<ToDoItem> todos, ILogger log)
        {
            await messaging.AddMessageAsync(JsonSerializer.Serialize(todos));
            return "Done dana done!!!!!!";
        }



        [FunctionName("SequenceDbOperations_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("SequenceDbOperations", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}