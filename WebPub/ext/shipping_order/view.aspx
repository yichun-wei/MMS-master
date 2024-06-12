<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="ext_shipping_order_view" %>

<%@ Reference Control="~/include/client/ext/shipping_order/view/goods_block.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />

    <script type="text/javascript">
        $(function () {
            $("input.checkin").datepicker({
                minDate: 'today'
            });
            $("input.checkout").datepicker({
                minDate: 'today + 1'
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷出貨單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">出貨單編號：</th>
                    <td width="370"><asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th width="140">出貨單狀態：</th>
                    <td><asp:Literal ID="litStatus" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP訂單編號：</th>
                    <td><asp:Literal ID="litErpOrderNumber" runat="server" /></td>
                    <th>訂單日期：</th>
                    <td><asp:Literal ID="litCdt" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP訂單狀態：</th>
                    <td><asp:Literal ID="litErpStatus" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td><asp:Literal ID="litEdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>預計到港日：</th>
                    <td><asp:TextBox ID="txtEta" CssClass="checkin" runat="server" /><asp:Literal ID="litEta" Visible="false" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td><asp:Literal ID="litCdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td><asp:Literal ID="litCustomerName" runat="server" /></td>
                    <th>幣別：</th>
                    <td><asp:Label ID="lblCurrencyCode" runat="server" /></td>
                </tr>
                <tr>
                    <th>選擇訂單：</th>
                    <td>
                        <ul class="keyword_list">
                            <asp:Literal ID="litExtOrderIdxes" runat="server" />
                        </ul>
                    </td>
                    <th>表頭折扣：</th>
                    <td class="discount">
                        <asp:Literal ID="litHeaderDiscountList" runat="server" />
                    </td>
                </tr>
            </table>

            <%--連動幣別--%>
            <script type="text/javascript">
                function resetCurrencyCode() {
                    var currencyCode = $("#<%=this.lblCurrencyCode.ClientID%>").html();
                    $(".dev-currency-code-disp").each(function () {
                        $(this).html(currencyCode);
                    });
                }
            </script>

            <asp:PlaceHolder ID="phUpdateErpOrder" Visible="false" runat="server">
                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnUpdateErpOrder" Text="狀態更新" runat="server" OnClick="btnUpdateErpOrder_Click" /></div>
                    </div>
                </div>
            </asp:PlaceHolder>

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
            <asp:PlaceHolder ID="phGoodsList" runat="server" />

            <table class="form totel">
                <%--外銷出貨單不需要看折扣的金額--%>
                <tr style="display: none;">
                    <th>總金額：</th>
                    <td><asp:Label ID="lblTotalAmtDisp" Text="0" runat="server" /></td>
                </tr>
                <tr style="display: none;">
                    <th>折扣金額：</th>
                    <td><asp:Label ID="lblDctAmtDisp" Text="0" runat="server" /></td>
                </tr>
                <tr>
                    <th><%--折扣後總金額--%>總金額：</th>
                    <td><span class="total_price"><asp:Label ID="lblDctTotalAmtDisp" Text="0" CssClass="total_price" runat="server" /></span></td>
                </tr>
            </table>
            <div class="btnBox">
                <div>
                    <div><asp:LinkButton ID="btnSave" Text="儲存" runat="server" OnClick="btnSave_Click" /></div>
                    <div><asp:LinkButton ID="btnConfirmShipment" Text="確認出貨" Visible="false" runat="server" OnClick="btnConfirmShipment_Click" /></div>
                    <div><asp:LinkButton ID="btnToErp" Text="上傳至 ERP" Visible="false" runat="server" OnClick="btnToErp_Click" /></div>
                    <div><a href="javascript:void(0);" onclick="return printShipping();" onkeypress="return printShipping();">列印</a></div>
                    <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        function printShipping() {
            window.open("../../popup/ext/print/shipping.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }
    </script>
</asp:Content>

