using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HttpCalls
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            if (req.Method == "GET")
            { 
             _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");
                return response;

            }
            else
                if (req.Method == "POST")
            {
                var body = req.Body;
                var bodyData = new StreamReader(body).ReadToEnd();
                var response = req.CreateResponse(HttpStatusCode.Created);
                response.WriteString($"{bodyData} {bodyData.ToString()}");
                return response;
            }

            return null;
           
        }
    }
}
