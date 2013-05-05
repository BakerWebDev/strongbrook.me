<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Events.ascx.cs" Inherits="Includes_SideNavigation_Events" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="calendar"><a href="Calendar.aspx">Calendar</a></li>
</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>