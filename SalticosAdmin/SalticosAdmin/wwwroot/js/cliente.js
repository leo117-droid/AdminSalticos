﻿let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
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
            "url": "/Admin/Cliente/ObtenerTodos"
        },
        "columns": [
            { "data": "nombre", "width": "20%" },
            { "data": "apellidos", "width": "20%" },
            { "data": "cedula", "width": "10%" },
            { "data": "telefono", "width": "10%" },
            { "data": "correo", "width": "10%" },

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/Cliente/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick = Delete("/Admin/Cliente/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                }, "width": "20%"
            }
        ]
    });
}


function Delete(url) {
    swal({
        title: "¿Está seguro de eliminar el cliente?",
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

                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
