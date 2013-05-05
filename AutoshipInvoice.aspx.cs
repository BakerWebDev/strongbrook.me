using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class AutoshipInvoice : Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Autoship.AutoOrderID == 0)
        {
            Response.Write(Resources.Shopping.UnableToFindAutoship);
            Response.End();
        }
    }
    #endregion

    #region Models
    public AutoOrderResponse Autoship
    {
        get
        {
            if (_autoship == null)
            {
                var response = ExigoApiContext.CreateWebServiceContext().GetAutoOrders(new GetAutoOrdersRequest
                {
                    CustomerID = Identity.Current.CustomerID,
                    AutoOrderID = Convert.ToInt32(Request.QueryString["id"])
                }).AutoOrders;

                if (response.Length == 1) _autoship = response[0];
                else _autoship = new AutoOrderResponse();
            }
            return _autoship;
        }
    }
    private AutoOrderResponse _autoship;

    public WarehouseResponse Warehouse
    {
        get
        {
            if (_warehouse == null)
            {
                _warehouse = ExigoApiContext.CreateWebServiceContext().GetWarehouses(new GetWarehousesRequest()).Warehouses.ToList().Where(w => w.WarehouseID == Autoship.WarehouseID).FirstOrDefault();
            }
            return _warehouse;
        }
    }
    private WarehouseResponse _warehouse;

    public string ShipMethodDescription
    {
        get
        {
            if (_shipMethodDescription == null)
            {
                var response = ExigoApiContext.CreateWebServiceContext().GetShipMethods(new GetShipMethodsRequest
                {
                    WarehouseID = Autoship.WarehouseID,
                    CurrencyCode = Autoship.CurrencyCode
                });

                if (response.ShipMethods.Where(s => s.ShipMethodID == Autoship.ShipMethodID).Count() > 0)
                    _shipMethodDescription = response.ShipMethods.Where(s => s.ShipMethodID == Autoship.ShipMethodID).FirstOrDefault().Description;
                else 
                    _shipMethodDescription = Autoship.ShipMethodID.ToString();
            }
            return _shipMethodDescription;
        }
    }
    private string _shipMethodDescription;
    #endregion

    #region Render
    public void RenderBillingSummary()
    {
        StringBuilder html = new StringBuilder();
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);


        var response = ExigoApiContext.CreateWebServiceContext().GetCustomerBilling(new GetCustomerBillingRequest
        {
            CustomerID = Identity.Current.CustomerID
        });


        if (Autoship.PaymentType == Exigo.WebService.AutoOrderPaymentType.PrimaryCreditCard)
        {
            html.AppendLine(string.Format(Resources.Shopping.PrimaryCreditCardEndingIn,
                                response.PrimaryCreditCard.CreditCardNumberDisplay,
                                response.PrimaryCreditCard.ExpirationMonth,
                                response.PrimaryCreditCard.ExpirationYear));

            writer.Write(html.ToString());
            return;
        }


        if (Autoship.PaymentType == Exigo.WebService.AutoOrderPaymentType.SecondaryCreditCard)
        {
            html.AppendLine(string.Format(Resources.Shopping.SecondaryCreditCardEndingIn,
                                response.SecondaryCreditCard.CreditCardNumberDisplay,
                                response.SecondaryCreditCard.ExpirationMonth,
                                response.SecondaryCreditCard.ExpirationYear));

            writer.Write(html.ToString());
            return;
        }


        if (Autoship.PaymentType == Exigo.WebService.AutoOrderPaymentType.CheckingAccount)
        {
            html.AppendLine(string.Format(Resources.Shopping.BankAccountEndingIn_formatted,
                                response.BankAccount.BankAccountNumberDisplay));

            writer.Write(html.ToString());
            return;
        }


        if (Autoship.PaymentType == Exigo.WebService.AutoOrderPaymentType.WillSendPayment)
        {
            html.AppendLine(Resources.Shopping.CustomerToSendPayments);

            writer.Write(html.ToString());
            return;
        }


        if (Autoship.PaymentType == Exigo.WebService.AutoOrderPaymentType.BankDraft)
        {
            html.AppendLine(Resources.Shopping.CustomerToSendPaymentsBankDraft);

            writer.Write(html.ToString());
            return;
        }
    }
    #endregion
}
