let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Pagina",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar page _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url": "/Admin/Capacitacion/ObtenerTodos"
        },
        "columns": [
            {
                "data": "fecha",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            { "data": "tema", "width": "40%" },
            { "data": "duracion", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/Capacitacion/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a href="/Admin/CapacitacionPersonal/Index/${data}" class="btn btn-primary btn-secondary btn-outline-white" style="cursor:pointer">
                                <i class="bi bi-people-fill"></i> Participantes
                            </a>

                            <a onclick = Delete("/Admin/Capacitacion/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                }, "width": "40%"
            }
        ]
    });
}


function Delete(url) {
    swal({
        title: "Esta seguro de Eliminar la Capacitacion?",
        text: "Este registro no se podra recuperar",
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

                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
