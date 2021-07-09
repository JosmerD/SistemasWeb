using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemasWeb.Areas.Categorias.Models;
using SistemasWeb.Library;
using Microsoft.AspNetCore.Identity;
using SistemasWeb.Models;
using SistemasWeb.Data;
using SistemasWeb.Controllers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace SistemasWeb.Areas.Categorias.Controllers
{
    [Area("Categorias")]
    [Authorize(Roles ="Admin")]
    public class CategoriasController : Controller
    {
        private TCategoria _categoria;
        private LCategorias _lcategorias;
        private SignInManager<IdentityUser> _signInManager;
        private static DataPaginador<TCategoria> models;
        private static IdentityError identityError=null;
        public CategoriasController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _lcategorias = new LCategorias(context);
        }
        public IActionResult Categoria(int id, String Search, int Registros)
        {
            if (_signInManager.IsSignedIn(User))
            {
                Object[] objects = new Object[3];
                var data = _lcategorias.getTCategoria(Search);
                if (0 < data.Count)
                {
                    var url = Request.Scheme + "://" + Request.Host.Value;
                    objects = new LPaginador<TCategoria>().paginador(_lcategorias.getTCategoria(Search)
                  , id, Registros, "Categorias", "Categorias", "Categoria", url);
                }
                else
                {
                    objects[0] = "No hay datos que mostrar";
                    objects[1] = "No hay datos que mostrar";
                    objects[2] = new List<TCategoria>();
                }

                models = new DataPaginador<TCategoria>
                {
                    List = (List<TCategoria>)objects[2],
                    Pagi_info = (String)objects[0],
                    Pagi_navegacion = (String)objects[1],
                    Input = new TCategoria()
                };
                if (identityError!=null)
                {
                    models.Pagi_info = identityError.Description;
                    identityError = null;
                }
                return View(models);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

        }
        [HttpPost]
        public String GetCategorias(DataPaginador<TCategoria> model)
        {
            if (model.Input.Categoria != null && model.Input.Descripcion != null)
            {
                var data = _lcategorias.RegistrarCategoria(model.Input);
                return JsonConvert.SerializeObject(data);
            }
            else
            {
                return "Llene los campos requeridos";
            }

        }
        [HttpPost]
        public IActionResult UpdateEstado(int id)
        {
            identityError = _lcategorias.UpdateEstado(id);
            return Redirect("/Categorias/Categoria?area=Categorias");
        }
        [HttpPost]
        public String EliminarCategoria(int CategoriaID)
        {
            identityError = _lcategorias.DeleteCategoria(CategoriaID);

            return JsonConvert.SerializeObject(identityError);
        }
    }
}
