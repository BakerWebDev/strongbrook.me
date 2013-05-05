<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="ShoppingCart.aspx.cs" Inherits="ShoppingCartPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'orders'
        };
    </script>

    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1><%=Resources.Shopping.Orders %></h1>

    <div class="sidebar">
        <navigation:Orders ID="SideNavigation" ActiveNavigation="cart" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <table width="100%">
                    <tr>
                        <td></td>
                        <td valign="top" style="width: 100%;">
                            <% if(Shopping.Cart.Items.Where(c => c.Type == ShoppingCartItemType.Default).Count() == 0)
                               { %>
                            <p><%=Resources.Shopping.YourShoppingCartIsEmpty %>
                            </p>
                            <p>
                                <a href="<%=Shopping.GetStepUrl(ShoppingStep.ProductList) %>" class="btn btn-success"><%=Resources.Shopping.ContinueShopping %></a>
                            </p>
                            <% }
                               else
                               { %>
                            <div id="cartheader">
                                <a href="<%=Shopping.GetStepUrl(ShoppingStep.ProductList) %>" class="btn"><%=Resources.Shopping.ContinueShopping %></a>
                                <a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "UpdateCart") %>"
                                    class="btn pull-right"><%=Resources.Shopping.UpdateCart %></a>
                            </div>
                            <div class="clearfix">
                            </div>
                            <table cellpadding="0" cellspacing="0" id="cartcontents">

                                <tr>
                                    <th class="description"><%=Resources.Shopping.Item %>
                                    </th>
                                    <th class="priceeach"><%=Resources.Shopping.PriceEach %>
                                    </th>
                                    <th class="bv"><%=Resources.Shopping.BV %>
                                    </th>
                                    <th class="quantity"><%=Resources.Shopping.Quantity %>
                                    </th>
                                    <th class="total"><%=Resources.Shopping.Total %>
                                    </th>
                                    <th class="options">&nbsp;
                                    </th>
                                </tr>
                                <%  int renderedItemCounter = 0;
                                    foreach(var item in CartItems)
                                    { %>
                                <tr>
                                    <td class="description">
                                        <img src="<%=GlobalUtilities.GetProductImagePath(item.Image) %>" alt="<%=item.Description %>" title="<%=item.Description %>" />
                                        <span class="producttitle"><a href="<%=Shopping.GetStepUrl(ShoppingStep.ProductDetail) %>?item=<%=item.ItemCode%>">
                                            <%=item.Description%></a></span><br />
                                        <span class="itemcode"><%=Resources.Shopping.SKU %>:
                                            <%=item.ItemCode%></span>
                                        <div class="clearfix">
                                        </div>
                                    </td>
                                    <td class="priceeach">
                                        <%=item.PriceEach.ToString("C")%>
                                    </td>
                                    <td class="bv">
                                        <%=item.BV.ToString("N2")%>
                                    </td>

                                    <td class="quantity">
                                        <input type="text" id="<%=Shopping.Cart.GetFormFieldID("Quantity", renderedItemCounter) %>"
                                            name="<%=Shopping.Cart.GetFormFieldID("Quantity", renderedItemCounter) %>" value="<%=item.Quantity.ToString("0") %>"
                                            maxlength="9" />
                                    </td>
                                    <td class="total">
                                        <%=item.PriceTotal.ToString("C")%>
                                    </td>
                                    <td class="options">
                                        <a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "RemoveFromCart|" + item.ItemCode + "|" + item.Type) %>">
                                            <img src="Assets/Images/btnDelete.png" alt="<%=Resources.Shopping.Remove %>" title="<%=string.Format(Resources.Shopping.RemoveFromCart, item.Description) %>" /></a>
                                        <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("ItemCode", renderedItemCounter) %>"
                                            name="<%=Shopping.Cart.GetFormFieldID("ItemCode", renderedItemCounter) %>" value="<%=item.ItemCode %>" />
                                        <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("ParentItemCode", renderedItemCounter) %>"
                                            name="<%=Shopping.Cart.GetFormFieldID("ParentItemCode", renderedItemCounter) %>"
                                            value="<%=item.ParentItemCode %>" />
                                        <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("Type", renderedItemCounter) %>"
                                            name="<%=Shopping.Cart.GetFormFieldID("Type", renderedItemCounter) %>" value="<%=item.Type %>" />
                                    </td>
                                </tr>
                                <% renderedItemCounter++; %>
                                <% } %>
                            </table>
                            <div id="shoppingcartfooter">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="width: 100%" valign="top">&nbsp;
                                        </td>
                                        <td valign="top">
                                            <div class="ordertotals">
                                                <table>
                                                    <tr class="grandtotal">
                                                        <td class="fieldlabel"><%=Resources.Shopping.Subtotal %>
                                                        </td>
                                                        <td class="value">
                                                            <%=string.Format("{0:C}", CalculatedOrder.SubTotal)%>
                                                        </td>
                                                    </tr>
                                                    <tr class="checkout">
                                                        <td class="fieldlabel">&nbsp;
                                                        </td>
                                                        <td class="value">
                                                            <a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "Checkout") %>" class="btn btn-success"><%=Resources.Shopping.Checkout %></a>
                                                        </td>
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
