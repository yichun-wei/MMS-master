<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_ext_shipping_order_edit_goods_block" %>

<%@ Reference Control="~/include/client/ext/shipping_order/edit/goods_item_block.ascx" %>

<asp:Panel ID="divGoodsBlock" runat="server">
    <asp:Literal ID="litAnchor" runat="server" />
    <div class="ListTable_title"><asp:Literal ID="litTitle" runat="server" /></div>
    <table class="ListTable">
        <tr>
            <th>序號</th>
            <th>型號</th>
            <th>料號</th>
            <th>數量/最大值</th>
            <th>單價</th>
            <th>折扣後單價</th>
            <th style="display: none;">折扣</th>
            <th>備註</th>
            <th>小計 (<span class="dev-currency-code-disp"></span>) </th>
            <th>編輯</th>
        </tr>
        <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
        <tr>
            <td class="no"></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td style="display: none;"></td>
            <td></td>
            <td></td>
            <td></td>
            <td class="add">
                <asp:HyperLink ID="lnkSearchAdd" NavigateUrl="javascript:;" Text="新增" runat="server" />
                <asp:Button ID="btnSearchAdd" CssClass="display_none dev-goods-block-search-add" runat="server" OnClick="btnSearchAdd_Click" />
            </td>
        </tr>
        <tr class="last">
            <td class="no">合計</td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td style="display: none;"></td>
            <td></td>
            <td></td>
            <td>
                <asp:Label ID="lblSubtotal" CssClass="dev-goods-block-subtotal-disp" runat="server" /></td>
            <td></td>
        </tr>
    </table>

    <div class="dev-goods-block-existed-goods-items">
        <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />
    </div>
    <div class="dev-goods-block-subtotal">
        <asp:HiddenField ID="hidSubtotal" runat="server" />
    </div>
    <asp:Panel ID="InitVals" CssClass="dev-goods-block-pg-attr" runat="server" />
</asp:Panel>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
