$(document).ready(function () {
  
    var tempE = $("#EditAlert").val();
    var tempC = $("#CreateAlert").val();
    var tempD = $("#DeleteAlert").val();
    if (tempC != "" && tempC != null) {
        toastr.success(tempC, "Success");
        temp = null;
    } if (tempE != "" && tempE != null) {
        toastr.success(tempE, "Success");
        tempE = null
    } if (tempD != "" && tempD != null) {
        toastr.warning(tempD, "Warning");
        tempD = null;
    }
    var Errorer = $("#Error").val();
    if (Errorer != "" && Errorer != null) {
        toastr.error(Errorer, "Error");
        Errorer = null;
    }

   
    $('#vertical-menu').addClass('show');
    debugger;
   
    $('#vertical_toggle').click(function () {
        debugger;
        console.log($('#vertical_toggle').val());
        $('#vertical-menu').collapse('toggle');
    });

});



  

