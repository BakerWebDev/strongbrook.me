/*
    Exigo Calendar

    Dependencies: 
     - DateJS (datejs.com)
     - qTip 2 (craigsworks.com/projects/qtip2)
     - FullCalendar (arshaw.com/fullcalendar)
     - CalendarNode.cs

     Sample call:
         $('#calendar').calendar({
            eventsourceurl: '<%=Request.Url.AbsolutePath%>'
        });
*/


(function ($) {
    var defaultSettings = {
        eventsourceurl: 'Calendar.aspx',
        eventsourcebasequery: '?action=calendarevents&filter=',
        eventdetailsurl: 'CalendarDetails.aspx?id=',
        currenteventfilter: 'all',
        corporateclass: 'theme-corporate',
        distributorclass: 'theme-distributor',
        personalclass: 'theme-personal',
        cache: true,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay,basicWeek'
        },
        buttonText: {
            prev: '&nbsp;&#9668;&nbsp;',  // left triangle
            next: '&nbsp;&#9658;&nbsp;',  // right triangle
            prevYear: '&nbsp;&lt;&lt;&nbsp;', // <<
            nextYear: '&nbsp;&gt;&gt;&nbsp;', // >>
            today: 'Today',
            month: 'Month',
            week: 'Week',
            agendaDay: 'Day',
            basicWeek: 'List'
        }
    };

    var eventfilters = {
        all: "all",
        corporate: "corporate",
        distributor: "distributor",
        personal: "personal"
    };

    var views = {
        small: "small",
        large: "large"
    };

    var settings = {};

    $.fn.calendar = function (action, options) {

        // Get the settings that were passed into the method
        if (actions[action]) {
            return actions[action].apply(this, Array.prototype.slice.call(arguments, 1)); // Call the action that was passed into the function
        } else if (typeof action === 'object' || !action) {
            return actions.init.apply(this, arguments); // Call the "init" method if the action is an object, or it's blank
        } else {
            $.error('Action ' + action + ' does not exist'); // Throw an error - the action they gave us doesn't exist and it's not an object
        }
    };

    var actions =
    {
        geteventsourceurl: function (eventfilter) {
            return settings.eventsourceurl + settings.eventsourcebasequery + eventfilter;
        },

        geteventsource: function (filter, cssclass) {
            var eventsource = {
                url: actions.geteventsourceurl(filter),
                type: 'GET',
                cache: settings.cache,
                className: cssclass,
                error: function () { alert('there was an error while fetching events!'); }
            };

            return eventsource;
        },

        changeeventsource: function (eventfilter) {
            settings.currenteventfilter = eventfilter;

            // Remove all existing sources
            this.fullCalendar('removeEventSource', eventsources.corporate.call());
            this.fullCalendar('removeEventSource', eventsources.distributor.call());
            this.fullCalendar('removeEventSource', eventsources.personal.call());

            // Based on the requested filter, set the appropriate sources
            switch (eventfilter) {
                case eventfilters.all:
                    this.fullCalendar('addEventSource', eventsources.corporate.call());
                    //this.fullCalendar('addEventSource', eventsources.distributor.call());
                    //this.fullCalendar('addEventSource', eventsources.personal.call());
                    break;
                case eventfilters.corporate: this.fullCalendar('addEventSource', eventsources.corporate.call()); break;
                case eventfilters.distributor: this.fullCalendar('addEventSource', eventsources.distributor.call()); break;
                case eventfilters.personal: this.fullCalendar('addEventSource', eventsources.personal.call()); break;
            }
        },

        // The initializer - starts the plugin
        init: function (options) {
            settings = $.extend(defaultSettings, options || {});


            var date = new Date();
            var d = date.getDate();
            var m = date.getMonth();
            var y = date.getFullYear();

            this.fullCalendar({
                editable: false,
                header: settings.header,
                buttonText: settings.buttonText,
                eventClick: events.custom_eventClick,
                eventRender: events.custom_renderEvent,
                eventSources: [
                    eventsources.corporate.call(),
                    eventsources.distributor.call(),
                    eventsources.personal.call()
                ]
            });
        },
    };

    var eventsources = {
        corporate: function () {
            return actions.geteventsource(eventfilters.corporate, settings.corporateclass);
        },
        distributor: function () {
            return actions.geteventsource(eventfilters.distributor, settings.distributorclass);
        },
        personal: function () {
            return actions.geteventsource(eventfilters.personal, settings.personalclass);
        }
    };

    var events = {
        custom_eventClick: function (calendarEvent, jsEvent, view) {
            window.location = settings.eventdetailsurl + calendarEvent.id;
        },

        custom_renderEvent: function (event, element, view) {
            var text = "";
            if (!event.personal) {
                text += "<h6>Posted by:</h6><p>{0}</p>".format(event.ownerdetails);
            }
            if (event.description != null && event.description != '') {
                text += "<h6>Summary:</h6>{0}".format(event.description);
            }
            if (event.location != null && event.location != '') {
                text += "<h6>Where:</h6>{0}".format(event.location);
            }
            if (event.allDay == false) {
                text += "<h6>When:</h6>{0}".format(event.formatteddate);
            }

            var iconType = "";
            if (event.personal) {
                var iconType = "<i class='icon icon-eye-open'></i>";
                if (event.private) iconType = "<i class='icon icon-eye-close'></i>";
            }

            element.qtip({
                content: {
                    text: text,
                    title: iconType + event.title
                },
                position: {
                    at: 'top center', // Position the tooltip above the link
                    my: 'bottom center',
                    viewport: $(window), // Keep the tooltip on-screen at all times
                    effect: false // Disable positioning animation
                },
                style: {
                    classes: 'qtip-shadow qtip-bootstrap'
                }
            });
        }
    }

})(jQuery);

