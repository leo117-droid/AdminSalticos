using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IPersonalRepositorio : IRepositorio<Personal>
    {
        void Actualizar(Personal personal);


    }
}