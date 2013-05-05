<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Invoice.Master"
    AutoEventWireup="true" CodeFile="AutoshipInvoice.aspx.cs" Inherits="AutoshipInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <div id="invoice">
        <div id="invoiceheader">
            <h1><%=Resources.Shopping.Autoship %></h1>
            <div class="invoicedetails">
                <table>
                    <tr>
                        <td class="label"><%=Resources.Shopping.CurrentDate %>:
                        </td>
                        <td class="value">
                            <%=DateTime.Now.ToString("dddd, MMMM d, yyyy") %>
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><%=Resources.Shopping.Description %>:
                        </td>
                        <td class="value">
                            <%=(!string.IsNullOrEmpty(Autoship.Description)) ? Autoship.Description : Resources.Shopping.AutoshipNumber + Autoship.AutoOrderID %>
                        </td>
                    </tr>
                    <tr>
                        <td class="label"><%=Resources.Shopping.AutoshipNumber %>:
                        </td>
                        <td class="value">
                            <%=Autoship.AutoOrderID %>
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
                            <h2><%=GlobalSettings.Company.Name %></h2>
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
                    <td class="label"><%=Resources.Shopping.ShipTo %>:
                    </td>
                    <td class="value">
                        <%=Autoship.FirstName %> <%=Autoship.LastName %><br />
                        <%=Autoship.Address1 %><br />
                        <%=(!string.IsNullOrEmpty(Autoship.Address2)) ? Autoship.Address2 + "<br />" : string.Empty %>
                        <%=Autoship.City %>, <%=Autoship.State %> <%=Autoship.Zip %>, <%=Autoship.Country %>
                    </td>


                    <td class="label"><%=Resources.Shopping.BillingTo %>:
                    </td>
                    <td class="value">
                        <% RenderBillingSummary(); %>
                    </td>


                    <td class="label"><%=Resources.Shopping.FulfillmentCenter %>:
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
                    <th><%=Resources.Shopping.CustomerNumber %></th>
                    <th><%=Resources.Shopping.Status %></th>
                    <th><%=Resources.Shopping.Frequency %></th>
                    <th><%=Resources.Shopping.LastRunDate %></th>
                    <th><%=Resources.Shopping.NextRunDate %></th>
                    <th><%=Resources.Shopping.ShipVia %></th>
                </tr>
                <tr>
                    <td><%=Autoship.CustomerID %></td>
                    <td><%=Autoship.AutoOrderStatus.ToString() %></td>
                    <td><%=Autoship.Frequency.ToString() %></td>
                    <td><%=(Autoship.LastRunDate != new DateTime()) ? Convert.ToDateTime(Autoship.LastRunDate).ToString("dddd, MMMM d, yyyy") : "---"%></td>
                    <td><%=(Autoship.NextRunDate != new DateTime()) ? Convert.ToDateTime(Autoship.NextRunDate).ToString("dddd, MMMM d, yyyy") : "---"%></td>
                    <td><%=ShipMethodDescription%></td>
                </tr>
            </table>
        </div>

        <div class="orderitemdetails">

            <table>
                <tr>
                    <th><%=Resources.Shopping.SKU %></th>
                    <th><%=Resources.Shopping.Description %></th>
                    <th><%=Resources.Shopping.Quantity %></th>
                    <th><%=Resources.Shopping.Unit %> <%=Resources.Shopping.Price %></th>
                    <th><%=Resources.Shopping.Unit %> <%=Resources.Shopping.BV %></th>
                    <th><%=Resources.Shopping.Unit %> <%=Resources.Shopping.CV %></th>
                    <th class=" totalcolumnwidth"><%=Resources.Shopping.LineTotal %></th>
                </tr>

                <% foreach(var item in Autoship.Details)
                   { %>
                <tr>
                    <td><%=item.ItemCode %></td>
                    <td><%=item.Description %></td>
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

                        <b><%=Resources.Shopping.Notes %>:</b><br />
                        <div class="noteswrapper">
                            <%=Autoship.Notes %>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="ordertotals">

                        <table>
                            <tr class="subtotal">
                                <td class="label"><%=Resources.Shopping.Subtotal %></td>
                                <td class="value"><%=string.Format("{0:C}", Autoship.SubTotal) %></td>
                            </tr>

                            <% if(Autoship.DiscountTotal != 0)
                               { %>
                            <tr class="discount">
                                <td class="label"><%=Resources.Shopping.Discount %></td>
                                <td class="value"><%=string.Format("{0:C}", Autoship.DiscountTotal) %></td>
                            </tr>
                            <% } %>

                            <tr class="shipping">
                                <td class="label"><%=Resources.Shopping.EstimatedShippingAndHandling %></td>
                                <td class="value"><%=string.Format("{0:C}", Autoship.ShippingTotal) %></td>
                            </tr>
                            <tr class="tax">
                                <td class="label"><%=Resources.Shopping.EstimatedTaxes %></td>
                                <td class="value"><%=string.Format("{0:C}", Autoship.TaxTotal) %></td>
                            </tr>
                            <tr class="grandtotal">
                                <td class="label"><%=Resources.Shopping.EstimatedTotal %></td>
                                <td class="value"><%=string.Format("{0:C}", Autoship.Total) %></td>
                            </tr>
                        </table>

                    </div>

                </td>
            </tr>
        </table>

        <div class="clearfix"></div>

        <div class="thankyoumessage">
            <%=Resources.Shopping.Invoice_ThankYouAutoship %>       
        </div>
    </div>
</asp:Content>
