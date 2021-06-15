$(function () {

    $shipmentGrid = $('#dischargePlanGrid').jqGrid({
        mtype: 'Get',
        url: 'DeliveryOrder/GetDeliveryOrder',
        datatype: 'json',
        colNames: ['Job Ref', 'Charge Party', 'ATA', 'ETA', 'ETD', 'Load Port', 'Discharge Port', 'DischargePlanStatus'],
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
                    baseLinkUrl: 'DeliveryOrderDetails/DeliveryOrderDetails',
                    idName: 'JobRef'
                }
            },
            {
                key: false,
                name: 'ChargeParty',
                width: '272px'
            },
            {
                key: false,
                name: 'ATA',
                formatter: 'date',
                width: '140px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },            
            {
                key: false,
                name: 'ETA',
                formatter: 'date',
                width: '140px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ETD',
                formatter: 'date',
                width: '140px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
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
                name: 'Status'
            }
        ],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#dPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,
        //altclass: 'shipmentRow',
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display'
    });

    $('#CompanyNameLabel').html('Charge Party');
});