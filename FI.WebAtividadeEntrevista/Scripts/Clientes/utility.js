function cpfMask(cpf) {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
}
function cpfInputMask(o, f) {
    v_obj = o;
    v_fun = f;
    setTimeout('initCpfInputMask()', 50);
}

function initCpfInputMask() {
    v_obj.value = v_fun(v_obj.value);
}

function cpf(value) {
    value = value.replace(/\D/g, "");

    if (value.length <= 11) {
        value = value.replace(/(\d{3})(\d)/, "$1.$2");
        value = value.replace(/(\d{3})(\d)/, "$1.$2");
        value = value.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    }

    return value;
}

function validateCPF(value) {
    value = value.replace(/\D/g, "");

    if (value.length !== 11) {
        return false;
    }

    if (/^(\d)\1{10}$/.test(value)) {
        return false;
    }

    let sum = 0;
    let remainder;

    for (let i = 1; i <= 9; i++) {
        sum += parseInt(value[i - 1], 10) * (11 - i);
    }
    remainder = (sum * 10) % 11;

    if (remainder === 10 || remainder === 11) {
        remainder = 0;
    }
    if (remainder !== parseInt(value.charAt(9), 10)) {
        return false;
    }

    sum = 0;
    for (let i = 1; i <= 10; i++) {
        sum += parseInt(value[i - 1], 10) * (12 - i);
    }
    remainder = (sum * 10) % 11;

    if (remainder === 10 || remainder === 11) {
        remainder = 0;
    }

    return remainder === parseInt(value.charAt(10), 10);
}


function getNewId() {
    return ++lastId;
}

function editarBeneficiario(id) {
    var beneficiarioClicado = beneficiarios.find(x => x.Id == id);

    $('#BeneficiarioCPF').val(beneficiarioClicado.CPF);
    $('#BeneficiarioNome').val(beneficiarioClicado.Nome);
    $('#BeneficiarioAlterando').val(beneficiarioClicado.Id);

    $("#btnAction").html('Alterar');
    $("#btnAction").removeClass('btn-success').addClass('btn-warning');
}

function deletarBeneficiario(id) {
    $('#tabelaBeneficiarios #' + id).remove();

    var index = beneficiarios.findIndex(function (beneficiario) {
        return beneficiario.Id == id;
    });

    if (index !== -1) {
        var beneficiarioParaRemover = beneficiarios[index];

        if (beneficiarioParaRemover.Action === "Register") {
            beneficiarios.splice(index, 1);
        } else {
            beneficiarioParaRemover.Action = "Remove";
        }
    }
}