<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="PersonalSettings.aspx.cs" Inherits="PersonalSettings" %>

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
            exigo.forms.restrictInput($('#txtPhone, #txtPhone2, #txtMobilePhone'), '^[0-9/./\-]+$');
            exigo.forms.restrictInput($('#txtEmail'), '^[a-zA-Z0-9_/\-/\@/\.]+$');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="settings" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Settings</h2>
        <div class="well well-large well-white">


            <exigo:ErrorManager ID="Error" runat="server" />


            <div class="row-fluid">
                <span class="span3">
                    <label for="txtFirstName">First Name:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtFirstName" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtLastName">Last Name:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtLastName" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtLastName">Company:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtCompany" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtEmail">Email:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtEmail" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtPhone">Home Phone:</label></span>
                <span class="span9">
                    <span class="span5">
                        <asp:TextBox ID="txtPhone" ClientIDMode="Static" runat="server" /></span></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtMobilePhone">Mobile Phone:</label></span>
                <span class="span9">
                    <span class="span5">
                        <asp:TextBox ID="txtMobilePhone" ClientIDMode="Static" runat="server" /></span></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtPhone2">Office Phone:</label></span>
                <span class="span9">
                    <span class="span5">
                        <asp:TextBox ID="txtPhone2" ClientIDMode="Static" runat="server" /></span></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="lstBirthMonth">Date of Birth:</label></span>
                <span class="span9">
                    <asp:DropDownList ID="lstBirthMonth" CssClass="input-small" ClientIDMode="Static" runat="server" />
                    <asp:DropDownList ID="lstBirthDay" CssClass="input-mini" ClientIDMode="Static" runat="server" />
                    <asp:DropDownList ID="lstBirthYear" CssClass="input-small" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="lstCountry">Country:</label></span>
                <span class="span9">
                    <asp:DropDownList ID="lstCountry" ClientIDMode="Static" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PopulateRegions_SelectedIndexChanged" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtAddress1">Address 1:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtAddress1" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtAddress2">Address 2:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtAddress2" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtCity">City:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtCity" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="lstState">State:</label></span>
                <span class="span9">
                    <asp:UpdatePanel ID="Update_Address" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="lstState" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lstCountry" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtZip">Zip:</label></span>
                <span class="span9 ">
                    <span class="span5">
                        <asp:TextBox ID="txtZip" ClientIDMode="Static" runat="server" /></span></span>
            </div>
            <div class="row-fluid formactions">
                <span class="span3"></span>
                <span class="span9">
                    <asp:LinkButton ID="cmdSubmitChanges" Text="Save" CssClass="btn btn-success" OnClick="SubmitChanges_Click" runat="server" />
            </div>



        </div>
    </div>
</asp:Content>

