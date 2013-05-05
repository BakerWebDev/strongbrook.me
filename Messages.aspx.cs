using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Messages : System.Web.UI.Page, IPostBackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Force __doPostBack javascript functions to work
        Page.ClientScript.GetPostBackClientHyperlink(this, "");
    }

    #region Properties
    public int FolderID
    {
        get
        {
            return (Request.QueryString["f"] != null) ? Convert.ToInt32(Request.QueryString["f"]) : 1;
        }
    }
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
                html.AppendFormat("<li><a href='javascript:;' onclick='messagelist.moveSelectedIDsToFolder({0})'>{1}</a></li>",
                    folder.MailFolderID,
                    folder.Name);
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
    public void RenderEmails()
    {
        var html = new StringBuilder();


        // Get the data
        var service = new MessagesService();
        var emails = service.GetEmails(FolderID);

        var settings = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => c);
        
        // Assemble the emails list
        html.AppendFormat("<table class='table messageslist'>");

        if(emails.Count == 0)
        {
            html.AppendFormat("<tr><td colspan='5'><p class='nomessages'>(no messages to display)</p></td></tr>");
        }
        else
        {
            foreach(var email in emails)
            {
                var statusCssClass = (email.MailStatusTypeID == 0) ? "status-unread" : "status-read";
                var hasAttachmentsDisplay = (email.HasAttachment) ? "<i class='icon-file'></i>" : string.Empty;

                var formattedDate = email.MailDate.ToString("MMM d");
                if(email.MailDate.Date == DateTime.Now.Date) formattedDate = email.MailDate.ToString("h:mm tt");

                html.AppendFormat(@"
                        <tr class='{0}' data-id='{1}'>
                            <td class='options'>
                                <input type='checkbox' /></td>
                            <td class='details'>{2}</td>
                            <td class='from clickable'>
                                {3}</td>
                            <td class='summary clickable'>{4}</td>
                            <td class='received clickable'>{5}</td>
                        </tr>",
                    statusCssClass,
                    email.MailID,
                    hasAttachmentsDisplay,
                    email.MailFrom,
                    email.Subject,
                    formattedDate
                );
            }
        }

        html.AppendFormat("</table>");


        // Render the emails
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
            case "viewmessage":
                service.UpdateEmailsStatus(new int[] {Convert.ToInt32(argument)}, MailStatusType.Read);
                Response.Redirect("ViewMessage.aspx?id=" + argument);
                break;

            case "markallasread":
                var allIDs = service.GetAllEmailIDs(MailStatusType.New, FolderID).ToArray();
                service.UpdateEmailsStatus(allIDs, MailStatusType.Read);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "markselectedasread":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToBeMarkedAsRead = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.UpdateEmailsStatus(idsToBeMarkedAsRead, MailStatusType.Read);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "markselectedasunread":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToBeMarkedAsUnread = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.UpdateEmailsStatus(idsToBeMarkedAsUnread, MailStatusType.New);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "movetodeleted":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToMoveToDeleted = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.MoveEmails(idsToMoveToDeleted, 4);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "deletedrafts":
                if(string.IsNullOrEmpty(argument)) return;

                var draftsToPermanentlyDelete = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.PermanentlyDeleteEmails(draftsToPermanentlyDelete);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "deleteemails":
                if(string.IsNullOrEmpty(argument)) return;

                var idsToPermanentlyDelete = Array.ConvertAll(argument.Split(','), s => int.Parse(s));
                service.PermanentlyDeleteEmails(idsToPermanentlyDelete);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "emptytrash":
                var allTrashIDs = service.GetAllEmailIDs(FolderID).ToArray();
                service.PermanentlyDeleteEmails(allTrashIDs);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "movetofolder":
                var moveToFolderID = Convert.ToInt32(argument);
                var idsToMove = Array.ConvertAll(eventArgument.Split('|')[2].Split(','), s => int.Parse(s));

                service.MoveEmails(idsToMove, moveToFolderID);
                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "movetonewfolder":
                var newFolderDescription = argument;
                var idsToMoveToNewFolder = Array.ConvertAll(eventArgument.Split('|')[2].Split(','), s => int.Parse(s));

                // First, create the new folder.
                var newFolderID = service.CreatePersonalEmailFolder(newFolderDescription);

                // Next, move the selected emails to the new folder.
                service.MoveEmails(idsToMoveToNewFolder, newFolderID);

                Response.Redirect(Request.Url.AbsoluteUri);
                break;
        }
    }
    #endregion
}