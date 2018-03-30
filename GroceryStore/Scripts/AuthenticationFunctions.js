function validateLoginForm() {
    if (loginForm.usernameData.value === "") {
        return false;
    }
    if (loginForm.passwordData.value === "") {
        return false;
    }
    return true;
}

function validateCreateAccountForm() {
    if (createAccountForm.usernameData.value === "") {
        return false;
    }
    if (createAccountForm.passwordData.value === "") {
        return false;
    }
    if (createAccountForm.addressData.value === "") {
        return false;
    }
    if (createAccountForm.emailData.value === "") {
        return false;
    }
    var emailRe = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!emailRe.test(String(createAccountForm.emailData.value).toLowerCase())) {
        return false;
    }
    if (createAccountForm.phoneData.value === "") {
        return false;
    }
    var phoneRe = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;
    if (!phoneRe.test(String(createAccountForm.phoneData.value).toLowerCase())) {
        return false;
    }
    return true;
}
