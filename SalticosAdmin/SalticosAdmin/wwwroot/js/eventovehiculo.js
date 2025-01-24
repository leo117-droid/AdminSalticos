let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    var id = obtenerIdDesdeURL(); // Obtener la ID de la URL
    var padreId = id;

    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros por página",
            "zeroRecords": "Ningún registro",
            "info": "Mostrar página _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
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
            "url": `/Admin/EventoVehiculo/ObtenerTodos?id=${id}`
        },
        "columns": [
            { "data": "vehiculo.modelo" },
            { "data": "vehiculo.placa" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            
                            <a onclick = Delete("/Admin/EventoVehiculo/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                }
            }
        ]
    });
}


function obtenerIdDesdeURL() {

    var urlParams = window.location.pathname.split('/');
    return urlParams[urlParams.length - 1];
}



function Delete(url) {
    swal({
        title: "Esta seguro de Eliminar el Vehiculo?",
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
