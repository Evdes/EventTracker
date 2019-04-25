$(document).ready(function () {

    //Delete timeframes from edited event
    $(".DeleteTimeframeButton").click(function () {
        var timeframeToRemove = "#Timeframe_" + $(this).data('id') ;
        $(timeframeToRemove).remove();
    })

    //Add empty timeframes when adding event
    $("#addTimeframe").click(function (e) {
        e.preventDefault();
        var i = $(".timeframes").length;
        var dateOfToday = $("#Timeframes_0__EventDate").val();
        var newTimeframe = `<div class="timeframes">
            <label for= "Timeframes_` + i + `__EventDate" > Event Date</label >
                <input min=` + dateOfToday + ` type="date" data-val="true" data-val-required="The Event Date field is required." id="Timeframes_` + i + `__EventDate" name="Timeframes[` + i + `].EventDate" value=` + dateOfToday + ` />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].EventDate" data-valmsg-replace="true"></span>
                <label for="Timeframes_` + i + `__Starttime">Start</label>
                <input min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The Start field is required." id="Timeframes_` + i + `__Starttime" name="Timeframes[` + i + `].Starttime" value="0" />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Starttime" data-valmsg-replace="true"></span>
                <label for="Timeframes_` + i + `__Endtime">End</label>
                <input min="0" max="24" type="number" data-val="true" data-val-range="Invalid hour" data-val-range-max="24" data-val-range-min="0" data-val-required="The End field is required." id="Timeframes_` + i + `__Endtime" name="Timeframes[` + i + `].Endtime" value="0" />
                <span class="field-validation-valid" data-valmsg-for="Timeframes[` + i + `].Endtime" data-valmsg-replace="true"></span>
            </div>`
        $("#TimeframesToAdd").append(newTimeframe);
    })
});

