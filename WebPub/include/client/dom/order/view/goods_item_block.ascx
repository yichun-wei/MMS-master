<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_dom_order_view_goods_item_block" %>
<tr id="htmlTr" runat="server">
    <td class="textBox"><asp:Literal ID="litSource" runat="server" /></td>
    <td><asp:Literal ID="litPartNo" runat="server" /></td>
    <td><asp:Label ID="lblErpOnHand" CssClass="dev-goods-erp-on-hand" runat="server" /></td>
    <td>
        <asp:Literal ID="litQty" runat="server" /><asp:PlaceHolder ID="phMaxQtyContainer" runat="server">/
            <asp:Label ID="litMaxQty" runat="server" /></asp:PlaceHolder>
    </td>
    <td>
        <asp:Literal ID="litUnitPrice" runat="server" /></td>
    <td>
        <asp:Literal ID="litDiscount" runat="server" /></td>
    <td class="remark">
        <asp:Label ID="lblRmk" CssClass="truncate" length="6" runat="server" /></td>
    <td>
        <asp:Literal ID="litPaidAmt" runat="server" /></td>
</tr>
