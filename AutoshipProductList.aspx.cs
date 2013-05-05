using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

/// <summary>
/// URL Query String Variables
/// page = current page for the pager (1 is defaulted in the Page_Load)
/// wid = optional web ID (whatever value is set in the settings file is defaulted if this varable is not present)
/// wcid = optional web category ID (whatever value is set in the settings file is defaulted if this varable is not present)
/// </summary>
public partial class AutoshipProductList : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["wid"] != null) WebID = Convert.ToInt32(Request.QueryString["wid"]);
        else WebID = Autoship.Configuration.WebID;

        if (Request.QueryString["wcid"] != null) WebCategoryID = Convert.ToInt32(Request.QueryString["wcid"]);
        else WebCategoryID = Autoship.Configuration.WebCategoryID;

        if (Request.QueryString["page"] != null) CurrentPage = Convert.ToInt32(Request.QueryString["page"]);
        else CurrentPage = 1;

        if (Request.QueryString["pagesize"] != null) CurrentPageSize = Convert.ToInt32(Request.QueryString["pagesize"]);
        else CurrentPageSize = PageSizes[0];

        if (Request.QueryString["sort"] != null) CurrentSortType = Request.QueryString["sort"];
        else CurrentSortType = string.Empty;
    }
    #endregion

    #region Properties
    public int WebID { get; set; }
    public int WebCategoryID { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentPageSize { get; set; }
    public string CurrentSortType { get; set; }

    public int Columns = 4;
    public int[] PageSizes
    {
        get
        {
            return new[] { (Columns * 2), (Columns * 4), (Columns * 6) };
        }
    }
    public Dictionary<string, string> SortTypes
    {
        get
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("pricedesc", Resources.Shopping.Price_HighToLow);
            dictionary.Add("priceasc", Resources.Shopping.Price_LowToHigh);
            dictionary.Add("bvdesc", Resources.Shopping.BV_HighToLow);
            dictionary.Add("bvasc", Resources.Shopping.BV_LowToHigh);
            dictionary.Add("cvdesc", Resources.Shopping.CV_HighToLow);
            dictionary.Add("cvasc", Resources.Shopping.CV_LowToHigh);

            return dictionary;
        }
    }

    protected List<ItemResponse> ExigoItems
    {
        get
        {
            if (_exigoItems == null)
            {
                // Get the items from Exigo
                GetItemsRequest req = new GetItemsRequest();
                req.WarehouseID = Autoship.Configuration.WarehouseID;
                req.CurrencyCode = Autoship.Configuration.CurrencyCode;
                req.PriceType = Autoship.Configuration.PriceTypeID;
                req.LanguageID = Autoship.Configuration.LanguageID;
                req.WebID = WebID;
                req.WebCategoryID = WebCategoryID;
                req.RestrictToWarehouse = true;
                req.ReturnLongDetail = false;
                GetItemsResponse res = ExigoApiContext.CreateWebServiceContext().GetItems(req);
                var filteredList = res.Items.ToList();


                // Apply the filters accordingly
                if (!string.IsNullOrEmpty(CurrentSortType))
                {
                    switch (CurrentSortType)
                    {
                        case "pricedesc": filteredList = filteredList.OrderByDescending(i => i.Price).ToList(); break;
                        case "priceasc": filteredList = filteredList.OrderBy(i => i.Price).ToList(); break;
                        case "bvdesc": filteredList = filteredList.OrderByDescending(i => i.BusinessVolume).ToList(); break;
                        case "bvasc": filteredList = filteredList.OrderBy(i => i.BusinessVolume).ToList(); break;
                        case "cvdesc": filteredList = filteredList.OrderByDescending(i => i.CommissionableVolume).ToList(); break;
                        case "cvasc": filteredList = filteredList.OrderBy(i => i.CommissionableVolume).ToList(); break;
                    }                        
                }

                _exigoItems = filteredList;
            }

            return _exigoItems;
        }
    }
    private List<ItemResponse> _exigoItems;

    public AutoshipCartManager Autoship
    {
        get
        {
            if (_autoship == null)
            {
                _autoship = new AutoshipCartManager();
            }
            return _autoship;
        }
    }
    private AutoshipCartManager _autoship;
    #endregion

    #region Render
    public void RenderProductListHeader()
    {
        StringBuilder html = new StringBuilder();


        html.AppendLine(@"
            <div id='productlistheader'>
                <div class='searchresults'>
                    <span class='totalitemcount'>" + ExigoItems.Count + @"</span> " + Resources.Shopping.ProductsFound + @"</div>
                <div class='sorttype'>
                    " + Resources.Shopping.SortBy + @":
                    <select id='lstSortType' name='lstSortType' onchange=""__doPostBack('" + Page.UniqueID + @"', 'ChangeSortType|' + $(this).val())"">
                        <option value=''>--- " + Resources.Shopping.Choose + @" ---</option>
        ");


        foreach (var sortType in SortTypes)
        {
            string selected = (sortType.Key == CurrentSortType) ? "selected" : string.Empty;
            html.AppendLine(@"
                <option 
                    value='" + sortType.Key + @"'" + selected + @">" + sortType.Value + "</option>");
        }


        html.AppendLine(@"
                    </select>
                </div>
                <div class='clearfix'>
                </div>
            </div>
        ");


        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderTopPagerBar()
    {
        StringBuilder html = new StringBuilder();


        int firstItemIndex = (CurrentPageSize * (CurrentPage - 1) + 1);
        int lastItemIndex = (ExigoItems.Skip((CurrentPage - 1) * CurrentPageSize).Take(CurrentPageSize).Count());
        if (lastItemIndex < CurrentPageSize) lastItemIndex = ((CurrentPage - 1) * CurrentPageSize) + lastItemIndex;


        html.AppendLine(@"
            <div class='pagerbar top'>
                <div class='itemsxofy'>
                    " + string.Format(Resources.Shopping.ItemsOf, firstItemIndex, lastItemIndex) + " " + ExigoItems.Count + @"</div>
                <div class='pagesize'>
                    " + Resources.Shopping.Show + @" <select id='lstPageSize' class='input-small' name='lstPageSize' onchange=""__doPostBack('" + Page.UniqueID + @"', 'ChangePageSize|' + $(this).val())"">
        ");


        foreach (var pageSize in PageSizes)
        {
            string selected = (pageSize == CurrentPageSize) ? "selected" : string.Empty;
            html.AppendLine(@"
                <option 
                    value='" + pageSize + @"' 
                    " + selected + @">" + pageSize + "</option>");
        }


        html.AppendLine(@"
                    </select> " + Resources.Shopping.PerPage + @"
                </div>
                " + ProductListPager() + @"
                <div class='clearfix'></div>
            </div>
        ");


        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderBottomPagerBar()
    {
        StringBuilder html = new StringBuilder();


        html.AppendLine(@"
            <div class='pargerbar bottom'>
                " + ProductListPager() + @"
                <div class='clearfix'>
                </div>
            </div>
        ");


        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    public void RenderProductListItems()
    {
        StringBuilder html = new StringBuilder();
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);


        html.AppendLine(@"
            <div id='productlistitems'>
                <table class='producttable' cellpadding='0' cellspacing='0'>
        ");


        if (ExigoItems.Count > 0)
        {
            int renderedItemCounter = 0;
            int currentTableCellCounter = 0;
            foreach (var item in ExigoItems.Skip((CurrentPage - 1) * CurrentPageSize))
            {
                if (renderedItemCounter < CurrentPageSize)
                {
                    // Start a new table row if we need to
                    if (currentTableCellCounter % Columns == 0) html.AppendLine("<tr>");


                    // Determine the width of each column - this will make them appear with even widths
                    html.AppendLine(string.Format("<td class='productwrapper' style='width: {0:0}%'>", 100 / Columns));


                    // Render the products
                    html.AppendLine(ProductCell(item, renderedItemCounter));


                    // End the table cell
                    html.AppendLine("</td>");


                    // Increment our counters
                    renderedItemCounter++;
                    currentTableCellCounter++;


                    // End the current table row and restart the table cell counterif applicable
                    if (currentTableCellCounter % Columns == 0)
                    {
                        html.AppendLine("</tr>");
                        currentTableCellCounter = 0;
                    }


                    // If we are done with all the products, ensure that we don't have any unfinished table rows.
                    if (renderedItemCounter >= CurrentPageSize)
                    {
                        if (currentTableCellCounter % Columns != 0)
                        {
                            // Add a table cell for each missing product in the last row
                            for (int x = currentTableCellCounter % Columns; x < Columns; x++) // Add a table cell for each missing product in the last row
                            {
                                html.AppendLine(string.Format("<td class='productwrapper' style='width: {0:0}%'>&nbsp;</td>", 100 / Columns));
                            }


                            // Finally, close the row
                            html.AppendLine("</tr>");
                        }


                        // Break the foreach loop
                        break;
                    }
                }
                else
                {
                    // Increment our counters
                    renderedItemCounter++;
                    currentTableCellCounter++;
                }
            }


            // If we are done with all the products, ensure that we don't have any unfinished table rows.
            if (currentTableCellCounter % Columns != 0)
            {
                // Add a table cell for each missing product in the last row
                for (int x = currentTableCellCounter % Columns; x < Columns; x++) // Add a table cell for each missing product in the last row
                {
                    html.AppendLine(string.Format("<td class='productwrapper' style='width: {0:0}%'>&nbsp;</td>", 100 / Columns));
                }


                // Finally, close the row
                html.AppendLine("</tr>");
            }



        }
        else
        {
            html.AppendLine(@"
                <tr>
                    <td class='productwrapper'>" + Resources.Shopping.NoItemsToDisplay + @"</td>
                </tr>
            ");
        }

        html.AppendLine(@"
                </table>
            </div>
        ");


        writer.Write(html.ToString());
    }

    private string ProductCell(ItemResponse item, int renderedItemCounter)
    {
        StringBuilder html = new StringBuilder();

        html.AppendLine(@"
            <div class='picture'>
                <a href='" + Autoship.UrlProductDetail + "?item=" + item.ItemCode + @"'>
                    <img src='" + GlobalUtilities.GetProductImagePath(item.SmallPicture) + @"' alt='" + item.Description + @"' title='" + item.Description + @"' /></a></div>
            <div class='detailswrapper'>
                <div class='description'>
                    <a href='" + Autoship.UrlProductDetail + "?item=" + item.ItemCode + @"' title='View Details'>" + item.Description + @"</a></div>
                <div class='price'>
                    " + item.Price.ToString("C") + @"</div>
                <div class='volume'>
                    " + item.BusinessVolume.ToString("N2") + @" " + Resources.Shopping.BV + @"</div>
            </div>
        ");


        if (item.IsGroupMaster)
        {
            html.AppendLine(@"
                <div class='productgroup'>
                    " + Resources.Shopping.ChooseA + " " + item.GroupMembersDescription.ToLower() + @":
                    <br />
                    <select class='input-small' id='" + Autoship.Cart.GetFormFieldID("ItemCode", renderedItemCounter) + @"' name='" + Autoship.Cart.GetFormFieldID("ItemCode", renderedItemCounter) + @"'>");

            foreach (var groupMember in item.GroupMembers)
            {
                html.AppendLine("<option value='" + groupMember.ItemCode + "'>" + groupMember.MemberDescription + "</option>");
            }


            html.AppendLine("</select></div>");
        }

        html.AppendLine(@"
            <div class='addtocart'>
                <a onclick=""" + Page.ClientScript.GetPostBackClientHyperlink(this, "AddToCart|" + renderedItemCounter) + @""" class='btn btn-success'>" + Resources.Shopping.AddToCart + @"</a>
            </div>");


        if (!item.IsGroupMaster) html.AppendLine(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", Autoship.Cart.GetFormFieldID("ItemCode", renderedItemCounter), item.ItemCode));
        html.AppendLine(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", Autoship.Cart.GetFormFieldID("Quantity", renderedItemCounter), 1));
        html.AppendLine(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", Autoship.Cart.GetFormFieldID("ParentItemCode", renderedItemCounter), string.Empty));
        html.AppendLine(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", Autoship.Cart.GetFormFieldID("Type", renderedItemCounter), ShoppingCartItemType.Default));


        return html.ToString();
    }
    private string ProductListPager()
    {
        StringBuilder html = new StringBuilder();


        // Get the total number of pages
        int totalPages = ExigoItems.Count / CurrentPageSize;
        if (totalPages == 0) totalPages = 1;

        if (ExigoItems.Count > CurrentPageSize && ExigoItems.Count % CurrentPageSize > 0) totalPages++;

        // If we only have one page (or less), don't show the pager. Let's render a blank space to fill the gap and return the string;
        if (totalPages <= 1)
        {
            html.AppendLine("<div class='pager'>&nbsp;</div>");
            return html.ToString();
        }


        // Define the local variables
        int maxPagesToDisplay = 5;
        int maxPagesOffset = (maxPagesToDisplay % 2 == 0) ? 1 : 0;
        int firstPageToRender = ((CurrentPage - (maxPagesToDisplay / 2) + maxPagesOffset) >= 1) ? CurrentPage - (maxPagesToDisplay / 2) + maxPagesOffset : 1;
        int lastPageToRender = ((CurrentPage + (maxPagesToDisplay / 2) + maxPagesOffset) <= totalPages) ? CurrentPage + (maxPagesToDisplay / 2) + maxPagesOffset : totalPages;


        // Assemble the pager
        html.AppendLine("<div class='pager'>");


        // Render the with the previous arrow
        if (CurrentPage > 1)
        {
            html.AppendLine(string.Format("<a href='{0}' class='previous'>< {1}</a>",
                NewPageUrl(CurrentPage - 1), Resources.Shopping.Previous));
        }


        // Render the page links
        if (totalPages > (maxPagesToDisplay + 1) && CurrentPage + maxPagesToDisplay >= totalPages)
        {
            for (var i = 0; i < maxPagesToDisplay; i++)
            {
                html.AppendLine(string.Format("<a href='{2}' class='{1}'>{0}</a>",
                    (lastPageToRender - maxPagesToDisplay + 1) + i,
                    ((lastPageToRender - maxPagesToDisplay + 1) + i == CurrentPage) ? "Active" : string.Empty,
                    NewPageUrl((lastPageToRender - maxPagesToDisplay + 1) + i)));
            }
        }
        else
        {
            for (var i = 0; i < maxPagesToDisplay; i++)
            {
                if (firstPageToRender + i > totalPages) break;

                html.AppendLine(string.Format("<a href='{2}' class='{1}'>{0}</a>",
                    firstPageToRender + i,
                    (firstPageToRender + i == CurrentPage) ? "Active" : string.Empty,
                    NewPageUrl(firstPageToRender + i)));
            }
        }

        // Render the next arrow
        if (totalPages > CurrentPage)
        {
            html.AppendLine(string.Format("<a href='{0}' class='next'>{1} ></a>",
                NewPageUrl(CurrentPage + 1), Resources.Shopping.Next));
        }


        // Finish the list
        html.AppendLine("</div>");



        return html.ToString();
    }
    #endregion

    #region Helper Methods
    public string GetBaseUrl()
    {
        string newUrl = Request.Url.AbsolutePath;

        newUrl = newUrl.AppendQueryString("wid");
        newUrl = newUrl.AppendQueryString("wcid");

        return newUrl;
    }
    public string NewPageUrl(int page)
    {
        string newUrl = GetBaseUrl();

        newUrl = newUrl.AppendQueryString("page", page);
        newUrl = newUrl.AppendQueryString("sort");
        newUrl = newUrl.AppendQueryString("pagesize");

        return newUrl;
    }
    public string NewSortUrl(string sortType)
    {
        string newUrl = GetBaseUrl();

        newUrl = newUrl.AppendQueryString("page", 1);
        newUrl = newUrl.AppendQueryString("sort", sortType);
        newUrl = newUrl.AppendQueryString("pagesize");

        return newUrl;
    }
    public string NewPageSize(int pageSize)
    {
        string newUrl = GetBaseUrl();

        newUrl = newUrl.AppendQueryString("page", 1);
        newUrl = newUrl.AppendQueryString("sort");
        newUrl = newUrl.AppendQueryString("pagesize", pageSize);

        return newUrl;
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');

        switch (args[0])
        {
            case "AddToCart":
                Autoship.Cart.Items.Add(new ShoppingCartItem(Autoship.Cart, Convert.ToInt32(args[1])));
                Autoship.Cart.Save();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
                break;
            case "ChangeSortType":
                Response.Redirect(NewSortUrl(args[1]));
                break;
            case "ChangePageSize":
                Response.Redirect(NewPageSize(Convert.ToInt32(args[1])));
                break;
        }
    }
    #endregion
}