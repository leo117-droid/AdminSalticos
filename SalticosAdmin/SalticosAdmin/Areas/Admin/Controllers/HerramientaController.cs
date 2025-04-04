﻿using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HerramientaController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public HerramientaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Herramienta herramienta = new Herramienta();


            if (id == null)
            {
                // Crear una nueva herramienta
                return View(herramienta);
            }

            // Actualizamos
            herramienta = await _unidadTrabajo.Herramienta.Obtener(id.GetValueOrDefault());
            if (herramienta == null)
            {
                return NotFound();
            }
            return View(herramienta);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Herramienta herramienta)
        {
            if (herramienta.Cantidad < 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad no puede ser negativa.");
            }

            if (ModelState.IsValid)
            {
                var usuarioNombre = User.Identity.Name;

                if (herramienta.Id == 0)
                {
                    await _unidadTrabajo.Herramienta.Agregar(herramienta);
                    TempData[DS.Exitosa] = "Herrramienta creada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó la herramienta '{herramienta.Nombre}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Herramienta.Actualizar(herramienta);
                    TempData[DS.Exitosa] = "Herramienta actualizada Exitosamente";

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó la herramienta '{herramienta.Nombre}'", usuarioNombre);

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar herramienta";
            return View(herramienta);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Herramienta.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var herramientaBd = await _unidadTrabajo.Herramienta.Obtener(id);

            var usuarioNombre = User.Identity.Name;

            if (herramientaBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Herramienta" });
            }
            _unidadTrabajo.Herramienta.Remover(herramientaBd);
            await _unidadTrabajo.Guardar();

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó la herramienta '{herramientaBd.Nombre}'", usuarioNombre);

            return Json(new { success = true, message = "Herramienta borrada exitosamente" });

        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Herramienta.ObtenerTodos();
            if (id == 0)
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
                }
            }
            else
            {
                if(nombre != null)
                {
                    valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
                }
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
