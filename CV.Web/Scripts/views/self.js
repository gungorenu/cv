function addContent() {
    blockUI(".paneltbl");

    try {
        $.ajax({
            type: 'POST',
            url: newContentPath,
            data: {
                "ownerId": contentOwner,
            },
            success: onAddContent,
            beforeSend: function (xhr, settings) {
                InAJAX = true;
            },
            complete: function (xhr, textStatus) {
                InAJAX = false;
                unblockUI(".paneltbl");
            }
        });
    }
    catch (err) {
        alert('Error on new content adding: ' + err);
    }
}

function onAddContent(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("New Content appended to self information successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}