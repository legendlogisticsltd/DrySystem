$(function () {

    
   
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
        url: 'GetVesselDetails', 
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
        
        autowidth: true,
        multiselect: false,
        rownumbers: true
       
    });

    $containerGrid = $('#containerGrid').jqGrid({
        mtype: 'Get',
        url: 'GetContainerDetails', 
        editurl: 'ProcessContainerData',
        onAfterSaveCell: reload,
        datatype: 'json',
        colNames: ['ID', 'Container No', 'Seal No', 'Gross Weight', 'Unit', 'Net Weight', 'Unit', 'Measurement', 'Unit', 'UniversalSerialNr','Modify'],
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
                align: 'center',
                name: 'ContainerNo',
                index: 'ContainerNo',

                editable: false
            },
            {
                key: false,
                align: 'center',
                name: 'SealNo',
                editable: true
            },
            {
                key: false,
                align: 'center',
                name: 'GrossWeight',
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                align: 'center',
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
                align: 'center',
                name: 'NettWeight',
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                align: 'center',
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
                align: 'center',
                name: 'Measurement',
                editable: true,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
            },
            {
                key: false,
                align: 'center',
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
            },
            {
                name: 'actions', index: 'actions', formatter: 'actions',
                width: '150px',
                formatoptions: {
                    keys: true,
                    size: 10,
                    editbutton: true,
                    delbutton: true,
                    savebutton: true,
                    cancelbutton: true
                }
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
        rownumbers: true,

        loadComplete: function () {
            if (blstatus == "ORIGINAL ISSUED") {
                $(".ui-inline-del").addClass('ui-state-disabled');
                $(".ui-inline-edit").addClass('ui-state-disabled');
            }
        }
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

    $("#ExportInvoice").click(function () {
        if (allocatestatus == "False") {
            alert('Please wait till the Containers are allocated and try again.');
            event.preventDefault();
        }
        else {
            ReloadPage()
        }
    })
    

    if (housebl != "") {
        $("#Save").hide();
        $("#Update").show();
    }
    else {
        $("#Save").show();
        $("#Update").hide();
        $("#printbls").hide();
        $("#ExportInvoice").hide();
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
        $("#ShipmentDetailsModel_ShipperAddress").css('background-color', '#E9ECEF');      
        $("#BLDetailsModel_ConsigneeAddressBL").attr('readonly', 'readonly');
        $("#BLDetailsModel_ConsigneeAddressBL").css('background-color', '#E9ECEF');   
        $("#notifypartynameDropDownList").prop("disabled", true);
        $("#dischagentnameblDropDownList").prop("disabled", true);
        $("#BLDetailsModel_NotifyPartyAddress").attr('readonly', 'readonly');
        $("#BLDetailsModel_NotifyPartyAddress").css('background-color', '#E9ECEF');   
        $("#ShipmentDetailsModel_DischAgentAddress").attr('readonly', 'readonly');
        $("#ShipmentDetailsModel_DischAgentAddress").css('background-color', '#E9ECEF');
        $("#closingDatePicker").prop("disabled", true);
        $("#blfinalisedDatePicker").prop("disabled", true);
        $("#blfinalisedDatePicker").datepicker().next('button').hide();
        $("#ladenDatePicker").prop("disabled", true);
        $("#ladenDatePicker").datepicker().next('button').hide();
        $("#issueDatePicker").prop("disabled", true);
        $("#issueDatePicker").datepicker().next('button').hide();
        $("#MarksAndNo").attr('readonly', 'readonly');
        $("#MarksAndNo").css('background-color', '#E9ECEF');   
        $("#CargoDescription").attr('readonly', 'readonly');
        $("#CargoDescription").css('background-color', '#E9ECEF');   
        $("#InvoiceRemark").attr('readonly', 'readonly');
        $("#InvoiceRemark").css('background-color', '#E9ECEF');   
        $("#NoofOriginalBLissuedZero").prop("disabled", true);
        $("#NoofOriginalBLissuedOne").prop("disabled", true);
        $("#NoofOriginalBLissuedThree").prop("disabled", true);
        $("#HBLPrepaidType").prop("disabled", true);
        $("#HBLCollectType").prop("disabled", true);
        $("#MBLPrepaidTypeYes").prop("disabled", true);
        $("#MBLCollectType").prop("disabled", true);
        $("#Allocate").prop("disabled", true);
        $("#Update").hide();
        $("#ContainerList").multiselect('disable');
        $('#PreCarriage0').prop("disabled",true);
        $('#PreCarriage1').prop("disabled",true);
        $('#PortAliasNo').prop("disabled",true);
        $('#PortAliasYes').prop("disabled",true);
        $('#BLDetailsModel_DischPortAlias').prop("disabled",true);
        $('#BLDetailsModel_LoadPortAlias').prop("disabled",true);
        
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
        if (PrintApproval == "Approval Requested") {
            alert('You have already sent a request for Re-Print BL')
            event.preventDefault();
        }
        else {
            RePrintMethod();
        }
    })

    if (PrintApproval == "Approval Requested") {
        $("#PrintApproval").html('Re-Print Requested');
        $("#PrintApproval").show();
    }
    else if (PrintApproval == "Rejected") {
        $("#PrintApproval").html('Re-Print ' + PrintApproval);
        $("#PrintApproval").show();
    }
    else {
        $("#PrintApproval").hide();
    }


    

    //ContainerCount();
    if ($('#shipperDropDownList').find(":selected").attr('value') != "") {
        var selected_val = $('#shipperDropDownList').find(":selected").attr('value');
        var address = $('#ShipmentDetailsModel_ShipperAddress');
        get_add(selected_val, address);
    }

    

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

    MarksAndNoCount();
    CargoDescriptionCount();
    InvoiceRemarkCount();
    if (transShipmentType.toUpperCase() == "NO") {
        $('#PrecarriageDiv').hide();
    }
    else {
        $('#PrecarriageDiv').show();
        if (preCarriage == "" || preCarriage == null) {
            $('#PreCarriage0').prop("checked", true);
        }
        
    }

    $("#PortAliasYes").click(function () {
        $('#LoadPortLabel').css('display', 'block');
        $('#LoadPortValid').css('display', 'block');
        $("#BLDetailsModel_LoadPortAlias").css('display', 'block');
        $('#DischPortLabel').css('display', 'block');
        $('#DischPortValid').css('display', 'block');
        $("#BLDetailsModel_DischPortAlias").css('display', 'block');
        $('#BLDetailsModel_LoadPortAlias').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Load Port Alias"
                }
            });
        });
        $('#BLDetailsModel_DischPortAlias').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Enter Discharge Port Alias"
                }
            });
        });
    });

    $("#PortAliasNo").click(function () {
        $('#LoadPortLabel').css('display', 'none');
        $('#LoadPortValid').css('display', 'none');
        $("#BLDetailsModel_LoadPortAlias").css('display', 'none');
        $('#DischPortLabel').css('display', 'none');
        $('#DischPortValid').css('display', 'none');
        $("#BLDetailsModel_DischPortAlias").css('display', 'none');
    });

    if (PortAlias.toUpperCase() == "YES") {
        $("#PortAliasYes").trigger('click');
    }
    else {
        $("#PortAliasNo").trigger('click');
    }
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
        //selected.remove();
    });
    
    var Quantity = $('#ShipmentDetailsModel_Quantity').val();
    var LoadPort = $('#ShipmentDetailsModel_LoadPort').val();
    var DischPort = $('#ShipmentDetailsModel_DischPort').val();
    var LDepotTerminal = $('#ShipmentDetailsModel_LDepotTerminal').val();
    var UniversalSerialNr = usn;
    $.ajax({
        type: "POST",
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
            if (data.success == true) {
                $("#ContainerList option:selected").remove();
                $("#ContainerList").multiselect('rebuild');
                $('#containerGrid').setGridParam({ datatype: "json" }).trigger('reloadGrid');
            }
            else {
                alert(data.msg);
            }
        }
    });
}

