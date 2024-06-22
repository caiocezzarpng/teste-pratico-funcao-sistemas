
$(document).ready(function () {

    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable({
            title: 'Clientes',
            paging: true, //Enable paging
            pageSize: 5, //Set page size (default: 10)
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: urlClienteList,
                removeAction: '/Cliente/Excluir'
            },
            fields: {
                Nome: {
                    title: 'Nome',
                    width: '50%'
                },
                Email: {
                    title: 'Email',
                    width: '35%'
                },
                Alterar: {
                    title: '',
                    display: function (data) {
                        return '<button onclick="window.location.href=\'' + urlAlteracao + '/' + data.record.Id + '\'" class="btn btn-primary btn-sm">Alterar</button>';
                    }
                },
                Apagar: { 
                    title: '',
                    display: function (data) {
                        return '<button onclick="apagarCliente(' + data.record.Id + ')" class="btn btn-danger btn-sm">Apagar</button>';
                    }
                }
            }
        });
    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable('load');
})

function apagarCliente(clienteId) {
    if (confirm('Tem certeza que deseja excluir este cliente?')) {
        $.post('/Cliente/Excluir', { id: clienteId }, function (data) {
            debugger
            if (data.Result === "OK") {
                $('#gridClientes').jtable('reload');
            } else {
                alert(data.Message);
            }
        });
    }
}