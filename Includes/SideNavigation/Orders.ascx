<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Orders.ascx.cs" Inherits="Includes_SideNavigation_Orders" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="products"><a href="ShoppingProductList.aspx">Browse Products</a></li>
    <li data-key="cart"><a href="ShoppingCart.aspx">My Cart</a></li>
    <li data-key="orderhistory"><a href="OrderHistory.aspx">Order History</a></li>
    <li data-key="downlineorders"><a href="DownlineOrders.aspx">Downline Order History</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>