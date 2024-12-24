using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IBitacoraRepositorio : IRepositorio<Bitacora>
    {
        Task RegistrarBitacora(string accion, string usuario);

        Task<IEnumerable<Bitacora>> ObtenerEntreFechas(DateTime fechaInicio, DateTime fechaFin);
    }
}
