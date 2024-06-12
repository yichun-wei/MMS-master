<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_det_item_block.ascx.cs" Inherits="include_client_ext_shipping_order_edit_2_goods_det_item_block" %>

<asp:Panel ID="divDetItemBlock" runat="server">
<span class="o_no"><asp:Literal ID="litOdrNo" runat="server" />：</span><asp:TextBox ID="txtQty" CssClass="dev-goods-qty" MaxLength="6" runat="server" /><span class="max"> / <asp:Label ID="lblMaxQty" CssClass="dev-goods-max-qty" runat="server" /></span>
</asp:Panel>
