using DataAccess.General;
using DataAccess.helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.common;
using Models.General;
using NLog.Extensions.Logging;
using System.Text;

namespace Event_Resistration_APIs
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
            // Add services to the container.
            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["JwtSettings:Issuer"],
                            ValidAudience = Configuration["JwtSettings:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]))
                        };
                    });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddOptions();
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));
            services.Configure<contextConfiguration>(Configuration.GetSection("ConnectionStrings"));

            services.AddTransient<Iconnection, ConnectionRepository>();

            services.AddScoped<IuserMaster, userMasterRepository>();
            services.AddScoped<IemailService, emailServiceRepository>();
            services.AddScoped<IemailConfiguration, emailConfigurationRepository>();
            services.AddScoped<IemailLog, emailLogRepository>();
            services.AddScoped<IparticipantMaster, participantRepository>();
            services.AddScoped<IeventMaster, eventRepository>();
            services.AddScoped<IotpManager, otpManagerRepository>();
        }

        public void Configure(IApplicationBuilder app
                             , IWebHostEnvironment environment
                             , IServiceProvider serviceProvider)
        {
            app.UseCors(options =>
                        options.WithOrigins (
                                "http://localhost:7171",
                                "http://localhost:7171/",
                                "https://localhost:7171",
                                "https://localhost:7171/",
                                "http://localhost:4200",
                                "http://localhost:4200/",
                                "https://localhost:4200",
                                "https://localhost:4200/",
                                "http://localhost:4300",
                                "http://localhost:4300/",
                                 "https://localhost:4300",
                                "https://localhost:4300/",
                                "http://localhost:4400",
                                "http://localhost:4400/",
                                "https://localhost:4400",
                                "https://localhost:4400/",
                                "https://eventsapi.regnumdigital.co.in",
                                "https://eventsapi.regnumdigital.co.in/",
                                "http://eventsapi.regnumdigital.co.in",
                                "http://eventsapi.regnumdigital.co.in/",
                                 "https://events.regnumdigital.co.in",
                                "https://events.regnumdigital.co.in/",
                                "http://events.regnumdigital.co.in",
                                "http://events.regnumdigital.co.in/"

                                )
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .AllowAnyMethod()
                        );


            // Configure the HTTP request pipeline.

            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //                    Path.Combine(environment.ContentRootPath, "documents")),
            //    RequestPath = "/documents"
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();               
            });
        }
    }

}
