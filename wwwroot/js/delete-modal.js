var deleteModal = document.getElementById('deleteModal');

deleteModal.addEventListener('show.bs.modal', function (event) {

	var button = event.relatedTarget;

	var id = button.getAttribute('data-id');
	var title = button.getAttribute('data-title');
	var pageNumber = button.getAttribute('data-pageNumber');


	document.getElementById('deleteId').value = id;
	document.getElementById('deletePageNumber').value = pageNumber;
	document.getElementById('itemTitle').textContent = title;
});