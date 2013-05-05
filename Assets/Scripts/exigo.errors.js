var exigo = exigo || {};

// Syntax for 'errors' variable: [1|0]|error string
// 1 = successful, 0 = there were errors
exigo.throwErrors = function (selector, errors) {
    if (errors != '') {
        var type            = errors.split('|')[0];
		var heading         = errors.split('|')[1];
		var errormessage    = errors.split('|')[2];
		type = (type == "true" || type == "1") ? "success" : "danger";

		$(selector).html('<div class="alert alert-block alert-' + type + '"><a class="close" data-dismiss="alert" href="#">×</a>' +
				'<strong class="alert-heading">' + heading + '</strong><br />' + errormessage + '</div>').show();
	}
};