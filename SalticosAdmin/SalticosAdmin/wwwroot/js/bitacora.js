﻿let datatable;

$(document).ready(function () {
    loadDataTable();

    $('#btnBuscar').on('click', function () {
        filterDataTable();
    });

    $('#btnLimpiarFiltro').on('click', function () {
        clearDataTable();
    });


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
            "url": "/Admin/Bitacora/ConsultarConFiltro",
            "data": function (d) {
                d.fechainicial = $('#fechaInicio').val();
                d.fechafinal = $('#fechaFin').val();
            }
        },
        "columns": [
            {
                "data": "fecha",
                "width": "15%",
                "render": function (data) {
                    var fecha = new Date(data);
                    var options = { year: 'numeric', month: 'short', day: 'numeric' };
                    return fecha.toLocaleDateString('es-ES', options);
                }
            },
            {
                "data": "hora",
                "width": "15%",
                "render": function (data) {
                    var partes = data.split(':'); 
                    var hora = parseInt(partes[0], 10); 
                    var minutos = partes[1]; 
                    var periodo = hora >= 12 ? 'PM' : 'AM'; 

                    hora = hora % 12 || 12;

                    return `${hora}:${minutos} ${periodo}`;
                }
            },
            { "data": "accion", "width": "50%" },
            { "data": "usuario", "width": "20%" }
        ],
        "order": [[0, "desc"]] // Ordenar por la primera columna (fecha) en orden descendente
    });
}


function filterDataTable() {
    datatable.ajax.reload();
}

function clearDataTable() {
    $('#fechaInicio').val('');
    $('#fechaFin').val('');
    datatable.ajax.reload();
}
