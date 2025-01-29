using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoServicioAdicionalController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoServicioAdicionalController(IUnidadTrabajo unidadTrabajo)
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
            EventoServicioAdicionalVM eventoServicioAdicionalVM = new EventoServicioAdicionalVM()
            {
                IdEvento = eventoID,

                ListaServicioAdicional = _unidadTrabajo.EventoServicioAdicional.ObtenerServicioAdicional("ServicioAdicional", eventoID)
            };
                
            if (relacionId == null)
            {
                return View(eventoServicioAdicionalVM);
            }
            else
            {

                eventoServicioAdicionalVM.IdRelacion = relacionId.Value;
                var eventoServicioAdicionalOBJ = await _unidadTrabajo.EventoServicioAdicional.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdRelacion);
                if (eventoServicioAdicionalOBJ == null)
                {
                    return NotFound();
                }


                eventoServicioAdicionalVM.IdServicioAdicional = eventoServicioAdicionalOBJ.IdServicioAdicional;


                return View(eventoServicioAdicionalVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoServicioAdicionalVM eventoServicioAdicionalVM)
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

                var eventoActual = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdEvento);
                if (eventoActual == null)
                {
                    TempData[DS.Error] = "Evento no encontrado.";
                    return View(eventoServicioAdicionalVM);
                }

                //Se obtiene la cantidad actual del servicio en el evento
                int cantidadActualEvento = 0;
                if (eventoServicioAdicionalVM.IdRelacion != 0)
                {
                    var eventoServicioActual = await _unidadTrabajo.EventoServicioAdicional.ObtenerPrimero(x => x.Id == eventoServicioAdicionalVM.IdRelacion);
                    if (eventoServicioActual != null)
                    {
                        cantidadActualEvento = eventoServicioActual.Cantidad;
                    }
                }


                var eventosSolapados = await _unidadTrabajo.Evento.ObtenerEventosSolapados(
                    eventoActual.Fecha,
                    eventoActual.HoraInicio,
                    eventoActual.HoraFinal
                );

                var servicioAdicional = await _unidadTrabajo.ServicioAdicional.ObtenerPrimero(m => m.Id == eventoServicioAdicionalVM.IdServicioAdicional);
                if (servicioAdicional == null)
                {
                    TempData[DS.Error] = "Servicio no encontrado.";
                    return View(eventoServicioAdicionalVM);
                }

                var eventoServiciosAdicionales = await _unidadTrabajo.EventoServicioAdicional.ObtenerTodos();

                int cantidadAsignadaEnEventosSolapados = eventoServiciosAdicionales
                    .Where(es => eventosSolapados.Any(e => e.Id == es.IdEvento)
                                && es.IdServicioAdicional == eventoServicioAdicionalVM.IdServicioAdicional)
                    .Sum(em => em.Cantidad);

                int cantidadTotalAsignada = cantidadAsignadaEnEventosSolapados + eventoServicioAdicionalVM.Cantidad - cantidadActualEvento;
                if (cantidadTotalAsignada > servicioAdicional.Inventario)
                {
                    if (cantidadActualEvento == cantidadAsignadaEnEventosSolapados)
                    {
                        TempData[DS.Error] = $"No hay suficiente del servicio disponible. Solo hay {servicioAdicional.Inventario} unidades.";

                    }
                    else
                    {
                        TempData[DS.Error] = $"No hay suficiente mobiliario disponible. Solo quedan {servicioAdicional.Inventario - cantidadAsignadaEnEventosSolapados} unidades.";

                    }
                    eventoServicioAdicionalVM.ListaServicioAdicional = _unidadTrabajo.EventoServicioAdicional.ObtenerServicioAdicional("ServicioAdicional", eventoServicioAdicionalVM.IdEvento);
                    return View(eventoServicioAdicionalVM);
                }


                if (eventoServicioAdicionalVM.IdRelacion == 0)
                {
                    EventoServicioAdicional existeServicioAdicional = await _unidadTrabajo.EventoServicioAdicional.ObtenerPrimero(X => X.IdEvento == eventoServicioAdicionalVM.IdEvento && X.IdServicioAdicional == eventoServicioAdicionalVM.IdServicioAdicional);
                    if (existeServicioAdicional != null)
                    {
                        TempData[DS.Error] = "Servicio Adicional ya existente";

                    }
                    else
                    {
                        var eventoServicioAdicional = new EventoServicioAdicional();
                        eventoServicioAdicional.IdEvento = eventoServicioAdicionalVM.IdEvento;
                        eventoServicioAdicional.IdServicioAdicional = eventoServicioAdicionalVM.IdServicioAdicional;
                        eventoServicioAdicional.Cantidad = eventoServicioAdicionalVM.Cantidad;

                        eventoServicioAdicional.ServicioAdicional = await _unidadTrabajo.ServicioAdicional.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdServicioAdicional);
                        eventoServicioAdicional.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdEvento);

                        await _unidadTrabajo.EventoServicioAdicional.Agregar(eventoServicioAdicional);

                        TempData[DS.Exitosa] = "Servicio adicional agregado exitosamente";

                        var usuarioNombre = User.Identity.Name;
                        
                        var servicioAdicionalBitacora = await _unidadTrabajo.ServicioAdicional.Obtener(eventoServicioAdicional.IdServicioAdicional);
                        
                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó en evento del {eventoServicioAdicional.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                            $" a las {eventoServicioAdicional.Evento.HoraInicio} el servicio de {servicioAdicionalBitacora.Nombre}'", usuarioNombre);



                    }

                }
                else
                {

                    var eventoServicioAdicional = new EventoServicioAdicional();
                    eventoServicioAdicional.IdEvento = eventoServicioAdicionalVM.IdEvento;
                    eventoServicioAdicional.IdServicioAdicional = eventoServicioAdicionalVM.IdServicioAdicional;
                    eventoServicioAdicional.Cantidad = eventoServicioAdicionalVM.Cantidad;
                    eventoServicioAdicional.Id = eventoServicioAdicionalVM.IdRelacion;

                    eventoServicioAdicional.ServicioAdicional = await _unidadTrabajo.ServicioAdicional.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdServicioAdicional);
                    eventoServicioAdicional.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoServicioAdicionalVM.IdEvento);

                    _unidadTrabajo.EventoServicioAdicional.Actualizar(eventoServicioAdicional);
                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";

                    var usuarioNombre = User.Identity.Name;

                    var servicioAdicionalBitacora = await _unidadTrabajo.ServicioAdicional.Obtener(eventoServicioAdicional.IdServicioAdicional);
                    
                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó en evento del {eventoServicioAdicional.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                            $" a las {eventoServicioAdicional.Evento.HoraInicio} el servicio de {servicioAdicionalBitacora.Nombre}'", usuarioNombre);


                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoServicioAdicional", new { id = eventoServicioAdicionalVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar servicio adicional al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoServicioAdicionalVM);
            
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

            var todos = await _unidadTrabajo.EventoServicioAdicional.ObtenerTodos(incluirPropiedades: "ServicioAdicional");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }

        


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoServicioAdicionalBd = await _unidadTrabajo.EventoServicioAdicional.Obtener(id);
            if(EventoServicioAdicionalBd == null)
            {
                return Json(new { success = false, message = "Error al borrar servicio adicional del evento" });
            }

            _unidadTrabajo.EventoServicioAdicional.Remover(EventoServicioAdicionalBd);
            await _unidadTrabajo.Guardar();

            var usuarioNombre = User.Identity.Name;

            var eventoBitacora = await _unidadTrabajo.Evento.Obtener(EventoServicioAdicionalBd.IdEvento);
            var servicioAdicionalBitacora = await _unidadTrabajo.ServicioAdicional.Obtener(EventoServicioAdicionalBd.IdServicioAdicional);
            

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó en evento del {eventoBitacora.Fecha.ToString("dd/MM/yyyy")}" +
                $" a las {eventoBitacora.HoraInicio} el servicio de {servicioAdicionalBitacora.Nombre}", usuarioNombre);
            


            return Json(new { success = true, message = "Servicio adicional borrado del evento exitosamente" });

        }



        #endregion
    }
}
