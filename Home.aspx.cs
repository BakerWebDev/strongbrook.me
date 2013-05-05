using Exigo.OData;
using Exigo.RankQualificationGoals;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Home : System.Web.UI.Page
{
    private int customerID = Identity.Current.CustomerID;
    public int rankID = Identity.Current.Ranks.CurrentPeriodRankID;
    private int periodType = PeriodTypes.Default;
    public string actualRankTitle = Identity.Current.Ranks.HighestAchievedRankDescription;
    public string currentMonthsQualifiedRank = Identity.Current.Ranks.CurrentPeriodRankDescription;
    public string guaranteedMinPaidAsRank = Identity.Current.Ranks.HighestCurrentPeriodRankDescription;

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region Properties
    public int ViewingRankID
    {
        get
        {
            if(Request.QueryString["rankid"] != null)
            {
                return Convert.ToInt32(Request.QueryString["rankid"]);
            }
            else
            {
                return (Identity.Current.Ranks.CurrentPeriodRankID > 0) ? Identity.Current.Ranks.CurrentPeriodRankID : 1;
            }
        }
    }
    public string ViewingRankDescription { get; set; }

    public int CurrentRankID { get; set; }

    public PeriodVolume Volumes
    {
        get
        {
            if(_volumes == null)
            {
                _volumes = FetchCurrentVolumes();
            }
            return _volumes;
        }
    }
    private PeriodVolume _volumes;

    public PeriodVolume VolumesLastMonth
    {
        get
        {
            if(_volumesLastMonth == null)
            {
                _volumesLastMonth = FetchLastMonthsVolumes();
            }
            return _volumesLastMonth;
        }
    }
    private PeriodVolume _volumesLastMonth;

    public CommissionResponse Commissions
    {
        get
        {
            if(_commissions == null)
            {
                _commissions = FetchCurrentCommissions();
            }
            return _commissions;
        }
    }
    private CommissionResponse _commissions;
    #endregion Properties

    #region Fetching Data

    public PeriodVolume FetchCurrentVolumes()
    {
        return ExigoApiContext.CreateODataContext().PeriodVolumes
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == periodType)
            .Where(c => c.Period.IsCurrentPeriod)
            .FirstOrDefault();
    }
    public PeriodVolume FetchLastMonthsVolumes()
    {
        var pvcontext = ExigoApiContext.CreateODataContext().PeriodVolumes;

        var row = (from pv in pvcontext
                   where pv.PeriodTypeID == periodType
                   where pv.CustomerID == Identity.Current.CustomerID
                   where pv.PeriodID == GlobalUtilities.GetCurrentPeriodID() -1
                   select new
                   {
                       pv.CustomerID,
                       pv.PeriodTypeID,
                       pv.PeriodID,
                       pv.Volume1,
                       pv.Volume3
                   }).SingleOrDefault();

        if (row == null) return new PeriodVolume();

        // Assemble the model
        var model = new PeriodVolume();
        model.CustomerID = row.CustomerID;
        model.PeriodTypeID = row.PeriodTypeID;
        model.PeriodID = row.PeriodID;
        model.Volume1 = row.Volume1;
        model.Volume3 = row.Volume3;

        // Return the model
        return model; 
    }

    public CommissionResponse FetchCurrentCommissions()
    {
        try
        {
            return ExigoApiContext.CreateWebServiceContext().GetRealTimeCommissions(new GetRealTimeCommissionsRequest
            {
                CustomerID = Identity.Current.CustomerID
            }).Commissions[0];
        }
        catch
        {
            return new CommissionResponse();
        }
    }

    #region Current Commissions Earnings
    public decimal CurrentWeeklyCommissionsEarnings()
	{
        decimal amount;
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == FetchPreviousCommissionRunID()
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

        if(query != null)
        {
            amount = query.Earnings;
        }
        else
        {
            amount = 0;
        }
        
		return amount;	
	}
    public string CurrentWeeklyCommissionsPeriodDescription()
	{
        string dateString;
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == FetchPreviousCommissionRunID()
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.CommissionRun.Period.PeriodDescription
					 }).FirstOrDefault();

        if(query != null)
        {
            dateString = query.PeriodDescription;
        }
        else
        {
            dateString = "this period not avail.";
        }
        
		return dateString;	
	}
	public decimal CurrentMonthlyCommissionsEarnings()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == 877
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}	
    public decimal CurrentMonthsCombinedCommissionsEarnings()
    {
        decimal sum = 0;
        var MonthlyCommissionsTotal = CurrentMonthlyCommissionsEarnings();
        var WeeklyCommissionsTotal = CurrentWeeklyCommissionsEarnings();

        sum = WeeklyCommissionsTotal + MonthlyCommissionsTotal;

        return sum;
    }
    public decimal LatestCheckPaidAmount()
    {
        decimal amount = 0;
        var WeeklyCommissionsTotal = CurrentWeeklyCommissionsEarnings();
        amount = WeeklyCommissionsTotal;
        return amount;
    }
    public string LatestCheckPaidPeriodDescription()
    {
        string dateString = "";
        var WeeklyCommissionsPeriodDescription = CurrentWeeklyCommissionsPeriodDescription();
        dateString = WeeklyCommissionsPeriodDescription;
        return dateString;
    }
    #endregion Current Commissions Earnings

    #region Commissions Earnings 1 Month Ago



    public decimal CommissionsEarnings1MonthAgoWeeklyWeek1()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;

        var details = CommissionRunDetails2;

		var query = (from c in context
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == details.CommissionRunID // 780
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}








    public decimal CommissionsEarnings1MonthAgoWeeklyWeek2()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == 809
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}

	public decimal CommissionsEarnings1MonthAgoMonthly()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;
        var details = CommissionRunDetails;

		var query = (from c in context
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == details.CommissionRunID // 877
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}	

    public decimal CombinedCommissionsEarnings1MonthAgo()
    {
        decimal sum = 0;
        var MonthlyCommissionsTotal = CommissionsEarnings1MonthAgoMonthly();
        var WeeklyCommissionsTotalWeek1 = CommissionsEarnings1MonthAgoWeeklyWeek1();
        //var WeeklyCommissionsTotalWeek2 = CommissionsEarnings1MonthAgoWeeklyWeek2();

        sum = WeeklyCommissionsTotalWeek1; // + WeeklyCommissionsTotalWeek2 + MonthlyCommissionsTotal;

        return sum;
    }
    #endregion Commissions Earnings 1 Month Ago

    #region Commissions Earnings 2 Months Ago
    public decimal CommissionsEarnings2MonthsAgoWeekly()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == 874
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}
	public decimal CommissionsEarnings2MonthsAgoMonthly()
	{
        var context = ExigoApiContext.CreateODataContext().Commissions;

		var query = (from c in context // .Expand("CommissionRun")
					 where c.CustomerID == Identity.Current.CustomerID
					 where c.CommissionRunID == 834
					 where c.CurrencyCode == Identity.Current.CurrencyCode
					 select new
					 {
						 c.Earnings
					 }).FirstOrDefault();

		var results = query.Earnings;
		return results;	
	}	
    public decimal CombinedCommissionsEarnings2MonthsAgo()
    {
        decimal sum = 0;
        var MonthlyCommissionsTotal = CommissionsEarnings2MonthsAgoMonthly();
        var WeeklyCommissionsTotal = CommissionsEarnings2MonthsAgoWeekly();

        sum = WeeklyCommissionsTotal + MonthlyCommissionsTotal;

        return sum;
    }
    #endregion Commissions Earnings 2 Months Ago

    #region Fetch GPR Data

    public List<decimal> WeeklyList = new List<decimal>();
    public List<decimal> MonthlyList = new List<decimal>();

    public List<ReportDataNode> FetchGPRWeeklyReportData()
    {
        //      Query the OData tables      //
        #region Query the OData tables
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume99 != 0);
        #endregion Query the OData tables

        //      Fetch the nodes     //
        #region Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            VolumeBucket99  = c.PeriodVolume.Volume99, // GPR Credits Weekly
        }).ToList();
        #endregion Fetch the nodes

        //      Add values to the Weekly GPR List  //
        #region Add values to the Weekly GPR List
        foreach (var customer in nodes)
        {
            decimal theValueOfVolumeBucket99 = customer.VolumeBucket99;

            WeeklyList.Add(theValueOfVolumeBucket99);
        }
        #endregion

        // Return the nodes
        return nodes;
    }

    public List<ReportDataNode> FetchGPRMonthlyReportData()
    {
        #region Query the OData tables
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Monthly)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume83 != 0);
        #endregion Query the OData tables

        #region Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            VolumeBucket83 = c.PeriodVolume.Volume83, // GPR Credits Monthly
        }).ToList();
        #endregion Fetch the nodes

        #region Add values to the Monthly GPR List
        foreach (var customer in nodes)
        {
            decimal theValueOfVolumeBucket83 = customer.VolumeBucket83;

            MonthlyList.Add(theValueOfVolumeBucket83);
        }
        #endregion

        // Return the nodes
        return nodes;
    }



    public ReportDataNode FetchPersonalGPRWeeklyReportData()
    {
        //      Query the OData tables      //
        #region Query the OData tables
        var query = ExigoApiContext.CreateODataContext().PeriodVolumes
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly)
            .Where(c => c.Period.IsCurrentPeriod);
        #endregion Query the OData tables

        //      Fetch the nodes     //
        #region Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            VolumeBucket99  = c.Volume99, // GPR Credits Weekly
        }).FirstOrDefault();
        #endregion Fetch the nodes

        // Return the nodes
        return nodes;
    }

    #endregion Fetch GPR Data

    #endregion Fetching Data

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if(Request.QueryString["datakey"] != null)
        {
            Response.Clear();

            switch(Request.QueryString["datakey"])
            {
                case "qualifications":
                    try
                    {
                        writer.Write(GetRankAdvancementWidgetHTML(Convert.ToInt32(Request.QueryString["rid"])));
                    }
                    catch(Exception ex)
                    {
                        if(ex.Message.Contains("Unavailable"))
                        {
                            throw ex;
                        }
                        else
                        {
                            writer.Write(ex.Message);
                        }
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

    public void RenderCompanyNews()
    {
        var html = new StringBuilder();

        // Get the data
        var response = ExigoApiContext.CreateWebServiceContext().GetCompanyNews(new GetCompanyNewsRequest
        {
            DepartmentType = NewsDepartments.Backoffice
        });


        // Filter the news results
        var filteredNewsResults = response.CompanyNews
            .Where(c => c.WebSettings == NewsWebSettings.AccessAvailable)
            .Where(c => c.CompanySettings != NewsCompanySettings.AccessNotAvailable);

        // Compile the news list
        html.AppendFormat("<ul class='list-companynews'>");
        foreach(var news in filteredNewsResults)
        {
            html.AppendFormat("<li><a href='NewsDetail.aspx?id={1}'>{0}</a></li>", news.Description, news.NewsID);
        }
        html.AppendFormat("</ul>");


        // Render the results
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderRecentActivity()
    {
        var html = new StringBuilder();

        // Get the data
        var items = ExigoApiContext.CreateODataContext().CustomerWall
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => c);


        // Compile the news list
        html.AppendFormat("<ul class='list-recentactivity'>");
        foreach(var item in items)
        {
            html.AppendFormat("<li class='color3'>");
            html.AppendFormat("<a href='javascript:;'><img src='{0}' /></a>", GlobalUtilities.GetCustomerTinyAvatarUrl(item.CustomerID));
            html.AppendFormat("<p>");
            html.AppendFormat("{0}. ", item.Text);
            html.AppendFormat("<span class='date color4'>{0:M/d/yyyy}</span>", item.EntryDate);
            html.AppendFormat("</p>");
            html.AppendFormat("</li>");
        }
        html.AppendFormat("</ul>");


        // Render the results
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderMessagesTile()
    {
        // Get the unread messages count
        var service = new MessagesService();
        var unreadMailCount = service.GetUnreadMailCount();
        var unreadMailCountDisplay = (unreadMailCount > 99) ? "99+" : unreadMailCount.ToString();


        var html = new StringBuilder();

        if(unreadMailCount > 0)
        {
            html.AppendFormat("<div class='tile tile-icon-highlighted size-1x1 theme-magenta tile-animated'>");
            html.AppendFormat("<h2 class='animated'>{0}<img style='max-height:48px;' src='Assets/Images/icnEnvelope.png' /></h2>", unreadMailCountDisplay);
            html.AppendFormat("<h4>{0}</h4>", Resources.Dashboard.NewMessages);
            html.AppendFormat("</div>");
        }
        else
        {
            html.AppendFormat("<div class='tile tile-icon size-1x1 theme-aqua2'>");
            html.AppendFormat("<img style='max-height:48px;' src='Assets/Images/icnEnvelope.png' />");
            html.AppendFormat("<h4>{0}</h4>", Resources.Dashboard.Messages);
            html.AppendFormat("</div>");
        }


        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #region Render GPR Data

    public void Render_UniLevelDownlineGPR_Count_Weekly()
    {
        var html = new StringBuilder();

        #region Render the count of nodes.
        var nodes = FetchGPRWeeklyReportData();

        html.AppendFormat(@"
            {0}
            "
            , nodes.Count()
            );
        #endregion

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }
    public void Render_UniLevelDownline_GPRsPerPerson_Weekly()
    {
        var html = new StringBuilder();

        #region Render the number of GPR's for each person in UniLeval downline.
        var nodes = FetchGPRWeeklyReportData();

        foreach (var number in WeeklyList)
        {
            html.AppendFormat(@"
                {0}
                "
                , number.ToString("N")
                );
        }
        #endregion Render the number of GPR's for each person in UniLeval downline.

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }
    public void Render_UniLevelDownline_GPR_Average_Per_Month()
    {
        var html = new StringBuilder();

        //      Build the data to be used.
        var nodes = FetchGPRMonthlyReportData();

        //      If nodes returns any data proceed.
        if (nodes.Count > 0)
        {
            #region Render the Average number of GPR's for UniLevel downline.

            #region Turn the list into an array, and get the average of those numbers.

            // Turn the list into an array
            decimal[] listNumbersConverted = MonthlyList.ToArray();

            // Add the credits for each qualified person together.
            decimal sumOfGPRs = 0;
            foreach (decimal x in listNumbersConverted)
            {
                sumOfGPRs = x + sumOfGPRs++;
            }

            // Get the total number of qualified downline customers.
            decimal count = listNumbersConverted.Count();

            // Divide the total number of organizational credits by the number of customers in the organization.
            decimal averageNum = sumOfGPRs / count;

            #endregion Turn the list into an array, and get the average of those numbers.

            if (averageNum != 0)
            {
                html.AppendFormat(@"
                    {0:N}
                    "
                    , averageNum.ToString("N")
                    );
//                html.AppendFormat(@"
//                    {1}<br />
//                    {0:N}
//                    "
//                    , averageNum.ToString("N")
//                    , count
//                    );
            }
            else
            {
                html.AppendFormat(@"
                {0}
                "
                    , "0"
                    );
            }

            #endregion Render the Average number of GPR's for UniLevel downline.
        }
        else
        {
            html.AppendFormat(@"
                {0}
                "
                , "0"
                );
        }

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void Render_Personal_GPR_Count_PeriodType_Weekly()
    {
        var html = new StringBuilder();

        var nodes = FetchPersonalGPRWeeklyReportData();

        if (nodes.VolumeBucket99 != null)
        {
            var number = nodes.VolumeBucket99;

            html.AppendFormat(@"
                {0}
                "
                , number.ToString("N0")
                );
        }
        else
        {
            html.AppendFormat(@"
                {0}
                "
                , "0"
                );            
        }
       

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }

    #endregion  Render GPR Data
    public string GetRankAdvancementWidgetHTML(int rankID)
    {
        var html = new StringBuilder();

        // Get the data
        var goalsService = new RankQualificationGoalsService();
        var goals = goalsService.GetRankQualificationGoals(Identity.Current.CustomerID, rankID);


        html.AppendFormat("<a href='javascript:loadRankAdvancement({0});' class='previous'><img src='Assets/Images/btnArrowLeftWhite.png' /></a>", goals.PreviousRankID);
        html.AppendFormat("<a href='javascript:loadRankAdvancement({0});' class='next'><img src='Assets/Images/btnArrowRightWhite.png' /></a>", goals.NextRankID);

        html.AppendFormat("<div class='content'>");
        html.AppendFormat("<h3>{0}</h3>", goals.RankDescription);

        // Progress Bar
        html.AppendFormat("<div class='progress progress-danger'>");
        html.AppendFormat("<div class='bar' style='width: 60%'></div>");
        html.AppendFormat("</div>");

        // Render the percent complete
        html.AppendFormat("<div class='row-fluid'>");
        html.AppendFormat("<span class='span4 percent'>");
        html.AppendFormat("<h4>{0:0}%</h4>", goals.TotalPercentComplete);
        html.AppendFormat("</span>");

        // Render the remaining decimal goal qualifications
        html.AppendFormat("<span class='span8 goals'>");
        foreach(var goal in goals.RankQualifications.Where(c => !c.IsBoolean).Where(c => !c.IsQualified))
        {
            html.AppendFormat("<div class='goal'>");
            html.AppendFormat("<div class='description'>{0}</div>", goal.Label);
            html.AppendFormat("<div class='progress progress-warning'>");
            html.AppendFormat("<div class='bar' style='width: {0:0}%'></div>", goal.RequiredToActualAsPercent);
            html.AppendFormat("</div>");
            html.AppendFormat("<div class='stats'>{0:N0} / {1:N0}</div>", goal.ActualValue, goal.RequiredValue);
            html.AppendFormat("<div class='clearfix'></div>");
            html.AppendFormat("</div>");
        }

        // Render the remaining boolean goal qualifications
        html.AppendFormat("<ul>");
        foreach(var goal in goals.RankQualifications.Where(c => c.IsBoolean).Where(c => !c.IsQualified))
        {
            html.AppendFormat("<li>{0}</li>", goal.NotQualifiedDescription);
        }
        html.AppendFormat("</ul>");
        html.AppendFormat("</span>");

        html.AppendFormat("</div>");
        html.AppendFormat("</div>");


        return html.ToString();
    }
    public void RenderCurrentCheckReceivedAmount()
    {
        var LastCommissionsCheckPaidAmount = LatestCheckPaidAmount();

        var html = new StringBuilder();

        html.AppendFormat(@"<tr><td style=""text-align:right;"">{0:C}</td></tr>", LastCommissionsCheckPaidAmount);
       
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderCurrentCheckReceivedPeriodDescription()
    {
        var LastCommissionsCheckPaidPeriodDescription = LatestCheckPaidPeriodDescription();

        var html = new StringBuilder();

        html.AppendFormat(@"<tr><td style=""text-align:right;"">{0:C}</td></tr>", LastCommissionsCheckPaidPeriodDescription);
       
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void Render3MonthsOfEarnings()
    {
        DateTime now = DateTime.Now;
        var ThisMonth = now.ToString("MMMM");
        var LastMonth = now.AddMonths(-1).ToString("MMMM");
        var TwoMonthsAgo = now.AddMonths(-2).ToString("MMMM");

        //var commissionsTotal = Commissions.PeriodID.ToString("C");
        var CurrentCommissionsTotal = CurrentMonthsCombinedCommissionsEarnings();
        var CommissionsTotal1MonthAgo = CombinedCommissionsEarnings1MonthAgo();
        var CommissionsTotal2MonthsAgo = CombinedCommissionsEarnings2MonthsAgo();


        var html = new StringBuilder();

        html.AppendFormat(@"<tr><td style=""text-align:right"">{0}: </td><td style=""text-align:right;"">{1:C}</td></tr>", TwoMonthsAgo, CommissionsTotal1MonthAgo);
        html.AppendFormat(@"<tr><td style=""text-align:right"">{0}: </td><td style=""text-align:right;"">{1:C}</td></tr>", LastMonth, CommissionsTotal1MonthAgo);
        html.AppendFormat(@"<tr><td style=""text-align:right"">{0}: </td><td style=""text-align:right;"">{1:C}</td></tr>", ThisMonth, CommissionsTotal1MonthAgo);
       

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderMyBonuses()
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
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #endregion Render



    #region Travis'
    public void RenderCommissionRunSummary()
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
        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }

    #endregion Travis'

    #region Travis'
    private int PeriodTypeID2 = PeriodTypes.Monthly;
    //public int PeriodTypeID2 { get; set; }
    #endregion Travis'

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
                    //if (CurrentCommissions.Count > 0)
                    //{
                    //    periodID = CurrentCommissions.FirstOrDefault().PeriodID;
                    //}
                    //else
                    //{
                    //    periodID = PriorCommissions.FirstOrDefault().CommissionRun.PeriodID;
                    //}
                    periodID = PriorCommissions.FirstOrDefault().CommissionRun.PeriodID;
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

    #region I added this
    public int MonthlyCommissionRunID
    {
        get
        {
            if(monthlyCommissionRunID == null)
            {
                monthlyCommissionRunID = 736; //FetchMonthlyCommissionRunID();
            }
            return Convert.ToInt32(monthlyCommissionRunID);
        }
    }
    private int? monthlyCommissionRunID;

    public int PreviousCommissionRunID
    {
        get
        {
            if(previousCommissionRunID == null)
            {
                previousCommissionRunID = FetchCurrentCommissionRunID();
            }
            return Convert.ToInt32(previousCommissionRunID);
        }
    }
    private int? previousCommissionRunID;


    #endregion I added this




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

                // Prior Commissions
                if (CommissionType == CommissionPeriodType.Prior)
                {
                    var details = CommissionRunDetails;
                    var data = CommissionDetails2;
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
                    _dataModel.Volume2 = PeriodVolumes.Volume2;
                    _dataModel.Volume3 = PeriodVolumes.Volume3;

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

    //public List<CommissionResponse> CurrentCommissions
    //{
    //    get
    //    {
    //        _currentCommissions = _currentCommissions ?? FetchCurrentCommissions();
    //        return _currentCommissions;
    //    }
    //}
    //private List<CommissionResponse> _currentCommissions;

    //public List<CommissionBonusResponse> CurrentCommissionBonuses
    //{
    //    get
    //    {
    //        _currentCommissionBonuses = _currentCommissionBonuses ?? FetchCurrentCommissionBonuses();
    //        return _currentCommissionBonuses;
    //    }
    //}
    //private List<CommissionBonusResponse> _currentCommissionBonuses;

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



    public Commission CommissionDetails2
    {
        get
        {
            if (_commissionDetails2 == null)
            {
                _commissionDetails2 = FetchCommissionDetails2();
            }
            return _commissionDetails2;
        }
    }
    private Commission _commissionDetails2;


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

    public CommissionRun CommissionRunDetails2
    {
        get
        {
            if (_commissionRunDetails2 == null)
            {
                _commissionRunDetails2 = FetchCommissionRunDetails2();
            }
            return _commissionRunDetails2;
        }
    }
    private CommissionRun _commissionRunDetails2;

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
    //private List<CommissionResponse> FetchCurrentCommissions()
    //{
    //    return ExigoApiContext.CreateWebServiceContext().GetRealTimeCommissions(new GetRealTimeCommissionsRequest
    //    {
    //        CustomerID = Identity.Current.CustomerID
    //    }).Commissions
    //        .Where(c => c.PeriodType == PeriodTypeID2)
    //        .OrderByDescending(c => c.PeriodID).ToList();        
    //}
    //private List<CommissionBonusResponse> FetchCurrentCommissionBonuses()
    //{
    //    return CurrentCommissions[0].Bonuses.ToList();
    //}

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




    #region I added this
    private Commission FetchCommissionDetails2()
    {
        return (from c in ExigoApiContext.CreateODataContext().Commissions
                where c.CustomerID == Identity.Current.CustomerID
                where c.CommissionRunID == MonthlyCommissionRunID
                where c.CurrencyCode == Identity.Current.CurrencyCode
                select c).FirstOrDefault();
    }
    public CommissionRun FetchCommissionRunDetails2()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns.Expand("Period")
                    where c.CommissionRunID == MonthlyCommissionRunID
                    select c).FirstOrDefault();

        if (data == null) return new CommissionRun();
        else return data;
    }


    private Commission FetchPriorCommissionDetails()
    {
        return (from c in ExigoApiContext.CreateODataContext().Commissions
                where c.CustomerID == Identity.Current.CustomerID
                where c.CommissionRunID == CurrentCommissionRunID
                where c.CurrencyCode == Identity.Current.CurrencyCode
                select c).FirstOrDefault();
    }
    public CommissionRun FetchPriorCommissionRunDetails()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns.Expand("Period")
                    where c.CommissionRunID == CurrentCommissionRunID
                    select c).FirstOrDefault();

        if (data == null) return new CommissionRun();
        else return data;
    }
    private List<CommissionBonus> FetchPriorCommissionBonuses()
    {
        return (from c in ExigoApiContext.CreateODataContext().CommissionBonuses
                where c.CustomerID == Identity.Current.CustomerID
                where c.CommissionRunID == CurrentCommissionRunID
                orderby c.BonusDescription
                select c).ToList();
    }

    #endregion I added this
    //private List<CommissionBonus> FetchCommissionBonuses()
    //{
    //    return (from c in ExigoApiContext.CreateODataContext().CommissionBonuses
    //            where c.CustomerID == Identity.Current.CustomerID
    //            where c.CommissionRunID == MonthlyCommissionRunID
    //            orderby c.BonusDescription
    //            select c).ToList();
    //}



    private PeriodVolume FetchPeriodVolumes()
    {
        return (from o in ExigoApiContext.CreateODataContext().PeriodVolumes
                where o.CustomerID == Identity.Current.CustomerID
                where o.PeriodTypeID == PeriodTypeID2
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







    // This seems to work for Monthly but not for Weekly.

    int January = 15;
    int February = 16;
    int March = 17;

    private int FetchMonthlyCommissionRunID()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns
                    where c.PeriodID == 884// January // PeriodID
                    orderby c.CommissionRunID descending
                    select new
                    {
                        c.CommissionRunID
                    }).FirstOrDefault();

        if (data == null) return 0;
        else return data.CommissionRunID;
    }

    int Week17 = 236;

    //private int FetchMonthlyCommissionRunID()
    //{
    //    var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns
    //                where c.PeriodID == January // PeriodID
    //                orderby c.CommissionRunID descending
    //                select new
    //                {
    //                    c.CommissionRunID
    //                }).FirstOrDefault();

    //    if (data == null) return 0;
    //    else return data.CommissionRunID;
    //}




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


    #region I added this
    private int FetchPreviousCommissionRunID()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns
                    where c.PeriodID == PeriodID - 1
                    orderby c.CommissionRunID descending
                    select new
                    {
                        c.CommissionRunID
                    }).FirstOrDefault();

        if (data == null) return 0;
        else return data.CommissionRunID;
    }
    #endregion I added this


































    #region I added this
    private int FetchCommissionRunIDFrom1MonthAgo()
    {
        var data = (from c in ExigoApiContext.CreateODataContext().CommissionRuns
                    where c.PeriodID == PeriodID - 1
                    orderby c.CommissionRunID descending
                    select new
                    {
                        c.CommissionRunID
                    }).FirstOrDefault();

        if (data == null) return 0;
        else return data.CommissionRunID;
    }
    #endregion I added this

    // Misc. Data
    private Period FetchPeriodDetails()
    {
        return (from c in ExigoApiContext.CreateODataContext().Periods
                where c.PeriodID == PeriodID
                where c.PeriodTypeID == PeriodTypeID2
                select new Period()
                {
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                }).FirstOrDefault();
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
        public decimal Volume2 { get; set; }
        public decimal Volume3 { get; set; }

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
        Prior = 1,
        OneMonthAgo = 2,
        TwoMonthsAgo = 3
    }
    #endregion





    #region Models
    public class ReportDataNode
    {
        public int CustomerID { get; set; }
        public decimal VolumeBucket83 { get; set; }
        public decimal VolumeBucket98 { get; set; }
        public decimal VolumeBucket99 { get; set; }
        public decimal VolumeBucket100 { get; set; }
    }
    #endregion Models















}
