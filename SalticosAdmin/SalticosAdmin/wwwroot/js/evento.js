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
            { "data": "direccion" },
            { "data": "provincia" },
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
                        <div class="text-center">
                            <!-- Botón Editar fuera del dropdown -->
                            <a href="/Admin/Evento/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            
                            <!-- Dropdown para las demás acciones -->
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-primary btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="bi bi-gear"></i> Acciones
                                </button>
                                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                                    <li><a class="dropdown-item" href="/Admin/EventoAlimentacion/Index/${data}"><i class="bi bi-cart"></i> Alimentación</a></li>
                                    <li><a class="dropdown-item" href="/Admin/EventoVehiculo/Index/${data}"><i class="bi bi-car-front-fill"></i> Vehículo</a></li>
                                    <li><a class="dropdown-item" href="/Admin/EventoMobiliario/Index/${data}"><i class="bi bi-wrench"></i> Mobiliario</a></li>
                                    <li><a class="dropdown-item" href="/Admin/EventoPersonal/Index/${data}"><i class="bi bi-person-fill-add"></i> Personal</a></li>
                                    <li><a class="dropdown-item" href="/Admin/EventoInflable/Index/${data}"><i class="bi bi-balloon-fill"></i> Inflable</a></li>
                                    <li><a class="dropdown-item" href="/Admin/EventoServicioAdicional/Index/${data}"><i class="bi bi-plus-square-dotted"></i> Servicios Adicionales</a></li>
                                </ul>
                            </div>

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
