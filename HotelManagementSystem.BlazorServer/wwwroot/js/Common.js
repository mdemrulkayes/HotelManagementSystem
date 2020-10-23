function DataTable() {
    $(document).ready(function () {
        $('#tblHotelRoom').DataTable();
    });
}

function ShowToaster(type,header,message) {
    $.toastDefaults = {

        // top-left, top-right, bottom-left, bottom-right, top-center, and bottom-center
        position: 'top-right',

        // is dismissable?
        dismissible: true,

        // is stackable?
        stackable: true,

        // pause dely on hover
        pauseDelayOnHover: true,

        // additional CSS Classes
        style: {
            toast: '',
            info: '',
            success: '',
            warning: '',
            error: '',
        }

    };

    $.toast({
        title: header,
        content: message,
        type: type,
        delay: 10000
    });
}