<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Commissions.ascx.cs" Inherits="Includes_SideNavigation_Commissions" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="summary"><a href="CommissionsPortal.aspx">Summary</a></li>
    <li data-key="commissions"><a href="Commissions.aspx">Commissions</a></li>
    <li data-key="volumes"><a href="Volumes.aspx">Volumes</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>