<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="ext_quotn_view" %>

<%@ Register TagPrefix="UC" TagName="GoodsBlock" Src="~/include/client/ext/quotn/view/goods_block.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷報價單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">報價單編號：</th>
                    <td width="370"><asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th width="140">報價單日期：</th>
                    <td><asp:Literal ID="litQuotnDate" runat="server" /></td>
                </tr>
                <tr>
                    <th>報價單狀態：</th>
                    <td><asp:Literal ID="litStatus" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td><asp:Literal ID="litEdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>建單人：</th>
                    <td><asp:Literal ID="litCName" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td><asp:Literal ID="litCdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td><asp:Literal ID="litCustomerName" runat="server" /></td>
                    <th>幣別：</th>
                    <td><asp:Label ID="lblCurrencyCode" CssClass="dev-currency-code" runat="server" /></td>
                </tr>
                <tr>
                    <th>備註：</th>
                    <td><asp:Literal ID="litRmk" runat="server" /></td>
                </tr>
            </table>

            <%--連動幣別--%>
            <script type="text/javascript">
                function resetCurrencyCode() {
                    var currencyCode = $(".dev-currency-code").html();
                    $(".dev-currency-code-disp").each(function () {
                        $(this).html(currencyCode);
                    });
                }
            </script>

            <div class="form_title">客戶資訊</div>
            <div class="formBox">
                <table class="form">
                    <tr>
                        <th width="140">客戶編號：</th>
                        <td width="370"><asp:Literal ID="litCustomerNumber" runat="server" /></td>
                        <th width="140">地址：</th>
                        <td><asp:Literal ID="litCustomerAddr" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>客戶名稱：</th>
                        <td><asp:Literal ID="litCustomerName2" runat="server" /></td>
                        <th>TEL：</th>
                        <td><asp:Literal ID="litCustomerTel" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>聯絡人：</th>
                        <td><asp:Literal ID="litCustomerConName" runat="server" /></td>
                        <th>FAX：</th>
                        <td><asp:Literal ID="litCustomerFax" runat="server" /></td>
                    </tr>
                </table>
            </div>
            <div class="form_title">收貨人資訊</div>
            <div class="formBox">
                <table class="form">
                    <tr>
                        <th width="140">收貨人：</th>
                        <td width="370"><asp:Literal ID="litRcptName" runat="server" /></td>
                        <th width="140">貨運方式：</th>
                        <td><asp:Literal ID="litFreightWay" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>客戶名稱：</th>
                        <td><asp:Literal ID="litRcptCusterName" runat="server" /></td>
                        <th>地址：</th>
                        <td><asp:Literal ID="litRcptAddr" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>TEL：</th>
                        <td><asp:Literal ID="litRcptTel" runat="server" /></td>
                        <th>FAX：</th>
                        <td><asp:Literal ID="litRcptFax" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="rondane_box">
            <%--一般品項--%>
            <UC:GoodsBlock ID="ucGeneralGoods" Title="一般訂單" runat="server" />

            <%--手動新增--%>
            <UC:GoodsBlock ID="ucManualGoods" Title="手動新增" runat="server" />

            <table class="form totel">
                <tr>
                    <th>總金額：</th>
                    <td><asp:Label ID="lblTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />(未稅)</td>
                </tr>
            </table>
            <div class="btnBox">
                <div>
                    <div><asp:LinkButton ID="btnCopyOrder" Text="複製" runat="server" OnClick="btnCopyOrder_Click" /></div>
                    <div><asp:LinkButton ID="btnReadjust" Text="修改" Visible="false" runat="server" OnClick="btnReadjust_Click" /></div>
                    <div><a href="javascript:void(0);" onclick="return printQuotn();" onkeypress="return printQuotn();">列印</a></div>
                    <div><asp:LinkButton ID="btnCancel" Text="取消" Visible="false" runat="server" OnClick="btnCancel_Click" /></div>
                    <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        function printQuotn() {
            window.open("../../popup/ext/print/quotn.aspx?quotnSId=".concat($.getUrlVar('quotnSId')));
            return false;
        }
    </script>
</asp:Content>

