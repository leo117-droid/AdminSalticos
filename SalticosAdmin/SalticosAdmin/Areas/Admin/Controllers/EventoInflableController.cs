using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoInflableController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoInflableController(IUnidadTrabajo unidadTrabajo)
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


        public async Task<IActionResult> Upsert(int eventoID, int? relacionId)
        {


            EventoInflableVM eventoInflableVM = new EventoInflableVM()
            {
                IdEvento = eventoID,

                ListaInflable = _unidadTrabajo.EventoInflable.ObtenerInflable("Inflable", eventoID)
            };


            if (relacionId == null)
            {
                return View(eventoInflableVM);
            }
            else
            {

                eventoInflableVM.IdRelacion = relacionId.Value;
                var eventoInflableOBJ = await _unidadTrabajo.EventoInflable.ObtenerPrimero(X => X.Id == eventoInflableVM.IdRelacion);
                if (eventoInflableOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                eventoInflableVM.IdInflable = eventoInflableOBJ.IdInflable;


                return View(eventoInflableVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoInflableVM eventoInflableVM)
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
                if (eventoInflableVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    EventoInflable existeInflable = await _unidadTrabajo.EventoInflable.ObtenerPrimero(X => X.IdEvento == eventoInflableVM.IdEvento && X.IdInflable == eventoInflableVM.IdInflable);
                    if (existeInflable != null)
                    {
                        TempData[DS.Error] = "inflable ya existente";
                        
                    }
                    else
                    {
                        var eventoInflable = new EventoInflable();
                        eventoInflable.IdEvento = eventoInflableVM.IdEvento;
                        eventoInflable.IdInflable = eventoInflableVM.IdInflable;

                        eventoInflable.Inflable = await _unidadTrabajo.Inflable.ObtenerPrimero(X => X.Id == eventoInflableVM.IdInflable);
                        eventoInflable.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoInflableVM.IdEvento);

                        await _unidadTrabajo.EventoInflable.Agregar(eventoInflable);

                        TempData[DS.Exitosa] = "Inflable agregado exitosamente";

                        var usuarioNombre = User.Identity.Name;

                        var inflableBitacora = await _unidadTrabajo.Inflable.Obtener(eventoInflable.IdInflable);


                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó en evento del {eventoInflable.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                            $" a las {eventoInflable.Evento.HoraInicio} el inflable {inflableBitacora.Nombre}", usuarioNombre);


                    }

                }
                else
                {

                    var eventoInflable = new EventoInflable();
                    eventoInflable.IdEvento = eventoInflableVM.IdEvento;
                    eventoInflable.IdInflable = eventoInflableVM.IdInflable;
                    eventoInflable.Id = eventoInflableVM.IdRelacion;

                    eventoInflable.Inflable = await _unidadTrabajo.Inflable.ObtenerPrimero(X => X.Id == eventoInflableVM.IdInflable);
                    eventoInflable.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoInflableVM.IdEvento);

                    _unidadTrabajo.EventoInflable.Actualizar(eventoInflable);
                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";

                    var usuarioNombre = User.Identity.Name;

                    var inflableBitacora = await _unidadTrabajo.Inflable.Obtener(eventoInflable.IdInflable);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó en evento del {eventoInflable.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                        $" a las {eventoInflable.Evento.HoraInicio} el inflable {inflableBitacora.Nombre}", usuarioNombre);


                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoInflable", new { id = eventoInflableVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar inflable al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoInflableVM);
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

            var todos = await _unidadTrabajo.EventoInflable.ObtenerTodos(incluirPropiedades: "Inflable");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoInflableBd = await _unidadTrabajo.EventoInflable.Obtener(id);
            if(EventoInflableBd == null)
            {
                return Json(new { success = false, message = "Error al borrar inflable del evento" });
            }

            _unidadTrabajo.EventoInflable.Remover(EventoInflableBd);
            await _unidadTrabajo.Guardar();

            var usuarioNombre = User.Identity.Name;

            var eventoBitacora = await _unidadTrabajo.Evento.Obtener(EventoInflableBd.IdEvento);
            var inflableBitacora = await _unidadTrabajo.Inflable.Obtener(EventoInflableBd.IdInflable);

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó en evento del {eventoBitacora.Fecha.ToString("dd/MM/yyyy")}" +
           $" a las {eventoBitacora.HoraInicio} el inflable {inflableBitacora.Nombre}", usuarioNombre);


            return Json(new { success = true, message = "Inflable borrado del evento exitosamente" });

        }

        #endregion
    }
}
