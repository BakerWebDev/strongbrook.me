using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Exigo.WebService;

public partial class AutoshipProductDetail : Page, IPostBackEventHandler
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

    public ItemResponse Item
    {
        get
        {
            if (_item == null)
            {
                var request = new GetItemsRequest();
                request.WarehouseID = Autoship.Configuration.WarehouseID;
                request.CurrencyCode = Autoship.Configuration.CurrencyCode;
                request.LanguageID = Autoship.Configuration.LanguageID;
                request.PriceType = Autoship.Configuration.PriceTypeID;
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
                Autoship.Cart.AddItemsToBasket(false);
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
                break;
        }
    }
    #endregion
}