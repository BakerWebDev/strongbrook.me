using Exigo.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ManageEvent : System.Web.UI.Page
{
    public int MinuteStep = 15;
    public const string URL_CALENDAR = "Calendar.aspx";
    public const string URL_CALENDARDETAILS = "CalendarDetails.aspx";




    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            // Ensure we have at least one calendar. If not, let's move them back to the calendar and explain that this feature is not yet ready.
            var service = new CalendarService();
            var calendars = service.GetCalendars();
            if(calendars.First().CalendarID == 0)
            {
                Response.Redirect(URL_CALENDAR + "?status=0");
            }

            // Ensure that, if we are editing an event, it is the backoffice owner's event.
            if(CalendarItemID != 0)
            {
                if(!service.ValidateCalendarItem(CalendarItemID))
                {
                    Response.Redirect(URL_CALENDARDETAILS + "?id=" + CalendarItemID);
                }
            }

            PopulateCalendarOptions();
            PopulateCalendarItemRepeatTypeOptions();
            PopulateCalendarItemTypeOptions();
            PopulateDefaultFormOptions();
            PopulateTimeZones();
            PopulateExistingData();
        }
    }

    #region Properties
    public int CalendarID
    {
        get { return Convert.ToInt32(lstCalendar.SelectedValue); }
        set { lstCalendar.SelectedValue = value.ToString(); }
    }
    public int CalendarItemID
    {
        get { return (Request.QueryString["id"] != null) ? Convert.ToInt32(Request.QueryString["id"]) : 0; }
    }
    public DateTime FormattedStartDate
    {
        get 
        {
            return Convert.ToDateTime(StartDate + " " + StartTime);
        }
        set
        {
            StartDate = value.ToString("dddd, MMMM d, yyyy");
            StartTime = value.ToString("h:mm tt");
        }
    }
    public string StartDate
    {
        get { return txtStartDate.Text; }
        set { txtStartDate.Text = value; }
    }
    public string StartTime
    {
        get { return txtStartTime.Text; }
        set { txtStartTime.Text = value; }
    }
    public DateTime FormattedEndDate
    {
        get 
        {
            return Convert.ToDateTime(EndDate + " " + EndTime);
        }
        set
        {
            EndDate = value.ToString("dddd, MMMM d, yyyy");
            EndTime = value.ToString("h:mm tt");
        }
    }
    public string EndDate
    {
        get { return txtEndDate.Text; }
        set { txtEndDate.Text = value; }
    }
    public string EndTime
    {
        get { return txtEndTime.Text; }
        set { txtEndTime.Text = value; }
    }
    public string TimeZoneOffset
    {
        get { return lstTimeZone.SelectedValue; }
        set { lstTimeZone.SelectedValue = value; }
    }
    public string EventTitle
    {
        get { return txtTitle.Text; }
        set { txtTitle.Text = value; }
    }
    public string Description
    {
        get { return txtSummary.Text; }
        set { txtSummary.Text = value; }
    }
    public string Location
    {
        get { return txtLocation.Text; }
        set { txtLocation.Text = value; }
    }
    public bool AllDay
    {
        get { return chkAllDay.Checked; }
        set { chkAllDay.Checked = value; }
    }
    public int CalendarItemTypeID
    {
        get { return Convert.ToInt32(lstCalendarItemType.SelectedValue); }
        set { lstCalendarItemType.SelectedValue = value.ToString(); }
    }
    public int CalendarPrivacyTypeID
    {
        get { return (chkIsPublic.Checked) ? CalendarPrivacyTypes.Public : CalendarPrivacyTypes.Private; }
        set { chkIsPublic.Checked = (value == CalendarPrivacyTypes.Public); }
    }
    public int CalendarItemRepeatTypeID
    {
        get { return Convert.ToInt32(lstCalendarItemRepeatTypes.SelectedValue); }
        set { lstCalendarItemRepeatTypes.SelectedValue = value.ToString(); }
    }

    public int NewCalendarItemID { get; set; }
    #endregion

    #region Saving Changes
    public void SaveChanges_Click(object sender, EventArgs e)
    {
        // If we are creating a new event...
        if(CalendarItemID == 0)
        {
            CreateNewItem();
            Response.Redirect(URL_CALENDAR + "?status=1");
        }

        // If we are updating an existing event...
        else
        {
            UpdateExistingItem();
            Response.Redirect(URL_CALENDARDETAILS + "?id=" + NewCalendarItemID + "&status=1");
        }
    }

    public void CreateNewItem()
    {
        var request = new CreateCalendarItemRequest();
        request.CustomerID                          = Identity.Current.CustomerID;
        request.CalendarID                          = CalendarID;
        request.CalendarItemStatusID                = CalendarItemStatuses.Active;
        request.CalendarItemTypeID                  = CalendarItemTypeID;
        request.CalendarItemRepeatTypeID            = CalendarItemRepeatTypeID;
        request.AllDay                              = AllDay;
        request.CalendarPrivacyTypeID               = CalendarPrivacyTypeID;

        request.Title                               = EventTitle;
        request.Description                         = Description;
        request.Location                            = Location;
        request.StartDate                           = FormattedStartDate;
        request.EndDate                             = FormattedEndDate;
        request.TimeZoneOffset                      = TimeZoneOffset;
        request.IsCorporate                         = false;

        var service = new CalendarService();
        NewCalendarItemID = service.CreateCalendarItem(request);
    }
    public void UpdateExistingItem()
    {
        var request = new UpdateCalendarItemRequest();
        request.CalendarItemID                      = CalendarItemID;
        request.CalendarID                          = CalendarID;
        request.CalendarItemStatusID                = CalendarItemStatuses.Active;
        request.CalendarItemTypeID                  = CalendarItemTypeID;
        request.CalendarItemRepeatTypeID            = CalendarItemRepeatTypeID;
        request.AllDay                              = AllDay;
        request.CalendarPrivacyTypeID               = CalendarPrivacyTypeID;

        request.Title                               = EventTitle;
        request.Description                         = Description;
        request.Location                            = Location;
        request.StartDate                           = FormattedStartDate;
        request.EndDate                             = FormattedEndDate;
        request.TimeZoneOffset                      = TimeZoneOffset;
        request.IsCorporate                         = false;

        var service = new CalendarService();
        service.UpdateCalendarItem(request);

        NewCalendarItemID = CalendarItemID;
    }
    #endregion

    #region Populate Form Options
    private void PopulateCalendarOptions()
    {
        var service = new CalendarService();
        var nodes = service.GetCalendars();


        lstCalendar.Items.Clear();
        foreach(var node in nodes)
        {
            var item = new ListItem();
            item.Text = node.Description;
            item.Value = node.CalendarID.ToString();
            lstCalendar.Items.Add(item);
        }
    }
    private void PopulateCalendarItemRepeatTypeOptions()
    {
        var service = new CalendarService();
        var nodes = service.GetCalendarItemRepeatTypes();


        lstCalendarItemRepeatTypes.Items.Clear();
        foreach(var node in nodes)
        {
            var itemText = "";
            switch(node.CalendarItemRepeatTypeID)
            {
                case CalendarItemRepeatTypes.None: itemText = "Do not repeat this event"; break;
                case CalendarItemRepeatTypes.Daily: itemText = "Every day"; break;
                case CalendarItemRepeatTypes.Weekly: itemText = "Every week"; break;
                case CalendarItemRepeatTypes.BiWeekly: itemText = "Every two weeks"; break;
                case CalendarItemRepeatTypes.Monthly: itemText = "Every month"; break;
                case CalendarItemRepeatTypes.Yearly: itemText = "Every year"; break;
            }


            var item = new ListItem();
            item.Text = itemText;
            item.Value = node.CalendarItemRepeatTypeID.ToString();
            lstCalendarItemRepeatTypes.Items.Add(item);
        }
    }
    private void PopulateCalendarItemTypeOptions()
    {
        var service = new CalendarService();
        var nodes = service.GetCalendarItemTypes();


        lstCalendarItemType.Items.Clear();
        foreach(var node in nodes)
        {
            var item = new ListItem();
            item.Text = node.CalendarItemTypeDescription;
            item.Value = node.CalendarItemTypeID.ToString();
            lstCalendarItemType.Items.Add(item);
        }
    }
    private void PopulateTimeZones()
    {
        lstTimeZone.Items.Clear();        
        lstTimeZone.Items.Add(new ListItem { Text = "Eastern Standard Time (EST)", Value = "-0500" });
        lstTimeZone.Items.Add(new ListItem { Text = "Central Standard Time (CST)", Value = "-0600" });
        lstTimeZone.Items.Add(new ListItem { Text = "Mountain Standard Time (MST)", Value = "-0700" });
        lstTimeZone.Items.Add(new ListItem { Text = "Pacific Standard Time (PST)", Value = "-0800" });
        lstTimeZone.Items.Add(new ListItem { Text = "Greenwich Mean Time (GMT)", Value = "-000" });
    }
    private void PopulateDefaultFormOptions()
    {
        // This only applies to creating new events
        if(CalendarItemID == 0)
        {
            var defaultStartDate = DateTime.Now;
            if(defaultStartDate.Minute % MinuteStep != 0)
            {
                defaultStartDate = defaultStartDate.AddMinutes(MinuteStep - (defaultStartDate.Minute % MinuteStep));
            }

            var defaultEndDate = defaultStartDate.AddHours(1);


            txtStartDate.Text = defaultStartDate.ToString("dddd, MMMM d, yyyy");
            txtStartTime.Text = defaultStartDate.ToString("h:mm tt");
            txtEndDate.Text = defaultEndDate.ToString("dddd, MMMM d, yyyy");
            txtEndTime.Text = defaultEndDate.ToString("h:mm tt");
        }
    }

    private void PopulateExistingData()
    {
        if(CalendarItemID == 0) return;

        var service = new CalendarService();
        var item = service.GetCalendarItem(CalendarItemID);
        if(item == null) Response.Redirect(URL_CALENDAR);


        FormattedStartDate = ((DateTime)item.StartDate);
        FormattedEndDate = ((DateTime)item.EndDate);
        TimeZoneOffset = item.TimeZoneOffset;
        CalendarItemRepeatTypeID = ((int)item.CalendarItemRepeatTypeID);
        AllDay = ((bool)item.AllDay);
        CalendarPrivacyTypeID = ((int)item.CalendarPrivacyTypeID);

        CalendarID = ((int)item.CalendarID);
        CalendarItemTypeID = ((int)item.CalendarItemTypeID);
        EventTitle = item.Title;
        Location = item.Location;
        Description = item.Description;
    }
    #endregion
}