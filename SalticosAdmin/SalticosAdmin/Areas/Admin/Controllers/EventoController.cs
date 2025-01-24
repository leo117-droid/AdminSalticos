using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SalticosAdmin.AccesoDeDatos.Data;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;
using System.Drawing.Printing;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnviroment;


        public EventoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnviroment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task<IActionResult> Index()
        {
            EventoVM eventoVM = new EventoVM()
            {
                Evento = new Evento(),
                ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente"),
                EventoLista = await _unidadTrabajo.Evento.ObtenerTodos(incluirPropiedades: "Cliente")
            };


            return View(eventoVM);
        }

        public IActionResult Intermedia()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            EventoVM eventoVM = new EventoVM()
            {
                Evento = new Evento(),
                ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente"), //
            };

            if (id == null)
            {
                //Crear nuevo personal
                return View(eventoVM);
            }
            else
            {
                eventoVM.Evento = await _unidadTrabajo.Evento.Obtener(id.GetValueOrDefault());
                if (eventoVM.Evento == null)
                {
                    return NotFound();
                }
                eventoVM.Evento = await _unidadTrabajo.Evento.Obtener(id.GetValueOrDefault());
                return View(eventoVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoVM eventoVM)
        {
            if (ModelState.IsValid)
            {

                var horaInicio = eventoVM.Evento.HoraInicio;
                if (horaInicio >= eventoVM.Evento.HoraFinal) {
                    ModelState.AddModelError("Evento.HoraInicio", "La hora inicio no puede ser posterior o igual al la de fin");
                    eventoVM.ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente");
                    return View(eventoVM);
                }

                var usuarioNombre = User.Identity.Name;


                if (eventoVM.Evento.Id == 0)
                {
                    await _unidadTrabajo.Evento.Agregar(eventoVM.Evento);
                    TempData[DS.Exitosa] = "Evento creado Exitosamente";

                    var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoVM.Evento.ClienteId);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó el evento de '{clienteBitacora.Nombre} {clienteBitacora.Apellidos} para {eventoVM.Evento.Fecha.ToString("dd/MM/yyyy")}'", usuarioNombre);

                }
                else
                {
                    _unidadTrabajo.Evento.Actualizar(eventoVM.Evento);
                    TempData[DS.Exitosa] = "Evento actualizado Exitosamente";

                    var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoVM.Evento.ClienteId);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó el evento de '{clienteBitacora.Nombre} {clienteBitacora.Apellidos} para {eventoVM.Evento.Fecha.ToString("dd/MM/yyyy")}'", usuarioNombre);
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }
            eventoVM.ClienteLista = _unidadTrabajo.Evento.ObtenerTodosDropdownLista("Cliente");

            return View(eventoVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Evento.ObtenerTodos(incluirPropiedades: "Cliente");
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
       
            var eventoBd = await _unidadTrabajo.Evento.Obtener(id);
            

            var usuarioNombre = User.Identity.Name;


            if (eventoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar Evento" });
            }

            //Eliminar entidades intermedias

            var eventoAlimentacion = await _unidadTrabajo.EventoAlimentacion.ObtenerTodos(ea => ea.IdEvento == id);
            if(eventoAlimentacion != null)
            {
                _unidadTrabajo.EventoAlimentacion.RemoverRango(eventoAlimentacion);
            }

            var eventoVehiculo = await _unidadTrabajo.EventoVehiculo.ObtenerTodos(ea => ea.IdEvento == id);
            if (eventoVehiculo != null)
            {
                _unidadTrabajo.EventoVehiculo.RemoverRango(eventoVehiculo);
            }
            var eventoMobiliario = await _unidadTrabajo.EventoMobiliario.ObtenerTodos(ea => ea.IdEvento == id);
            if (eventoMobiliario != null)
            {
                _unidadTrabajo.EventoMobiliario.RemoverRango(eventoMobiliario);
            }
            var eventoPersonal = await _unidadTrabajo.EventoPersonal.ObtenerTodos(ea => ea.IdEvento == id);
            if (eventoPersonal != null)
            {
                _unidadTrabajo.EventoPersonal.RemoverRango(eventoPersonal);
            }
            var eventoInflable= await _unidadTrabajo.EventoInflable.ObtenerTodos(ea => ea.IdEvento == id);
            if (eventoInflable != null)
            {
                _unidadTrabajo.EventoInflable.RemoverRango(eventoInflable);
            }
            var eventoServicioAdicional = await _unidadTrabajo.EventoServicioAdicional.ObtenerTodos(ea => ea.IdEvento == id);
            if (eventoServicioAdicional != null)
            {
                _unidadTrabajo.EventoServicioAdicional.RemoverRango(eventoServicioAdicional);
            }

            _unidadTrabajo.Evento.Remover(eventoBd);
            await _unidadTrabajo.Guardar();

            var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoBd.ClienteId);

            

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó el evento de '{clienteBitacora.Nombre} {clienteBitacora.Apellidos} para {eventoBd.Fecha.ToString("dd/MM/yyyy")}'", usuarioNombre);
           

            return Json(new { success = true, message = "Evento borrado exitosamente" });
            
        }



       





        #endregion
    }
}
