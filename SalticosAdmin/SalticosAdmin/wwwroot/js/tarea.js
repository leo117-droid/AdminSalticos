$(document).ready(function () {
    cargarTareas();
});

function cargarTareas() {
    $.ajax({
        url: "/Admin/Tarea/ObtenerTodos",
        method: "GET",
        success: function (response) {
            const contenedor = $("#task-list");
            contenedor.empty();

            if (response.data && response.data.length > 0) {
                response.data.forEach(tarea => {
                    const descripcion = tarea.descripcion && tarea.descripcion.trim() !== ""
                        ? tarea.descripcion
                        : "<em>Sin descripción</em>";

                    const card = `
                        <div class="col-md-6 col-lg-4" id="tarea-${tarea.id}">
                            <div class="card-tarea shadow-sm ${getEstadoClase(tarea.estado)}">
                                <div class="card-body">
                                    <button class="btn-close float-end" aria-label="Close" onclick="EliminarTarea(${tarea.id})"></button>
                                    <h5 class="card-tarea-title">${tarea.titulo}</h5>
                                    <p class="card-text">${descripcion}</p>
                                    <p>
                                        <span class="badge bg-${getEstadoBadge(tarea.estado)}">${tarea.estado}</span>
                                        <span class="badge bg-${getPrioridadBadge(tarea.prioridad)}">${tarea.prioridad}</span>
                                    </p>
                                    <p class="text-muted">
                                        <i class="bi bi-calendar-event"></i> ${moment(tarea.fecha).format("DD/MM/YYYY")} 
                                        <i class="bi bi-clock"></i> ${moment(tarea.hora, "HH:mm:ss").format("hh:mm A")}
                                    </p>
                                    <div class="d-flex justify-content-between">
                                        <button class="btn btn-success" onclick="ActualizarEstado(${tarea.id})">
                                            <i class="bi bi-check-circle"></i> Completar
                                        </button>
                                        <a href="/Admin/Tarea/Upsert/${tarea.id}" class="btn btn-primary">
                                            <i class="bi bi-pencil-square"></i> Editar
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                    contenedor.append(card);
                });
            } else {
                contenedor.html('<p class="text-center text-muted">No hay tareas pendientes</p>');
            }
        },
        error: function () {
            toastr.error("No se pudieron cargar las tareas.");
        }
    });
}

function getEstadoBadge(estado) {
    switch (estado) {
        case "Pendiente": return "pendiente";
        case "En Progreso": return "en-progreso";
        case "Completada": return "completado";
        default: return "default";
    }
}


function getEstadoClase(estado) {
    switch (estado) {
        case "Pendiente": return "bg-pendiente";
        case "En Progreso": return "bg-en-progreso";
        case "Completada": return "bg-completada";
        default: return "";
    }
}


function getPrioridadBadge(prioridad) {
    switch (prioridad) {
        case "Alta": return "alto";
        case "Media": return "bajo";
        case "Baja": return "medio";
        default: return "light";
    }
}

function ActualizarEstado(id) {
    swal({
        title: "¿Está seguro de marcar esta tarea como completada?",
        icon: "info",
        buttons: true,
        dangerMode: false
    }).then((completar) => {
        if (completar) {
            $.ajax({
                type: "POST",
                url: "/Admin/Tarea/ActualizarEstado",
                data: { id: id },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        // Refrescar la página automáticamente
                        location.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Hubo un error al intentar marcar la tarea como completada.");
                }
            });
        }
    });
}

function EliminarTarea(id) {
    swal({
        title: "¿Está seguro de eliminar esta tarea?",
        text: "¡No podrás deshacer esta acción!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((confirmar) => {
        if (confirmar) {
            $.ajax({
                type: "POST",
                url: "/Admin/Tarea/Delete",
                data: { id: id },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        // Eliminar la tarjeta del DOM
                        $(`#tarea-${id}`).remove();

                        // Si no hay más tareas, mostrar el mensaje de vacío
                        if ($("#task-list").children().length === 0) {
                            $("#task-list").html('<p class="text-center text-muted">No hay tareas pendientes</p>');
                        }
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Hubo un error al intentar eliminar la tarea.");
                }
            });
        }
    });
}
