﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoMobiliarioController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoMobiliarioController(IUnidadTrabajo unidadTrabajo)
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
            EventoMobiliarioVM eventoMobiliarioVM = new EventoMobiliarioVM()
            {
                IdEvento = eventoID,

                ListaMobiliario = _unidadTrabajo.EventoMobiliario.ObtenerMobiliario("Mobiliario", eventoID)
            };

            if (relacionId == null)
            {
                return View(eventoMobiliarioVM);
            }
            else
            {

                eventoMobiliarioVM.IdRelacion = relacionId.Value;
                var eventoMobiliarioOBJ = await _unidadTrabajo.EventoMobiliario.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdRelacion);
                if (eventoMobiliarioOBJ == null)
                {
                    return NotFound();
                }


                eventoMobiliarioVM.IdMobiliario = eventoMobiliarioOBJ.IdMobiliario;


                return View(eventoMobiliarioVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoMobiliarioVM eventoMobiliarioVM)
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

                var eventoActual = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdEvento);
                if (eventoActual == null)
                {
                    TempData[DS.Error] = "Evento no encontrado.";
                    return View(eventoMobiliarioVM);
                }

                //Se obtiene la cantidad actual del mobiliario en el evento
                int cantidadActualEvento = 0;
                if (eventoMobiliarioVM.IdRelacion != 0)
                {
                    var eventoMobiliarioActual = await _unidadTrabajo.EventoMobiliario.ObtenerPrimero(x => x.Id == eventoMobiliarioVM.IdRelacion);
                    if (eventoMobiliarioActual != null)
                    {
                        cantidadActualEvento = eventoMobiliarioActual.Cantidad;
                    }
                }

                var eventosSolapados = await _unidadTrabajo.Evento.ObtenerEventosSolapados(
                    eventoActual.Fecha,
                    eventoActual.HoraInicio,
                    eventoActual.HoraFinal
                );

                var mobiliario = await _unidadTrabajo.Mobiliario.ObtenerPrimero(m => m.Id == eventoMobiliarioVM.IdMobiliario);
                if (mobiliario == null)
                {
                    TempData[DS.Error] = "Mobiliario no encontrado.";
                    return View(eventoMobiliarioVM);
                }

                var eventoMobiliarios = await _unidadTrabajo.EventoMobiliario.ObtenerTodos();

                int cantidadAsignadaEnEventosSolapados = eventoMobiliarios
                    .Where(em => eventosSolapados.Any(e => e.Id == em.IdEvento)
                                && em.IdMobiliario == eventoMobiliarioVM.IdMobiliario)
                    .Sum(em => em.Cantidad);

                
                int cantidadTotalAsignada = cantidadAsignadaEnEventosSolapados + eventoMobiliarioVM.Cantidad - cantidadActualEvento;
                if (cantidadTotalAsignada > mobiliario.Inventario)
                {

                    if (cantidadActualEvento == cantidadAsignadaEnEventosSolapados)
                    {
                        TempData[DS.Error] = $"No hay suficiente mobiliario disponible. Solo hay {mobiliario.Inventario} unidades.";

                    }
                    else
                    {
                        TempData[DS.Error] = $"No hay suficiente mobiliario disponible. Solo quedan {mobiliario.Inventario - cantidadAsignadaEnEventosSolapados} unidades.";

                    }

                    eventoMobiliarioVM.ListaMobiliario = _unidadTrabajo.EventoMobiliario.ObtenerMobiliario("Mobiliario", eventoMobiliarioVM.IdEvento);
                    return View(eventoMobiliarioVM);
                }


                if (eventoMobiliarioVM.IdRelacion == 0)
                {
                    EventoMobiliario existeMobiliario = await _unidadTrabajo.EventoMobiliario.ObtenerPrimero(X => X.IdEvento == eventoMobiliarioVM.IdEvento && X.IdMobiliario == eventoMobiliarioVM.IdMobiliario);
                    if (existeMobiliario != null)
                    {
                        TempData[DS.Error] = "Mobiliario ya existente";

                    }
                    else
                    {
                        var eventoMobiliario = new EventoMobiliario();
                        eventoMobiliario.IdEvento = eventoMobiliarioVM.IdEvento;
                        eventoMobiliario.IdMobiliario = eventoMobiliarioVM.IdMobiliario;
                        eventoMobiliario.Cantidad = eventoMobiliarioVM.Cantidad;

                        eventoMobiliario.Mobiliario = await _unidadTrabajo.Mobiliario.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdMobiliario);
                        eventoMobiliario.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdEvento);

                        await _unidadTrabajo.EventoMobiliario.Agregar(eventoMobiliario);

                        TempData[DS.Exitosa] = "Mobiliario agregado exitosamente";

                        var usuarioNombre = User.Identity.Name;

                        var mobiliarioBitacora = await _unidadTrabajo.Mobiliario.Obtener(eventoMobiliario.IdMobiliario);

                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó en evento del {eventoMobiliario.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                            $" a las {eventoMobiliario.Evento.HoraInicio} el mobiliario {mobiliarioBitacora.Nombre}", usuarioNombre);


                    }

                }
                else
                {

                    var eventoMobiliario = new EventoMobiliario();
                    eventoMobiliario.IdEvento = eventoMobiliarioVM.IdEvento;
                    eventoMobiliario.IdMobiliario = eventoMobiliarioVM.IdMobiliario;
                    eventoMobiliario.Cantidad = eventoMobiliarioVM.Cantidad;
                    eventoMobiliario.Id = eventoMobiliarioVM.IdRelacion;
                    
                    eventoMobiliario.Mobiliario = await _unidadTrabajo.Mobiliario.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdMobiliario);
                    eventoMobiliario.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoMobiliarioVM.IdEvento);

                    _unidadTrabajo.EventoMobiliario.Actualizar(eventoMobiliario);
                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";

                    var usuarioNombre = User.Identity.Name;

                    var mobiliarioBitacora = await _unidadTrabajo.Mobiliario.Obtener(eventoMobiliario.IdMobiliario);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó en evento del {eventoMobiliario.Evento.Fecha.ToString("dd/MM/yyyy")}" +
                        $" a las {eventoMobiliario.Evento.HoraInicio} el mobiliario {mobiliarioBitacora.Nombre}", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoMobiliario", new { id = eventoMobiliarioVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar mobiliario al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoMobiliarioVM);
            
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

            var todos = await _unidadTrabajo.EventoMobiliario.ObtenerTodos(incluirPropiedades: "Mobiliario");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }

        


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoMobiliarioBd = await _unidadTrabajo.EventoMobiliario.Obtener(id);
            if(EventoMobiliarioBd == null)
            {
                return Json(new { success = false, message = "Error al borrar mobiliario del evento" });
            }

            _unidadTrabajo.EventoMobiliario.Remover(EventoMobiliarioBd);
            await _unidadTrabajo.Guardar();

            var usuarioNombre = User.Identity.Name;

            var eventoBitacora = await _unidadTrabajo.Evento.Obtener(EventoMobiliarioBd.IdEvento);
            var mobiliarioBitacora = await _unidadTrabajo.Mobiliario.Obtener(EventoMobiliarioBd.IdMobiliario);


            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó en evento del {eventoBitacora.Fecha.ToString("dd/MM/yyyy")}" +
                            $" a las {eventoBitacora.HoraInicio} el mobiliario {mobiliarioBitacora.Nombre}", usuarioNombre);


            return Json(new { success = true, message = "Mobiliario borrado del evento exitosamente" });

        }



        #endregion
    }
}
