using Exigo.OData.Extended;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;

namespace Exigo.Calendars
{
    public class CalendarService
    {
        private string DefaultPersonalCalendarDescription = "Personal Calendar";
        private string CalendarDetailsUrl = "CalendarDetails.aspx";




        #region Constructors
        public CalendarService()
        {

        }
        #endregion

        #region Fetching Data
        public List<Calendar> GetCalendars()
        {
            return GetCalendars(Identity.Current.CustomerID);
        }
        public List<Calendar> GetCalendars(int customerID)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var calendars = context.Calendars.Where(c => c.CustomerID == customerID).ToList();

            if(calendars.Count == 0)
            {
                EnsureAtLeastOneCalendar();
                calendars.Add(new Calendar
                {
                    CalendarID = 0,
                    CustomerID = Identity.Current.CustomerID,
                    CalendarPrivacyTypeID = CalendarPrivacyTypes.Public,
                    Description = DefaultPersonalCalendarDescription
                });
            }

            return calendars;
        }

        public CalendarItem GetCalendarItem(int calendarItemID)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var item = context.CalendarItems
                .Where(c => c.CalendarItemID == calendarItemID)
                .FirstOrDefault();

            return item;
        }
        public List<CalendarItem> GetCalendarItems(int calendarID)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarItems.Where(c => c.CalendarID == calendarID).ToList();

            return nodes;
        }
        public List<CalendarItem> GetCalendarItems(int calendarID, DateTime startDate, DateTime endDate)
        {
            return GetCalendarItems(calendarID, startDate, endDate, "all");
        }
        public List<CalendarItem> GetCalendarItems(int calendarID, DateTime startDate, DateTime endDate, string filter)
        {
            var nodes = new List<CalendarItem>();

            switch(filter)
            {
                case "all":
                    FetchCorporateCalendarItems(startDate, endDate).ForEach(c => nodes.Add(c));
                    FetchDistributorCalendarItems(startDate, endDate).ForEach(c => nodes.Add(c));
                    FetchPersonalCalendarItems(calendarID, startDate, endDate).ForEach(c => nodes.Add(c));
                    break;

                case "corporate":
                    FetchCorporateCalendarItems(startDate, endDate).ForEach(c => nodes.Add(c));
                    break;

                case "distributor":
                    FetchDistributorCalendarItems(startDate, endDate).ForEach(c => nodes.Add(c));
                    break;

                case "personal":
                default:
                    if(calendarID != 0) 
                    {
                        FetchPersonalCalendarItems(calendarID, startDate, endDate).ForEach(c => nodes.Add(c));
                    }
                    break;
            }

            return nodes;
        }

        private List<CalendarItem> FetchPersonalCalendarItems(int calendarID, DateTime startDate, DateTime endDate)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = new List<CalendarItem>();

            // Add the non-repeating nodes first
            var nonrepeatingNodesQuery = context.CalendarItems
                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                .Where(c => c.IsCorporate == false)                
                .Where(c => c.StartDate >= startDate)
                .Where(c => c.CalendarItemRepeatTypeID == CalendarItemRepeatTypes.None);
            if(calendarID != 0) nonrepeatingNodesQuery = nonrepeatingNodesQuery.Where(c => c.CalendarID == calendarID);
            var nonrepeatingNodes = nonrepeatingNodesQuery.ToList();
            nonrepeatingNodes.ForEach(c => nodes.Add(c));


            // Add the repeating nodes next
            var repeatingNodesQuery = context.CalendarItems
                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                .Where(c => c.IsCorporate == false)
                .Where(c => c.CalendarItemRepeatTypeID != CalendarItemRepeatTypes.None);
            if(calendarID != 0) repeatingNodesQuery = repeatingNodesQuery.Where(c => c.CalendarID == calendarID);
            var repeatingNodes = repeatingNodesQuery.ToList();

            var repeatingNodeInstances = CalculateRepeatingEvents(repeatingNodes, startDate, endDate);
            repeatingNodeInstances.ForEach(c => nodes.Add(c));


            return nodes;
        }
        private List<CalendarItem> FetchDistributorCalendarItems(DateTime startDate, DateTime endDate)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = new List<CalendarItem>();

            // Add the non-repeating nodes first
            var nonrepeatingNodes = context.CalendarItems
                .Where(c => c.CustomerID != Identity.Current.CustomerID)
                .Where(c => c.IsCorporate == false)
                .Where(c => c.CalendarPrivacyTypeID == CalendarPrivacyTypes.Public)
                .Where(c => c.StartDate >= startDate)
                .Where(c => c.CalendarItemRepeatTypeID == CalendarItemRepeatTypes.None)
                .ToList();
            nonrepeatingNodes.ForEach(c => nodes.Add(c));


            // Add the repeating nodes next
            var repeatingNodes = context.CalendarItems
                .Where(c => c.IsCorporate == false)
                .Where(c => c.CustomerID != Identity.Current.CustomerID)
                .Where(c => c.CalendarPrivacyTypeID == CalendarPrivacyTypes.Public)
                .Where(c => c.CalendarItemRepeatTypeID != CalendarItemRepeatTypes.None)
                .ToList();

            var repeatingNodeInstances = CalculateRepeatingEvents(repeatingNodes, startDate, endDate);
            repeatingNodeInstances.ForEach(c => nodes.Add(c));


            return nodes;
        }
        private List<CalendarItem> FetchCorporateCalendarItems(DateTime startDate, DateTime endDate)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = new List<CalendarItem>();

            // Add the non-repeating nodes first
            var nonrepeatingNodes = context.CalendarItems
                .Where(c => c.IsCorporate == true)
                .Where(c => c.CalendarPrivacyTypeID == CalendarPrivacyTypes.Public)
                .Where(c => c.StartDate >= startDate)
                .Where(c => c.CalendarItemRepeatTypeID == CalendarItemRepeatTypes.None)
                .ToList();
            nonrepeatingNodes.ForEach(c => nodes.Add(c));


            // Add the repeating nodes next
            var repeatingNodes = context.CalendarItems
                .Where(c => c.IsCorporate == true)
                .Where(c => c.CalendarPrivacyTypeID == CalendarPrivacyTypes.Public)
                .Where(c => c.CalendarItemRepeatTypeID != CalendarItemRepeatTypes.None)
                .ToList();

            var repeatingNodeInstances = CalculateRepeatingEvents(repeatingNodes, startDate, endDate);
            repeatingNodeInstances.ForEach(c => nodes.Add(c));


            return nodes;
        }

        private List<CalendarItem> CalculateRepeatingEvents(List<CalendarItem> repeatingTemplateItems, DateTime startDate, DateTime endDate)
        {
            var nodes = new List<Exigo.OData.Extended.CalendarItem>();

            foreach(var node in repeatingTemplateItems)
            {
                var originalStartDate = node.StartDate;
                var lengthOfEventInMinutes = ((DateTime)node.StartDate).Subtract(((DateTime)node.EndDate)).TotalMinutes;

                var totalDifferenceInDays = Math.Abs(Convert.ToInt32(Math.Floor(((DateTime)originalStartDate).Subtract(startDate).TotalDays)));
                var totalSkips = 0;
                var incrementInDays = 0;

                switch(node.CalendarItemRepeatTypeID)
                {
                    case CalendarItemRepeatTypes.Daily:
                        incrementInDays = 1;
                        break;
                    case CalendarItemRepeatTypes.Weekly:
                        incrementInDays = 7;
                        break;
                    case CalendarItemRepeatTypes.BiWeekly:
                        incrementInDays = 14;
                        break;
                    case CalendarItemRepeatTypes.Monthly:
                        incrementInDays = 28;
                        break;
                    case CalendarItemRepeatTypes.Yearly:
                        incrementInDays = 365;
                        break;
                }

                // Skip the original start date ahead so that we can start with a date closer to the start of our window of time.
                totalSkips = Convert.ToInt32(Math.Floor((decimal)totalDifferenceInDays / (decimal)incrementInDays));
                originalStartDate = ((DateTime)originalStartDate).AddDays(totalSkips * incrementInDays);

                for(var x = originalStartDate; x <= endDate; x = ((DateTime)x).AddDays(incrementInDays))
                {
                    if(x >= startDate && x <= endDate)
                    {
                        // Account for leap year
                        if(GlobalUtilities.IsLeapYear(((DateTime)x).Year))
                            x = ((DateTime)x).AddDays(1);

                        var nodeInstance = GlobalUtilities.Clone(node, CloneType.Reflection);
                        nodeInstance.StartDate = x;
                        nodeInstance.EndDate = ((DateTime)x).AddMinutes(lengthOfEventInMinutes);
                        nodes.Add(nodeInstance);
                    }
                }
            }

            return nodes;
        }

        public List<CalendarStatus> GetCalendarStatuses()
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarStatuses.ToList();

            return nodes;
        }
        public List<CalendarPrivacyType> GetCalendarPrivacyType()
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarPrivacyTypes.ToList();

            return nodes;
        }
        public List<CalendarItemStatus> GetCalendarItemStatuses()
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarItemStatuses.ToList();

            return nodes;
        }
        public List<CalendarItemType> GetCalendarItemTypes()
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarItemTypes.ToList();

            return nodes;
        }
        public List<CalendarItemRepeatType> GetCalendarItemRepeatTypes()
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var nodes = context.CalendarItemRepeatTypes.ToList();

            return nodes;
        }
        #endregion

        #region Creating Data
        /// <summary>
        /// Uses OData to ensure that there is at least one calendar for the current customer using a TaskFactory.
        /// </summary>
        public void EnsureAtLeastOneCalendar()
        {
            // Use OData to ensure that there is at least one calendar there. 
            // This method is designed to be used in a Factory.
            var factory = new TaskFactory();
            var customerID = Identity.Current.CustomerID;
            factory.StartNew(() =>
            {
                var context = ExigoApiContext.CreateCustomODataContext();
                var calendars = context.Calendars.Where(c => c.CustomerID == customerID);

                // If OData says we don't have any calendars, create one.
                if(calendars.Count() == 0)
                {
                    var newCalendarID = CreateCalendar(new CreateCalendarRequest
                    {
                        Description = DefaultPersonalCalendarDescription
                    });
                    CreateInitialCalendarItems(newCalendarID);
                }
            });
        }
        public void CreateInitialCalendarItems(int calendarID)
        {
            // Create the customer's distributorship anniversary date.
            try
            {
                var anniversary = new CreateCalendarItemRequest();
                anniversary.CalendarID = calendarID;
                anniversary.Title = "My Distributorship Anniversary";
                anniversary.StartDate = Identity.Current.JoinedDate;
                anniversary.EndDate = Identity.Current.JoinedDate;
                anniversary.TimeZoneOffset = "-000";
                anniversary.CalendarItemRepeatTypeID = CalendarItemRepeatTypes.Yearly;
                anniversary.CalendarItemTypeID = CalendarItemTypes.Anniversary;
                anniversary.CalendarPrivacyTypeID = CalendarPrivacyTypes.Private;
                anniversary.AllDay = true;
                anniversary.IsCorporate = false;
                CreateCalendarItem(anniversary);
            }
            catch {}


            // Add their birthday if we have it.
            try
            {
                var data = ExigoApiContext.CreateODataContext().Customers
                    .Where(c => c.CustomerID == Identity.Current.CustomerID)
                    .Select(c => new {
                        c.BirthDate
                    })
                    .SingleOrDefault();

                if(data != null)
                {
                    var birthdate = Convert.ToDateTime(data.BirthDate);
                   
                    var birthday = new CreateCalendarItemRequest();
                    birthday.CalendarID = calendarID;
                    birthday.Title = "Happy Birthday!";
                    birthday.StartDate = birthdate;
                    birthday.EndDate = birthdate;
                    birthday.TimeZoneOffset = "-000";
                    birthday.CalendarItemRepeatTypeID = CalendarItemRepeatTypes.Yearly;
                    birthday.CalendarItemTypeID = CalendarItemTypes.Birthday;
                    birthday.CalendarPrivacyTypeID = CalendarPrivacyTypes.Private;
                    birthday.AllDay = true;
                    birthday.IsCorporate = false;
                    CreateCalendarItem(birthday);
                    
                }
            }
            catch {}
        }

        public int CreateCalendar(CreateCalendarRequest request)
        {
            var calendar = new Exigo.OData.Extended.Calendar();
            calendar.CustomerID = request.CustomerID;
            calendar.CalendarStatusID = request.CalendarStatusID;
            calendar.CalendarPrivacyTypeID = request.CalendarPrivacyTypeID;
            calendar.Description = request.Description;

            var context = ExigoApiContext.CreateCustomODataContext();
            context.AddToCalendars(calendar);
            context.SaveChanges();

            return calendar.CalendarID;
        }

        public int CreateCalendarItem(CreateCalendarItemRequest request)
        {
            // Ensure that we do not save a time if this is an all-day event
            if(request.AllDay)
            {
                request.StartDate = request.StartDate.Date;
                request.EndDate = request.EndDate.Date;
            }


            var calendaritem = new Exigo.OData.Extended.CalendarItem();
            calendaritem.CustomerID = request.CustomerID;
            calendaritem.CalendarID = request.CalendarID;
            calendaritem.CalendarItemTypeID = request.CalendarItemTypeID;
            calendaritem.CalendarItemStatusID = request.CalendarItemStatusID;
            calendaritem.CalendarPrivacyTypeID = request.CalendarPrivacyTypeID;
            calendaritem.CalendarItemRepeatTypeID = request.CalendarItemRepeatTypeID;

            calendaritem.Title = request.Title;
            calendaritem.Description = request.Description;
            calendaritem.Location = request.Location;
            calendaritem.StartDate = request.StartDate;
            calendaritem.EndDate = request.EndDate;
            calendaritem.TimeZoneOffset = request.TimeZoneOffset;
            calendaritem.AllDay = request.AllDay;
            calendaritem.IsCorporate = request.IsCorporate;

            var context = ExigoApiContext.CreateCustomODataContext();
            context.AddToCalendarItems(calendaritem);
            context.SaveChanges();

            return calendaritem.CalendarItemID;
        }
        #endregion

        #region Updating Data
        public bool UpdateCalendarItem(UpdateCalendarItemRequest request)
        {
            try
            {
                var context = ExigoApiContext.CreateCustomODataContext();
                var calendaritem = context.CalendarItems
                    .Where(c => c.CalendarItemID == request.CalendarItemID)
                    .FirstOrDefault();
                if(calendaritem == null)
                    return false;

                calendaritem.CalendarID = request.CalendarID;
                calendaritem.CalendarItemTypeID = request.CalendarItemTypeID;
                calendaritem.CalendarItemStatusID = request.CalendarItemStatusID;
                calendaritem.CalendarPrivacyTypeID = request.CalendarPrivacyTypeID;
                calendaritem.CalendarItemRepeatTypeID = request.CalendarItemRepeatTypeID;

                calendaritem.Title = request.Title;
                calendaritem.Description = request.Description;
                calendaritem.Location = request.Location;
                calendaritem.StartDate = request.StartDate;
                calendaritem.EndDate = request.EndDate;
                calendaritem.TimeZoneOffset = request.TimeZoneOffset;
                calendaritem.AllDay = request.AllDay;
                calendaritem.IsCorporate = request.IsCorporate;

                context.UpdateObject(calendaritem);
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MarkCalendarItemAsPrivate(int calendarItemID)
        {
            try
            {
                var context = ExigoApiContext.CreateCustomODataContext();
                var calendaritem = context.CalendarItems
                    .Where(c => c.CalendarItemID == calendarItemID)
                    .FirstOrDefault();
                if(calendaritem == null)
                    return false;

                calendaritem.CalendarPrivacyTypeID = CalendarPrivacyTypes.Private;

                context.UpdateObject(calendaritem);
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool MarkCalendarItemAsPublic(int calendarItemID)
        {
            try
            {
                var context = ExigoApiContext.CreateCustomODataContext();
                var calendaritem = context.CalendarItems
                    .Where(c => c.CalendarItemID == calendarItemID)
                    .FirstOrDefault();
                if(calendaritem == null)
                    return false;

                calendaritem.CalendarPrivacyTypeID = CalendarPrivacyTypes.Public;

                context.UpdateObject(calendaritem);
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Deleting Data
        public bool DeleteCalendarItem(int calendarItemID)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var calendarItem = context.CalendarItems
                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                .Where(c => c.CalendarItemID == calendarItemID)
                .FirstOrDefault();

            if(calendarItem == null) return false;

            context.DeleteObject(calendarItem);
            context.SaveChanges();
            return true;
        }
        #endregion

        #region Validating Data
        /// <summary>
        /// Validates whether the provided calendar item ID belongs to the backoffice owner.
        /// </summary>
        /// <param name="calendarItemID">The ID of the calendar item to validate.</param>
        /// <returns>True if the item belongs to the backoffice owner.</returns>
        public bool ValidateCalendarItem(int calendarItemID)
        {
            var context = ExigoApiContext.CreateCustomODataContext();
            var count = context.CalendarItems
                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                .Where(c => c.CalendarItemID == calendarItemID)
                .Count();

            return count > 0;
        }
        #endregion

        #region Formatting JSON
        public string GetDataAsJson(string start, string end, string filter)
        {
            var service = new CalendarService();
            var calendarID = service.GetCalendars().FirstOrDefault().CalendarID;
            var startDate = service.UnixTimeStampToDateTime(start);
            var endDate = service.UnixTimeStampToDateTime(end);
            var eventFilter = filter;

            // Fetch the data
            var items = service.GetCalendarItems(calendarID, startDate, endDate, eventFilter);


            // Compile the nodes into a list
            var nodes = new List<CalendarNode>();
            foreach(var item in items)
            {
                var node = new CalendarNode();
                node.CalendarID = item.CalendarItemID;
                node.Title = item.Title;
                node.Description = item.Description;
                node.Location = item.Location;
                node.AllDay = ((bool)item.AllDay);
                node.StartDate = Convert.ToDateTime(item.StartDate + " " + item.TimeZoneOffset);
                node.EndDate = Convert.ToDateTime(item.EndDate + " " + item.TimeZoneOffset);
                node.FormattedDateTime = service.GetCalendarItemDateSpanHtml(((bool)item.AllDay), node.StartDate, node.EndDate, item.TimeZoneOffset);
                node.DetailUrl = CalendarDetailsUrl + "?id=" + item.CalendarItemID;
                node.IsPrivate = (item.CalendarPrivacyTypeID == CalendarPrivacyTypes.Private);
                node.IsPersonal = (item.CustomerID == Identity.Current.CustomerID);

                if(!node.IsPersonal)
                {
                    node.OwnerDetails = string.Format("<img src='{0}' class='pull-left' style='max-width: 20px; max-height: 20px; margin-right: 5px;' /> {1}<div class='clear'></div>",
                        GlobalUtilities.GetCustomerTinyAvatarUrl(((int)item.CustomerID)),
                        GlobalUtilities.Coalesce(item.Customer.Company, item.Customer.FirstName + " " + item.Customer.LastName));
                }

                nodes.Add(node);
            }

            // Serialize the list of nodes into JSON
            var stream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(List<CalendarNode>));
            serializer.WriteObject(stream, nodes);
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();


            return json;
        }
        #endregion

        #region Helper Methods
        public DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var unixTimeStampAsDouble = Convert.ToDouble(unixTimeStamp);
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStampAsDouble).ToLocalTime();
            return dtDateTime;
        }

        public string GetCalendarItemDateSpanHtml(bool allDay, DateTime start, DateTime end, string timeZoneOffset)
        {
            var result = "";
            var startDate = (!allDay) ? Convert.ToDateTime(start + " " + timeZoneOffset) : start;
            var endDate = (!allDay) ? Convert.ToDateTime(end + " " + timeZoneOffset) : end;

            if(allDay)
            {
                result = string.Format("{0:dddd, MMMM d, yyyy}", startDate);
            }
            else if(startDate.Date == endDate.Date)
            {
                if(startDate == endDate) result = string.Format("{0:dddd, MMMM d, yyyy}<br />{0:h:mm tt}", startDate);
                else result = string.Format("{0:dddd, MMMM d, yyyy}<br />{0:h:mm tt} - {1:h:mm tt}", startDate, endDate);
            }
            else
            {
                result = string.Format("Starts: {0:dddd, MMMM d, yyyy h:mm tt}<br />Ends: {1:dddd, MMMM d, yyyy h:mm tt}", startDate, endDate);
            }

            return result;
        }
        #endregion
    }

    #region Requests
    public class CreateCalendarRequest
    {
        public CreateCalendarRequest()
        {
            this.CustomerID = Identity.Current.CustomerID;
            this.CalendarStatusID = CalendarStatuses.Active;
            this.CalendarPrivacyTypeID = CalendarPrivacyTypes.Public;
        }

        public int CustomerID { get; set; }
        public int CalendarStatusID { get; set; }
        public int CalendarPrivacyTypeID { get; set; }
        public string Description { get; set; }
    }

    public class CreateCalendarItemRequest
    {
        public CreateCalendarItemRequest()
        {
            this.CustomerID = Identity.Current.CustomerID;
            this.CalendarItemStatusID = CalendarItemStatuses.Active;
            this.CalendarItemTypeID = CalendarItemTypes.Appointment;
            this.CalendarPrivacyTypeID = CalendarPrivacyTypes.Private;
            this.CalendarItemRepeatTypeID = CalendarItemRepeatTypes.None;
        }

        public int CustomerID { get; set; }
        public int CalendarID { get; set; }
        public int CalendarItemTypeID { get; set; }
        public int CalendarItemStatusID { get; set; }
        public int CalendarPrivacyTypeID { get; set; }
        public int CalendarItemRepeatTypeID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZoneOffset { get; set; }
        public bool AllDay { get; set; }
        public bool IsCorporate { get; set; }
    }

    public class UpdateCalendarItemRequest
    {
        public UpdateCalendarItemRequest()
        {
        }

        public int CalendarID { get; set; }
        public int CalendarItemID { get; set; }
        public int CalendarItemTypeID { get; set; }
        public int CalendarItemStatusID { get; set; }
        public int CalendarPrivacyTypeID { get; set; }
        public int CalendarItemRepeatTypeID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZoneOffset { get; set; }
        public bool AllDay { get; set; }
        public bool IsCorporate { get; set; }
    }

    public class GetCalendarItemsRequest
    {
        public GetCalendarItemsRequest()
        {
            this.CustomerID = Identity.Current.CustomerID;
            this.CalendarItemStatusID = CalendarItemStatuses.Active;
        }

        public int CustomerID { get; set; }
        public int CalendarID { get; set; }
        public int? CalendarItemTypeID { get; set; }
        public int? CalendarItemStatusID { get; set; }
        public int? CalendarPrivacyTypeID { get; set; }
        public int? CalendarItemRepeatTypeID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? AllDay { get; set; }
        public bool? IsCorporate { get; set; }
    }
    #endregion

    #region Enums/Helper Classes
    public static class CalendarStatuses
    {
        public const int Active = 1;
        public const int Deleted = 2;
    }
    public static class CalendarItemStatuses
    {
        public const int Active = 1;
        public const int Deleted = 2;
    }
    public static class CalendarItemTypes
    {
        public const int Appointment = 1;
        public const int ToDo = 2;
        public const int Birthday = 3;
        public const int Anniversary = 4;
    }
    public static class CalendarPrivacyTypes
    {
        public const int Public = 1;
        public const int Private = 2;
    }
    public static class CalendarItemRepeatTypes
    {
        public const int None = 1;
        public const int Daily = 2;
        public const int Weekly = 3;
        public const int BiWeekly = 4;
        public const int Monthly = 5;
        public const int Yearly = 6;
    }
    #endregion
}