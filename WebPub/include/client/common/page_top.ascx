<%@ Control Language="C#" AutoEventWireup="true" CodeFile="page_top.ascx.cs" Inherits="include_client_common_page_top" %>
<div id="header">
    <div>
        <div class="logo"><a href="<%=Seec.Marketing.SystemDefine.HomePageUrl %>">士林電機營銷系統</a></div>
        <div class="mem"><asp:Literal ID="litActor" runat="server" />，您好<a href="<%=Seec.Marketing.SystemDefine.HttpsWebSiteRoot %>logout.aspx">登出</a></div>
    </div>
</div>

<div id="Menu">
    <asp:Literal ID="litMenu" runat="server" />
</div>
