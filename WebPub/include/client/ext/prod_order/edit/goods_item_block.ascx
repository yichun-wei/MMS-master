﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_item_block.ascx.cs" Inherits="include_client_ext_prod_order_edit_goods_item_block" %>
<tr id="htmlTr" runat="server">
    <td class="no"><asp:Literal ID="litSeqNo" runat="server" /></td>
    <td><asp:Literal ID="litOrgCode" runat="server" /></td>
    <td><asp:Literal ID="litPartNo" runat="server" /></td>
    <td><asp:Label ID="lblQty" CssClass="dev-goods-qty" runat="server" /></td>
    <td><asp:Literal ID="litCumProdQty" runat="server" /></td>
    <td><asp:Label ID="lblErpOnHand" CssClass="dev-goods-erp-on-hand" runat="server" /></td>
    <td class="number"><asp:TextBox ID="txtProdQty" CssClass="dev-prod-goods-qty" MaxLength="6" runat="server" /></td>
    <td class="data"><asp:TextBox ID="txtEstFpmsDate" CssClass="checkin dev-est-fpms-date" runat="server" /></td>
    <td class="textBox"><asp:Label ID="lblRmk" CssClass="truncate" length="6" runat="server" /><asp:Panel ID="InitVals" CssClass="dev-goods-item-pg-attr" runat="server" /></td>
</tr>
