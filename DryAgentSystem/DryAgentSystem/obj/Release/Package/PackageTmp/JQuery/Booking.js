$(function () {

    $bookingGrid = $('#bookingGrid').jqGrid({
        mtype: 'Get',
        url: 'Booking/GetBookingData', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['Booking ID', 'Booking No', 'QuoteRef ID', 'Agency Name', 'Discharge Port', 'Load Port', 'Booking Status'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'BookingID',
                index: 'BookingID',
                width: '88px',
                classes: 'myLink',
                formatter: 'showlink',
                formatoptions: {
                    baseLinkUrl: 'BookingDetails/BookingDetails',
                    idName: 'BookingID'
                }
            },
            {
                key: false,
                name: 'BookingNo',
                width:'159px',
                
            },
            {
                key: false,
                name: 'QuoteRefID'
            },
            {
                key: false,
                name: 'CompanyName'
            },
            {
                key: false,
                name: 'DischargePort'
            },
            {
                key: false,
                name: 'LoadPort'
            },
            {
                key: false,
                name: 'BookingStatus'
            }
        ],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#bookingPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        rownumbers: true,
        viewrecords: true,
        multiselect: false,
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
    });

    //.navGrid('#quoteChargesPager', { edit: false, add: false, del: false, search: false, refresh: true });



    //$("#submit").click(function () {
    //    document.forms[0].submit();
    //    return false;
    //});
    var $s = $("#statusDropDownList").selectmenu();
    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
});