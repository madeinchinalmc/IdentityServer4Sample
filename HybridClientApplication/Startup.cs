using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using HybridClientApplication.AuthRequirements;
using Microsoft.AspNetCore.Authorization;

namespace HybridClientApplication
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies",
                options => {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";  //改变AccessDenied 路径 
                }
            )
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ClientId = "hybrid client";
                options.SaveTokens = true;
                options.ClientSecret = "hybrid client";
                options.SignInScheme = "Cookies";
                options.ResponseType = "code id_token";  //响应类型为hybrid flow三种情况之一
                options.Scope.Clear();
                options.Scope.Add("api1");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add(OidcConstants.StandardScopes.Email);
                options.Scope.Add(OidcConstants.StandardScopes.Phone);
                options.Scope.Add(OidcConstants.StandardScopes.Address);
                options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                options.Scope.Add("roles");
                options.Scope.Add("locations");

                //过滤掉集合的属性，remove的属性会显示出来(防止被过滤)，delete的属性取消显示（被过滤掉）
                options.ClaimActions.Remove("nbf");
                options.ClaimActions.DeleteClaim("sid");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name, //"name",
                    RoleClaimType = "role"
                };
            });
            services.AddAuthorization(options =>
            {
                // options.AddPolicy("SmithInSomewhere", builder =>  //new Microsoft.AspNetCore.Authorization.AuthorizationPolicy()
                //{
                //    builder.RequireAuthenticatedUser();
                //    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith");
                //    builder.RequireClaim("location", "somewhere");

                //});    
                options.AddPolicy("SmithInSomewhere", builder =>
                {
                    builder.AddRequirements(new SmithInRequirement());
                });
            });
            services.AddSingleton<IAuthorizationHandler, SmithInHandler>();
            services.AddSingleton<IAuthorizationHandler, CustomHandler>();
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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
