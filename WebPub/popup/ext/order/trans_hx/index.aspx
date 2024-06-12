<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="popup_ext_order_trans_hx_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="popup">
        <div class="ListTable_title">修改歷程</div>
        <asp:PlaceHolder ID="phPageList" runat="server" />
    </div>
</asp:Content>

