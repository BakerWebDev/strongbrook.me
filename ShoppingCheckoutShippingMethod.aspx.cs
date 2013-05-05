using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class ShoppingCheckoutShippingMethod : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Populate resource references
        cmdCheckoutShippingMethodNext_Click.Text = Resources.Shopping.Continue;

        // If there are no items in the cart, redirect them to the cart page.
        if (Shopping.Cart.Items.Count == 0)
        {
            Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Cart));
            return;
        }


        if (!IsPostBack)
        {
            PopulateAvailableShippingMethods_Load();

        }
    }

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
                    ShipMethodID = 1,
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

    public int SelectedShippingMethodID
    {
        get { return Convert.ToInt32(rdoShipMethod.SelectedValue); }
        set { rdoShipMethod.SelectedValue = value.ToString(); }
    }
    #endregion

    #region Load Data
    public void PopulatePropertyBagValues_Load()
    {
        if (Shopping.PropertyBag.ShipMethodID != 0 && rdoShipMethod.Items.FindByValue(Shopping.PropertyBag.ShipMethodID.ToString()) != null)
        {
            // De-select any previously selected items
            rdoShipMethod.Items.Cast<ListItem>().Where(i => i.Selected == true).ToList().ForEach(i => i.Selected = false);

            rdoShipMethod.Items.FindByValue(Shopping.PropertyBag.ShipMethodID.ToString()).Selected = true;
        }
    }
    #endregion

    #region Save Data
    public void SaveDataToPropertyBag()
    {
        Shopping.PropertyBag.ShipMethodID = SelectedShippingMethodID;

        Shopping.PropertyBag.Save();
    }
    #endregion

    #region Event Handlers
    public void SelectShipMethod_Click(object sender, EventArgs e)
    {
        SaveDataToPropertyBag();
        Response.Redirect(Shopping.GetStepUrl(ShoppingStep.Payment));
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
                else
                {
                    rdoShipMethod.SelectedIndex = 0;
                }
            }
        }

        // If we only have one ship method available, save the property bag and move on.
        if (rdoShipMethod.Items.Count == 1)
        {
            SelectShipMethod_Click(null, null);
        }
    }
    #endregion
}
