// wwwroot/js/site.js
$(function () {
    // ===== Registro Modal =====
    $('#registerModal').on('show.bs.modal', function () {
        $('input[name="registerType"]').prop('checked', false);
        $('#adminCodeGroup').hide();
        $('#adminCode').val('');
    });

    $('input[name="registerType"]').change(function () {
        $('#adminCodeGroup').toggle(this.value === 'Usuario');
    });

    $('#registerContinueBtn').click(function () {
        var tipo = $('input[name="registerType"]:checked').val();
        if (!tipo) { return alert('Seleccione Cliente o Usuario'); }
        if (tipo === 'Cliente') {
            window.location.href = $(this).data('clientUrl');
        } else {
            var code = $('#adminCode').val();
            if (!code) { return alert('Ingrese el código de administrador'); }
            window.location.href = $(this).data('adminUrl') + '?adminCode=' + encodeURIComponent(code);
        }
    });

    // ===== Login Modal =====
    var $loginModal = $('#LoginModal');
    if ($loginModal.find('.alert-danger').length) {
        new bootstrap.Modal($loginModal[0]).show();
    }
    $loginModal.on('hidden.bs.modal', function () {
        $(this).find('.alert-danger').remove();
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
    });

    // ===== Paginación de productos =====
    var perPage = 20;
    var $items = $('.product-item');
    var $pagination = $('#pagination');
    var totalPages = Math.ceil($items.length / perPage);

    function showPage(page) {
        var start = (page - 1) * perPage;
        var end = start + perPage;
        $items.each(function (i) {
            $(this).toggle(i >= start && i < end);
        });
        $pagination.find('li').removeClass('active')
            .filter('[data-page="' + page + '"]').addClass('active');
    }

    // Construir controles de paginación
    for (var i = 1; i <= totalPages; i++) {
        var $li = $('<li/>', {
            'class': 'page-item' + (i === 1 ? ' active' : ''),
            'data-page': i
        });
        $('<a/>', {
            'class': 'page-link',
            href: '#',
            text: i,
            click: (function (p) {
                return function (e) {
                    e.preventDefault();
                    showPage(p);
                };
            })(i)
        }).appendTo($li);
        $pagination.append($li);
    }

    // Mostrar primera página
    if ($items.length) showPage(1);

    $(function () {
        var $formToDelete = null;

        // Cuando clicas en cualquier .btn-delete
        $('.btn-delete').click(function () {
            // Guardamos el form padre
            $formToDelete = $(this).closest('form.delete-form');
            // Abrimos el modal
            var modal = new bootstrap.Modal($('#confirmDeleteModal'));
            modal.show();
        });

        // Si confirmas en el modal, enviamos el form
        $('#confirmDeleteBtn').click(function () {
            if ($formToDelete) {
                $formToDelete.submit();
            }
        });
    });

    $(document).ready(function () {

        // ————————————————
        // 1) RECONTEO DE SUBTOTAL / TOTAL
        // ————————————————
        function recalcCard(wrapper) {
            let total = 0;
            // buscamos la tabla dentro de esta carta
            const $rows = $(wrapper).find('tbody tr');
            $rows.each(function () {
                const $row = $(this);
                const qty = parseInt($row.find('.cantidad-input').val()) || 0;
                const unitPrice = parseFloat($row.find('.cantidad-input').data('unit-price')) || 0;
                const subtotal = qty * unitPrice;

                // actualizo el cell
                $row.find('.det-subtotal')
                    .text(subtotal.toLocaleString(undefined, { style: 'currency', currency: 'ARS' }));
                total += subtotal;
            });

            // actualizo total en el footer
            $(wrapper).find('.pedido-total')
                .text(total.toLocaleString(undefined, { style: 'currency', currency: 'ARS' }));
        }

        // Enlazamos el evento input en cada cantidad-input y disparamos recalc al inicio
        $('.pedido-card').each(function () {
            const wrapper = this;
            $(wrapper).find('.cantidad-input').on('input', () => recalcCard(wrapper));
            recalcCard(wrapper);
        });

        // ————————————————
        // 2) PAGINACIÓN DE PEDIDOS
        // ————————————————
        const $cards = $('.pedido-card').parent(); // wrapper .col-*
        const perPage = 20;
        const totalPages = Math.ceil($cards.length / perPage);
        const $pagination = $('#pedidoPagination');

        function showPage(page) {
            const start = (page - 1) * perPage;
            const end = start + perPage;
            $cards.each(function (i) {
                $(this).toggle(i >= start && i < end);
            });
            $pagination.find('li').removeClass('active')
                .filter(`[data-page="${page}"]`).addClass('active');
        }

        // crear controles
        for (let i = 1; i <= totalPages; i++) {
            const $li = $(`<li class="page-item${i === 1 ? ' active' : ''}" data-page="${i}"></li>`);
            $('<a class="page-link" href="#">')
                .text(i)
                .on('click', e => {
                    e.preventDefault();
                    showPage(i);
                })
                .appendTo($li);
            $pagination.append($li);
        }
        if ($cards.length) showPage(1);
    })
});