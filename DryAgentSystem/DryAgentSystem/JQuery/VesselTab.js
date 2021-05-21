$(function () {

    //$("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //$("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

    $vesselGrid = $('#vesselGrid').jqGrid({
        mtype: 'Get',
        url: 'GetVesselDetails', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['ID', 'Vessel Name', 'Voy No', 'Load Port', 'Discharge Port', 'ETD POL', 'ETA POD', 'Carrier', 'UniversalSerialNr', 'CarrierBookingRefNo'],
        colModel: [
            {
                key: true,
                //hidden: true,
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
            }
        }
    });


    $('#vesselGrid').navGrid('#vesselPager', { edit: false, add: false, del: false, search: false, refresh: true });


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

    //$("#DateSOB").datepicker({
    //    dateFormat: "mm-dd-yy",
    //    changeMonth: true,
    //    changeYear: true,
    //    showOn: 'both',

    //    //yearRange: "-60:+0"
    //});
    //if ($("#DateSOB").val() == "01-01-0001") {
    //    $("#DateSOB").datepicker("setDate","");
    //}

    //$("#DateSOB").datepicker().next('button').button({
    //    icons: {
    //        primary: 'ui-icon-calendar'
    //    },
    //    text: false
    //});
    //$("#DateSOB").datepicker().show();

    //$("#DateATA").datepicker({
    //    dateFormat: "mm-dd-yy",
    //    changeMonth: true,
    //    changeYear: true,
    //    showOn: 'both',

    //    //yearRange: "-60:+0"
    //});
    //if ($("#DateATA").val() == "01-01-0001") {
    //    $("#DateATA").datepicker("setDate", null);
    //}

    //$("#DateATA").datepicker().next('button').button({
    //    icons: {
    //        primary: 'ui-icon-calendar'
    //    },
    //    text: false
    //});
    //$("#DateATA").datepicker().show();

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
        $("#ETA").prop("disabled", true);
        //$("#DateSOB").prop("disabled", true);
        //$("#DateATA").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);
        
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
        $("#ETA").prop("disabled", true);
        //$("#DateSOB").prop("disabled", true);
        //$("#DateATA").prop("disabled", true);
        $("#loadportDropDownList").prop("disabled", true);
        $("#dischportDropDownList").prop("disabled", true);

    }


});