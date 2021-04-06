using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.OpenApi.Models;
using Movies.API.Authorization;
using Movies.Infrastructure.Interfaces;
using Movies.Domain.Repositories;

namespace Movies.API
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
            services.AddControllers().AddNewtonsoftJson();

            services.AddAutoMapper(typeof(Startup));

            services.AddMvc();

            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeader<ApiKeyProvider>(options => { options.Realm = "Movies API"; options.KeyName = "X-API-KEY"; });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Movies API",
                    Version = "v1",
                    Description = "Movies Microservice API"
                });

                options.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme
                {
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-API-KEY" }
                        },
                        new string[] { }
                    }
                });
            });

            services.AddSingleton<IMovieData, MovieData>();
            services.AddSingleton<IMovieRepository, MovieRepository>();

            services.AddSingleton(_ =>
            {
                return Configuration.GetSection("ApiKeys")
                    ?.GetChildren()
                    ?.ToList()
                    ?.Select(x => new ApiKey(x.GetValue<string>("Key"), x.GetValue<string>("OwnerName")))
                    ?.ToList<IApiKey>() ?? new List<IApiKey>();
            });
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies API V1");
            });
        }
    }
}
