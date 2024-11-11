
function isValidEmail(email,isShowErrorMessage,spanId) {
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;  // Basic email regex pattern
    var isValid = emailPattern.test(email);
    if (isShowErrorMessage) {
        if (isValid) {
            $('#' + spanId).hide();
            return true;
        } else {
            $('#' + spanId).show();
            return false;
        }
    } else {
        return isValid;
    }
}

function validateForm(value, isShowError, errorId) {
    var isValid = value != null && value != "";

    if (isShowError) {
        if (isValid) {
            $('#' + errorId).hide();
            return true;
        } else {
            $('#' + errorId).show();
            return false;
        }
    } else {
        return isValid;
    }
}
