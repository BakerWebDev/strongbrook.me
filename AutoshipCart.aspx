<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="AutoshipCart.aspx.cs" Inherits="AutoshipCartPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'autoships'
        };
    </script>

    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">

    <h1><%=Resources.Shopping.Autoships %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="cart" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">

                <table width="100%">
                    <tr>
                        <td></td>
                        <td valign="top" style="width: 100%;">
                            <% if(Autoship.Cart.Items.Count() == 0)
                               { %>
                            <p><%=Resources.Shopping.AutoshipCartEmpty %>
                            </p>
                            <p>
                                <a href="<%=Autoship.UrlProductList %>" class="btn btn-success back"><%=Resources.Shopping.ContinueShopping %>
                                </a>
                            </p>
                            <% }
                               else
                               { %>
                            <div id="cartheader">
                                <a href="<%=Autoship.UrlProductList %>" class="btn"><%=Resources.Shopping.ContinueShopping %>
                                </a><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "UpdateCart") %>"
                                    class="btn pull-right"><%=Resources.Shopping.UpdateCart %></a>
                            </div>
                            <div class="clearfix">
                            </div>
                            <table cellpadding="0" cellspacing="0" id="cartcontents">
                                <tr>
                                    <th class="description"><%=Resources.Shopping.Item %> </th>
                                    <th class="priceeach"><%=Resources.Shopping.PriceEach %> </th>
                                    <th class="bv"><%=Resources.Shopping.BV %> </th>
                                    <th class="quantity"><%=Resources.Shopping.Quantity %> </th>
                                    <th class="total"><%=Resources.Shopping.Total %> </th>
                                    <th class="options">&nbsp; </th>
                                </tr>
                                <%  int renderedItemCounter = 0;
                                    foreach(var item in CartItems)
                                    { %>
                                <tr>
                                    <td class="description">
                                        <img src="<%=GlobalUtilities.GetProductImagePath(item.Image) %>" alt="<%=item.Description %>"
                                            title="<%=item.Description %>" />
                                        <span class="producttitle"><a href="<%=Autoship.UrlProductDetail %>?item=<%=item.ItemCode%>">
                                            <%=item.Description%></a></span>
                                        <br />
                                        <span class="itemcode">SKU:
                                            <%=item.ItemCode%></span>
                                        <div class="ClearAllFloats">
                                        </div>
                                    </td>
                                    <td class="priceeach">
                                        <%=item.PriceEach.ToString("C")%>
                                    </td>
                                    <td class="bv">
                                        <%=item.BV.ToString("N2")%>
                                    </td>
                                    <td class="quantity">
                                        <input type="text" id="<%=Autoship.Cart.GetFormFieldID("Quantity", renderedItemCounter) %>"
                                            name="<%=Autoship.Cart.GetFormFieldID("Quantity", renderedItemCounter) %>" value="<%=item.Quantity.ToString("0") %>"
                                            maxlength="9" />
                                    </td>
                                    <td class="total">
                                        <%=item.PriceTotal.ToString("C")%>
                                    </td>
                                    <td class="options"><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "RemoveFromCart|" + item.ItemCode) %>">
                                        <img src="Assets/Images/btnDelete.png" alt="<%=Resources.Shopping.Remove %>" title="<%=string.Format(Resources.Shopping.RemoveFromCart, item.Description) %>" />
                                    </a>
                                        <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("ItemCode", renderedItemCounter) %>"
                                            name="<%=Autoship.Cart.GetFormFieldID("ItemCode", renderedItemCounter) %>" value="<%=item.ItemCode %>" />
                                        <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("ParentItemCode", renderedItemCounter) %>"
                                            name="<%=Autoship.Cart.GetFormFieldID("ParentItemCode", renderedItemCounter) %>"
                                            value="<%=item.ParentItemCode %>" />
                                        <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("Type", renderedItemCounter) %>"
                                            name="<%=Autoship.Cart.GetFormFieldID("Type", renderedItemCounter) %>" value="<%=item.Type %>" />
                                    </td>
                                </tr>
                                <% renderedItemCounter++; %>
                                <% } %>
                            </table>
                            <div id="shoppingcartfooter">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="width: 100%" valign="top">&nbsp; </td>
                                        <td valign="top">
                                            <div class="ordertotals">
                                                <table>
                                                    <tr class="grandtotal">
                                                        <td class="fieldlabel"><%=Resources.Shopping.Subtotal %> </td>
                                                        <td class="value">
                                                            <%=string.Format("{0:C}", CalculatedOrder.SubTotal)%>
                                                        </td>
                                                    </tr>
                                                    <tr class="checkout">
                                                        <td class="fieldlabel">&nbsp; </td>
                                                        <td class="value"><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "Checkout") %>"
                                                            class="btn btn-success AddToCart"><%=Resources.Shopping.Continue %></a></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <% } %>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
