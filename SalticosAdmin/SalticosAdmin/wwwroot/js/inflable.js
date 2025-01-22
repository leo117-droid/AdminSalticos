﻿let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    var dropdownTamanno = document.getElementById("categoriaTamannoSelect");

    var dropdownEdad = document.getElementById("categoriaEdadSelect");


    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros por página",
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
            "url": "/Admin/Inflable/ConsultarConFiltro"
        },
        "columns": [
            { "data": "nombre" },
            { "data": "descripcion"  },
            { "data": "dimensiones"},
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) {
                        return "Activo";
                    }
                    else {
                        return "Inactivo";
                    }
                },
            }, 
            {
                "data": "precio", "className": "text-end",
                "render": function (data) {
                    var d = `₡${data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
                    return d
                } },
            {
                "data": "precioHoraAdicional", "className": "text-end",
                "render": function (data) {
                    var d = `₡${data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
                    return d
                } },
            { "data": "categoriaTamanno.nombre" },
            { "data": "categoriasEdad.nombre"},

            {
                "data": "id",
                "width": "250px", // Define un ancho fijo para la columna
                "render": function (data) {
                    return `
                    <div class="d-flex justify-content-center gap-2">
                        <a href="/Admin/Inflable/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer; width: 70px;"> 
                            <i class="bi bi-pencil-square"></i> Editar
                        </a>
                        <a onclick="Delete('/Admin/Inflable/Delete/${data}')" class="btn btn-danger text-white" style="cursor:pointer; width: 70px;"> 
                            <i class="bi bi-trash3-fill"></i> Eliminar
                        </a>
                    </div>
                `;
                }
            }



        ]
    });
}



function refreshDataTable() {
    var dropdownTamanno = document.getElementById("categoriaTamannoSelect");

    var dropdownEdad = document.getElementById("categoriaEdadSelect");

    datatable.clear().draw();

    datatable.ajax.url(`/Admin/Inflable/ConsultarConFiltro?categoriaTamannoId=${dropdownTamanno.value}&categoriaEdadId=${dropdownEdad.value}`).load();

}

function clearDataTable() {

    document.getElementById('categoriaTamannoSelect').value = "";
    document.getElementById('categoriaEdadSelect').value = "";

    datatable.clear().draw();

    datatable.ajax.url(`/Admin/Inflable/ConsultarConFiltro`).load();

}


function Delete(url) {
    swal({
        title: "Esta seguro de Eliminar el Inflable?",
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
