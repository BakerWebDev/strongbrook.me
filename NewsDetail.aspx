<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="NewsDetail.aspx.cs" Inherits="NewsDetailPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'home'
        };
    </script>
    <link href="Assets/Styles/news.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1><%=GlobalSettings.Company.Name %> News</h1>

    <div class="well well-large well-white">
        <div id="news">
            <% RenderCompanyNewsDetail(); %>
        </div>
    </div>
</asp:Content>

