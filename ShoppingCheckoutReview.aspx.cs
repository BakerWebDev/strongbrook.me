using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class ShoppingCheckoutReview : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        cmdPlaceOrder.Text = Resources.Shopping.PlaceYourOrder;

        Shopping.PropertyBag.ReferredByEndOfCheckout = true;
        Shopping.PropertyBag.Save();


        // If there are no items in the cart, redirect them to the cart page.
        if (Shopping.Cart.Items.Count == 0)
        {
            Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Cart));
            return;
        }


        if (!IsPostBack)
        {
            PopulateAvailableShippingMethods_Load();

            PopulatePropertyBagValues_Load();
        }
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

                    ItemCodes = Shopping.Cart.Items.Where(i => i.Type == ShoppingCartItemType.Default).Select(i => i.ItemCode).Distinct().ToArray(),
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
                    ReturnShipMethods = true,
                    ShipMethodID = Shopping.PropertyBag.ShipMethodID,
                    Details = Shopping.Cart.Items.Select(i => new OrderDetailRequest
                    {
                        ItemCode = i.ItemCode,
                        Quantity = i.Quantity,
                        ParentItemCode = i.ParentItemCode
                    }).ToArray(),

                    City = Shopping.PropertyBag.ShippingCity,
                    State = Shopping.PropertyBag.ShippingState,
                    Zip = Shopping.PropertyBag.ShippingZip,
                    Country = Shopping.PropertyBag.ShippingCountry
                });
            }
            return _calculatedOrder;
        }
    }
    private CalculateOrderResponse _calculatedOrder;

    private string NewCreditCardPaymentToken
    {
        get
        {
            if (!string.IsNullOrEmpty(_newCreditCardPaymentToken))
            {
                var paymentApi = new ExigoPaymentApi();
                _newCreditCardPaymentToken = paymentApi.FetchCreditCardToken
                (
                    Shopping.PropertyBag.CreditCardNameOnCard,
                    Shopping.PropertyBag.CreditCardExpirationDate.Month,
                    Shopping.PropertyBag.CreditCardExpirationDate.Year
                );
            }
            return _newCreditCardPaymentToken;
        }
    }
    private string _newCreditCardPaymentToken;

    private int NewOrderID;
    #endregion

    #region Load Data
    public void PopulatePropertyBagValues_Load()
    {
    }
    #endregion

    #region Order Details
    private OrderDetailRequest[] GetOrderDetails()
    {
        List<OrderDetailRequest> details = new List<OrderDetailRequest>();

        // Add the items from the shopping cart
        Shopping.Cart.Items.Where(c => c.Type == ShoppingCartItemType.Default).ToList().ForEach(item =>
        {
            details.Add(new OrderDetailRequest
            {
                ItemCode        = item.ItemCode,
                Quantity        = item.Quantity,
                ParentItemCode  = item.ParentItemCode                    
            });
        });

        return details.ToArray();
    }
    #endregion

    #region Exigo API Requests
    // Creating an order
    private CreateOrderRequest Request_CreateOrder()
    {
        CreateOrderRequest request = new CreateOrderRequest();

        request.CustomerID = Identity.Current.CustomerID;

        request.CurrencyCode = Shopping.Configuration.CurrencyCode;
        request.WarehouseID = Shopping.Configuration.WarehouseID;
        request.PriceType = Shopping.Configuration.PriceTypeID;
        request.ShipMethodID = Shopping.PropertyBag.ShipMethodID;
        request.OrderDate = DateTime.Now;
        request.OrderStatus = OrderStatusType.Incomplete;
        request.OrderType = Exigo.WebService.OrderType.APIOrder;

        request.FirstName = Shopping.PropertyBag.ShippingFirstName;
        request.LastName = Shopping.PropertyBag.ShippingLastName;
        request.Address1 = Shopping.PropertyBag.ShippingAddress1;
        request.Address2 = Shopping.PropertyBag.ShippingAddress2;
        request.City = Shopping.PropertyBag.ShippingCity;
        request.State = Shopping.PropertyBag.ShippingState;
        request.Zip = Shopping.PropertyBag.ShippingZip;
        request.Country = Shopping.PropertyBag.ShippingCountry;
        request.Phone = Shopping.PropertyBag.Phone;
        request.Email = Shopping.PropertyBag.Email;

        request.Details = GetOrderDetails();

        return request;
    }


    // Handling credit cards
    private SetAccountCreditCardTokenRequest Request_SaveNewCreditCardToAccount()
    {
        SetAccountCreditCardTokenRequest request = new SetAccountCreditCardTokenRequest();

        request.CustomerID = Identity.Current.CustomerID;

        request.CreditCardToken = NewCreditCardPaymentToken;
        request.CreditCardAccountType = AccountCreditCardType.Secondary;
        request.ExpirationMonth = Shopping.PropertyBag.CreditCardExpirationDate.Month;
        request.ExpirationYear = Shopping.PropertyBag.CreditCardExpirationDate.Year;

        request.BillingName = Shopping.PropertyBag.CreditCardNameOnCard;
        request.BillingAddress = Shopping.PropertyBag.CreditCardBillingAddress;
        request.BillingCity = Shopping.PropertyBag.CreditCardBillingCity;
        request.BillingState = Shopping.PropertyBag.CreditCardBillingState;
        request.BillingZip = Shopping.PropertyBag.CreditCardBillingZip;
        request.BillingCountry = Shopping.PropertyBag.CreditCardBillingCountry;

        return request;
    }
    private ChargeCreditCardTokenRequest Request_ChargeNewCreditCard()
    {
        ChargeCreditCardTokenRequest request = new ChargeCreditCardTokenRequest();

        request.CreditCardToken = NewCreditCardPaymentToken;

        request.BillingName = Shopping.PropertyBag.CreditCardNameOnCard;
        request.BillingAddress = Shopping.PropertyBag.CreditCardBillingAddress;
        request.BillingCity = Shopping.PropertyBag.CreditCardBillingCity;
        request.BillingState = Shopping.PropertyBag.CreditCardBillingState;
        request.BillingZip = Shopping.PropertyBag.CreditCardBillingZip;
        request.BillingCountry = Shopping.PropertyBag.CreditCardBillingCountry;

        return request;
    }
    private ChargeCreditCardTokenOnFileRequest Request_ChargeCreditCardOnFile(AccountCreditCardType creditCardType)
    {
        ChargeCreditCardTokenOnFileRequest request = new ChargeCreditCardTokenOnFileRequest();

        request.CreditCardAccountType = creditCardType;

        return request;
    }


    // Handling bank accounts
    private SetAccountCheckingRequest Request_SaveNewBankAccountToAccount()
    {
        SetAccountCheckingRequest request = new SetAccountCheckingRequest();

        request.CustomerID = Identity.Current.CustomerID;

        request.BankName = Shopping.PropertyBag.BankAccountBankName;
        request.NameOnAccount = Shopping.PropertyBag.BankAccountNameOnAccount;
        request.BankAccountType = Shopping.PropertyBag.BankAccountAccountType;
        request.BankAccountNumber = Shopping.PropertyBag.BankAccountAccountNumber;
        request.BankRoutingNumber = Shopping.PropertyBag.BankAccountRoutingNumber;

        request.BillingAddress = Shopping.PropertyBag.BankAccountBankAddress;
        request.BillingCity = Shopping.PropertyBag.BankAccountBankCity;
        request.BillingState = Shopping.PropertyBag.BankAccountBankState;
        request.BillingZip = Shopping.PropertyBag.BankAccountBankZip;
        request.BillingCountry = Shopping.PropertyBag.BankAccountBankCountry;

        return request;
    }
    private DebitBankAccountRequest Request_ChargeNewBankAccount()
    {
        DebitBankAccountRequest request = new DebitBankAccountRequest();

        request.BankName = Shopping.PropertyBag.BankAccountBankName;
        request.NameOnAccount = Shopping.PropertyBag.BankAccountNameOnAccount;
        request.BankAccountType = Shopping.PropertyBag.BankAccountAccountType;
        request.BankAccountNumber = Shopping.PropertyBag.BankAccountAccountNumber;
        request.BankRoutingNumber = Shopping.PropertyBag.BankAccountRoutingNumber;

        request.BillingAddress = Shopping.PropertyBag.BankAccountBankAddress;
        request.BillingCity = Shopping.PropertyBag.BankAccountBankCity;
        request.BillingState = Shopping.PropertyBag.BankAccountBankState;
        request.BillingZip = Shopping.PropertyBag.BankAccountBankZip;
        request.BillingCountry = Shopping.PropertyBag.BankAccountBankCountry;

        return request;
    }
    private DebitBankAccountOnFileRequest Request_ChargeBankAccountOnFile()
    {
        DebitBankAccountOnFileRequest request = new DebitBankAccountOnFileRequest();

        return request;
    }

    #region Obsolete Methods
    [Obsolete("This method saves the credit card WITHOUT using tokenization, which is NOT PCI-Compliant. This method should not be used unless otherwise informed by Exigo.")]
    private SetAccountCreditCardRequest Request_SaveCreditCard_Legacy()
    {
        // DEVELOPER NOTE: This method saves the credit card WITHOUT using tokenization, 
        // which is NOT PCI-Compliant. This method should not be used unless otherwise informed
        // by Exigo.


        var request = new SetAccountCreditCardRequest();

        request.CreditCardAccountType       = AccountCreditCardType.Secondary;
        request.BillingName                 = Shopping.PropertyBag.CreditCardNameOnCard;
        request.CreditCardNumber            = Shopping.PropertyBag.CreditCardNumber;
        request.ExpirationMonth             = Shopping.PropertyBag.CreditCardExpirationDate.Month;
        request.ExpirationYear              = Shopping.PropertyBag.CreditCardExpirationDate.Year;
        request.CvcCode                     = Shopping.PropertyBag.CreditCardCvc;

        request.BillingAddress              = Shopping.PropertyBag.CreditCardBillingAddress;
        request.BillingCity                 = Shopping.PropertyBag.CreditCardBillingCity;
        request.BillingState                = Shopping.PropertyBag.CreditCardBillingState;
        request.BillingZip                  = Shopping.PropertyBag.CreditCardBillingZip;
        request.BillingCountry              = Shopping.PropertyBag.CreditCardBillingCountry;

        return request;
    }

    [Obsolete("This method charges the credit card WITHOUT using tokenization, which is NOT PCI-Compliant. This method should not be used unless otherwise informed by Exigo.")]
    private ChargeCreditCardRequest Request_ChargeCreditCard_Legacy()
    {
        // DEVELOPER NOTE: This method charges the credit card WITHOUT using tokenization, 
        // which is NOT PCI-Compliant. This method should not be used unless otherwise informed
        // by Exigo.


        var request = new ChargeCreditCardRequest();

        request.BillingName                 = Shopping.PropertyBag.CreditCardNameOnCard;
        request.CreditCardNumber            = Shopping.PropertyBag.CreditCardNumber;
        request.ExpirationMonth             = Shopping.PropertyBag.CreditCardExpirationDate.Month;
        request.ExpirationYear              = Shopping.PropertyBag.CreditCardExpirationDate.Year;
        request.CvcCode                     = Shopping.PropertyBag.CreditCardCvc;

        request.BillingAddress              = Shopping.PropertyBag.CreditCardBillingAddress;
        request.BillingCity                 = Shopping.PropertyBag.CreditCardBillingCity;
        request.BillingState                = Shopping.PropertyBag.CreditCardBillingState;
        request.BillingZip                  = Shopping.PropertyBag.CreditCardBillingZip;
        request.BillingCountry              = Shopping.PropertyBag.CreditCardBillingCountry;

        return request;
    }
    #endregion
    #endregion

    #region Exigo API Transaction Requests
    private TransactionalRequest Request_OrderTransaction()
    {
        TransactionalRequest request = new TransactionalRequest();
        List<ApiRequest> details = new List<ApiRequest>();


        // Add the request to create an order
        details.Add(Request_CreateOrder());


        // Add the requests that handle the payment method for the order
        switch (Shopping.PropertyBag.PaymentType)
        {
            case ShoppingCartPropertyBag.PaymentMethodType.NewCreditCard:
                if (!IsTestCreditCard(Shopping.PropertyBag.CreditCardNumber))
                {
                    details.Add(Request_SaveNewCreditCardToAccount());
                    details.Add(Request_ChargeNewCreditCard());
                }
                break;

            case ShoppingCartPropertyBag.PaymentMethodType.PrimaryCreditCard:
                details.Add(Request_ChargeCreditCardOnFile(AccountCreditCardType.Primary));
                break;

            case ShoppingCartPropertyBag.PaymentMethodType.SecondaryCreditCard:
                details.Add(Request_ChargeCreditCardOnFile(AccountCreditCardType.Secondary));
                break;

            case ShoppingCartPropertyBag.PaymentMethodType.NewBankAccount:
                if (!IsTestBankAccountAccountNumber(Shopping.PropertyBag.BankAccountAccountNumber))
                {
                    details.Add(Request_SaveNewBankAccountToAccount());
                    details.Add(Request_ChargeNewBankAccount());
                }
                break;
                
            case ShoppingCartPropertyBag.PaymentMethodType.BankAccountOnFile:
                details.Add(Request_ChargeBankAccountOnFile());
                break;                
        }


        request.TransactionRequests = details.ToArray();
        return request;
    }
    #endregion

    #region Submit Order
    private void SubmitOrderToExigoAPI()
    {
        // Call the Exigo API to process the order transaction and all the requests contained inside.
        var orderResponse = ExigoApiContext.CreateWebServiceContext().ProcessTransaction(Request_OrderTransaction());


        // If successful, parse each APIResponse object and grab the necessary variables.
        if (orderResponse.Result.Status == ResultStatus.Success)
        {
            foreach (var apiResponse in orderResponse.TransactionResponses)
            {
                if (apiResponse is CreateOrderResponse) NewOrderID = ((CreateOrderResponse)apiResponse).OrderID;
            }
        }
    }
    #endregion

    #region Validation Methods
    // Validates the credit card against a Luhn algorithm to ensure the card is valid. This method will return true if the card number is identified as a test credit card number.
    private bool IsCreditCardValid(string cardNumber)
    {
        if (IsTestCreditCard(cardNumber)) return true;

        const string allowed = "0123456789";
        int i;

        StringBuilder cleanNumber = new StringBuilder();
        for (i = 0; i < cardNumber.Length; i++)
        {
            if (allowed.IndexOf(cardNumber.Substring(i, 1)) >= 0)
                cleanNumber.Append(cardNumber.Substring(i, 1));
        }
        if (cleanNumber.Length < 13 || cleanNumber.Length > 16)
            return false;

        for (i = cleanNumber.Length + 1; i <= 16; i++)
            cleanNumber.Insert(0, "0");

        int multiplier, digit, sum, total = 0;
        string number = cleanNumber.ToString();

        for (i = 1; i <= 16; i++)
        {
            multiplier = 1 + (i % 2);
            digit = int.Parse(number.Substring(i - 1, 1));
            sum = digit * multiplier;
            if (sum > 9)
                sum -= 9;
            total += sum;
        }

        return (total % 10 == 0);
    }

    // Return a boolean that reflects whether the credit card equals "9696" or not.
    private bool IsTestCreditCard(string cardNumber)
    {
        return (cardNumber == "9696");
    }

    // Return a boolean that reflects whether the credit card equals "9696" or not.
    private bool IsTestBankAccountAccountNumber(string bankAccountAccountNumber)
    {
        return (bankAccountAccountNumber == "9696");
    }
    #endregion

    #region Populate Data
    public void PopulateAvailableShippingMethods_Load()
    {
        // Populate the available ship methods if we have any.
        if (CalculatedOrder.ShipMethods.Length == 0)
        {
            // Do nothing
        }
        else
        {
            rdoShipMethod.Items.Clear();


            foreach (var shipMethod in CalculatedOrder.ShipMethods
                .Where(c => Shopping.Configuration.AvailableShipMethods.Contains(c.ShipMethodID))
                .OrderBy(s => s.ShippingAmount))
            {
                ListItem newListItem = new ListItem
                {
                    Text = shipMethod.Description + " (" + shipMethod.ShippingAmount.ToString("C") + ")",
                    Value = shipMethod.ShipMethodID.ToString()
                };


                if (Shopping.PropertyBag.ShipMethodID == shipMethod.ShipMethodID)
                {
                    // De-select any previously selected items
                    rdoShipMethod.Items.Cast<ListItem>().Where(i => i.Selected == true).ToList().ForEach(i => i.Selected = false);

                    // Select our new list item
                    newListItem.Selected = true;
                }


                rdoShipMethod.Items.Add(newListItem);
            }


            // Do one final check to see if any ship methods in the radio list are currently selected, and if not, select the least expensive option that isn't $0.00
            var selectedShippingMethodItems = rdoShipMethod.Items.Cast<ListItem>().Where(i => i.Selected == true).ToList();
            if (selectedShippingMethodItems.Count == 0)
            {
                var defaultShippingMethodID = CalculatedOrder.ShipMethods.Where(s => s.ShippingAmount > 0M).OrderBy(s => s.ShippingAmount).FirstOrDefault();
                if (defaultShippingMethodID != null)
                {
                    rdoShipMethod.Items.FindByValue(defaultShippingMethodID.ShipMethodID.ToString()).Selected = true;
                }
            }
        }
    }
    #endregion

    #region View Models
    public class ShoppingCartItemViewModel
    {
        public ShoppingCartItemViewModel(ItemResponse itemResponse, ShoppingCart cart, ShoppingCartItemType type)
        {
            this.ItemCode = itemResponse.ItemCode;
            this.Description = itemResponse.Description;
            this.PriceEach = itemResponse.Price;
            this.BV = itemResponse.BusinessVolume;
            this.CV = itemResponse.CommissionableVolume;
            this.Image = GlobalUtilities.GetProductImagePath(itemResponse.TinyPicture);

            var quantity = cart.Items
                        .Where(i => i.ItemCode == itemResponse.ItemCode && i.Type == type)
                        .Select(i => i.Quantity).FirstOrDefault();

            this.Quantity = quantity;
            this.PriceTotal = itemResponse.Price * quantity;
        }

        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal PriceEach { get; set; }
        public decimal PriceTotal { get; set; }
        public decimal BV { get; set; }
        public decimal CV { get; set; }
        public ShoppingCartItemType Type { get; set; }
        public string Image { get; set; }
        public decimal Quantity { get; set; }
    }    
    #endregion

    #region Event Handlers
    public void PlaceOrder_Click(object sender, EventArgs e)
    {
        try
        {
            // Submit our order to the Exigo API for order creation and payment processing.
            SubmitOrderToExigoAPI();


            // Reset the cart now that we're done with it.
            Shopping.Reset();


            // Redirect to the final page.
            Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Complete) + "?orderid=" + NewOrderID);
        }
        catch (Exception ex)
        {
            ApplicationErrors.ErrorMessage += ex.Message;
        }
    }

    public void ChangeShippingMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        RaisePostBackEvent("ChangeShippingMethod");
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');

        switch (args[0])
        {
            case "ChangeShippingMethod":
                Shopping.PropertyBag.ShipMethodID = Convert.ToInt32(rdoShipMethod.SelectedValue);
                Shopping.PropertyBag.Save();

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "ChangeShippingAddress":
                Shopping.PropertyBag.ReferredByEndOfCheckout = true;
                Shopping.PropertyBag.Save();
                Response.Redirect(Shopping.GetStepUrl(ShoppingStep.ShippingAddress));
                break;

            case "ChangePayment":
                Shopping.PropertyBag.ReferredByEndOfCheckout = true;
                Shopping.PropertyBag.Save();
                Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Payment));
                break;

            case "ChangeItems":
                Shopping.PropertyBag.ReferredByEndOfCheckout = true;
                Shopping.PropertyBag.Save();
                Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Cart));
                break;


            default:
                throw new Exception("RaisePostBackEvent argument '" + args[0] + "' is not defined.");
        }
    }
    #endregion
}
