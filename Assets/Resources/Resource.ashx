<%@ WebHandler Language="C#" Class="Resources" %>

using System;
using System.Web;
using Exigo.GlobalResources;

public class Resources : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        // Capture our query string variables
        var name = context.Request.QueryString["name"] ?? "resources";
        var path = context.Request.QueryString["path"] ?? "Resources";
        
        
        // Clean up any references to .resx - our code enters that automatically.
        if(path.Contains(".resx")) path = path.Replace(".resx", "");
        
        
        // Set the content type of the document to javascript
        context.Response.ContentType = "text/javascript";        
        
        
        // Create our factory
        var service = new GlobalResourcesToJavaScriptService();
        service.JavaScriptObjectName = name;
        service.GlobalResXFileName = path;
        
        
        // Write our javascript to the page.
        context.Response.Write(service.GetJavaScript());
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}