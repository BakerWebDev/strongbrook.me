<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="ShoppingProductDetail.aspx.cs" Inherits="ShoppingProductDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'orders'
        };
    </script>

    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript">

        function ValidateQuantity($element) {
            var quantity = parseInt($element.val());
            if(quantity != null && quantity > 0 && quantity <= <%=Item.MaxAllowedOnOrder %>) return true;
            else return false;
        }

        $(function () {
            $('input:text[id*="Quantity_0"]').on({
                'keypress': function (e) {
                    var code = e.keyCode || e.which;
                    if (code == 13) {
                        e.preventDefault();
                        if (ValidateQuantity($(this))) {
                            $('A[id="AddToCart"]').click();
                        }
                    }
                },
                'blur': function (e) {
                    if (!ValidateQuantity($(this))) {
                        $(this).val('1');
                    }
                }
            }).focus();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1><%=Resources.Shopping.Orders %></h1>

    <div class="sidebar">
        <navigation:Orders ID="SideNavigation" ActiveNavigation="products" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <h2><%=Resources.Shopping.AllProducts %></h2>
                <p><%=Resources.Shopping.ReviewYourAvailableProducts_Shopping %></p>
                <table width="100%">
                    <tr>
                        <td valign="top">
                            <subnavigation:ShoppingProductNavigation runat="server" ID="ProductNavigation1" />
                        </td>
                        <td valign="top">
                            <table width="100%" id="productdetails">
                                <tr>
                                    <td id="picturewrapper">
                                        <img src="<%=GlobalUtilities.GetProductImagePath(Item.SmallPicture) %>" alt="<%=Item.Description %>"
                                            title="<%=Item.Description %>" />
                                    </td>
                                    <td id="productoptionswrapper">
                                        <h2>
                                            <%=Item.Description %></h2>
                                        <div id="itemcode">
                                            <%=Resources.Shopping.SKU %>:
                                                    <%=Item.ItemCode %>
                                        </div>
                                        <% if(!string.IsNullOrEmpty(Item.ShortDetail))
                                           { %>
                                        <div id="shortdescription">
                                            "<%=Item.ShortDetail%>"
                                        </div>
                                        <% } %>
                                        <div id="priceswrapper">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <span class="label"><%=Resources.Shopping.YourPrice %>:</span>
                                                        <div class="price">
                                                            <%=Item.Price.ToString("C") %>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <span class="label"><%=Resources.Shopping.BVEach %>:</span>
                                                        <div class="price">
                                                            <%=Item.BusinessVolume.ToString("N2") %>
                                                        </div>
                                                    </td>

                                                </tr>
                                            </table>
                                        </div>
                                        <div id="quantitywrapper">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <span class="label"><%=Resources.Shopping.Quantity %>:</span>
                                                        <div class="field">
                                                            <input type="text" id="<%=Shopping.Cart.GetFormFieldID("Quantity") %>" name="<%=Shopping.Cart.GetFormFieldID("Quantity") %>"
                                                                maxlength="9" value="1" />
                                                        </div>
                                                    </td>
                                                    <% /* ----- START ITEM CODE ----- */ %>
                                                    <% if(!Item.IsGroupMaster)
                                                       { %>
                                                    <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("ItemCode") %>" name="<%=Shopping.Cart.GetFormFieldID("ItemCode") %>"
                                                        value="<%=Item.ItemCode %>" />
                                                    <%  }  %>
                                                    <% /* ----- END ITEM CODE ----- */ %>
                                                    <% /* ----- START ITEM GROUP ----- */ %>
                                                    <%  if(Item.IsGroupMaster)
                                                        { %>
                                                    <td>


                                                        <span class="label" style="background-color:transparent; margin-bottom:10px;">

<%--
                                                            <%=Item.GroupMembersDescription %>:
--%>

                                                        </span>


                                                        <div class="field">
                                                            <select id="<%=Shopping.Cart.GetFormFieldID("ItemCode") %>" name="<%=Shopping.Cart.GetFormFieldID("ItemCode") %>">
                                                                <% foreach(var groupMember in Item.GroupMembers)
                                                                   { %>
                                                                <option value="<%=groupMember.ItemCode %>">
                                                                    <%=groupMember.MemberDescription %></option>
                                                                <% } %>
                                                            </select>
                                                        </div>
                                                    </td>
                                                    <%  } %>
                                                    <% /* ----- END ITEM GROUP ----- */ %>
                                                    <td>
                                                        <br />
                                                        <div class="field">
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div id="options">
                                            <a onclick="<%=Page.ClientScript.GetPostBackClientHyperlink(this, "AddToCart") %>"
                                                id="AddToCart" name="addtocart" class="btn btn-success"><%=Resources.Shopping.AddToCart %></a>
                                        </div>
                                        <% /* ----- START HIDDEN FIELD VARIABLES ----- */ %>
                                        <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("ParentItemCode") %>" name="<%=Shopping.Cart.GetFormFieldID("ParentItemCode") %>"
                                            value="" />
                                        <input type="hidden" id="<%=Shopping.Cart.GetFormFieldID("Type") %>" name="<%=Shopping.Cart.GetFormFieldID("Type") %>"
                                            value="<%=ShoppingCartItemType.Default %>" />
                                        <% /* ----- END HIDDEN FIELD VARIABLES ----- */ %>
                                    </td>
                                </tr>
                            </table>
                            <%=Item.LongDetail%>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
