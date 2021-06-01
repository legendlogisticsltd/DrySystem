$(function () {

    $("#LoadFT").attr('readonly', 'readonly');
    $("#LoadPort").attr('readonly', 'readonly');
    $("#DischFT").attr('readonly', 'readonly');
    $("#DischPort").attr('readonly', 'readonly');
    $("#PlaceOfDelivery").attr('readonly', 'readonly');
    $("#PlaceOfReceipt").attr('readonly', 'readonly');
    $("#JobRef").attr('readonly', 'readonly');
    $("#Quantity").attr('readonly', 'readonly');
    $("#IDNo").attr('readonly', 'readonly');
    $("#loadportDropDownList").prop("disabled", true);
    $("#dischportDropDownList").prop("disabled", true);
    $("#ETDDatePicker").attr('readonly', 'readonly');
    $("#ETADatePicker").attr('readonly', 'readonly');

    $("#ETADatePicker").datepicker({
        dateFormat: "mm-dd-yy"
    });
    if ($("#ETADatePicker").val() == "01-01-0001") {
        $("#ETADatePicker").datepicker("setDate", '');
    }

    $("#ETADatePicker").datepicker().show();
    $("#ETADatePicker").datepicker("disable");
    $("#ETADatePicker").datepicker().next('button').hide();

    $("#ETDDatePicker").datepicker({
        dateFormat: "mm-dd-yy"
    });
    if ($("#ETDDatePicker").val() == "01-01-0001") {
        $("#ETDDatePicker").datepicker("setDate", '');
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
        $("#ATADatePicker").datepicker("setDate", '');
    }

    $("#ATADatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#ATADatePicker").datepicker().show();

    if (status == "DRAFT") {
        $("#PrintCRO").hide();
        $("#PrintIInvoice").hide();
    }
    //else if (invoiceStatus == "ISSUED") {
    //    //$("#PrintCRO").prop("disabled", true);
    //    $("#PrintIInvoice").attr("disabled", true);
    //    $("#PrintIInvoice").css("pointer-events", "none");
    //    $("#PrintIInvoice").css("opacity", 0.65);
    //    $("#ATADatePicker").attr('readonly', 'readonly');
    //    $("#Confirm").hide();
    //    $("#Save").hide();
    //}
    else {
        $("#ATADatePicker").datepicker("disable");
        $("#ATADatePicker").datepicker().next('button').hide();

        //$("#ATADatePicker").attr('readonly', 'readonly');
        $("#Confirm").hide();
        $("#Save").hide();
        $("#ATADatePicker").datepicker({
            "showButtonPanel": false
        });
    }

    //$("#PrincipalRemark").prop("disabled", true);
    $("#DischargePlanStatus").attr('readonly', 'readonly');
    $("#JobRef").attr('readonly', 'readonly');

    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });

    $("#PrintCRO").click(function () {
        ReloadPage();
    })

    $("#PrintIInvoice").click(function () {
        ReloadPage();
    })

    function ReloadPage() {
        setTimeout(function () {
            window.location.reload(1);
        }, 1000);
    }

});