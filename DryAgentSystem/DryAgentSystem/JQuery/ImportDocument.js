
$(function () {

    $userDetailGrid = $('#BLDataGrid').jqGrid({
        mtype: 'Get',
        url: 'ImportDocument/GetBLData',
        datatype: 'json',
        colNames: ['Job Ref', 'BL Types', 'Shipper NameBL', 'Consignee NameBL', 'Discharge Port', 'Load Port', 'Country POL'],
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
                    baseLinkUrl: 'DischargePlanDetails/DischargePlanDetails',
                    idName: 'JobRef'
                }
            },
            {
                key: false,
                name: 'BLTypes',
                width: '100px'
            },
            {
                key: false,
                name: 'ShipperNameBL',
                width: '357px'
            },
            {
                key: false,
                name: 'ConsigneeNameBL',
                width: '357px'
            },
            {
                key: false,
                name: 'LoadPort',
                width: '124px'
            },
            {
                key: false,
                name: 'DischargePort',
                width: '150px'
            },
            {
                key: false,
                name: 'CountryPOL',
                width: '200px'
            }
        ],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#BLDataPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,
        //altclass: 'ui-priority-secondary', //'altGridRowClass',
        loadtext: 'Loading Data please wait ...',
        //caption: 'User Details',
        emptyrecords: 'No records to display'
    }); 

    $('#CompanyNameLabel').html('Consignee Name');
});