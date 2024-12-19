using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _db;
        public IHerramientaRepositorio Herramienta { get; private set; }
        public IClienteRepositorio Cliente { get; private set; }
        public IContactoRepositorio Contacto { get; private set; }
        public ITarifasTransporteRepositorio TarifasTransporte { get; private set; }

        public IVehiculoRepositorio Vehiculo { get; private set; }

        public ICategoriasEdadRepositorio CategoriasEdad { get; }

        public ICategoriaTamannoRepositorio CategoriaTamanno { get; private set; }

        public IRolPersonalRepositorio RolPersonal { get; private set; }


        public UnidadTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Herramienta = new HerramientaRepositorio(_db);
            Cliente = new ClienteRepositorio(_db);
            Contacto = new ContactoRepositorio(_db);
            TarifasTransporte = new TarifasTransporteRepositorio(_db);
            Vehiculo = new VehiculoRepositorio(_db);
            CategoriasEdad = new CategoriasEdadRepositorio(_db);
            CategoriaTamanno = new CategoriaTamannoRepositorio(_db);
            RolPersonal = new RolPersonalRepositorio(_db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task Guardar()
        {
            await _db.SaveChangesAsync();
        }
    }
}
