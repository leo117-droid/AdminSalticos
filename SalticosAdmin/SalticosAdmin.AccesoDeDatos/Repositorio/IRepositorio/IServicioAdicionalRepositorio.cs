using Microsoft.AspNetCore.Mvc.Rendering;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio
{
    public interface IServicioAdicionalRepositorio : IRepositorio<ServicioAdicional>
    {
        void Actualizar(ServicioAdicional servicioAdicional);

        Task<List<ServicioAdicional>> ObtenerServiciosAdicionalesSolapados(DateTime fechaEvento, TimeSpan horaInicio, TimeSpan horaFin);


    }
}