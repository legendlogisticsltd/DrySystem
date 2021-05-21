$(function ()
{
    $InvoiceDetailsGrid = $('#InvoiceDetailsGrid').jqGrid({
        mtype: 'Get',
        url: 'GetInvoiceLineItems', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['ID', 'Description', 'Quantity', 'Currency', 'Unit Rate', 'Ex-Rate', /*'GST',*/ 'Amount USD'],
        colModel: [
            {
                key: true,
                hidden: true,
                name: 'ID',
                index: 'ID',
            },
            {
                key: false,
                name: 'Description'
            },
            {
                key: false,
                name: 'Quantity'
            },
            {
                key: false,
                name: 'Currency'
            },
            {
                key: false,
                name: 'UnitRate'
            },
            {
                key: false,
                name: 'ExRate'
            },
            //{
            //    key: false,
            //    name: 'GST'
            //},
            {
                key: false,
                name: 'AmountUSD'
            }
        ],
        loadonce: true,
        responsive: true,
        gridview: true,
        autoencode: true,
        pager: '#InvoiceDetailsPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        altRows: true,        
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display',
      
        autowidth: true,
        multiselect: false,

    });


    $('#InvoiceDetailsGrid').navGrid('#InvoiceDetailsPager', { edit: false, add: false, del: false, search: false, refresh: true });


    $("#InvoiceDatePicker").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',



        //yearRange: "-60:+0"
    });
    
    if ($("#InvoiceDatePicker").val() == "01-01-0001") {
        $("#InvoiceDatePicker").datepicker("setDate", new Date());
    }
    $("#InvoiceDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#InvoiceDatePicker").datepicker().show();

    $("#DueDatePicker").datepicker({

        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both',



        //yearRange: "-60:+0"
    });
    
    if ($("#DueDatePicker").val() == "01-01-0001") {
        $("#DueDatePicker").datepicker("setDate", new Date());
    }
    $("#DueDatePicker").datepicker().next('button').button({
        icons: {
            primary: 'ui-icon-calendar'
        },
        text: false
    });

    $("#DueDatePicker").datepicker().show();




});