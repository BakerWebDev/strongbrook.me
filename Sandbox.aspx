<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Sandbox.aspx.cs" Inherits="Sandbox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/commissions.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Sandbox</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="personallyenrolled" runat="server" />
    </div>

    <div class="maincontent">


                <div id="Row_2">












                    <a id="Sandbox">
                        <div class="tile theme-lightblue" title="Definition">
                            <h2></h2>
                            <br />
                            <h2><% ArrayTest1(); %></h2>
                        </div>
                    </a>
















                </div>

    </div>
</asp:Content>

