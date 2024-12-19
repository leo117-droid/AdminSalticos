using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IUnidadTrabajo : IDisposable
    {
        IHerramientaRepositorio Herramienta { get; }

        IClienteRepositorio Cliente { get; }

        IContactoRepositorio Contacto { get; }

        ITarifasTransporteRepositorio TarifasTransporte { get; }

        IVehiculoRepositorio Vehiculo { get; }

        ICategoriasEdadRepositorio CategoriasEdad { get; }
        ICategoriaTamannoRepositorio CategoriaTamanno { get; }

        Task Guardar();
    }

   
}
