// Write your JavaScript code.
$(document).ready(function () {
    $('form').on('submit', function () {
        if ($(this).valid()) {
            $('#loadingOverlay').removeClass('d-none');
        }
    });
});
