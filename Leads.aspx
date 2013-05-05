<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Leads.aspx.cs" Inherits="Leads" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Scripts/exigo.report.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };

        $(function () {
            // Report
            setInitialSort('IndentedSort', report.sortOrderTypes.ASCENDING);
            initializeReport();
        });
    </script>

    <script>

        function test() {
            <%=Page.ClientScript.GetPostBackEventReference(this, "EditAutoship") %>
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization id="SideNavigation" ActiveNavigation="leads" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Leads</h2>

<%--
        <div class="btn-toolbar">
            <% RenderCreatenewButton(); %>
        </div>
--%>

        <div class="well well-white list">
            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">

                        <th style="border-top:0px"><a class="sortable">Name</a></th>
                        <th style="border-top:0px">Contact Info</th>
                        <th style="border-top:0px"><a class="sortable">State</a></th>
                        <th style="border-top:0px" class="column-alignright">More Information(Coming Soon)</th>

                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

