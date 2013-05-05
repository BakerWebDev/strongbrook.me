using Exigo.Calendars;
using Exigo.OData.Extended;


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


public partial class CalendarDetails : System.Web.UI.Page, IPostBackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(CalendarItemID == 0 || Event == null)
        {
            Response.Redirect("Calendar.aspx");
        }

        if (Request.QueryString["status"] == "1")
        {                
            Error.Type = Exigo.WebControls.ErrorMessageType.Success;
            Error.Header = "Success!";
            Error.Message = "Your changes have been saved. Please allow up to one minute for your calendar to reflect your changes.";
        }
    }

    #region Properties
    public int CalendarItemID
    {
        get { return (Request.QueryString["id"] != null) ? Convert.ToInt32(Request.QueryString["id"]) : 0; }
    }

    public CalendarItem Event
    {
        get
        {
            if(_event == null)
            {
                var service = new CalendarService();
                _event = service.GetCalendarItem(CalendarItemID);
            }
            return _event;
        }
    }
    private CalendarItem _event;
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        if(eventArgument == "delete")
        {
            var service = new CalendarService();
            service.DeleteCalendarItem(CalendarItemID);
            Response.Redirect("Calendar.aspx?status=1");
        }

        if(eventArgument == "markasprivate")
        {
            var service = new CalendarService();
            service.MarkCalendarItemAsPrivate(CalendarItemID);
            Response.Redirect("CalendarDetails.aspx?id=" + CalendarItemID + "&status=1");
        }

        if(eventArgument == "markaspublic")
        {
            var service = new CalendarService();
            service.MarkCalendarItemAsPublic(CalendarItemID);
            Response.Redirect("CalendarDetails.aspx?id=" + CalendarItemID + "&status=1");
        }
    }
    #endregion
}