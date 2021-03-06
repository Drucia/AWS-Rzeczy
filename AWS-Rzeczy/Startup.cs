using Amazon;
using Amazon.CloudWatch;
using Amazon.DynamoDBv2;
using Amazon.EC2;
using Amazon.Runtime;
using Amazon.S3;
using AWS_Rzeczy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AWS_Rzeczy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            //services.Configure<MyAWSCredentials>(Configuration.GetSection("AWScredentials"));
            var awsCredentials = Configuration.GetSection("AWScredentials").Get<AWSCredentials>();
            var sessionCredentials = new SessionAWSCredentials(awsCredentials.AccessKey, awsCredentials.SecretKey, awsCredentials.SessionToken);

            var region = RegionEndpoint.USEast1; // The US East (Virginia) endpoint

            // AWS clients DI
            services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>(_ => new AmazonDynamoDBClient(sessionCredentials, region));
            services.AddTransient<IAmazonS3, AmazonS3Client>(_ => new AmazonS3Client(sessionCredentials, region));
            services.AddTransient<IS3Service, S3Service>();
            services.AddTransient<IAmazonEC2, AmazonEC2Client>(_ => new AmazonEC2Client(sessionCredentials, region));
            services.AddTransient<IAmazonCloudWatch, AmazonCloudWatchClient>(_ => new AmazonCloudWatchClient(sessionCredentials, region));
            services.AddTransient<ICloudWatchService, CloudWatchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
