$(function () {
    var FlagForCompanyExist = false;
    $("#Save").click(function () {
        
        var Phoneval = $("#PhoneNo").val();
        var Email = $("#Email").val();
        var regex = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var flag = regex.test(Email);
        $("#Emailalert").hide();
        $("#PhoneNumberalert").hide();
        if (Phoneval == isNaN || Phoneval.length < 10 || flag == false) {
            if (Phoneval == isNaN || Phoneval.length < 10) {
                $("#PhoneNumberalert").show();
            }
            if (flag == false) {
                $("#Emailalert").show();
            }

            return false;
        }
        if (FlagForCompanyExist == false) {
            document.forms[0].submit();
            return false;
        }
        return false;
    });

    //$("#CompanyName").on('change', function () {
    //    var CompanyName = encodeURIComponent($("#CompanyName").val());
    //    $.ajax({
    //        contentType: "application/json; charset=utf-8",
    //        data: Boolean,
    //        url: "GetCompanyNamesForUpadte?agentaddressbook=" + CompanyName,
    //        success: function (data) {
    //            if (data == true) {
    //                $("#CompanyNamealert").show();
    //                FlagForComapnyExist = true;
    //                return false;
    //            }
    //            else {
    //                $("#CompanyNamealert").hide();
    //                FlagForComapnyExist = false;
    //            }
    //        }
    //    });
    //});

    $("#CompanyName").on('change', function () {
        var CompanyName = $("#CompanyName").val();
        var CompanyID = $("#ID").val();
        AddressBook = JSON.stringify({ 'CompanyName': CompanyName, 'CompanyID': CompanyID });
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: AddressBook,
            url: "GetCompanyNamesForUpdate",
            datatype: 'json',
            success: function (data) {
                if (data == true) {
                    $("#CompanyNamealert").show();
                    FlagForCompanyExist = true;
                    return false;
                }
                else {
                    $("#CompanyNamealert").hide();
                    FlagForCompanyExist = false;
                }
            }
        });
    });


    $("#CustomAdd").click(function () {

        var CompanyNameval = $("#CompanyName").val();
        var Addressval =  $("#Address").val();
        var Countryval =  $("#CountryDropDownList").val();
        var ZipCodeval = $("#Zipcode").val();
        var Phoneval = "Phone No.: " + $("#PhoneNo").val();
        var Emailval = "Email: " + $("#Email").val();
        var Websiteval = "Website: " + $("#Website").val();


        var CustAddress = CompanyNameval;
        CustAddress += "\n";
        CustAddress += Addressval;
        CustAddress += "\n";
        CustAddress += Countryval;
       // CustAddress += "\n";
        CustAddress += ", "+ZipCodeval;
        CustAddress += "\n";
        CustAddress += Phoneval;
        CustAddress += "\n";
        if ($("#Email").val() != "") {

            var Emailval = "Email: " + $("#Email").val();
            CustAddress += Emailval;
            CustAddress += "\n";

        }
        if ($("#Website").val() != "") {
            var Websiteval = "Website: " + $("#Website").val();
            CustAddress += Websiteval;
        }

        $("#CustomAddress").val(CustAddress);

        $("#custADD").show();
        $("#custADDText").show();
    });

    


});