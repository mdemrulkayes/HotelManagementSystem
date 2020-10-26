function StartSpinner() {
    document.getElementById('overlay').style.display = "block";
}

function StopSpinner() {
    document.getElementById('overlay').style.display = "none";
}

function ShowToaster(type, header, message) {
    if (type === "success") {
        toastr.success(message, header, { timeOut: 10000 });
    }
    if (type === "error") {
        toastr.error(message, header, { timeOut: 10000 });
    }
}