using Exigo.Calendars;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

public partial class Calendar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["status"] == "1")
        {                
            Error.Type = Exigo.WebControls.ErrorMessageType.Success;
            Error.Header = "Success!";
            Error.Message = "Your changes have been saved. Please allow up to one minute for your calendar to reflect your changes.";
        }

        // Ensure at least one calendar
        var service = new CalendarService();
        service.EnsureAtLeastOneCalendar();
    }

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if(Request.QueryString["action"] != null)
        {
            var start = Request.QueryString["start"];
            var end = Request.QueryString["end"];
            var filter = Request.QueryString["filter"];

            var service = new CalendarService();
            var json = service.GetDataAsJson(start, end, filter);

            // Write the JSON
            Response.Clear();
            writer.Write(json);
            Response.End();

            // Write the JSON
            Response.Clear();
            writer.Write(json);
            Response.End();
        }
        else
        {
            base.Render(writer);
        }
    }

    private string GetEventDateSpan(DateTime start, DateTime end)
    {
        var result = "";

        if(start.Date == end.Date)
        {
            result = string.Format("{0:h:mm tt} - {1:h:mm tt}", start, end);
        }
        else
        {
            result = string.Format("Starts: {0:dddd, MMMM d, yyyy h:mm tt}<br />Ends: {1:dddd, MMMM d, yyyy h:mm tt}", start, end);
        }

        return result;
    }
    #endregion
}