using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ViewMessage : System.Web.UI.Page, IPostBackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // If we don't have a mail ID, move back to the inbox.
        if(MailID == 0 || ViewModel == null) Response.Redirect("Messages.aspx");

        // Force __doPostBack javascript functions to work
        Page.ClientScript.GetPostBackClientHyperlink(this, "");
    }

    #region Properties
    public int MailID
    {
        get { return (Request.QueryString["id"] != null) ? Convert.ToInt32(Request.QueryString["id"]) : 0; }
    }

    public Email ViewModel
    {
        get
        {
            if(_viewModel == null)
            {
                var service = new MessagesService();
                _viewModel = service.GetEmail(MailID);                
            }
            return _viewModel;
        }
    }
    private Email _viewModel;
    #endregion

    #region Render
    public void RenderEmailFolders()
    {
        var html = new StringBuilder();

        // Get the data
        var service = new MessagesService();
        var folders = service.GetEmailFolders();


        // Group the data
        var standardFolderTypes = new List<int> { 1, 2, 3, 4 };
        var standardFolders = folders.Where(c => standardFolderTypes.Contains(c.MailFolderTypeID));

        var personalFolderTypes = new List<int> { 0 };
        var personalFolders = folders.Where(c => personalFolderTypes.Contains(c.MailFolderTypeID));


        // Render the standard folders first
        html.AppendFormat("<ul class='nav nav-pills nav-stacked'>");
        foreach(var folder in standardFolders)
        {
            // Determine if we have any uinread messages in this folder
            var hasUnreadMessages = folder.UnreadCount > 0;
            var cssClass = (hasUnreadMessages) ? "active" : string.Empty;
            var unreadCountDisplay = (hasUnreadMessages) ? string.Format("&nbsp;({0})", folder.UnreadCount) : string.Empty;
            
            // Render the list item
            html.AppendFormat("<li class='{0}'><a href='Messages.aspx?f={3}'>{1}{2}</a></li>",
                cssClass,
                folder.Name,
                unreadCountDisplay,
                folder.MailFolderID);
        }
        html.AppendFormat("</ul>");


        // Render the personal folders next
        html.AppendFormat("<ul class='nav nav-pills nav-stacked'>");
        foreach(var folder in personalFolders)
        {
            // Determine if we have any uinread messages in this folder
            var hasUnreadMessages = folder.UnreadCount > 0;
            var cssClass = (hasUnreadMessages) ? "active" : string.Empty;
            var unreadCountDisplay = (hasUnreadMessages) ? string.Format("&nbsp;({0})", folder.UnreadCount) : string.Empty;
            
            // Render the list item
            html.AppendFormat("<li class='{0}'><a href='Messages.aspx?f={3}'>{1}{2}</a></li>",
                cssClass,
                folder.Name,
                unreadCountDisplay,
                folder.MailFolderID);
        }

        html.AppendFormat("</ul>");


        // Write the HTML to the screen
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderMailDate()
    {
        var date = ViewModel.MailDate;
        var html = new StringBuilder();

        if(date.Date == DateTime.Now.Date)
        {
            // Add the time
            html.AppendFormat("{0:h:mm tt}", date);


            // Add the length of time that has passed
            var datedifference = DateTime.Now.Subtract(date);
            if(Math.Floor(datedifference.TotalMinutes) < 1)                 html.AppendFormat(" ({0:0} minute ago) ", datedifference.TotalMinutes);
            else if(datedifference.TotalMinutes < 60)                       html.AppendFormat(" ({0:0} minutes ago) ", datedifference.TotalMinutes);
            else if(Math.Floor(datedifference.TotalHours) == 1)             html.AppendFormat(" ({0:0} hour ago) ", datedifference.TotalHours);
            else if(datedifference.TotalHours < 24)                         html.AppendFormat(" ({0:0} hours ago) ", datedifference.TotalHours);
        }
        else
        {
            html.AppendFormat("{0:MMMM d, yyyy}", date);
        }

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderMoveToEmailFolders()
    {
        var html = new StringBuilder();

        // Get the data
        var service = new MessagesService();
        var folders = service.GetEmailFolders();


        // Group the data
        var moveToFolderTypes = new List<int> { 0, 1 };
        var mvoeToFolders = folders.Where(c => moveToFolderTypes.Contains(c.MailFolderTypeID));


        // Render the personal folders next
        html.AppendFormat("<ul class='dropdown-menu'>");

        if(mvoeToFolders.Count() > 0)
        {
            foreach(var folder in mvoeToFolders)
            {            
                // Render the list item
                html.AppendFormat("<li><a href='javascript:;' onclick='messagelist.moveSelectedIDsToFolder({0}, {2})'>{1}</a></li>",
                    folder.MailFolderID,
                    folder.Name,
                    ViewModel.MailID);
            }

            // Render the divider
            html.AppendFormat("<li class='divider'></li>");
        }
            
        // Finally, render a 'Create new folder' link
        html.AppendFormat("<li><a href='javascript:;' onclick='folders.openCreateModal()'>Create new...</a></li>");

        html.AppendFormat("</ul>");


        // Write the HTML to the screen
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderAttachments()
    {
        // If we don't have any attachments, don't render anything.
        if(!ViewModel.HasAttachment) return;

        var html = new StringBuilder();

        html.AppendFormat("<hr />");
        html.AppendFormat("<strong>{0} {1}</strong>",
            ViewModel.Attachments.Count,
            (ViewModel.Attachments.Count == 1) ? "Attachment" : "Attachments");
        
                
        html.AppendFormat("<ul>");
        foreach(var attachment in ViewModel.Attachments)
        {
            // Determine the size of the attachment.
            var sizeDisplay = string.Empty;
            if(attachment.ContentLength < 1024)                 sizeDisplay = "1K";
            else                                                sizeDisplay = (attachment.ContentLength / 1024) + "K";


            html.AppendFormat("<li><a href='MessageAttachment.ashx?id={2}&a={3}'>{0}</a> <small>({1})</small></li>",
                attachment.FileName,
                sizeDisplay,
                attachment.MailID,
                attachment.AttachmentID);
        }
        html.AppendFormat("</ul>");

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        var action = eventArgument.Split('|')[0];
        var argument = eventArgument.Split('|')[1];

        var service = new MessagesService();

        switch(action)
        {
            case "reply":
                break;

            case "forward":
                break;

            case "markselectedasunread":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToBeMarkedAsUnread = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.UpdateEmailsStatus(idsToBeMarkedAsUnread, MailStatusType.New);
                Response.Redirect("Messages.aspx");
                break;

            case "movetodeleted":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToMoveToDeleted = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.MoveEmails(idsToMoveToDeleted, 4);
                Response.Redirect("Messages.aspx");
                break;

            case "movetofolder":
                var moveToFolderID = Convert.ToInt32(argument);
                var idsToMove = Array.ConvertAll(eventArgument.Split('|')[2].Split(','), s => int.Parse(s));

                service.MoveEmails(idsToMove, moveToFolderID);
                Response.Redirect("Messages.aspx");
                break;

            case "movetonewfolder":
                var newFolderDescription = argument;
                var idsToMoveToNewFolder = Array.ConvertAll(eventArgument.Split('|')[2].Split(','), s => int.Parse(s));

                // First, create the new folder.
                var newFolderID = service.CreatePersonalEmailFolder(newFolderDescription);

                // Next, move the selected emails to the new folder.
                service.MoveEmails(idsToMoveToNewFolder, newFolderID);

                Response.Redirect("Messages.aspx");
                break;
        }
    }
    #endregion
}