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

        // Método para obtener los inflables más solicitados y la cantidad de veces que fueron usados
        public List<InflableConCantidad> ObtenerInflablesMasSolicitados()
        {
            var inflablesMasSolicitados = _context.EventoInflable
                .GroupBy(ei => ei.IdInflable) // Agrupar por Inflable
                .Select(group => new InflableConCantidad
                {
                    IdInflable = group.Key,  // El Id del Inflable
                    Nombre = group.First().Inflable.Nombre,  // Nombre del inflable
                    Cantidad = group.Count()  // Contar las veces que aparece este inflable
                })
                .OrderByDescending(i => i.Cantidad)  // Ordenar por la cantidad de veces utilizado
                .ToList();

            return inflablesMasSolicitados;
        }
    }

    // Clase auxiliar para representar inflables con su cantidad de uso
    public class InflableConCantidad
    {
        public int IdInflable { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
    }


}
