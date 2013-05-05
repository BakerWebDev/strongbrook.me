<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Invoice.Master"
    AutoEventWireup="true" CodeFile="OrderInvoice.aspx.cs" Inherits="OrderInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div id="invoice">
        <div id="invoiceheader">
            <h1>Invoice</h1>
            <div class="invoicedetails">
                <table>
                    <tr>
                        <td class="label">Date:
                        </td>
                        <td class="value">
                            <%=string.Format("{0:MMMM d, yyyy}", Order.OrderDate) %>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Invoice #:
                        </td>
                        <td class="value">
                            <%=Order.OrderID %>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="companyinformation">
            <table>
                <tr>
                    <td>
                        <div class="address">
                            <h2>
                                <%=GlobalSettings.Company.Name %></h2>
                            <%=GlobalSettings.Company.Address %><br />
                            <%=GlobalSettings.Company.City %>,
                            <%=GlobalSettings.Company.State %>
                            <%=GlobalSettings.Company.Zip %><br />
                            <%=GlobalSettings.Company.Country %><br />
                            <%=GlobalSettings.Company.Phone %>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="sectiondivider">
        </div>
        <div class="billingandshippingaddresses">
            <table>
                <tr>
                    <td class="label">Ship To:
                    </td>
                    <td class="value">
                        <%=Order.FirstName %> <%=Order.LastName %><br />
                        <%=Order.Address1 %><br />
                        <%=(!string.IsNullOrEmpty(Order.Address2)) ? Order.Address2 + "<br />" : string.Empty %>
                        <%=Order.City %>, <%=Order.State %> <%=Order.Zip %>, <%=Order.Country %>
                    </td>

                    <td class="label">Fullfillment Center:
                    </td>
                    <td class="value">
                        <%=Warehouse.Description %><br />
                        <%=Warehouse.Address1%><br />
                        <%=(!string.IsNullOrEmpty(Warehouse.Address2)) ? Warehouse.Address2 + "<br />" : string.Empty%>
                        <%=Warehouse.City%>, <%=Warehouse.State%> <%=Warehouse.Zip%>, <%=Warehouse.Country%>
                    </td>
                </tr>
            </table>
        </div>





        <div class="orderglobaldetails">

            <table>
                <tr>
                    <th>Customer #</th>
                    <th>Source</th>
                    <th>Status</th>
                    <th>Ship Via</th>
                    <th>Shipped Date</th>
                    <th>Tracking</th>
                </tr>
                <tr>
                    <td><%=Order.CustomerID %></td>
                    <td><%=Order.OrderType.OrderTypeDescription %></td>
                    <td><%=Order.OrderStatus.OrderStatusDescription %></td>
                    <td><%=Order.ShipMethod.ShipMethodDescription %></td>
                    <td><%=GlobalUtilities.Coalesce(string.Format("{0:MM/d/yyyy}", Order.ShippedDate), "---") %></td>
                    <td><%=GlobalUtilities.Coalesce(Order.TrackingNumber1, "---") %></td>
                </tr>
            </table>
        </div>

        <div class="orderitemdetails">

            <table>
                <tr>
                    <th>SKU</th>
                    <th>Description</th>
                    <th>Quantity</th>
                    <th>Unit Price</th>
                    <th>Unit BV</th>
                    <th>Unit CV</th>
                    <th class="totalcolumnwidth">Line Total</th>
                </tr>

                <% foreach(var item in Order.Details)
                   { %>
                <tr>
                    <td><%=item.ItemCode %></td>
                    <td><%=item.ItemDescription %></td>
                    <td class="aligncenter"><%=string.Format("{0:N0}", item.Quantity) %></td>
                    <td class="alignright"><%=string.Format("{0:C}", item.PriceEach) %></td>
                    <td class="alignright"><%=string.Format("{0:N2}", item.BusinessVolumeEach) %></td>
                    <td class="alignright"><%=string.Format("{0:N2}", item.CommissionableVolumeEach)%></td>
                    <td class="alignright totalcolumnwidth"><%=string.Format("{0:C}", item.PriceTotal) %></td>
                </tr>
                <% } %>
            </table>

        </div>


        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td style="width: 50%">
                    <div class="ordernotes">

                        <b>Notes:</b><br />
                        <div class="noteswrapper">
                            <%=Order.Notes %>
                        </div>

                    </div>


                </td>
                <td>


                    <div class="ordertotals">

                        <table>
                            <tr class="subtotal">
                                <td class="label">Subtotal</td>
                                <td class="value ui-corner-tl ui-corner-tr"><%=string.Format("{0:C}", Order.SubTotal) %></td>
                            </tr>

                            <% if(Order.DiscountTotal != 0)
                               { %>
                            <tr class="discount">
                                <td class="label">Discount</td>
                                <td class="value"><%=string.Format("{0:C} ({1:0}%)", Order.DiscountTotal, Order.DiscountPercent) %></td>
                            </tr>
                            <% } %>

                            <tr class="shipping">
                                <td class="label">Shipping & Handling</td>
                                <td class="value"><%=string.Format("{0:C}", Order.ShippingTotal) %></td>
                            </tr>
                            <tr class="tax">
                                <td class="label">Tax</td>
                                <td class="value"><%=string.Format("{0:C}", Order.TaxTotal) %></td>
                            </tr>
                            <tr class="grandtotal">
                                <td class="label">Grand Total</td>
                                <td class="value"><%=string.Format("{0:C}", Order.Total) %></td>
                            </tr>
                            <tr class="totalpaid">
                                <td class="label">Total Paid</td>
                                <td class="value"><%=string.Format("{0:C}", Order.Payments.Sum(p => p.Amount)) %></td>
                            </tr>
                            <tr class="totaldue">
                                <td class="label">Total Due</td>
                                <td class="value ui-corner-bl ui-corner-br">
                                    <div class="totalcolumnwidth"><%=string.Format("{0:C}", Order.Total - Order.Payments.Sum(p => p.Amount)) %></div>
                                </td>
                            </tr>
                        </table>

                    </div>

                </td>
            </tr>
        </table>

        <div class="clearfix"></div>

        <div class="thankyoumessage">
            Thank you for your business!        
        </div>
    </div>
</asp:Content>
