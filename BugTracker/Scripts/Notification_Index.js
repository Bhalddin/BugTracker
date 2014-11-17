
$(function () {

    function Error() {
        alert("Could not update notification at this time.");
    }

    function MarkAsRead() {

        // get checkbox clicked;
        $box = $(this);

        // perform get, include notification's id.
        $.get("/Notifications/UpdateReadStatus", { id: $box.attr("data-id") })
            .done(function () {
                $box.prop("disabled", "disabled");
            })
            .error(Error);

        // prevent default
        return false;
    }

    // hook up the function to the on change event of the checkbox
    $(":checkbox").change(MarkAsRead);
});