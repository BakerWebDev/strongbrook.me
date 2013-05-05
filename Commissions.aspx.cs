using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Commissions : System.Web.UI.Page
{
    private int PeriodTypeID = PeriodTypes.Default;
    private bool DisplayCurrentCommissions = true;
    private bool DisplayPriorCommissions = true;




    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    #endregion

    #region Properties
    // Returns the latest commission period for the initial report.
    public int PeriodID
    {
        get
        {
            int periodID;
            if (Request.QueryString["id"] == null)
            {
                try
                {
                    if (CurrentCommissions.Count > 0)
                    {
                        periodID = CurrentCommissions.FirstOrDefault().PeriodID;
                    }
                    else
                    {
                        periodID = PriorCommissions.FirstOrDefault().CommissionRun.PeriodID;
                    }
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                periodID = Convert.ToInt32(Request.QueryString["id"]);
            }
            return periodID;
        }
    }

    public int CurrentCommissionRunID
    {
        get
        {
            if(commissionRunID == null)
            {
                commissionRunID = FetchCurrentCommissionRunID();
            }
            return Convert.ToInt32(commissionRunID);
        }
    }
    private int? commissionRunID;

    // Returns 0 if real time commission, 1 if prior commission.
    public CommissionPeriodType CommissionType
    {
        get
        {
            CommissionPeriodType type = CommissionPeriodType.Current;
            if (Request.QueryString["type"] != null)
            {
                type = (CommissionPeriodType)Enum.Parse(typeof(CommissionPeriodType), Request.QueryString["type"]);
            }
            return type;
        }
    }

    public CommissionModel DataModel
    {
        get
        {
            if(_dataModel == null)
            {
                _dataModel = new CommissionModel();

                // Current Commissions
                if (CommissionType == CommissionPeriodType.Current)
                {
                    var details = CurrentCommissions.Where(c => c.PeriodID == PeriodID).Select(c => c).FirstOrDefault();
                    var periodDetails = PeriodDetails;
                    var bonuses = CurrentCommissionBonuses;

                    _dataModel.CommissionRunID = 0;
                    _dataModel.CommissionDescription = details.PeriodDescription;
                    _dataModel.PeriodID = PeriodID;
                    _dataModel.PeriodTypeID = PeriodTypeID;
                    _dataModel.StartDate = periodDetails.StartDate;
                    _dataModel.EndDate = periodDetails.EndDate;
                    _dataModel.PaidAsRank = Identity.Current.Ranks.CurrentPeriodRankDescription;
                    _dataModel.Total = details.CommissionTotal;
                    _dataModel.Volume1 = PeriodVolumes.Volume1;
                    _dataModel.Volume3 = PeriodVolumes.Volume3;
                    _dataModel.Volume75 = PeriodVolumes.Volume75;
                    _dataModel.Volume79 = PeriodVolumes.Volume79;
                
                    foreach(var bonus in bonuses)
                    {
                        var detail = new CommissionBonusModel();
                        detail.BonusID = bonus.BonusID;
                        detail.Description = bonus.Description;                    
                        detail.Amount = bonus.Amount;
                        _dataModel.Bonuses.Add(detail);
                    }
                }

                // Prior Commissions
                if (CommissionType == CommissionPeriodType.Prior)
                {
                    var details = CommissionRunDetails;
                    var data = CommissionDetails;
                    var paidRank = PaidRankDescription;
                    var bonuses = CommissionBonuses;

                    _dataModel.CommissionRunID = details.CommissionRunID;
                    _dataModel.CommissionDescription = details.CommissionRunDescription;
                    _dataModel.PeriodID = details.PeriodID;
                    _dataModel.PeriodTypeID = details.PeriodTypeID;
                    _dataModel.StartDate = details.Period.StartDate;
                    _dataModel.EndDate = details.Period.EndDate;
                    _dataModel.PaidAsRank = PaidRankDescription;
                    _dataModel.Earned = data.Earnings;
                    _dataModel.Fee = data.Fee;
                    _dataModel.Total = data.Total;
                    _dataModel.Volume1 = PeriodVolumes.Volume1;
                    _dataModel.Volume3 = PeriodVolumes.Volume3;
                    _dataModel.Volume75 = PeriodVolumes.Volume75;
                    _dataModel.Volume79 = PeriodVolumes.Volume79;

                    foreach(var bonus in bonuses)
                    {
                        var detail = new CommissionBonusModel();
                        detail.BonusID = bonus.BonusID;
                        detail.Description = bonus.BonusDescription;                    
                        detail.Amount = bonus.Amount;
                        _dataModel.Bonuses.Add(detail);
                    }
                }
            }

            return _dataModel;
        }
    }
    private CommissionModel _dataModel;

    public List<CommissionResponse> CurrentCommissions
    {
        get
        {
            _currentCommissions = _currentCommissions ?? FetchCurrentCommissions();
            return _currentCommissions;
        }
    }
    private List<CommissionResponse> _currentCommissions;

    public List<CommissionBonusResponse> CurrentCommissionBonuses
    {
        get
        {
            _currentCommissionBonuses = _currentCommissionBonuses ?? FetchCurrentCommissionBonuses();
            return _currentCommissionBonuses;
        }
    }
    private List<CommissionBonusResponse> _currentCommissionBonuses;

    public List<Commission> PriorCommissions
    {
        get
        {
            if (priorCommissions == null)
            {
                priorCommissions = FetchPriorCommissions();
            }
            return priorCommissions;
        }
    }
    private List<Commission> priorCommissions;

    public Commission CommissionDetails
    {
        get
        {
            if (_commissionDetails == null)
            {
                _commissionDetails = FetchCommissionDetails();
            }
            return _commissionDetails;
        }
    }
    private Commission _commissionDetails;

    public CommissionRun CommissionRunDetails
    {
        get
        {
            if (_commissionRunDetails == null)
            {
                _commissionRunDetails = FetchCommissionRunDetails();
            }
            return _commissionRunDetails;
        }
    }
    private CommissionRun _commissionRunDetails;

    public List<CommissionBonus> CommissionBonuses
    {
        get
        {
            if (_commissionBonuses == null)
            {
                _commissionBonuses = FetchCommissionBonuses();
            }
            return _commissionBonuses;
        }
    }
    private List<CommissionBonus> _commissionBonuses;

    public string PaidRankDescription
    {
        get
        {
            if (_paidRankDescription == null)
            {
                _paidRankDescription = FetchPaidRankDescription();
            }
            return _paidRankDescription;
        }
    }
    private string _paidRankDescription;    

    public PeriodVolume PeriodVolumes
    {
        get
        {
            if (_periodVolumes == null)
            {
                _periodVolumes = FetchPeriodVolumes();
            }
            return _periodVolumes;
        }
    }
    private PeriodVolume _periodVolumes;

    public Period PeriodDetails
    {
        get
        {
            _periodDetails = _periodDetails ?? FetchPeriodDetails();
            return _periodDetails;
        }
    }
    private Period _periodDetails;
    #endregion

    #region Fetching Data
    // Current Commissions
    private List<CommissionResponse> FetchCurrentCommissions()
    {
        return ExigoApiContext.CreateWebServiceContext().GetRealTimeCommissions(new GetRealTimeCommissionsRequest
        {
            CustomerID = Identity.Current.CustomerID
        }).Commissions
            .Where(c => c.PeriodType == PeriodTypeID)
            .OrderByDescending(c => c.PeriodID).ToList();        
    }
    private List<CommissionBonusResponse> FetchCurrentCommissionBonuses()
    {
        return CurrentCommissions[0].Bonuses.ToList();
    }

    // Prior Commissions
    private List<Commission> FetchPriorCommissions()
    {
        return ExigoApiContext.CreateODataContext().Commissions.Expand("CommissionRun")
                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                .OrderByDescending(c => c.CommissionRunID)
                .Select(c => c).ToList();
    }
    private Commission FetchCommissionDetails()
    {
        return (from c in ExigoApiContext.CreateODataContext().Commissions
                where c.CustomerID == Identity.Current.CustomerID
                where c.CommissionRunID == CurrentCommissionRunID
                where c.CurrencyCode == Identity.Current.CurrencyCode
                select c).FirstOrDefault();
    }
    public CommissionRun FetchCommissionRunDetails()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns.Expand("Period")
                    where c.CommissionRunID == CurrentCommissionRunID
                    select c).FirstOrDefault();

        if (data == null) return new CommissionRun();
        else return data;
    }
    private List<CommissionBonus> FetchCommissionBonuses()
    {
        return (from c in ExigoApiContext.CreateODataContext().CommissionBonuses
                where c.CustomerID == Identity.Current.CustomerID
                where c.CommissionRunID == CurrentCommissionRunID
                orderby c.BonusDescription
                select c).ToList();
    }
    private PeriodVolume FetchPeriodVolumes()
    {
        return (from o in ExigoApiContext.CreateODataContext().PeriodVolumes
                where o.CustomerID == Identity.Current.CustomerID
                where o.PeriodTypeID == PeriodTypeID
                where o.PeriodID == PeriodID
                select new PeriodVolume()
                {
                    Rank = o.Rank,
                    PaidRank = o.PaidRank,
                    Volume1 = o.Volume1,
                    Volume2 = o.Volume2,
                    Volume3 = o.Volume3,
                }).FirstOrDefault();
    }
    private string FetchPaidRankDescription()
    {
        return (from c in ExigoApiContext.CreateODataContext().Ranks
                where c.RankID == CommissionDetails.PaidRankID
                select c.RankDescription).FirstOrDefault();
    }
    private int FetchCurrentCommissionRunID()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns
                    where c.PeriodID == PeriodID
                    orderby c.CommissionRunID descending
                    select new
                    {
                        c.CommissionRunID
                    }).FirstOrDefault();

        if (data == null) return 0;
        else return data.CommissionRunID;
    }

    // Misc. Data
    private Period FetchPeriodDetails()
    {
        return (from c in ExigoApiContext.CreateODataContext().Periods
                where c.PeriodID == PeriodID
                where c.PeriodTypeID == PeriodTypeID
                select new Period()
                {
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                }).FirstOrDefault();
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if (Request.QueryString["datakey"] != null)
        {
            Response.Clear();

            switch (Request.QueryString["datakey"])
            {
                case "summary":
                    try
                    {
                        RenderCommissionRunSummary(writer);
                    }
                    catch
                    {
                        writer.Write("Commission summary unavailable.");
                    }
                    break;
                case "details":
                    try
                    {
                        RenderCommissionDetails(writer);
                    }
                    catch
                    {
                        writer.Write("Commission details unavailable. ");
                    }
                    break;
                case "volumes":
                    try
                    {
                        RenderMyVolumes(writer);
                    }
                    catch
                    {
                        writer.Write("Commission volumes unavailable. ");
                    }
                    break;
                case "bonuses":
                    try
                    {
                        RenderMyBonuses(writer);
                    }
                    catch
                    {
                        writer.Write("Commission bonuses unavailable. ");
                    }
                    break;
                default:
                    return;
            }

            Response.End();
        }
        else
        {
            base.Render(writer);
        }
    }

    public void RenderAvailablePeriodsDropdown()
    {
        var html = new StringBuilder();

        // Start the dropdown
        html.AppendFormat("<select id='commissionperiods'>");


        // Add the current commission periods if applicable
        if(DisplayCurrentCommissions)
        {
            foreach(var commission in CurrentCommissions)                    
            {
                html.AppendFormat("<option value='{0}' data-type='{1}'>Current: {2}</option>",
                    commission.PeriodID,
                    (int)CommissionPeriodType.Current,
                    commission.PeriodDescription);
            }
        }

        // Add the prior commission periods if applicable
        if(DisplayPriorCommissions)
        {
            foreach(var commission in PriorCommissions)
            {
                html.AppendFormat("<option value='{0}' data-type='{1}'>{2}</option>",
                    commission.CommissionRun.PeriodID,
                    (int)CommissionPeriodType.Prior,
                    commission.CommissionRun.CommissionRunDescription);
            }
        }

        // End our dropdown
        html.AppendFormat("</select>");


        // Write the HTML to the page
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }

    private void RenderCommissionRunSummary(HtmlTextWriter writer)
    {
        var html = new StringBuilder();

        // Title
        html.AppendFormat("<h1 id='CurrentCommission' class='color2'>{0}</h1>", DataModel.CommissionDescription);

        // Subtitle
        var colorStyle = (DataModel.Total < 0) ? "style='color: red;'" : "style='color: green;'";
        if (CommissionType == CommissionPeriodType.Current)
        {
            html.AppendFormat("<h2>Will Recieve : <span id='CommissionAmount' {0}>{1}</span></h2>", 
                colorStyle, 
                DataModel.Total.ToString("C", GetCultureInfo()));
        }

        if (CommissionType == CommissionPeriodType.Prior)
        {
            html.AppendFormat("<h2>Paid <span id='CommissionAmount' {0}>{1}</span> as a <span id='PaidAsDescription'>{2}</span></h2>", 
                colorStyle,
                DataModel.Total.ToString("C", GetCultureInfo()),
                DataModel.PaidAsRank);
        }

        // Write the HTML to the page
        writer.Write(html.ToString());
    }
    private void RenderCommissionDetails(HtmlTextWriter writer)
    {
        var html = new StringBuilder();

        // Set up some helpful variables
        string Total = string.Empty;
        string Fee = string.Empty;
        string Earned = string.Empty;


        Earned = DataModel.Earned.ToString("C", GetCultureInfo());
        DataModel.Fee.ToString("C", GetCultureInfo());
        Total = DataModel.Total.ToString("C", GetCultureInfo());
        Fee = DataModel.Fee.ToString("C", GetCultureInfo());


        html.AppendFormat("<h2>Period Summary</h2>");
        html.AppendFormat("<table class='table '>");
        html.AppendFormat(@"
                <tr>
                    <th class='fieldlabel'>Description</th>
                    <th class='value'>Value</th>
                </tr>
            ");

        html.AppendFormat(@"
                <tr>
                    <td class='fieldlabel'>Start Date</td>
                    <td class='value'>{0:dddd, MMMM d, yyyy}</td>
                </tr>
            ", DataModel.StartDate);

        html.AppendFormat(@"
                <tr>
                    <td class='fieldlabel'>End Date</td>
                    <td class='value'>{0:dddd, MMMM d, yyyy}</td>
                </tr>
            ", DataModel.EndDate);

        html.AppendFormat(@"
                     <tr>
                        <td class='fieldlabel'>Paid As</td>
                        <td class='value'>{0}</td>
                     </tr>
                ", DataModel.PaidAsRank);


        if (CommissionType == CommissionPeriodType.Prior)
        {
            var earnedStyle = (DataModel.Earned < 0) ? "style='color: red;'" : string.Empty;
            html.AppendFormat(@"
                     <tr>
                        <td class='fieldlabel'>Earned</td>
                        <td class='value' {0}>{1}</td>
                    </tr>
                ", earnedStyle,
                 DataModel.Earned.ToString("C", GetCultureInfo()));

            var feeStyle = (DataModel.Earned < 0) ? "style='color: red;'" : string.Empty;
            html.AppendFormat(@"
                     <tr>
                        <td class='fieldlabel'>Fee(s):</td>
                        <td class='value' {0}>{1}</td>
                    </tr>
                ", feeStyle,
                 DataModel.Fee.ToString("C", GetCultureInfo()));
        }

        var totalStyle = (DataModel.Earned < 0) ? "style='color: red;'" : string.Empty;
        html.AppendFormat(@"
                 <tr>
                    <td class='fieldlabel'>Total Commissions</td>
                    <td class='value' {0}><b>{1}</b></td>
                 </tr>
            ", totalStyle,
             DataModel.Total.ToString("C", GetCultureInfo()));

        html.AppendFormat("</table>");


        // Write the HTML to the page
        writer.Write(html.ToString());
    }
    private void RenderMyBonuses(HtmlTextWriter writer)
    {
        var html = new StringBuilder();

        // Title
        html.AppendFormat("<h2>Bonuses</h2>");

        // Main Table
        html.AppendFormat("<table class='table'>");

       
        if(DataModel.Bonuses.Count == 0)
        {
            html.AppendFormat("<tr><td>No bonuses {0} earned for this period.</td></tr>",
                (CommissionType == CommissionPeriodType.Current) ? "have been" : "were");
        }
        else
        {
            html.AppendFormat(@"
                <tr>
                    <th class='fieldlabel'>Description</th>
                    <th class='value'>Earned</th>
                </tr>
            ");

            foreach (var bonus in DataModel.Bonuses)
            {                  
                // Define some contextual labels
                var amountStyle = (bonus.Amount < 0M) ? "style='color: red;'" : "";

                var currentBonusDescription = string.Format("<a href='CommissionBonusDetails.aspx?type={0}&ptid={1}&pid={2}&bid={3}'>{4}</a>", 
                    (int)CommissionPeriodType.Current,
                    DataModel.PeriodTypeID,
                    DataModel.PeriodID,
                    bonus.BonusID,
                    bonus.Description);

                var priorBonusDescription = string.Format("<a href='CommissionBonusDetails.aspx?type={0}&rid={1}&bid={2}'>{3}</a>", 
                    (int)CommissionPeriodType.Prior,
                    DataModel.CommissionRunID,
                    bonus.BonusID,
                    bonus.Description);


                html.AppendFormat(@"
                        <tr>
                            <td class='fieldlabel'>{0}</td>
                            <td class='value' {1}>{2}</td>
                        </tr>
                    ", (CommissionType == CommissionPeriodType.Current) ? currentBonusDescription : priorBonusDescription,
                        amountStyle,
                        bonus.Amount.ToString("C", GetCultureInfo()));
            }
        }

        // End the main table
        html.AppendFormat("</table>");
      
        // Write the HTML to the page
        writer.Write(html.ToString());
    }
    private void RenderMyVolumes(HtmlTextWriter writer)
    {
        var html = new StringBuilder();


        // Title
        html.AppendFormat("<h2>Volumes</h2>");

        // Main Table
        html.AppendFormat("<table class='table'>");
        html.AppendFormat(@"
                    <tr>
                        <th class='fieldlabel'>Description</th>
                        <th class='value'>Earned</th>
                    </tr>
                ");
        html.AppendFormat(@"
                <tr>
                    <td class=field'label'>PCV</td>
                    <td class='value'>{0:N0}</td>
                </tr>
            ", PeriodVolumes.Volume1);
        html.AppendFormat(@"
                <tr>
                    <td class='fieldlabel'>OCV</td>
                    <td class='value'>{0:N0}</td>
                </tr>
            ", PeriodVolumes.Volume3);
//        html.AppendFormat(@"
//                <tr>
//                    <td class='fieldlabel'>3 Month PCV</td>
//                    <td class='value'>{0:N0}</td>
//                </tr>
//            ", PeriodVolumes.Volume75);
//        html.AppendFormat(@"
//                <tr>
//                    <td class='fieldlabel'>3 Month OCV</td>
//                    <td class='value'>{0:N0}</td>
//                </tr>
//            ", PeriodVolumes.Volume79);

        // End the main table
        html.AppendFormat("</table>");


        // Write the HTML to the page
        writer.Write(html.ToString());
    }
    #endregion

    #region Helper Methods
    public CultureInfo GetCultureInfo()
    {
        string cultureInfo = "";
        var country = Identity.Current.Address.Country;

        switch (country)
        {
            case "US":
                cultureInfo = "en";
                break;
            case "CA":
                cultureInfo = "en";
                break;
            case "ID":
                cultureInfo = "id";
                break;
        }

        return new CultureInfo(cultureInfo);
    }
    #endregion

    #region Models & Enums
    public class CommissionModel
    {
        public CommissionModel()
        {
            this.Bonuses = new List<CommissionBonusModel>();
        }

        public int CommissionRunID { get; set; }
        public int PeriodID { get; set; }
        public int PeriodTypeID { get; set; }
        public string CommissionDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaidAsRank { get; set; }
        public decimal Earned { get; set; }
        public decimal Fee { get; set; }
        public decimal Total { get; set; }

        public decimal Volume1 { get; set; }
        public decimal Volume3 { get; set; }
        public decimal Volume75 { get; set; }
        public decimal Volume79 { get; set; }

        public List<CommissionBonusModel> Bonuses { get; set; }
    }

    public class CommissionBonusModel
    {
        public int BonusID { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public enum CommissionPeriodType
    {
        Current = 0,
        Prior = 1
    }
    #endregion
}