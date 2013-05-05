<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="BillingHistory.aspx.cs" Inherits="BillingHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'myaccount'
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="billinghistory" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Billing History</h2>
        <div class="well well-large well-white">
            <% RenderBillingHistory(); %>
        </div>
    </div>
</asp:Content>

