using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AlimentacionIngredienteController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public AlimentacionIngredienteController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index(int? id)
        {
            if (id != null)
            {
                ViewData["Id"] = id;
            }

            return View();
        }


        public async Task<IActionResult> Upsert(int alimentacionID, int? relacionId)
        {


            AlimentacionIngredienteVM alimentacionIngredienteVM = new AlimentacionIngredienteVM()
            {
                IdAlimentacion = alimentacionID,

                ListaIngrediente = _unidadTrabajo.AlimentacionIngrediente.ObtenerIngrediente("Ingrediente", alimentacionID)
            };


            if (relacionId == null)
            {
                return View(alimentacionIngredienteVM);
            }
            else
            {

                alimentacionIngredienteVM.IdRelacion = relacionId.Value;
                var alimentacionIngredienteOBJ = await _unidadTrabajo.AlimentacionIngrediente.ObtenerPrimero(X => X.Id == alimentacionIngredienteVM.IdRelacion);
                if (alimentacionIngredienteOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                alimentacionIngredienteVM.IdIngrediente = alimentacionIngredienteOBJ.IdIngrediente;


                return View(alimentacionIngredienteVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(AlimentacionIngredienteVM alimentacionIngredienteVM)
        {


            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values
                               .SelectMany(v => v.Errors)
                               .Select(e => e.ErrorMessage)
                               .ToList();

                foreach (var error in allErrors)
                {
                    Console.WriteLine(error);
                }
            }
            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                if (alimentacionIngredienteVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    AlimentacionIngrediente existeIngrediente = await _unidadTrabajo.AlimentacionIngrediente.ObtenerPrimero(X => X.IdAlimentacion == alimentacionIngredienteVM.IdAlimentacion && X.IdIngrediente == alimentacionIngredienteVM.IdIngrediente);
                    if (existeIngrediente != null)
                    {
                        TempData[DS.Error] = "ingrediente ya existente";
                        
                    }
                    else
                    {
                        var alimentacionIngrediente = new AlimentacionIngrediente();
                        alimentacionIngrediente.IdAlimentacion = alimentacionIngredienteVM.IdAlimentacion;
                        alimentacionIngrediente.IdIngrediente = alimentacionIngredienteVM.IdIngrediente;

                        alimentacionIngrediente.Ingrediente = await _unidadTrabajo.Ingrediente.ObtenerPrimero(X => X.Id == alimentacionIngredienteVM.IdIngrediente);
                        alimentacionIngrediente.Alimentacion = await _unidadTrabajo.Alimentacion.ObtenerPrimero(X => X.Id == alimentacionIngredienteVM.IdAlimentacion);

                        await _unidadTrabajo.AlimentacionIngrediente.Agregar(alimentacionIngrediente);

                        TempData[DS.Exitosa] = "Ingrediente agregado exitosamente";
                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el ingrediente '{alimentacionIngrediente.Ingrediente.Nombre}' al producto de alimentación '{alimentacionIngrediente.Alimentacion.Nombre}'", usuarioNombre);


                    }

                }
                else
                {

                    var alimentacionIngrediente = new AlimentacionIngrediente();
                    alimentacionIngrediente.IdAlimentacion = alimentacionIngredienteVM.IdAlimentacion;
                    alimentacionIngrediente.IdIngrediente = alimentacionIngredienteVM.IdIngrediente;
                    alimentacionIngrediente.Id = alimentacionIngredienteVM.IdRelacion;

                    alimentacionIngrediente.Ingrediente = await _unidadTrabajo.Ingrediente.ObtenerPrimero(X => X.Id == alimentacionIngredienteVM.IdIngrediente);
                    alimentacionIngrediente.Alimentacion = await _unidadTrabajo.Alimentacion.ObtenerPrimero(X => X.Id == alimentacionIngredienteVM.IdAlimentacion);

                    _unidadTrabajo.AlimentacionIngrediente.Actualizar(alimentacionIngrediente);
                    TempData[DS.Exitosa] = "Ingrediente actualizado exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el ingrediente '{alimentacionIngrediente.Ingrediente.Nombre}' en el producto de alimentación '{alimentacionIngrediente.Alimentacion.Nombre}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "AlimentacionIngrediente", new { id = alimentacionIngredienteVM.IdAlimentacion });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar ingrediente a producto de alimentacion";
            TempData[DS.Error] = mensajeError;
            return View(alimentacionIngredienteVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            int id = 0;
            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                int.TryParse(HttpContext.Request.Query["id"], out id);
            }

            var todos = await _unidadTrabajo.AlimentacionIngrediente.ObtenerTodos(incluirPropiedades: "Ingrediente");

            var filtrados = todos.Where(t => t.IdAlimentacion == id);
            return Json(new { data = filtrados });

        }

        


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioNombre = User.Identity.Name;

            var AlimentacionIngredienteBd = await _unidadTrabajo.AlimentacionIngrediente.Obtener(id);
            if(AlimentacionIngredienteBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Ingrediente de producto de Alimentación" });
            }

            _unidadTrabajo.AlimentacionIngrediente.Remover(AlimentacionIngredienteBd);
            await _unidadTrabajo.Guardar();
            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el ingrediente '{AlimentacionIngredienteBd.Ingrediente.Nombre}' del producto de alimentación '{AlimentacionIngredienteBd.Alimentacion.Nombre}'", usuarioNombre);


            return Json(new { success = true, message = "Ingrediente borrado de producto de alimentación exitosamente" });

        }



        #endregion
    }
}
