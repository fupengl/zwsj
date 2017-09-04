var loadFailed = function () {
    alert("Something went wrong while loading SWFUpload. If this were a real application we'd clean up and then give you an alternative");
};

var statusIconRenderer = function (value) {
    switch (value) {
        default:
            return value;
        case 'Pending':
            return '<img src="' + Ext.net.ResourceMgr.getIconUrl("Hourglass") + '" width=16 height=16>';
        case 'Sending':
            return '<div src="x-loading-indicator" width=16 height=16>';
        case 'Error':
            return '<img src="' + Ext.net.ResourceMgr.getIconUrl("Decline") + '" width=16 height=16>';
        case 'Cancelled':
        case 'Aborted':
            return '<img src="' + Ext.net.ResourceMgr.getIconUrl("Decline") + '" width=16 height=16>';
        case 'Uploaded':
            return '<img src="' + Ext.net.ResourceMgr.getIconUrl("Tick") + '" width=16 height=16>';
    }
};

var updateRecord = function (id, field, value) {
    var rec = App.UploadGrid.store.getById(id);

    rec.set(field, value);
    rec.commit();
};

var abortUpload = function (btn) {
    var selModel = btn.up('grid').getSelectionModel(),
                records;

    if (!selModel.hasSelection()) {
        Ext.Msg.alert('Error', 'Please select an upload to cancel');
        return true;
    }

    records = selModel.getSelection();
    App.MultiUpload1.abortUpload(records[0].getId());
    return true;
};

var removeUpload = function (btn) {
    var selModel = btn.up('grid').getSelectionModel(),
                records;

    if (!selModel.hasSelection()) {
        Ext.Msg.alert('Error', 'Please select an upload to remove');
        return true;
    }

    records = selModel.getSelection();
    App.MultiUpload1.removeUpload(records[0].getId());
};

var fileSelected = function (item, file) {
    //Example of cancelling a file to be selection
    if (file.name == 'image.jpg') {
        Ext.Msg.alert('Error', 'You cannot upload a file named "image.jpg"');
        return false;
    }
    this.up('grid').store.add({
        id: file.id,
        name: file.name,
        size: file.size,
        status: 'Pending',
        progress: 0
    });
};

var uploadError = function (item, file, errorCode, message) {
    updateRecord(file.id, 'progress', 0);
    updateRecord(file.id, 'status', 'Error');

    switch (errorCode) {
        case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
            alert("Error Code: HTTP Error, File name: " + file.name + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
            alert("Error Code: Upload Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.IO_ERROR:
            alert("Error Code: IO Error, File name: " + file.name + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
            alert("Error Code: Security Error, File name: " + file.name + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
            alert("Error Code: Upload Limit Exceeded, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
            alert("Error Code: File Validation Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
            updateRecord(file.id, 'status', 'Cancelled');
            break;
        case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
            updateRecord(file.id, 'status', 'Stopped');
            break;
        default:
            updateRecord(file.id, 'status', "Unhandled Error: " + errorCode);
            alert("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
    }
};

function fileDialogComplete(numFilesSelected, numFilesQueued) {
    try {
        if (numFilesQueued > 0) {
            this.startResizedUpload(this.getFile(0).ID, 100, 100, SWFUpload.RESIZE_ENCODING.JPEG, 100);
        }
    } catch (ex) {
        this.debug(ex);
    }
}

var fileSelectionError = function (item, file, errorCode, message) {
    if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
        alert("You have attempted to queue too many files.\n" + (message === 0 ? "You have reached the upload limit." : "You may select " + (message > 1 ? "up to " + message + " files." : "one file.")));
        return;
    }

    switch (errorCode) {
        case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
            alert("Error Code: File too big, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
            alert("Error Code: Zero byte file, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
            alert("Error Code: Invalid File Type, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
        default:
            alert("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
            break;
    }
};

