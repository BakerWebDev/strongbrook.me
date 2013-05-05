<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="AutoshipProductDetail.aspx.cs" Inherits="AutoshipProductDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'autoships'
        };
    </script>

    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" language="javascript">

        function ValidateQuantity($element) {
            var quantity = parseInt($element.val());
            if(quantity != null && quantity > 0 && quantity <= <%=Item.MaxAllowedOnOrder %>) return true;
            else return false;
        }

        $(document).ready(function () {
            $('input:text[id*="Quantity_0"]').bind('keypress', function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) {
                    e.preventDefault();
                    if (ValidateQuantity($(this))) {
                        $('A[id="AddToCart"]').click();
                    }
                }
            });

            $('input:text[id*="Quantity_0"]').bind('blur', function (e) {
                if (!ValidateQuantity($(this))) {
                    $(this).val('1');
                }
            });

            $('input:text[id*="Quantity_0"]').focus();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">


    <h1><%=Resources.Shopping.Autoships %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="products" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">

                <table width="100%">
                    <tr>
                        <td valign="top">
                            <subnavigation:AutoshipProductNavigation runat="server" ID="ProductNavigation1" />
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
                                            <%=Item.Description %>
                                        </h2>
                                        <div id="itemcode">
                                            <%=Resources.Shopping.SKU %>:
                                                    <%=Item.ItemCode %>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(Item.ShortDetail))
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
                                                            <input type="text" id="<%=Autoship.Cart.GetFormFieldID("Quantity") %>" name="<%=Autoship.Cart.GetFormFieldID("Quantity") %>"
                                                                maxlength="9" value="1" />
                                                        </div>
                                                    </td>
                                                    <% /* ----- START ITEM CODE ----- */ %>
                                                    <% if (!Item.IsGroupMaster)
                                                       { %>
                                                    <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("ItemCode") %>" name="<%=Autoship.Cart.GetFormFieldID("ItemCode") %>"
                                                        value="<%=Item.ItemCode %>" />
                                                    <%  }  %>
                                                    <% /* ----- END ITEM CODE ----- */ %>
                                                    <% /* ----- START ITEM GROUP ----- */ %>
                                                    <%  if (Item.IsGroupMaster)
                                                        { %>
                                                    <td>
                                                        <span class="label">
                                                            <%=Item.GroupMembersDescription %>:</span>
                                                        <div class="field">
                                                            <select id="<%=Autoship.Cart.GetFormFieldID("ItemCode") %>" name="<%=Autoship.Cart.GetFormFieldID("ItemCode") %>">
                                                                <% foreach (var groupMember in Item.GroupMembers)
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
                                        <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("ParentItemCode") %>" name="<%=Autoship.Cart.GetFormFieldID("ParentItemCode") %>"
                                            value="" />
                                        <input type="hidden" id="<%=Autoship.Cart.GetFormFieldID("Type") %>" name="<%=Autoship.Cart.GetFormFieldID("Type") %>"
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
