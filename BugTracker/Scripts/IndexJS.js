
$(function () {

    function AnchorAjax(e) {
        // grab the anchor clicked
        a = $(this);

        // check that there is an href, else exit
        ahref = a.attr("href");
        if (ahref.length < 5) {
            return false;
        }

        // replace the table.
        var target = $("#ticket-table");

        target.fadeToggle(250);
        target.load(ahref, function () {
            target.fadeToggle(250);
        });

        // stop default action and propogation
        e.preventDefault();
        e.stopPropogation();
    }

    // capture anchors on table to return as partial instead
    $("#ticket-table").on("click", "a", AnchorAjax);

});