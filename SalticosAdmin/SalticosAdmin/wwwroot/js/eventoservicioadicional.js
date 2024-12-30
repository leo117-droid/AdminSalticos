﻿let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    var id = obtenerIdDesdeURL(); // Obtener la ID de la URL
    var padreId = id;

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
            "url": `/Admin/EventoServicioAdicional/ObtenerTodos?id=${id}`
        },
        "columns": [
            { "data": "servicioAdicional.nombre" },
            { "data": "cantidad" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/EventoServicioAdicional/Upsert/${padreId}?relacionId=${data}&eventoID=${padreId}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick = Delete("/Admin/EventoServicioAdicional/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
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
        title: "Esta seguro de Eliminar el servicio adicional?",
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
