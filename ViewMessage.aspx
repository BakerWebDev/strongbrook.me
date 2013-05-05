<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="ViewMessage.aspx.cs" Inherits="ViewMessage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/messages.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.messages.js"></script>
    <script src="Assets/Plugins/tinymce/jscripts/tiny_mce/jquery.tinymce.js"></script>

    

    <script>
        // Set page variables
        var page = {
            activenavigation: 'messages'
        };

        messagelist.settings.currentmessageid = <%=ViewModel.MailID %>;

        $(function () {

            folders.init();
            message.init();
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
            <a href="Messages.aspx" title="Back to inbox" class="btn"><i class="icon-arrow-left"></i></a>

            <div class="btn-group">
                <a href="CreateMessage.aspx?rid=<%=ViewModel.MailID %>" title="Reply" class="btn"><i class="icon-comment"></i>&nbsp;Reply</a>
                <a href="CreateMessage.aspx?fid=<%=ViewModel.MailID %>" title="Forward" class="btn"><i class="icon-arrow-right"></i>&nbsp;Forward</a>
            </div>

            <a href="javascript:;" onclick="messagelist.moveSelectedIDsToDeletedFolder()" title="Delete" class="btn"><i class="icon-trash"></i></a>

            <div class="btn-group">
                <button class="btn dropdown-toggle" data-toggle="dropdown">Move to <span class="caret"></span></button>
                <% RenderMoveToEmailFolders(); %>
            </div>

            <div class="btn-group">
                <button class="btn dropdown-toggle" data-toggle="dropdown">More <span class="caret"></span></button>
                <ul class="dropdown-menu">
                    <li><a href="javascript:;" onclick="messagelist.markSelectedAsUnread(<%=ViewModel.MailID %>)">Mark as unread</a></li>
                </ul>
            </div>

            <div class="clearfix"></div>
        </div>

        <div class="viewmessage">

            <div class="row-fluid">
                <span class="span6">
                    <div class="from">
                        <div class="name">From: <a href="javascript:;"><strong><%=ViewModel.MailFromDisplay %></strong></a></div>
                        <div class="clearfix"></div>
                    </div>
                </span>
                <span class="span6">
                    <div class="options">
                        <div class="btn-group">
                            <a href="CreateMessage.aspx?rid=<%=ViewModel.MailID %>" title="Reply" class="btn"><i class="icon-comment"></i></a>
                            <button class="btn dropdown-toggle" data-toggle="dropdown">
                                <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu pull-right">
                                <li><a href="CreateMessage.aspx?rid=<%=ViewModel.MailID %>"><i class="icon-comment"></i>&nbsp;Reply</a></li>
                                <li><a href="CreateMessage.aspx?fid=<%=ViewModel.MailID %>"><i class="icon-arrow-right"></i>&nbsp;Forward</a></li>
                                <li><a href="javascript:;">Print</a></li>
                                <li><a href="javascript:;" onclick="messagelist.moveSelectedIDsToDeletedFolder()">Delete</a></li>
                                <li><a href="javascript:;" onclick="messagelist.markSelectedAsUnread(<%=ViewModel.MailID %>)">Mark as unread</a></li>
                            </ul>
                        </div>
                        <div class="age">
                            <% RenderMailDate(); %>
                        </div>
                    </div>
                    <div class="clearfix"></div>
                </span>
            </div>
            <div class="clearfix"></div>

            <div class="well well-large well-white">
                <h3><%=ViewModel.Subject %></h3>
                <hr />
                <%=ViewModel.Body %>

                <% RenderAttachments(); %>

                
                <div class="clearfix"></div>
            </div>

            <div class="well well-large well-white replybox">
                <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(Identity.Current.CustomerID) %>" class="avatar" />
                Click here to <a href="CreateMessage.aspx?rid=<%=ViewModel.MailID %>">Reply</a> or <a href="CreateMessage.aspx?fid=<%=ViewModel.MailID %>">Forward</a>.
            </div>
        </div>
    </div>
</asp:Content>

