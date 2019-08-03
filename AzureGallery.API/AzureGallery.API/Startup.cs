using AzureGallery.Models.HelperModels;
using AzureGallery.Services.IServices;
using AzureGallery.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using AzureGallery.Context;

namespace AzureGallery.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //MY SERVICE
            services.AddScoped<IAzureService, AzureService>();
            services.AddScoped<IIOService, IOService>();
            services.AddScoped<IJwtAuthService, JwtAuthService>();
            services.AddScoped<IHasherService, HasherService>();

            //AUTO MAPPER
            services.AddAutoMapper(o => { o.ValidateInlineMaps = false; });

            //DB CONTEXT
            services.AddDbContext<AzureGalleryContext>(o =>
            {
                o.UseInMemoryDatabase("AzureGalleryContextDb");
            });

            //AUTHENTICATION
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SignatureKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AngularApp", o =>
                {
                    o.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
                });
            });

            //SWAGGER
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "AzureGallery.API", Version = "v1" });
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AzureGalleryContext context)
        {
            Hosting.Environment = env;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            context.SeedData();

            app.UseCors("AngularApp");
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureGallery.API"); });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

    }
}
