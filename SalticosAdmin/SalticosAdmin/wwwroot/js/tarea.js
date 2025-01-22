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
                    const card = `
                        <div class="col-md-6 col-lg-4">
                            <div class="card shadow-sm">
                                <div class="card-body">
                                    <h5 class="card-title">${tarea.titulo}</h5>
                                    <p class="card-text">${tarea.descripcion}</p>
                                    <p>
                                        <span class="badge bg-${getEstadoBadge(tarea.estado)}">${tarea.estado}</span>
                                        <span class="badge bg-${getPrioridadBadge(tarea.prioridad)}">${tarea.prioridad}</span>
                                    </p>
                                    <p class="text-muted">
                                        <i class="bi bi-calendar-event"></i> ${moment(tarea.fecha).format("DD/MM/YYYY")} 
                                        <i class="bi bi-clock"></i> ${moment(tarea.hora, "HH:mm:ss").format("hh:mm A")}
                                    </p>
                                    <div class="d-flex justify-content-between">
                                        <button class="btn btn-primary" onclick="ActualizarEstado(${tarea.id})">
                                            <i class="bi bi-check-circle"></i> Completar
                                        </button>
                                        <a href="/Admin/Tarea/Upsert/${tarea.id}" class="btn btn-success">
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
    return estado === "Pendiente" ? "warning" : "success";
}

function getPrioridadBadge(prioridad) {
    switch (prioridad) {
        case "Alta": return "danger";
        case "Media": return "primary";
        case "Baja": return "secondary";
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
                        // Recargar o actualizar la lista de tareas si es necesario
                        datatable.ajax.reload();
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

