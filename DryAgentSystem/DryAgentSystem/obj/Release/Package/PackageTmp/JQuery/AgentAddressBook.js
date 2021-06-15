
$(function () {

    $userDetailGrid = $('#AgentAddressBookDataGrid').jqGrid({
        mtype: 'Get',
        url: 'AgentAddressBook/GetAgentAddressBookData',
        datatype: 'json',
        colNames: ['ID', 'Agency Name', 'Email', 'Phone No','Delete'],
        colModel: [
            {
                key: true,
                //hidden: true,
                name: 'ID',
                index: 'ID',
                //align: 'center',
                classes: 'myLink',
                formatter: 'showlink',
                width: '85px',
                formatoptions: {
                    baseLinkUrl: 'AgentAddressBookDetails/UpdateAgentAddressDetails',
                    idName: 'ID'
                }
            },
            {
                key: false,
                name: 'CompanyName',
                width: '402px'
            },
            //{
            //    key: false,
            //    name: 'Owner',
            //    width: '300px'
            //},
            {
                key: false,
                name: 'Email',
                width: '300px'
            },
            {
                key: false,
                name: 'PhoneNo',
                width: '200px'
            },
            {
                name: 'actions', index: 'actions', formatter: 'actions',
                formatoptions: {
                    keys: true,
                    editbutton: false,
                    delOptions: { url: 'AgentAddressBook/DeleteAgentAddressDetails' }
                }
            }],
        loadonce: true,
        responsive: true,
        shrinkToFit: false,
        //forceFit: true,
        autowidth: true,
        gridview: true,
        autoencode: true,
        pager: '#AgentAddressBookDataPager',
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
        //pager-you have to choose here what icons should appear at the bottom  
        //like edit,create,delete icons  
    });


    function reloadGrid(refresh) {
        if (refresh == true) {
            //Set initial values of your filters here
            //$("#txtId").val("");
            //$("#txtEmployeeName").val("");

            //reload grid's page 1 instead of current page
            $("#AgentAddressBookDataGrid").trigger("reloadGrid", { page: 1 });
        }
        else {
            $("#AgentAddressBookDataGrid").trigger("reloadGrid");
        }
    }

    

});

