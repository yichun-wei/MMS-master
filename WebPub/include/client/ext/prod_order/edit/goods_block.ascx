<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_ext_prod_order_edit_goods_block" %>

<%@ Reference Control="~/include/client/ext/prod_order/edit/goods_item_block.ascx" %>

<asp:Panel ID="divGoodsBlock" runat="server">
    <div class="ListTable_title">
        <asp:Literal ID="litTitle" runat="server" />
    </div>
    <table class="ListTable">
        <tr>
            <th>序號</th>
            <th>製造部門</th>
            <th>料號</th>
            <th>需求量</th>
            <th>累計生產量</th>
            <th>庫存量</th>
            <th>生產量</th>
            <th>預計繳庫日</th>
            <th>備註</th>
        </tr>
        <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
    </table>

    <asp:Panel ID="InitVals" CssClass="dev-goods-block-pg-attr" runat="server" />
</asp:Panel>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
