var fileFormatCheck = {
    checkExcelExtension: function (file) {
        var dot = file.lastIndexOf('.');
        if (dot >= 0) {
            var ext = file.substr(dot + 1, file.length).toLowerCase();
            if (ext in { 'xls': '', 'xlsx': '' }) return true;
        }
        return "请选择Excel文件.";
    },
    checkImageExtension: function (file) {
        var dot = file.lastIndexOf('.');
        if (dot >= 0) {
            var ext = file.substr(dot + 1, file.length).toLowerCase();
            if (ext in { 'jpg': '', 'gif': '', 'png': '', '.bmp': '', '.jpeg': '' }) return true;
        }
        return "请选择图片文件.";
    },
    checkPdfExtension: function (file) {
        var dot = file.lastIndexOf('.');
        if (dot >= 0) {
            var ext = file.substr(dot + 1, file.length).toLowerCase();
            if (ext in { 'pdf': ''}) return true;
        }
        return "请选择PDF文件.";
    },
    checkSoftwareExtension: function (file) {
        var dot = file.lastIndexOf('.');
        if (dot >= 0) {
            var ext = file.substr(dot + 1, file.length).toLowerCase();
            if (ext in { 'ipa': '', 'apk': '' }) return true;
        }
        return "请选择PDF文件.";
    }
};