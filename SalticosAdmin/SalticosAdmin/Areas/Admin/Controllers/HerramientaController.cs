﻿using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CapacitacionController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public CapacitacionController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Capacitacion capacitacion = new Capacitacion();


            if (id == null)
            {
                // Crear una nueva capacitacion
                return View(capacitacion);
            }

            // Actualizamos
            capacitacion = await _unidadTrabajo.Capacitacion.Obtener(id.GetValueOrDefault());
            if (capacitacion == null)
            {
                return NotFound();
            }
            return View(capacitacion);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Capacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                if (capacitacion.Id == 0)
                {
                    await _unidadTrabajo.Capacitacion.Agregar(capacitacion);
                    TempData[DS.Exitosa] = "Herrramienta creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Capacitacion.Actualizar(capacitacion);
                    TempData[DS.Exitosa] = "Herramienta actualizada Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar capacitacion";
            return View(capacitacion);
        }
        

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Capacitacion.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var capacitacionBd = await _unidadTrabajo.Capacitacion.Obtener(id);
            if (capacitacionBd == null)
            {
                return Json(new { success = false, message = "Error al borrar capacitacion" });
            }
            _unidadTrabajo.Capacitacion.Remover(capacitacionBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "CapacitacionBd borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Capacitacion.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }

        #endregion
    }
}
