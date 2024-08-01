<script>
    document.addEventListener('DOMContentLoaded', function () {
    var toastElements = [].slice.call(document.querySelectorAll('.toast'));
    toastElements.forEach(function (toastEl) {
      var toast = new bootstrap.Toast(toastEl);
    toast.show();
    });
  });
</script>
