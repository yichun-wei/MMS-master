<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_ext_order_edit_manual_goods_item_block" %>
<tr id="htmlTr" runat="server">
    <td class="no"><asp:Literal ID="litSeqNo" runat="server" /></td>
    <td class="textBox"><asp:Literal ID="litModel" runat="server" /></td>
    <td><asp:Literal ID="litPartNo" runat="server" /></td>
    <td class="number"><asp:Label ID="lblQty" CssClass="dev-goods-qty" runat="server" /></td>
    <td><asp:Label ID="lblUnitPrice" CssClass="dev-goods-unit-price" runat="server" /></td>
    <td class="discount"><asp:Label ID="lblDiscount" CssClass="dev-goods-discount" runat="server" />%</td>
    <td class="remark"><asp:Label ID="lblRmk" CssClass="truncate" length="6" runat="server" /></td>
    <td><asp:Label ID="lblPaidAmt" CssClass="dev-goods-paid-amt" runat="server" /></td>
    <td class="add">
        <asp:HyperLink ID="lnkSearchMapping" NavigateUrl="javascript:;" Text="選擇" runat="server" />
        <asp:Button ID="btnSearchMapping" CssClass="display_none dev-goods-item-search-mapping" runat="server" OnClick="btnSearchMapping_Click" />
        <asp:Panel ID="InitVals" CssClass="dev-goods-item-pg-attr" runat="server" />
    </td>
</tr>
