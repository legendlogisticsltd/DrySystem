$(function () {

    $shipmentGrid = $('#shipmentGrid').jqGrid({
        mtype: 'Get',
        url: 'DischargePlan/GetDischargePlan',
        datatype: 'json',
        colNames: ['Job Ref','Charge Party', 'IDNo', 'Load Port', 'Discharge Port', 'Quantity', 'DischargePlanStatus', 'ATA', 'ETA', 'ETD', 'Created By'],
        colModel: [            
            {
                key: true,
                name: 'JobRef',
                index: 'JobRef',
                width: '177px',
                classes: 'myLink',
                formatter: 'showlink',
                formatoptions: {
                    baseLinkUrl: 'DischargePlanDetails/UpdateDischargePlanDetails',
                    idName: 'JobRef'
                }
            },
            {
                key: false,
                name: 'ChargeParty',
                width: '289px'
            },
            {
                key: true,
                hidden: true,
                name: 'Id',
                index: 'IDNo',
                width: '50px',
                //classes: 'myLink',
                //formatter: 'showlink',
                //formatoptions: {
                //    baseLinkUrl: 'DischargePlanDetails/UpdateDischargePlanDetails',
                //    idName: 'IDNo'
                //}
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
                name: 'Quantity',
                width: '75px'
            },
            {
                key: false,
                name: 'DischargePlanStatus'
            },
            {
                key: false,
                name: 'ATA',
                formatter: 'date',
                width: '103px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ETA',
                formatter: 'date',
                width: '103px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ETD',
                formatter: 'date',
                width: '103px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'User'
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
        //altclass: 'shipmentRow',
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display',
        
    });

    $('#CompanyNameLabel').html('Charge Party');
});