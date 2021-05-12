$(function () {

   // jQuery.validator.methods["date"] = function (value, element) { return true; };
    $("#startDatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'

        //yearRange: "-60:+0"
    });

    if ($("#startDatePicker").val() == "01-01-0001") {
        $("#startDatePicker").datepicker("setDate", new Date());
    }
    

    $("#startDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#startDatePicker").datepicker().show();

    $("#endDatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',
        //yearRange: "-60:+0"
    });

    var enddate = new Date().getDate;

    if ($("#endDatePicker").val() == "01-01-0001") {
        $("#endDatePicker").datepicker("setDate", enddate + 30);
    }

    $("#endDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#endDatePicker").datepicker().show();

    

    
    //$("#cutoffDatePicker").datetimepicker().next('button').button({
    //    icons: {
    //        primary: 'ui-icon-calendar'
    //    },
    //    text: false
    //});
    //if ($("#cutoffDatePicker").val() == "01-01-0001 12:00 AM") {
    //    $("#cutoffDatePicker").datetimepicker("setDateTime", "");
    //}
    $("#cutoffDatePicker").datetimepicker().show();

    $("#pickupDatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'

        //yearRange: "-60:+0"
    });
    $("#pickupDatePicker").datepicker();

    if ($("#pickupDatePicker").val() == "01-01-0001") {
        $("#pickupDatePicker").datepicker("setDate", new Date());
    }

    $("#pickupDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#pickupDatePicker").datepicker().show();

    $("#GrossweightMeasurement").val("KGS");
    $("#GrossweightMeasurement").attr('readonly', 'readonly');
    

    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });

    $("#BookingNo").attr('readonly', 'readonly');
    $("#QuoteRefID").attr('readonly', 'readonly');
    $("#GTotalSalesCal").attr('readonly', 'readonly');
    $("#Rate").attr('readonly', 'readonly');
    $("#RateCountered").attr('readonly', 'readonly');
    $("#CompanyName").attr('readonly', 'readonly');
    $("#EquipmentType").attr('readonly', 'readonly');
    $("#Quantity").attr('readonly', 'readonly');
    $("#LoadPort").attr('readonly', 'readonly');
    $("#DischargePort").attr('readonly', 'readonly');
    $("#PlaceOfReceipt").attr('readonly', 'readonly');
    $("#PlaceOfDelivery").attr('readonly', 'readonly');
    $("#PODFreeDays").attr('readonly', 'readonly');
    $("#POLFreeDays").attr('readonly', 'readonly');
    $("#CargoType").attr('readonly', 'readonly');
    $("#UNNo").attr('readonly', 'readonly');
    $("#IMO").attr('readonly', 'readonly');

    var $s = $("#collectionDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    $("#BookingStatus").attr('readonly', 'readonly');
    $("#ContainerReleaseOrderNo").attr('readonly', 'readonly');
    $("#TransshipmentTypeYes").prop("disabled", true);
    $("#TransshipmentTypeNo").prop("disabled", true);
    $("#TransshipmentPortText").attr('readonly', 'readonly');

    


    if (BookingStatus == "CONFIRMED") {

        $("#Save").hide();
        $("#Update").hide();
        $("#Confirm").hide();
        $("#Shipment").hide();
        $("#LoadAgent").attr('readonly', 'readonly');
        $("#DischAgent").attr('readonly', 'readonly');
        $("#ServiceMode").attr('readonly', 'readonly');
        $("#Commodity").attr('readonly', 'readonly');
        $("#CommodityGroup").attr('readonly', 'readonly');
        $("#Grossweight").attr('readonly', 'readonly');
        $("#GrossweightMeasurement").attr('readonly', 'readonly');
        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#LoadTerminal").attr('readonly', 'readonly');
        $("#DischargeTerminal").attr('readonly', 'readonly');
        $("#Remark").attr('readonly', 'readonly');
        $("#CRORemarks").attr('readonly', 'readonly');
        $("#paymentDropDownList").prop("disabled", true);
        $("#pickupDatePicker").prop("disabled", true);
        $("#cutoffDatePicker").attr('readonly', 'readonly');
        $("#startDatePicker").prop("disabled", true);
        $("#startDatePicker").datepicker({
            "showButtonPanel": false
        });
        $("#endDatePicker").prop("disabled", true);
        $("#AddressAttn").attr('readonly', 'readonly');
        $("#AddressFax").attr('readonly', 'readonly');
        $("#AddressTel").attr('readonly', 'readonly');
        $("#AddressTo").attr('readonly', 'readonly');
        $("#Volume").attr('readonly', 'readonly');
        $("#SOC").attr('readonly', 'readonly');
        $("#collectionDropDownList").prop("disabled", true);
    }

    else if (BookingStatus == "ISSUED") {

        $("#Save").hide();
        $("#Update").hide();
        $("#Confirm").hide();
        $("#Edit").hide();
        $("#Issue").hide();
        if (hasshipment == "True") {
            $("#Shipment").hide();
        }

        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#LoadAgent").attr('readonly', 'readonly');
        $("#DischAgent").attr('readonly', 'readonly');
        $("#ServiceMode").attr('readonly', 'readonly');
        $("#Commodity").attr('readonly', 'readonly');
        $("#CommodityGroup").attr('readonly', 'readonly');
        $("#Grossweight").attr('readonly', 'readonly');
        $("#GrossweightMeasurement").attr('readonly', 'readonly');
        $("#LoadTerminal").attr('readonly', 'readonly');
        $("#DischargeTerminal").attr('readonly', 'readonly');
        $("#Remark").attr('readonly', 'readonly');
        $("#CRORemarks").attr('readonly', 'readonly');
        $("#AddressAttn").attr('readonly', 'readonly');
        $("#AddressFax").attr('readonly', 'readonly');
        $("#AddressTel").attr('readonly', 'readonly');
        $("#AddressTo").attr('readonly', 'readonly');
        $("#Volume").attr('readonly', 'readonly');
        $("#SOC").attr('readonly', 'readonly');
        $("#collectionDropDownList").prop("disabled", true);
        $("#pickupDatePicker").prop("disabled", true);
        $("#cutoffDatePicker").attr('readonly', 'readonly');
        $("#startDatePicker").prop("disabled", true);
        $("#endDatePicker").prop("disabled", true);


    }

    else if (BookingStatus == "DRAFT") {
        $("#Edit").hide();
        $("#Save").hide();
        $("#PrintBC").hide();
        $("#PrintCRO").hide();
        $("#Issue").hide();
        $("#Shipment").hide();
    }

    else {
        $("#Update").hide();
        $("#Confirm").hide();
        $("#Edit").hide();
        $("#Issue").hide();
        $("#Shipment").hide();

        $("#PrintBC").hide();
        $("#PrintCRO").hide();
        $("#BookingStatus").val("DRAFT");
    }


    if (transshipmenttype == "Yes") {
        $('#TransshipmentPortLabel').css('visibility', 'visible');
        $("#TransshipmentPortText").show();
    }

    else {
        $('#TransshipmentPortLabel').css('visibility', 'hidden');
        $("#TransshipmentPortText").hide();
    }

    $("#RateTypeReefer").click(function () {
        $('#TemperatureLabel').css('display', 'block');
        $('#TemperatureValid').css('display', 'block');
        $('#Temperature').css('display', 'block');
        $('#HumidityLabel').css('display', 'block');
        $('#HumidityValid').css('display', 'block');
        $('#Humidity').css('display', 'block');
        $('#VentilationLabel').css('display', 'block');
        $('#VentilationValid').css('display', 'block');
        $('#Ventilation').css('display', 'block');
        $('#Temperature').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Temperature"
                }
            });
        });
        $('#Humidity').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Humidity"
                }
            });
        });
        $('#Ventilation').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Ventilation"
                }
            });
        });
    });

    $("#RateTypeDry").click(function () {
        $('#TemperatureLabel').css('display', 'none');
        $('#TemperatureValid').css('display', 'none');
        $('#Temperature').css('display', 'none');
        $('#HumidityLabel').css('display', 'none');
        $('#HumidityValid').css('display', 'none');
        $('#Humidity').css('display', 'none');
        $('#VentilationLabel').css('display', 'none');
        $('#VentilationValid').css('display', 'none');
        $('#Ventilation').css('display', 'none');
    });


    if (ratetype == "REEFER") {
        $('#TemperatureLabel').css('display', 'block');
        $('#TemperatureValid').css('display', 'block');
        $('#Temperature').css('display', 'block');
        $('#HumidityLabel').css('display', 'block');
        $('#HumidityValid').css('display', 'block');
        $('#Humidity').css('display', 'block');
        $('#VentilationLabel').css('display', 'block');
        $('#VentilationValid').css('display', 'block');
        $('#Ventilation').css('display', 'block');
        $('#Temperature').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Temperature"
                }
            });
        });
        $('#Humidity').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Humidity"
                }
            });
        });
        $('#Ventilation').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Ventilation"
                }
            });
        });
    }


    
});