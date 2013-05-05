<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Subscriptions.aspx.cs" Inherits="Subscriptions" %>

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
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="subscriptions" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Subscriptions</h2>
        <div class="well well-large well-white">
            <% RenderSubscriptionHistory(); %>
        </div>
    </div>
</asp:Content>

