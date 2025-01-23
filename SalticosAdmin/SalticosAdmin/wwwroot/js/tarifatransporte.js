let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros por página",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar page _PAGE_ de _PAGES_",
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
            "url": "/Admin/TarifasTransporte/ObtenerTodos"
        },
        "columns": [
            { "data": "provincia", "width": "40%" },
            {
                "data": "precio", "className": "text-end",
                "render": function (data) {
                    var d = `₡${data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
                    return d
                } },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/TarifasTransporte/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick = Delete("/Admin/TarifasTransporte/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
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
        title: "Esta seguro de Eliminar la Tarifa de Transporte?",
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
