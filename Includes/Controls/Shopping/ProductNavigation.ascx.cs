using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Exigo.WebService;

public partial class ShoppingProductNavigation : UserControl
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["wid"] != null) WebID = Convert.ToInt32(Request.QueryString["wid"]);
            else WebID = Shopping.Configuration.WebID;

            WebCategoryID = Shopping.Configuration.WebCategoryID;
        }
    }
    #endregion

    #region Product List Settings
    public int WebID { get; set; }
    public int WebCategoryID { get; set; }
    public bool ReturnLongDetail { get; set; }
    public bool ShowCategoryName { get; set; }
    #endregion

    #region Public Properties (used for ease of reference and auto-cleaning of data)
    protected ItemResponse[] Products
    {
        get
        {
            if (_products == null)
            {
                GetItemsRequest req = new GetItemsRequest();
                req.CurrencyCode = Shopping.Configuration.CurrencyCode;
                req.LanguageID = Shopping.Configuration.LanguageID;
                req.PriceType = Shopping.Configuration.PriceTypeID;
                req.WarehouseID = Shopping.Configuration.WarehouseID;
                req.ReturnLongDetail = false;
                req.WebID = WebID;
                req.WebCategoryID = WebCategoryID;
                GetItemsResponse res = ExigoApiContext.CreateWebServiceContext().GetItems(req);

                _products = res.Items;
            }

            return _products;
        }
    }
    private ItemResponse[] _products;

    public ShoppingCartManager Shopping
    {
        get
        {
            if (_shopping == null)
            {
                _shopping = new ShoppingCartManager();
            }
            return _shopping;
        }
    }
    private ShoppingCartManager _shopping;
    #endregion

    #region Render
    // Render the Categories and Products Dynamically
    public void RenderProductListNavigation()
    {
        StringBuilder s = new StringBuilder();

        // Define the variables we will need throughout this
        string cat = "";
        int activeCategoryID = (Request.QueryString["wcid"] != null) ? Convert.ToInt32(Request.QueryString["wcid"]) : 0;
        string activeItemCode = (Request.QueryString["ItemCode"] != null) ? Request.QueryString["ItemCode"].ToString() : "";
        var list = Products;

        // Only render stuff if we have the right items
        if (Products.Length > 0)
        {
            foreach (var i in list)
            {
                // Render the category first, if we are dealing with a new category
                if (cat != i.Category)
                {
                    if (cat != "") s.Append("</ul>");
                    else s.Append("<ul>");

                    s.Append(string.Format("<h4>{0}</h4>", i.Category, i.Category.Replace(" ", "")));

                    s.Append("<ul>");

                    // Build the View All link
                    s.Append(string.Format("<li><a href='{0}'>View All</a></li>",
                        Shopping.UrlProductList + "?wid=" + WebID + "&wcid=" + i.CategoryID));

                    cat = i.Category;
                }

                s.Append(string.Format("<li><a href='{0}' rel='{2}' class='{3}'>{1}</a></li>"
                    , Shopping.UrlProductDetail + "?item=" + i.ItemCode
                    , (i.IsGroupMaster) ? i.GroupDescription : i.Description
                    , i.ItemCode
                    , (i.ItemCode == activeItemCode) ? "active" : ""));
            }
        }


        // Write it to the page
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(s.ToString());
    }
    #endregion
}