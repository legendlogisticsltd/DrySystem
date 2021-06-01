
$(function () {

    $userDetailGrid = $('#BLDataGrid').jqGrid({
        mtype: 'Get',
        url: 'ImportDocument/GetBLData',
        datatype: 'json',
        colNames: ['Job Ref', 'BL Types', 'Shipper NameBL', 'Consignee NameBL', 'Discharge Port', 'Load Port', 'Country POL',
            'Placeo of Delivery', 'Cargo Description'],
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
                width: '200px'
            },
            {
                key: false,
                name: 'ConsigneeNameBL',
                width: '200px'
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
            },
            {
                key: false,
                name: 'PlaceofDelivery',
                width: '200px'
            },
            {
                key: false,
                name: 'CargoDescription',
                width: '300px'
            },
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
        altclass: 'ui-priority-secondary', //'altGridRowClass',
        loadtext: 'Loading Data please wait ...',
        //caption: 'User Details',
        emptyrecords: 'No records to display'
    }).navGrid('#BLDataPager', { edit: false, add: false, del: false, search: false, refresh: true });


    function reloadGrid(refresh) {
        if (refresh == true) {
            //Set initial values of your filters here
            //$("#txtId").val("");
            //$("#txtEmployeeName").val("");

            //reload grid's page 1 instead of current page
            $("#BLDataGrid").trigger("reloadGrid", { page: 1 });
        }
        else {
            $("#BLDataGrid").trigger("reloadGrid");
        }
    }

    var $s = $("#statusDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

});



//$(function () {

//    $userDetailGrid = $('#BLDataGrid').jqGrid({
//        mtype: 'Get',
//        url: 'ImportDocument/GetBLData',
//        datatype: 'json',
//        colNames: ['Job Ref', 'BL Types', 'Shipper NameBL', 'Consignee NameBL', 'Load Port', 'Discharge Port', 'Shipper AddressBL',
//            'Consignee AddressBL', 'Notify Party Name', 'Disch Agent NameBL', 'Notify Party Address', 'Disch Agent Address', 'Country POL',
//            'Placeo of Delivery', 'Marks/No', 'Cargo Description', 'Place of Issue', 'No of Original BL issued'],
//        colModel: [
//            {
//                key: true,
//                //hidden: true,
//                name: 'JobRef',
//                index: 'JobRef',
//                width: '177px',
//                classes: 'myLink',
//                formatter: 'showlink',
//                formatoptions: {
//                    baseLinkUrl: 'ShipmentDetails/ShipmentDetails',
//                    idName: 'JobRef'
//                }
//            },
//            {
//                key: false,
//                name: 'BLTypes',
//                width: '100px'
//            },
//            {
//                key: false,
//                name: 'ShipperNameBL',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'ConsigneeNameBL',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'LoadPort',
//                width: '124px'
//            },
//            {
//                key: false,
//                name: 'DischargePort',
//                width: '150px'
//            },
//            {
//                key: false,
//                name: 'ShipperAddressBL',
//                width: '400px'
//            },
//            {
//                key: false,
//                name: 'ConsigneeAddressBL',
//                width: '400px'
//            },
//            {
//                key: false,
//                name: 'NotifyPartyName',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'DischAgentNameBL',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'NotifyPartyAddress',
//                width: '400px'
//            },
//            {
//                key: false,
//                name: 'DischAgentAddress',
//                width: '400px'
//            },
//            {
//                key: false,
//                name: 'CountryPOL',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'PlaceofDelivery',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'MarksandNo',
//                width: '80px'
//            },
//            {
//                key: false,
//                name: 'CargoDescription',
//                width: '300px'
//            },
//            {
//                key: false,
//                name: 'PlaceofIssue',
//                width: '200px'
//            },
//            {
//                key: false,
//                name: 'NoofOriginalBLissued',
//                width: '200px'
//            }
//        ],
//        loadonce: true,
//        responsive: true,
//        shrinkToFit: false,
//        //forceFit: true,
//        autowidth: true,
//        gridview: true,
//        autoencode: true,
//        pager: '#BLDataPager',
//        rowNum: 10,
//        rowList: [10, 20, 30, 40],
//        height: 'auto',
//        viewrecords: true,
//        multiselect: false,
//        altRows: true,
//        altclass: 'ui-priority-secondary', //'altGridRowClass',
//        loadtext: 'Loading Data please wait ...',
//        //caption: 'User Details',
//        emptyrecords: 'No records to display'
//    }).navGrid('#BLDataPager', { edit: false, add: false, del: false, search: false, refresh: true });


//    function reloadGrid(refresh) {
//        if (refresh == true) {
//            //Set initial values of your filters here
//            //$("#txtId").val("");
//            //$("#txtEmployeeName").val("");

//            //reload grid's page 1 instead of current page
//            $("#BLDataGrid").trigger("reloadGrid", { page: 1 });
//        }
//        else {
//            $("#BLDataGrid").trigger("reloadGrid");
//        }
//    }

//    var $s = $("#statusDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
//    var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
//    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
//    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");

//});