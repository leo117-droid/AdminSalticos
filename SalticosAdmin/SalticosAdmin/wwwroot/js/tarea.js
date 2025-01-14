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
            { "data": "titulo"},
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
                }            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Tarea/Editar/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick="Delete('/Admin/Tarea/Delete/${data}')" class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="bi bi-trash3-fill"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "¿Está seguro de eliminar esta tarea?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
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

