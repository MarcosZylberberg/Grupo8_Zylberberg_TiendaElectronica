$(function () {
    // ===== Registro Modal =====
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

    // ===== Login Modal =====
    var $loginModal = $('#LoginModal');

    // Si hay un error, lo abrimos
    if ($loginModal.find('.alert-danger').length > 0) {
        new bootstrap.Modal($loginModal.get(0)).show();
    }

    // Al cerrar el modal, limpiamos alerta, backdrop y clase modal-open
    $loginModal.on('hidden.bs.modal', function () {
        // 1) quitar el mensaje de error
        $(this).find('.alert-danger').remove();
        // 2) quitar el backdrop que queda “flotando”
        $('.modal-backdrop').remove();
        // 3) quitar la clase que inmoviliza el scroll del body
        $('body').removeClass('modal-open');
    });
});