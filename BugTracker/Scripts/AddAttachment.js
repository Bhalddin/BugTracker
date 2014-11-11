
$(function () {

    // var to hold attachment count.
    var attachCount = 1;

    function generateForm() {
        // get new form from template
        var newForm = $( $("#attachTemplate").html() );

        // set the names of the inputs
        newForm.find("input").attr("name", "attachment" + attachCount);
        // customize the labels
        var labels = newForm.find("label");
        labels.attr("for", "attachment" + attachCount);
        labels[0].innerHTML = "File Attachment #" + attachCount;
        labels[1].innerHTML = "File #" + attachCount + " Description";


        // increment and return
        attachCount++;
        return newForm;
    }

    function GetAnotherForm (e) {

        // get the latest p element with an id. It will be the container for our new incoming partial.
        var button = $(this);

        // load the new form.
        button.replaceWith( generateForm() );

        e.preventDefault();
        e.stopPropagation();
    }

    // get another attachment form when they want to add an attachment
    $("form").on("click", "#attachButton", GetAnotherForm);

});