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
        IRolPersonalRepositorio RolPersonal { get; }

        IIngredienteRepositorio Ingrediente { get; }

        IPersonalRepositorio Personal { get; }

        IInflableRepositorio Inflable { get; }
        ICapacitacionRepositorio Capacitacion { get; }
        IServicioAdicionalRepositorio ServicioAdicional { get; }

        IMobiliarioRepositorio Mobiliario { get; }
        IAlimentacionRepositorio Alimentacion { get; }
        IBitacoraRepositorio Bitacora { get; }

        ICapacitacionPersonalRepositorio CapacitacionPersonal { get; }

        IAlimentacionIngredienteRepositorio AlimentacionIngrediente { get; }
        IEventoRepositorio Evento { get; }
        IEventoAlimentacionRepositorio EventoAlimentacion { get; }
        IEventoVehiculoRepositorio EventoVehiculo { get; }

        IEventoMobiliarioRepositorio EventoMobiliario { get; }
        IEventoPersonalRepositorio EventoPersonal { get; }
        IEventoInflableRepositorio EventoInflable { get; }


        Task Guardar();
    }

   
}
