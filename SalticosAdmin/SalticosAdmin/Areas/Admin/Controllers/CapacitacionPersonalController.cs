using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CapacitacionPersonalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public CapacitacionPersonalController(IUnidadTrabajo unidadTrabajo)
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


        public async Task<IActionResult> Upsert(int capacitacionID, int? relacionId)
        {


            CapacitacionPersonalVM capacitacionPersonalVM = new CapacitacionPersonalVM()
            {
                IdCapacitacion = capacitacionID,

                ListaPersonal = _unidadTrabajo.CapacitacionPersonal.ObtenerPersonal("Personal", capacitacionID)
            };


            if (relacionId == null)
            {
                return View(capacitacionPersonalVM);
            }
            else
            {

                capacitacionPersonalVM.IdRelacion = relacionId.Value;
                var capacitacionPersonalOBJ = await _unidadTrabajo.CapacitacionPersonal.ObtenerPrimero(X => X.Id == capacitacionPersonalVM.IdRelacion);
                if (capacitacionPersonalOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                capacitacionPersonalVM.IdPersonal = capacitacionPersonalOBJ.IdPersonal;


                return View(capacitacionPersonalVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CapacitacionPersonalVM capacitacionPersonalVM)
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

                if (capacitacionPersonalVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    CapacitacionPersonal existePersonal = await _unidadTrabajo.CapacitacionPersonal.ObtenerPrimero(X => X.IdCapacitacion == capacitacionPersonalVM.IdCapacitacion && X.IdPersonal == capacitacionPersonalVM.IdPersonal);
                    if (existePersonal != null)
                    {
                        TempData[DS.Error] = "personal ya existente";
                        
                    }
                    else
                    {
                        var capacitacionPersonal = new CapacitacionPersonal();
                        capacitacionPersonal.IdCapacitacion = capacitacionPersonalVM.IdCapacitacion;
                        capacitacionPersonal.IdPersonal = capacitacionPersonalVM.IdPersonal;

                        capacitacionPersonal.Personal = await _unidadTrabajo.Personal.ObtenerPrimero(X => X.Id == capacitacionPersonalVM.IdPersonal);
                        capacitacionPersonal.Capacitacion = await _unidadTrabajo.Capacitacion.ObtenerPrimero(X => X.Id == capacitacionPersonalVM.IdCapacitacion);

                        await _unidadTrabajo.CapacitacionPersonal.Agregar(capacitacionPersonal);

                        TempData[DS.Exitosa] = "Personal agregado exitosamente";

                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el personal '{capacitacionPersonal.Personal.Nombre}' a la capacitación '{capacitacionPersonal.Capacitacion.Tema}'", usuarioNombre);

                    }

                }
                else
                {

                    var capacitacionPersonal = new CapacitacionPersonal();
                    capacitacionPersonal.IdCapacitacion = capacitacionPersonalVM.IdCapacitacion;
                    capacitacionPersonal.IdPersonal = capacitacionPersonalVM.IdPersonal;
                    capacitacionPersonal.Id = capacitacionPersonalVM.IdRelacion;

                    capacitacionPersonal.Personal = await _unidadTrabajo.Personal.ObtenerPrimero(X => X.Id == capacitacionPersonalVM.IdPersonal);
                    capacitacionPersonal.Capacitacion = await _unidadTrabajo.Capacitacion.ObtenerPrimero(X => X.Id == capacitacionPersonalVM.IdCapacitacion);

                    _unidadTrabajo.CapacitacionPersonal.Actualizar(capacitacionPersonal);
                    TempData[DS.Exitosa] = "Personal actualizado exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el personal '{capacitacionPersonal.Personal.Nombre}' en la capacitación '{capacitacionPersonal.Capacitacion.Tema}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "CapacitacionPersonal", new { id = capacitacionPersonalVM.IdCapacitacion });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar personal a capacitacion";
            TempData[DS.Error] = mensajeError;
            return View(capacitacionPersonalVM);
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

            var todos = await _unidadTrabajo.CapacitacionPersonal.ObtenerTodos(incluirPropiedades: "Personal");

            var filtrados = todos.Where(t => t.IdCapacitacion == id);
            return Json(new { data = filtrados });

        }

        


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioNombre = User.Identity.Name;

            var CapacitacionPersonalBd = await _unidadTrabajo.CapacitacionPersonal.Obtener(id);
            if(CapacitacionPersonalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Personal de Capacitacion" });
            }

            _unidadTrabajo.CapacitacionPersonal.Remover(CapacitacionPersonalBd);
            await _unidadTrabajo.Guardar();
            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el personal '{CapacitacionPersonalBd.Personal.Nombre}' de la capacitación '{CapacitacionPersonalBd.Capacitacion.Tema}'", usuarioNombre);


            return Json(new { success = true, message = "personal borrado de capacitacion exitosamente" });

        }



        #endregion
    }
}
