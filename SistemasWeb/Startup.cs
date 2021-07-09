using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemasWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemasWeb
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //  .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options => {
                //Obtiene o establece un valor que especifica si un script del
                //lado del cliente puede acceder a una cookie.
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.LoginPath = "/Home/Index";
                options.AccessDeniedPath = "/Usuario/Account/AccessDenied";
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMvc().AddMvcOptions(options =>
            {
                options.EnableEndpointRouting = false;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();


            //app.UseEndpoints(endpoints =>
            //{

            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();

            //    endpoints.MapAreaControllerRoute(
            //        name: "Principal",
            //        areaName: "Principal",
            //        pattern: "Principal/{controller=Principal}/{action=Index}/{id?}");
            //    endpoints.MapAreaControllerRoute(
            //       name: "Categorias",
            //       areaName: "Categorias",
            //       pattern: "Categorias/{controller=Categorias}/{action=Index}/{id?}");


            //});
            app.UseEndpoints(endpoints =>

            {

                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();



                endpoints.MapAreaControllerRoute(name: "Principal", areaName: "Principal", pattern: "{controller=Principal}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(name: "Categorias", areaName: "Categorias", pattern: "{controller=Categorias}/{action=Categoria}/{id?}");
                endpoints.MapAreaControllerRoute(name: "Cursos", areaName: "Cursos", pattern: "{controller=Cursos}/{action=Cursos}/{id?}");
                endpoints.MapAreaControllerRoute(name: "Inscripciones", areaName: "Inscripciones", pattern: "{controller=Inscripciones}/{action=Inscripciones}/{id?}");



            });

        }
    }
}
