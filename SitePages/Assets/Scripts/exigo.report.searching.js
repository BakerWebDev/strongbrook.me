var exigo = exigo || {};
exigo.report = exigo.report || {};
exigo.report.searching = {};

var searching = exigo.report.searching;

searching.listdatasources = {};

searching.settings = {
	wrapperSelector: "#searchwrapper",
	searchFieldSelector: "#lstSearchField",
	searchOperatorSelector: "#lstSearchOperator"
};

searching.operators = {
	equals: {
		type: "equals",
		querystring: "eq",
		text: "Equals",
		instructions: null
	},
	notequals: {
		type: "notequals",
		querystring: "neq",
		text: "Does not equal",
		instructions: null
	},
	greaterthan: {
		type: "greaterthan",
		querystring: "gt",
		text: "Greater than",
		instructions: null
	},
	greaterthanorequals: {
		type: "greaterthanorequals",
		querystring: "gte",
		text: "Greater than or equals",
		instructions: null
	},
	lessthan: {
		type: "lessthan",
		querystring: "lt",
		text: "Less than",
		instructions: null
	},
	lessthanorequals: {
		type: "lessthanorequals",
		querystring: "lte",
		text: "Less than or equals",
		instructions: null
	},
	between: {
		type: "between",
		querystring: "btw",
		text: "Between",
		instructions: null
	},
	isinlist: {
		type: "isinlist",
		querystring: "il",
		text: "Is in list",
		instructions: "Separate by commas(,)"
	},
	isnotinlist: {
		type: "isnotinlist",
		querystring: "nil",
		text: "Is not in list",
		instructions: "Separate by commas(,)"
	}
};
searching.fieldtypes = {
	string: {
		type: "string",
		operators: [
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.isinlist,
			searching.operators.isnotinlist
		],
		instructions: null
	},
	int: {
		type: "int",
		operators: [
			searching.operators.between,
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.greaterthan,
			searching.operators.greaterthanorequals,
			searching.operators.lessthan,
			searching.operators.lessthanorequals,
			searching.operators.isinlist,
			searching.operators.isnotinlist
		],
		instructions: "Search by a number ..."
	},
	date: {
		type: "date",
		operators: [
			searching.operators.between,
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.greaterthan,
			searching.operators.greaterthanorequals,
			searching.operators.lessthan,
			searching.operators.lessthanorequals
		],
		instructions: "Search by a date ...(m/d/yyyy)"
	},
	bool: {
		type: "bool",
		operators: [
			searching.operators.equals
		],
		instructions: null
	},
	intlist: {
		type: "int",
		operators: [
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.greaterthan,
			searching.operators.greaterthanorequals,
			searching.operators.lessthan,
			searching.operators.lessthanorequals
		],
		instructions: null
	},
	stringlist: {
		type: "string",
		operators: [
			searching.operators.equals,
			searching.operators.notequals
		],
		instructions: null
	},
	datelist: {
		type: "date",
		operators: [
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.greaterthan,
			searching.operators.greaterthanorequals,
			searching.operators.lessthan,
			searching.operators.lessthanorequals
		],
		instructions: null
	}
};
searching.fieldgroups = {
	general: {
		fieldtypes: [
			searching.fieldtypes.string,
			searching.fieldtypes.int,
			searching.fieldtypes.date
		],
		operatortypes: [
			searching.operators.equals,
			searching.operators.notequals,
			searching.operators.greaterthan,
			searching.operators.greaterthanorequals,
			searching.operators.lessthan,
			searching.operators.lessthanorequals,
			searching.operators.isinlist,
			searching.operators.isnotinlist,
		]
	},
	range: {
		fieldtypes: [
			searching.fieldtypes.string,
			searching.fieldtypes.int,
			searching.fieldtypes.date
		],
		operatortypes: [
			searching.operators.between
		]
	},
	bool: {
		fieldtypes: [
			searching.fieldtypes.bool
		],
		operatortypes: [
			searching.operators.equals
		]
	}
};


searching.actions = {
	showFilterGroup: function () {
		var $fieldtypedropdown = $(searching.settings.searchFieldSelector);
		var $operatortypedropdown = $(searching.settings.searchOperatorSelector);


		// Identify the current field type and operator type
		var fieldtypekey = $fieldtypedropdown.find('option:selected').attr('data-fieldtype');
		var listdatasourcekey = $fieldtypedropdown.find('option:selected').attr('data-source');
		var operatortypekey = $operatortypedropdown.find('option:selected').attr('data-operatortype')
		var fieldtype = searching.fieldtypes[fieldtypekey];
		var operatortype = searching.operators[operatortypekey];
		if (listdatasourcekey) {
			fieldtype = searching.fieldtypes[fieldtypekey + 'list'];
			operatortype = searching.operators[operatortypekey + 'list'];
		}


		// Toggle the display of the appropriate filter group
		var $filtergroups = $(searching.settings.wrapperSelector + ' .filtergroup');
		var $filtergroup;

		if (listdatasourcekey) {
			$filtergroup = $filtergroups.filter('[data-source="' + listdatasourcekey + '"]');
		}
		else {
			$filtergroup = $filtergroups.filter('[data-fieldtypes*= "' + fieldtypekey + '"][data-operatortypes*="' + operatortypekey + '"]:not([data-source])');
		}

		$filtergroups.hide();
		$filtergroup.show();


		// Toggle the display of the appropriate operator options based on the field type
		var $operatortypedropdownoptions = $operatortypedropdown.find('option');
		$operatortypedropdownoptions.attr("disabled", true).hide();

		for (i = 0; i < fieldtype.operators.length; i++) {
			$operatortypedropdownoptions.filter('option[data-operatortype="' + fieldtype.operators[i].type + '"]').attr("disabled", false).show();
		}
		
		var $selectedoperatortypeoption = $operatortypedropdownoptions.filter('option:selected');
		if ($selectedoperatortypeoption.is(':enabled') == false) {
			$selectedoperatortypeoption.attr('selected', false);
			$operatortypedropdownoptions.first().attr('selected', true);
			$operatortypedropdown.trigger('change');
		}


		// Update the HTML5 placeholder for all search boxes
		if (!listdatasourcekey) {
			var instructions = fieldtype.instructions || operatortype.instructions || "Search ...";
			$filtergroup.find('input[type="search"]:visible').each(function () {
				var $textbox = $(this);
				$textbox.attr('placeholder', instructions);
			});
		}
	},
	initializeOperatorOptions: function () {
	    $(searching.settings.wrapperSelector).append('<select id="' + searching.settings.searchOperatorSelector.substring(1) + '" class="input-medium"></select>');
		var $operatortypedropdown = $(searching.settings.searchOperatorSelector);

		for (i in searching.operators) {
			var operator = searching.operators[i];
			$operatortypedropdown.append('<option data-operatortype="' + operator.type + '" value="' + operator.querystring + '">' + operator.text + '</option>');
		}
	},
	initializeFieldGroups: function () {
		var $searchwrapper = $(searching.settings.wrapperSelector);
		var html, fieldgroup, fieldtypes, operatortypes;

		// General field group
		fieldgroup = searching.fieldgroups.general;
		fieldtypes = searching.actions.serialize(fieldgroup.fieldtypes);
		operatortypes = searching.actions.serialize(fieldgroup.operatortypes);
		html = ' <span class="filtergroup" data-fieldtypes="' + fieldtypes + '" data-operatortypes="' + operatortypes + '">';
		html += searching.actions.getTextBoxHtml();
		html += " <a href='javascript:search();' class='btn btn-success btn-margin-bottom'>Search</a>";
		html += " <a href='javascript:reset();' class='btn btn-margin-bottom'>Reset</a>";
		html += '</span>';
		$searchwrapper.append(html);


		// Range field group
		fieldgroup = searching.fieldgroups.range;
		fieldtypes = searching.actions.serialize(fieldgroup.fieldtypes);
		operatortypes = searching.actions.serialize(fieldgroup.operatortypes);
		html = ' <span class="filtergroup" data-fieldtypes="' + fieldtypes + '" data-operatortypes="' + operatortypes + '">';
		html += searching.actions.getTextBoxHtml();
		html += ' and ';
		html += searching.actions.getTextBoxHtml();
		html += " <a href='javascript:search();' class='btn btn-success btn-margin-bottom'>Search</a>";
		html += " <a href='javascript:reset();' class='btn btn-margin-bottom'>Reset</a>";
		html += '</span>';
		$searchwrapper.append(html);


		// Bool field group
		fieldgroup = searching.fieldgroups.bool;
		fieldtypes = searching.actions.serialize(fieldgroup.fieldtypes);
		operatortypes = searching.actions.serialize(fieldgroup.operatortypes);
		html = ' <span class="filtergroup" data-fieldtypes="' + fieldtypes + '" data-operatortypes="' + operatortypes + '">';
		html += searching.actions.getBoolDropdownHtml();
		html += " <a href='javascript:search();' class='btn btn-success btn-margin-bottom'>Search</a>";
		html += " <a href='javascript:reset();' class='btn btn-margin-bottom'>Reset</a>";
		html += '</span>';
		$searchwrapper.append(html);


		// All list field groups
		for (i in searching.listdatasources) {
			var datasource = searching.listdatasources[i];

			html = ' <span class="filtergroup" data-source="' + i + '">';
			html += searching.actions.getListHtml(datasource);
			html += " <a href='javascript:search();' class='btn btn-success btn-margin-bottom'>Search</a>";
			html += " <a href='javascript:reset();' class='btn btn-margin-bottom'>Reset</a>";
			html += '</span>';
			$searchwrapper.append(html);
		}
	},
	serialize: function (collection) {
		var result = "";

		for (i in collection) {
			if (result != "") result += ",";
			result += collection[i].type;
		}

		return result;
	},
	getTextBoxHtml: function () {
		var html = "";

		html = "<input type='search' class='search' placeholder='Search...' />";

		return html;
	},
	getListHtml: function (datasource) {
		var html = "";

		html = "<select>";
		for (i in datasource) {
			var option = datasource[i];
			html += "<option value='" + option.value + "'>" + option.text + "</option>";
		}
		html += "</select>";

		return html;
	},
	getBoolDropdownHtml: function () {
		var html = "";

		html = "<select>";
		html += "<option value='1'>True</option>";
		html += "<option value='0'>False</option>";
		html += "</select>";

		return html;
	}
};



function initializeReportSearch() {
    // Activate the advanced search settings
    report.settings.useAdvancedSearching = true;

	// Create our search fields and options
	searching.actions.initializeOperatorOptions();
	searching.actions.initializeFieldGroups();


	$(searching.settings.searchOperatorSelector).on('change', function () {
		searching.actions.showFilterGroup();
	});
	$(searching.settings.searchFieldSelector).on('change', function () {
		searching.actions.showFilterGroup();
	}).trigger('change');


	$(searching.settings.wrapperSelector + ' .filtergroup input').on('keypress', function (event) {
		if (event.which == 13) {
			search();
			event.preventDefault();
		}
	});
	$(searching.settings.wrapperSelector + ' .filtergroup select').on('change', function (event) {
		search();
	});

	$(searching.settings.searchFieldSelector).focus();
}

function search() {
	var $fieldtypedropdown = $(searching.settings.searchFieldSelector);
	var $operatortypedropdown = $(searching.settings.searchOperatorSelector);

	var fieldname = $fieldtypedropdown.val();
	var operator = $operatortypedropdown.val();
	var searchfilter;

	var $fieldgroup = $(searching.settings.wrapperSelector + ' .filtergroup:visible');	
	var listdatasource = $fieldtypedropdown.attr('data-source');
	
	if($fieldgroup.find('input').length == 1) {
		searchfilter = $fieldgroup.find('input').val();
	}
	if($fieldgroup.find('input').length == 2) {
		var first = $fieldgroup.find('input').first().val();
		var second = $fieldgroup.find('input').last().val();
		searchfilter = first + ',' + second;
	}
	if ($fieldgroup.find('select').length == 1) {
		searchfilter = $fieldgroup.find('select').val();
	}

    // If we passed in an asterisk, reset the filters as if we were resetting the report.
	if (searchfilter == '*') {
	    reset();
	}

	// If we didn't pass an asterisk, call the exigo.report's searchReport() method, passing in the filters.
	else {
	    searchReport(fieldname, operator, searchfilter);
	}
}
function reset() {
    $(searching.settings.wrapperSelector + ' .filtergroup input').val('');
    $(searching.settings.wrapperSelector + ' .filtergroup select option:first-child').attr('selected', true);
	searchReport();
}