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