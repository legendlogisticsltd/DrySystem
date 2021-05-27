$(function () {

    $userDetailGrid = $('#rateRequestDataGrid').jqGrid({
        mtype: 'Get',
        //url: '@Url.Action("GetUserDetails","Portal")',
        url: 'RateRequest/GetRateRequestData',
        datatype: 'json',
        colNames: ['Rate ID', 'Agency Name', 'Place of Receipt', 'Load Port', 'Trans-Shipment Port', 'Discharge Port', 'Place of Delivery', 'Valid From', 'Valid Till', 'Status', 'Quantity', 'Equipment Type', 'Rate',
            'Rate Countered', 'Agent Name'], //'ShipmentTerm','CargoType''PlaceOfReceipt', 'PlaceOfDelivery', 'POLFreeDays', 'PODFreeDays',
        //colModel takes the data from controller and binds to grid   
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'RateID',
                index: 'RateID',
                //align: 'center',
                classes: 'myLink',
                formatter: 'showlink',
                width: '85px',
                formatoptions: {
                    baseLinkUrl: 'RateRequestDetails/UpdateRateRequestDetails',
                    idName: 'RateID'
                }
            },
            {
                key: false,
                name: 'CompanyName',
                width: '280px'
            },
            //{
            //    key: false,
            //    name: 'ShipmentTerm'
            //},
            {
                key: false,
                name: 'PlaceOfReceipt'
            },
            {
                key: false,
                name: 'LoadPort',
                width: '124px'
            },
            {
                key: false,
                name: 'TransshipmentPort'
            },
            {
                key: false,
                name: 'DischargePort',
                width: '150px'
            },
            
            {
                key: false,
                name: 'PlaceOfDelivery'
            },
            //{
            //    key: false,
            //    name: 'POLFreeDays'
            //},
            //{
            //    key: false,
            //    name: 'PODFreeDays'
            //},
            {
                key: false,
                name: 'EffectiveDate',
                formatter: 'date',
                width: '140px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ValidityDate ',
                formatter: 'date',
                width: '130px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'Status'
            },
            {
                key: false,
                name: 'Quantity',
                width: '95px'
            },
            {
                key: false,
                name: 'EquipmentType',
                width: '155px'
            },
            //{
            //    key: false,
            //    name: 'CargoType'
            //},
            {
                key: false,
                name: 'Rate',
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
                width: '75px'
            },
            {
                key: false,
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
                name: 'RateCountered'
            },
            {
                key: false,
                name: 'AgentName',
                width: '130px'

            }],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#rateRequestDataPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,
        //altclass: 'ui-priority-secondary',
        loadtext: 'Loading Data please wait ...',
        //caption: 'User Details',
        emptyrecords: 'No records to display',
        rownumbers: true,
        //jsonReader:
        //{
        //    root: "rows",
        //    total: "total",
        //    records: "records",
        //    page: "page",
        //    repeatitems: false,
        //    Id: 'ID'
        //},


        //pager-you have to choose here what icons should appear at the bottom  
        //like edit,create,delete icons  
    }).navGrid('#rateRequestDataPager', { edit: false, add: false, del: false, search: false, refresh: true });


    function reloadGrid(refresh) {
        if (refresh == true) {
            //Set initial values of your filters here
            //$("#txtId").val("");
            //$("#txtEmployeeName").val("");

            //reload grid's page 1 instead of current page
            $("#rateRequestDataGrid").trigger("reloadGrid", { page: 1 });
        }
        else {
            $("#rateRequestDataGrid").trigger("reloadGrid");
        }
    }

});