let datatable;

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
            "url": "/Admin/Seguro/ObtenerTodos"
        },
        "columns": [
            { "data": "tipoSeguro" },
            { "data": "nombreAseguradora" },
            { "data": "numeroPoliza" },
            {
                "data": "fechaInicio",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            {
                "data": "fechaVencimiento",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) {
                        return "Vigente";
                    }
                    else {
                        return "Vencido";
                    }
                },
            }, 
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/Seguro/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>

                            <a onclick = Delete("/Admin/Seguro/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                }, 
            }
        ]
    });
}

function validarFechas(id) {
    $.ajax({
        type: "GET",
        url: `/Admin/Seguro/ObtenerSeguroPorId/${id}`,
        success: function (data) {
            var fechaInicio = new Date(data.fechaInicio);
            var fechaVencimiento = new Date(data.fechaVencimiento);

            if (fechaVencimiento <= fechaInicio) {
                swal("Error", "La fecha de vencimiento debe ser posterior a la fecha de inicio", "error");
                return false;
            } else {
                window.location.href = `/Admin/Seguro/Upsert/${id}`;
            }
        }
    });
}

function Delete(url) {
    swal({
        title: "¿Está seguro de eliminar el seguro?",
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
