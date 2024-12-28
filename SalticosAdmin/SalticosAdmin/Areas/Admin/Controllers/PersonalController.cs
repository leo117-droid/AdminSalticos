﻿using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public EventoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnviroment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task<IActionResult> Index()
        {
            EventoVM eventoVM = new EventoVM()
            {
                Evento = new Evento(),
                ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente"),
                EventoLista = await _unidadTrabajo.Evento.ObtenerTodos(incluirPropiedades: "Cliente")
            };


            return View(eventoVM);
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            EventoVM eventoVM = new EventoVM()
            {
                Evento = new Evento(),
                ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente"),
            };

            if(id == null)
            {
                //Crear nuevo personal
                return View(eventoVM);
            }
            else
            {
                eventoVM.Evento = await _unidadTrabajo.Evento.Obtener(id.GetValueOrDefault());
                if (eventoVM.Evento == null)
                {
                    return NotFound();
                }
                return View(eventoVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoVM eventoVM)
        {

            if (ModelState.IsValid)
            {
                //var usuarioNombre = User.Identity.Name;
                var usuarioNombre = "usuarioPrueba";

                if (eventoVM.Evento.Id == 0)
                {
                    await _unidadTrabajo.Evento.Agregar(eventoVM.Evento);
                    TempData[DS.Exitosa] = "Evento creado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se insertó el evento '{eventoVM.Evento.Id}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Evento.Actualizar(eventoVM.Evento);
                    TempData[DS.Exitosa] = "Evento actualizado Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el evento '{eventoVM.Evento.Id}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }
            eventoVM.ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente");

            return View(eventoVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Evento.ObtenerTodos(incluirPropiedades:"Cliente");
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var eventoBd = await _unidadTrabajo.Evento.Obtener(id);

            //var usuarioNombre = User.Identity.Name;
            var usuarioNombre = "usuarioPrueba";
            
            if (eventoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Evento" });
            }
            _unidadTrabajo.Evento.Remover(eventoBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el evento '{eventoBd.Id}'", usuarioNombre);

            return Json(new { success = true, message = "Evento borrado exitosamente" });
        }

        #endregion
    }
}
