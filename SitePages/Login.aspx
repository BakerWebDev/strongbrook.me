<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" MasterPageFile="~/MasterPages/Public.master" %>



<asp:Content ID="Head1" runat="server" ContentPlaceHolderID="Head">
    <style>

    </style>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Content">



    <div class="row-fluid">
        <span class="span4 offset4">

            <h1>Sign In</h1>
            <div class="loginwrapper well">

                <%=ErrorString %>

                <div class="row-fluid">
                    <span class="span12">
                        <label for="txtLoginName">Username</label>
                        <asp:TextBox ID="txtLoginName" runat="server" CssClass="span12" placeholder="Username" />
                        <asp:RequiredFieldValidator ID="rtxtLoginName" ControlToValidate="txtLoginName" ErrorMessage="Required"
                            Display="Dynamic" SetFocusOnError="true" runat="server" />
                    </span>
                </div>

                <div class="row-fluid">
                    <span class="span12">
                        <label for="txtPassword">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="span12" placeholder="Password" />
                        <asp:RequiredFieldValidator ID="rtxtPassword" ControlToValidate="txtPassword" ErrorMessage="Required"
                            Display="Dynamic" SetFocusOnError="true" runat="server" />
                    </span>
                </div>

                <div class="row-fluid">
                    <span class="span12 checkbox">
                        <asp:CheckBox ID="chkRememberMe" runat="server" Text="Remember me on this computer" />
                    </span>
                </div>

                <div class="row-fluid">
                    <span class="span12">
                        <asp:LinkButton ID="cmdSignIn" runat="server" CssClass="btn btn-primary btn-large btn-block" OnClick="SignIn_Click"><i class="icon-lock icon-white"></i> Sign in</asp:LinkButton>
                    </span>
                </div>
            </div>

        </span>
    </div>
</asp:Content>
