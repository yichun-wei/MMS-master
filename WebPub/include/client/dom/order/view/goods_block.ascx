<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_dom_order_view_goods_block" %>

<%@ Reference Control="~/include/client/dom/order/view/goods_item_block.ascx" %>

<asp:Literal ID="litAnchor" runat="server" />
<div class="ListTable_title">
    <asp:Literal ID="litTitle" runat="server" />
</div>
<table class="ListTable">
    <tr>
        <th>來源</th>
        <th>料號</th>
        <th>庫存量</th>
        <th>數量/最大值</th>
        <th>單價</th>
        <th>折扣</th>
        <th>備註</th>
        <th>小計 (RMB) </th>
    </tr>
    <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
    <tr class="last">
        <td>合計</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>
            <asp:Label ID="lblSubtotal" CssClass="dev-goods-block-subtotal-disp" runat="server" /></td>
    </tr>
</table>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
