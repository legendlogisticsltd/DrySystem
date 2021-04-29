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
       
    });


    $('#vesselGrid').navGrid('#vesselPager', { edit: false, add: false, del: false, search: false, refresh: true });

    $tankGrid = $('#tankGrid').jqGrid({
        mtype: 'Get',
        url: 'GetTankDetails', //'/QuotationDetails/GetQuoteChargesList'
        editurl: 'ProcessTankData',
        datatype: 'json',
        colNames: ['ID', 'Tank No', 'Seal No', 'Gross Weight', 'Net Weight', 'Measurement','UniversalSerialNr'],
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
                name: 'TankNo',
                index: 'TankNo',
                editable: true
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
            add: true,
            addicon: "ui-icon-plus",
            addtext: "Add",
            save: true,
            saveicon: "ui-icon-disk",
            savetext: "Save",
            //saveurl: '/ShipmentDetails/AddTank/',
            cancel: true,
            cancelicon: "ui-icon-cancel",
            canceltext: "Cancel",
            addParams: { position: "last" }
        });

    $("#blfinalisedDatePicker").datepicker().show();
    $("#ETADatePicker").datepicker().show();
    $("#ETDDatePicker").datepicker().show();
    $("#issueDatePicker").datepicker().show();
    $("#ladenDatePicker").datepicker().show();

});

function MarksAndNoCount() {
    var i = document.getElementById("MarksAndNo").value.length;
    document.getElementById("CountingCharactersMarksAndNo").innerHTML =  i;
}

function CargoDescriptionCount() {
    var i = document.getElementById("CargoDescription").value.length;
    document.getElementById("CountingCharactersCargo").innerHTML = i;
}