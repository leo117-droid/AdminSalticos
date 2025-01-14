using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.Modelos;

namespace SalticosAdmin.Servicios
{
    public class InformeServicio
    {
        private readonly ApplicationDbContext _context;

        public InformeServicio(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener los inflables más utilizados
        public List<Inflable> ObtenerInflablesMasUtilizados()
        {
            var inflablesUtilizados = _context.EventoInflable
                .GroupBy(ei => ei.IdInflable)  // Agrupar por Inflable
                .Select(g => new
                {
                    InflableId = g.Key,
                    Cantidad = g.Count()  // Contar cuántas veces ha sido utilizado
                })
                .OrderByDescending(x => x.Cantidad)  // Ordenar por la cantidad
                .ToList();

            // Obtener los inflables con sus nombres y cantidades
            var inflables = inflablesUtilizados.Select(x =>
            {
                var inflable = _context.Inflables.FirstOrDefault(i => i.Id == x.InflableId);
                inflable.Nombre = inflable.Nombre;  // Asignar el nombre al inflable
                return inflable;
            }).ToList();

            return inflables;
        }
    }

}
