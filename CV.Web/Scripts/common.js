var InAJAX = false;

function blockUI(ele) {
    $(ele).css('opacity', 0.3);
    $.blockUI({ message: $("#loading") });
}

function unblockUI(ele) {
    setTimeout(function () {
        $(ele).css('opacity', 1);
        $.unblockUI();
    }, 1000);
}

function jqueryPreventNonNumeric(e) {
    var a = [];
    var k = e.which;

    if (k == 0 || k == 8) return;

    for (i = 48; i < 58; i++)
        a.push(i);

    if (!($.inArray(k, a) >= 0))
        e.preventDefault();
}

function getDateFromYearMonthFormat(yearMonth) {
    var months = [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December'
    ];

    var value = yearMonth.split(" ");
    var month = months.indexOf(value[1]);
    return new Date(value[0], month, 1);
}
