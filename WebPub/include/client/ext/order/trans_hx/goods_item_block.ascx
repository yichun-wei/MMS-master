﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_ext_order_trans_hx_goods_item_block" %>
<tr id="htmlTr" runat="server">
    <td class="no"><asp:Literal ID="litSeqNo" runat="server" /></td>
    <td class="textBox"><asp:Literal ID="litModel" runat="server" /></td>
    <td><asp:Literal ID="litPartNo" runat="server" /></td>
    <td><asp:Literal ID="litQty" runat="server" /></td>
    <td><asp:Literal ID="litUnitPrice" runat="server" /></td>
    <td><asp:Literal ID="litDiscount" runat="server" /></td>
    <td class="textBox"><asp:Label ID="lblRmk" CssClass="truncate" length="6" runat="server" /></td>
    <td><asp:Literal ID="litPaidAmt" runat="server" /></td>
</tr>
