using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;

namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    namespace SalticosAdmin.AccesoDeDatos.Repositorio
    {
        public class TareaRepositorio : Repositorio<Tarea>, ITareaRepositorio
        {
            private readonly ApplicationDbContext _db;

            public TareaRepositorio(ApplicationDbContext db) : base(db)
            {
                _db = db;
            }

            public void Actualizar(Tarea tarea)
            {
                var tareaBD = _db.Tareas.FirstOrDefault(t => t.Id == tarea.Id);
                if (tareaBD != null)
                {
                    tareaBD.Titulo = tarea.Titulo;
                    tareaBD.Descripcion = tarea.Descripcion;
                    tareaBD.Prioridad = tarea.Prioridad;
                    tareaBD.Fecha = tarea.Fecha;
                    tareaBD.Hora = tarea.Hora;
                    tareaBD.Estado = tarea.Estado;

                    _db.SaveChanges();
                }
            }
            public IEnumerable<SelectListItem> ObtenerTodosDropdownLista(string obj)
            {
                if (obj == "Prioridad")
                {
                    return _db.Tareas.Select(t => t.Prioridad).Distinct()
                        .Select(prioridad => new SelectListItem
                        {
                            Text = prioridad,
                            Value = prioridad
                        });
                }
                if (obj == "Estado")
                {
                    return _db.Tareas.Select(t => t.Estado).Distinct()
                        .Select(estado => new SelectListItem
                        {
                            Text = estado,
                            Value = estado
                        });
                }

                return null;
            }
            public async Task<IEnumerable<Tarea>> FiltrarPorEstado(string estado)
            {
                return await _db.Tareas.Where(t => t.Estado == estado).ToListAsync();
            }
            public async Task<IEnumerable<Tarea>> FiltrarPorPrioridad(string prioridad)
            {
                return await _db.Tareas.Where(t => t.Prioridad == prioridad).ToListAsync();
            }

        }
    }


}
