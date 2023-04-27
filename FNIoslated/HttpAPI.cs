using System.Net;
using System.Text.Json;
using FNIoslated.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FNIoslated
{
    public class HttpAPI
    {
        private readonly ILogger _logger;
        CompanyContext context;
         
        public HttpAPI(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpAPI>();
            
            context = new CompanyContext();
        }

        [Function("GetDept")]
        public HttpResponseData Get([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req
             )
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            var departments = context.Departments.ToList();
            response.WriteString(JsonSerializer.Serialize(departments));

            return response;
        }
        [Function("PostDept")]
        public HttpResponseData Post([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
             
            var bodyData = new StreamReader(req.Body).ReadToEnd();
            Department? dept = JsonSerializer.Deserialize<Department>(bodyData);
            var result = context.Departments.Add(dept);
            context.SaveChanges();
            response.WriteString("Record Added Successfuly");
            response.WriteAsJsonAsync( result.Entity);

            return response;
        }
    }
}
