using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AutoshipForecastDetails : System.Web.UI.Page
{
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
    public List<ReportDataNode> ViewModel
    {
        get
        {
            if(_viewModel == null)
            {
                _viewModel = FetchReportData();
            }
            return _viewModel;
        }
    }
    private List<ReportDataNode> _viewModel;

    public int PeriodID
    {
        get { return Convert.ToInt32(Request.QueryString["pid"]); }
    }
    public int PeriodTypeID
    {
        get { return Convert.ToInt32(Request.QueryString["ptid"]); }
    }
    public Period Period
    {
        get
        {
            if(_period == null)
            {
                _period = ExigoApiContext.CreateODataContext().Periods
                    .Where(c => c.PeriodID == PeriodID)
                    .Where(c => c.PeriodTypeID == PeriodTypeID)
                    .FirstOrDefault();
            }
            return _period;
        }
    }
    private Period _period;

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
    public List<ReportDataNode> FetchReportData()
    {
        /*var helper = new SqlHelper();

        var datatable = helper.GetTable(@"
                DECLARE @CustomerID INT = {0}
                DECLARE @Leg INT = {1}
                DECLARE @PeriodTypeID INT = {2}
                DECLARE @PeriodID INT = {3}

                DECLARE @lft        INT,
                        @rgt        INT,
                        @leftleft   INT,
                        @leftright  INT,
                        @rightleft  INT,
                        @rightright INT

                DECLARE @StartRange DATETIME,
                        @EndRange   DATETIME

                SELECT @StartRange = min(StartDate)
	                 , @EndRange = max(EndDate)
                FROM
	                Periods
                WHERE
	                PeriodTypeID = @PeriodTypeID
	                AND PeriodID = @PeriodID



                SELECT @leftleft = lft
	                 , @leftright = rgt
                FROM
	                BinaryTree
                WHERE
	                ParentID = @CustomerID
	                AND Placement = 0



                SELECT @rightleft = lft
	                 , @rightright = rgt
                FROM
	                BinaryTree
                WHERE
	                ParentID = @CustomerID
	                AND Placement = 1



                IF @leg = 0
                BEGIN
	                SELECT @Lft = @leftleft
		                 , @rgt = @leftright
	                FROM
		                BinaryTree
	                WHERE
		                ParentID = @CustomerID

                END



                IF @leg = 1
                BEGIN
	                SELECT @Lft = @rightleft
		                 , @rgt = @rightright
	                FROM
		                BinaryTree
	                WHERE
		                ParentID = @CustomerID

                END



                IF @leg = -1
                BEGIN
	                SELECT @Lft = lft + 1
		                 , @rgt = rgt - 1
	                FROM
		                BinaryTree
	                WHERE
		                CustomerID = @CustomerID
                END




                SELECT ao.CustomerID AS CustomerID
	                 , ao.BusinessVolumeTotal AS AutoshipVolume
	                 , ao.FirstName
                     , ao.LastName
                     , ao.Company
	                 , NextShipDate = CASE
		                   WHEN aosc.ProcessedDate IS NULL THEN
			                   aosc.ScheduledDate
		                   ELSE
			                   aosc.ScheduledDate
	                   END
	                 , ao.AutoOrderID
	                 , CASE
		                   WHEN t.lft BETWEEN @leftleft AND @leftright THEN
			                   'Left'
		                   WHEN t.lft BETWEEN @rightleft AND @rightright THEN
			                   'Right'
		                   ELSE
			                   ''
	                   END AS Leg
	                 , isnull(o.OrderStatusID, 0) AS OrderStatusID
                FROM
	                BinaryTree t
	                INNER JOIN AutoOrders ao
		                ON t.CustomerID = ao.CustomerID AND ao.AutoOrderStatusID = 0 AND ao.BusinessVolumeTotal > 0
	                INNER JOIN Periods p
		                ON p.PeriodTypeID = @PeriodTypeID AND p.PeriodID = @PeriodID
	                LEFT JOIN AutoOrderSchedules aosc
		                ON aosc.autoorderid = ao.autoorderid AND aosc.IsEnabled = 1 AND aosc.ScheduledDate >= @StartRange AND aosc.ScheduledDate < @EndRange + 1
	                LEFT JOIN Orders o
		                ON o.OrderID = aosc.OrderID
                WHERE
	                t.lft BETWEEN @lft AND @rgt
	                AND p.PeriodID = @PeriodID
	                AND p.PeriodTypeID = @PeriodTypeID
	                AND aosc.ScheduledDate >= @StartRange
	                AND aosc.ScheduledDate < @EndRange + 1
                ORDER BY
	                NextShipDate
            ", 
                    Identity.Current.CustomerID,
                    (int)LegFilter,
                    PeriodTypeID,
                    PeriodID);


        // Assemble the nodes
        var nodes = new List<ReportDataNode>();            
        foreach(DataRow row in datatable.Rows)
        {
            var node = new ReportDataNode();

            node.CustomerID             = Convert.ToInt32(row["CustomerID"]);
            node.AutoOrderID            = Convert.ToInt32(row["AutoOrderID"]);
            node.AutoshipVolume         = Convert.ToDecimal(row["AutoshipVolume"]);
            node.FirstName              = row["FirstName"].ToString();
            node.LastName               = row["LastName"].ToString();
            node.Company                = row["Company"].ToString();
            node.NextShipDate           = Convert.ToDateTime(row["NextShipDate"]);
            node.OrderStatusID          = Convert.ToInt32(row["OrderStatusID"]);
            node.Leg                    = row["Leg"].ToString();
            node.Shipped                = (node.OrderStatusID >= 7);

            nodes.Add(node);
        }*/


        // Return the nodes
        return new List<ReportDataNode>();
    }
    #endregion

    #region Event Handlers
    public void ChangeLegFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath + "?leg=" + lstLegFilter.SelectedValue + "&pid=" + PeriodID + "&ptid=" + PeriodTypeID);
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
    public class ReportDataNode
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string DisplayName
        {
            get
            {
                return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName);
            }
        }
        public bool Shipped { get; set; }
        public int AutoOrderID { get; set; }
        public int OrderStatusID { get; set; }
        public decimal AutoshipVolume { get; set; }
        public DateTime NextShipDate { get; set; }
        public string Leg { get; set; }
    }

    public enum LegFilterType
    {
        All = -1,
        Left = 0,
        Right = 1
    }
    #endregion
}