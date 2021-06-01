$(function () {

    //$("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //$("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
   
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

    $containerGrid = $('#containerGrid').jqGrid({
        mtype: 'Get',
        url: 'GetContainerDetails', //'/QuotationDetails/GetQuoteChargesList'
        editurl: 'ProcessContainerData',
        onAfterSaveCell: reload,
        datatype: 'json',
        colNames: ['ID', 'Container No', 'Seal No', 'Gross Weight', 'Unit', 'Net Weight', 'Unit', 'Measurement', 'Unit', 'UniversalSerialNr'],
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
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                name: 'GrossWeightUnit',
                editable: true,
                edittype: 'select',
                formatter: 'select',
                editoptions: {
                    value: 'KGS:KGS;MT:MT;CBM:CBM;LTR:LTR;GRM:GRM'
                }
            },
            {
                key: false,
                name: 'NettWeight',
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                name: 'NetWeightUnit',
                editable: true,
                edittype: 'select',
                formatter: 'select',
                editoptions: {
                    value: 'KGS:KGS;MT:MT;CBM:CBM;LTR:LTR;GRM:GRM'
                }
            },
            {
                key: false,
                name: 'Measurement',
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                name: 'MeasurementUnit',
                editable: true,
                edittype: 'select',
                formatter: 'select',
                editoptions: {
                    value: 'CBM:CBM;M3:M3'
                }
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
        pager: '#containerPager',
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



    $('#containerGrid').navGrid('#containerPager', { edit: false, add: false, del: true, deltext: "Delete", search: false, refresh: false });

    $('#containerGrid').jqGrid('inlineNav', '#containerPager',
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
        $("#containerGrid").trigger("reloadGrid");
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
        $("#ShipmentDetailsModel_MBLMAWB").attr('readonly', 'readonly');
        $("#ShipmentDetailsModel_CustomerRef").attr('readonly', 'readonly');
        $("#BLDetailsModel_NoOfPkgs").attr('readonly', 'readonly');
        $("#packageDropDownList").prop("disabled", true);
        $("#shipperDropDownList").prop("disabled", true);
        $("#consigneenameblDropDownList").prop("disabled", true);
        $("#ShipmentDetailsModel_ShipperAddress").attr('readonly', 'readonly');
        $("#BLDetailsModel_ConsigneeAddressBL").attr('readonly', 'readonly');
        $("#notifypartynameDropDownList").prop("disabled", true);
        $("#dischagentnameblDropDownList").prop("disabled", true);
        $("#BLDetailsModel_NotifyPartyAddress").attr('readonly', 'readonly');
        $("#ShipmentDetailsModel_DischAgentAddress").attr('readonly', 'readonly');
        $("#closingDatePicker").prop("disabled", true);
        $("#blfinalisedDatePicker").prop("disabled", true);
        $("#blfinalisedDatePicker").datepicker().next('button').hide();
        $("#ladenDatePicker").prop("disabled", true);
        $("#ladenDatePicker").datepicker().next('button').hide();
        $("#issueDatePicker").prop("disabled", true);
        $("#issueDatePicker").datepicker().next('button').hide();
        $("#MarksAndNo").attr('readonly', 'readonly');
        $("#CargoDescription").attr('readonly', 'readonly');
        $("#InvoiceRemark").attr('readonly', 'readonly');
        $("#NoofOriginalBLissuedZero").prop("disabled", true);
        $("#NoofOriginalBLissuedOne").prop("disabled", true);
        $("#NoofOriginalBLissuedThree").prop("disabled", true);
        $("#HBLPrepaidType").prop("disabled", true);
        $("#HBLCollectType").prop("disabled", true);
        $("#MBLPrepaidTypeYes").prop("disabled", true);
        $("#MBLCollectType").prop("disabled", true);
        $("#Allocate").hide();
        $("#Update").hide();
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



    if (housebl != "") {
        $("#Save").hide();
        $("#Update").show();
    }
    else {
        $("#Save").show();
        $("#Update").hide();
        $("#printbl").hide();
        $("#ExportInvoice").hide();
    }

    //ContainerCount();

    

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

    $('#notifypartynameDropDownList').change(function () {
        var selected_val = $('#notifypartynameDropDownList').find(":selected").attr('value');
        var address = $('#BLDetailsModel_NotifyPartyAddress');
        get_add(selected_val, address);
    });

    $('#dischagentnameblDropDownList').change(function () {
        var selected_val = $('#dischagentnameblDropDownList').find(":selected").attr('value');
        var address = $('#ShipmentDetailsModel_DischAgentAddress');
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

function RePrintMethod() {
    $.ajax({
        type: "POST",
        url: 'MailSend',
        data: {
            housebl: housebl,
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
    var ShipperNameSI = $('#shippernamesiDropDownList').find(":selected").attr('value');
    var ShipperAddressSI = $('#BLDetailsModel_ShipperAddressSI').val();
    var ConsigneeNameSI = $('#consigneenamesiDropDownList').find(":selected").attr('value');
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

function allocate() {
    //var SelectedContainerList = $("#ContainerList :selected").
    var SelectedContainerListArray="";
    $("#ContainerList :selected").each(function (i,selected) {
        SelectedContainerListArray += $(selected).text() + ",";
        selected.remove();
    });
    
    var Quantity = $('#ShipmentDetailsModel_Quantity').val();
    var LoadPort = $('#ShipmentDetailsModel_LoadPort').val();
    var DischPort = $('#ShipmentDetailsModel_DischPort').val();
    var LDepotTerminal = $('#ShipmentDetailsModel_LDepotTerminal').val();
    var UniversalSerialNr = usn;
    $.ajax({
        type: "GET",
        url: 'AllocateContainers',
        data: {
            SelectedContainerListArray: SelectedContainerListArray,
            Quantity: Quantity,
            LoadPort: LoadPort,
            DischPort: DischPort,
            LDepotTerminal: LDepotTerminal,
            UniversalSerialNr: UniversalSerialNr
        },
        dataType: "json",
        success: function (data) {            
            $("#ContainerList").multiselect('rebuild');
            //if (data.length > 0) {
            //    $("#ContainerList").appendTo(data[0].Value);
            //}
            //else {
            //    $("#ContainerList").appendTo('');
            //}
            $('#containerGrid').setGridParam({ datatype: "json" }).trigger('reloadGrid');
        }
    });
}

