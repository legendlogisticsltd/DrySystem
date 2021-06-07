$(function () {
    $("#LoadFT").attr('readonly', 'readonly');
    $("#LoadPort").attr('readonly', 'readonly');
    $("#DischFT").attr('readonly', 'readonly');
    $("#DischPort").attr('readonly', 'readonly');
    $("#PlaceOfDelivery").attr('readonly', 'readonly');
    $("#PlaceOfReceipt").attr('readonly', 'readonly');
    $("#JobRef").attr('readonly', 'readonly');
    $("#PortPair").attr('readonly', 'readonly');
    $("#Quantity").attr('readonly', 'readonly');

    $("#loadportDropDownList").prop("disabled", true);
    $("#dischportDropDownList").prop("disabled", true);
    //$("#ETDDatePicker").attr('readonly', 'readonly');
    //$("#ETADatePicker").attr('readonly', 'readonly');

    $("#DischargePlanStatus").val("DRAFT");
    $("#DischargePlanStatus").attr('readonly', 'readonly');

    $("#ETADatePicker").datepicker({
        dateFormat: "mm-dd-yy"
    });

    if ($("#ETADatePicker").val() == "01-01-0001") {
        $("#ETADatePicker").datepicker("setDate", new Date());
    }

    $("#ETADatePicker").datepicker().show();
    $("#ETADatePicker").datepicker("disable");
    $("#ETADatePicker").datepicker().next('button').hide();

    $("#ETDDatePicker").datepicker({
        dateFormat: "mm-dd-yy"
    });

    if ($("#ETDDatePicker").val() == "01-01-0001") {
        $("#ETDDatePicker").datepicker("setDate", new Date());
    }

    $("#ETDDatePicker").datepicker().show();
    $("#ETDDatePicker").datepicker("disable");
    $("#ETDDatePicker").datepicker().next('button').hide();

    $("#ATADatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'
    });

    if ($("#ATADatePicker").val() == "01-01-0001") {
        $("#ATADatePicker").datepicker("setDate", new Date());
    }

    $("#ATADatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#ATADatePicker").datepicker().show();
   
    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });

   

});