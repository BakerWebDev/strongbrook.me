<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="SubscriptionsPortal.aspx.cs" Inherits="SubscriptionsPortal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'subscriptions'
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Subscriptions</h1>

    <div class="sidebar">          
        <navigation:Subscriptions id="SideNavigation" ActiveNavigation="summary" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            Coming soon!
        </div>
    </div>
</asp:Content>

