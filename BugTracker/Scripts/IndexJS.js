
$(function () {

    function FilterColumns() {

        // get selector clicked
        $.each( $("#col-selector input"), function (index, value) {
            var box = $(value);
            if ( box.prop("checked") ) {
                $( box.val() ).show();
            }
            else {
                $( box.val() ).hide();
            }
        });

    }

    function AnchorAjax(e) {
        // grab the anchor clicked
        a = $(this);

        // there are some links that we don't want to capture, so we will give those links a data-ignore attribute
        if (a.hasClass("ignore")) {
            return true;
        }

        // if there isn't an href, then clicked on one of the placeholder a's so we should just quit.
        ahref = a.attr("href");
        if (!ahref) {
            return false;
        }

        // replace the table.
        var target = $("#ticket-table");

        target.fadeToggle(250);
        target.load(ahref, function () {
            // run function to persist selection of colums, and to not show the hidden colums initially.
            FilterColumns();
            // now show target.
            target.fadeToggle(250);
        });

        // stop default action and propogation
        e.preventDefault();
        e.stopPropagation();
    }

    // capture anchors on table to return as partial instead
    $("#ticket-table").on("click", "a", AnchorAjax);

    // react to clicks in the col-selector AND porform the initial filtering.
    $("#col-selector").on("click", "input", FilterColumns);
    FilterColumns();
});