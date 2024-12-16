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
        title: "Esta seguro de Eliminar el Cliente?",
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
