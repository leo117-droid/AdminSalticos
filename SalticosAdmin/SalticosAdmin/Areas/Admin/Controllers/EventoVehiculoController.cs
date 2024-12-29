using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoVehiculoController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoVehiculoController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index(int? id)
        {
            if (id != null)
            {
                ViewData["Id"] = id;
            }

            return View();
        }


        public async Task<IActionResult> Upsert(int eventoID, int? relacionId)
        {


            EventoVehiculoVM eventoVehiculoVM = new EventoVehiculoVM()
            {
                IdEvento = eventoID,

                ListaVehiculo = _unidadTrabajo.EventoVehiculo.ObtenerVehiculo("Vehiculo", eventoID)
            };


            if (relacionId == null)
            {
                return View(eventoVehiculoVM);
            }
            else
            {

                eventoVehiculoVM.IdRelacion = relacionId.Value;
                var eventoVehiculoOBJ = await _unidadTrabajo.EventoVehiculo.ObtenerPrimero(X => X.Id == eventoVehiculoVM.IdRelacion);
                if (eventoVehiculoOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                eventoVehiculoVM.IdVehiculo = eventoVehiculoOBJ.IdVehiculo;


                return View(eventoVehiculoVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoVehiculoVM eventoVehiculoVM)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values
                               .SelectMany(v => v.Errors)
                               .Select(e => e.ErrorMessage)
                               .ToList();

                foreach (var error in allErrors)
                {
                    Console.WriteLine(error);
                }
            }
            if (ModelState.IsValid)
            {
                if (eventoVehiculoVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    EventoVehiculo existeVehiculo = await _unidadTrabajo.EventoVehiculo.ObtenerPrimero(X => X.IdEvento == eventoVehiculoVM.IdEvento && X.IdVehiculo == eventoVehiculoVM.IdVehiculo);
                    if (existeVehiculo != null)
                    {
                        TempData[DS.Error] = "vehiculo ya existente";
                        
                    }
                    else
                    {
                        var eventoVehiculo = new EventoVehiculo();
                        eventoVehiculo.IdEvento = eventoVehiculoVM.IdEvento;
                        eventoVehiculo.IdVehiculo = eventoVehiculoVM.IdVehiculo;

                        eventoVehiculo.Vehiculo = await _unidadTrabajo.Vehiculo.ObtenerPrimero(X => X.Id == eventoVehiculoVM.IdVehiculo);
                        eventoVehiculo.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoVehiculoVM.IdEvento);

                        await _unidadTrabajo.EventoVehiculo.Agregar(eventoVehiculo);

                        TempData[DS.Exitosa] = "Vehiculo agregado exitosamente";

                       
                    }

                }
                else
                {

                    var eventoVehiculo = new EventoVehiculo();
                    eventoVehiculo.IdEvento = eventoVehiculoVM.IdEvento;
                    eventoVehiculo.IdVehiculo = eventoVehiculoVM.IdVehiculo;
                    eventoVehiculo.Id = eventoVehiculoVM.IdRelacion;

                    eventoVehiculo.Vehiculo = await _unidadTrabajo.Vehiculo.ObtenerPrimero(X => X.Id == eventoVehiculoVM.IdVehiculo);
                    eventoVehiculo.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoVehiculoVM.IdEvento);

                    _unidadTrabajo.EventoVehiculo.Actualizar(eventoVehiculo);
                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";


                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoVehiculo", new { id = eventoVehiculoVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar vehiculo al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoVehiculoVM);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            int id = 0;
            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                int.TryParse(HttpContext.Request.Query["id"], out id);
            }

            var todos = await _unidadTrabajo.EventoVehiculo.ObtenerTodos(incluirPropiedades: "Vehiculo");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoVehiculoBd = await _unidadTrabajo.EventoVehiculo.Obtener(id);
            if(EventoVehiculoBd == null)
            {
                return Json(new { success = false, message = "Error al borrar vehiculo del evento" });
            }

            _unidadTrabajo.EventoVehiculo.Remover(EventoVehiculoBd);
            await _unidadTrabajo.Guardar();

            return Json(new { success = true, message = "Vehiculo borrado del evento exitosamente" });

        }

        #endregion
    }
}
