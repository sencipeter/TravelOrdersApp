// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById("employeeSelect").addEventListener("change", function () {
    var selectedId = this.value;
    if (selectedId) {
        // Reload page with ?employeeId=XYZ
        window.location.href = window.location.pathname + "?employeeId=" + encodeURIComponent(selectedId);
    }
});