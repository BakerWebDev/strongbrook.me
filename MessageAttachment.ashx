<%@ WebHandler Language="C#" Class="MessageAttachment" %>

using System;
using System.Web;
using Exigo.WebService;

public class MessageAttachment : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        // If we are missing any variables, stop here. We need it all.
        if(context.Request.QueryString["id"] == null || context.Request.QueryString["a"] == null) return;
        
        // Store the provided variables for easier access
        var mailID = Convert.ToInt32(context.Request.QueryString["id"]);
        var attachmentID = Convert.ToInt32(context.Request.QueryString["a"]);
        
        // Get the attachment data from the web service
        var response = ExigoApiContext.CreateWebServiceContext().GetEmailAttachment(new GetEmailAttachmentRequest
        {
            CustomerID = Identity.Current.CustomerID,
            MailID = mailID,
            AttachmentID = attachmentID
        });
        
        // Determine the response's content type
        var extension = "." + response.Attachment.FileName.Split('.')[response.Attachment.FileName.Split('.').Length - 1].ToLower();
        var contentType = "text/plain";        
        try
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
            contentType = key.GetValue("Content Type").ToString();
        }
        catch {}
        
        
        // Write the binary data to the page.
        context.Response.ContentType = contentType;
        context.Response.AddHeader("content-disposition", @"attachment;filename=""" + response.Attachment.FileName + @"""");
        context.Response.BinaryWrite(response.Attachment.BinaryData);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}