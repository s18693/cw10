//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
using cw3.DAL;
using cw3.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using cw3.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using cw3.Models;
using Microsoft.EntityFrameworkCore;

namespace cw3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //Dodajemy tu wszystkie serwisy
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IDbService, MockDbService>();
            //domyslne dla naszej dalszej pracy !!!!!!
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "buka",
                    ValidAudience = "studenta",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                };


            });


            services.AddSingleton<IStudentsDbService, SqlServerDbService>();
            services.AddTransient<IDbService, DbService>();
            //services.AddScoped<SchoolContext>();

            services.AddDbContext<SchoolContext>(option => option.UseSqlServer("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"));

            services.AddControllers();

            //services.AddControllers().AddXmlSerializerFormatters(); 
        }

        /*
        public void ConfigrueService(IServiceCollection service)
        {
            service.AddTransient<IDbService,DbService>();
            service.AddControllers();
        }
        */

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbService dbService)
        {
            //developkment
            if (env.IsDevelopment())
            {
                //obsluga bledow
                app.UseDeveloperExceptionPage();
            }
            //context - html, polecenie kore przychodzi
            //Middleware - kolejnoœæ ma istotne znaczenie
            /*
                        app.Use(async (context, next) =>
                        {
                            System.Console.WriteLine("StartCheck");

                            //sprawdzamy czy klucz jest
                            if (!context.Request.Headers.ContainsKey("Index"))
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("Nie podano indexu w headers");
                                return;
                            }
                            //Czy jest taki student
                            var index = context.Request.Headers["Index"].ToString();
                            if (!dbService.checkIndex(index))
                            {
                                //informacja nie ma takiego studenta
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("Nie ma takiego studenta");
                                return;
                            }
                            System.Console.WriteLine("Checked");
                            //Zapis
                            await next();
                        });
            */
            //Najpierw sprawdzamy studenta a potem zapisujemy
            //Zapis po sprawdzeniu
            //app.UseMiddleware<LoggingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
