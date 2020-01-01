using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Outmatch.API.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Outmatch.API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Outmatch.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Outmatch.API
{
    public class Startup
    {
        // Calls the configuration settings of the application 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Dependancy Injection Container - Will contain anything created to be consumed by another part of the application.  All services will be held here.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Mirosoft Identity Services to the services method
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
            {
                // Add weak password ability
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            // Add the DbContext method to services so it can be called from other aread of the webapp, and call the database connection string.
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            // Add the JET Token Service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // Add authentication as a service.  Tell DotNet Core the type of authentication
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Add roles based authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireGlobalAdminRole", policy => policy.RequireRole("GlobalAdmin"));
                options.AddPolicy("RequireClientAdminRole", policy => policy.RequireRole("GlobalAdmin", "ClientAdmin"));
                options.AddPolicy("RequireLocationAdminRole", policy => policy.RequireRole("GlobalAdmin", "ClientAdmin", "LocationAdmin"));
                options.AddPolicy("RequireReportsOnlyRole", policy => policy.RequireRole("GlobalAdmin", "ClientAdmin", "LocationAdmin", "ReportsOnly"));
            });

            // Add all other servies
            services.AddDbContext<DataContext>(x => x.UseSqlite
                (Configuration.GetConnectionString("DefaultConnection")));      // Make the DbContext service available to the rest of the application
            services.AddControllers(options => 
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson();                                               // Make the Controllers service available to the rest of the application
            services.AddCors();                                                 // Make the CORS policy service available to the rest of the application
            services.AddAutoMapper(typeof(ClientRepository).Assembly);          // Make AutoMapper service available to the rest of the application
            services.AddScoped<IClientRepository, ClientRepository>();          // Make the client repository service available to the rest of the application
            services.AddScoped<ILocationsRepository, LocationsRepository>();    // Make the locations repository service available to the rest of the application
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This is the middleware that intracts with the app and the API.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // If in production mode, pass any errors so they can be read
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            // app.UseHttpsRedirection();

            // Routes all requests to the appropriate location 
            app.UseRouting();

            // Defines a CORS policyModify the http headers to prevent the 'Access-Control-Allow-Origin' error from blocking the API.  
            // Allows different addresses to provide content from the API
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Controls authorization for logging users in and determines what areas they have access to
            app.UseAuthentication();
            app.UseAuthorization();

            // As the web applications starts up, this maps the controller endpoints into the web application so the web application knows how to route the requests.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}