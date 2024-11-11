
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
