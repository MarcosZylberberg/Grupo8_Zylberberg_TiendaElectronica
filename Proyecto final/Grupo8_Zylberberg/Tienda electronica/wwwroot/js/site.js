// wwwroot/js/site.js

$(function () {
    // Al abrir el modal, resetear inputs
    $('#registerModal').on('show.bs.modal', function () {
        $('input[name="registerType"]').prop('checked', false);
        $('#adminCodeGroup').hide();
        $('#adminCode').val('');
    });

    // Mostrar/ocultar campo de código
    $('input[name="registerType"]').change(function () {
        if ($(this).val() === 'Usuario') {
            $('#adminCodeGroup').show();
        } else {
            $('#adminCodeGroup').hide();
        }
    });

    // Al hacer click en "Continuar"
    $('#registerContinueBtn').click(function () {
        var tipo = $('input[name="registerType"]:checked').val();
        if (!tipo) {
            alert('Seleccione Cliente o Usuario');
            return;
        }
        var btn = $(this);
        if (tipo === 'Cliente') {
            window.location.href = btn.data('clientUrl');
        } else {
            var code = $('#adminCode').val();
            if (!code) {
                alert('Ingrese el código de administrador');
                return;
            }
            var adminUrl = btn.data('adminUrl');
            window.location.href = adminUrl + '?adminCode=' + encodeURIComponent(code);
        }
    });
});