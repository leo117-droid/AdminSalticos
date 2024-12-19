using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IRolPersonalRepositorio : IRepositorio<RolPersonal>
    {
        void Actualizar(RolPersonal rolPersonal);

    }
}
