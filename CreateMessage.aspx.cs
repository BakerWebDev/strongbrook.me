using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CreateMessage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            PopulateFromDropdown();
            PopulatetxtToField();
            PopulateReplyAndForwardingContentIfApplicable();
        }
    }

    #region Properties
    public string PreferredFromCookieName = "FromPreference";

    public FlowType Flow
    {
        get
        {
            if(ForwardMailID != 0) return FlowType.Forward;
            else if(ReplyMailID != 0) return FlowType.Reply;
            else return FlowType.New;
        }
    }
    public enum FlowType
    {
        New = 0,
        Reply = 1,
        Forward = 2
    }

    public Email ExistingEmail
    {
        get
        {
            if(_existingEmail == null)
            {
                int mailID = (Flow == FlowType.Reply) ? ReplyMailID : ForwardMailID;

                var service = new MessagesService();
                _existingEmail = service.GetEmail(mailID);
            }
            return _existingEmail;
        }
    }
    private Email _existingEmail;

    public int ForwardMailID
    {
        get { return (Request.QueryString["fid"] != null) ? Convert.ToInt32(Request.QueryString["fid"]) : 0; }
    }
    public int ReplyMailID
    {
        get { return (Request.QueryString["rid"] != null) ? Convert.ToInt32(Request.QueryString["rid"]) : 0; }
    }
    public string emailTo
    {
        get { return (Request.QueryString["to"] != null) ? Request.QueryString["to"] : txtTo.Text; }
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
    public void RenderForwardedAttachments()
    {
        // If this isn't a forwarded email, or if we don't have any attachments in the forwarding email, stop here.
        if(Flow != FlowType.Forward || !ExistingEmail.HasAttachment) return;
        
        var html = new StringBuilder();
        

        var existingAttachmentsCount = 0;
        foreach(var attachment in ExistingEmail.Attachments)
        {
            html.AppendFormat(@"
                    <div class='checkbox'>
                        <input id='chkForwardedAttachment{0}' name='chkForwardedAttachment{0}' type='checkbox' checked='true' value='{1}|{2}' />
                        <label for='chkForwardedAttachment{0}'>{3}</label>
                    </div>
                ",
                 existingAttachmentsCount,
                 attachment.MailID,
                 attachment.AttachmentID,
                 attachment.FileName);

            existingAttachmentsCount++;
        }


        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #endregion

    #region Event Handlers
    public void SendMessage_Click(object sender, EventArgs e)
    {
        // Save the From email address preference
        SetFromPreferenceCookie();


        // First, send the email to the recipient
        var request = new CreateEmailRequest();

        // Get the customer ID this is supposed to go to.
        var toCustomer = ExigoApiContext.CreateODataContext().UniLevelTree //EmailFromSettings
            .Where(c => c.Customer.Email == emailTo) // txtTo.Text) //.MailFrom == txtTo.Text)
            .FirstOrDefault();
        if(toCustomer != null)
        {
            request.CustomerID = toCustomer.CustomerID;
        }

        // Settings
        request.Priority = MailPriority.Normal;
        request.MailFolderType = MailForderType.Inbox;
        request.MailStatusType = MailStatusType.New;

        // Content
        request.MailFrom = lstFrom.SelectedValue;
        request.MailTo = txtTo.Text;
        request.ReplyTo = txtTo.Text;
        request.Subject = txtSubject.Text;
        request.Content = txtMessage.Text;

        // Attachments
        request.Attachments = GetNewAttachments();
        //request.ForwardedAttachments = GetForwardedAttachments();

        var response = ExigoApiContext.CreateWebServiceContext().CreateEmail(request);




        // Next, send the email to the backoffice owner's Sent Items folder.        
        var selfrequest = new CreateEmailRequest();

        // Settings
        selfrequest.CustomerID = Identity.Current.CustomerID;
        selfrequest.Priority = MailPriority.Normal;
        selfrequest.MailFolderType = MailForderType.SentItems;
        selfrequest.MailStatusType = MailStatusType.Read;

        // Content
        selfrequest.MailFrom = lstFrom.SelectedValue;
        selfrequest.MailTo = txtTo.Text;
        selfrequest.ReplyTo = txtTo.Text;
        selfrequest.Subject = txtSubject.Text;
        selfrequest.Content = txtMessage.Text;

        // Attachments
        selfrequest.Attachments = GetNewAttachments();
        //selfrequest.ForwardedAttachments = GetForwardedAttachments();

        var selfresponse = ExigoApiContext.CreateWebServiceContext().CreateEmail(selfrequest);



        Response.Redirect("Messages.aspx");
    }
    #endregion

    #region Attachments
    public Exigo.WebService.EmailAttachment[] GetNewAttachments()
    {
        var details = new List<Exigo.WebService.EmailAttachment>();

        if(uploadAttachment1.HasFile)
        {
            var attachment = new Exigo.WebService.EmailAttachment();
            attachment.FileName = uploadAttachment1.FileName;
            attachment.ContentLength = uploadAttachment1.FileBytes.Length;
            attachment.BinaryData = uploadAttachment1.FileBytes;
            details.Add(attachment);
        }

        if(uploadAttachment2.HasFile)
        {
            var attachment = new Exigo.WebService.EmailAttachment();
            attachment.FileName = uploadAttachment2.FileName;
            attachment.ContentLength = uploadAttachment2.FileBytes.Length;
            attachment.BinaryData = uploadAttachment2.FileBytes;
            details.Add(attachment);
        }

        if(uploadAttachment3.HasFile)
        {
            var attachment = new Exigo.WebService.EmailAttachment();
            attachment.FileName = uploadAttachment3.FileName;
            attachment.ContentLength = uploadAttachment3.FileBytes.Length;
            attachment.BinaryData = uploadAttachment3.FileBytes;
            details.Add(attachment);
        }

        if(uploadAttachment4.HasFile)
        {
            var attachment = new Exigo.WebService.EmailAttachment();
            attachment.FileName = uploadAttachment4.FileName;
            attachment.ContentLength = uploadAttachment4.FileBytes.Length;
            attachment.BinaryData = uploadAttachment4.FileBytes;
            details.Add(attachment);
        }

        if(uploadAttachment5.HasFile)
        {
            var attachment = new Exigo.WebService.EmailAttachment();
            attachment.FileName = uploadAttachment5.FileName;
            attachment.ContentLength = uploadAttachment5.FileBytes.Length;
            attachment.BinaryData = uploadAttachment5.FileBytes;
            details.Add(attachment);
        }

        return details.ToArray();
    }
    //public Exigo.WebService.ForwardedAttachment[] GetForwardedAttachments()
    //{
    //    var details = new List<Exigo.WebService.ForwardedAttachment>();
    //    var forwardedAttachmentCount = ExistingEmail.Attachments.Count;

    //    for(var x = 0; x < forwardedAttachmentCount; x++)
    //    {
    //        var checkbox = Request.Form["chkForwardedAttachment" + x];

    //        if(checkbox != null)
    //        {
    //            var mailID = Convert.ToInt32(checkbox.Split('|')[0]);
    //            var attachmentID = Convert.ToInt32(checkbox.Split('|')[1]);

    //            var attachment = new ForwardedAttachment();
    //            attachment.MailID = mailID;
    //            attachment.AttachmentID = attachmentID;
    //            details.Add(attachment);
    //        }
    //    }

    //    return details.ToArray();
    //}
    #endregion

    #region Populating Form Options
    public void PopulateFromDropdown()
    {
        // Get the data
        var settings = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => c);


        // Populate the dropdown
        lstFrom.Items.Clear();
        foreach(var setting in settings)
        {
            var item = new ListItem();
            item.Text = setting.FirstName + " " + setting.LastName; // setting.Email;
            item.Value = setting.Email;
            lstFrom.Items.Add(item);
        }

        if(!string.IsNullOrEmpty(GetFromPreferenceCookie()))
        {
            var preferredFromItem = lstFrom.Items.Cast<ListItem>().Where(c => c.Value == GetFromPreferenceCookie()).Select(c => c).FirstOrDefault();
            if(preferredFromItem != null)
            {
                preferredFromItem.Selected = true;
            }
        }
    }

    public void PopulatetxtToField()
    {
        if(emailTo != null)
        {
            txtTo.Text = emailTo;
        }
    }

    public void PopulateReplyAndForwardingContentIfApplicable()
    {
        // If this is a new email, stop here.
        if(Flow == FlowType.New) return;
        
        // Populate the default options, starting with the To setting.
        if(Flow == FlowType.Reply)
        {
            txtTo.Text = ExistingEmail.MailFrom;
        }

        // Populate the subject line
        switch(Flow)
        {
            case FlowType.Reply: txtSubject.Text = "RE:" + ExistingEmail.Subject; break;
            case FlowType.Forward: txtSubject.Text = "FW:" + ExistingEmail.Subject; break;
        }

        // Populate the body
        switch(Flow)
        {
            case FlowType.Reply: 
                txtMessage.Text = string.Format(@"
                        <br /><br /><br />
                        On {0:MMMM d, yyyy} at {0:h:mm tt}, <a href='mailto:{1}'>{1}</a> wrote:
                        <div style='padding-left: 15px; border-left: 1px solid #CCC;'>
                            {2}
                        </div>",
                    ExistingEmail.MailDate,
                    ExistingEmail.MailFromDisplay,
                    ExistingEmail.Body);
                break;
                break;
            case FlowType.Forward: 
                txtMessage.Text = string.Format(@"
                        <br /><br /><br />
                        <div style='padding-left: 15px;'>
                            ---------- Forwarded message ----------<br />
                            From: <a href='mailto:{0}'>{0}</a><br />
                            Date: {1:MMMM d, yyyy h:mm tt}<br />
                            Subject: {2}<br />
                            To: <a href='mailto:{3}'>{3}</a><br />
                            <br />
                            {4}
                        </div>",
                    ExistingEmail.MailFromDisplay,
                    ExistingEmail.MailDate,
                    ExistingEmail.Subject,
                    ExistingEmail.MailTo,
                    ExistingEmail.Body);
                break;
        }
    }
    #endregion

    #region Helper Methods
    // Cookie methods
    public string GetFromPreferenceCookie()
    {
        var cookie = Request.Cookies[PreferredFromCookieName];
        if(cookie == null)
        {
            cookie = new HttpCookie(PreferredFromCookieName);
            cookie.Value = lstFrom.SelectedValue;
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
        }

        return cookie.Value;
    }
    public void SetFromPreferenceCookie()
    {
        var cookie = Request.Cookies[PreferredFromCookieName];
        if(cookie == null)
        {
            cookie = new HttpCookie(PreferredFromCookieName);
        }
        cookie.Value = lstFrom.SelectedValue;
        cookie.Expires = DateTime.Now.AddYears(1);

        Response.Cookies.Add(cookie);
    }
    #endregion
}