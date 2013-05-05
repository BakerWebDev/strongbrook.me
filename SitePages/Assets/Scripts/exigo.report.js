var exigo = exigo || {};
exigo.report = {};
exigo.report.infiniteScroller = {};

// 'report' is an alias for report. 
// This is to make the code a little less tedious.
var report = exigo.report.infiniteScroller;


/*********/
/* ENUMS */
/*********/
report.statuses = {
    READY: 'ready',
    BUSY: 'busy',
    CLOSED: 'closed'
};
report.sortOrderTypes = {
    ASCENDING: 'asc',
    DESCENDING: 'desc'
};


/********************/
/* DEFAULT SETTINGS */
/********************/
report.settings = {
    requestUrl: exigo.util.url.getPageName() + '?action=fetch',
    gridReportTableWrapperID: 'gridreporttablewrapper',
    gridReportTableID: 'gridreporttable',
    infiniteScrollTriggerID: 'infinitescrolltrigger',
    searchID: 'txtSearch',
    loadingImageUrl: 'Assets/Images/imgLoading.gif',
    loadingText: 'Loading ...',
    searchWrapperSelector: "#searchwrapper",
    searchFieldSelector: "#lstSearchField",
    searchOperatorSelector: "#lstSearchOperator",
    recordsPerPage: 50,
    triggerTopScrollOffset: 50,
    useAdvancedSearching: false,
    height: 350, // <---This changes the height of the white background report area.
    triggerFieldKey: null,
    triggerFieldKeyValue: 0,
    current: {
        sortOrder: report.sortOrderTypes.ASCENDING,
        sortField: 'CustomerID',
        page: 1,
        searchField: '',
        searchOperator: '',
        searchFilter: '',
        incrementPage: function () {
            this.page++;
        },
        resetPage: function () {
            this.page = 1;
        },
        resetSearchField: function () {
            this.searchField = '';
        },
        resetSearchOperator: function () {
            this.searchOperator = '';
        },
        resetSearchFilter: function () {
            this.searchFilter = '';
        }
    }
};



/***********/
/* ACTIONS */
/***********/
report.actions = {
    getRequestUrl: function () {
        var url = report.settings.requestUrl;
        var currentUserID = getUrlVars()["id"];

        

        url = url + "&page=" + report.settings.current.page;
        url = url + "&recordcount=" + report.settings.recordsPerPage;
        url = url + "&sortfield=" + report.settings.current.sortField;
        url = url + "&sortorder=" + report.settings.current.sortOrder;

        //I added this in an attempt to add "id" to the Querystring and use it as a parameter in the page that it is going to.
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }
        url = url + "&id=" + currentUserID;


        // trigger Field Keys - used when we are paging with SQL instead of OData
        if (report.settings.triggerFieldKey != null) {
            url = url + "&triggerfieldkey=" + report.settings.triggerFieldKey;
            url = url + "&triggerfieldkeyvalue=" + report.settings.triggerFieldKeyValue;
        }

        // Searching
        if (report.settings.current.searchFilter != '') {
            if (report.settings.useAdvancedSearching) {
                url = url + "&searchfield=" + report.settings.current.searchField;
                url = url + "&searchoperator=" + report.settings.current.searchOperator;
                url = url + "&searchfilter=" + report.settings.current.searchFilter;
            }
            else {
                url = url + "&searchfilter=" + report.settings.current.searchFilter;
            }
        }
        return url;
    },
    sort: function (newSortField) {
        if (report.settings.current.sortField != newSortField) report.settings.current.sortOrder = report.sortOrderTypes.ASCENDING;
        else {
            if (report.settings.current.sortOrder == report.sortOrderTypes.ASCENDING) report.settings.current.sortOrder = report.sortOrderTypes.DESCENDING;
            else report.settings.current.sortOrder = report.sortOrderTypes.ASCENDING;
        }

        report.settings.current.sortField = newSortField;

        $('a.sortable').removeClass(report.sortOrderTypes.ASCENDING);
        $('a.sortable').removeClass(report.sortOrderTypes.DESCENDING);
        $('a.sortable[data-field="' + report.settings.current.sortField + '"]').addClass(report.settings.current.sortOrder);

        report.results.clear();
        report.results.render();
    },
    search: function (searchField, searchOperator, searchFilter) {
        if (report.settings.useAdvancedSearching) {
            if ($(report.settings.searchFieldSelector).val() == '') {
                if (report.settings.current.searchFilter != '') {
                    report.search.clear();
                }
            }
            else {
                report.settings.current.searchField = searchField || '';
                report.settings.current.searchOperator = searchOperator || '';
                report.settings.current.searchFilter = searchFilter || '';
            }
        }
        else {
            if ($('#' + report.settings.searchID).val() == '') {
                if (report.settings.current.searchFilter != '') {
                    report.search.clear();
                }
            }
            else {
                report.settings.current.searchFilter = $('#' + report.settings.searchID).val();
            }
        }

        report.results.clear();
        report.results.render();
    },
    getScrollParent: function () {
        return (report.settings.height != null) ? $('.' + report.settings.gridReportTableWrapperID) : $(window);
    },
    init: function () {
        // Determine if we are using advanced searching through the exigo.report.Searching plugin.
        report.settings.useAdvancedSearching = (exigo.report.searching != null);


        // Add the trigger
        $('#' + report.settings.gridReportTableID).after("" +
			"<div id='" + report.settings.infiniteScrollTriggerID + "' data-status='" + report.statuses.READY + "'>" +
			"<div class='preloaderwrapper'>" +
			"<img src='" + report.settings.loadingImageUrl + "' /> " + report.settings.loadingText + "" +
			"</div>" +
			"</div>");

        report.results.render();

        if (report.settings.height != null) {
            report.actions.getScrollParent().css({
                'height': report.settings.height + 'px',
                'overflow-y': 'scroll'
            });
        }

        report.actions.getScrollParent().on('scroll', function () {
            if (report.trigger.isTriggered()) {
                switch (report.trigger.getStatus()) {
                    case report.statuses.READY:
                        report.trigger.setStatus(report.statuses.BUSY);
                        report.results.loadNextPage();
                        break;

                    case report.statuses.BUSY:
                        break;

                    case report.statuses.CLOSED:
                        report.trigger.disable();
                        break;
                }
            }
        });

        $('#' + report.settings.searchID).on('blur', function (event) {
            if (report.settings.current.searchFilter != '' && $('#' + report.settings.searchID).val() == '') {
                report.actions.search();
            }
            else if (report.settings.current.searchFilter != $('#' + report.settings.searchID).val()) {
                report.actions.search();
            }
        });
        $('#' + report.settings.searchID).on('keypress', function (event) {
            if (event.which == 13) {
                if (report.settings.current.searchFilter != $('#' + report.settings.searchID).val()) {
                    report.actions.search();
                }
                event.preventDefault();
            }
        });
        $('a.sortable[data-field]').on('click', function () {
            report.actions.sort($(this).attr('data-field'))
        });
    }
};


/***********************************/
/* INFINITE SCROLL TRIGGER ACTIONS */
/***********************************/
report.trigger = {
    setStatus: function (statusType) {
        $('#' + report.settings.infiniteScrollTriggerID).attr('data-status', statusType);
    },
    getStatus: function () {
        return $('#' + report.settings.infiniteScrollTriggerID).attr('data-status');
    },
    enable: function () {
        this.setStatus(report.statuses.READY);
        $('#' + report.settings.infiniteScrollTriggerID).show();
    },
    disable: function () {
        this.setStatus(report.statuses.CLOSED);
        $('#' + report.settings.infiniteScrollTriggerID).hide();
    },
    isStatus: function (statusType) {
        return $('#' + report.settings.infiniteScrollTriggerID).attr('data-status') == statusType;
    },
    isTriggered: function () {
        var docViewTop = report.actions.getScrollParent().scrollTop() + report.settings.triggerTopScrollOffset;
        var docViewBottom = docViewTop + report.actions.getScrollParent().height();

        var elemTop = $('#' + report.settings.infiniteScrollTriggerID).offset().top;
        var elemBottom = elemTop + $('#' + report.settings.infiniteScrollTriggerID).height();

        return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
    }
};


/*************/
/* SEARCHING */
/*************/
report.search = {
    clear: function () {
        report.settings.current.resetSearchField();
        report.settings.current.resetSearchOperator();
        report.settings.current.resetSearchFilter();
    }
};


/*************/
/* RESULTS */
/*************/
report.results = {
    loadNextPage: function () {
        report.settings.current.incrementPage();
        this.render();
    },
    render: function () {
        $.ajax({
            url: report.actions.getRequestUrl(),
            type: 'GET',
            cache: false,
            success: function (data) {
                // Insert the HTML of the results into the table.
                var html = data.split('^')[1];
                $('#' + report.settings.gridReportTableID).append(html);


                // Update the status of the infinite scroll trigger
                var resultCount = data.split('^')[0];
                if (resultCount == report.settings.recordsPerPage) report.trigger.setStatus(report.statuses.READY);
                else report.trigger.disable();

                if (report.settings.triggerFieldKey != null) {
                    report.settings.triggerFieldKeyValue = data.split('^')[2];
                }

            },
            error: function (xhr, status, error) {
                report.trigger.disable();
            }
        });
    },
    clear: function () {
        report.settings.current.page = 1;
        report.trigger.enable();
        $('#' + report.settings.gridReportTableID).find('tr[class!="table-headers"]').remove();
    }
};


/********************************/
/* PUBLICLY-ACESSIBLE FUNCTIONS */
/********************************/
function searchReport(searchField, searchOperator, searchFilter) {
    report.actions.search(searchField, searchOperator, searchFilter);
}
function setInitialSort(field, order) {
    report.settings.current.sortField = field;
    report.settings.current.sortOrder = order;
}
function initializeReport() {
    report.actions.init();
}
function resetSearch() {
    report.search.clear();
}
function setReportSetting(key, value) {
    report.settings[key] = value;
}