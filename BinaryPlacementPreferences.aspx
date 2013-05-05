<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="BinaryPlacementPreferences.aspx.cs" Inherits="BinaryPlacementPreferences" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'myaccount'
        };
    </script>

    <script src="Assets/Scripts/exigo.errors.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="placement" runat="server" />
    </div>
    <div class="maincontent">

        <exigo:ErrorManager ID="Error" runat="server" />

        <h2>Placement Preferences</h2>
        <div class="well well-large well-white">

            <div class="row-fluid">
                <span class="span3">Placement Preference</span>
                <span class="span9">
                    <asp:RadioButtonList ID="rdoPlacementPreference" runat="server" ClientIDMode="Static" CssClass="radio" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="offset3 span9">
                    <asp:LinkButton ID="cmdSaveChanges" runat="server" Text="Save Changes" OnClick="SaveChanges_Click" CssClass="btn btn-success" ClientIDMode="Static" />
                </span>
            </div>

        </div>
    </div>
</asp:Content>

