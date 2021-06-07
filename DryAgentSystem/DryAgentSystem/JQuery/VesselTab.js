$(function () {

    //$("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //$("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    $vesselGrid = $('#vesselGrid').jqGrid({
        mtype: 'Get',
        url: 'GetVesselDetails', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['ID', 'Vessel Name', 'Voy No', 'Load Port', 'Discharge Port', 'ETD POL', 'ETA POD', 'Carrier', 'UniversalSerialNr', 'CarrierBookingRefNo','Delete'],
        colModel: [
            {
                key: true,
                hidden: true,
                name: 'ID',
                index: 'ID',
                width: '100px',
                align: 'center',
            },
            {
                key: false,
                name: 'VesselName',
                align: 'center',
            },
            {
                key: false,
                name: 'VoyNo',
                align: 'center',
            },
            {
                key: false,
                name: 'LoadPort',
                align: 'center',
            },
            {
                key: false,
                name: 'DischPort',
                align: 'center',
            },
            {
                key: false,
                name: 'ETD',
                align: 'center',
                formatter: 'date',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ETA',
                align: 'center',
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
            //{
            //    key: false,
            //    hidden: true,
            //    name: 'DateATA',
            //    formatter: 'date',
            //    formatoptions: {
            //        srcformat: 'm-d-Y',
            //        newformat: 'm-d-Y'
            //    }
            //},
            //{
            //    key: false,
            //    hidden: true,
            //    name: 'DateSOB',
            //    formatter: 'date',
            //    formatoptions: {
            //        srcformat: 'm-d-Y',
            //        newformat: 'm-d-Y'
            //    }
            //},
            {
                key: false,
                hidden: true,
                name: 'UniversalSerialNr'
            },
            {
                key: false,
                hidden: true,
                name: 'CarrierBookingRefNo'
            },
            {
                name: 'actions', index: 'actions', formatter: 'actions',
                width: '60px',
                formatoptions: {
                    keys: true,
                    editbutton: false,
                    delbutton: true,
                    delOptions: { url: 'DeleteVessel' }
                }
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
        rownumbers: true,
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
        autowidth: false,
        shrinkToFit: true,
        multiselect: false,
        onSelectRow: function (id) {
            
            if (id != null) {
                var selRowId = $vesselGrid.jqGrid('getGridParam', 'selrow');
                var ID = $vesselGrid.jqGrid('getCell', selRowId, 'ID');
                var VesselName = $vesselGrid.jqGrid('getCell', selRowId, 'VesselName');
                var VoyNo = $vesselGrid.jqGrid('getCell', selRowId, 'VoyNo');
                var LoadPort = $vesselGrid.jqGrid('getCell', selRowId, 'LoadPort');
                var DischPort = $vesselGrid.jqGrid('getCell', selRowId, 'DischPort');
                var ETD = $vesselGrid.jqGrid('getCell', selRowId, 'ETD');
                var ETA = $vesselGrid.jqGrid('getCell', selRowId, 'ETA');
                var Carrier = $vesselGrid.jqGrid('getCell', selRowId, 'Carrier');
                //var DateATA = $vesselGrid.jqGrid('getCell', selRowId, 'DateATA');
                //var DateSOB = $vesselGrid.jqGrid('getCell', selRowId, 'DateSOB');
                var UniversalSerialNr = $vesselGrid.jqGrid('getCell', selRowId, 'UniversalSerialNr');
                var CarrierBookingRefNo = $vesselGrid.jqGrid('getCell', selRowId, 'CarrierBookingRefNo');

                //DateATA = $.trim(DateATA);
                //DateSOB = $.trim(DateSOB);

                $('#Carrier').val(Carrier);
                $('#VesselName').val(VesselName);
                $('#VoyNo').val(VoyNo);
                $('#loadportDropDownList').val(LoadPort);
                $('#loadportDropDownList').prop('selected', 'selected');
                $('#dischportDropDownList').val(DischPort);
                $('#dischportDropDownList').prop('selected', 'selected');
                $('#ETA').val(ETA);
                $('#ETD').val(ETD);
                //$('#DateATA').val(DateATA);
                //$('#DateSOB').val(DateSOB);
                $('#UniversalSerialNr').val(UniversalSerialNr);
                $('#ID').val(ID);
                $('#CarrierBookingRefNo').val(CarrierBookingRefNo);
                if (VesselName == "TRUCKING") {
                    $("#TransportTrucking").trigger('click');
                }
                else {
                    $("#TransportSeaways").trigger('click');
                }
            }
        },
        loadComplete: function () {
            if ((BookingStatus == "CONFIRMED") || (BookingStatus == "ISSUED")) {
                $(".ui-inline-del").addClass('ui-state-disabled');
            }
            
        }
    });

    
    $('#vesselGrid').navGrid('#vesselPager', { edit: false, add: false, del: false, search: false, refresh: false });

    $("#TransportSeaways").prop("checked", true);

    $("#ETA").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',
        //yearRange: "-60:+0"
    });

    if ($("#ETA").val() == "01-01-0001") {
        $("#ETA").datepicker("setDate", new Date());
    }
    $("#ETA").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#ETA").datepicker().show();



    $("#ETD").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',

        //yearRange: "-60:+0"
    });

    //var today = new Date();
    //var lastOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    if ($("#ETD").val() == "01-01-0001") {
        $("#ETD").datepicker("setDate", new Date());
    }

    $("#ETD").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });
    $("#ETD").datepicker().show();

    

    function ReloadGrid() {
        $("#vesselGrid").setGridParam({ page: 1 }).trigger("reloadGrid");
        //$("#vesselGrid").jqGrid("setGridParam", { datatype: "json" })
        //    .trigger("reloadGrid", [{ current: true }]);
    }

    //$("#vesselForm    ").submit(function (event) {
    //    event.preventDefault();
    //}

    //$('#CreateVessel').click(function (event) {
    //    event.preventDefault();
    //    event.stopImmediatePropagation();
    //    $('#vesselForm').submit();

    //    $.ajax({
    //        url: 'VesselDetailsTab',
    //        type: 'POST',
    //        dataType: 'json',
    //        data: $("#vesselForm").serialize(),
    //        success: function (data) {
    //            $('#vessel').html(data);
    //        }
    //    });
    //});
    
    //$("#submit").click(function () {
    //    document.forms[0].submit();
    //    return false;
    //});

    if (BookingStatus == "CONFIRMED") {

        $("#CreateVessel").hide();
        $("#UpdateVessel").hide();
        $("#DeleteVessel").hide();
        $("#CarrierBookingRefNo").attr('readonly', 'readonly');
        $("#Carrier").attr('readonly', 'readonly');
        $("#VesselName").attr('readonly', 'readonly');
        $("#VoyNo").attr('readonly', 'readonly');
        $("#ETD").prop("disabled", true);
        $("#ETD").datepicker().next('button').hide();
        $("#ETA").prop("disabled", true);
        $("#ETA").datepicker().next('button').hide();
        //$("#DateSOB").prop("disabled", true);
        //$("#DateATA").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#TransportTrucking").prop("disabled", true);
        $("#TransportSeaways").prop("disabled", true);
    }
    if (BookingStatus == "ISSUED") {

        $("#CreateVessel").hide();
        $("#UpdateVessel").hide();
        $("#DeleteVessel").hide();
        $("#CarrierBookingRefNo").attr('readonly', 'readonly');
        $("#Carrier").attr('readonly', 'readonly');
        $("#VesselName").attr('readonly', 'readonly');
        $("#VoyNo").attr('readonly', 'readonly');
        $("#ETD").prop("disabled", true);
        $("#ETD").datepicker().next('button').hide();
        $("#ETA").prop("disabled", true);
        $("#ETA").datepicker().next('button').hide();
        //$("#DateSOB").prop("disabled", true);
        //$("#DateATA").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        $("#TransportTrucking").prop("disabled", true);
        $("#TransportSeaways").prop("disabled", true);
    }

    $('#CreateVessel').click(function () {
        
        var LoadPort = $('#loadportDropDownList').find(":selected").attr('value');
        var CarrierBookingRefNo = $('#CarrierBookingRefNo').val();
        var Carrier = $('#Carrier').val();
        var VesselName = $('#VesselName').val();
        var VoyNo = $('#VoyNo').val();
        var DischPort = $('#dischportDropDownList').find(":selected").attr('value');
        var valid = "The following fields are missing value.\nPlease check ";
        if (VesselName != "TRUCKING") {
            if ((LoadPort == "") || (CarrierBookingRefNo == "") || (Carrier == "") || (VesselName == "") || (VoyNo == "") || (DischPort == "")) {

                if (Carrier == "") {
                    valid += "Carrier. ";
                }
                if (CarrierBookingRefNo == "") {
                    valid += "CarrierBookingRefNo. ";
                }
                if (VesselName == "") {
                    valid += "VesselName. ";
                }
                if (VoyNo == "") {
                    valid += "VoyNo. ";
                }
                if (LoadPort == "") {
                    valid += "LoadPort. ";
                }
                if (DischPort == "") {
                    valid += "DischPort. ";
                }

                alert(valid);
                event.preventDefault();
            }
            else {
                var url = 'CreateVessel';
                vesselentry(url);
            }
        }
        else {
            if ((LoadPort == "") || (DischPort == "")) {

                if (LoadPort == "") {
                    valid += "LoadPort ";
                }
                
                if (DischPort == "") {
                    valid += "DischPort ";
                }

                alert(valid);
                event.preventDefault();
            }
            else {
                var url = 'CreateVessel';
                vesselentry(url);
            }
        }
        
        
        
    });

    $('#UpdateVessel').click(function () {
        var LoadPort = $('#loadportDropDownList').find(":selected").attr('value');
        var CarrierBookingRefNo = $('#CarrierBookingRefNo').val();
        var Carrier = $('#Carrier').val();
        var VesselName = $('#VesselName').val();
        var VoyNo = $('#VoyNo').val();
        var DischPort = $('#dischportDropDownList').find(":selected").attr('value');
        var valid = "The following fields are missing value.\nPlease check ";
        if (VesselName != "TRUCKING") {
            if ((LoadPort == "") || (CarrierBookingRefNo == "") || (Carrier == "") || (VesselName == "") || (VoyNo == "") || (DischPort == "")) {

                if (Carrier == "") {
                    valid += "Carrier. ";
                }
                if (CarrierBookingRefNo == "") {
                    valid += "CarrierBookingRefNo. ";
                }
                if (VesselName == "") {
                    valid += "VesselName. ";
                }
                if (VoyNo == "") {
                    valid += "VoyNo. ";
                }
                if (LoadPort == "") {
                    valid += "LoadPort. ";
                }
                if (DischPort == "") {
                    valid += "DischPort. ";
                }

                alert(valid);
                event.preventDefault();
            }
            else {
                var url = 'UpdateVessel';
                vesselentry(url);
            }
        }
        else {
            if ((LoadPort == "") || (DischPort == "")) {

                if (LoadPort == "") {
                    valid += "LoadPort ";
                }

                if (DischPort == "") {
                    valid += "DischPort ";
                }

                alert(valid);
                event.preventDefault();
            }
            else {
                var url = 'UpdateVessel';
                vesselentry(url);
            }
        }



        
    });

    

    $("#TransportSeaways").click(function () {

        $("#CarrierLabel, #Carrier, #CarrierLabelAstrik,#CarrierBookingRefNoLabel, #CarrierBookingRefNo, #VoyNoLabel, #VoyNo, #VoyNoLabelAstrik").removeClass("HideAndShow");
        
        $('#VesselName').removeAttr('readonly', 'readonly');
        if ($('#VesselName').val() == "TRUCKING") {
            $('#VesselName').val('');
        }
        $('#LoadPortLabel').html('Load Port');
        $('#DischPortLabel').html('Discharge Port');
        
    });

    $("#TransportTrucking").click(function () {
        $("#CarrierLabel, #Carrier, #CarrierLabelAstrik,#CarrierBookingRefNoLabel, #CarrierBookingRefNo, #VoyNoLabel, #VoyNo, #VoyNoLabelAstrik").addClass("HideAndShow");
        $('#Carrier').val('');
        $('#CarrierBookingRefNo').val('');
        $('#VoyNo').val('');
        $('#VesselName').attr('readonly', 'readonly');
        $('#VesselName').val('TRUCKING');
        $('#LoadPortLabel').html('From');
        $('#DischPortLabel').html('To');
    });

   
});

function vesselentry(url) {    
    var LoadPort = $('#loadportDropDownList').find(":selected").attr('value');
    var CarrierBookingRefNo = $('#CarrierBookingRefNo').val();
    var Carrier = $('#Carrier').val();
    var VesselName = $('#VesselName').val();
    var VoyNo = $('#VoyNo').val();
    var DischPort = $('#dischportDropDownList').find(":selected").attr('value');
    var ETA = $('#ETA').val();
    var ETD = $('#ETD').val();
    var UniversalSerialNr = $('#UniversalSerialNr').val();
    var ID = $('#ID').val();
    if (VesselName == 'TRUCKING') {
        var TransportMode = VesselName;
    }
    else {
        var TransportMode = 'SEAWAYS'
    }
    debugger;
    $.ajax({
        type: "POST",
        url: url,
        data: {
            LoadPort: LoadPort,
            CarrierBookingRefNo: CarrierBookingRefNo,
            Carrier: Carrier,
            VesselName: VesselName,
            VoyNo: VoyNo,
            DischPort: DischPort,
            ETA: ETA,
            ETD: ETD,
            UniversalSerialNr: UniversalSerialNr,
            ID: ID,
            TransportMode: TransportMode
        },
        dataType: "json",
        success: function (result) {
            alert(result.msg);//You can also not pop up a prompt box.You could code anything what you want   
            $('#vesselGrid').setGridParam({ datatype: "json" }).trigger('reloadGrid');
        }
    });
}