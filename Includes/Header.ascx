<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Includes_Header" %>

<div id="header-wrapper">
    <img alt="Strongbrook" src="Assets/Images/Header/header-bkg.png" />
    <div id="header-container">
        <div id="header-member-info">
            <%if(Identity.Current != null) { %>
            <%=FullName %>
            <% } %>
            <asp:LinkButton ID="cmdLogout" runat="server" OnClick="Click_Logout" Text="Logout" />
        </div>
    </div>
</div>
