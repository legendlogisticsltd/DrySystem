$(function () {

    $bookingGrid = $('#bookingGrid').jqGrid({
        mtype: 'Get',
        url: 'Booking/GetBookingData', 
        datatype: 'json',
        colNames: ['Booking ID', 'Booking No', 'QuoteRef ID', 'Agency Name', 'Discharge Port', 'Load Port', 'Booking Status'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'BookingID',
                index: 'BookingID',
                width: '92px',
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
                name: 'CompanyName',
                width: '259px',
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
        
    });

    
    
});