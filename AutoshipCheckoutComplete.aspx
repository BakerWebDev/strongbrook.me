<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="AutoshipCheckoutComplete.aspx.cs" Inherits="AutoshipCheckoutComplete" %>

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
                <div id="shoppingcheckout">

                    <p><%=Resources.Shopping.AutoshipSaved %>
                    </p>
                    <p>
                        <a href="AutoshipInvoice.aspx?id=<%=Request.QueryString["orderid"] %>" class="btn btn-success" target="_blank"><%=Resources.Shopping.ViewInvoice %></a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
