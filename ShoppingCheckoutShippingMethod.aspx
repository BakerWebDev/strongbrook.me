<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="ShoppingCheckoutShippingMethod.aspx.cs" Inherits="ShoppingCheckoutShippingMethod" %>

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
        <navigation:Orders ID="SideNavigation" ActiveNavigation="summary" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <div id="shoppingcheckout">
                    <h2><%=Resources.Shopping.ChooseYourShippingOptions %></h2>
                    <p><%=Resources.Shopping.ChooseAShippingSpeed %>:
                    </p>

                    <asp:RadioButtonList ID="rdoShipMethod" runat="server" CssClass="checkboxes" />

                    <br />
                    <p>
                        <asp:LinkButton ID="cmdCheckoutShippingMethodNext_Click" runat="server" CssClass="btn btn-success" OnClick="SelectShipMethod_Click" />
                    </p>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
