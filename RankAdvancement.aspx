<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="RankAdvancement.aspx.cs" Inherits="RankAdvancement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/ranks.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.rankadvancement.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'ranks'
        };

        $(function () {
            rankadvancement.settings.requesturl = "<%=Request.Url.AbsolutePath %>";
            rankadvancement.init();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Ranks</h1>

    <div class="sidebar">
        <ul id="ranks" class="nav nav-pills nav-stacked">
            <% foreach(var rank in Ranks)
               { %>
            <li <%=(rank.RankID == Identity.Current.Ranks.CurrentPeriodRankID) ? "class='active'" : string.Empty %>><a href="javascript:;" data-rank="<%=rank.RankID %>"><%=rank.RankDescription %></a></li>
            <% } %>
        </ul>
    </div>
    <div class="maincontent">
        <h2>Rank Promotion Status</h2>
        <div class="well well-large well-white">
            <div id="rankadvancement">
                <!-- Loaded dynamically -->
            </div>
            <div class="clearfix"></div>
        </div>
    </div>
</asp:Content>

