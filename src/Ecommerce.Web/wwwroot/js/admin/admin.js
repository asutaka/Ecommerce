// Custom Admin JavaScript for AdminLTE 3

$(document).ready(function () {
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-toggle="popover"]').popover();

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Confirm delete actions
    $('.btn-delete').on('click', function (e) {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) {
            e.preventDefault();
            return false;
        }
    });

    // Add active class to current menu item
    var currentPath = window.location.pathname;
    $('.nav-sidebar a').each(function () {
        var href = $(this).attr('href');
        if (currentPath.indexOf(href) !== -1 && href !== '/Admin/Dashboard') {
            $(this).addClass('active');
            $(this).parents('.nav-item').addClass('menu-open');
        }
    });
});

// Show loading overlay
function showLoading() {
    var overlay = $('<div class="loading-overlay"><div class="loading-spinner"></div></div>');
    $('body').append(overlay);
}

// Hide loading overlay
function hideLoading() {
    $('.loading-overlay').remove();
}

// Display toast notification
function showToast(message, type = 'success') {
    $(document).Toasts('create', {
        class: 'bg-' + type,
        title: type === 'success' ? 'Thành công' : 'Thông báo',
        body: message,
        autohide: true,
        delay: 3000,
    });
}

// AJAX form submission helper
function submitFormAjax(formId, successCallback) {
    showLoading();

    var form = $('#' + formId);
    var formData = new FormData(form[0]);

    $.ajax({
        url: form.attr('action'),
        type: form.attr('method'),
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            hideLoading();
            if (successCallback) {
                successCallback(response);
            }
            showToast('Thao tác thành công!', 'success');
        },
        error: function (xhr) {
            hideLoading();
            var errorMessage = xhr.responseJSON?.message || 'Có lỗi xảy ra!';
            showToast(errorMessage, 'danger');
        }
    });
}

// Format currency Vietnam
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Format date
function formatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
}
