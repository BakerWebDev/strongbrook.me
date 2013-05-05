using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Exigo.Calendars
{
    [DataContract]
    [Serializable]
    public class CalendarNode
    {
        [DataMember(Name = "ownerdetails")]
        public string OwnerDetails { get; set; }

        [DataMember(Name = "id")]
        public int CalendarID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "formatteddate")]
        public string FormattedDateTime { get; set; }

        [DataMember(Name = "detailurl")]
        public string DetailUrl { get; set; }

        [DataMember(Name = "allDay")]
        public bool AllDay { get; set; }

        [DataMember(Name = "personal")]
        public bool IsPersonal { get; set; }

        [DataMember(Name = "private")]
        public bool IsPrivate { get; set; }

        [DataMember(Name = "start")]
        private string FormattedStartDate { get; set; }
        [IgnoreDataMember]
        public DateTime StartDate
        {
            get { return DateTime.ParseExact(FormattedStartDate, "o", CultureInfo.InvariantCulture); }
            set { FormattedStartDate = value.ToString("o"); }
        }

        [DataMember(Name = "end")]
        private string FormattedEndDate { get; set; }
        [IgnoreDataMember]
        public DateTime EndDate
        {
            get { return DateTime.ParseExact(FormattedEndDate, "o", CultureInfo.InvariantCulture); }
            set { FormattedEndDate = value.ToString("o"); }
        }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "backgroundColor")]
        public string BackgroundColor { get; set; }

        [DataMember(Name = "borderColor")]
        public string BorderColor { get; set; }

        [DataMember(Name = "textColor")]
        public string TextColor { get; set; }
    }
}