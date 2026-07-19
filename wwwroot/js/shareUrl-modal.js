document.addEventListener('DOMContentLoaded', function () {

    const shareModal = document.getElementById('shareModal');

    shareModal.addEventListener('shown.bs.modal', function (event) {

        const button = event.relatedTarget;
        const tableurl = button.dataset.shareTableUrl;
        const gridurl = button.dataset.shareGridUrl;

        document.getElementById('shareTableUrlInput').value = tableurl;
        document.getElementById('shareGridUrlInput').value = gridurl;


        const copyButton = document.getElementById('copyShareUrlButton');

        if (copyButton) {
            copyButton.focus();
        }
    });

    document.getElementById('copyTableShareUrlButton')
        .addEventListener('click', async function () {

            await navigator.clipboard.writeText(
                document.getElementById('shareTableUrlInput').value
            );

            this.innerText = "Copied!";
        });

    document.getElementById('copyGridShareUrlButton')
        .addEventListener('click', async function () {

            await navigator.clipboard.writeText(
                document.getElementById('shareGridUrlInput').value
            );

            this.innerText = "Copied!";
        });
});