

// Datepickers

$(document).ready(function () {
    let selectedStart;
    let selectedEnd;

    let maxDate = new Date();
    maxDate.setMonth(maxDate.getMonth() + 6);

    // Update RentalEnd datepicker based on the selected start date
    function updateEndDatepicker() {

        let firstUnavailableDate = null;

        for (let i = 0; i < disabledDates.length; i++) {
            let dateToCheck = new Date(disabledDates[i]);
            if (dateToCheck > selectedStart) {
                firstUnavailableDate = dateToCheck;
                break;
            }
        }

        $('#endDateDiv').datepicker('option', 'minDate', selectedStart);
        $('#endDateDiv').datepicker('option', 'beforeShowDay', function (date) {
            let dateString = $.datepicker.formatDate('yy-mm-dd', date);

            let isNotDisabled = disabledDates.indexOf(dateString) === -1;
            let isBeforeMaxDate = date <= maxDate;
            let isAllowed = isNotDisabled && isBeforeMaxDate;

            if (firstUnavailableDate) {
                isAllowed = isAllowed && date < firstUnavailableDate;
            }

            return [isAllowed];
        });
    }

    // RentalStart datepicker
    $('#startDateDiv').datepicker({
        inline: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        minDate: 0,
        maxDate: maxDate,
        defaultDate: null,
        onSelect: function () {
            selectedStart = $(this).datepicker("getDate");

            $('#endDateDiv').datepicker('option', 'minDate', selectedStart);

            let date = selectedStart.toLocaleDateString('en-CA');
            $('#startDateForm').val(date);
            $('#endDateForm').val(date);

            updateEndDatepicker();
        },
        beforeShowDay: function (date) {
            let dateString = $.datepicker.formatDate('yy-mm-dd', date);
            return [disabledDates.indexOf(dateString) === -1];
        }
    });

    // RentalEnd datepicker
    $('#endDateDiv').datepicker({
        inline: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        minDate: 0,
        maxDate: maxDate,
        defaultDate: null,
        onSelect: function () {
            selectedEnd = $(this).datepicker("getDate");
            let date = selectedEnd.toLocaleDateString('en-CA');
            $('#endDateForm').val(date);
        },
        beforeShowDay: function (date) {
            let dateString = $.datepicker.formatDate('yy-mm-dd', date);
            return [disabledDates.indexOf(dateString) === -1];
        }
    });
});