var inputs = new Array(false, false);

$(function () {
    $("#UserName").bind('input', function () { CheckUserName(); CheckSubmit(); });
    $("#Password").bind('input', function () { CheckPassword(); CheckSubmit(); });
    CheckUserName();
    CheckPassword();
});

function CheckUserName() {
    var userNameLength = $("#UserName").val().length;
    inputs[0] = (userNameLength != 0);
    if (userNameLength == 0)
        $("#UserNameEmpty").show();
    else
        $("#UserNameEmpty").hide();
}

function CheckPassword() {
    var passwordLength = $("#Password").val().length;
    inputs[1] = (passwordLength != 0);
    if (passwordLength == 0)
        $("#PasswordEmpty").show();
    else
        $("#PasswordEmpty").hide();
}

function CheckSubmit() {
    for (i = 0; i < inputs.length; i++) {
        if (!inputs[i]) {
            $("#Logon").attr('disabled', 'disabled');
            $("#Logon").attr('class', 'button-disabled')
            return;
        }
    }
    $("#Logon").removeAttr('disabled');
    $("#Logon").attr('class', 'button');
}