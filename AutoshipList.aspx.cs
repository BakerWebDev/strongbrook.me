using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebControls;
using Exigo.WebService;

public partial class AutoshipList : Page, IPostBackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    #region Properties
    public List<AutoOrderResponse> Autoships
    {
        get
        {
            if (_autoships == null)
            {
                _autoships = FetchAutoships();
            }
            return _autoships;
        }
    }
    private List<AutoOrderResponse> _autoships;

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

    #region Fetch Data
    public List<AutoOrderResponse> FetchAutoships()
    {
        return ExigoApiContext.CreateWebServiceContext().GetAutoOrders(new GetAutoOrdersRequest
        {
            CustomerID = Identity.Current.CustomerID,
            AutoOrderStatus = AutoOrderStatusType.Active
        }).AutoOrders
        .Where(c => Autoship.AutoshipSettings.AvailableFrequencyTypes.Contains(c.Frequency))
        .ToList();
    }
    #endregion

    #region Render
    public void RenderAutoshipList()
    {
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);


        // First, get the primary and secondary 
        var customerPaymentTypes = ExigoApiContext.CreateWebServiceContext().GetCustomerBilling(new GetCustomerBillingRequest
        {
            CustomerID = Identity.Current.CustomerID
        });


        StringBuilder html = new StringBuilder();

        var currentFrequency = string.Empty;
        var autoships = Autoships.OrderBy(c => c.Frequency);


        if (autoships.Count() == 0)
        {
            writer.Write(@"                
                    <p>" + Resources.Shopping.NoAutoshipMessage + @"</p>
                    <p><a href=""{0}"" class='btn btn-success'>" + Resources.Shopping.CreateMyAutoship + @"</a></p>
                ", Page.ClientScript.GetPostBackClientHyperlink(this, "NewAutoship"));
            return;
        }


        foreach(var autoship in autoships)
        {
            string customFrequencyDescription = string.Empty;
            switch(autoship.Frequency)
            {
                case Exigo.WebService.FrequencyType.BiMonthly:          customFrequencyDescription = Resources.Shopping.BiMonthly; break;
                case Exigo.WebService.FrequencyType.BiWeekly:           customFrequencyDescription = Resources.Shopping.BiWeekly; break;
                case Exigo.WebService.FrequencyType.EveryFourWeeks:     customFrequencyDescription = Resources.Shopping.TwentyEightDay; break;
                case Exigo.WebService.FrequencyType.Monthly:            customFrequencyDescription = Resources.Shopping.Monthly; break;
                case Exigo.WebService.FrequencyType.Quarterly:          customFrequencyDescription = Resources.Shopping.Quarterly; break;
                case Exigo.WebService.FrequencyType.SemiYearly:         customFrequencyDescription = Resources.Shopping.BiAnnual; break;
                case Exigo.WebService.FrequencyType.Weekly:             customFrequencyDescription = Resources.Shopping.Weekly; break;
                case Exigo.WebService.FrequencyType.Yearly:             customFrequencyDescription = Resources.Shopping.Annual; break;
                default:                                                customFrequencyDescription = Resources.Shopping.Other; break;
            }

            if(currentFrequency != autoship.Frequency.ToString())
            {
                html.Append("</table>");
                html.Append("<h3>" + string.Format(Resources.Shopping.Autoships_formatted, customFrequencyDescription) + "</h3>");
                html.Append("<table class='table'>");
                html.Append(string.Format(@"
                    <tr>
                        <th>{0}
                        </th>
                        <th>{1}
                        </th>
                        <th>{2}
                        </th>
                        <th class='price'>{3}
                        </th>
                        <th class='price'>{4}
                        </th>
                        <th class='options'>&nbsp;
                        </th>
                    </tr>", Resources.Shopping.Description, 
                          Resources.Shopping.PaymentMethod, 
                          Resources.Shopping.NextRunDate,
                          Resources.Shopping.Total,
                          Resources.Shopping.BV
                          ));

                currentFrequency = autoship.Frequency.ToString();
            }

            var autoshipdescription = autoship.Description;
            if(string.IsNullOrEmpty(autoshipdescription)) autoshipdescription = Resources.Shopping.Autoship + " #" + autoship.AutoOrderID;

            var customPaymentTypeDescription = string.Empty;
            switch(autoship.PaymentType)
            {
                case Exigo.WebService.AutoOrderPaymentType.BankDraft:                   customPaymentTypeDescription = Resources.Shopping.BankDraft; break;
                case Exigo.WebService.AutoOrderPaymentType.CheckingAccount:             customPaymentTypeDescription = string.Format(Resources.Shopping.BankAccountEndingIn_formatted, customerPaymentTypes.BankAccount.BankAccountNumberDisplay); break;
                case Exigo.WebService.AutoOrderPaymentType.PrimaryCreditCard:           customPaymentTypeDescription = string.Format(Resources.Shopping.CreditCardEndingIn_formatted, customerPaymentTypes.PrimaryCreditCard.CreditCardNumberDisplay); break;
                case Exigo.WebService.AutoOrderPaymentType.PrimaryWalletAccount:        customPaymentTypeDescription = string.Format(Resources.Shopping.WalletAccountEndingIn_formatted, customerPaymentTypes.PrimaryWalletAccount.WalletAccountDisplay); break;
                case Exigo.WebService.AutoOrderPaymentType.SecondaryCreditCard:         customPaymentTypeDescription = string.Format(Resources.Shopping.CreditCardEndingIn_formatted, customerPaymentTypes.SecondaryCreditCard.CreditCardNumberDisplay); break;
                case Exigo.WebService.AutoOrderPaymentType.SecondaryWalletAccount:      customPaymentTypeDescription = string.Format(Resources.Shopping.WalletAccountEndingIn_formatted, customerPaymentTypes.SecondaryWallletAccount.WalletAccountDisplay); break;
                case Exigo.WebService.AutoOrderPaymentType.WillSendPayment:             customPaymentTypeDescription = Resources.Shopping.ManualPayments; break;
            }

            var customNextRunDateDescription = (autoship.NextRunDate != new DateTime()) ? Convert.ToDateTime(autoship.NextRunDate).ToString("M/d/yyyy") : "---";


            html.Append(string.Format(@"
                    <tr>
                        <td>
                            {0}
                            <input type='hidden' id='AutoshipDescription{1}' value='{0}' />
                            <input type='hidden' id='AutoshipFrequencyDescription{1}' value='{2}' />                      
                        </td>
                        <td>
                            {3}
                        </td>
                        <td>
                            {4}
                        </td>
                        <td class='price'>
                            {5:C}
                        </td>
                        <td class='price'>
                            {6:N0}
                        </td>
                        <td class='options'>
                            <div class='btn-group'>
                                <a class='btn' href='AutoshipInvoice.aspx?id={1}' target='_blank'>" + Resources.Shopping.View + @"</a>
                                <a class='btn' href='javascript:EditExistingAutoship({1});'>" + Resources.Shopping.Edit + @"</a>
                                <a class='btn' href='javascript:DeleteExistingAutoship({1});'>" + Resources.Shopping.Delete + @"</a>
                            </div>
                        </td>
                    </tr>", autoshipdescription,
                          autoship.AutoOrderID,
                          customFrequencyDescription,
                          customPaymentTypeDescription,
                          customNextRunDateDescription,
                          autoship.Total,
                          autoship.BusinessVolumeTotal));
        }

        html.Append("</table>");
        html.Append("</section>");

        writer.Write(html.ToString());
    }
    #endregion

    #region Deleting Autoships
    public void DeleteAutoship(int autoOrderID)
    {
        ExigoApiContext.CreateWebServiceContext().ChangeAutoOrderStatus(new ChangeAutoOrderStatusRequest
        {
            AutoOrderID = autoOrderID,
            AutoOrderStatus = AutoOrderStatusType.Deleted
        });
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] arguments = eventArgument.Split('|');

        switch (arguments[0])
        {
            case "NewAutoship":
                Autoship.Reset();
                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.ProductList));
                break;

            case "EditAutoship":
                Autoship.Cart.LoadAutoship(Convert.ToInt32(arguments[1]), Identity.Current.CustomerID);
                Autoship.Cart.Save();

                Autoship.PropertyBag.LoadAutoshipIntoPropertyBag(Convert.ToInt32(arguments[1]));
                Autoship.PropertyBag.Save();

                Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                break;

            case "DeleteAutoship":
                DeleteAutoship(Convert.ToInt32(arguments[1]));

                if (Autoship.Cart.AutoshipID == Convert.ToInt32(arguments[1]))
                {
                    Autoship.Reset();
                }

                Response.Redirect(Request.Url.AbsolutePath + "?deleted=" + arguments[1]);
                break;
        }
    }
    #endregion
}
