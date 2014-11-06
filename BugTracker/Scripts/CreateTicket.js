
$(function () {

    function GetAnotherForm (e) {

        // get the link from the anchor.
        var aHref = $(this).attr("href");

        // get the latest p element with an id.
        var p = $("p[id]").last();

        // load the new form.
        p.load(aHref);

        e.preventDefault();
        e.stopPropagation();
        return false;
    }

    // get another attachment form when they want to add an attachment
    $("form").on("click", "a", GetAnotherForm);

});