<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="ShoppingCheckoutReview.aspx.cs" Inherits="ShoppingCheckoutReview" %>

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
                <div id="shoppingcheckout">

                    <exigo:applicationerrormodal id="ApplicationErrors" runat="server" />

                    <h2><%=Resources.Shopping.ReviewYourOrder %></h2>
                    <div id="ordertotalsreviewwrapper">
                        <asp:LinkButton ID="cmdPlaceOrder" runat="server" CssClass="btn btn-success placeorderbutton" OnClick="PlaceOrder_Click" />
                        <table cellpadding="0" cellspacing="0" id="ordertotalsreview">
                            <tr>
                                <td colspan="2">
                                    <h3><%=Resources.Shopping.OrderSummary %></h3>
                                </td>
                            </tr>
                            <tr class="subtotal">
                                <td class="fieldlabel"><%=Resources.Shopping.Subtotal %> </td>
                                <td class="value">
                                    <%=string.Format("{0:C}", CalculatedOrder.SubTotal)%>
                                </td>
                            </tr>
                            <% if(CalculatedOrder.DiscountTotal > 0)
                               { %>
                            <tr class="discount">
                                <td class="fieldlabel"><%=Resources.Shopping.Discount %> </td>
                                <td class="value">
                                    <%=string.Format("<nobr>{0:C} ({1:P0})</nobr>", CalculatedOrder.DiscountTotal, CalculatedOrder.DiscountPercent)%>
                                </td>
                            </tr>
                            <% } %>
                            <tr class="shipping">
                                <td class="fieldlabel"><%=Resources.Shopping.Shipping %><br />
                                    <span style="FONT-SIZE: 10px;">
                                        <%=CalculatedOrder.ShipMethods.Where(s => s.ShipMethodID.ToString() == rdoShipMethod.SelectedValue).FirstOrDefault().Description %>
                                    </span></td>
                                <td class="value">
                                    <%=string.Format("{0:C}", CalculatedOrder.ShippingTotal)%>
                                </td>
                            </tr>
                            <tr class="taxes">
                                <td class="fieldlabel"><%=Resources.Shopping.Taxes %> </td>
                                <td class="value">
                                    <%=string.Format("{0:C}", CalculatedOrder.TaxTotal)%>
                                </td>
                            </tr>
                            <tr class="grandtotal">
                                <td colspan="2"><%=Resources.Shopping.OrderTotal %><br />
                                    <%=string.Format("{0:C}", CalculatedOrder.Total)%>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <table cellpadding="0" cellspacing="0" class="orderreview">
                        <tr>
                            <th class="shippingaddress"><%=Resources.Shopping.ShippingInformation %> </th>
                            <th class="billinginformation"><%=Resources.Shopping.BillingInformation %> </th>
                            <th class="billinginformation"><%=Resources.Shopping.ShippingOptions %> </th>
                        </tr>
                        <tr>
                            <td class="shippingaddress"><strong><%=Resources.Shopping.ShippingAddress %>:</strong><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "ChangeShippingAddress") %>">
                        (<%=Resources.Shopping.Change %>)</a>
                                <br />
                                <%=Shopping.PropertyBag.ShippingFirstName %>
                                <%=Shopping.PropertyBag.ShippingLastName%><br />
                                <%=Shopping.PropertyBag.ShippingAddress1 + ((!string.IsNullOrEmpty(Shopping.PropertyBag.ShippingAddress2)) ? "<br />" + Shopping.PropertyBag.ShippingAddress2 : "")%><br />
                                <%=Shopping.PropertyBag.ShippingCity%>,
                                <%=Shopping.PropertyBag.ShippingState%>
                                <%=Shopping.PropertyBag.ShippingZip%><br />
                                <%=Shopping.PropertyBag.ShippingCountry%><br />
                                <br />
                                <%=Resources.Shopping.Phone %>:
                                <%=Shopping.PropertyBag.Phone%><br />
                                <%=Resources.Shopping.Email %>:
                                <%=Shopping.PropertyBag.Email%>
                            </td>
                            <td class="billinginformation"><strong><%=Resources.Shopping.PaymentMethod %>:</strong><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "ChangePayment") %>">
                        (<%=Resources.Shopping.Change %>)</a>
                                <br />
                                <% if(Shopping.PropertyBag.PaymentType == ShoppingCartPropertyBag.PaymentMethodType.NewCreditCard
                                   || Shopping.PropertyBag.PaymentType == ShoppingCartPropertyBag.PaymentMethodType.PrimaryCreditCard
                                   || Shopping.PropertyBag.PaymentType == ShoppingCartPropertyBag.PaymentMethodType.SecondaryCreditCard)
                                   { %>
                        <%=Resources.Shopping.CreditDebitCardEndingIn %>
                        <%=Shopping.PropertyBag.CreditCardNumber.Substring(Shopping.PropertyBag.CreditCardNumber.Length - 4, 4)%>
                                <br />
                                <br />
                                <strong><%=Resources.Shopping.BillingAddress %>:</strong>
                                <br />
                                <%=Shopping.PropertyBag.CreditCardNameOnCard%><br />
                                <%=Shopping.PropertyBag.CreditCardBillingAddress%><br />
                                <%=Shopping.PropertyBag.CreditCardBillingCity%>,
                                <%=Shopping.PropertyBag.CreditCardBillingState%>
                                <%=Shopping.PropertyBag.CreditCardBillingZip%><br />
                                <%=Shopping.PropertyBag.CreditCardBillingCountry%>
                                <% } %>
                                <% if(Shopping.PropertyBag.PaymentType == ShoppingCartPropertyBag.PaymentMethodType.BankAccountOnFile
                                   || Shopping.PropertyBag.PaymentType == ShoppingCartPropertyBag.PaymentMethodType.NewBankAccount)
                                   { %>
                        <%=Resources.Shopping.BankAccountEndingIn %>
                        <%=Shopping.PropertyBag.BankAccountAccountNumber.Substring(Shopping.PropertyBag.CreditCardNumber.Length - 4, 4)%>
                                <br />
                                <br />
                                <strong><%=Resources.Shopping.BankAddress %>:</strong>
                                <br />
                                <%=Shopping.PropertyBag.BankAccountBankName%><br />
                                <%=Shopping.PropertyBag.BankAccountBankAddress%><br />
                                <%=Shopping.PropertyBag.BankAccountBankCity%>,
                            <%=Shopping.PropertyBag.BankAccountBankState%>
                                <%=Shopping.PropertyBag.BankAccountBankZip%><br />
                                <%=Shopping.PropertyBag.BankAccountBankCountry%>
                                <% } %>
                            </td>
                            <td class="shippingoptions checkboxes"><strong><%=Resources.Shopping.ChooseAShippingSpeed %>:</strong>
                                <br />
                                <asp:RadioButtonList ID="rdoShipMethod" runat="server" RepeatLayout="Flow" OnSelectedIndexChanged="ChangeShippingMethod_SelectedIndexChanged"
                                    AutoPostBack="true" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <h3><%=Resources.Shopping.TodaysOrder %></h3>
                    <table cellpadding="0" cellspacing="0" id="cartcontents" class="cartreviewtable">
                        <tr>
                            <th class="description"><%=Resources.Shopping.Item %> </th>
                            <th class="priceeach"><%=Resources.Shopping.PriceEach %></th>
                            <th class="quantity"><%=Resources.Shopping.Quantity %></th>
                            <th class="total"><%=Resources.Shopping.Total %> </th>
                        </tr>
                        <% foreach(var item in CartItems.Where(i => i.Type == ShoppingCartItemType.Default))
                           { %>
                        <tr>
                            <td class="description">
                                <img src="<%=item.Image %>" alt="<%=item.Description %>" title="<%=item.Description %>" />
                                <span class="producttitle"><a href="<%=Shopping.GetStepUrl(ShoppingStep.ProductDetail) %>?item=<%=item.ItemCode %>">
                                    <%=item.Description %></a></span>
                                <br />
                                <span class="itemcode"><%=Resources.Shopping.SKU %>:
                            <%=item.ItemCode %></span>
                                <div class="clearfix">
                                </div>
                            </td>
                            <td class="priceeach">
                                <%=item.PriceEach.ToString("C") %>
                            </td>
                            <td class="quantity">
                                <%=item.Quantity.ToString("N0") %>
                            </td>
                            <td class="total" style="border-right:1px solid #aaa;">
                                <%=item.PriceTotal.ToString("C") %>
                            </td>
                        </tr>
                        <% } %>
                        <tr>
                            <td colspan="6" class="options"><a href="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "ChangeItems") %>" class="btn"><%=Resources.Shopping.ChangeItems %></a></td>
                        </tr>
                    </table>
                    <br />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
