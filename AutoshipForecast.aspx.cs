using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AutoshipForecast : System.Web.UI.Page
{
    /// <summary>
    /// How many periods will display in the report.
    /// </summary>
    public int ForecastPeriodCount          = 10;
   




    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            PopulateLegFilters();
        }
    }
    #endregion

    #region Properties
    public AutoshipForecastModel Model
    {
        get
        {
            if(_model == null)
            {
                _model = FetchReportDataAsModel();
            }
            return _model;
        }
    }
    private AutoshipForecastModel _model;

    public int CurrentPeriodID
    {
        get
        {
            if(_currentPeriodID == null)
            {
                _currentPeriodID = GlobalUtilities.GetCurrentPeriodID();
            }
            return Convert.ToInt32(_currentPeriodID);
        }
    }
    private int? _currentPeriodID;

    public LegFilterType LegFilter
    {
        get
        {
            return (Request.QueryString["leg"] != null) 
                ? (LegFilterType)Enum.Parse(typeof(LegFilterType), Request.QueryString["leg"].ToString())
                : LegFilterType.All;
        }
    }
    #endregion

    #region Fetching Data
    public AutoshipForecastModel FetchReportDataAsModel()
    {
        /*var helper = new SqlHelper();
        var datatable = helper.GetTable(@"
                DECLARE @CustomerID INT = {0}
                DECLARE @Leg INT = {1}
                DECLARE @PeriodTypeID INT = {2}
                DECLARE @PeriodCount INT = {3}
                DECLARE @CurrentPeriodID INT = {4}


                -- Get the Start and End date range based on the number of periods the report will display
                DECLARE @StartRange DATETIME,
                        @EndRange   DATETIME

                SELECT @StartRange = min(StartDate)
	                 , @EndRange = max(EndDate)
                FROM
	                Periods
                WHERE
	                PeriodTypeID = @PeriodTypeID
	                AND PeriodID BETWEEN @CurrentPeriodID AND (@CurrentPeriodID + @PeriodCount)


                -- Create a temp table to store the periods we will be working with
                CREATE TABLE #p(
	                PeriodId INT,
	                PeriodDescription VARCHAR(100),
	                Startdate DATETIME,
	                EndDate DATETIME
                )
                INSERT #p
                SELECT PeriodID
	                 , PeriodDescription
	                 , StartDate
	                 , EndDate
                FROM
	                Periods
                WHERE
	                PeriodTypeID = @PeriodTypeID
	                AND PeriodID BETWEEN @CurrentPeriodID AND (@CurrentPeriodID + @PeriodCount)


                -- Filter by the left or right leg
                DECLARE @lft        INT,
                        @rgt        INT,
                        @leftleft   INT,
                        @leftright  INT,
                        @rightleft  INT,
                        @rightright INT

                -- Get the left and right positions of our left leg
                SELECT @leftleft = lft
	                 , @leftright = rgt
                FROM
	                BinaryTree
                WHERE
	                ParentID = @CustomerID
	                AND placement = 0

                -- Get the left and right positions of our right leg
                SELECT @rightleft = lft
	                 , @rightright = rgt
                FROM
	                binaryTree
                WHERE
	                ParentID = @CustomerID
	                AND placement = 1

                -- If we are filtering by the left leg...
                IF @leg = 0
                BEGIN
	                SELECT @Lft = @leftleft
		                 , @rgt = @leftright
	                FROM
		                binaryTree
	                WHERE
		                ParentID = @CustomerID
                END

                -- If we are filtering by the right leg...
                IF @leg = 1
                BEGIN
	                SELECT @Lft = @rightleft
		                 , @rgt = @rightright
	                FROM
		                binaryTree
	                WHERE
		                ParentID = @CustomerID
                END


                -- If we are not filtering by any legs...
                IF @leg = -1
                BEGIN
	                SELECT @Lft = @leftleft
		                 , @rgt = @rightright
	                FROM
		                binaryTree
	                WHERE
		                ParentID = @CustomerID
                END



                -- Create a temp table to select our core data
                CREATE TABLE #t(
	                PeriodID INT,
	                PeriodDescription VARCHAR(100),
	                StartDate DATETIME,
	                EndDate DATETIME,
	                CustomersCount INT,
	                ProjectedOrdersCount INT,
	                ActualOrdersCount INT,
	                ProjectedVolume INT,
	                ActualVolume INT
                )
                INSERT #t
                SELECT p.PeriodID
	                 , p.PeriodDescription
	                 , p.StartDate
	                 , p.EndDate
	                 , DistCount = count(*)
	                 , ProjectedCount = sum(CASE
		                   WHEN isnull(o.OrderStatusID, 0) < 7 THEN
			                   1
		                   ELSE
			                   0
	                   END)
	                 , ShippedCount = sum(CASE
		                   WHEN isnull(o.OrderStatusID, 0) >= 7 THEN
			                   1
		                   ELSE
			                   0
	                   END)
	                 , ProjectedVolume = sum(CASE
		                   WHEN isnull(o.OrderStatusID, 0) < 7 THEN
			                   ao.BusinessVolumeTotal
		                   ELSE
			                   0
	                   END)
	                 , ShippedVolume = sum(CASE
		                   WHEN isnull(o.OrderStatusID, 0) >= 7 THEN
			                   o.BusinessVolumeTotal
		                   ELSE
			                   0
	                   END)

                FROM
	                BinaryTree t
	                INNER JOIN AutoOrders ao
		                ON t.CustomerID = ao.CustomerID AND ao.AutoOrderStatusID = 0 AND ao.BusinessVolumeTotal > 0
	                INNER JOIN AutoOrderSchedules aosc
		                ON aosc.autoorderid = ao.autoorderid AND aosc.IsEnabled = 1 AND aosc.ScheduledDate >= @StartRange AND aosc.ScheduledDate < @EndRange + 1
	                INNER JOIN Periods p
		                ON aosc.scheduledDate >= p.Startdate AND aosc.scheduledDate < p.Enddate + 1 AND p.PeriodTypeID = @PeriodTypeID
	                LEFT JOIN Orders o
		                ON o.OrderID = aosc.OrderID
                WHERE
	                t.lft BETWEEN @lft AND @rgt
	                AND p.PeriodTypeID = @PeriodTypeID
                GROUP BY
	                p.PeriodID
                  , p.PeriodDescription
                  , p.StartDate
                  , p.EndDate
                ORDER BY
	                p.PeriodID


                -- Filter our results down a bit
                INSERT #t
                SELECT PeriodID
	                 , PeriodDescription
	                 , StartDate
	                 , EndDate
	                 , 0
	                 , 0
	                 , 0
	                 , 0
	                 , 0
                FROM
	                #p
                WHERE
	                PeriodID NOT IN (SELECT periodID
					                 FROM
						                 #t)


                -- Select our data from the temp table
                SELECT *
                FROM
	                #t
                ORDER BY
	                PeriodID

                DROP TABLE #p
                DROP TABLE #t
            ", 
                Identity.Current.CustomerID,                // Whose downline are we looking at
                (int)LegFilter,                             // Leg (-1 = All legs, 0 = Left Leg, 1 = Right Leg)
                PeriodTypes.Default,                        // The period type to use
                ForecastPeriodCount,                        // How many periods to show
                CurrentPeriodID                             // The current period ID
             );


        // If we didn't get back any results, return a new model instance.
        if(datatable.Rows.Count == 0) return new AutoshipForecastModel();

        
        // Group our data into a model so we can deal with it easier.
        var model = new AutoshipForecastModel();
        var periods = new List<AutoshipForecastPeriodNode>();

        foreach(DataRow row in datatable.Rows)
        {
            var period = new AutoshipForecastPeriodNode();
            period.CurrentPeriodID = CurrentPeriodID;
            period.PeriodID = Convert.ToInt32(row["PeriodID"]);
            period.PeriodDescription = row["PeriodDescription"].ToString();
            period.StartDate = Convert.ToDateTime(row["StartDate"]);
            period.EndDate = Convert.ToDateTime(row["EndDate"]);
            period.CustomersCount = Convert.ToInt32(row["CustomersCount"]);
            period.ProjectedOrdersCount = Convert.ToInt32(row["ProjectedOrdersCount"]);
            period.ActualOrdersCount = Convert.ToInt32(row["ActualOrdersCount"]);
            period.ProjectedVolume = Convert.ToDecimal(row["ProjectedVolume"]);
            period.ActualVolume = Convert.ToDecimal(row["ActualVolume"]);
            periods.Add(period);
        }
        model.Periods = periods;


        // Do some basic calculations
        model.CalculatePercentCompletePerPeriod();*/


        // Return our completed model
        return new AutoshipForecastModel();
    }
    #endregion

    #region Render
    public void RenderAllProgressBars()
    {
        var html = new StringBuilder();


        foreach(var period in Model.Periods)
        {
            html.Append(GetProgressBarHTML(period));
        }


        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    private string GetProgressBarHTML(AutoshipForecastPeriodNode period)
    {
        var html = new StringBuilder();


        // Start the progress bar
        html.AppendFormat("<div class='weekprogressbar'>");

        html.AppendFormat("<div class='row-fluid'>");
        html.AppendFormat("<span class='span1'>");
        html.AppendFormat("<br /><a href='AutoshipForecastDetails.aspx?leg={0}&pid={1}&ptid={2}' class='btn' title='" + Resources.AutoshipForecast.ViewDetails + "'><i class='icon-search'></i></a>", 
            (int)LegFilter, 
            period.PeriodID,
            (int)PeriodTypes.Default);
        html.AppendFormat("</span>");
        html.AppendFormat("<span class='span11'>");


        // Render the period description and date
        var currentPeriodDescriptionDisplay = (period.IsCurrentPeriod) ? "(" + Resources.AutoshipForecast.Current + ") " : string.Empty;
        var dateDisplay = (period.IsCurrentPeriod) 
            ? string.Format("Ending {0:M/d/yyyy}", period.EndDate) 
            : string.Format("{0:M/d/yyyy} {1} {2:M/d/yyyy}", period.StartDate, Resources.AutoshipForecast.Thru, period.EndDate);

        html.AppendFormat("<div class='pull-left'>");
        html.AppendFormat("<span class='description'>{0} {1}</span>", 
            currentPeriodDescriptionDisplay,
            period.PeriodDescription);
        html.AppendFormat("<span class='date'>{0}</span>", dateDisplay);
        html.AppendFormat("</div>");


        // Render the volumes
        html.AppendFormat("<div class='pull-right'>");
        html.AppendFormat("<span class='amount'>{0:N0} {1}</span>", period.CustomersCount, Resources.AutoshipForecast.Distributors);
        if(period.IsCurrentPeriod) html.AppendFormat("<span class='amount actual'>{0}: {1:N0}</span>", Resources.AutoshipForecast.Actual, period.ActualVolume);
        html.AppendFormat("<span class='amount projected'>{0}: {1:N0}</span>", Resources.AutoshipForecast.Projected, period.ProjectedVolume);
        html.AppendFormat("</div>");


        // Render the progress bar
        var maxprojectedvolume = Model.HighestProjectedVolumePeriod.ProjectedVolume * 1.1M; // Add a bit more to this to ensure that we don't have a bar maxed out. It looks better.
        var projectedbarwidth = (period.ProjectedVolume / maxprojectedvolume) * 100M;

        html.AppendFormat("<div class='progress active'>");
        if(period.IsCurrentPeriod)
        {
            // Determine the amount of bar width to give to the actual and projected. 
            var total = projectedbarwidth;
            var actualvolumetotalratio = period.PercentOfActualVolumeToProjectedVolume / 100M;
            var actualbarwidth = actualvolumetotalratio * total;
            projectedbarwidth = (total - actualbarwidth);

            html.AppendFormat("<span class='progress-striped'>");
            html.AppendFormat("<div class='bar bar-warning' style='width: {0}%;'></div>", actualbarwidth);
            html.AppendFormat("</span>");
        }

        // Determine some CSS classes for some stylistic improvements
        var barCssClass = string.Empty;
        var barArrow = string.Empty;
        if(Model.HighestProjectedVolumePeriod.PeriodID == period.PeriodID) 
        {
            barCssClass = "bar-success";
            barArrow = "<div class='arrow arrow-highest'></div>";
        }
        if(Model.LowestProjectedVolumePeriod.PeriodID == period.PeriodID) 
        {
            barCssClass = "bar-danger";
            barArrow = "<div class='arrow arrow-lowest'></div>";
        }

        // Render the projected bar
        html.AppendFormat("<div class='bar {0}' style='width: {1}%;'>{2}</div>", 
            barCssClass,
            projectedbarwidth,
            barArrow);
        html.AppendFormat("</div>");


        // End the progress bar
        html.AppendFormat("</div>");

        html.AppendFormat("</span>");

        return html.ToString();
    }
    #endregion

    #region Event Handlers
    public void ChangeLegFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?leg=" + lstLegFilter.SelectedValue);
    }
    #endregion

    #region Populate Form Options
    public void PopulateLegFilters()
    {
        lstLegFilter.Items.Clear();

        foreach(var enumvalue in Enum.GetValues(typeof(LegFilterType)))
        {
            var listitem = new ListItem();
            listitem.Text = Enum.GetName(typeof(LegFilterType), enumvalue).ToString();
            listitem.Value = ((int)((LegFilterType)Enum.Parse(typeof(LegFilterType), enumvalue.ToString()))).ToString();
            lstLegFilter.Items.Add(listitem);
        }

        lstLegFilter.SelectedValue = ((int)LegFilter).ToString();
    }
    #endregion

    #region Models & Enums
    public class AutoshipForecastModel
    {
        public AutoshipForecastModel()
        {
            this.Periods = new List<AutoshipForecastPeriodNode>();
        }

        public List<AutoshipForecastPeriodNode> Periods { get; set; }

        public AutoshipForecastPeriodNode HighestProjectedVolumePeriod
        {
            get
            {
                return this.Periods.OrderByDescending(c => c.ProjectedVolume).FirstOrDefault();
            }
        }
        public AutoshipForecastPeriodNode LowestProjectedVolumePeriod
        {
            get
            {
                return this.Periods.OrderBy(c => c.ProjectedVolume).FirstOrDefault();
            }
        }

        public int TotalCustomersCount
        {
            get
            {
                return this.Periods.Sum(c => c.CustomersCount);
            }
        }
        public int TotalProjectedOrdersCount
        {
            get
            {
                return this.Periods.Sum(c => c.ProjectedOrdersCount);
            }
        }
        public decimal TotalProjectedVolume
        {
            get
            {
                return this.Periods.Sum(c => c.ProjectedVolume);
            }
        }

        /// <summary>
        /// Calculates each period's percentage of projected volume relative to the total projected volume of all periods.
        /// </summary>
        public void CalculatePercentCompletePerPeriod()
        {
            foreach(var period in this.Periods)
            {
                period.PercentOfProjectedVolumeInAllPeriods = (period.ProjectedVolume / this.TotalProjectedVolume) * 100;
            }
        }
    }

    public class AutoshipForecastPeriodNode
    {
        public int PeriodID { get; set; }
        public string PeriodDescription { get; set; }
        public int CurrentPeriodID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomersCount { get; set; }
        public int ProjectedOrdersCount { get; set; }
        public int ActualOrdersCount { get; set; }
        public decimal ProjectedVolume { get; set; }
        public decimal ActualVolume { get; set; }

        public decimal PercentOfProjectedVolumeInAllPeriods { get; set; }

        public bool IsCurrentPeriod
        {
            get { return this.PeriodID == this.CurrentPeriodID; }
        }
        public decimal PercentOfActualVolumeToProjectedVolume
        {
            get
            {
                if(this.ProjectedVolume == 0) return 0;
                return (this.ActualVolume / this.ProjectedVolume) * 100;
            }
        }
        public decimal AverageProjectedVolumePerCustomer 
        {
            get
            {
                if(this.ProjectedOrdersCount == 0) return 0;
                return this.ProjectedVolume / this.ProjectedOrdersCount;
            }
        }
    }

    public enum LegFilterType
    {
        All = -1,
        Left = 0,
        Right = 1
    }
    #endregion
}