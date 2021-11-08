var lastSelectedCompany = null;
var companyLoader = null;
var contentOwner = null;

// ajax calls

function ajaxSaveCompany() {
    $("#savemessage").text("");

    if (!validateCheckName()) return false;

    if (!validateCheckPositions()) return false;

    if (!validateCheckDates()) return false;

    var companyId = 0;
    if (lastSelectedCompany != null)
        companyId = lastSelectedCompany.attr('data-val-id');

    try {
        $.ajax({
            type: 'POST',
            url: companySavePath,
            data: {
                "Name": $("#Name").val(),
                "Positions": $("#Positions").val(),
                "Link": $("#Link").val(),
                "StartDate": $("#StartDate").val(),
                "EndDate": $("#EndDate").val(),
                "CompanyID": companyId
            },
            success: onSaveCompany
        });
    }
    catch (err) {
    }

    return true;
}

function ajaxLoadCompanyDetails() {
    var companyId = lastSelectedCompany.attr('data-val-id');

    $("div#companydetails").empty();
    $("div#companydetails").hide();
    $("div#companydetails").load(getCompanyInfoPath + companyId);
    $("div#companydetails").show();

    contentOwner = companyId;
    ajaxLoadContents();

    unblockUI(".paneltbl");
}

function ajaxDeleteCompany() {
    var companyId = lastSelectedCompany.attr('data-val-id');
    blockUI("table.elementstbl");
    try {
        $.ajax({
            type: 'POST',
            url: deleteCompanyPath,
            data: { "companyId": companyId },
            success: onDeleteCompany,
            beforeSend: function (xhr, settings) {
                InAJAX = true;
            },
            complete: function (xhr, textStatus) {
                InAJAX = false;
                unblockUI("table.elementstbl");
            }
        });
    }
    catch (err) {
        alert('Error on deleting company: ' + err);
    }
}

// helper functions

function loadCompanyInfo() {
    if (companyLoader != null)
        clearTimeout(companyLoader);
    companyLoader = null;
    blockUI(".paneltbl");
    companyLoader = setTimeout(function () {
        if (lastSelectedCompany == null) return;
        companyLoader = null;
        ajaxLoadCompanyDetails();
        return null;
    }, 1000);
}

function printCompanyInfo(data) {
    if (data == "FAILURE") {
        $("#companydetailseditor").hide();
        $("#companydetails").hide();
        $("#companyInfoError").show();
        return false;
    }
    var companyInfo = data["CompanyInfo"];

    // viewer
    $("#companyNameCell").text(companyInfo["Name"]);
    $("#companyPositionsCell").text(companyInfo["Positions"]);
    $("#companyLinkCell").attr("href", companyInfo["Link"]);
    var parsedDate = new Date(+companyInfo["StartDate"].replace(/\/Date\((\d+)\)\//, '$1'));
    $("#companyStartYearCell").text(parsedDate.toString("yyyy MMMM"));
    if (companyInfo["EndDate"] != null) {
        parsedDate = new Date(+companyInfo["EndDate"].replace(/\/Date\((\d+)\)\//, '$1'));
        $("#companyEndYearCell").text(parsedDate.toString("yyyy MMMM"));
    }
    else {
        $("#companyEndYearCell").text('still working');
    }

    if (data["IsAdministrator"] == true) {
        $("#companydetailseditor").show();
    }
    else {
        $("#companydetails").show();
    }

    ajaxLoadContents();
    return true;
}

function onSaveCompany(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("Company info is saved successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function onDeleteCompany(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("Company info is deleted successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function newCompany() {
    lastSelectedCompany = null;
    contentOwner = null;
    $("#companydetails").load(newCompanyInfoPartialView);
    $("#companydetails").show();
    $("div#contents").hide();
}

function addCompanyContent() {
    if (lastSelectedCompany == null)
        return false;

    var companyId = lastSelectedCompany.attr('data-val-id');
    contentOwner = companyId;
    blockUI(".paneltbl");

    try {
        $.ajax({
            type: 'POST',
            url: newContentPath,
            data: {
                "ownerId": companyId,
            },
            success: onAddCompanyContent,
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

function onAddCompanyContent(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("New Content appended to company information successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

// validators

function validateCheckName() {
    var len = $("#Name").val().length;
    if (len == 0) $("#CompanyNameEmpty").show();
    else $("#CompanyNameEmpty").hide();
    return len != 0;
}

function validateCheckPositions() {
    var len = $("#Positions").val().length;
    if (len == 0) $("#PositionsEmpty").show();
    else $("#PositionsEmpty").hide();
    return len != 0;
}

function validateCheckDates() {
    if (!(validateCheckStartDate() && validateCheckEndDate()))
        return false;

    if ($("#EndDate").val() == '')
        return true;

    var date1 = getDateFromYearMonthFormat($("#StartDate").val());
    var date2 = getDateFromYearMonthFormat($("#EndDate").val());

    if (date1 > date2) {
        $("#savemessage").text("End date cannot be older than start date!");
        $("#savemessage").show();
    }

    return date1 < date2;
}

function validateCheckStartDate() {
    var len = $("#StartDate").val().length;
    $("#StartDateInvalid").hide();
    if (len == 0) $("#StartDateEmpty").show();
    else $("#StartDateEmpty").hide();
    if (len == 0) return false;

    try {
        var date = $("#StartDate").val();
        date = getDateFromYearMonthFormat(date);
        var now = new Date();

        var dateS = date.toString("yyyy.MM.dd");
        var nowS = now.toString("yyyy.MM.dd");

        if (nowS < dateS) {
            $("#StartDateInvalid").show();
            return false;
        }
        return true;
    }
    catch (err) {
        $("#StartDateInvalid").show();
        return false;
    }
}

function validateCheckEndDate() {
    var len = $("#EndDate").val().length;
    $("#EndDateInvalid").hide();
    if (len == 0) return true;

    try {
        var date = getDateFromYearMonthFormat($("#EndDate").val());
        var now = new Date();

        var dateS = date.toString("yyyy.MM.dd");
        var nowS = now.toString("yyyy.MM.dd");
        if (nowS < dateS) {
            $("#EndDateInvalid").show();
            return false;
        }
        return true;
    }
    catch (err) {
        $("#EndDateInvalid").show();
        return false;
    }
}

// dialog

function deleteSelectedCompany() {
    if (InAJAX) return;

    $("#ConfirmDeleteCompanyDialog").dialog('open');
}


