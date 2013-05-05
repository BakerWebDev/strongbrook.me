using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class AutoshipCheckoutPayment : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        PopulateCardTypes_OnPageLoad();

        // If there are no items in the cart, redirect them to the cart page.
        if (Autoship.Cart.Items.Count == 0)
        {
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
            return;
        }


        if (!IsPostBack)
        {
            PopulateAllCountryRegions((!string.IsNullOrEmpty(Autoship.PropertyBag.ShippingCountry)) ? Autoship.PropertyBag.ShippingCountry : Autoship.Cart.Country);
            PopulateExpirationDateFields_PageLoad();
            PopulatePropertyBagValues_PageLoad();
            PopulateDefaultFields();
        }

        FetchAndPopulatePaymentMethodsOnFile();
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

    public List<CreditCardOnFile> CreditCardsOnFile
    {
        get
        {
            if (_creditCardsOnFile == null)
            {
                _creditCardsOnFile = new List<CreditCardOnFile>();
            }
            return _creditCardsOnFile;
        }
        set { _creditCardsOnFile = value; }
    }
    private List<CreditCardOnFile> _creditCardsOnFile;

    public List<BankAccountOnFile> BankAccountsOnFile
    {
        get
        {
            if (_bankAccountsOnFile == null)
            {
                _bankAccountsOnFile = new List<BankAccountOnFile>();
            }
            return _bankAccountsOnFile;
        }
        set { _bankAccountsOnFile = value; }
    }
    private List<BankAccountOnFile> _bankAccountsOnFile;

    public string CreditCardNameOnCard
    {
        get { return txtCreditCardBillingName.Text.FormatForExigo(); }
        set { txtCreditCardBillingName.Text = value; }
    }
    public string CreditCardNumber
    {
        get { return txtCreditCardNumber.Text.FormatForExigo(ExigoDataFormatType.CreditCardNumber); }
        set { txtCreditCardNumber.Text = value; }
    }
    public int CreditCardExpirationMonth
    {
        get { return Convert.ToInt32(lstCreditCardExpirationMonth.SelectedValue); }
        set { lstCreditCardExpirationMonth.SelectedValue = value.ToString(); }
    }
    public int CreditCardExpirationYear
    {
        get { return Convert.ToInt32(lstCreditCardExpirationYear.SelectedValue); }
        set { lstCreditCardExpirationYear.SelectedValue = value.ToString(); }
    }
    public string CreditCardCvc
    {
        get { return txtCreditCardCvc.Text.FormatForExigo(); }
        set { txtCreditCardCvc.Text = value; }
    }
    public string CreditCardBillingAddress
    {
        get { return txtCreditCardBillingAddress.Text.FormatForExigo(); }
        set { txtCreditCardBillingAddress.Text = value; }
    }
    public string CreditCardBillingCity
    {
        get { return txtCreditCardBillingCity.Text.FormatForExigo(); }
        set { txtCreditCardBillingCity.Text = value; }
    }
    public string CreditCardBillingState
    {
        get { return lstCreditCardBillingState.SelectedValue; }
        set { lstCreditCardBillingState.SelectedValue = value; }
    }
    public string CreditCardBillingZip
    {
        get { return txtCreditCardBillingZip.Text.FormatForExigo(); }
        set { txtCreditCardBillingZip.Text = value; }
    }
    public string CreditCardBillingCountry
    {
        get { return lstCreditCardBillingCountry.SelectedValue; }
        set { lstCreditCardBillingCountry.SelectedValue = value; }
    }
    public AccountCreditCardType SaveNewCardAs
    {
        get { return (AccountCreditCardType)Enum.Parse(typeof(AccountCreditCardType), lstSaveCardAs.SelectedValue); }
        set { lstSaveCardAs.SelectedValue = ((int)value).ToString(); }
    }


    public string BankAccountNameOnAccount
    {
        get { return txtBankAccountNameOnAccount.Text.FormatForExigo(); }
        set { txtBankAccountNameOnAccount.Text = value; }
    }
    public string BankAccountAccountNumber
    {
        get { return txtBankAccountAccountNumber.Text.FormatForExigo(ExigoDataFormatType.BankAccountNumber); }
        set { txtBankAccountAccountNumber.Text = value; }
    }
    public string BankAccountConfirmAccountNumber
    {
        get { return txtBankAccountConfirmAccountNumber.Text.FormatForExigo(ExigoDataFormatType.BankAccountNumber); }
        set { txtBankAccountConfirmAccountNumber.Text = value; }
    }
    public string BankAccountRoutingNumber
    {
        get { return txtBankAccountRoutingNumber.Text.FormatForExigo(ExigoDataFormatType.BankRoutingNumber); }
        set { txtBankAccountRoutingNumber.Text = value; }
    }
    public BankAccountType BankAccountAccountType
    {
        get { return (BankAccountType)Enum.Parse(typeof(BankAccountType), lstBankAccountAccountType.SelectedValue); }
        set { lstBankAccountAccountType.SelectedValue = value.ToString(); }
    }
    public string BankAccountBankName
    {
        get { return txtBankAccountBankName.Text.FormatForExigo(); }
        set { txtBankAccountBankName.Text = value; }
    }
    public string BankAccountBankAddress
    {
        get { return txtBankAccountBankAddress.Text.FormatForExigo(); }
        set { txtBankAccountBankAddress.Text = value; }
    }
    public string BankAccountBankCity
    {
        get { return txtBankAccountBankCity.Text.FormatForExigo(); }
        set { txtBankAccountBankCity.Text = value; }
    }
    public string BankAccountBankState
    {
        get { return lstBankAccountBankState.SelectedValue; }
        set { lstBankAccountBankState.SelectedValue = value; }
    }
    public string BankAccountBankZip
    {
        get { return txtBankAccountBankZip.Text.FormatForExigo(); }
        set { txtBankAccountBankZip.Text = value; }
    }
    public string BankAccountBankCountry
    {
        get { return lstBankAccountBankCountry.SelectedValue; }
        set { lstBankAccountBankCountry.SelectedValue = value; }
    }
    #endregion

    #region Load Data
    public void PopulatePropertyBagValues_PageLoad()
    {
        if (Autoship.PropertyBag.PaymentType == AutoshipCartPropertyBag.PaymentMethodType.NewCreditCard)
        {
            CreditCardNameOnCard = Autoship.PropertyBag.CreditCardNameOnCard;
            CreditCardNumber = Autoship.PropertyBag.CreditCardNumber;
            CreditCardExpirationMonth = Autoship.PropertyBag.CreditCardExpirationDate.Month;
            CreditCardExpirationYear = Autoship.PropertyBag.CreditCardExpirationDate.Year;

            CreditCardBillingAddress = Autoship.PropertyBag.CreditCardBillingAddress;
            CreditCardBillingCity = Autoship.PropertyBag.CreditCardBillingCity;
            CreditCardBillingState = Autoship.PropertyBag.CreditCardBillingState;
            CreditCardBillingZip = Autoship.PropertyBag.CreditCardBillingZip;
            CreditCardBillingCountry = Autoship.PropertyBag.CreditCardBillingCountry;
        }

        if (Autoship.PropertyBag.PaymentType == AutoshipCartPropertyBag.PaymentMethodType.NewBankAccount)
        {
            BankAccountNameOnAccount = Autoship.PropertyBag.BankAccountNameOnAccount;
            BankAccountAccountNumber = Autoship.PropertyBag.BankAccountAccountNumber;
            BankAccountRoutingNumber = Autoship.PropertyBag.BankAccountRoutingNumber;
            BankAccountBankName = Autoship.PropertyBag.BankAccountBankName;

            BankAccountBankAddress = Autoship.PropertyBag.BankAccountBankAddress;
            BankAccountBankCity = Autoship.PropertyBag.BankAccountBankCity;
            BankAccountBankState = Autoship.PropertyBag.BankAccountBankState;
            BankAccountBankZip = Autoship.PropertyBag.BankAccountBankZip;
            BankAccountBankCountry = Autoship.PropertyBag.BankAccountBankCountry;
        }
    }

    public void PopulateCardTypes_OnPageLoad()
    {
        lstSaveCardAs.Items.Clear();

        lstSaveCardAs.Items.Add(new ListItem { Value = "0", Text = Resources.Shopping.Primary });
        lstSaveCardAs.Items.Add(new ListItem { Value = "1", Text = Resources.Shopping.Secondary });


        lstBankAccountAccountType.Items.Clear();

        lstBankAccountAccountType.Items.Add(new ListItem { Value = "CheckingPersonal", Text = Resources.Shopping.PersonalCheckingAccount });
        lstBankAccountAccountType.Items.Add(new ListItem { Value = "CheckingBusiness", Text = Resources.Shopping.BusinessCheckingAccount });
    }
    #endregion

    #region Save Data
    public void SaveCardDataToPropertyBag(string paymentType)
    {
        // Save the payment
        if (paymentType == "New")
        {
            Autoship.PropertyBag.PaymentType = AutoshipCartPropertyBag.PaymentMethodType.NewCreditCard;
            Autoship.PropertyBag.CreditCardType = SaveNewCardAs;

            Autoship.PropertyBag.CreditCardNameOnCard = CreditCardNameOnCard;
            Autoship.PropertyBag.CreditCardNumber = CreditCardNumber;
            Autoship.PropertyBag.CreditCardExpirationDate = new DateTime(CreditCardExpirationYear, CreditCardExpirationMonth, 1);
            Autoship.PropertyBag.CreditCardCvc = CreditCardCvc;
            Autoship.PropertyBag.CreditCardBillingAddress = CreditCardBillingAddress;
            Autoship.PropertyBag.CreditCardBillingCity = CreditCardBillingCity;
            Autoship.PropertyBag.CreditCardBillingState = CreditCardBillingState;
            Autoship.PropertyBag.CreditCardBillingZip = CreditCardBillingZip;
            Autoship.PropertyBag.CreditCardBillingCountry = CreditCardBillingCountry;
        }
        else
        {
            var parsedCardTypeEnum = (AccountCreditCardType)Enum.Parse(typeof(AccountCreditCardType), paymentType);
            if (parsedCardTypeEnum == AccountCreditCardType.Primary)
            {
                Autoship.PropertyBag.PaymentType = AutoshipCartPropertyBag.PaymentMethodType.PrimaryCreditCard;
                Autoship.PropertyBag.CreditCardType = AccountCreditCardType.Primary;

                var primaryCreditCardOnFile = CreditCardsOnFile.Where(c => c.CreditCardType == AccountCreditCardType.Primary).FirstOrDefault();
                Autoship.PropertyBag.CreditCardNameOnCard = primaryCreditCardOnFile.NameOnCard;
                Autoship.PropertyBag.CreditCardNumber = primaryCreditCardOnFile.CreditCardNumberDisplay;
                Autoship.PropertyBag.CreditCardExpirationDate = primaryCreditCardOnFile.ExpirationDate;
                Autoship.PropertyBag.CreditCardCvc = string.Empty;
                Autoship.PropertyBag.CreditCardBillingAddress = primaryCreditCardOnFile.BillingAddress;
                Autoship.PropertyBag.CreditCardBillingCity = primaryCreditCardOnFile.BillingCity;
                Autoship.PropertyBag.CreditCardBillingState = primaryCreditCardOnFile.BillingState;
                Autoship.PropertyBag.CreditCardBillingZip = primaryCreditCardOnFile.BillingZip;
                Autoship.PropertyBag.CreditCardBillingCountry = primaryCreditCardOnFile.BillingCountry;

            }

            if (parsedCardTypeEnum == AccountCreditCardType.Secondary)
            {
                Autoship.PropertyBag.PaymentType = AutoshipCartPropertyBag.PaymentMethodType.SecondaryCreditCard;
                Autoship.PropertyBag.CreditCardType = AccountCreditCardType.Secondary;

                var secondaryCreditCardOnFile = CreditCardsOnFile.Where(c => c.CreditCardType == AccountCreditCardType.Secondary).FirstOrDefault();
                Autoship.PropertyBag.CreditCardNameOnCard = secondaryCreditCardOnFile.NameOnCard;
                Autoship.PropertyBag.CreditCardNumber = secondaryCreditCardOnFile.CreditCardNumberDisplay;
                Autoship.PropertyBag.CreditCardExpirationDate = secondaryCreditCardOnFile.ExpirationDate;
                Autoship.PropertyBag.CreditCardCvc = string.Empty;
                Autoship.PropertyBag.CreditCardBillingAddress = secondaryCreditCardOnFile.BillingAddress;
                Autoship.PropertyBag.CreditCardBillingCity = secondaryCreditCardOnFile.BillingCity;
                Autoship.PropertyBag.CreditCardBillingState = secondaryCreditCardOnFile.BillingState;
                Autoship.PropertyBag.CreditCardBillingZip = secondaryCreditCardOnFile.BillingZip;
                Autoship.PropertyBag.CreditCardBillingCountry = secondaryCreditCardOnFile.BillingCountry;
            }

        }


        Autoship.PropertyBag.Save();
    }
    public void SaveBankAccountDataToPropertyBag(string paymentType)
    {
        // Decide what information needs to be saved
        if (paymentType == "New")
        {
            Autoship.PropertyBag.PaymentType = AutoshipCartPropertyBag.PaymentMethodType.NewBankAccount;

            Autoship.PropertyBag.BankAccountNameOnAccount = BankAccountNameOnAccount;
            Autoship.PropertyBag.BankAccountAccountNumber = BankAccountAccountNumber;
            Autoship.PropertyBag.BankAccountRoutingNumber = BankAccountRoutingNumber;
            Autoship.PropertyBag.BankAccountBankName = BankAccountBankName;
            Autoship.PropertyBag.BankAccountAccountType = BankAccountAccountType;

            Autoship.PropertyBag.BankAccountBankAddress = BankAccountBankAddress;
            Autoship.PropertyBag.BankAccountBankCity = BankAccountBankCity;
            Autoship.PropertyBag.BankAccountBankState = BankAccountBankState;
            Autoship.PropertyBag.BankAccountBankZip = BankAccountBankZip;
            Autoship.PropertyBag.BankAccountBankCountry = BankAccountBankCountry;
        }
        else
        {
            Autoship.PropertyBag.PaymentType = AutoshipCartPropertyBag.PaymentMethodType.BankAccountOnFile;

            var bankAccountOnFile = BankAccountsOnFile.FirstOrDefault();
            Autoship.PropertyBag.BankAccountNameOnAccount = bankAccountOnFile.BillingName;
            Autoship.PropertyBag.BankAccountAccountNumber = bankAccountOnFile.AccountNumberDisplay;
            Autoship.PropertyBag.BankAccountRoutingNumber = bankAccountOnFile.RoutingNumber;
            Autoship.PropertyBag.BankAccountBankName = bankAccountOnFile.BankName;
            Autoship.PropertyBag.BankAccountAccountType = bankAccountOnFile.AccountType;

            Autoship.PropertyBag.BankAccountBankAddress = bankAccountOnFile.BillingAddress;
            Autoship.PropertyBag.BankAccountBankCity = bankAccountOnFile.BillingCity;
            Autoship.PropertyBag.BankAccountBankState = bankAccountOnFile.BillingState;
            Autoship.PropertyBag.BankAccountBankZip = bankAccountOnFile.BillingZip;
            Autoship.PropertyBag.BankAccountBankCountry = bankAccountOnFile.BillingCountry;
        }


        Autoship.PropertyBag.Save();
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');

        switch (args[0])
        {
            case "UseCard":
                SaveCardDataToPropertyBag(args[1]);

                if (Autoship.PropertyBag.ReferredByEndOfCheckout)
                {
                    Autoship.PropertyBag.ReferredByEndOfCheckout = false;
                    Autoship.PropertyBag.Save();
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                else
                {
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                break;

            case "UseBankAccount":
                SaveBankAccountDataToPropertyBag(args[1]);

                if (Autoship.PropertyBag.ReferredByEndOfCheckout)
                {
                    Autoship.PropertyBag.ReferredByEndOfCheckout = false;
                    Autoship.PropertyBag.Save();
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                else
                {
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                break;

            default:
                throw new Exception("RaisePostBackEvent argument '" + args[0] + "' is not defined.");
        }
    }
    #endregion

    #region Render
    public void RenderCreditCardOnFile(AccountCreditCardType creditCardType)
    {
        if (IsCreditCardOnFileValid(creditCardType))
        {
            StringBuilder html = new StringBuilder();

            var creditCardOnFile = CreditCardsOnFile.Where(c => c.CreditCardType == creditCardType).FirstOrDefault();

            html.Append(@"
                <tr>
                    <td class='options'>
                        <a href=""" + Page.ClientScript.GetPostBackClientHyperlink(this, "UseCard|" + creditCardType.ToString()) + @""" class='btn btn-success Next'>
                            " + string.Format(Resources.Shopping.UseMy_Card, creditCardType.ToString().ToLower()) + @"</a>
                    </td>
                    <td class='description'>
                        <span class='producttitle'>"
                              + Resources.Shopping.CreditDebitEndingIn + " " + creditCardOnFile.CreditCardNumberDisplay.Replace("*", "") + @"</span>
                    </td>
                    <td class='nameoncard'>
                        " + creditCardOnFile.NameOnCard + @"
                    </td>
                    <td class='expirationdate'>
                        " + creditCardOnFile.ExpirationDate.ToString("M/yyyy") + @"
                    </td>
                </tr>
            ");


            HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
            writer.Write(html.ToString());
        }
    }
    public void RenderBankAccountOnFile(BankAccountType bankAccountType)
    {
        if (IsBankAccountOnFileValid(bankAccountType))
        {
            StringBuilder html = new StringBuilder();

            var bankAccountOnFile = BankAccountsOnFile.Where(b => b.AccountType == bankAccountType).FirstOrDefault();

            html.Append(@"
                <tr>
                    <td class='options'>
                        <a href=""" + Page.ClientScript.GetPostBackClientHyperlink(this, "UseBankAccount|" + bankAccountType.ToString()) + @""" class='btn btn-success Next'>
                        " + Resources.Shopping.UseThisCheckingAccount + @"</a>
                    </td>
                    <td class='description'>
                        <span class='producttitle'>
                            " + Resources.Shopping.CheckingAccountEndingIn + " " + bankAccountOnFile.AccountNumberDisplay.Replace("*", "") + @"</span>
                    </td>
                    <td class='nameoncard'>
                        " + bankAccountOnFile.BillingName + @"
                    </td>
                    <td class='expirationdate'>
                        ---
                    </td>
                </tr>
            ");


            HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
            writer.Write(html.ToString());
        }
    }
    #endregion

    #region Data Fetching
    public void FetchAndPopulatePaymentMethodsOnFile()
    {
        // Get the data
        var paymentData = ExigoApiContext.CreateWebServiceContext().GetCustomerBilling(new GetCustomerBillingRequest
        {
            CustomerID = Identity.Current.CustomerID
        });


        // Add the primary card
        var primaryCard = paymentData.PrimaryCreditCard;
        CreditCardsOnFile.Add(new CreditCardOnFile(AccountCreditCardType.Primary)
        {
            CreditCardProvider = (CreditCardOnFile.CreditCardProviderType)primaryCard.CreditCardType,
            NameOnCard = primaryCard.BillingName,
            CreditCardNumberDisplay = primaryCard.CreditCardNumberDisplay,
            ExpirationDate = new DateTime(primaryCard.ExpirationYear, primaryCard.ExpirationMonth, 1),

            BillingAddress = primaryCard.BillingAddress,
            BillingCity = primaryCard.BillingCity,
            BillingState = primaryCard.BillingState,
            BillingZip = primaryCard.BillingZip,
            BillingCountry = primaryCard.BillingCountry
        });


        // Add the secondardy card
        var secondaryCard = paymentData.SecondaryCreditCard;
        CreditCardsOnFile.Add(new CreditCardOnFile(AccountCreditCardType.Secondary)
        {
            CreditCardProvider = (CreditCardOnFile.CreditCardProviderType)secondaryCard.CreditCardType,
            NameOnCard = secondaryCard.BillingName,
            CreditCardNumberDisplay = secondaryCard.CreditCardNumberDisplay,
            ExpirationDate = new DateTime(secondaryCard.ExpirationYear, secondaryCard.ExpirationMonth, 1),

            BillingAddress = secondaryCard.BillingAddress,
            BillingCity = secondaryCard.BillingCity,
            BillingState = secondaryCard.BillingState,
            BillingZip = secondaryCard.BillingZip,
            BillingCountry = secondaryCard.BillingCountry
        });


        // Add the bank account
        var bankAccountOnFile = paymentData.BankAccount;
        BankAccountsOnFile.Add(new BankAccountOnFile(bankAccountOnFile.BankAccountType)
        {
            AccountNumberDisplay = bankAccountOnFile.BankAccountNumberDisplay,
            RoutingNumber = bankAccountOnFile.BankRoutingNumber,
            BankName = bankAccountOnFile.BankName,

            BillingName = bankAccountOnFile.NameOnAccount,
            BillingAddress = bankAccountOnFile.BillingAddress,
            BillingCity = bankAccountOnFile.BillingCity,
            BillingState = bankAccountOnFile.BillingState,
            BillingZip = bankAccountOnFile.BillingZip,
            BillingCountry = bankAccountOnFile.BillingCountry
        });
    }
    #endregion

    #region Helper Methods
    public bool IsCreditCardOnFileValid(AccountCreditCardType creditCardType)
    {
        var creditCardOnFile = CreditCardsOnFile.Where(c => c.CreditCardType == creditCardType).FirstOrDefault();

        if (creditCardOnFile == null) return false;

        TimeSpan expirationDateDifference = creditCardOnFile.ExpirationDate.Subtract(DateTime.Now);

        return (!string.IsNullOrEmpty(creditCardOnFile.CreditCardNumberDisplay)
                && expirationDateDifference.Days > 0
                && !string.IsNullOrEmpty(creditCardOnFile.NameOnCard)
                && !string.IsNullOrEmpty(creditCardOnFile.BillingAddress)
                && !string.IsNullOrEmpty(creditCardOnFile.BillingCity)
                && !string.IsNullOrEmpty(creditCardOnFile.BillingState)
                && !string.IsNullOrEmpty(creditCardOnFile.BillingZip)
                && !string.IsNullOrEmpty(creditCardOnFile.BillingCountry));
    }
    public bool IsBankAccountOnFileValid(BankAccountType bankAccountType)
    {
        var bankAccountOnFile = BankAccountsOnFile.Where(b => b.AccountType == bankAccountType).FirstOrDefault();

        if (bankAccountOnFile == null) return false;

        return (!string.IsNullOrEmpty(bankAccountOnFile.AccountNumberDisplay)
                && !string.IsNullOrEmpty(bankAccountOnFile.RoutingNumber)
                && !string.IsNullOrEmpty(bankAccountOnFile.BankName)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingName)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingAddress)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingCity)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingState)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingZip)
                && !string.IsNullOrEmpty(bankAccountOnFile.BillingCountry));
    }

    public bool HasOneOrMoreValidPaymentMethodsOnFile()
    {
        foreach (var creditCardOnFile in CreditCardsOnFile)
        {
            if (IsCreditCardOnFileValid(creditCardOnFile.CreditCardType))
            {
                return true;
            }
        }

        foreach (var bankAccountOnFile in BankAccountsOnFile)
        {
            if (IsBankAccountOnFileValid(bankAccountOnFile.AccountType))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Country/Region Population
    public Dictionary<DropDownList, DropDownList> CountryRegionDropdownsDictionary
    {
        get
        {
            Dictionary<DropDownList, DropDownList> dict = new Dictionary<DropDownList, DropDownList>();

            dict.Add(lstCreditCardBillingCountry, lstCreditCardBillingState);
            dict.Add(lstBankAccountBankCountry, lstBankAccountBankState);

            return dict;
        }
    }
    public void PopulateRegions(string countryCode, DropDownList regionList)
    {
        // Get the data
        var response = ExigoApiContext.CreateWebServiceContext().GetCountryRegions(new GetCountryRegionsRequest()
        {
            CountryCode = countryCode
        });

        // Clear the items from the region dropdown
        regionList.Items.Clear();

        // Populate the new regions into the dropdown
        foreach (RegionResponse r in response.Regions)
        {
            regionList.Items.Add(new ListItem()
            {
                Value = r.RegionCode,
                Text = r.RegionName
            });
        }
    }
    public void PopulateAllCountryRegions(string optionalCountryCode)
    {
        // Get the data
        var response = ExigoApiContext.CreateWebServiceContext().GetCountryRegions(new GetCountryRegionsRequest()
        {
            CountryCode = (!string.IsNullOrEmpty(optionalCountryCode)) ? optionalCountryCode : ""
        });

        foreach (KeyValuePair<DropDownList, DropDownList> pair in CountryRegionDropdownsDictionary)
        {

            // Clear the items from the dropdowns
            pair.Key.Items.Clear();
            pair.Value.Items.Clear();

            // Populate the new regions into the dropdown
            foreach (CountryResponse r in response.Countries)
            {
                pair.Key.Items.Add(new ListItem()
                {
                    Value = r.CountryCode,
                    Text = r.CountryName
                });
            }
            if (!string.IsNullOrEmpty(optionalCountryCode)) pair.Key.SelectedValue = optionalCountryCode; // Set the default country if we provided one

            // Populate the new regions into the dropdown
            foreach (RegionResponse r in response.Regions)
            {
                pair.Value.Items.Add(new ListItem()
                {
                    Value = r.RegionCode,
                    Text = r.RegionName
                });
            }
        }
    }
    public void PopulateRegions_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is DropDownList)
        {
            DropDownList regionsList;
            if (CountryRegionDropdownsDictionary.TryGetValue(((DropDownList)sender), out regionsList))
            {
                PopulateRegions(((DropDownList)sender).SelectedValue, regionsList);
            }
        }
    }
    #endregion

    #region Misc. Data Population
    private void PopulateExpirationDateFields_PageLoad() // Populates the expiration dates 
    {
        int startyear = DateTime.Now.Year;
        int endyear = DateTime.Now.AddYears(20).Year;


        lstCreditCardExpirationMonth.Items.Clear();
        lstCreditCardExpirationYear.Items.Clear();

        for (int month = 1; month <= 12; month++)
        {
            DateTime date = new DateTime(1900, month, 1);
            string monthDescription = date.ToString("MMMM");
            lstCreditCardExpirationMonth.Items.Add(new ListItem(Convert.ToString(month + " - " + monthDescription), Convert.ToString(month)));
        }

        for (int year = startyear; year <= endyear; year++)
        {
            lstCreditCardExpirationYear.Items.Add(new ListItem(Convert.ToString(year), Convert.ToString(year)));
        }

        lstCreditCardExpirationMonth.SelectedValue = DateTime.Now.Month.ToString();
        lstCreditCardExpirationYear.SelectedValue = DateTime.Now.Year.ToString();
    }

    private void PopulateDefaultFields()
    {
        if (!Autoship.AddressSettings.AllowZipCode)
        {
            var defaultZipCode = Autoship.AddressSettings.DefaultZipCode;
            CreditCardBillingZip = defaultZipCode;
        }
    }
    #endregion

    #region Models
    public class CreditCardOnFile
    {
        public CreditCardOnFile(AccountCreditCardType creditCardType)
        {
            CreditCardType = creditCardType;
        }

        public AccountCreditCardType CreditCardType { get; set; }

        public CreditCardProviderType CreditCardProvider { get; set; }
        public string CreditCardNumberDisplay { get; set; }
        public string NameOnCard { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }

        public enum CreditCardProviderType
        {
            Unknown = 1,
            Visa = 2,
            MasterCard = 3,
            AmericanExpress = 4,
            Discover = 5,
            JCB = 6
        }
    }

    public class BankAccountOnFile
    {
        public BankAccountOnFile(BankAccountType accountType)
        {
            this.AccountType = accountType;
        }

        public string BankName { get; set; }
        public string AccountNumberDisplay { get; set; }
        public string RoutingNumber { get; set; }
        public BankAccountType AccountType { get; set; }

        public string BillingName { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }
    }
    #endregion
}