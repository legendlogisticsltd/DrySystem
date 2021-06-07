$(function () {

    $InvoiceGrid = $('#InvoiceGrid').jqGrid({
        mtype: 'Get',
        url: 'Invoice/GetInvoiceData', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['Invoice No.', 'JobRef No', 'Company Name', 'Load Port', 'Discharge Port', 'Status'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'InvoiceNo',
                index: 'InvoiceNo',
                width: '177px',
                classes: 'myLink',
                formatter: 'showlink',
                formatoptions: {
                    baseLinkUrl: 'InvoiceDetails/InvoiceDetails',
                    idName: 'InvoiceNo'
                }
            },
            {
                key: false,
                name: 'JobRefNo',
                width: '335px',

            },
            {
                key: false,
                name: 'CompanyName',
                width: '159px',

            },
            {
                key: false,
                name: 'LoadPort'
            },
            {
                key: false,
                name: 'DischargePort'
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
        pager: '#InvoicePager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        altRows: true,        
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display',
    });
      
    //var $s = $("#statusDropDownList").selectmenu();
    //var $s = $("#loadportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
    //var $s = $("#dischportDropDownList").selectmenu().selectmenu("menuWidget").addClass("overflow");
});