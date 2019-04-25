$(document).ready(function () {

    //Delete timeframes from edited event
    $(".DeleteTimeframeButton").click(removeTimeframe);

    //Add empty timeframes when adding event
    $("#addTimeframe").click(function (e) {
        e.preventDefault();

        //Set counter for dynamical generation of ID attributes for modelbinding
        var i = $(".timeframes").length;

        //Html element to be added
        var newTimeframe = `<div class="timeframes" id="Timeframe_` + i + `">
            <label for= "Timeframes_` + i + `__EventDate" > Event Date</label >
                <input type="date" data-val="true" data-val-required="The Event Date field is required." id="Timeframes_` + i + `__EventDate" name="Timeframes[` + i + `].EventDate" />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].EventDate" data-valmsg-replace="true"></span>
                <label for="Timeframes_` + i + `__Starttime">Start</label>
                <input min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The Start field is required." id="Timeframes_` + i + `__Starttime" name="Timeframes[` + i + `].Starttime" value="0" />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Starttime" data-valmsg-replace="true"></span>
                <label for="Timeframes_` + i + `__Endtime">End</label>
                <input min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The End field is required." id="Timeframes_` + i + `__Endtime" name="Timeframes[` + i + `].Endtime" value="0" />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Endtime" data-valmsg-replace="true"></span>
                <button type="button" id="DeleteTimeframeButton_` + i + `" class="DeleteTimeframeButton" data-id=` + i + `>Delete Timeframe</button>
            </div>`

        //Set click event on generated button
        $("#TimeFramesToAdd").append(newTimeframe);
        var button = $("#DeleteTimeframeButton_" + i);
        button.click(removeTimeframe);
    });

    //***Helper Functions***
    function removeTimeframe() {
        if ($(".timeframes").length > 1) {
            var timeframeToRemove = "#Timeframe_" + $(this).data('id');
            $(timeframeToRemove).remove();
        }
    }
});


