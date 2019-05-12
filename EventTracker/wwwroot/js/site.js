$(document).ready(function () {

    //Delete timeframes from edited event
    $(".btn-delete-timeframe").click(removeTimeframe);

    //Add empty timeframes when adding event
    $("#addTimeframe").click(function (e) {
        e.preventDefault();

        //Set counter for dynamical generation of ID attributes for modelbinding
        var i = $(".timeframes").length;

        //Html element to be added
        var newTimeframe = `
            <div class="timeframes form-row align-items-center" id="Timeframe_` + i + `">
                <div class="col-sm-auto col-md-4 col-lg-2">
                    <label for= "Timeframes_` + i + `__EventDate" > Event Date</label >
                    <input class="form-control" type="date" data-val="true" data-val-required="The Event Date field is required." id="Timeframes_` + i + `__EventDate" name="Timeframes[` + i + `].EventDate" />
                </div>
                <div class="col-sm-auto col-md-3 col-lg-2">
                    <label for="Timeframes_` + i + `__Starttime">Start</label>
                    <input class="form-control" min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The Start field is required." id="Timeframes_` + i + `__Starttime" name="Timeframes[` + i + `].Starttime" value="0" />
                </div>
                <div class="col-sm-auto col-md-3 col-lg-2">
                    <label for="Timeframes_` + i + `__Endtime">End</label>
                    <input class="form-control" min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The End field is required." id="Timeframes_` + i + `__Endtime" name="Timeframes[` + i + `].Endtime" value="0" />
                    
                </div>
                <div class="col-sm-auto col-md-1">
                    <button type="button" id="DeleteTimeframeButton_` + i + `" class="btn-delete-timeframe btn-delete-md btn-danger form-control" data-id=` + i + `><i class="fa fa-remove"></i></button>
                </div>
            </div>
            <div><span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].EventDate" data-valmsg-replace="true"></span></div>
            <div><span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Starttime" data-valmsg-replace="true"></span></div>
            <div><span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Endtime" data-valmsg-replace="true"></span></div>`


        $("#TimeFramesToAdd").append(newTimeframe);

        //Set click event on generated button
        var button = $("#DeleteTimeframeButton_" + i);
        button.click(removeTimeframe);
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
        if ($(".timeframes").length > 1) {
            var timeframeToRemove = "#Timeframe_" + $(this).data('id');
            $(timeframeToRemove).remove();
        }
    }
});