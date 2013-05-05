<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="AutoshipForecast.aspx.cs" Inherits="AutoshipForecast" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'autoships'
        };
    </script>

    <link href="Assets/Styles/autoshipforecast.min.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1><%=Resources.AutoshipForecast.Autoships %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="forecast" runat="server" />
    </div>
    <div class="maincontent">
        <h2><%=Resources.AutoshipForecast.BinaryAutoshipForecast %>
                    <% if(LegFilter == LegFilterType.All)
                       { %> - <%=Resources.AutoshipForecast.AllLegs %> <% } %>
            <% if(LegFilter == LegFilterType.Left)
               { %> - <%=Resources.AutoshipForecast.LeftLegOnly %> <% } %>
            <% if(LegFilter == LegFilterType.Right)
               { %> - <%=Resources.AutoshipForecast.RightLegOnly %> <% } %>
        </h2>
        <div class="well well-large well-white">
            <div id="autoshipforecast">

                <label for="lstLegFilter"><%=Resources.AutoshipForecast.FilterByLeg %>: </label>
                <asp:DropDownList ID="lstLegFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ChangeLegFilter_SelectedIndexChanged" ClientIDMode="Static" />
                <div class="well well-large well-white">
                    <% RenderAllProgressBars(); %>
                </div>

                <div class="clearfix"></div>

            </div>
        </div>
    </div>
</asp:Content>
