<%@ Control Language="C#" AutoEventWireup="true" CodeFile="goods_block.ascx.cs" Inherits="include_client_ext_quotn_readjust_goods_block" %>

<%@ Reference Control="~/include/client/ext/quotn/readjust/goods_item_block.ascx" %>

<asp:Panel ID="divGoodsBlock" runat="server">
    <div class="ListTable_title">
        <asp:Literal ID="litTitle" runat="server" />
    </div>
    <table class="ListTable">
        <tr>
            <th>序號</th>
            <th>選擇</th>
            <th>型號</th>
            <th>料號</th>
            <th>數量</th>
            <th>單價</th>
            <th>折扣</th>
            <th>備註</th>
            <th>小計 (<span class="dev-currency-code-disp"></span>) </th>
            <th>編輯</th>
        </tr>
        <asp:PlaceHolder ID="phGoodsItemList" runat="server" />
        <tr class="dev-input-add-container" style="display: none;">
            <td class="no"></td>
            <td></td>
            <td></td>
            <td class="input_name">
                <div class="dev-ac-goods-keyword-container">
                    <Ajax:AutoCompleteExtender ID="acGoodsKeyword" TargetControlID="txtGoodsKeyword" MinimumPrefixLength="1" CompletionSetCount="10" CompletionInterval="500" ServicePath="~/net_talk/ws/auto_complete.asmx" ServiceMethod="GetExtGoodsList" UseContextKey="true" EnableCaching="false" runat="server" />
                    <asp:TextBox ID="txtGoodsKeyword" CssClass="dev-ac-goods-keyword" runat="server" />
                </div>
                <img src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/common/images/loader.gif" alt="載入中" class="dev-ac-goods-item-loading" style="display: none;" />
            </td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td colspan="10" class="add_btn">
                <asp:HyperLink ID="lnkSearchAdd" NavigateUrl="javascript:;" Text="搜尋新增" runat="server" />
                <asp:Button ID="btnSearchAdd" CssClass="display_none dev-goods-block-search-add" runat="server" OnClick="btnSearchAdd_Click" />
                <asp:HyperLink ID="lnkInputAdd" NavigateUrl="javascript:;" Text="手動新增" runat="server" />
            </td>
        </tr>
        <tr class="last">
            <td class="no">合計</td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td><asp:Label ID="lblSubtotal" CssClass="dev-goods-block-subtotal-disp" runat="server" /></td>
            <td></td>
        </tr>
    </table>

    <div class="dev-goods-block-auto-complete-id">
        <asp:HiddenField ID="hidAutoCompleteClientID" runat="server" />
    </div>
    <div class="dev-goods-block-existed-goods-items">
        <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />
    </div>
    <div class="dev-goods-block-subtotal">
        <asp:HiddenField ID="hidSubtotal" runat="server" />
    </div>
    <asp:Panel ID="InitVals" CssClass="dev-goods-block-pg-attr" runat="server" />
</asp:Panel>
<div class="btn_top"><a href="javascript:void(0)">top</a></div>
