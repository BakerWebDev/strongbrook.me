<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" ValidateRequest="false" AutoEventWireup="true" CodeFile="ManageEvent.aspx.cs" Inherits="ManageEvent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'events'
        };
    </script>



    <style>
        /* css for timepicker */
        .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
        .ui-timepicker-div dl { text-align: left; }
            .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
            .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
        .ui-timepicker-div td { font-size: 90%; }
        .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }

        .ui-timepicker-rtl { direction: rtl; }
            .ui-timepicker-rtl dl { text-align: right; }
                .ui-timepicker-rtl dl dd { margin: 0 65px 10px 10px; }
    </style>

    <link href="Assets/Themes/ui-lightness/jquery-ui.custom.css" rel="stylesheet" />
    <script src="Assets/Scripts/jquery-ui.min.js"></script>
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Plugins/jquery.datetimepicker/jquery.datetimepicker.js"></script>
    <script src="Assets/Plugins/tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
    <script src="Assets/Scripts/exigo.dates.js"></script>




    <script>
        // Set the default time zone based on the client's macine.
        var isNewEvent = <%=(CalendarItemID == 0) ? "true" : "false" %>;
        function padTimeZoneOffset(number, length) {
            var str = "" + number;
            while (str.length < length) {
                str = '0' + str;
            }
            return str;
        }
        function setDefaultTimeZoneOffset() {
            if(isNewEvent) {
                var offset = new Date().getTimezoneOffset();
                offset = ((offset < 0 ? '+' : '-') + padTimeZoneOffset(parseInt(Math.abs(offset / 60)), 2) + padTimeZoneOffset(Math.abs(offset % 60), 2));
                var $option = $('#lstTimeZone').children('option[value="' + offset + '"]');
                if($option.length == 1) {
                    $('#lstTimeZone').val(offset);
                }
            }
        }

        $(function () {
            setDefaultTimeZoneOffset();
        });




        var endDateHasFocussed = false;
        var endTimeHasFocussed = false;


        function updateRepeatOptions(unparsedDate) {
            var date = Date.parse(unparsedDate);
            var $dropdown = $('#lstCalendarItemRepeatTypes');
            var $options = $dropdown.children('option');

            // Weekly
            $options.filter('[value="3"]').text('Every {0}'.format(date.getDayName()));

            // Bi-Weekly
            $options.filter('[value="4"]').text('Every other {0}'.format(date.getDayName()));

            // Monthly
            $options.filter('[value="5"]').text('The {0} of every month'.format(date.getOrdinalDayNumber()));
        }

        $(function () {
            $('#txtStartDate, #txtEndDate').datepicker({
                numberOfMonths: 2,
                dateFormat: 'DD, MM d, yy'
            });

            $('#txtStartTime').timepicker({
                showSecond: false,
                timeFormat: 'h:mm TT',
                stepHour: 1,
                stepMinute: <%=MinuteStep%>,
                showTimezone: false,
                timezone: 'CST',
                defaultValue: '9:00 AM'
            });
            $('#txtEndTime').timepicker({
                showSecond: false,
                timeFormat: 'h:mm TT',
                stepHour: 1,
                stepMinute: <%=MinuteStep%>,
                showTimezone: false,
                timezone: 'CST',
                defaultValue: '10:00 AM'
            });

            $('#txtStartDate').on('change', function (event) {
                var $field = $(event.target);
                if (!endDateHasFocussed && $('#txtEndDate').val() == '') {
                    $('#txtEndDate').val($('#txtStartDate').val());
                }

                // Change the repeat item type option's text
                if ($field.val() != '') {
                    updateRepeatOptions($field.val());
                }
            }).triggerHandler('change');
            $('#chkAllDay').on('change', function (event) {
                var $checkbox = $(event.target);

                if ($checkbox.is(':checked')) {
                    $('.timewrapper').hide().find('input').val('');
                }
                else {
                    $('.timewrapper').show().find('input');
                }
            }).triggerHandler('change');
        });

        tinyMCE.init({
            // General options
            mode: "textareas",
            theme: "advanced",
            editor_selector: "editor",
            plugins: "autolink,spellchecker,iespell",

            // Theme options
            theme_advanced_buttons1: "bold,italic,underline,strikethrough,sub,sup,|,forecolor,backcolor,|,justifyleft,justifycenter,justifyright,justifyfull,|,undo,redo",
            theme_advanced_toolbar_location: "top",
            theme_advanced_toolbar_align: "left",
            theme_advanced_statusbar_location: "bottom",
            theme_advanced_resizing: true,

            // Skin options
            skin: "o2k7",
            skin_variant: "silver"
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Events</h1>

    <div class="sidebar">
        <a href="ManageEvent.aspx" class="btn btn-block"><i class="icon-edit"></i>&nbsp;New Event</a>
        <br />
        <ul class="nav nav-pills nav-stacked">
            <li><a href="Calendar.aspx">◄ Back to Calendar</a></li>
        </ul>
    </div>
    <div class="maincontent">
        <exigo:ErrorManager ID="Error" runat="server" />
        <div class="well well-large well-white">

            <h2><%=(CalendarItemID == 0) ? "Create an Event" : "Edit Event" %></h2>



            <h3>Event Date</h3>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="lstCalendar">Calendar:</label>
                </span>
                <span class="span9">
                    <asp:DropDownList ID="lstCalendar" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="txtStartDate">Starts:</label>
                </span>
                <span class="span6">
                    <asp:TextBox ID="txtStartDate" placeholder="Choose a date..." CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
                <span class="span3 timewrapper">
                    <asp:TextBox ID="txtStartTime" placeholder="Choose a time..." CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="txtEndDate">Ends:</label>
                </span>
                <span class="span6">
                    <asp:TextBox ID="txtEndDate" placeholder="Choose a date..." CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
                <span class="span3 timewrapper">
                    <asp:TextBox ID="txtEndTime" placeholder="Choose a time..." CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid timewrapper">
                <span class="span3 fieldlabel">
                    <label for="lstTimeZone">Time Zone:</label>
                </span>
                <span class="span9">
                    <asp:DropDownList ID="lstTimeZone" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="lstCalendarItemRepeatTypes">Repeat Options:</label>
                </span>
                <span class="span9">
                    <asp:DropDownList ID="lstCalendarItemRepeatTypes" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="chkAllDay">Additional Settings:</label>
                </span>
                <span class="span9">
                    <p class="checkbox">
                        <asp:CheckBox ID="chkAllDay" Text="All-day event" runat="server" ClientIDMode="Static" /></p>
                    <p class="checkbox">
                        <asp:CheckBox ID="chkIsPublic" Text="Allow other distributors to see this event" runat="server" ClientIDMode="Static" /></p>
                </span>
            </div>


            <h3>Event Details</h3>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="lstCalendarItemType">Event Type:</label>
                </span>
                <span class="span9">
                    <asp:DropDownList ID="lstCalendarItemType" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="txtTitle">Title:</label>
                </span>
                <span class="span9">
                    <asp:TextBox ID="txtTitle" CssClass="span12" placeholder="Name your event." runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="txtLocation">Location:</label>
                </span>
                <span class="span9">
                    <asp:TextBox ID="txtLocation" CssClass="editor span12" TextMode="MultiLine" Rows="5" placeholder="Where is your event?" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3 fieldlabel">
                    <label for="txtSummary">Summary:</label>
                </span>
                <span class="span9">
                    <asp:TextBox ID="txtSummary" CssClass="editor span12" TextMode="MultiLine" Rows="10" placeholder="Describe your event." runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid formactions">
                <span class="span3 fieldlabel"></span>
                <span class="span9">
                    <asp:LinkButton ID="submitbutton" Text="Save" OnClick="SaveChanges_Click" ClientIDMode="Static" CssClass="btn btn-success" runat="server" />
                    <a href="Calendar.aspx" class="btn">Cancel</a></span>
            </div>

            <div class="clearfix"></div>
        </div>
    </div>
</asp:Content>

