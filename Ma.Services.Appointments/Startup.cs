﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#pragma warning disable 1591

namespace Ma.Services.Appointments
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddApiVersioning();
            services.AddAutoMapper(typeof(Startup));

            // We use JWT (JSON Web Token) to authenticate the client which use this micro service.
            var key = Encoding.UTF8.GetBytes(Configuration["JwtSecret"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>                           
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,   // Not required as no third-party is involved
                    ValidateAudience = false, // Not required as no third-party is involved
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),                   
                }
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new Info
                {
                    Title = "Appointment Management API",
                    Version = "v1.0",
                    Description = "The goal of the appointment service is to provide a cloud-based functionallity to help you to organize you time effective.",
                    Contact = new Contact
                    {
                        Name = "Alexandr Miller",
                        Email = "info@miller-alex.de"
                    },
                    License = new License
                    {
                        Url = "https://raw.githubusercontent.com/miller-alex-github/personal-site/master/LICENSE"
                    }
                });

                c.EnableAnnotations();
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });

                // Add all XML Documentation files from bin directory.
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xml => c.IncludeXmlComments(xml));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "docs/{documentName}/docs.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v1.0/docs.json", "v1.0");
                c.RoutePrefix = "docs";
            });

            app.AutoMigrateDatabase();
        }
    }
}
