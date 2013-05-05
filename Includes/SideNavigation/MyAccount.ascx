<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccount.ascx.cs" Inherits="Includes_SideNavigation_MyAccount" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="settings"><a href="PersonalSettings.aspx">Settings</a></li>
    <li data-key="login"><a href="LoginSettings.aspx">Login</a></li>
    <%--<li data-key="website"><a href="WebsiteSettings.aspx">Website</a></li>--%>
    <%--<li data-key="placement"><a href="BinaryPlacementPreferences.aspx">Placement Preferences</a></li>--%>
    <li data-key="notifications"><a href="EmailNotifications.aspx">Notifications</a></li>
    <%--<li data-key="subscriptions"><a href="Subscriptions.aspx">Subscriptions</a></li>--%>
    <li data-key="billinghistory"><a href="BillingHistory.aspx">Billing History</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>