<script>
    // Remove this JavaScript block if you're using Blazor components for toasts
    function showToasts() {
        var toastElements = [].slice.call(document.querySelectorAll('.toast'));
    toastElements.forEach(function (toastEl) {
            var toast = new bootstrap.Toast(toastEl);
    toast.show();
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        showToasts();
    });
</script>
