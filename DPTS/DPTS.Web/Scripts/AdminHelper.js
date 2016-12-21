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

$("#AddRoleForm").submit(function (event) {
    var role = $("#Name").val();
    if (!role) {
        //alert("Please enter role name.");
        event.preventDefault();
        alertify.error("Please enter role name");
    }
});

$("#UserAddToRole").submit(function (event) {
    var role = $("#Username").val();
    if (!role) {
        event.preventDefault();
        alertify.error("Please enter username.");
        return
    }

    var roleName = $("#RoleName").val();
    if (!roleName) {
        event.preventDefault();
        alertify.error("Please select role.");
        return
    }

});
$("#DeleteRoleForUser").submit(function (event) {
    var role = $("#Username").val();
    if (!role) {
        event.preventDefault();
        alertify.error("Please select username.");
        return
    }

    var roleName = $("#RoleName").val();
    if (!roleName) {
        event.preventDefault();
        alertify.error("Please select role.");
        return
    }

});