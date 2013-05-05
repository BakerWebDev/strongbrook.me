using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;


public class GridReportHelper
{
    /// <summary>
    /// Helps with operations dealing with Exigo's custom grid reporting feature.
    /// </summary>
	public GridReportHelper()
	{
        // Get the report parameters
        var queryStrings        = HttpContext.Current.Request.QueryString;
        this.Page               = GlobalUtilities.TryParse<int>(queryStrings["page"], 1);
        this.RecordCount        = GlobalUtilities.TryParse<int>(queryStrings["record count"], 50);
        this.SortField          = queryStrings["sortfield"].ToString();
        this.SortOrder          = queryStrings["sortorder"].ToString();
        this.SearchField        = (queryStrings["searchfield"] != null) ? queryStrings["searchfield"].ToString() : string.Empty;
        this.SearchOperator     = (queryStrings["searchoperator"] != null) ? queryStrings["searchoperator"].ToString() : string.Empty;
        this.SearchFilter       = (queryStrings["searchfilter"] != null) ? queryStrings["searchfilter"].ToString() : string.Empty; 
        this.custID             = (queryStrings["id"] != null) ? queryStrings["id"].ToString() : string.Empty;
	}

    #region Properties
    public int Page { get; set; }
    public int RecordCount { get; set; }
    private string SortField { get; set; }
    private string SortOrder { get; set; }
    private string SearchField { get; set; }
    private string SearchOperator { get; set; }
    private string SearchFilter { get; set; }
    private string custID { get; set; }
    #endregion

    #region Ordering & Filtering Methods
    public IOrderedQueryable<T> ApplyOrdering<T>(IQueryable<T> query)
    {
        return query.OrderBy(this.SortField, this.SortOrder);
    }
    public IQueryable<T> ApplyFiltering<T>(IQueryable<T> query)
    {
        if(!string.IsNullOrEmpty(this.SearchField) && !string.IsNullOrEmpty(this.SearchOperator) && !string.IsNullOrEmpty(this.SearchFilter))
        {
            return query.Where(this.SearchField, this.SearchOperator, this.SearchFilter);
        }
        else 
        {
            return query;
        }
    }
    #endregion
}