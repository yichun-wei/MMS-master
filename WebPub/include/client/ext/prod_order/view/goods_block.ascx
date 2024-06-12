<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_ext_prod_order_view_goods_block" %>

<%@ Reference Control="~/include/client/ext/prod_order/view/goods_item_block.ascx" %>

<div class="ListTable_title"><asp:Literal ID="litTitle" runat="server" /></div>
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
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
