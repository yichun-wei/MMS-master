<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_dom_pg_order_view_goods_item_block" %>
<tr>
    <td class="no">
        <asp:Literal ID="litSeqNo" runat="server" /></td>
    <td>
        <asp:Literal ID="litSummary" runat="server" /></td>
    <td>
        <asp:Literal ID="litPartNo" runat="server" /></td>
    <td class="remark">
        <asp:Label ID="lblRmk" CssClass="truncate" length="6" runat="server" /></td>
    <td>
        <asp:Literal ID="litQty" runat="server" /><asp:PlaceHolder ID="phMaxQty" Visible="false" runat="server">/
            <asp:Literal ID="litMaxQty" runat="server" /></asp:PlaceHolder>
    </td>
</tr>
