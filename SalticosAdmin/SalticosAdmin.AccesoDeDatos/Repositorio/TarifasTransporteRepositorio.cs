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
    public class TarifasTransporteRepositorio : Repositorio<TarifasTransporte>, ITarifasTransporteRepositorio
    {
        private readonly ApplicationDbContext _db;

        public TarifasTransporteRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(TarifasTransporte tarifasTransporte)
        {
            var tarifasBD = _db.TarifasTransportes.FirstOrDefault(b => b.Id == tarifasTransporte.Id);
            if (tarifasBD != null)
            {
                tarifasBD.Provincia = tarifasTransporte.Provincia;
                tarifasBD.Precio = tarifasTransporte.Precio;
                _db.SaveChanges();

            }
        }

    }
}
