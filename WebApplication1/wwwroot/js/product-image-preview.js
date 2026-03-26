// Used by both Product and Admin Product update pages.
// It previews the selected image file before submitting the form.
(function () {
    function initPreview() {
        var input = document.getElementById("imageInput");
        var img = document.getElementById("previewImage");
        if (!input || !img) return;

        input.addEventListener("change", function (event) {
            var files = event.target.files;
            if (!files || files.length === 0) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                img.src = e.target.result;
            };
            reader.readAsDataURL(files[0]);
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initPreview);
    } else {
        initPreview();
    }
})();

