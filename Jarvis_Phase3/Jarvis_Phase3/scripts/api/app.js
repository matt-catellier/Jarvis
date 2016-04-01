// TODO: Replace with the URL of your WebService app
var serviceUrl = 'http://jarvis.kpawa.com/api/Devices' // 'http://localhost:59235/api/Devices' //
function sendRequest() {
    $.ajax({
        type: 'get',
        url: serviceUrl
    }).done(function (data) {
        $('#devices').replaceWith("<ul id='devices' />")
        for (var i = 0; i < data.devices.length; i++) {
            callback(data.devices[i]);
        }

    }).error(function (jqXHR, textStatus, errorThrown) {
        $('#value').text(jqXHR.responseText || textStatus);
    });
}

function callback(val) {
    //  $("#manufacturers").replaceWith("<span id='value1'>(Result)</span>");
    $("#value").replaceWith("<ul id='devices' />");
    var str = "Category: " + val.category + " - Provider: " + val.provider;
    $('<li/>', { text: str }).appendTo($('#devices'));
}