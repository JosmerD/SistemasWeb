using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemasWeb.Areas.Cursos.Models;
using SistemasWeb.Data;
using SistemasWeb.Library;
using SistemasWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SistemasWeb.Controllers
{
    public class HomeController : Controller
    {
        private LCursos _curso;
        private static DataPaginador<TCursos> models;
        private static DataCurso _dataCurso;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private static IdentityError identityError;
        public HomeController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _curso = new LCursos(context, null);

            TCursos t = new TCursos {
           CategoriaID=1,
           Costo=200

            };
        }
        // IServiceProvider _serviceProvider;
        //public HomeController(IServiceProvider serviceProvider)
        //{
        //   // _serviceProvider = serviceProvider;
        //}

        //    public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult Index(int id, String filtrar)
        {
            Object[] objects = new Object[3];
            var data = _curso.getTCursos(filtrar);
            if (0<data.Count)
            {
                var url = Request.Scheme + "://" + Request.Host.Value;
                objects = new LPaginador<TCursos>().paginador(data, id, 10, "", "Home", "Index", url);
            }
            else
            {
                objects[0] = "No hay datos que mostrar";
                objects[1] = "No hay datos que mostrar";
                objects[2] = new List<TCursos>();
            }
            models = new DataPaginador<TCursos>
            {
                List = (List<TCursos>)objects[2],
                Pagi_info = (String)objects[0],
                Pagi_navegacion = (String)objects[1],                
                Input = new TCursos()
            };
            if (identityError !=null)
            {
                models.Pagi_info = identityError.Description;
                identityError = null;
            }
         //   await CreateRolesAsync(_serviceProvider);

            return View(models);
        }
        public IActionResult Detalles(int id)
        {
            var model = _curso.getTCurso(id);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ObtenerAsync(int cursoID,int vista)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                var idUser = await _userManager.GetUserIdAsync(user);
                var data = _curso.Inscripcion(idUser, cursoID);
                
                if (data.Description.Equals("Done"))
                {
                    return Redirect("/Inscripciones/Inscripciones?area=Inscripciones");
                }
                else
                {
                    identityError = data;
                    if (vista.Equals(1))
                    {
                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        _dataCurso = _curso.getTCurso(cursoID);
                        _dataCurso.ErrorMessage = data.Description;
                        return View("Detalles", _dataCurso);
                    }
                }
               
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task CreateRolesAsync (IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            String[] rolesNames = { "Admin", "Student" };

            foreach (var item in rolesNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(item);
                
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(item));
                }
            }
            var user = await userManager.FindByIdAsync("be9c9519-e81c-4f29-b218-7a22802eec04");
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
