$(function () {

   

    //jQuery.validator.methods["date"] = function (value, element) { return true; };
    //var startDatePicker = $("#startDatePicker").datepicker();
    $("#startDatePicker").datepicker({
        
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',
        


        //yearRange: "-60:+0"
    });
    if ($("#startDatePicker").val() == "01-01-0001")
        {
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


    var today = new Date();
    var lastOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);


    if ($("#endDatePicker").val() == "01-01-0001") {
        $("#endDatePicker").datepicker("setDate", lastOfMonth);
    }

    $("#endDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#endDatePicker").datepicker().show();

    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#TransshipmentPortText").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#loadplantDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#dischplantDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#shipmentDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#equipmentDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    var $s = $("#agencyDropDownList").selectmenu();

    $("#Status").val("DRAFT");
    $("#Status").attr('readonly', 'readonly');

    $("#GrossWtUnit").val("KGS");
    $("#GrossWtUnit").attr('readonly', 'readonly');

    $('.selectExportList').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '225px'
    });

    $('.selectExportList').multiselect('selectAll', false);
    $('.selectExportList').multiselect('updateButtonText');

    $('.selectImportList').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '225px'
    });

    $('.selectImportList').multiselect('selectAll', false);
    $('.selectImportList').multiselect('updateButtonText');
    
    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });

    if ((transshipmenttype == "Yes") || (transshipmenttype == "YES"))
    {
        $("#TransshipmentTypeYes").prop("checked", true);
    }
    else {
        $("#TransshipmentTypeNo").prop("checked", true);
    }
    

    //$('#TransshipmentPortLabel').css('visibility', 'hidden');
    $("#TransshipmentPortText").selectmenu("widget").hide();

    $("#TransshipmentTypeYes").click(function () {
        $('#TransshipmentPortLabel').css('display', 'block');
        $("#TransshipmentPortText").selectmenu("widget").show();
       
       
    });

    $("#TransshipmentTypeNo").click(function () {
        $('#TransshipmentPortLabel').css('display', 'none');
        $("#TransshipmentPortText").selectmenu("widget").hide();
    });

    if ((transshipmenttype == "Yes") || (transshipmenttype == "YES")) {
        $('#TransshipmentPortLabel').css('display', 'block');
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

    if (ratetype == "REEFER") {
        $("#RateTypeReefer").prop("checked", true);
    }
    else {
        $("#RateTypeDry").prop("checked", true);
    }


});

