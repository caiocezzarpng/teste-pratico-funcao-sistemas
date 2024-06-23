
$(document).ready(function () {

    if (document.getElementById("gridClientes"))
        $('#gridClientes').jtable({
            title: 'Clientes',
            paging: true, 
            pageSize: 5, 
            sorting: true, 
            defaultSorting: 'Nome ASC', 
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
    if (confirm('Tem certeza que deseja excluir este cliente? Todos os beneficiários atrelados a ele tambem serão excluidos')) {
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