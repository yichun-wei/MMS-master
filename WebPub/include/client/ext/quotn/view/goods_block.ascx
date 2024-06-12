<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_ext_quotn_view_goods_block" %>

<%@ Reference Control="~/include/client/ext/quotn/view/goods_item_block.ascx" %>

<div class="ListTable_title"><asp:Literal ID="litTitle" runat="server" /></div>
<table class="ListTable">
    <tr>
        <th>序號</th>
        <th>型號</th>
        <th>料號</th>
        <th>數量</th>
        <th>單價</th>
        <th>折扣</th>
        <th>備註</th>
        <th>小計 (<span class="dev-currency-code-disp"></span>) </th>
    </tr>
    <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
    <tr class="last">
        <td class="no">合計</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td><asp:Label ID="lblSubtotal" CssClass="dev-goods-block-subtotal-disp" runat="server" /></td>
    </tr>
</table>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
