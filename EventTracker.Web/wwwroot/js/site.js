$(document).ready(function () {

    //Delete timeframes from edited event
    $(".btn-delete-timeframe").click(removeTimeframe);

    //Add empty timeframe
    $("#addTimeframe").click(function () {
        $.get('/Events/TimeFrameEntry', function (template) {
            $("#timeframes").append(template);
            var button = $("#timeframes").children().last().children().find(".btn-delete-timeframe");
            button.click(removeTimeframe);
        });
    });

    //Toggle password visibility input
    $(".toggle-password").click(function () {
        $(this).children().toggleClass("fa-eye fa-eye-slash");
        var input = $(this).parent().siblings(".password");
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

    //Autofade alerts
    $(".alert-dismissible").fadeTo(3000, 500).slideUp(500, function () {
        $(".alert-dismissible").alert('close');
    });

    /*
    ####################################################
    HELPER FUNCTIONS
    ####################################################
    */
    function removeTimeframe() {
        if (document.getElementById("timeframes").getElementsByTagName("li").length > 1) {
            $(this).parent().parent().parent().parent().remove()
        }
    }
});

//Maintain & keep scroll position after post - back & postback & refresh.
document.addEventListener('DOMContentLoaded', function () {
    var sep = '\uE000'; // an unusual char: unicode 'Private Use, First'

    window.addEventListener('pagehide', function (e) {
        window.name += sep + window.pageXOffset + sep + window.pageYOffset;
    });

    if (window.name && window.name.indexOf(sep) > -1) {
        var parts = window.name.split(sep);
        if (parts.length >= 3) {
            window.name = parts[0];
            window.scrollTo(parseFloat(parts[parts.length - 2]), parseFloat(parts[parts.length - 1]));
        }
    }
});