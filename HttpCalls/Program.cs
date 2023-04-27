using HttpCalls;
using HttpCalls.Models;
using HttpCalls.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace HttpCalls
{
    public class Program
    {
        public static void Main()
        {
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s => 
                {
                   s.AddDbContext<CompanyContext>(
                options => SqlServerDbContextOptionsExtensions.UseSqlServer(options,connectionString));
                    s.AddScoped<IServices<Department,int>, DepartmentService>();
                })  
                .Build();
            
           

            host.Run();
        }


            

    }
}