using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class AutoshipCheckoutShippingAddress : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // If there are no items in the cart, redirect them to the cart page.
        if (Autoship.Cart.Items.Count == 0)
        {
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
            return;
        }


        if (!IsPostBack)
        {
            PopulateAllCountryRegions((!string.IsNullOrEmpty(Autoship.PropertyBag.ShippingCountry)) ? Autoship.PropertyBag.ShippingCountry : Autoship.Cart.Country);
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

    public List<AddressOnFile> AddressesOnFile
    {
        get
        {
            if (_addressesOnFile == null)
            {
                var customerData = ExigoApiContext.CreateODataContext().Customers
                                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                                .Select(c => new Customer
                                {
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    Email = c.Email,
                                    Phone = c.Phone,
                                    MainAddress1 = c.MainAddress1,
                                    MainAddress2 = c.MainAddress2,
                                    MainCity = c.MainCity,
                                    MainState = c.MainState,
                                    MainZip = c.MainZip,
                                    MainCountry = c.MainCountry,
                                    MailAddress1 = c.MailAddress1,
                                    MailAddress2 = c.MailAddress2,
                                    MailCity = c.MailCity,
                                    MailState = c.MailState,
                                    MailZip = c.MailZip,
                                    MailCountry = c.MailCountry,
                                    OtherAddress1 = c.OtherAddress1,
                                    OtherAddress2 = c.OtherAddress2,
                                    OtherCity = c.OtherCity,
                                    OtherState = c.OtherState,
                                    OtherZip = c.OtherZip,
                                    OtherCountry = c.OtherCountry
                                }).FirstOrDefault();


                // Create  the AddressOnFile objects
                _addressesOnFile = new List<AddressOnFile>();

                _addressesOnFile.Add(new AddressOnFile(AutoshipCartPropertyBag.AddressType.Main)
                {
                    FirstName = customerData.FirstName,
                    LastName = customerData.LastName,
                    Email = customerData.Email,
                    Phone = customerData.Phone,
                    Address1 = customerData.MainAddress1,
                    Address2 = customerData.MainAddress2,
                    City = customerData.MainCity,
                    State = customerData.MainState,
                    Zip = customerData.MainZip,
                    Country = customerData.MainCountry
                });

                _addressesOnFile.Add(new AddressOnFile(AutoshipCartPropertyBag.AddressType.Mailing)
                {
                    FirstName = customerData.FirstName,
                    LastName = customerData.LastName,
                    Email = customerData.Email,
                    Phone = customerData.Phone,
                    Address1 = customerData.MailAddress1,
                    Address2 = customerData.MailAddress2,
                    City = customerData.MailCity,
                    State = customerData.MailState,
                    Zip = customerData.MailZip,
                    Country = customerData.MailCountry
                });

                _addressesOnFile.Add(new AddressOnFile(AutoshipCartPropertyBag.AddressType.Other)
                {
                    FirstName = customerData.FirstName,
                    LastName = customerData.LastName,
                    Email = customerData.Email,
                    Phone = customerData.Phone,
                    Address1 = customerData.OtherAddress1,
                    Address2 = customerData.OtherAddress2,
                    City = customerData.OtherCity,
                    State = customerData.OtherState,
                    Zip = customerData.OtherZip,
                    Country = customerData.OtherCountry
                });

            }
            return _addressesOnFile;
        }
    }
    private List<AddressOnFile> _addressesOnFile;

    public string FirstName
    {
        get { return txtFirstName.Text.FormatForExigo(); }
        set { txtFirstName.Text = value; }
    }
    public string LastName
    {
        get { return txtLastName.Text.FormatForExigo(); }
        set { txtLastName.Text = value; }
    }
    public string Email
    {
        get { return txtEmail.Text.FormatForExigo(ExigoDataFormatType.Email); }
        set { txtEmail.Text = value; }
    }
    public string Phone
    {
        get { return txtPhone.Text.FormatForExigo(ExigoDataFormatType.Phone); }
        set { txtPhone.Text = value; }
    }

    public string Address1
    {
        get { return txtAddress1.Text.FormatForExigo(); }
        set { txtAddress1.Text = value; }
    }
    public string Address2
    {
        get { return txtAddress2.Text.FormatForExigo(); }
        set { txtAddress2.Text = value; }
    }
    public string City
    {
        get { return txtCity.Text.FormatForExigo(); }
        set { txtCity.Text = value; }
    }
    public string State
    {
        get { return lstState.SelectedValue; }
        set { lstState.SelectedValue = value; }
    }
    public string Zip
    {
        get { return txtZip.Text.FormatForExigo(); }
        set { txtZip.Text = value; }
    }
    public string Country
    {
        get { return lstCountry.SelectedValue; }
        set { lstCountry.SelectedValue = value; }
    }
    #endregion

    #region Load Data
    public void PopulatePropertyBagValues_Load()
    {
        if (Autoship.PropertyBag.ShippingAddressType == AutoshipCartPropertyBag.AddressType.New)
        {
            FirstName = Autoship.PropertyBag.ShippingFirstName;
            LastName = Autoship.PropertyBag.ShippingLastName;
            Email = Autoship.PropertyBag.Email;
            Phone = Autoship.PropertyBag.Phone;

            Address1 = Autoship.PropertyBag.ShippingAddress1;
            Address2 = Autoship.PropertyBag.ShippingAddress2;
            City = Autoship.PropertyBag.ShippingCity;
            State = Autoship.PropertyBag.ShippingState;
            Zip = Autoship.PropertyBag.ShippingZip;
            Country = Autoship.PropertyBag.ShippingCountry;
        }
    }
    #endregion

    #region Save Data
    public void SaveDataToPropertyBag(string addressType)
    {
        // Save the address type
        var parsedAddressTypeEnum = (AutoshipCartPropertyBag.AddressType)Enum.Parse(typeof(AutoshipCartPropertyBag.AddressType), addressType);
        Autoship.PropertyBag.ShippingAddressType = parsedAddressTypeEnum;


        // Decide what information needs to be saved
        if (parsedAddressTypeEnum == AutoshipCartPropertyBag.AddressType.New)
        {
            Autoship.PropertyBag.ShippingFirstName = FirstName;
            Autoship.PropertyBag.ShippingLastName = LastName;
            Autoship.PropertyBag.Email = Email;
            Autoship.PropertyBag.Phone = Phone;

            Autoship.PropertyBag.ShippingAddress1 = Address1;
            Autoship.PropertyBag.ShippingAddress2 = Address2;
            Autoship.PropertyBag.ShippingCity = City;
            Autoship.PropertyBag.ShippingState = State;
            Autoship.PropertyBag.ShippingZip = Zip;
            Autoship.PropertyBag.ShippingCountry = Country;
        }
        else 
        {
            var addressOnFile = AddressesOnFile.Where(a => a.AddressType == parsedAddressTypeEnum).FirstOrDefault();

                
            // Now, save the correct address
            Autoship.PropertyBag.ShippingFirstName = addressOnFile.FirstName;
            Autoship.PropertyBag.ShippingLastName = addressOnFile.LastName;
            Autoship.PropertyBag.Email = addressOnFile.Email;
            Autoship.PropertyBag.Phone = addressOnFile.Phone;

            Autoship.PropertyBag.ShippingAddress1 = addressOnFile.Address1;
            Autoship.PropertyBag.ShippingAddress2 = addressOnFile.Address2;
            Autoship.PropertyBag.ShippingCity = addressOnFile.City;
            Autoship.PropertyBag.ShippingState = addressOnFile.State;
            Autoship.PropertyBag.ShippingZip = addressOnFile.Zip;
            Autoship.PropertyBag.ShippingCountry = addressOnFile.Country;
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
            case "ShipToAddress":
                SaveDataToPropertyBag(args[1]);

                if (Autoship.PropertyBag.ReferredByEndOfCheckout)
                {
                    Autoship.PropertyBag.ReferredByEndOfCheckout = false;
                    Autoship.PropertyBag.Save();
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                else
                {
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.ShippingMethod));
                }
                break;

            default:
                throw new Exception("RaisePostBackEvent argument '" + args[0] + "' is not defined.");
        }
    }
    #endregion

    #region Render
    public void RenderAddress(AutoshipCartPropertyBag.AddressType addressType)
    {
        if (IsAddressOnFileValid(addressType))
        {
            StringBuilder html = new StringBuilder();

            var addressOnFile = AddressesOnFile.Where(a => a.AddressType == addressType).FirstOrDefault();

            html.Append(@"
                <td valign='top'>
                    <a href=""" + Page.ClientScript.GetPostBackClientHyperlink(this, "ShipToAddress|" + addressType.ToString()) + @""" class='btn btn-success Next'>
                    " + Resources.Shopping.ShipToThisAddress + @"</a>
                    <br />
                    <br />
                    <strong>" + addressOnFile.FirstName + @" " + addressOnFile.LastName + @"</strong><br />
                    " + addressOnFile.Address1 + ((!string.IsNullOrEmpty(addressOnFile.Address2)) ? "<br />" + addressOnFile.Address2 : "") + @"<br />
                    " + addressOnFile.City + @", " + addressOnFile.State + @" " + addressOnFile.Zip + @"<br />
                    " + addressOnFile.Country + @"<br />
                    <br />
                    " + Resources.Shopping.Phone + ": " + addressOnFile.Phone + @"<br />
                    " + Resources.Shopping.Email + ": " + addressOnFile.Email + @"
                </td>
            ");


            HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
            writer.Write(html.ToString());
        }
    }
    #endregion

    #region Helper Methods
    public bool IsAddressOnFileValid(AutoshipCartPropertyBag.AddressType addressType)
    {
        var addressOnFile = AddressesOnFile.Where(a => a.AddressType == addressType).FirstOrDefault();

        return (!string.IsNullOrEmpty(addressOnFile.FirstName)
                && !string.IsNullOrEmpty(addressOnFile.LastName)
                && !string.IsNullOrEmpty(addressOnFile.Address1)
                && !string.IsNullOrEmpty(addressOnFile.City)
                && !string.IsNullOrEmpty(addressOnFile.State)
                && !string.IsNullOrEmpty(addressOnFile.Zip)
                && !string.IsNullOrEmpty(addressOnFile.Country));
    }

    public bool HasOneOrMoreValidAddressesOnFile()
    {
        foreach (var addressOnFile in AddressesOnFile)
        {
            if (IsAddressOnFileValid(addressOnFile.AddressType))
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

            dict.Add(lstCountry, lstState);

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
                if (Autoship.PropertyBag.Market.Countries.Contains(r.CountryCode))
                {
                    pair.Key.Items.Add(new ListItem()
                    {
                        Value = r.CountryCode,
                        Text = r.CountryName
                    });
                }
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

    #region Models
    public class AddressOnFile
    {
        public AddressOnFile(AutoshipCartPropertyBag.AddressType addressType)
        {
            AddressType = addressType;
        }

        public AutoshipCartPropertyBag.AddressType AddressType { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
    #endregion
}
