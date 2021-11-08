var lastSelectedProject = null;
var projectLoader = null;
var contentOwner = null;

// ajax calls

function ajaxSaveProject() {
    $("#savemessage").text("");

    if (!validateCheckName()) return false;

    if (!validateCheckCompany()) return false;

    if (!validateCheckDates()) return false;

    var projectId = 0;
    if (lastSelectedProject != null)
        projectId = lastSelectedProject.attr('data-val-id');

    try {
        $.ajax({
            type: 'POST',
            url: projectSavePath,
            data: {
                "Name": $("#Name").val(),
                "CompanyID": $("#CompanyID").val(),
                "Link": $("#Link").val(),
                "StartDate": $("#StartDate").val(),
                "EndDate": $("#EndDate").val(),
                "ProjectID": projectId
            },
            success: onSaveProject
        });
    }
    catch (err) {
    }

    return true;
}

function ajaxLoadProjectDetails() {
    var projectId = lastSelectedProject.attr('data-val-id');

    $("div#projectdetails").empty();
    $("div#projectdetails").hide();
    $("div#projectdetails").load(getProjectInfoPath + projectId);
    $("div#projectdetails").show();

    contentOwner = projectId;
    ajaxLoadContents();

    unblockUI(".paneltbl");
}

function ajaxDeleteProject() {
    var projectId = lastSelectedProject.attr('data-val-id');
    blockUI("table.elementstbl");
    try {
        $.ajax({
            type: 'POST',
            url: deleteProjectPath,
            data: { "projectId": projectId },
            success: onDeleteProject,
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

function loadProjectInfo() {
    if (projectLoader != null)
        clearTimeout(projectLoader);
    projectLoader = null;
    blockUI(".paneltbl");
    projectLoader = setTimeout(function () {
        if (lastSelectedProject == null) return;
        projectLoader = null;
        ajaxLoadProjectDetails();
        return null;
    }, 1000);
}

function printProjectInfo(data) {
    if (data == "FAILURE") {
        $("#projectdetailseditor").hide();
        $("#projectdetails").hide();
        $("#projectInfoError").show();
        return false;
    }
    var projectInfo = data["ProjectInfo"];

    // viewer
    $("#projectNameCell").text(projectInfo["Name"]);
    $("#projectCompanyCell").text(projectInfo["Company"]);
    $("#projectLinkCell").attr("href", projectInfo["Link"]);
    var parsedDate = new Date(+projectInfo["StartDate"].replace(/\/Date\((\d+)\)\//, '$1'));
    $("#projectStartYearCell").text(parsedDate.toString("yyyy MMMM"));
    if (projectInfo["EndDate"] != null) {
        parsedDate = new Date(+projectInfo["EndDate"].replace(/\/Date\((\d+)\)\//, '$1'));
        $("#projectEndYearCell").text(parsedDate.toString("yyyy MMMM"));
    }
    else {
        $("#projectEndYearCell").text('still working');
    }

    if (data["IsAdministrator"] == true) {
        $("#projectdetailseditor").show();
    }
    else {
        $("#projectdetails").show();
    }

    ajaxLoadContents();
    return true;
}

function onSaveProject(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("Project info is saved successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function onDeleteProject(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("Project info is deleted successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

function newProject() {
    lastSelectedProject = null;
    contentOwner = null;
    $("#projectdetails").load(newProjectInfoPartialView);
    $("#projectdetails").show();
    $("div#contents").hide();
}

function addProjectContent() {
    if (lastSelectedProject == null)
        return false;

    var projectId = lastSelectedProject.attr('data-val-id');
    contentOwner = projectId;
    blockUI(".paneltbl");

    try {
        $.ajax({
            type: 'POST',
            url: newContentPath,
            data: {
                "ownerId": projectId,
            },
            success: onAddProjectContent,
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

function onAddProjectContent(data) {
    if (data != null) {
        if (!data.startsWith("SUCCESS"))
            $("#savemessage").text(data);
        else {
            $("#savemessage").text("New Content appended to project information successfully. In seconds you will be redirected to index.");
            setTimeout("gotoIndex()", 3000);
        }
    }
}

// validators

function validateCheckName() {
    var len = $("#Name").val().length;
    if (len == 0) $("#ProjectNameEmpty").show();
    else $("#ProjectNameEmpty").hide();
    return len != 0;
}

function validateCheckCompany() {
    if ($("#CompanyID") == null)
        return true;
    var len = $("#CompanyID").val().length;
    if (len == 0) $("#CompanyEmpty").show();
    else $("#CompanyEmpty").hide();
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

function deleteSelectedProject() {
    if (InAJAX) return;

    $("#ConfirmDeleteProjectDialog").dialog('open');
}


