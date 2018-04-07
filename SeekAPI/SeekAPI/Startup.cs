using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using ArangoDB.Client;
using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JobModel.AutoFac;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Builder;

namespace SeekAPI
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }

        public IConfiguration Configuration { get; }

        public const string CorsPolicy = "Cors";

        public const string ArangoConnectionId = "_system";

        public const string SwaggerApiName = "job-api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The return type IServiceProvider is very important to get AutoFac working. Without this returned IServiceProvider, AutoFac will fail.</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            AutoFacContainer autoFacContainer = new AutoFacContainer();

            ContainerBuilder builder = autoFacContainer.ContainerBuilder;

            builder.RegisterInstance(new JobModel.Entities.JobCategory()
            {
                _key = "2034"
            });

            services.AddCors(options =>
                    options.AddPolicy(
                        CorsPolicy,
                        corsBuilder =>
                            corsBuilder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                        )
                );

            services.AddMvc().AddJsonOptions(json =>
                {
                    json.SerializerSettings.Error = OnJsonError;
                    json.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddSwaggerGen(
                setup => 
                    setup.SwaggerDoc(SwaggerApiName,
                    new Info
                    {
                        Version = "1",
                        Title = "Job API Server",
                        Description = "Job API",
                        TermsOfService = "N/A"
                    })
                );

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);

        }

        public void OnJsonError(object source, ErrorEventArgs error)
        {
            Debugger.Break();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(CorsPolicy);
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(setup => setup.SwaggerEndpoint($"/swagger/{SwaggerApiName}/swagger.json", "Job API"));
            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute("spaFallback", new { controller = "Home", action = "Spa" });
            });
        }
    }
}
