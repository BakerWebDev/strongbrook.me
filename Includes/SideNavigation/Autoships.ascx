<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Autoships.ascx.cs" Inherits="Includes_SideNavigation_Autoships" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <%--<li data-key="forecast"><a href="AutoshipForecast.aspx">Autoship Forecast</a></li>--%>
    <li data-key="list"><a href="AutoshipList.aspx">My Autoships</a></li>
    <li data-key="cart"><a href="AutoshipCart.aspx">My Autoship Cart</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>