$(function () {

    $quoteRefGrid = $('#quoteRefGrid').jqGrid({
        mtype: 'Get',        
        url: 'Quotation/GetQuoteRefData',
        datatype: 'json',
        colNames: ['Quote Ref ID', 'Rate ID', 'Agency Name', 'Place Of Receipt', 'Load Port', 'Trans-Shipment Port', 'Discharge Port', 'Place Of Delivery', 'Valid From', 'Valid Till   ', 'Status', 'Quantity', 'Equipment Type',
            'Rate', 'Rate Countered', 'Agent Name'], //'ShipmentTerm','CargoType',  , 'POLFreeDays', 'PODFreeDays',
        //colModel takes the data from controller and binds to grid   
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'QuoteRefID',
                index: 'QuoteRefID',
                classes: 'myLink',
                width: '150px',
                formatter: 'showlink',
                formatoptions: {
                    baseLinkUrl: 'QuotationDetails/QuotationDetails',
                    idName: 'QuoteRefID'
                }
            },
            {
                key: true,
                //hidden: true,
                name: 'RateID',
                index: 'RateID',
                width: '85px'
            },
            {
                key: false,
                name: 'CompanyName',
                width: '219px'
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
                width: '124px'
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
                width: '111px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'ValidityDate ',
                formatter: 'date',
                width: '111px',
                formatoptions: {
                    srcformat: 'm-d-Y',
                    newformat: 'm-d-Y'
                }
            },
            {
                key: false,
                name: 'StatusDIS',
                width:'94px'
            },
            {
                key: false,
                name: 'Quantity',
                width: '100px'
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
                width: '65px'
            },
            {
                key: false,
                name: 'RateCountered',
                formatter: 'number',
                formatoptions: {
                    decimalPlaces: 2
                },
                width: '150px'
            },
            {
                key: false,
                name: 'AgentName',
                width:'125px'
            }],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#quoteRefDataPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,
        altclass: 'ui-priority-secondary',
        loadtext: 'Loading Data please wait ...',
        //caption: 'User Details',
        emptyrecords: 'No records to display',
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
    }).navGrid('#quoteRefDataPager', { edit: false, add: false, del: false, search: true, refresh: true });

    function reloadGrid(refresh) {
        if (refresh == true) {
            //Set initial values of your filters here
            //$("#txtId").val("");
            //$("#txtEmployeeName").val("");

            //reload grid's page 1 instead of current page
            $("#quoteRefGrid").trigger("reloadGrid", { page: 1 });
        }
        else {
            $("#quoteRefGrid").trigger("reloadGrid");
        }
    }


    var $s = $("#statusDropDownList").selectmenu();//.selectmenu("menuWidget").addClass("overflow");
    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

});