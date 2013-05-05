<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="AutoshipProductList.aspx.cs" Inherits="AutoshipProductList" %>

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
<h1><%=Resources.Shopping.Autoship %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="products" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <table style="width: 100%;">
                    <tr>
                        <td valign="top">
                            <subnavigation:AutoshipProductNavigation runat="server" id="ProductNavigation1" />
                        </td>
                        <td valign="top" style="width: 100%;">
                            <div id="productlist">
                                <% RenderProductListHeader(); %>
                                <% RenderTopPagerBar(); %>
                                <% RenderProductListItems(); %>
                                <% RenderBottomPagerBar(); %>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
