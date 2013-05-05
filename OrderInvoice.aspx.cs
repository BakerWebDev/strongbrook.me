using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class OrderInvoice : Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Order == null)
        {
            Response.Write("We were unable to find the order you requested. Please contact customer service for more information.");
            Response.End();
        }
    }
    #endregion

    #region Models
    public Order Order
    {
        get
        {
            if (_order == null)
            {
                _order = (from o in ExigoApiContext.CreateODataContext().Orders.Expand("OrderType").Expand("OrderStatus").Expand("ShipMethod").Expand("Details").Expand("Payments")
                            where o.OrderID == Convert.ToInt32(Request.QueryString["id"])
                            select o).FirstOrDefault();
            }
            return _order;
        }
    }
    private Order _order;

    public WarehouseResponse Warehouse
    {
        get
        {
            if (_warehouse == null)
            {
                _warehouse = ExigoApiContext.CreateWebServiceContext().GetWarehouses(new GetWarehousesRequest()).Warehouses.ToList().Where(w => w.WarehouseID == Order.WarehouseID).FirstOrDefault();
            }
            return _warehouse;
        }
    }
    private WarehouseResponse _warehouse;
    #endregion
}