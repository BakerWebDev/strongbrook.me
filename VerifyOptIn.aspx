<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VerifyOptIn.aspx.cs" Inherits="VerifyOptIn" MasterPageFile="~/MasterPages/Public.master" %>

<asp:Content ID="Head1" runat="server" ContentPlaceHolderID="Head">
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Content">
    <asp:Panel ID="Panel_Verified" runat="server">
        <h2>Email Communications</h2>
        <p>Thank you! Your email address '<%=Email %>' has been verified, and you have been successfully subscribed to email communications from Company Name.</p>

    </asp:Panel>
    <asp:Panel ID="Panel_AccessDenied" runat="server" Visible="false">
        <p>Your email could not be verified at this time. Please contact customer service.</p>
    </asp:Panel>
</asp:Content>
