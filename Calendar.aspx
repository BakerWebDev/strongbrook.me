<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="Calendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Plugins/jquery.fullcalendar/fullcalendar.css" rel="stylesheet" />
    <link href="Assets/Plugins/jquery.qtip/jquery.qtip.css" rel="stylesheet" />
    <script src="Assets/Plugins/jquery.fullcalendar/fullcalendar.js"></script>
    <script src="Assets/Plugins/jquery.qtip/jquery.qtip.js"></script>
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Scripts/exigo.dates.js"></script>
    <script src="Assets/Scripts/exigo.calendar.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'events'
        };

        $(function () {
            // Toggle the display of the side navigation links
            $('.javascriptlinks').show();
            $('.pagelinks').hide();
        });

        $(function () {
            $('#calendar').calendar({
                eventsourceurl: '<%=Request.Url.AbsolutePath%>'
            });


            $('.subnavigation a').on('click', function (event) {
                var $link = $(event.target);
                var $parentitem = $link.parent('li');
                var $parentlist = $parentitem.parent('ul');

                $parentlist.find('li.active').removeClass('active');
                $parentitem.addClass('active');
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Events</h1>

    <%--
    <div class="sidebar">
        <a href="ManageEvent.aspx" class="btn btn-block"><i class="icon-edit"></i>&nbsp;New Event</a>
        <br />
        <ul class="nav nav-pills nav-stacked">
            <li class="active"><a href="javascript:;" onclick="$('#calendar').calendar('changeeventsource', 'all');">All Events</a></li>
            <li><a href="javascript:;" onclick="$('#calendar').calendar('changeeventsource', 'corporate');">Corporate</a></li>
            <li><a href="javascript:;" onclick="$('#calendar').calendar('changeeventsource', 'distributor');">Distributors</a></li>
            <li><a href="javascript:;" onclick="$('#calendar').calendar('changeeventsource', 'personal');">Personal</a></li>
        </ul>
    </div>
    --%>

    <div class="maincontent" style="border-left:0px;">
        <exigo:ErrorManager ID="Error" runat="server" />

            <%--<div id="calendar"></div>--%>
            <div style="width: 1050px;">
                <iframe name="frm-disclaimer" src="http://www.strongbrookcalendar.com" style="width: 1050px; height: 1050px; overflow: scroll; border: 0px;"></iframe>
            </div>

    </div>
</asp:Content>

