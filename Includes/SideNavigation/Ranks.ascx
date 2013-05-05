<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ranks.ascx.cs" Inherits="Includes_SideNavigation_Ranks" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="rankadvancement"><a href="RankAdvancement.aspx">Rank Promotion Status</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>