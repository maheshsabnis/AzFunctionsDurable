using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using HttpCalls.Models;
using HttpCalls.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;

namespace HttpCalls
{
    public class DeptApi
    {
        private readonly ILogger _logger;
        private IServices<Department, int> deptServ;
        ResponseObject<Department> response;    

        public DeptApi(ILoggerFactory loggerFactory, IServices<Department, int> serv)
        {
            _logger = loggerFactory.CreateLogger<DeptApi>();
            deptServ = serv;
            response = new ResponseObject<Department>();
        }   

        [Function("DeptApi")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete")] HttpRequestData req)
        {
            
            var resp = req.CreateResponse(HttpStatusCode.OK);
           // resp.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            switch (req.Method)
            {
                case "GET":
                    response = await deptServ.GetAsync();
                   
                    break;
                case "POST":
                    var postbody = new StreamReader(req.Body).ReadToEnd();
                    var newdept = JsonSerializer.Deserialize<Department>(postbody);
                    response = await deptServ.CreateAsync(newdept);
                   
                    break;
                case "PUT":
                    var id = Convert.ToInt32(req.Url.Query.Split('=')[1]);
                    var putbody = new StreamReader(req.Body).ReadToEnd();
                    var depttoupdate = JsonSerializer.Deserialize<Department>(putbody);
                    response = await deptServ.UpdateAsync(id, depttoupdate);
                     
                    break;
                case "DELETE":
                     id = Convert.ToInt32( req.Url.Query.Split('=')[1]);
                    response = await deptServ.DeleteAsync(id);
                    break;
                default:
                    break;
            }

            await resp.WriteAsJsonAsync(response);

            return resp;
        }
    }
}
