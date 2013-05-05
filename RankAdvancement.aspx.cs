using Exigo.OData;
using Exigo.RankQualificationGoals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RankAdvancement : System.Web.UI.Page
{
    private int customerID = Identity.Current.CustomerID;
    public int rankID = Identity.Current.Ranks.CurrentPeriodRankID;
    private int periodTypeID = (int)PeriodTypes.Default;





    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the current rank ID
        CurrentRankID = (Identity.Current.Ranks.CurrentPeriodRankID > 0) ? Identity.Current.Ranks.CurrentPeriodRankID : 1;

        ViewingRankDescription = GlobalUtilities.Coalesce((from r in ExigoApiContext.CreateODataContext().Ranks
                                                           where r.RankID == ViewingRankID
                                                           select r.RankDescription).FirstOrDefault(), "Unavailable");
    }
    #endregion Page Load

    #region Propertiess
    public List<Rank> Ranks
    {
        get
        {
            if(_ranks == null)
            {
                _ranks = FetchAllRanks();
            }
            return _ranks;
        }
    }
    private List<Rank> _ranks;

    public int ViewingRankID
    {
        get
        {
            if (Request.QueryString["rankid"] != null)
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
    #endregion Properties

    #region Fetching Data
    public List<Rank> FetchAllRanks()
    {
        var ranks = ExigoApiContext.CreateODataContext().Ranks
            .Where(c => c.RankDescription != null && c.RankDescription != "")
            .Where(c => c.RankID > 0)
            .Where(c => c.RankID < 16)
            .Select(c => c)
            .ToList();

        if(ranks.Count() > 0) return ranks;
        else return new List<Rank>();
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
                case "qualifications":
                    try
                    {
                        RenderQualifications(writer, ViewingRankID);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Unavailable"))
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
    public void RenderQualifications(HtmlTextWriter writer, int selectedRankID)
    {
        // Get the data
        var goalsService = new RankQualificationGoalsService();
        var goals = goalsService.GetRankQualificationGoals(Identity.Current.CustomerID, ViewingRankID);


        // Render the name of the rank we are viewing
        writer.Write("<h1 class='color2'>{0}</h1>", ViewingRankDescription);


        // Render the percent complete
        var percentComplete = goals.TotalPercentComplete;
        if(percentComplete == 100)
        {
            writer.Write(string.Format("<span class='label label-success'>Qualified</span>"));
        }
        else
        {
            writer.Write(string.Format("<span class='label label-important'>{0:N0}% Complete</span>", percentComplete));
        }


        // Start with the decimal qualifications
        writer.Write("<div class='row-fluid'>");
        writer.Write("<span class='span8'>");
        writer.Write("<h3>My Goals</h3>");
        
        var falseDecimalGoals = goals.RankQualifications.Where(c => !c.IsBoolean && c.IsQualified == false);
        foreach(var goal in falseDecimalGoals)
        {

            writer.Write(string.Format(@"
                    <div class='goal'>
                        <div class='row-fluid'>
                            <span class='span10'>
                                <span class='description'>{0}</span>
                                <div class='progress {1}'>
                                    <div class='bar' style='width: {2:0}%;'></div>
                                </div>                                
                                <span class='totals'>{3:N0} / {4:N0}</span>
                            </span>
                            <span class='span2'>
                                <span class='percent'>{2:N0}%</span>
                            </span>
                        </div>
                    </div>
            ",
                goal.Label,
                goal.GetProgressBarColorClass(),
                goal.RequiredToActualAsPercent,
                goal.ActualValueAsDecimal,
                goal.RequiredValueAsDecimal));
        }

        writer.Write("</span>");


        // Now, render all the non-qualified description
        var falseGoals = goals.RankQualifications.Where(c => c.IsQualified == false);

        writer.Write("<span class='span4'>");
        writer.Write("<h3>What's Left?</h3>");
        writer.Write("<ul class='tips'>");

        foreach(var goal in falseGoals)
        {
            writer.Write(string.Format(@"
                <li>{0}</li>
            ", goal.NotQualifiedDescription));
        }

        writer.Write("</ul>");
        writer.Write("</span>");
        writer.Write("</div>");
    }
    #endregion Render
}