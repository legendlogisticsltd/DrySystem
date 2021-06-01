var shipmentGrid;
var selectedRowData;
var totalRows;
var disabledRows = 0;
$(function () {

    shipmentGrid = $('#deliveryOrderGrid').jqGrid({
        loadComplete: function () {
            $(this).find(">tbody>tr.jqgrow>td:nth-child(" + (3 + 1) + ")")
                .each(function () {
                var ValueiCol2 = this.parentElement.cells[3].innerHTML;
                    if (ValueiCol2 != "&nbsp;") {
                    disabledRows++;
                };
                $(this.parentElement).find("td > input.cbox").attr("disabled", (ValueiCol2 != "&nbsp;" ? true : false));
            });
            checkPrintStatus(disabledRows);
        },
        onSelectAll: function (aRowids, status) {
            //if (status) {
            //    $("#deliveryOrderGrid").jqGrid('resetSelection');
            //    shipmentGrid[0].p.selarrrow = shipmentGrid.find("tr.jqgrow:not(:has(td > input.cbox:disabled))")
            //        .map(function () {
            //            $("#deliveryOrderGrid").jqGrid('setSelection', this.id);
            //            return this.id;
            //        }); // convert to set of ids
            //        //.get(); // convert to instance of Array
            //}

            if (status) {
                shipmentGrid[0].p.selarrrow = shipmentGrid.find("tr.jqgrow:has(td > input.cbox:disabled)")
                    .map(function () {
                        $("#deliveryOrderGrid").jqGrid('resetSelection', this.id);                        
                    });
                shipmentGrid[0].p.selarrrow = shipmentGrid.find("tr.jqgrow:not(:has(td > input.cbox:disabled))")
                    .map(function () {
                        return this.id;
                    });
            }
        },
        beforeSelectRow: function (rowid, e) {
            var cbsdis = $("tr#" + rowid + ".jqgrow > td > input.cbox:disabled", shipmentGrid);
            if (cbsdis.length === 0) {
                return true;    // allow select the row
            } else {
                return false;   // not allow select the row
            }
        },   
        
        mtype: 'Get',
        url: 'GetDeliveryContainers',
        datatype: 'json',
        colNames: ['Container No', 'Seal No', 'DO Number', 'DO Issue Date'],
        colModel: [
            {
                key: false,
                name: 'ContainerNo'
            },
            {
                key: false,
                name: 'SealNo'
            },
            {
                key: false,
                name: 'DONumber'
            },
            {
                key: false,
                name: 'DOIssueDate',
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
        pager: '#deliveryPager',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: 'auto',
        viewrecords: true,
        multiselect: true,        
        altRows: true,
        altclass: 'shipmentRow',
        loadtext: 'Loading Data please wait ...',
        emptyrecords: 'No records to display'
    });

   // var YourCellValue = $('#deliveryOrderGrid').jqGrid('getCell', shipmentGrid, 'YourCellId')
    //alert(YourCellValue);

    $("#ETADatePicker").attr('readonly', 'readonly');
    $("#ATADatePicker").attr('readonly', 'readonly');
    $("#VesselName").attr('readonly', 'readonly');
    $("#JobRef").attr('readonly', 'readonly');

    $("#ETADatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'
    });
    if ($("#ETADatePicker").val() == "01-01-0001") {
        $("#ETADatePicker").datepicker("setDate", '');
    }
    $("#ETADatePicker").datepicker().show();
    $("#ETADatePicker").datepicker("disable");
    $("#ETADatePicker").datepicker().next('button').hide();

    $("#ATADatePicker").datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        showOn: 'both'
    });

    if ($("#ATADatePicker").val() == "01-01-0001") {
        $("#ATADatePicker").datepicker("setDate", '');
    }
    $("#ATADatePicker").datepicker().show();
    $("#ATADatePicker").datepicker("disable");
    $("#ATADatePicker").datepicker().next('button').hide();

    function ReloadPage() {
        setTimeout(function () {
            window.location.reload(1);
        }, 1000);
    }

    function checkPrintStatus(disabledRows) {
        totalRows = $("#deliveryOrderGrid").getGridParam("reccount");
        if (totalRows == disabledRows) {
            $("#PrintContainers").attr("disabled", true);
            $("#PrintContainers").css("pointer-events", "none");
            $("#PrintContainers").css("opacity", 0.65);
        }
    }

    /* Multi select column Data jqGrid*/

    $("#PrintContainers").click(function () {
        var $this = $(this);
        $("#DishToalert").hide();
        $("#DishAttnalert").hide();

        if ($("#DishAttn").val == "" || $("#DishTo").val() == "") {
            if ($("#DishTo").val() == "") {
                $("#DishToalert").show();
            }
            if ($("#DishAttn").val() == "") {
                $("#DishAttnalert").show();

            }
            return false;
        }
        var myGrid = $('#deliveryOrderGrid');
        var selectedRowIds = myGrid.jqGrid("getGridParam", 'selarrrow');
        var containerNo = [];
        for (selectedRowIndex = 0; selectedRowIndex < selectedRowIds.length; selectedRowIndex++) {
            selectedRowData = myGrid.getRowData(selectedRowIds[selectedRowIndex]);
            containerNo.push(selectedRowData);
        }

        var params = new URLSearchParams(window.location.search);
        var JobRef = params.get('JobRef');
        var to = $("#DishTo").val();
        var attn = $("#DishAttn").val();
        containerNo = JSON.stringify({ 'containerNo': containerNo, 'JobRef': JobRef, 'DishTo': to, 'DishAttn': attn });
        //console.log(newArray);
        $.ajax({
            type: 'POST',
            url: 'UpdateRecords',
            data: containerNo,
            contentType: 'application/json; charset=utf-8',
            datatype: 'json',
            async: false,
            complete: function (data) {
                ReloadPage();
                if (data.status == 200) {
                    $this.attr("href", "CreateDeliveryOrder");
                    $this.attr("target", "_blank");
                }
            }
        });
    });
});