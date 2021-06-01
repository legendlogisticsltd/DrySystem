$(function () {

    $shipmentGrid = $('#shipmentGrid').jqGrid({
        mtype: 'Get',
        url: 'DischargePlan/GetDischargePlan',
        datatype: 'json',
        colNames: ['Job Ref', 'IDNo', 'Load Port', 'Discharge Port', 'Country', 'Quantity', 'DischargePlanStatus', 'ATA'],
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
                key: true,
                //hidden: true,
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
                name: 'Country'
            },
            {
                key: false,
                name: 'Quantity'
            },
            {
                key: false,
                name: 'Status'
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
        altclass: 'shipmentRow',
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

    var $s = $("#statusDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //var $s = $("#companyDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
});