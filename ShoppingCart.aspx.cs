using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.WebService;

public partial class ShoppingCartPage : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
            
    }
    #endregion

    #region Properties
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

    public List<ShoppingCartItemViewModel> CartItems
    {
        get
        {
            if (_cartItems == null)
            {
                var response = ExigoApiContext.CreateWebServiceContext().GetItems(new GetItemsRequest
                {
                    WarehouseID = Shopping.Configuration.WarehouseID,
                    PriceType = Shopping.Configuration.PriceTypeID,
                    CurrencyCode = Shopping.Configuration.CurrencyCode,
                    LanguageID = Shopping.Configuration.LanguageID,
                    RestrictToWarehouse = true,
                    ReturnLongDetail = false,

                    ItemCodes = Shopping.Cart.Items.Select(i => i.ItemCode).Distinct().ToArray(),
                });


                _cartItems = new List<ShoppingCartItemViewModel>();
                foreach (var itemResponse in response.Items)
                {
                    var itemViewModel = new ShoppingCartItemViewModel(itemResponse, Shopping.Cart, ShoppingCartItemType.Default);
                    _cartItems.Add(itemViewModel);
                }
            }
            return _cartItems;
        }
    }
    private List<ShoppingCartItemViewModel> _cartItems;

    public CalculateOrderResponse CalculatedOrder
    {
        get
        {
            if (_calculatedOrder == null)
            {
                _calculatedOrder = ExigoApiContext.CreateWebServiceContext().CalculateOrder(new CalculateOrderRequest
                {
                    WarehouseID = Shopping.Configuration.WarehouseID,
                    PriceType = Shopping.Configuration.PriceTypeID,
                    CurrencyCode = Shopping.Configuration.CurrencyCode,
                    ReturnShipMethods = false,
                    ShipMethodID = 1,
                    Details = Shopping.Cart.Items.Select(i => new OrderDetailRequest
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
    public class ShoppingCartItemViewModel
    {
        public ShoppingCartItemViewModel(ItemResponse itemResponse, ShoppingCart cart, ShoppingCartItemType type)
        {
            this.ItemCode = itemResponse.ItemCode;
            this.Description = itemResponse.Description;
            this.PriceEach = itemResponse.Price;
            this.Type = type;
            this.BV = itemResponse.BusinessVolume;
            this.CV = itemResponse.CommissionableVolume;
            this.Image = GlobalSettings.Shopping.ProductImagePath + itemResponse.SmallPicture;

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
                Shopping.Cart.Items.Remove(arguments[1], (ShoppingCartItemType)Enum.Parse(typeof(ShoppingCartItemType), arguments[2]));
                Shopping.Cart.Save();

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "UpdateCart":
                Shopping.Cart.AddItemsToBasket(true);

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "Checkout":
                Shopping.Cart.AddItemsToBasket(true);

                if (Shopping.PropertyBag.ReferredByEndOfCheckout)
                {
                    Shopping.PropertyBag.ReferredByEndOfCheckout = false;
                    Shopping.PropertyBag.Save();
                    Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Review));
                }
                else
                {
                    Response.Redirect(Shopping.GetStepUrl(ShoppingStep.ShippingAddress));
                }
                break;
        }
    }
    #endregion
}
