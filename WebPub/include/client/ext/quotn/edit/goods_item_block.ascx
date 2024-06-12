<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_ext_quotn_edit_goods_item_block" %>

<tr id="htmlTr" runat="server">
    <td class="no">
        <asp:Literal ID="litSeqNo" runat="server" /></td>
    <td class="no"><asp:CheckBox ID="chkSel" runat="server" /></td>
    <td class="textBox"><asp:Literal ID="litModel" runat="server" /></td>
    <td>
        <asp:Literal ID="litPartNo" runat="server" /></td>
    <td class="number">
        <Ajax:FilteredTextBoxExtender TargetControlID="txtQty" FilterType="Numbers" runat="server" />
        <asp:TextBox ID="txtQty" CssClass="dev-goods-qty" MaxLength="6" runat="server" /></td>
    <td>
        <asp:TextBox ID="txtUnitPrice" CssClass="dev-goods-unit-price" MaxLength="12" onkeyup="ValidateFloat4(this,value);" runat="server" /></td>
    <td class="discount">
        <asp:TextBox ID="txtDiscount" CssClass="dev-goods-discount" MaxLength="6" runat="server" />%</td>
    <td class="remark">
        <asp:TextBox ID="txtRmk" runat="server" /></td>
    <td>
        <asp:Label ID="lblPaidAmtDisp" CssClass="dev-goods-paid-amt-disp" runat="server" />
        <div class="dev-goods-paid-amt">
            <asp:HiddenField ID="hidPaidAmt" runat="server" />
        </div>
    </td>
    <td class="del">
        <asp:LinkButton ID="btnRemove" Text="刪除" runat="server" OnClick="btnRemove_Click" />
        <asp:Panel ID="InitVals" CssClass="dev-goods-item-pg-attr" runat="server" />
    </td>
</tr>
