<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_dom_pg_order_view_goods_block" %>

<%@ Reference Control="~/include/client/dom/pg_order/view/goods_item_block.ascx" %>

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
        <th>數量<asp:PlaceHolder ID="phMaxQty" Visible="false" runat="server">/最大值</asp:PlaceHolder>
        </th>
    </tr>
    <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
</table>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
