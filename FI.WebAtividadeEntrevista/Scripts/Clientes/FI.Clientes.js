var beneficiarios = [];
var lastId = 0;

$(document).ready(function () {
    $('#CPF').on('input', function (e) {
        cpfInputMask(this, cpf);
        $(this).attr('maxlength', '14');
    });

    $('#CPF').on('paste', function (e) {
        var clipboardData = e.originalEvent.clipboardData || window.clipboardData;
        var pastedData = clipboardData.getData('text');
        if (pastedData.length > 14) {
            $(this).val(pastedData.substring(0, 14));
        }
    });

    $('#BeneficiarioCPF').on('input', function (e) {
        cpfInputMask(this, cpf);
        $(this).attr('maxlength', '14');
    });

    $('#BeneficiarioCPF').on('paste', function (e) {
        var clipboardData = e.originalEvent.clipboardData || window.clipboardData;
        var pastedData = clipboardData.getData('text');
        if (pastedData.length > 14) {
            $(this).val(pastedData.substring(0, 14));
        }
    });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        let cpf = $(this).find("#CPF").val().replace(/\D/g, '');
        if (!validateCPF(cpf)) {
            ModalDialog("Operação Inválida", "O CPF informado é inválido.");
            return;
        }

        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "CPF": cpf,
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "beneficiariosJson": JSON.stringify(beneficiarios)
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status = 409)
                        ModalDialog("Operação Inválida", "Já existe um cliente cadastrado com o CPF informado.");
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    $("#formCadastro")[0].reset();
                    beneficiarios = [];
                    $('#tabelaBeneficiarios tbody').empty();
                }
        });
    })

    $('#beneficiarioForm').submit(function (event) {
        event.preventDefault();

        let cpfSemMascara = $(this).find("#BeneficiarioCPF").val().replace(/\D/g, '');
        if (!validateCPF(cpfSemMascara)) {
            $('#alertMessage').text("O CPF informado é inválido.");
            return;
        }

        var formData = {
            CPF: cpfMask(cpfSemMascara),
            Nome: $('#BeneficiarioNome').val()
        };
        var cpfDuplicado = beneficiarios.some(function (beneficiario) {
            return beneficiario.CPF == formData.CPF && beneficiario.Id != $('#BeneficiarioAlterando').val();
        });

        if (cpfDuplicado) {
            $('#alertMessage').text('Já existe um beneficiário com esse CPF para este cliente.');
            return;
        }
        var index = beneficiarios.findIndex(function (beneficiario) {
            return beneficiario.Id == $('#BeneficiarioAlterando').val();
        });

        if (index !== -1) {
            var beneficiarioParaEditar = beneficiarios[index];

            $("#btnAction").html('Incluir');
            $("#btnAction").removeClass('btn-warning').addClass('btn-success');

            beneficiarioParaEditar.CPF = formData.CPF;
            beneficiarioParaEditar.Nome = formData.Nome;

            $('#tabelaBeneficiarios #' + beneficiarioParaEditar.Id).find('td:nth-child(1)').text(beneficiarioParaEditar.CPF);
            $('#tabelaBeneficiarios #' + beneficiarioParaEditar.Id).find('td:nth-child(2)').text(beneficiarioParaEditar.Nome);

            beneficiarioParaEditar.Action = beneficiarioParaEditar.Action === "Insert" ? "Insert" : "Update";
        } else {
            $("#btnAction").html('Incluir');
            $("#btnAction").removeClass('btn-warning').addClass('btn-success');

            formData.Id = getNewId();
            formData.Action = "Register";
            beneficiarios.push(formData);

            var newRow = '<tr id="' + formData.Id + '">                                                                                                                                                                          ' +
                '             <td>' + formData.CPF + '</td>                                                                                                                                                                      ' +
                '             <td>' + formData.Nome + '</td>                                                                                                                                                                     ' +
                '             <td>                                                                                                                                                                                               ' +
                '                  <button type="button" class="btn btn-sm btn-primary" onclick="editarBeneficiario(this.value)" id="editBeneficiarioBtn" value="' + formData.Id + '">Alterar</button>                          ' +
                '                  <button type="button" class="btn btn-sm btn-primary" onclick="deletarBeneficiario(this.value)" id="delBeneficiarioBtn" value="' + formData.Id + '" style="margin-left: 10px">Excluir</button> ' +
                '             </td>                                                                                                                                                                                              ' +
                '         </tr>                                                                                                                                                                                                  ';

            $('#tabelaBeneficiarios tbody').append(newRow);
        }

        $('#BeneficiarioCPF').val('');
        $('#BeneficiarioNome').val('');
        $('#alertMessage').text('');
        $('#BeneficiarioAlterando').val('');
    });

    $("#btnShowBeneficiarios").click(function () {
        $('#beneficiariosModal').modal('show');
    });
});

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
