<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="WebsiteSettings.aspx.cs" Inherits="WebsiteSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'myaccount'
        };
    </script>


    <script src="Assets/Scripts/exigo.forms.js"></script>
    <script src="Assets/Scripts/exigo.errors.js"></script>
    <script src="Assets/Plugins/tinymce/jscripts/tiny_mce/tiny_mce.js"></script>

    <script>
        $(function () {
            tinyMCE.init({
                // General options
                mode: "textareas",
                theme: "simple",
                editor_selector: "notes"
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="website" runat="server" />
    </div>
    <div class="maincontent">

        <exigo:ErrorManager ID="Error" runat="server" />

        <h2>Website Profile</h2>

        <div class="well well-large well-white">

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
                    <label for="txtCompany">Company:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtCompany" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtPhone">Home Phone:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtPhone" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtPhone2">Office Phone:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtPhone2" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtEmail">Email:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtEmail" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="lstCountry">Country:</label></span>
                <span class="span9">
                    <asp:DropDownList ID="lstCountry" AutoPostBack="true" OnSelectedIndexChanged="PopulateRegions_SelectedIndexChanged" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtAddress">Address:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtAddress" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtCity">City:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtCity" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="lstState">State/Region:</label></span>
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
                <span class="span9">
                    <asp:TextBox ID="txtZip" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid formactions">
                <span class="span3"></span>
                <span class="span9">
                    <asp:Button ID="cmdSubmit2" Text="Save" CssClass="btn btn-success" OnClick="Submit_Click" ClientIDMode="Static" runat="server" /></span>
            </div>
        </div>




        <div style="display:none">
        <h2>About Me</h2>
        <div class="well well-large well-white">
            <div class="row-fluid">
                <span class="span3">
                    <label for="uploadPhoto">Photo:</label></span>
                <span class="span9">
                    <asp:FileUpload ID="uploadPhoto" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtNotes">My Story:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtNotes" CssClass="notes span12" TextMode="MultiLine" ClientIDMode="Static" runat="server" /></span>
            </div>
            <div class="row-fluid formactions">
                <span class="span3"></span>
                <span class="span9">
                    <asp:Button ID="cmdSubmit3" Text="Save" CssClass="btn btn-success" OnClick="Submit_Click" ClientIDMode="Static" runat="server" /></span>
            </div>
        </div>
        <h2>Social Networking</h2>
        <div class="well well-large well-white">
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtFacebook">Facebook: </label>
                </span>
                <span class="span9">
                    <asp:TextBox ID="txtFacebook" ClientIDMode="Static" runat="server" placeholder="Username" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtTwitter">Twitter:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtTwitter" ClientIDMode="Static" runat="server" placeholder="Username" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtLinkedIn">LinkedIn:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtLinkedIn" ClientIDMode="Static" runat="server" placeholder="URL" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtYouTube">YouTube:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtYouTube" ClientIDMode="Static" runat="server" placeholder="Channel URL" /></span>
            </div>
            <div class="row-fluid">
                <span class="span3">
                    <label for="txtPinterest">Pinterest:</label></span>
                <span class="span9">
                    <asp:TextBox ID="txtPinterest" ClientIDMode="Static" runat="server" placeholder="Username" /></span>
            </div>
            <div class="row-fluid formactions">
                <span class="span3"></span>
                <span class="span9">
                    <asp:Button ID="cmdSubmit4" Text="Save" CssClass="btn btn-success" OnClick="Submit_Click" ClientIDMode="Static" runat="server" /></span>
            </div>
        </div>
        </div>




    </div>
</asp:Content>

