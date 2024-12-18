﻿using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriasEdadController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriasEdadController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            CategoriasEdad categoriaEdad = new CategoriasEdad();


            if (id == null)
            {
                // Crear una nueva categoria por edad
                return View(categoriaEdad);
            }

            // Actualizamos
            categoriaEdad = await _unidadTrabajo.CategoriasEdad.Obtener(id.GetValueOrDefault());
            if (categoriaEdad == null)
            {
                return NotFound();
            }
            return View(categoriaEdad);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CategoriasEdad categoriasEdad)
        {
            if (ModelState.IsValid)
            {
                if (categoriasEdad.Id == 0)
                {
                    await _unidadTrabajo.CategoriasEdad.Agregar(categoriasEdad);
                    TempData[DS.Exitosa] = "Categoria por edad creado Exitosamente";
                }
                else
                {
                    _unidadTrabajo.CategoriasEdad.Actualizar(categoriasEdad);
                    TempData[DS.Exitosa] = "Categoria por edad actualizado Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar categoria por edad";
            return View(categoriasEdad);
        }


        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.CategoriasEdad.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriasEdadBd = await _unidadTrabajo.CategoriasEdad.Obtener(id);
            if (categoriasEdadBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoría por edad" });
            }
            _unidadTrabajo.CategoriasEdad.Remover(categoriasEdadBd);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoría por edad borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.CategoriasEdad.ObtenerTodos();
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
