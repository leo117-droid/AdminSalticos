let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por página",
            "zeroRecords": "Sin tareas pendientes",
            "info": "Mostrando página _PAGE_ de _PAGES_",
            "infoEmpty": "No hay tareas disponibles",
            "infoFiltered": "(filtrado de _MAX_ registros totales)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url": "/Admin/Tarea/ObtenerTodos"
        },
        "columns": [
            {
                "data": "id", "render": function (data) {
                    return `
                        <div class="text-center">
                            <a onclick="ActualizarEstado(${data})" class="btn btn-primary text-white" style="cursor:pointer">
                                <i class="bi bi-check-circle"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "10%"
            },
            { "data": "titulo" },
            { "data": "descripcion" },
            { "data": "estado" },
            { "data": "prioridad" },
            {
                "data": "fecha",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            {
                "data": "hora",
                "render": function (data) {
                    return moment(data, "HH:mm:ss").format("hh:mm A");
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Tarea/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "10%"
            }
        ]
    });
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

