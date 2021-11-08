var lastSelectedContent = null;

// ajax calls

function ajaxShiftContentUp(contentId) {
    blockUI(".paneltbl");
    try {
        $.ajax({
            type: 'POST',
            url: upContentPath,
            data: {
                "contentId": contentId,
            },
            success: function (data) {
                onShiftContent(data, contentId, 'up');
            },
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
        alert('Error on shifting content up: ' + err);
    }
}

function ajaxShiftContentDown(contentId) {
    blockUI(".paneltbl");
    try {
        $.ajax({
            type: 'POST',
            url: downContentPath,
            data: {
                "contentId": contentId,
            },
            success: function (data) {
                onShiftContent(data, contentId, 'down');
            },
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
        alert('Error on shifting content down: ' + err);
    }
}

function ajaxLoadContents() {
    try {
        $("div#contents").empty();
        $("div#contents").hide();
        $("div#contents").load(getContentsPath + contentOwner);
        $("div#contents").show();
    }
    catch (err) {
        alert("Error loading contents: " + err);
    }
}

function ajaxDeleteContent() {
    blockUI(".paneltbl");
    try {
        $.ajax({
            type: 'POST',
            url: deleteContentPath,
            data: {
                "contentId": lastSelectedContent,
            },
            success: function (data) {
                onDeleteContent(data, lastSelectedContent);
            },
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
        alert('Error on deleting content: ' + err);
    }
}

function ajaxSaveContent(contentId) {
    $("#ContentEmpty_" + contentId).hide();

    if (!validateCheckContent(contentId))
        return false;

    blockUI(".paneltbl");
    var ownerId = $("#OwnerID_" + contentId).val();
    var rank = $("#Rank_" + contentId).val();
    var header = $("#Header_" + contentId).val();
    var content = $("#Content_" + contentId).val();

    try {
        $.ajax({
            type: 'POST',
            url: saveContentPath,
            data:
                {
                    "ContentID": contentId,
                    "Rank": rank,
                    "Header": header,
                    "Content" : content,
                    "OwnerId": ownerId
                },
            success: function (data) {
                onSaveContent(data, contentId);
            },
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
    }
}

// helper functions

function gotoIndex() {
    window.location.reload(true);
}

function onSaveContent(data, contentId) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage_" + contentId).text(data);
        else {
            $("#savemessage_" + contentId).text("Content is saved successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function deleteSelectedContent(contentId) {
    if (InAJAX) return;

    lastSelectedContent = contentId;

    $("#ConfirmDeleteContentDialog").dialog('open');
}

function onDeleteContent(data, contentId) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage_" + contentId).text(data);
        else {
            $("#savemessage_" + contentId).text("Content is deleted successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function onShiftContent(data, contentId, direction) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage_" + contentId).text(data);
        else {
            $("#savemessage_" + contentId).text("Content is shifted " + direction + " successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

// validate 

function validateCheckContent(contentId) {

    var fieldSet = $("fieldset.editor[data-val-id='" + contentId + "']")[0];
    var contentArea = fieldSet.getElementsByTagName("textarea")[0];
    var value = $.trim(contentArea.value);

    if (value == '') {
        $("#ContentEmpty_" + contentId).show();
        return false;
    }
    return true;
}
