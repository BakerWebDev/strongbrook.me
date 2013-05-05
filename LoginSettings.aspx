<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="LoginSettings.aspx.cs" Inherits="LoginSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'myaccount'
        };
    </script>


    <script src="Assets/Scripts/exigo.forms.js"></script>
    <script src="Assets/Scripts/exigo.errors.js"></script>
    <script>
        $(function () {
            Exigo.Forms.restrictInput($('#txtNewUsername, #txtNewPassword, #txtConfirmNewPassword'), '^[a-zA-Z0-9_\-]+$');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="login" runat="server" />
    </div>
    <div class="maincontent">


        <exigo:ErrorManager ID="Error" runat="server" />


        <h2>Login</h2>
        <div class="well well-large well-white">

            <h2>Current Settings</h2>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtNewUsername">Username:</label></span>
                <span class="span9">
                    <%=Identity.Current.Website.LoginName %></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtOldPassword">Password:</label></span>
                <span class="span9">********</span>
            </div>

            <br />
            <br />

            <section class="contentsection">
                <h2 style="margin-bottom:0px;">Change Username</h2>
                <span>
                    Please select a username. This username will be the name for your replicated Strongbrook website, and will serve as the login name for your Member Portal. Your website address will be in the following format 
                    www.username.strongbrook.com. Your username cannot contain any blank spaces or special characters (_,/,\,',;,",-,!,@,#,$,%,etc.). Enter your desired username in the space below.
                </span><br /><br />
                <div class="row-fluid">
                    <span class="span3">
                        <label for="txtNewUsername">* Desired Username:</label></span>
                    <span class="span9">
                        <asp:TextBox ID="txtNewUsername" ClientIDMode="Static" runat="server" /></span>
                </div>
                <div class="row-fluid formactions">
                    <span class="span3"></span>
                    <span class="span9">
                        <asp:LinkButton ID="cmdSubmitLoginNameChanges" Text="Save" CssClass="btn btn-success" OnClick="SubmitLoginNameChanges_Click" runat="server" />
                </div>
            </section>

            <br />
            <br />

            <section class="contentsection">
                <h2>Change Password</h2>
                <div class="row-fluid">
                    <span class="span3">
                        <label for="txtOldPassword">* Old Password:</label></span>
                    <span class="span9">
                        <asp:TextBox ID="txtOldPassword" ClientIDMode="Static" runat="server" /></span>
                </div>
                <div class="row-fluid">
                    <span class="span3">
                        <label for="txtNewPassword">* New Password:</label></span>
                    <span class="span9">
                        <asp:TextBox ID="txtNewPassword" ClientIDMode="Static" runat="server" /></span>
                </div>
                <div class="row-fluid">
                    <span class="span3">
                        <label for="txtConfirmNewPassword">* Retype Password:</label></span>
                    <span class="span9">
                        <asp:TextBox ID="txtConfirmNewPassword" ClientIDMode="Static" runat="server" /></span>
                </div>
                <div class="row-fluid formactions">
                    <span class="span3"></span>
                    <span class="span9">
                        <asp:LinkButton ID="cmdSubmitPasswordChanges" Text="Save" CssClass="btn btn-success" OnClick="SubmitPasswordChanges_Click" runat="server" />
                </div>
            </section>
        </div>
    </div>
</asp:Content>

