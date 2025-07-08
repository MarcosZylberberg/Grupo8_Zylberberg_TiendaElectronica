// wwwroot/js/site.js
$(document).ready(function () {

    // ===== 1) Registro Modal =====
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
        if (!tipo) {
            alert('Seleccione Cliente o Usuario');
            return;
        }
        if (tipo === 'Cliente') {
            window.location.href = $(this).data('clientUrl');
        } else {
            var code = $('#adminCode').val();
            if (!code) {
                alert('Ingrese el código de administrador');
                return;
            }
            window.location.href = $(this).data('adminUrl') +
                '?adminCode=' + encodeURIComponent(code);
        }
    });


    // ===== 2) Login Modal =====
    var $loginModal = $('#LoginModal');
    if ($loginModal.find('.alert-danger').length) {
        new bootstrap.Modal($loginModal[0]).show();
    }
    $loginModal.on('hidden.bs.modal', function () {
        $(this).find('.alert-danger').remove();
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
    });


    // ===== 3) Paginación de productos =====
    (function () {
        var perPage = 20;
        var $items = $('.product-item');
        var $pagination = $('#pagination');
        var totalPages = Math.ceil($items.length / perPage);

        function showPage(page) {
            var start = (page - 1) * perPage, end = start + perPage;
            $items.each(function (i) {
                $(this).toggle(i >= start && i < end);
            });
            $pagination
                .find('li').removeClass('active')
                .filter('[data-page="' + page + '"]').addClass('active');
        }

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

        if ($items.length) showPage(1);
    })();


    // ===== 4) Confirmación de eliminación =====
    (function () {
        var $formToDelete = null;
        $('.btn-delete').click(function () {
            $formToDelete = $(this).closest('form.delete-form');
            new bootstrap.Modal($('#confirmDeleteModal')).show();
        });
        $('#confirmDeleteBtn').click(function () {
            if ($formToDelete) $formToDelete.submit();
        });
    })();


    $(function () {
        // ———————————————
        // 1) Formateador de moneda único
        // ———————————————
        const currencyFormatter = new Intl.NumberFormat(undefined, {
            style: 'currency',
            currency: 'ARS'
        });

        // ———————————————
        // 2) Función de recálculo genérica
        // ———————————————
        function recalcCard($card) {
            if ($card.hasClass('completado')) return; // no tocar completados

            let total = 0;
            $card.find('tbody tr').each((_, tr) => {
                const $row = $(tr);
                const qty = parseInt($row.find('.cantidad-input').val(), 10) || 0;
                const unitPrice = parseFloat($row.find('.cantidad-input').data('unit-price')) || 0;
                const subtotal = qty * unitPrice;

                $row.find('.det-subtotal').text(
                    currencyFormatter.format(subtotal)
                );
                total += subtotal;
            });

            $card.find('.pedido-total').text(
                currencyFormatter.format(total)
            );
        }

        // ———————————————
        // 3) Recalcular al cargar (sólo no completados)
        // ———————————————
        $('.pedido-card').not('.completado').each((_, card) => {
            recalcCard($(card));
        });

        // ———————————————
        // 4) Delegar eventos de input/change
        //    sólo en tarjetas no completadas
        // ———————————————
        $(document).on('input change', '.pedido-card:not(.completado) .cantidad-input', function () {
            const $input = $(this);
            const $card = $input.closest('.pedido-card');

            // recálculo inmediato en cliente
            recalcCard($card);

            // guardar en servidor
            const nuevaCant = parseInt($input.val(), 10) || 0;
            const detalleId = $input.data('detalle-id');
            const token = $('input[name="__RequestVerificationToken"]').first().val();

            $.post('/Pedidos/UpdateQuantity', {
                detalleId,
                cantidad: nuevaCant,
                __RequestVerificationToken: token
            })
                .done(resp => {
                    if (resp.success) {
                        // opcional: podrías actualizar con resp.nuevoSubtotal/nuevoTotal...
                        // pero como recalcCard() usa qty y unit-price, basta con recalcCard:
                        recalcCard($card);
                    }
                })
                .fail((xhr, status, error) => {
                    console.error('UpdateQuantity error:', status, error, xhr.responseText);
                    alert('No se pudo guardar la cantidad en el servidor');
                });
        });

        // ———————————————
        // 5) Recalcular tras cualquier AJAX que reemplace la tarjeta
        // ———————————————
        $(document).on('ajaxComplete', '.pedido-card:not(.completado)', function () {
            recalcCard($(this));
        });
    });

    // ===== 7) Paginación de Mis Pedidos =====
    (function () {
        var $cards = $('.pedido-card').parent(),
            perPage2 = 20,
            totalPages2 = Math.ceil($cards.length / perPage2),
            $pagi2 = $('#pedidoPagination');

        function showPedidoPage(page) {
            var start = (page - 1) * perPage2,
                end = start + perPage2;
            $cards.each(function (i) {
                $(this).toggle(i >= start && i < end);
            });
            $pagi2
                .find('li').removeClass('active')
                .filter('[data-page="' + page + '"]').addClass('active');
        }

        for (let i = 1; i <= totalPages2; i++) {
            var $li2 = $('<li/>', {
                'class': 'page-item' + (i === 1 ? ' active' : ''),
                'data-page': i
            });
            $('<a/>', {
                'class': 'page-link',
                href: '#',
                text: i
            })
                .appendTo($li2)
                .click(function (e) {
                    e.preventDefault();
                    showPedidoPage(i);
                });
            $pagi2.append($li2);
        }
        if ($cards.length) showPedidoPage(1);
    })();

});