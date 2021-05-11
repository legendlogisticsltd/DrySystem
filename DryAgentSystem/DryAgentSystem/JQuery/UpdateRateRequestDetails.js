$(function () {

    //jQuery.validator.methods["date"] = function (value, element) { return true; };
    //var startDatePicker = $("#startDatePicker").datepicker();
    $("#startDatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'

        //yearRange: "-60:+0"
    });
    $("#startDatePicker").datepicker();

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

    $("#endDatePicker").datepicker();

    $("#endDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#endDatePicker").datepicker().show();

    if (status == "DRAFT" || status == "APPROVAL REQUIRED" || status == "APPROVED" || status == "REJECTED") {
        if (rateCountered <= 0) {
            $("#rateCounteredLabel").hide();
            $("#RateCountered").hide();
            $("#offerRateLabel").hide();
            $("#OfferRate").hide();
        }
        else {
            $("#offerRateLabel").hide();
            $("#OfferRate").hide();
        }
        
    }

    $("#PrincipalRemark").prop("disabled", true);
    $("#Status").attr('readonly', 'readonly');
    $("#RateID").attr('readonly', 'readonly');
    $("#GrossWtUnit").attr('readonly', 'readonly');


    if (status == "COUNTER OFFER") {

        $("#Fetch").hide();
        $("#Approval").prop("value", "Accept Offer");

        $("#Rate").attr('readonly', 'readonly');
        $("#RateCountered").attr('readonly', 'readonly');
        $("#Quantity").attr('readonly', 'readonly');
        //$("#Quantity").prop("disabled", true);

        $("#agencyDropDownList").prop("disabled", true);

        $("#ShipperName").attr('readonly', 'readonly');
        $("#GrossWt").attr('readonly', 'readonly');
        $("#TransshipmentTypeYes").prop("disabled", true);
        $("#TransshipmentTypeNo").prop("disabled", true);
        $("#TransshipmentPortText").prop("disabled", true);
        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#companyDropDownList").prop("disabled", true);
        $("#equipmentDropDownList").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#dischplantDropDownList").prop("disabled", true);
        $("#loadplantDropDownList").prop("disabled", true);
        $("#shipmentDropDownList").prop("disabled", true);

        $("#startDatePicker").attr('readonly', 'readonly');
        $("#endDatePicker").attr('readonly', 'readonly');

        $("#POLFreeDays").attr('readonly', 'readonly');
        $("#PODFreeDays").attr('readonly', 'readonly');

        $("#CargoType").attr('readonly', 'readonly');
        $("#UNNo").attr('readonly', 'readonly');
        $("#IMO").attr('readonly', 'readonly');
        $("#Remark").attr('readonly', 'readonly');

        $("#ExportChargesList").attr('readonly', 'readonly');
        $("#ImportChargesList").attr('readonly', 'readonly');
        //$("#rateChargesGrid").hide();
    }
    if (status == "APPROVAL REQUIRED") {

        $("#Save").hide();
        $("#Fetch").hide();
        $("#Approval").hide();

        $("#Rate").prop("disabled", true);
        $("#Quantity").prop("disabled", true);
        $("#RateCountered").prop("disabled", true);

        $("#ShipperName").attr('readonly', 'readonly');
        $("#GrossWt").attr('readonly', 'readonly');
        $("#agencyDropDownList").prop("disabled", true);
        $("#TransshipmentTypeYes").prop("disabled", true);
        $("#TransshipmentTypeNo").prop("disabled", true);
        $("#TransshipmentPortText").prop("disabled", true);
        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#companyDropDownList").prop("disabled", true);
        $("#equipmentDropDownList").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#dischplantDropDownList").prop("disabled", true);
        $("#loadplantDropDownList").prop("disabled", true);
        $("#shipmentDropDownList").prop("disabled", true);

        $("#startDatePicker").prop("disabled", true);
        $("#startDatePicker").datepicker({
            "showButtonPanel": false
        });
        $("#endDatePicker").prop("disabled", true);

        $("#POLFreeDays").prop("disabled", true);
        $("#PODFreeDays").prop("disabled", true);

        $("#CargoType").prop("disabled", true);
        $("#UNNo").attr('readonly', 'readonly');
        $("#IMO").attr('readonly', 'readonly');
        $("#Remark").prop("disabled", true);

        $("#ExportChargesList").attr('readonly', 'readonly');
        $("#ImportChargesList").attr('readonly', 'readonly');

        //$("#rateChargesGrid").hide();
    }


    if (status == "APPROVED") {

        $("#Save").hide();
        $("#Fetch").hide();
        $("#Approval").hide();

        $("#Rate").prop("disabled", true);
        $("#Quantity").prop("disabled", true);
        $("#RateCountered").prop("disabled", true);

        $("#ShipperName").attr('readonly', 'readonly');
        $("#GrossWt").attr('readonly', 'readonly');
        $("#agencyDropDownList").prop("disabled", true);
        $("#TransshipmentTypeYes").prop("disabled", true);
        $("#TransshipmentTypeNo").prop("disabled", true);
        $("#TransshipmentPortText").prop("disabled", true);
        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#companyDropDownList").prop("disabled", true);
        $("#equipmentDropDownList").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#dischplantDropDownList").prop("disabled", true);
        $("#loadplantDropDownList").prop("disabled", true);
        $("#shipmentDropDownList").prop("disabled", true);

        $("#startDatePicker").prop("disabled", true);
        $("#endDatePicker").prop("disabled", true);

        $("#POLFreeDays").prop("disabled", true);
        $("#PODFreeDays").prop("disabled", true);

        $("#CargoType").prop("disabled", true);
        $("#UNNo").attr('readonly', 'readonly');
        $("#IMO").attr('readonly', 'readonly');
        $("#Remark").prop("disabled", true);

        $("#ExportChargesList").attr('readonly', 'readonly');
        $("#ImportChargesList").attr('readonly', 'readonly');
        //$("#ExportChargesList").prop("disabled", true);
        //$("#ImportChargesList").prop("disabled", true);

    }

    if (status == "REJECTED") {

        $("#Save").hide();
        $("#Fetch").hide();
        $("#Approval").hide();

        $("#Rate").prop("disabled", true);
        $("#RateCountered").prop("disabled", true);
        $("#Quantity").prop("disabled", true);

        $("#ShipperName").attr('readonly', 'readonly');
        $("#GrossWt").attr('readonly', 'readonly');
        $("#agencyDropDownList").prop("disabled", true);
        $("#TransshipmentTypeYes").prop("disabled", true);
        $("#TransshipmentTypeNo").prop("disabled", true);
        $("#TransshipmentPortText").prop("disabled", true);
        $("#RateTypeReefer").prop("disabled", true);
        $("#RateTypeDry").prop("disabled", true);
        $("#Temperature").attr('readonly', 'readonly');
        $("#Humidity").attr('readonly', 'readonly');
        $("#Ventilation").attr('readonly', 'readonly');
        $("#companyDropDownList").prop("disabled", true);
        $("#equipmentDropDownList").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#dischplantDropDownList").prop("disabled", true);
        $("#loadplantDropDownList").prop("disabled", true);
        $("#shipmentDropDownList").prop("disabled", true);

        $("#startDatePicker").prop("disabled", true);
        $("#endDatePicker").prop("disabled", true);

        $("#POLFreeDays").prop("disabled", true);
        $("#PODFreeDays").prop("disabled", true);

        $("#CargoType").prop("disabled", true);
        $("#UNNo").attr('readonly', 'readonly');
        $("#IMO").attr('readonly', 'readonly');
        $("#Remark").prop("disabled", true);

        $("#ExportChargesList").attr('readonly', 'readonly');
        $("#ImportChargesList").attr('readonly', 'readonly');

        //$("#rateChargesGrid").hide();

    }

    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#TransshipmentPortText").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#loadplantDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#dischplantDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#shipmentDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#equipmentDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#agencyDropDownList").selectmenu();

    $('.selectExportList').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '225px'
    });

    $('.selectImportList').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '225px'
    });

    $("#Fetch").click(function () {      
        $('.selectExportList').multiselect('selectAll', false);
        $('.selectExportList').multiselect('updateButtonText');
        $('.selectImportList').multiselect('selectAll', false);
        $('.selectImportList').multiselect('updateButtonText');
    });


    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });


    $quoteChargesGrid = $('#rateChargesGrid').jqGrid({
        mtype: 'Get',
        url: 'GetQuoteChargesList', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['Description'],
        colModel: [
            //{
            //    key: true,
            //    //hidden: true,
            //    name: 'ID',
            //    index: 'ID',
            //    width: '50px'
            //},
            {
                key: false,
                name: 'Description',
                width: '250px'
            }
        ],
        loadonce: true,
        responsive: true,
        autowidth: false,
        gridview: true,
        autoencode: true,
        //pager: '#quoteChargesPager',
        //rowNum: 10,
        //rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        rownumbers: true,
        multiselect: false,
        altRows: true,
        altclass: 'quoteRefDataRow',
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display'
    });


    $('#TransshipmentPortLabel').css('visibility', 'hidden');
    $("#TransshipmentPortText").selectmenu("widget").hide();

    $("#TransshipmentTypeYes").click(function () {
        $('#TransshipmentPortLabel').css('visibility', 'visible');
        $("#TransshipmentPortText").selectmenu("widget").show();
    });

    $("#TransshipmentTypeNo").click(function () {
        $('#TransshipmentPortLabel').css('visibility', 'hidden');
        $("#TransshipmentPortText").selectmenu("widget").hide();
    });

    if ((transshipmenttype == "Yes") || (transshipmenttype == "YES")) {
        $('#TransshipmentPortLabel').css('visibility', 'visible');
        $("#TransshipmentPortText").selectmenu("widget").show();
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

    $("#companyDropDownList").selectmenu({
        width: 300
    });
});