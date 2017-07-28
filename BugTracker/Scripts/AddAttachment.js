
$(function () {

    // var to hold attachment count.
    var attachmentCount = 1;

    // get another attachment form when they want to add an attachment
    $("form").on("click", "#attachButton", GetAnotherForm);


    function generateForm() {
        // get new form from template
        var newForm = $($("#attachTemplate").html());

        // set the names of the inputs
        newForm
            .find("input")
            .attr("name", "attachment" + attachmentCount);

        // customize the labels
        var labels = newForm.find("label");
        labels.attr("for", "attachment" + attachmentCount);
        labels[0].innerHTML = "File Attachment #" + attachmentCount;
        labels[1].innerHTML = "File #" + attachmentCount + " Description";


        // increment and return
        attachmentCount++;
        return newForm;
    }

    function GetAnotherForm(e) {

        // get the latest div element with an id. It will be the container for our new incoming partial.
        var button = $(this);

        // load the new form.
        // note: replacing the button with the template works b/c there is also a button IN the template too that becomes the new button.
        button.replaceWith(generateForm());

        e.preventDefault();
        e.stopPropagation();
    }
});