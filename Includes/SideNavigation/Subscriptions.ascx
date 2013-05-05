<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Subscriptions.ascx.cs" Inherits="Includes_SideNavigation_Subscriptions" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="summary"><a href="SubscriptionsPortal.aspx">Summary</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>