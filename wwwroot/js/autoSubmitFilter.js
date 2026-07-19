document.querySelectorAll(".auto-submit").forEach(select => {
    select.addEventListener("change", function () {
        this.form.submit();
    });
});


document.querySelectorAll(".search-submit").forEach(input => {

    let submitted = false;

    input.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            submitted = true;
            this.form.submit();
        }
    });

    input.addEventListener("blur", function () {
        if (!submitted) {
            this.form.submit();
        }
        submitted = false;
    });

});