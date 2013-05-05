using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Exigo.WebService;

public partial class ShoppingProductDetail : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ItemCode)) ThrowItemNotFoundException();
    }
    #endregion

    #region Properties
    public string ItemCode
    {
        get
        {
            if (Request.QueryString["item"] != null) return Request.QueryString["item"].ToString();
            else return string.Empty;
        }
    }

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

    public ItemResponse Item
    {
        get
        {
            if (_item == null)
            {
                var request = new GetItemsRequest();
                request.WarehouseID = Shopping.Configuration.WarehouseID;
                request.CurrencyCode = Shopping.Configuration.CurrencyCode;
                request.LanguageID = Shopping.Configuration.LanguageID;
                request.PriceType = Shopping.Configuration.PriceTypeID;
                request.ReturnLongDetail = true;
                request.RestrictToWarehouse = true;
                request.ItemCodes = new string[] { ItemCode };
                var response = ExigoApiContext.CreateWebServiceContext().GetItems(request);

                if (response.Items.Length == 1) _item = response.Items[0];
                else ThrowItemNotFoundException();
            }
            return _item;
        }
    }
    private ItemResponse _item;
    #endregion

    #region Helper Methods
    private void ThrowItemNotFoundException()
    {
        Response.Redirect("ItemNotFound.aspx?item=" + ItemCode);
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case "AddToCart":
                Shopping.Cart.AddItemsToBasket(false);
                Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Cart));
                break;
        }
    }
    #endregion
}