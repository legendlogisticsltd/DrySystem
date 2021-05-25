
$(function () {

    
    $quoteChargesGrid = $('#quoteChargesGrid').jqGrid({
        mtype: 'Get',
        url: 'GetQuoteChargesList', //'/QuotationDetails/GetQuoteChargesList'
        datatype: 'json',
        colNames: ['ID', 'Description'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'ID',
                hidden: true,
                index: 'ID',
                width: '50px'
            },
            {
                key: false,
                name: 'Description',
                width: '200px'
            }
        ],
        loadonce: true,
        responsive: true,

        //autowidth: true,
        gridview: true,
        autoencode: true,
        //pager: '#quoteChargesPager',
        //rowNum: 10,
        //rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: false,
        rownumbers: true,
        altRows: true,
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display'
    });

    //.navGrid('#quoteChargesPager', { edit: false, add: false, del: false, search: false, refresh: true });

    if (rateCountered <= 0) {
        $("#rateCounteredLabel").hide();
        $("#RateCountered").hide();
    }

    $("#StatusDIS").attr('readonly', 'readonly');
    $("#QuoteRefID").attr('readonly', 'readonly');
    $("#RateID").attr('readonly', 'readonly');
    $("#RateID").attr('readonly', 'readonly');
    $("#Rate").attr('readonly', 'readonly');
    $("#Quantity").attr('readonly', 'readonly');
    $("#QuantityLifted").attr('readonly', 'readonly');
    $("#RateCountered").attr('readonly', 'readonly');
    $("#EquipmentType").attr('readonly', 'readonly');
    $("#LoadPort").attr('readonly', 'readonly');
    $("#DischargePort").attr('readonly', 'readonly');
    $("#PlaceOfReceipt").attr('readonly', 'readonly');
    $("#PlaceOfDelivery").attr('readonly', 'readonly');
    $("#CompanyName").attr('readonly', 'readonly');
    $("#ShipmentTerm").attr('readonly', 'readonly');

    $("#GTotalSalesCal").attr('readonly','readonly');

    $("#POLFreeDays").attr('readonly', 'readonly');
    $("#PODFreeDays").attr('readonly', 'readonly');

    $("#CargoType").attr('readonly', 'readonly');
    $("#UNNo").attr('readonly', 'readonly');
    $("#IMO").attr('readonly', 'readonly');
    $("#Remark").attr('readonly', 'readonly');

    $("#ShipperName3").attr('readonly', 'readonly');
    $("#GrossWt").attr('readonly', 'readonly');
    $("#GrossWtUnit").attr('readonly', 'readonly');


    if ((quantity - quantitylifted) > 0) {
        $("#Save").show();
        //$("#Save").prop("disabled", true); //Disabling for now will make it live for April 15 release.
    }
    else {
        $("#Save").hide();
    }

    $("#submit").click(function () {
        document.forms[0].submit();
        return false;
    });

    $("#TransshipmentTypeYes").prop("disabled", true);
    $("#TransshipmentTypeNo").prop("disabled", true);
    $("#TransshipmentPortText").attr('readonly', 'readonly');
    $("#AgencyType").attr('readonly', 'readonly');
    $("#RateTypeReefer").prop("disabled", true);
    $("#RateTypeDry").prop("disabled", true);
    $("#Temperature").attr('readonly', 'readonly');
    $("#Humidity").attr('readonly', 'readonly');
    $("#Ventilation").attr('readonly', 'readonly');

    //$('#TransshipmentPortLabel').css('visibility', 'hidden');
    //$("#TransshipmentPortText").css('visibility', 'hidden');

    //$("#TransshipmentTypeYes").click(function () {
    //    $('#TransshipmentPortLabel').css('visibility', 'visible');
    //    $("#TransshipmentPortText").css('visibility', 'visible');
    //});

    //$("#TransshipmentTypeNo").click(function () {
    //    $('#TransshipmentPortLabel').css('visibility', 'hidden');
    //    $("#TransshipmentPortText").css('visibility', 'hidden');
    //});
    
    if (transshipmenttype == "Yes") {
        $('#TransshipmentPortLabel').css('visibility', 'visible');
        $("#TransshipmentPortText").css('visibility', 'visible');
    }

    else {
        $('#TransshipmentPortLabel').css('visibility', 'hidden');
        $("#TransshipmentPortText").css('visibility', 'hidden');
    }
    

    if (ratetype == "REEFER") {
        $('#TemperatureLabel').css('display', 'block');
        $('#Temperature').css('display', 'block');
        $('#HumidityLabel').css('display', 'block');
        $('#Humidity').css('display', 'block');
        $('#VentilationLabel').css('display', 'block');
        $('#Ventilation').css('display', 'block');
    }
});