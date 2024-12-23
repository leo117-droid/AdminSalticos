﻿using SalticosAdmin.AccesoDeDatos.Data;
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

        public ICategoriasEdadRepositorio CategoriasEdad { get; private set; }

        public ICategoriaTamannoRepositorio CategoriaTamanno { get; private set; }

        public IRolPersonalRepositorio RolPersonal { get; private set; }

        public IIngredienteRepositorio Ingrediente { get; private set; }

        public IPersonalRepositorio Personal { get; private set; }

        public IInflableRepositorio Inflable { get; private set; }
        public ICapacitacionRepositorio Capacitacion { get; private set; }
        public IServicioAdicionalRepositorio ServicioAdicional { get; private set; }

        public IMobiliarioRepositorio Mobiliario { get; private set; }

        public IAlimentacionRepositorio Alimentacion { get; private set; }
        public IBitacoraRepositorio Bitacora { get; private set; }



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
            Ingrediente = new IngredienteRepositorio(_db);
            Personal = new PersonalRepositorio(_db);
            Inflable = new InflableRepositorio(_db);
            Capacitacion = new CapacitacionRepositorio(_db);
            ServicioAdicional = new ServicioAdicionalRepositorio(_db);
            Mobiliario = new MobiliarioRepositorio(_db);
            Alimentacion = new AlimentacionRepositorio(_db);
            Bitacora = new BitacoraRepositorio(_db);


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
