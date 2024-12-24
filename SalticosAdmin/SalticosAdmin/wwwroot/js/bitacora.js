let datatable;

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
            "url": "/Admin/Bitacora/ConsultarConFiltro",
            "data": function (d) {
                d.fechainicial = $('#fechaInicio').val();
                d.fechafinal = $('#fechaFin').val();
            }
        },
        "columns": [
            {
                "data": "fecha",
                "width": "20%",
                "render": function (data) {
                    var fecha = new Date(data);
                    var options = { year: 'numeric', month: 'short', day: 'numeric' };
                    return fecha.toLocaleDateString('es-ES', options);
                }
            },
            {
                "data": "hora",
                "width": "20%",
                "render": function (data) {
                    var partes = data.split(':'); 
                    var hora = parseInt(partes[0], 10); 
                    var minutos = partes[1]; 
                    var periodo = hora >= 12 ? 'PM' : 'AM'; 

                    hora = hora % 12 || 12;

                    return `${hora}:${minutos} ${periodo}`;
                }
            },
            { "data": "accion", "width": "30%" },
            { "data": "usuario", "width": "30%" }
        ]
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
