using Microsoft.AspNetCore.Mvc;
using SalticosAdmin.AccesoDeDatos.Repositorio;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventoAlimentacionController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;


        public EventoAlimentacionController(IUnidadTrabajo unidadTrabajo)
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


            EventoAlimentacionVM eventoAlimentacionVM = new EventoAlimentacionVM()
            {
                IdEvento = eventoID,

                ListaAlimentacion = _unidadTrabajo.EventoAlimentacion.ObtenerAlimentacion("Alimentacion", eventoID)
            };


            if (relacionId == null)
            {
                return View(eventoAlimentacionVM);
            }
            else
            {

                eventoAlimentacionVM.IdRelacion = relacionId.Value;
                var eventoAlimentacionOBJ = await _unidadTrabajo.EventoAlimentacion.ObtenerPrimero(X => X.Id == eventoAlimentacionVM.IdRelacion);
                if (eventoAlimentacionOBJ == null)
                {
                    return NotFound();
                }


                //  productoPrecioVM.tipoPrecioNombre = productoPrecioVM.ListaPrecios.Where(x => x.Value.Equals(productoPrecioOBJ.Idprecio.ToString())).Select(x=>x.Text).FirstOrDefault();
                eventoAlimentacionVM.IdAlimentacion = eventoAlimentacionOBJ.IdAlimentacion;


                return View(eventoAlimentacionVM);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EventoAlimentacionVM eventoAlimentacionVM)
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
                if (eventoAlimentacionVM.IdRelacion == 0)
                {
                    //productoPrecioVM.productoPrecio.Idproducto = id;
                    EventoAlimentacion existeAlimento = await _unidadTrabajo.EventoAlimentacion.ObtenerPrimero(X => X.IdEvento == eventoAlimentacionVM.IdEvento && X.IdAlimentacion == eventoAlimentacionVM.IdAlimentacion);
                    if (existeAlimento != null)
                    {
                        TempData[DS.Error] = "alimento ya existente";
                        
                    }
                    else
                    {
                        var eventoAlimentacion = new EventoAlimentacion();
                        eventoAlimentacion.IdEvento = eventoAlimentacionVM.IdEvento;
                        eventoAlimentacion.IdAlimentacion = eventoAlimentacionVM.IdAlimentacion;
                        eventoAlimentacion.Cantidad = eventoAlimentacionVM.Cantidad;

                        eventoAlimentacion.Alimentacion = await _unidadTrabajo.Alimentacion.ObtenerPrimero(X => X.Id == eventoAlimentacionVM.IdAlimentacion);
                        eventoAlimentacion.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoAlimentacionVM.IdEvento);

                        await _unidadTrabajo.EventoAlimentacion.Agregar(eventoAlimentacion);

                        var usuarioNombre = User.Identity.Name;

                        var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoAlimentacion.Evento.ClienteId);
                        var alimentacionBitacora = await _unidadTrabajo.Alimentacion.Obtener(eventoAlimentacion.IdAlimentacion);

                        await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se agregó en evento de '{clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                            $" para {eventoAlimentacion.Evento.Fecha.ToString("dd/MM/yyyy")} el producto de alimentación {alimentacionBitacora.Nombre}", usuarioNombre);


                        TempData[DS.Exitosa] = "Alimentacion agregado exitosamente";

                       
                    }

                }
                else
                {

                    var eventoAlimentacion = new EventoAlimentacion();
                    eventoAlimentacion.IdEvento = eventoAlimentacionVM.IdEvento;
                    eventoAlimentacion.IdAlimentacion = eventoAlimentacionVM.IdAlimentacion;
                    eventoAlimentacion.Cantidad = eventoAlimentacionVM.Cantidad;
                    eventoAlimentacion.Id = eventoAlimentacionVM.IdRelacion;

                    eventoAlimentacion.Alimentacion = await _unidadTrabajo.Alimentacion.ObtenerPrimero(X => X.Id == eventoAlimentacionVM.IdAlimentacion);
                    eventoAlimentacion.Evento = await _unidadTrabajo.Evento.ObtenerPrimero(X => X.Id == eventoAlimentacionVM.IdEvento);

                    _unidadTrabajo.EventoAlimentacion.Actualizar(eventoAlimentacion);

                    var usuarioNombre = User.Identity.Name;

                    var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoAlimentacion.Evento.ClienteId);
                    var alimentacionBitacora = await _unidadTrabajo.Alimentacion.Obtener(eventoAlimentacion.IdAlimentacion);

                    await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se actualizó en evento de {clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                        $" para {eventoAlimentacion.Evento.Fecha.ToString("dd/MM/yyyy")} producto de alimentación {alimentacionBitacora.Nombre}", usuarioNombre);

                    TempData[DS.Exitosa] = "Evento actualizado exitosamente";


                }
                await _unidadTrabajo.Guardar();
                string returnUrl = Url.Action("Index", "EventoAlimentacion", new { id = eventoAlimentacionVM.IdEvento });
                return Redirect(returnUrl);
            }
            var mensajeError = "Error al grabar alimentacion al evento";
            TempData[DS.Error] = mensajeError;
            return View(eventoAlimentacionVM);
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

            var todos = await _unidadTrabajo.EventoAlimentacion.ObtenerTodos(incluirPropiedades: "Alimentacion");

            var filtrados = todos.Where(t => t.IdEvento == id);
            return Json(new { data = filtrados });

        }

        


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var EventoAlimentacionBd = await _unidadTrabajo.EventoAlimentacion.Obtener(id);
            if(EventoAlimentacionBd == null)
            {
                return Json(new { success = false, message = "Error al borrar alimentacion del evento" });
            }

            _unidadTrabajo.EventoAlimentacion.Remover(EventoAlimentacionBd);
            await _unidadTrabajo.Guardar();

            var usuarioNombre = User.Identity.Name;
            
            var eventoBitacora = await _unidadTrabajo.Evento.Obtener(EventoAlimentacionBd.IdEvento);
            var alimentacionBitacora = await _unidadTrabajo.Alimentacion.Obtener(EventoAlimentacionBd.IdAlimentacion);
            var clienteBitacora = await _unidadTrabajo.Cliente.Obtener(eventoBitacora.ClienteId);

            await _unidadTrabajo.Bitacora.RegistrarBitacora($"Se eliminó en evento de {clienteBitacora.Nombre} {clienteBitacora.Apellidos}" +
                $" para {eventoBitacora.Fecha.ToString("dd/MM/yyyy")} el producto de alimentación {alimentacionBitacora.Nombre}", usuarioNombre);

            return Json(new { success = true, message = "Alimentacion borrado del evento exitosamente" });

        }



        #endregion
    }
}
