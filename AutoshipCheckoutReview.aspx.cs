using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class AutoshipCheckoutReview : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        Autoship.PropertyBag.ReferredByEndOfCheckout = true;
        Autoship.PropertyBag.Save();


        cmdPlaceOrder.Text = Resources.Shopping.SaveYourAutoship;

        // If there are no items in the cart, redirect them to the cart page.
        if (Autoship.Cart.Items.Count == 0)
        {
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
            return;
        }


        // If there is no credit card number on the billing account, redirect to the Billing step.
        if (Autoship.PropertyBag.CreditCardNumber.Length == 0)
        {
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Payment));
        }


        if (!IsPostBack)
        {
            PopulateAvailableShippingMethods_Load();

            PopulatePropertyBagValues_Load();
        }
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

    public List<ShoppingCartItemViewModel> CartItems
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


                _cartItems = new List<ShoppingCartItemViewModel>();
                foreach (var itemResponse in response.Items)
                {
                    var itemViewModel = new ShoppingCartItemViewModel(itemResponse, Autoship.Cart);
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
                    WarehouseID = Autoship.Configuration.WarehouseID,
                    PriceType = Autoship.Configuration.PriceTypeID,
                    CurrencyCode = Autoship.Configuration.CurrencyCode,
                    ReturnShipMethods = true,
                    ShipMethodID = Autoship.PropertyBag.ShipMethodID,
                    Details = Autoship.Cart.Items.Select(i => new OrderDetailRequest
                    {
                        ItemCode = i.ItemCode,
                        Quantity = i.Quantity,
                        ParentItemCode = i.ParentItemCode
                    }).ToArray(),

                    City = Autoship.PropertyBag.ShippingCity,
                    State = Autoship.PropertyBag.ShippingState,
                    Zip = Autoship.PropertyBag.ShippingZip,
                    Country = Autoship.PropertyBag.ShippingCountry
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
                    Autoship.PropertyBag.CreditCardNumber,
                    Autoship.PropertyBag.CreditCardExpirationDate.Month,
                    Autoship.PropertyBag.CreditCardExpirationDate.Year
                );
            }
            return _newCreditCardPaymentToken;
        }
    }
    private string _newCreditCardPaymentToken;

    private int NewAutoOrderID;
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
        Autoship.Cart.Items.ForEach(item =>
        {
            details.Add(new OrderDetailRequest
            {
                ItemCode = item.ItemCode,
                Quantity = item.Quantity,
                ParentItemCode = item.ParentItemCode
            });
        });

        return details.ToArray();
    }
    #endregion

    #region Exigo API Requests
    // Creating an order
    private CreateAutoOrderRequest Request_SaveAutoship()
    {
        CreateAutoOrderRequest request = new CreateAutoOrderRequest();

        if (Autoship.PropertyBag.ExistingAutoshipID > 0)
        {
            request.ExistingAutoOrderID = Autoship.PropertyBag.ExistingAutoshipID;
            request.OverwriteExistingAutoOrder = true;
        }

        request.CustomerID = Identity.Current.CustomerID;
        request.CurrencyCode = Autoship.Configuration.CurrencyCode;
        request.WarehouseID = Autoship.Configuration.WarehouseID;
        request.PriceType = Autoship.Configuration.PriceTypeID;
        request.ShipMethodID = Autoship.PropertyBag.ShipMethodID;
        request.Frequency = Autoship.PropertyBag.Frequency;
        request.StartDate = Autoship.PropertyBag.StartDate;
        request.Description = Autoship.PropertyBag.AutoshipDescription;

        switch(Autoship.PropertyBag.PaymentType)
        {
            case AutoshipCartPropertyBag.PaymentMethodType.PrimaryCreditCard: request.PaymentType = Exigo.WebService.AutoOrderPaymentType.PrimaryCreditCard; break;
            case AutoshipCartPropertyBag.PaymentMethodType.SecondaryCreditCard: request.PaymentType = Exigo.WebService.AutoOrderPaymentType.SecondaryCreditCard; break;
            case AutoshipCartPropertyBag.PaymentMethodType.NewCreditCard: 
                if (Autoship.PropertyBag.CreditCardType == AccountCreditCardType.Primary) request.PaymentType = Exigo.WebService.AutoOrderPaymentType.PrimaryCreditCard;
                if (Autoship.PropertyBag.CreditCardType == AccountCreditCardType.Secondary) request.PaymentType = Exigo.WebService.AutoOrderPaymentType.SecondaryCreditCard; 
                break;

            case AutoshipCartPropertyBag.PaymentMethodType.BankAccountOnFile:
            case AutoshipCartPropertyBag.PaymentMethodType.NewBankAccount: request.PaymentType = Exigo.WebService.AutoOrderPaymentType.CheckingAccount; break;

            default: throw new Exception(Autoship.PropertyBag.PaymentType.ToString() + " is an unsupported payment type for autoships at this time.");
        }

        request.FirstName = Autoship.PropertyBag.ShippingFirstName;
        request.LastName = Autoship.PropertyBag.ShippingLastName;
        request.Address1 = Autoship.PropertyBag.ShippingAddress1;
        request.Address2 = Autoship.PropertyBag.ShippingAddress2;
        request.City = Autoship.PropertyBag.ShippingCity;
        request.State = Autoship.PropertyBag.ShippingState;
        request.Zip = Autoship.PropertyBag.ShippingZip;
        request.Country = Autoship.PropertyBag.ShippingCountry;
        request.Phone = Autoship.PropertyBag.Phone;
        request.Email = Autoship.PropertyBag.Email;

        request.Details = GetOrderDetails();

        return request;
    }


    // Handling credit cards
    private SetAccountCreditCardTokenRequest Request_SaveNewCreditCardToAccount(AccountCreditCardType creditCardType)
    {
        SetAccountCreditCardTokenRequest request = new SetAccountCreditCardTokenRequest();

        request.CustomerID = Identity.Current.CustomerID;

        request.CreditCardToken = NewCreditCardPaymentToken;
        request.CreditCardAccountType = creditCardType;
        request.ExpirationMonth = Autoship.PropertyBag.CreditCardExpirationDate.Month;
        request.ExpirationYear = Autoship.PropertyBag.CreditCardExpirationDate.Year;

        request.BillingName = Autoship.PropertyBag.CreditCardNameOnCard;
        request.BillingAddress = Autoship.PropertyBag.CreditCardBillingAddress;
        request.BillingCity = Autoship.PropertyBag.CreditCardBillingCity;
        request.BillingState = Autoship.PropertyBag.CreditCardBillingState;
        request.BillingZip = Autoship.PropertyBag.CreditCardBillingZip;
        request.BillingCountry = Autoship.PropertyBag.CreditCardBillingCountry;

        return request;
    }


    // Handling bank accounts
    private SetAccountCheckingRequest Request_SaveNewBankAccountToAccount()
    {
        SetAccountCheckingRequest request = new SetAccountCheckingRequest();

        request.CustomerID = Identity.Current.CustomerID;

        request.BankName = Autoship.PropertyBag.BankAccountBankName;
        request.NameOnAccount = Autoship.PropertyBag.BankAccountNameOnAccount;
        request.BankAccountType = Autoship.PropertyBag.BankAccountAccountType;
        request.BankAccountNumber = Autoship.PropertyBag.BankAccountAccountNumber;
        request.BankRoutingNumber = Autoship.PropertyBag.BankAccountRoutingNumber;

        request.BillingAddress = Autoship.PropertyBag.BankAccountBankAddress;
        request.BillingCity = Autoship.PropertyBag.BankAccountBankCity;
        request.BillingState = Autoship.PropertyBag.BankAccountBankState;
        request.BillingZip = Autoship.PropertyBag.BankAccountBankZip;
        request.BillingCountry = Autoship.PropertyBag.BankAccountBankCountry;

        return request;
    }
    #endregion

    #region Exigo API Transaction Requests
    private TransactionalRequest Request_AutoshipTransaction()
    {
        TransactionalRequest request = new TransactionalRequest();
        List<ApiRequest> details = new List<ApiRequest>();


        // Add the request to create an order
        details.Add(Request_SaveAutoship());


        // Add the requests that handle the payment method for the order
        switch (Autoship.PropertyBag.PaymentType)
        {
            case AutoshipCartPropertyBag.PaymentMethodType.NewCreditCard:
                details.Add(Request_SaveNewCreditCardToAccount(Autoship.PropertyBag.CreditCardType));
                break;

            case AutoshipCartPropertyBag.PaymentMethodType.NewBankAccount:
                details.Add(Request_SaveNewBankAccountToAccount());
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
        var orderResponse = ExigoApiContext.CreateWebServiceContext().ProcessTransaction(Request_AutoshipTransaction());


        // If successful, parse each APIResponse object and grab the necessary variables.
        if (orderResponse.Result.Status == ResultStatus.Success)
        {
            foreach (var apiResponse in orderResponse.TransactionResponses)
            {
                if (apiResponse is CreateAutoOrderResponse) NewAutoOrderID = ((CreateAutoOrderResponse)apiResponse).AutoOrderID;
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
                .Where(c => Autoship.Configuration.AvailableShipMethods.Contains(c.ShipMethodID))
                .OrderBy(s => s.ShippingAmount))
            {
                ListItem newListItem = new ListItem
                {
                    Text = shipMethod.Description + " (" + shipMethod.ShippingAmount.ToString("C") + ")",
                    Value = shipMethod.ShipMethodID.ToString()
                };


                if (Autoship.PropertyBag.ShipMethodID == shipMethod.ShipMethodID)
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
        public ShoppingCartItemViewModel(ItemResponse itemResponse, ShoppingCart cart)
        {
            this.ItemCode = itemResponse.ItemCode;
            this.Description = itemResponse.Description;
            this.PriceEach = itemResponse.Price;
            this.BV = itemResponse.BusinessVolume;
            this.CV = itemResponse.CommissionableVolume;
            this.Image = GlobalUtilities.GetProductImagePath(itemResponse.TinyPicture);

            var quantity = cart.Items
                        .Where(i => i.ItemCode == itemResponse.ItemCode)
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
            Autoship.Reset();


            // Redirect to the final page.
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Complete) + "?orderid=" + NewAutoOrderID);
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
                Autoship.PropertyBag.ShipMethodID = Convert.ToInt32(rdoShipMethod.SelectedValue);
                Autoship.PropertyBag.Save();

                Response.Redirect(Request.Url.AbsoluteUri);
                break;

            case "ChangeShippingAddress":
                Autoship.PropertyBag.ReferredByEndOfCheckout = true;
                Autoship.PropertyBag.Save();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.ShippingAddress));
                break;

            case "ChangePayment":
                Autoship.PropertyBag.ReferredByEndOfCheckout = true;
                Autoship.PropertyBag.Save();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Payment));
                break;

            case "ChangeItems":
                Autoship.PropertyBag.ReferredByEndOfCheckout = true;
                Autoship.PropertyBag.Save();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
                break;

            case "ChangeDetails":
                Autoship.PropertyBag.ReferredByEndOfCheckout = true;
                Autoship.PropertyBag.Save();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Details));
                break;


            default:
                throw new Exception("RaisePostBackEvent argument '" + args[0] + "' is not defined.");
        }
    }
    #endregion
}
