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
            "url": "/Admin/Evento/ObtenerTodos"
        },
        "columns": [
            {
                "data": "fecha",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            {
                "data": "horaInicio",
                "render": function (data) {
                    return moment(data, "HH:mm:ss").format("hh:mm A"); 
                }
            },
            {
                "data": "horaFinal",
                "render": function (data) {
                    return moment(data, "HH:mm:ss").format("hh:mm A"); 
                }
            },
            { "data": "direccion"},
            { "data": "provincia"},
            {
                "data": null, 
                "render": function (data) {
                    return `${data.cliente.nombre} ${data.cliente.apellidos}`;
                }
            },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/Evento/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>

                            <a href="/Admin/EventoAlimentacion/Index/${data}" class="btn btn-primary btn-secondary btn-outline-white" style="cursor:pointer">
                                <i class="bi bi-cart"></i> Alimentacion
                            </a>

                            <a href="/Admin/EventoVehiculo/Index/${data}" class="btn btn-primary btn-secondary btn-outline-white" style="cursor:pointer">
                                <i class="bi bi-car-front-fill"></i> Vehiculo
                            </a>

                            <a onclick = Delete("/Admin/Evento/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                },
            }
        ]
    });
}


function Delete(url) {
    swal({
        title: "Esta seguro de Eliminar el Evento?",
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
