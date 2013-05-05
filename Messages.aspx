<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Messages.aspx.cs" Inherits="Messages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/messages.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.messages.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'messages'
        };

        $(function () {
            messagelist.init();
            folders.init();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Messages</h1>

    <div class="sidebar">
        <a href="CreateMessage.aspx" class="btn btn-block"><i class="icon-edit"></i>&nbsp;Compose</a>
        <br />
        <% RenderEmailFolders(); %>
    </div>
    <div class="maincontent">
        <div class="btn-toolbar">
            <div class="btn-group">
                <a class="btn dropdown-toggle" data-toggle="dropdown">
                    <input type="checkbox" />&nbsp;<span class="caret"></span></a>
                <ul class="dropdown-menu">
                    <li><a href="javascript:;" onclick="messagelist.selectAll()">All</a></li>
                    <li><a href="javascript:;" onclick="messagelist.deselectAll()">None</a></li>
                    <li><a href="javascript:;" onclick="messagelist.selectRead()">Read</a></li>
                    <li><a href="javascript:;" onclick="messagelist.selectUnread()">Unread</a></li>
                </ul>
            </div>

            <a href="<%=Request.Url.AbsoluteUri %>" class="btn"><i class="icon-refresh"></i></a>

            <div class="btn-group" data-hidewhennoneselected="true">
                <a href="javascript:;" onclick="messagelist.moveSelectedIDsToDeletedFolder()" class="btn"><i class="icon-trash"></i></a>
                <a href="javascript:;" onclick="messagelist.markSelectedAsRead()" class="btn">Mark as read</a>
            </div>

            <a href="javascript:;" onclick="messagelist.emptyTrash()" class="btn" data-showindeleteditems="true" style="display: none;"><i class="icon-trash"></i>&nbsp;Empty Trash</a>

            <div class="btn-group" data-hidewhennoneselected="true">
                <button class="btn dropdown-toggle" data-toggle="dropdown">Move to <span class="caret"></span></button>
                <% RenderMoveToEmailFolders(); %>
            </div>
            <div class="btn-group">
                <button class="btn dropdown-toggle" data-toggle="dropdown">More <span class="caret"></span></button>
                <ul class="dropdown-menu">
                    <li data-hidewhennoneselected="true"><a href="javascript:;" onclick="messagelist.markSelectedAsUnread()">Mark as unread</a></li>
                    <li><a href="javascript:;" onclick="messagelist.markAllAsRead()">Mark all as read</a></li>
                </ul>
            </div>
            <div class="clearfix"></div>
        </div>

        <% RenderEmails(); %>

        <div id="PageDescription" class="well white" style="margin-top: 20px;">
            <p>
                Notice:
            </p>
            <p>
                This messaging service is designed for communication exclusively with your downline sales organization. This system will not send messages to individuals who are not part of your sales team. Messages sent through this service will be sent to the recipient’s Backoffice message box, and NOT to their email account.
            </p>
            <p>
                Since this messaging system is only designed to send messages to other team member’s Backoffice message boxes,  you must be logged into this system in your Backoffice to retrieve and send messages.  
            </p>
        </div>

    </div>
</asp:Content>

