/*********************/
/* DISPLAY FUNCTIONS */
/*********************/
var exigo = exigo || {};

exigo.forms = {};
exigo.forms.restrictInput = function (selector, expression) {
    selector.bind('keypress', function (event) {
        var regex = new RegExp(expression);
        var keyCode = !event.charCode ? event.which : event.charCode;
        var key = String.fromCharCode(keyCode);
        
        // Firefox and Opera need some handholding here.
        // 0=Unknown, 8=Backspace
        if (keyCode == "0" || keyCode == "8") return true;

        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
};


/*
// Restrict inputs to follow the provided regular expression
Exigo.Forms.restrictInput($('#txtPhone, #txtPhone2, #txtMobilePhone, #txtFax'), '^[0-9/./\-]+$');
Exigo.Forms.restrictInput($('#txtEmail'), '^[a-zA-Z0-9_/\-/\@/\.]+$');
Exigo.Forms.restrictInput($('#txtSSN'), '^[0-9/./\-]+$');
Exigo.Forms.restrictInput($('#txtLoginName, #txtPassword, #txtConfirmPassword'), '^[a-zA-Z0-9_\-]+$');
Exigo.Forms.restrictInput($('#txtHomeZip, #txtShippingZip, #txtCreditCardBillingZip, #txtACHBillingZip'), '^[a-zA-Z0-9/\-]+$');
Exigo.Forms.restrictInput($('#txtCreditCardNumber, #txtACHAccountNumber, #txtACHRoutingNumber'), '^[0-9]+$');
Exigo.Forms.restrictInput($('#txtCreditCardCVV'), '^[0-9]+$');
Exigo.Forms.restrictInput($('.productitem .quantity input:text'), '^[0-9]+$');*/
