<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="CalendarDetails.aspx.cs" Inherits="CalendarDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'events'
        };
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
            <h2>Event: <%=Event.Title %>
                <% if(Event.CustomerID == Identity.Current.CustomerID)
                   { %>
                <% if(Event.CalendarPrivacyTypeID == Exigo.Calendars.CalendarPrivacyTypes.Public)
                   { %>
                <span class="badge badge-warning"><i class="icon icon-eye-open icon-white"></i>&nbsp;Public</span>
                <% } %>
                <% if(Event.CalendarPrivacyTypeID == Exigo.Calendars.CalendarPrivacyTypes.Private)
                   { %>
                <span class="badge"><i class="icon icon-eye-close icon-white"></i>&nbsp;Private</span>
                <% } %>
                <% } %>
            </h2>

            <div class="row-fluid">
                <span class="span9">
                    <% if(Event.CustomerID != Identity.Current.CustomerID)
                       { %>
                    <h3>Posted by:</h3>
                    <p>
                        <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(((int)Event.CustomerID)) %>" class="pull-left" style="margin-right: 15px;" />
                        <div class="clearfix"></div>
                    </p>
                    <% } %>

                    <h3>Summary:</h3>
                    <%=Event.Description %>

                    <h3>Where:</h3>
                    <%=Event.Location%>

                    <h3>When:</h3>
                    <p><%=new Exigo.Calendars.CalendarService().GetCalendarItemDateSpanHtml(((bool)Event.AllDay), ((DateTime)Event.StartDate), ((DateTime)Event.EndDate), Event.TimeZoneOffset)%></p>
                </span>
                <span class="span3">
                    <ul class="nav nav-tabs nav-stacked">
                        <!--<li><a href="javascript:;"><i class="icon icon-share"></i>Share</a></li>-->

                        <% if(Event.CustomerID == Identity.Current.CustomerID)
                           { %>
                        <li><a href="ManageEvent.aspx?id=<%=Event.CalendarItemID %>"><i class="icon icon-edit"></i>&nbsp;Edit</a></li>

                        <% if(Event.CalendarPrivacyTypeID == Exigo.Calendars.CalendarPrivacyTypes.Public)
                           { %>
                        <li><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "markasprivate") %>"><i class="icon icon-eye-close"></i>&nbsp;Mark as private</a></li>
                        <% } %>

                        <% if(Event.CalendarPrivacyTypeID == Exigo.Calendars.CalendarPrivacyTypes.Private)
                           { %>
                        <li><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "markaspublic") %>"><i class="icon icon-eye-open"></i>&nbsp;Mark as public</a></li>
                        <% } %>

                        <li><a href="#deleteCalendarItemModal" role="button" data-toggle="modal"><i class="icon icon-trash"></i>&nbsp;Delete</a></li>
                        <% } %>
                    </ul>
                </span>
            </div>

            <div class="clearfix"></div>
        </div>
    </div>





    <div id="deleteCalendarItemModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="deleteCalendarItemModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <h2 id="deleteCalendarItemModalLabel">Deleting: <%=Event.Title %></h2>
        </div>
        <div class="modal-body">
            <p>Are you sure you want to delete <strong><%=Event.Title %></strong>?</p>
        </div>
        <div class="modal-footer">
            <a href="javascript:;" class="btn" data-dismiss="modal" aria-hidden="true">Cancel</a>
            <a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "delete") %>" class="btn btn-primary">Delete</a>
        </div>
    </div>
</asp:Content>

