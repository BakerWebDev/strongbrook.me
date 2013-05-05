<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Organization.ascx.cs" Inherits="SitePage_Includes_SideNavigation_Organization" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <li data-key="organizationexplorer"><a href="../OrganizationExplorerTemp.aspx">Organization Explorer</a></li>
    <li data-key="organizationdetails"><a href="../OrganizationDetails.aspx">Organization Details</a></li>
    <li data-key="unilevelwaitingroom"><a href="../UnilevelWaitingRoom.aspx">Unilevel Waiting Room</a></li>
    
    <li data-key="personallyenrolled"><a href="../PersonallyEnrolledTeam.aspx">Personally Enrolled Team</a></li>
    <li data-key="retailcustomers"><a href="../RetailCustomers.aspx">Clients / Customers</a></li>
    <li data-key="leads"><a href="../Leads.aspx">Leads</a></li>

    <li data-key="leads"><a href="GPRLeadManager.aspx">Game Plan Report Leads</a></li>
    <li data-key="leads"><a href="GPR_WeeklyDetails.aspx">Game Plan Report Weekly</a></li>
    <li data-key="leads"><a href="GPR_MonthlyDetails.aspx">Game Plan Report Monthly</a></li>

</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>