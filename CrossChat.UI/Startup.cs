using CrossChat.UI.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;

using AutoMapper;
using CrossChat.Services.JwtService;
using CrossChat.Domain.Common;
using CrossChat.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CrossChat.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using CrossChat.Domain.Common.constants;
using Microsoft.AspNetCore.Mvc;
using CrossChat.UI.Hubs;

namespace CrossChat.UI
{
    public class Startup
    {
        //public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {



            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddSignalR();
            //services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));
            JwtSettings.Audience = Configuration["JwtSettings:Audience"];
            JwtSettings.Encryptkey = Configuration["JwtSettings:Encryptkey"];
            JwtSettings.ExpirationMinutes =int.Parse( Configuration["JwtSettings:ExpirationMinutes"]);
            JwtSettings.Issuer = Configuration["JwtSettings:Issuer"];
            JwtSettings.SecretKey = Configuration["JwtSettings:SecretKey"];
            JwtSettings.NotBeforeMinutes = int.Parse(Configuration["JwtSettings:NotBeforeMinutes"]);


            EmailHelper.ApiKey = Configuration["MailConfig:Key"];
            EmailHelper.ApiSecret = Configuration["MailConfig:Secret"];
            EmailHelper.Subject = Configuration["MailConfig:Subject"];
            EmailHelper.Body = Configuration["MailConfig:Body"];

            var connectionstring = Configuration.GetConnectionString("Connection");
            services.AddDbContext<CrossChatContext>(option => option.UseSqlServer(connectionstring));
            services.AddScoped(typeof(IJwtService), typeof(JwtService));
            services.AddAutoMapper(typeof(AppMapper).GetTypeInfo().Assembly);
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddRazorPages().AddRazorRuntimeCompilation();
            //jwt start
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme   /*JwtBearerDefaults.AuthenticationScheme*/).AddCookie(
                options =>
                {
                   
                    options.LoginPath = new PathString("/Account/Login/");
                
                })
               .AddJwtBearer(options =>
               {
                   var secretkey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey);
                   var encryptionkey = Encoding.UTF8.GetBytes(JwtSettings.Encryptkey);

                   var validationParameters = new TokenValidationParameters
                   {
                       ClockSkew = TimeSpan.Zero, // default: 5 min
                       RequireSignedTokens = true,

                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                       RequireExpirationTime = true,
                       ValidateLifetime = true,

                       ValidateAudience = true, //default : false
                       ValidAudience = JwtSettings.Audience,

                       ValidateIssuer = true, //default : false
                       ValidIssuer = JwtSettings.Issuer,

                       TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                   };

                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = validationParameters;
               });
            //jwt end



            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddCors();
            services.AddMvc()
                    
                    .AddControllersAsServices()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0); ;


           

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);// iframe içinden çağrılabilsin diye eklendi


            var container = services.AddStructureMap(Configuration);

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(
              options => options.WithOrigins("http://localhost:33069").AllowAnyMethod()
            );
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                //Set Default Language
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("tr-TR") },
                DefaultRequestCulture = new RequestCulture("tr-TR")
            });
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

                
            
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            
            SeedDB.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

        }
    }
}
