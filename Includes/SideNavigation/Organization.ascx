<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Organization.ascx.cs" Inherits="Includes_SideNavigation_Organization" %>

<ul class="nav nav-pills nav-stacked sidenavigation">
    <%--<li data-key="organizationviewer"><a href="OrganizationViewer.aspx">Organization Viewer</a></li>--%>
    <li data-key="organizationexplorer"><a href="OrganizationExplorerTemp.aspx">Organization Explorer</a></li>
    <li data-key="organizationdetails"><a href="OrganizationDetails.aspx">Organization Details</a></li>
    <li data-key="unilevelwaitingroom"><a href="UnilevelWaitingRoom.aspx">Unilevel Waiting Room</a></li>
    
    <%--<li data-key="downlineautoships"><a href="DownlineAutoships.aspx">Downline Autoships</a></li>--%>
    <li data-key="personallyenrolled"><a href="PersonallyEnrolledTeam.aspx">Personally Enrolled Team</a></li>
    <%--<li data-key="preferredcustomers"><a href="PreferredCustomers.aspx">Preferred Customers</a></li>--%>
    <li data-key="retailcustomers"><a href="RetailCustomers.aspx">Clients / Customers</a></li>
    <li data-key="leads"><a href="Leads.aspx">Lead Capture</a></li>

</ul>

<script>
    $('.sidenavigation li[data-key="<%=ActiveNavigation.ToLower() %>"]').addClass('active');
</script>