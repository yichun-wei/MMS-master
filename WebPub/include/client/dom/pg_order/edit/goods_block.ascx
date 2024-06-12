<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_dom_pg_order_edit_goods_block" %>

<%@ Reference Control="~/include/client/dom/pg_order/edit/goods_item_block.ascx" %>

<asp:Panel ID="divGoodsBlock" runat="server">
    <asp:Literal ID="litAnchor" runat="server" />
    <div class="ListTable_title">
        <asp:Literal ID="litTitle" runat="server" />
    </div>
    <table class="ListTable">
        <tr>
            <th>序號</th>
            <th>摘要</th>
            <th>料號</th>
            <th>備註</th>
            <th>
                數量<asp:PlaceHolder ID="phMaxQty" Visible="false" runat="server">/最大值</asp:PlaceHolder>
            </th>
            <th>編輯</th>
        </tr>
        <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
        <tr>
            <td class="no"></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td class="add">
                <asp:HyperLink ID="lnkAdd" NavigateUrl="javascript:;" Text="新增" runat="server" />
                <asp:Button ID="btnAdd" CssClass="display_none dev-goods-block-add" runat="server" OnClick="btnAdd_Click" />
            </td>
        </tr>
    </table>

    <div class="dev-goods-block-existed-goods-items">
        <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />
    </div>
    <asp:Panel ID="InitVals" CssClass="dev-goods-block-pg-attr" runat="server" />
</asp:Panel>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
