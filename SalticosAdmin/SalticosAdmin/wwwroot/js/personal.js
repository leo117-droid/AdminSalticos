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
            "url": "/Admin/Personal/ConsultarConFiltro"
        },
        "columns": [
            { "data": "nombre" },
            { "data": "apellidos"  },
            { "data": "telefono"},
            { "data": "correo"},
            { "data": "cedula" },
            {
                "data": "fechaNacimiento",
                "render": function (data) {
                    if (!data) {
                        return "No especificada"; 
                    }

                    let formattedDate = moment(data).format("DD/MM/YYYY");
                    let age = moment().diff(data, 'years'); 

                    if (age < 18) {
                        return `<span class="text-danger">${formattedDate} (Menor de edad)</span>`;
                    }

                    return formattedDate;
                }
            },
            {
                "data": "fechaEntrada",
                "render": function (data) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            { "data": "rolPersonal.nombre"},

            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class = "text-center">
                            <a href="/Admin/Personal/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer"> 
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick = Delete("/Admin/Personal/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer"> 
                                <i class = "bi bi-trash3-fill"></i>
                            </a>

                        </div>
                    `;
                }
            }
        ]
    });
}


function refreshDataTable() {
    var dropdown = document.getElementById("rolPersonalSelect");

    datatable.clear().draw();

    datatable.ajax.url(`/Admin/Personal/ConsultarConFiltro?idRolPersonal=${dropdown.value}`).load();

}

function clearDataTable() {
    document.getElementById('rolPersonalSelect').value = "";

    datatable.clear().draw();

    datatable.ajax.url(`/Admin/Personal/ConsultarConFiltro`).load();

}



function Delete(url) {
    swal({
        title: "Esta seguro de Eliminar el Personal?",
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
