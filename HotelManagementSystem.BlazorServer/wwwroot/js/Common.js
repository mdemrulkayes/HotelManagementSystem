function DataTable() {
    $(document).ready(function () {
        $('#tblHotelRoom').DataTable();
    });
}

function DestroyDataTable() {
    $(document).ready(function () {
        $('#tblHotelRoom').DataTable().destroy();;
    });
}

function ShowToaster(type,header,message) {
    if (type === "success") {
        toastr.success(message,header,{ timeOut: 10000 });
    }
    if (type === "error") {
        toastr.error(message, header, { timeOut: 10000 });
    }
}