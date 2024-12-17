using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio
{
    public class VehiculoRepositorio : Repositorio<Vehiculo>, IVehiculoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public VehiculoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Vehiculo vehiculo)
        {
            var vehiculoBD = _db.Vehiculos.FirstOrDefault(b => b.Id == vehiculo.Id);
            if (vehiculoBD != null)
            {
                vehiculoBD.Marca = vehiculo.Marca;
                vehiculoBD.Modelo = vehiculo.Modelo;
                vehiculoBD.Placa = vehiculo.Placa;
                vehiculoBD.TipoVehiculo = vehiculo.TipoVehiculo;
                _db.SaveChanges();

            }
        }

    }
}
