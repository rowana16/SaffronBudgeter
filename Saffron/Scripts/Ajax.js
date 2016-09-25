function GetPartialTest() {
    console.log("Clicked, ID:");

    $.ajax({
        type: "Get",
        url: "/Partial/Test",
        //data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            console.log("Error! deleteBudgetBtn()" + response);
        },
        success: function (response) {
            console.log("Success! deleteBudgetBtn()");
            $("#divElement").html(response);
        }
    })
};
// editTransaction
function editTransactionBtn(id) {
    console.log("Clicked, ID: " + id)
    var dropdownElement = '#transactionDropdown-' + id;

    $.ajax({
        //Set up Modal 
        beforeSend: function () {
            //$(dropdownElement.Click());
            $("#editTransactionModal-Body").empty();
            $("#editTransactionModal-Body").append('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#editTransactionModal").modal('show');
            console.log("ModalUp");
        },

        type: "GET",
        url: "/Partial/Edit",
        data: { id: id },
        dataType: 'text',
        error: function (response) {
            console.log("Error editTransactionBtn(id)!" + response);
        },
        success: function (response) {
            console.log("Success! _EditTransactionPartial");
            $('#editTransactionModal-Body').html(response);
            //$('#display-account-balances').load("/Saver/_AccountBalancesPartial", { id: $("#current-acc-num").val() });
        }
    })
};

function submitEditTransactionForm() {
    var form = $("#editForm");
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    //var url = ;
    //var data = ;

    //console.log(data)

    $.ajax({
        type: "POST",
        url: "/Transactions/Edit",
        data: form.serialize(),
        error: function (responseEditTransaction) {
            console.log("Error submitEditTransactionForm" + responseEditTransaction);
        },
        success: function (responseEditTransaction) {
            console.log("Success! submitEditTransactionform()");
            $("#editTransactionModal-Body").empty();
            $("#editTransactionModal").modal('hide');
        }
    })
};