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
        altclass: 'quoteRefDataRow',
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
        altclass: 'quoteRefDataRow',
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

        $("#ExportInvoice").show();
        $("#Invoice").hide();
    }
    else {
        $("#ExportInvoice").hide();
        $("#Invoice").show();
    }

    if (blstatus == "ORIGINAL ISSUED"){
        $("#MANIFEST").show();
    }
    else {
        $("#MANIFEST").hide();
    }
    
    if (jobref != "") {
        $("#Save").hide();
        $("#Update").show();
    }
    else {
        $("#Save").show();
        $("#Update").hide();
        $("#printbl").hide();
    }

    ContainerCount();

    var $s = $("#packageDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#grossweightDropDownList").selectmenu();
    var $s = $("#netweightDropDownList").selectmenu();
    var $s = $("#munitDropDownList").selectmenu();

    $("#grossweightDropDownList").selectmenu({
        width: 170
    });
    $("#netweightDropDownList").selectmenu({
        width: 170
    });
    $("#munitDropDownList").selectmenu({
        width: 170
    });

    $('#ContainerList').change(ContainerCount);

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

function ContainerCount() {
    var count1 = $("#ContainerList :selected").length;
    document.getElementById("CountingContainers").innerHTML = count1;
}

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