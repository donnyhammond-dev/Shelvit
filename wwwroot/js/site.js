document.addEventListener("DOMContentLoaded", function () {

    // Only run on smaller screens
    if (window.innerWidth < 768) {

        // Check if user already chose a view
        const hasViewPreference = new URLSearchParams(window.location.search)
            .has("IsGridMode");

        if (!hasViewPreference) {

            window.location.href =
                window.location.pathname + "?IsGridMode=true";

                window.location.href = window.location.pathname + "?"
        }
    }
});