// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var dropZone = $('#dropZone');
    var fileInput = $('#fileInput');
    var uploadButton = $('#uploadButton');
    var alertContainer = $('#alertContainer');
    var isFileInputClicked = false;

    dropZone.on('click', function () {
        if (!isFileInputClicked) {
            isFileInputClicked = true;
            fileInput.click();
        }
    });

    fileInput.on('change', function () {
        isFileInputClicked = false; // Reset the flag
        if (fileInput[0].files.length > 0) {
            dropZone.find('p').text(fileInput[0].files[0].name);
            alertContainer.html(''); // Clear any existing alerts
        }
    });

    dropZone.on('dragover', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.addClass('dragover');
    });

    dropZone.on('dragleave', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.removeClass('dragover');
    });

    dropZone.on('drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
        dropZone.removeClass('dragover');
        var files = e.originalEvent.dataTransfer.files;
        if (files.length > 0) {
            fileInput[0].files = files;
            dropZone.find('p').text(files[0].name);
            alertContainer.html(''); // Clear any existing alerts
        }
    });

    $('#uploadForm').on('submit', function (e) {
        if (!fileInput.val()) {
            e.preventDefault();
            var alertHtml = `
                    <div class="alert alert-warning alert-dismissible fade show" role="alert">
                        <h3 class="alert-heading">Warning</h3>
                        <p>Please select a file before uploading.</p>
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>`;
            alertContainer.html(alertHtml);
            return false;
        }
        uploadButton.prop('disabled', true);
        uploadButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Uploading...');
    });
});