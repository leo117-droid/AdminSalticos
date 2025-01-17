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

        // Método para obtener los alimentos más solicitados y la cantidad de veces que fueron pedidos
        public List<AlimentoConCantidad> ObtenerAlimentosMasSolicitados()
        {
            var alimentosMasSolicitados = _context.EventoAlimentacion
                .GroupBy(ea => ea.IdAlimentacion) // Agrupar por Alimentación
                .Select(group => new AlimentoConCantidad
                {
                    IdAlimentacion = group.Key, // El Id del alimento
                    Nombre = group.First().Alimentacion.Nombre, // Nombre del alimento
                    Cantidad = group.Sum(ea => ea.Cantidad) // Sumar la cantidad total de solicitudes
                })
                .OrderByDescending(a => a.Cantidad) // Ordenar por la cantidad de veces solicitado
                .ToList();

            return alimentosMasSolicitados;
        }



        // Método para obtener los mobiliarios más solicitados
        public List<MobiliarioConCantidad> ObtenerMobiliariosMasSolicitados()
        {
            var mobiliariosMasSolicitados = _context.EventoMobiliario
                .GroupBy(em => em.IdMobiliario)
                .Select(group => new MobiliarioConCantidad
                {
                    IdMobiliario = group.Key,
                    Nombre = group.First().Mobiliario.Nombre,
                    Cantidad = group.Sum(em => em.Cantidad)
                })
                .OrderByDescending(m => m.Cantidad)
                .ToList();

            return mobiliariosMasSolicitados;
        }


    }
}




