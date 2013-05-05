var exigo = exigo || {};
exigo.util = {};


/******************/
/* url utilITIES */
/******************/
exigo.util.url = {};
exigo.util.url.getQueryString = function (key) {
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]").toLowerCase();
    var regexS = "[\\?&]" + key + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search.toLowerCase());
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
};
exigo.util.url.getPageName = function () {
    var path = window.location.pathname;
    var querylesspath = path.split('?')[0];
    var page = querylesspath.substring(querylesspath.lastIndexOf('/') + 1);
    return page;
};


/******************/
/* math utilITIES */
/******************/
exigo.util.math = {};
exigo.util.math.round = function (numberToRound, decimalPlaces) { var result = math.round(numberToRound * math.pow(10, decimalPlaces)) / math.pow(10, decimalPlaces); return result };


/************************/
/* formatTING utilITIES */
/************************/
exigo.util.format = {};
exigo.util.format.asOrdinalNumber = function (number) { var n = number % 100; var suffix = ['th', 'st', 'nd', 'rd', 'th']; var ord = n < 21 ? (n < 4 ? suffix[n] : suffix[0]) : (n % 10 > 4 ? suffix[0] : suffix[n % 10]); return number + ord };
exigo.util.format.formatNumber = function (number, prefix) { prefix = prefix || ''; number += ''; var splitStr = number.split('.'); var splitLeft = splitStr[0]; var splitRight = splitStr.length > 1 ? '.' + splitStr[1] : ''; var regx = /(\d+)(\d{3})/; while (regx.test(splitLeft)) { splitLeft = splitLeft.replace(regx, '$1' + ',' + '$2') } return prefix + splitLeft + splitRight };
exigo.util.format.unformatNumber = function (number) { return number.replace(/([^0-9\.\-])/g, '') * 1; };
exigo.util.format.trim = function (string) { return string.replace(/^\s+|\s+$/g, ""); };
exigo.util.format.ltrim = function (string) { return string.replace(/^\s+/g, ""); };
exigo.util.format.rtrim = function (string) { return string.replace(/\s+$/g, ""); };
exigo.util.format.stripHtmlTags = function (html) { return html.replace(/<([^>]+)>/g, ''); };

// C#-styled string formatting
// Example: "{0} is dead, but {1} is alive! {0} {2}".format("ASP", "ASP.NET");
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};


/*********************/
/* PAYMENT utilITIES */
/*********************/
exigo.util.payments = {};
exigo.util.payments.validateCreditCard = function (creditCardNumber) { creditCardNumber = creditCardNumber.toString(); var LuhnDigit = parseInt(creditCardNumber.substring(creditCardNumber.length - 1, creditCardNumber.length)); var LuhnLess = creditCardNumber.substring(0, creditCardNumber.length - 1); var sum = 0; for (i = 0; i < LuhnLess.length; i++) { sum += parseInt(LuhnLess.substring(i, i + 1)) } var delta = new Array(0, 1, 2, 3, 4, -4, -3, -2, -1, 0); for (i = LuhnLess.length - 1; i >= 0; i -= 2) { var deltaIndex = parseInt(LuhnLess.substring(i, i + 1)); var deltaValue = delta[deltaIndex]; sum += deltaValue } var mod10 = sum % 10; mod10 = 10 - mod10; if (mod10 == 10) { mod10 = 0 } if (mod10 == parseInt(LuhnDigit)) { return true } return false };


/*******************/
/* ARRAY utilITIES */
/*******************/
exigo.util.arrays = {};
exigo.util.arrays.find = function (array, searchStr) { var returnArray = false; for (i = 0; i < array.length; i++) { if (typeof (searchStr) == 'function') { if (searchStr.test(array[i])) { if (!returnArray) { returnArray = [] } returnArray.push(i) } } else { if (array[i] === searchStr) { if (!returnArray) { returnArray = [] } returnArray.push(i) } } } return returnArray };