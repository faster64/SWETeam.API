using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SWETeam.Common.Entities;
using SWETeam.Common.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWETeam.API
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
            services.AddSingleton(_ => Configuration);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region CORS
            services.AddCors();
            #endregion

            #region Authen
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = Configuration["jwt_settings:issuer"],
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["jwt_settings:issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt_settings:key"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region AddController + Uppercase first letter
            services.AddControllersWithViews()
             .AddJsonOptions(options =>
                 options.JsonSerializerOptions.PropertyNamingPolicy = null
             );
            #endregion

            #region Global filters
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new GlobalHeader(services.BuildServiceProvider()));
                options.Filters.Add(new RevokeTokenInterceptor());
            });
            #endregion

            #region Distributed Cache Redis
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "swe_team";
            });
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(Startup));
            #endregion

            #region FluentValidation
            //services.AddScoped<IValidator<UserCred>, UserCredValidator>();
            //services.AddValidatorsFromAssemblyContaining<UserCredValidator>();
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SWE Team API",
                    Version = "v1",
                    Description = "",
                    Contact = new OpenApiContact
                    {
                        Name = "SWE Team",
                        Email = "cuongnguyen.ftdev@gmail.com",
                        Url = new Uri("https://facebook.com/cuongnguyen.ftdev"),
                    },
                });
            });
            #endregion

            #region Dependency Injection
            // Base
            //services.AddScoped(typeof(IBLBase<>), typeof(BLBase<>));
            //services.AddScoped(typeof(IDLBase<>), typeof(DLBase<>));

            // Mongo
            //services.TryAddSingleton(typeof(IMongoService<>), typeof(MongoService<>));

            // Auth
            //services.AddScoped<IAuthService, AuthServicez>();

            // Device info
            //services.TryAddSingleton<IDeviceService, DeviceService>();

            // Mail
            //services.TryAddSingleton<IBLMail, BLMail>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthentication();

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SWE Team API V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            // Handle exception khi  try catch tại controller không xử lý được
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                var responseContent = new ServiceResult();

                responseContent.SetError(exception);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseContent));
            }));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
