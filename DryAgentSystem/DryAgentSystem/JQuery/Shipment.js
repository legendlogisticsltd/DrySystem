$(function () {

    $shipmentGrid = $('#shipmentGrid').jqGrid({
        mtype: 'Get',
        url: 'Shipment/GetShipmentData', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['JobRef', 'Charge Party', 'Booking No', 'QuoteRef ID', 'Shipment Status', 'Discharge Port', 'Load Port'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'JobRef',
                index: 'JobRef',
                width: '177px',
                classes: 'myLink',
                formatter: 'showlink',
                formatoptions: {
                    baseLinkUrl: 'ShipmentDetails/ShipmentDetails',
                    idName: 'JobRef'
                }
            },
            {
                key: false,
                name: 'ChargeParty',
                width: '335px',
                
            },
            {
                key: false,
                name: 'BookingNo',
                width:'159px',
                
            },
            {
                key: false,
                name: 'IDQuoteRef'
            },
            {
                key: false,
                name: 'BLTypes'
            },
            {
                key: false,
                name: 'DischPort'
            },
            {
                key: false,
                name: 'LoadPort'
            }
        ],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#shipmentPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,
        loadtext: 'Loading Data please wait ...',
        rownumbers: true,
        emptyrecords: 'No records to display',
        
    });

    
    
});