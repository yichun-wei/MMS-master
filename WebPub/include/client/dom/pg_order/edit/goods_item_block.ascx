<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_dom_pg_order_edit_goods_item_block" %>
<tr id="htmlTr" runat="server">
    <td class="no">
        <asp:Literal ID="litSeqNo" runat="server" /></td>
    <td>
        <asp:Literal ID="litSummary" runat="server" /></td>
    <td>
        <asp:Literal ID="litPartNo" runat="server" /></td>
    <td class="remark">
        <asp:TextBox ID="txtRmk" runat="server" /></td>
    <td class="quantity">
        <div>
            <Ajax:FilteredTextBoxExtender TargetControlID="txtQty" FilterType="Numbers" runat="server" />
            <asp:TextBox ID="txtQty" CssClass="dev-goods-qty" MaxLength="6" runat="server" />
            <asp:Label ID="lblMaxQtyContainer" CssClass="max" runat="server">/
                <asp:Label ID="lblMaxQty" CssClass="dev-goods-max-qty" runat="server" /></asp:Label>
        </div>
    </td>
    <td class="del">
        <asp:LinkButton ID="btnRemove" Text="刪除" runat="server" OnClick="btnRemove_Click" />
        <asp:Panel ID="InitVals" CssClass="dev-goods-item-pg-attr" runat="server" />
    </td>
</tr>
