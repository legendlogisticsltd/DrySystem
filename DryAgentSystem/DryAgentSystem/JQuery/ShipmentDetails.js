$(function () {

    //$("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //$("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
   // window.onload = detectPopupBlocker;

    $("#blfinalisedDatePicker").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',



        //yearRange: "-60:+0"
    });
    if ($("#blfinalisedDatePicker").val() == "01-01-0001") {
        $("#blfinalisedDatePicker").datepicker("setDate", new Date());
    }
    $("#blfinalisedDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#blfinalisedDatePicker").datepicker().show();

    $("#issueDatePicker").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',



        //yearRange: "-60:+0"
    });
    if ($("#issueDatePicker").val() == "01-01-0001") {
        $("#issueDatePicker").datepicker("setDate", new Date());
    }
    $("#issueDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#issueDatePicker").datepicker().show();

    $("#ladenDatePicker").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',



        //yearRange: "-60:+0"
    });
    if ($("#ladenDatePicker").val() == "01-01-0001") {
        $("#ladenDatePicker").datepicker("setDate", new Date());
    }
    $("#ladenDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#ladenDatePicker").datepicker().show();

    $("#closingDatePicker").datetimepicker().show();


    $vesselGrid = $('#vesselGrid').jqGrid({
        mtype: 'Get',
        url: 'GetVesselDetails', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['ID', 'Vessel Name', 'Voy No', 'Load Port', 'Discharge Port', 'ETD POL', 'ETA POD', 'Carrier', 'UniversalSerialNr', 'CarrierBookingRefNo'],
        colModel: [
            {
                key: true,
                hidden: true,
                name: 'ID',
                index: 'ID',
            },
            {
                key: false,
                name: 'VesselName'
            },
            {
                key: false,
                name: 'VoyNo'
            },
            {
                key: false,
                name: 'LoadPort'
            },
            {
                key: false,
                name: 'DischPort'
            },
            {
                key: false,
                name: 'ETD',
                formatter: 'date',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ETA',
                formatter: 'date',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                hidden: true,
                name: 'Carrier'
            },
            {
                key: false,
                hidden: true,
                name: 'UniversalSerialNr'
            },
            {
                key: false,
                hidden: true,
                name: 'CarrierBookingRefNo'
            }
        ],
        loadonce: true,
        responsive: true,
        gridview: true,
        autoencode: true,
        pager: '#vesselPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        altRows: true,        
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display',
        //jsonReader: {
        //    root: "rows",
        //    page: "page",
        //    total: "total",
        //    records: "records",
        //    repeatitems: false,
        //    Id: "0"
        //},
        autowidth: true,
        multiselect: false,
        rownumbers: true
       
    });


    $('#vesselGrid').navGrid('#vesselPager', { edit: false, add: false, del: false, search: false, refresh: true });

    $tankGrid = $('#tankGrid').jqGrid({
        mtype: 'Get',
        url: 'GetTankDetails', //'/QuotationDetails/GetQuoteChargesList'
        editurl: 'ProcessTankData',
        onAfterSaveCell: reload,
        datatype: 'json',
        colNames: ['ID', 'Container No', 'Seal No', 'Gross Weight', 'Net Weight', 'Measurement','UniversalSerialNr'],
        colModel: [
            {
                key: true,
                hidden: true,
                name: 'ID',
                index: 'ID',
                editable: false
            },
            {
                key: false,
                //hidden: true,
                name: 'ContainerNo',
                index: 'ContainerNo',

                editable: false
            },
            {
                key: false,
                name: 'SealNo',
                editable: true
            },
            {
                key: false,
                name: 'GrossWeight',
                editable: true
            },
            {
                key: false,
                name: 'NettWeight',
                editable: true
            },
            {
                key: false,
                name: 'Measurement',
                editable: true
            },
            {
                key: false,
                hidden: true,
                name: 'UniversalSerialNr'
            }
        ],
        loadonce: true,
        responsive: true,
        gridview: true,
        autoencode: true,
        pager: '#tankPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        altRows: true,
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display',
        autowidth: true,
        multiselect: false,
        rownumbers: true
        
    });



    $('#tankGrid').navGrid('#tankPager', { edit: false, add: false, del: true, deltext: "Delete", search: false, refresh: false });

    $('#tankGrid').jqGrid('inlineNav', '#tankPager',
        {
            edit: true,
            editicon: "ui-icon-pencil",
            edittext: "Edit",
            add: false,
            //addicon: "ui-icon-plus",
            //addtext: "Add",
            save: true,
            saveicon: "ui-icon-disk",
            savetext: "Save",
            //saveurl: '/ShipmentDetails/AddTank/',
            cancel: true,
            cancelicon: "ui-icon-cancel",
            canceltext: "Cancel",
            addParams: { position: "last" }
        });


    function reload(rowid, result) {
        $("#tankGrid").trigger("reloadGrid");
    }

    $('.selectContainerList').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        includeFilterClearBtn: true,
        enableCaseInsensitiveFiltering: true,
        buttonWidth: '250px',
        numberDisplayed: 1,
        maxHeight: 200
    });

    
    $('.selectContainerList').multiselect('updateButtonText');
    //$('.selectContainerList').click();
    
    $("#ShipmentDetailsModel_JobRef").attr('readonly', 'readonly');
    $("#BLDetailsModel_PlaceofIssue").attr('readonly', 'readonly');
    $("#BLDetailsModel_BLTypes").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_Quantity").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_LoadPort").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_DischPort").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_PlaceOfReceipt").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_PlaceOfDelivery").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_ShipmentTerm").attr('readonly', 'readonly');
    $("#ShipmentDetailsModel_HBLHAWB").attr('readonly', 'readonly');
    //$("#ExportInvoice").hide();

    if (bltypes == "") {
        $("#ORIGINAL").hide();
        $("#NON-NEGOTIABLE").hide();
        $("#SEAWAY").hide();
        $("#SURRENDER").hide();
    }

    if (invoicesave == "True") {

        $("#ORIGINAL").show();
        $("#NON-NEGOTIABLE").show();
        $("#SEAWAY").show();
        $("#SURRENDER").show();
    }
    else {
        $("#ORIGINAL").hide();
        $("#NON-NEGOTIABLE").hide();
        $("#SEAWAY").hide();
        $("#SURRENDER").hide();
    }

    if (blstatus == "ORIGINAL ISSUED"){
        $("#MANIFEST").show();
    }
    else {
        $("#MANIFEST").hide();
    }

    

    $("#SEAWAY").click(function () {
        
        if (blseawaystatus == "SEAWAY ISSUED") {
            alert('You have already printed SEAWAY BL. You cannot print again.\nPlease click Request Re-Print Button to send a request to HQ for allowing you to print again');
            event.preventDefault();
        }
        else {
            if (confirm('Are you sure you want to Proceed?\nPlease note that you can only print SEAWAY BL once')) {
                ReloadPage();
            }
            else {
                event.preventDefault();
            }
        }
            
    });

    $("#ORIGINAL").click(function () {
        if (blstatus == "ORIGINAL ISSUED") {
            alert('You have already printed ORIGINAL BL. You cannot print again.\nPlease click Request Re-Print Button to send a request to HQ for allowing you to print again');
            event.preventDefault();
        }
        else {
            if (confirm('Are you sure you want to Proceed?\nPlease note that you can only print ORIGINAL BL once')) {
                ReloadPage();
            }
            else {
                event.preventDefault();
            }
        }
    });

    if ((blstatus == "ORIGINAL ISSUED") || (blseawaystatus == "SEAWAY ISSUED")) {
        $("#RePrint").show();
    }
    else {
        $("#RePrint").hide();
    }


    $("#RePrint").click(function () {
        alert('Re-Print BL request Sent to HQ Successfully')
    })


    
    if (jobref != "") {
        $("#Save").hide();
        $("#Update").show();
    }
    else {
        $("#Save").show();
        $("#Update").hide();
        $("#printbl").hide();
    }

    //ContainerCount();

    var $s = $("#packageDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#grossweightDropDownList").selectmenu();
    var $s = $("#netweightDropDownList").selectmenu();
    var $s = $("#munitDropDownList").selectmenu();
    //var $s = $("#shippernamesiDropDownList").selectmenu();//.selectmenu("menuWidget").addClass("overflow");
    //var $s = $("#consigneenamesiDropDownList").selectmenu();//.selectmenu("menuWidget").addClass("overflow");
    //var $s = $("#shipperDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //var $s = $("#consigneenameblDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    $("#grossweightDropDownList").selectmenu({
        width: 170
    });
    $("#netweightDropDownList").selectmenu({
        width: 170
    });
    $("#munitDropDownList").selectmenu({
        width: 170
    });

    $('#shipperDropDownList').change(function () {
        var selected_val = $('#shipperDropDownList').find(":selected").attr('value');
        var address = $('#ShipmentDetailsModel_ShipperAddress');
        get_add(selected_val, address);
    });

    $('#consigneenamesiDropDownList').change(function () {
        var selected_val = $('#consigneenamesiDropDownList').find(":selected").attr('value');
        var address = $('#BLDetailsModel_ConsigneeAddressSI');
        get_add(selected_val, address);
    });

    $('#shippernamesiDropDownList').change(function () {
        var selected_val = $('#shippernamesiDropDownList').find(":selected").attr('value');
        var address = $('#BLDetailsModel_ShipperAddressSI');
        get_add(selected_val, address);
    });

    $('#consigneenameblDropDownList').change(function () {
        var selected_val = $('#consigneenameblDropDownList').find(":selected").attr('value');
        var address = $('#BLDetailsModel_ConsigneeAddressBL');
        get_add(selected_val, address);
    });

    $("ContainerList").prop('class', 'selectpicker show-tick form-control');
    $("ContainerList").attr('data-live-search', true);

    $('.ContainerList').multiselect({
        buttonWidth: '225px'
    });

  
    $('.ContainerList').multiselect('updateButtonText');
});

function MarksAndNoCount() {
    var i = document.getElementById("MarksAndNo").value.length;
    document.getElementById("CountingCharactersMarksAndNo").innerHTML =  i;
}

function CargoDescriptionCount() {
    var i = document.getElementById("CargoDescription").value.length;
    document.getElementById("CountingCharactersCargo").innerHTML = i;
}

function InvoiceRemarkCount() {
    var i = document.getElementById("InvoiceRemark").value.length;
    document.getElementById("CountingCharactersRemarks").innerHTML = i;
}

//function ContainerCount() {
//    var count1 = $("#ContainerList :selected").length;
//    document.getElementById("CountingContainers").innerHTML = count1;
//}

function ReloadPage() {
    setTimeout(function () {
        window.location.reload(1);
    }, 1000);
}

function detectPopupBlocker() {
    var windowUrl = 'about:blank';
    var windowId = 'TestPopup_' + new Date().getTime();
    var windowFeatures = 'left=0,top=0,width=400px,height=200px';
    var windowRef = window.open(windowUrl, windowId, windowFeatures);

    if (!windowRef) {
        alert('A popup blocker was detected. Please turn it off to use this application.');
    }
    else {
        // No popup blocker was detected...
        windowRef.close();
        document.getElementById('pageContent').style.display = 'block';
    }
}

function RePrintMethod() {
    debugger;
    $.ajax({
        type: "POST",
        url: 'MailSend',
        data: {
            jobref: jobref,
        },
        dataType: 'json',
        success: function (result) {

            alert(result.msg); //You can also not pop up a prompt box.You could code anything what you want               
        }
    });
}

function get_add(selected_val, address) {
    

    $.ajax({
        type: "GET",
        url: 'selectAdd',
        contentType: "application/json; charset=utf-8",
        data: { name: selected_val },
        dataType: "json",
        success: function (data) {
            if (data.length > 0) {
                address.val(data[0].Value);
            }
            else {
                address.val('');
            }
        }
    });
}

function shipping() {
    debugger;
    var ShipperNameSI = $('#shippernamesiDropDownList').find(":selected").attr('value');
    var ShipperAddressSI = $('#BLDetailsModel_ShipperAddressSI').val();
    var ConsigneeNameSI = $('#shippernamesiDropDownList').find(":selected").attr('value');
    var ConsigneeAddressSI = $('#BLDetailsModel_ConsigneeAddressSI').val();
    var UniversalSerialNr = usn;
    $.ajax({
        type: "POST",
        url: 'ShippingInstruction',
        data: {
            ShipperNameSI: ShipperNameSI,
            ShipperAddressSI: ShipperAddressSI,
            ConsigneeNameSI: ConsigneeNameSI,
            ConsigneeAddressSI: ConsigneeAddressSI,
            UniversalSerialNr: UniversalSerialNr
        },
        dataType: "json",
        success: function (result) {
             //You can also not pop up a prompt box.You could code anything what you want               
        }
    });

}
