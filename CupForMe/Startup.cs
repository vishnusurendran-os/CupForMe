using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CupForMe.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CupForMe.Utils;
using Microsoft.AspNetCore.Identity;
using CupForMe.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CupForMe
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddCors();

            services.AddControllers();

            ConfigureAuthContext(services);

            ConfigureCoreDatabaseContext(services);

            ConfigureReadDatabaseContext(services);
        }

        private void ConfigureCoreDatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<CupForMeWriteContext>(options => options.UseMySQL(Configuration.GetConnectionString("con_cupforme_write"), builder =>
            {
                builder.ExecutionStrategy(ctx => new CustomExecutionStrategy(ctx));

            }));
        }

        private void ConfigureReadDatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<CupForMeReadContext>(options => options.UseMySQL(Configuration.GetConnectionString("con_cupforme_read"), builder =>
            {
                builder.ExecutionStrategy(ctx => new CustomExecutionStrategy(ctx));
            }));
        }

        private void ConfigureAuthContext(IServiceCollection services)
        {
            services.Configure<PasswordHasherOptions>(options =>
            {
                options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
                options.IterationCount = 50000;
            });

            services.AddDbContext<AuthenticationContext>(options => options.UseMySQL(Configuration.GetConnectionString("con_cupforme_auth"), builder =>
            {
                builder.ExecutionStrategy(ctx => new CustomExecutionStrategy(ctx));

            }));
            services.AddIdentity<UserIdentity, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AuthenticationContext>().AddDefaultTokenProviders();

            var authSecret = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JwtSecret"].ToString()); // TODO: look into best practices for secret key

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                auth.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                auth.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = Boolean.Parse(Configuration["ApplicationSettings:RequireHttpsMetadata"]); //RequireHttpsMetadata TODO: move to config, notes say this should only be false for dev env
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(authSecret), // TODO: also should go in config
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                };

            });
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

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCors(x => x.WithOrigins(
                Configuration["ApplicationSettings:WebAppCorsOriginUrl"].ToString())
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
