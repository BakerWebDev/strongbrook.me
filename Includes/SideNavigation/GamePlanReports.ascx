<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GamePlanReports.ascx.cs" Inherits="Includes_SideNavigation_GamePlanReports" %>

<ul class="nav nav-pills nav-stacked sidenavigation">

    <li data-key="leads"><a href="GPR_LeadManager.aspx">Personal GPR Leads</a></li>
    <li data-key="weekly"><a href="GPR_WeeklyDetails.aspx">GPR Weekly Report</a></li>
    <li data-key="monthly"><a href="GPR_MonthlyDetails.aspx">GPR Monthly Report</a></li>

</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>