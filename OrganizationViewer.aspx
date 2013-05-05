<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="OrganizationViewer.aspx.cs" Inherits="OrganizationViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <link href="Assets/Plugins/jquery.jOrgChart/jquery.jOrgChart.min.css" rel="stylesheet" />
    <script src="Assets/Plugins/jquery.jOrgChart/jquery.jOrgChart.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };

        $(function () {
            $("#org").jOrgChart({
                chartElement: '#chart'
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="organizationviewer" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Organization Viewer</h2>

        <div class="alert alert-danger">
            <strong>DEVELOPER'S NOTE</strong><br />
            This report is not using live data. This is a demonstration of a UI concept for a visual organization report. You may take this UI and customize it as you see fit, then hook it up to the Exigo API if you'd like to use it.
        </div>

        <div class="btn-toolbar">
            <a href="javascript:;" class="btn"><i class="icon-plus"></i>&nbsp;Enroll new</a>
        </div>

        <div class="well well-white" style="overflow: scroll">


            <ul id="org" style="display: none">
                <li>
                    <img src="http://placehold.it/100x100" class="avatar" />
                    Strongbrook
                    <ul>
                        <li>
                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                        <li>
                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                            <ul>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                            <ul>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                                    <ul>
                                        <li>
                                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                    </ul>
                                </li>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                                    <ul>
                                        <li>
                                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                        <li>
                                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                        <li>
                                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li>
                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                        <li>
                            <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %>
                            <ul>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                                <li>
                                    <img src="http://placehold.it/100x100" class="avatar" /><%=Greeking.FullName %></li>
                            </ul>
                        </li>
                    </ul>
                </li>
            </ul>

            <div id="chart" class="orgChart"></div>


        </div>
    </div>
</asp:Content>

