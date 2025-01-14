using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoPersonalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoPersonalController(IUnidadTrabajo unidadTrabajo)
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


            EventoPersonalVM eventoPersonalVM = new EventoPersonalVM()
            {
                IdEvento = eventoID,

                ListaPersonal = _unidadTrabajo.EventoPersonal.ObtenerPersonal("Personal", eventoID)
            };


            if (relacionId == null)
            {
                return View(eventoPersonalVM);
            }
            else
            {

                eventoPersonalVM.IdRelacion = relacionId.Value;
                var eventoPersonalOBJ = await _unidadTrabajo.EventoPersonal.ObtenerPrimero(X => X.Id == eventoPersonalVM.IdRelacion);
                if (eventoPersonalOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                eventoPersonalVM.IdPersonal = eventoPersonalOBJ.IdPersonal;


                return View(eventoPersonalVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoPersonalVM eventoPersonalVM)
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
                if (eventoPersonalVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    EventoPersonal existePersonal = await _unidadTrabajo.EventoPersonal.ObtenerPrimero(X => X.IdEvento == eventoPersonalVM.IdEvento && X.IdPersonal == eventoPersonalVM.IdPersonal);
                    if (existePersonal != null)
                    {
                        TempData[DS.Error] = "Personal ya existente";
                        
                    }
                    else
                    {
                        var eventoPersonal = new EventoPersonal();
                        eventoPersonal.IdEvento = eventoPersonalVM.IdEvento;
                        eventoPersonal.IdPersonal = eventoPersonalVM.IdPersonal;

                        eventoPersonal.Personal = await _unidadTrabajo.Personal.ObtenerPrimero(X => X.Id == eventoPersonalVM.IdPersonal);
                        eventoPersonal.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoPersonalVM.IdEvento);

                        await _unidadTrabajo.EventoPersonal.Agregar(eventoPersonal);

                        TempData[DS.Exitosa] = "Evento agregado exitosamente";

                        var usuarioNombre = User.Identity.Name;

                        var personalBitacora = await _unidadTrabajo.Personal.Obtener(eventoPersonal.IdPersonal);
                        var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoPersonal.Evento.ClienteId);

                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó en evento de {clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                            $" para {eventoPersonal.Evento.Fecha.ToString("dd/MM/yyyy")} a {personalBitacora.Nombre}", usuarioNombre);

                    }

                }
                else
                {

                    var eventoPersonal = new EventoPersonal();
                    eventoPersonal.IdEvento = eventoPersonalVM.IdEvento;
                    eventoPersonal.IdPersonal = eventoPersonalVM.IdPersonal;
                    eventoPersonal.Id = eventoPersonalVM.IdRelacion;


                    eventoPersonal.Personal = await _unidadTrabajo.Personal.ObtenerPrimero(X => X.Id == eventoPersonalVM.IdPersonal);
                    eventoPersonal.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoPersonalVM.IdEvento);

                    _unidadTrabajo.EventoPersonal.Actualizar(eventoPersonal);
                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";

                    var usuarioNombre = User.Identity.Name;

                    var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoPersonal.Evento.ClienteId);
                    var personalBitacora = await _unidadTrabajo.Personal.Obtener(eventoPersonal.IdPersonal);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó en evento de {clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                        $" para {eventoPersonal.Evento.Fecha.ToString("dd/MM/yyyy")} a {personalBitacora.Nombre}", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoPersonal", new { id = eventoPersonalVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar personal al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoPersonalVM);
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

            var todos = await _unidadTrabajo.EventoPersonal.ObtenerTodos(incluirPropiedades: "Personal");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoPersonalBd = await _unidadTrabajo.EventoPersonal.Obtener(id);
            if(EventoPersonalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar personal del evento" });
            }

            _unidadTrabajo.EventoPersonal.Remover(EventoPersonalBd);
            await _unidadTrabajo.Guardar();

            var usuarioNombre = User.Identity.Name;

            var eventoBitacora = await _unidadTrabajo.Evento.Obtener(EventoPersonalBd.IdEvento);
            var personalBitacora = await _unidadTrabajo.Personal.Obtener(EventoPersonalBd.IdPersonal);
            var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoBitacora.ClienteId);

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó en evento de {clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                $" para {eventoBitacora.Fecha.ToString("dd/MM/yyyy")} a {personalBitacora.Nombre}", usuarioNombre);


            return Json(new { success = true, message = "Personal borrado del evento exitosamente" });

        }

        #endregion
    }
}
