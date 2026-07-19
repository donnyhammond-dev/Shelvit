var imageModal = document.getElementById('imageModal');

imageModal.addEventListener('show.bs.modal', function (event) {

    var button = event.relatedTarget;

    var id = button.dataset.id;
    var image = button.dataset.image;
    const pageNumber = button.dataset.pageNumber;

    document.getElementById('imageGameId').value = id;
    document.getElementById('imageUrlInput').value = image || "";
    document.getElementById('imageUpdatePageNumber').value = pageNumber;
});