using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.WebService;

public partial class AutoshipCartPage : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
            
    }
    #endregion

    #region Properties
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

    public List<AutoshipCartItemViewModel> CartItems
    {
        get
        {
            if (_cartItems == null)
            {
                var response = ExigoApiContext.CreateWebServiceContext().GetItems(new GetItemsRequest
                {
                    WarehouseID = Autoship.Configuration.WarehouseID,
                    PriceType = Autoship.Configuration.PriceTypeID,
                    CurrencyCode = Autoship.Configuration.CurrencyCode,
                    LanguageID = Autoship.Configuration.LanguageID,
                    RestrictToWarehouse = true,
                    ReturnLongDetail = false,

                    ItemCodes = Autoship.Cart.Items.Select(i => i.ItemCode).Distinct().ToArray(),
                });


                _cartItems = new List<AutoshipCartItemViewModel>();
                foreach (var itemResponse in response.Items)
                {
                    var itemViewModel = new AutoshipCartItemViewModel(itemResponse, Autoship.Cart, ShoppingCartItemType.Default);
                    _cartItems.Add(itemViewModel);

                }
            }
            return _cartItems;
        }
    }
    private List<AutoshipCartItemViewModel> _cartItems;

    public CalculateOrderResponse CalculatedOrder
    {
        get
        {
            if (_calculatedOrder == null)
            {
                _calculatedOrder = ExigoApiContext.CreateWebServiceContext().CalculateOrder(new CalculateOrderRequest
                {
                    WarehouseID = Autoship.Configuration.WarehouseID,
                    PriceType = Autoship.Configuration.PriceTypeID,
                    CurrencyCode = Autoship.Configuration.CurrencyCode,
                    ReturnShipMethods = false,
                    ShipMethodID = 1,
                    Details = Autoship.Cart.Items.Select(i => new OrderDetailRequest
                    {
                        ItemCode = i.ItemCode,
                        Quantity = i.Quantity,
                        ParentItemCode = i.ParentItemCode
                    }).ToArray(),

                    City = "Dallas",
                    State = "TX",
                    Zip = "75247",
                    Country = "US"
                });
            }
            return _calculatedOrder;
        }
    }
    private CalculateOrderResponse _calculatedOrder;
    #endregion

    #region Models
    public class AutoshipCartItemViewModel
    {
        public AutoshipCartItemViewModel(ItemResponse itemResponse, ShoppingCart cart, ShoppingCartItemType type)
        {
            this.ItemCode = itemResponse.ItemCode;
            this.Description = itemResponse.Description;
            this.PriceEach = itemResponse.Price;
            this.Type = type;
            this.BV = itemResponse.BusinessVolume;
            this.CV = itemResponse.CommissionableVolume;
            this.Image = GlobalSettings.Shopping.ProductImagePath + itemResponse.TinyPicture;

            this.ParentItemCode = cart.Items
                        .Where(i => i.ItemCode == itemResponse.ItemCode && i.Type == type)
                        .Select(i => i.ParentItemCode).FirstOrDefault();

            var quantity = cart.Items
                        .Where(i => i.ItemCode == itemResponse.ItemCode && i.Type == type)
                        .Select(i => i.Quantity).FirstOrDefault();

            this.Quantity = quantity;
            this.PriceTotal = itemResponse.Price *  quantity;

        }

        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal PriceEach { get; set; }
        public decimal PriceTotal { get; set; }
        public ShoppingCartItemType Type { get; set; }
        public decimal BV { get; set; }
        public decimal CV { get; set; }
        public string Image { get; set; }
        public decimal Quantity { get; set; }
        public string ParentItemCode { get; set; }
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] arguments = eventArgument.Split('|');

        switch (arguments[0])
        {
            case "RemoveFromCart":
                Autoship.Cart.Items.Remove(arguments[1]);
                Autoship.Cart.Save();

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "UpdateCart":
                Autoship.Cart.AddItemsToBasket(true);

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "Checkout":
                Autoship.Cart.AddItemsToBasket(true);

                if (Autoship.Cart.AutoshipID > 0)
                {
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                else
                {
                    if (Autoship.PropertyBag.ReferredByEndOfCheckout)
                    {
                        Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                    }
                    else
                    {
                        Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Details));
                    }
                }
                break;
        }
    }
    #endregion
}