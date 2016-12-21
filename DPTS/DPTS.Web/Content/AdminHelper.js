function deleterecord(delid, cntrname) {
    alertify.confirm("Do you want to Delete Permanently ?", function (e) {
        if (e) {
           $.ajax({
               url: '/' + cntrname + '/DeleteConfirmed/'+delid,
                type: 'POST',

                success: function (data) {
                    debugger;
                    if (data == "Deleted") {
                        alertify.success("Record Deleted Successfully");
                        //window.location.reload();
                        //window.location.reload();
                        window.setTimeout(function () { location.reload() }, 1000)

                    }
                }
            });
        }
        else {
            alertify.success("You've clicked Cancel");
        }
    });
    return false;
}
