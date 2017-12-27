using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MineId.Initializer;
using MineId.Models;

namespace MineId {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            // configuration de la connexion à la base de données PG
            string dbConnectionString = Configuration.GetConnectionString ("DataAccessPostgreSqlProvider");

            // Configuration liée à l'authentification jwt bearer
            var tokenConfig = Configuration.GetSection ("Token");

            services.AddDbContext<MineContext> (options => {
                options.UseNpgsql (
                    dbConnectionString,
                    b => b.MigrationsAssembly ("MineId"));
            });

            services.AddIdentity<MineUser, IdentityRole> ();

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options => {
                    options.TokenValidationParameters = new TokenValidationParameters () {
                    ValidIssuer = tokenConfig["Issuer"],
                    ValidAudience = tokenConfig["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (tokenConfig["Key"])),
                    ValidateLifetime = true
                    };
                });

            services.AddTransient<MineInitializer> ();

            services.AddMvc ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMvc ();
        }
    }
}