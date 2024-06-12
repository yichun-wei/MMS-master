<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="dom_order_shipping_view" %>

<%@ Reference Control="~/include/client/dom/order/view/goods_block.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
    <link href="../../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />

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
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" Runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>內銷出貨單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">地區：</th>
                    <td width="370">
                        <asp:Literal ID="litDomDistName" runat="server" /></td>
                    <th width="140">倉庫：</th>
                    <td>
                        <asp:Literal ID="litWhse" runat="server" /></td>
                </tr>
                <tr>
                    <th>訂單編號：</th>
                    <td>
                        <asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th>ERP交貨編號：</th>
                    <td>
                        <asp:Literal ID="litErpShipNumber" runat="server" /></td>
                </tr>
                <tr>
                    <th>訂單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" runat="server" /></td>
                    <th>訂單日期：</th>
                    <td>
                        <asp:Literal ID="litCdt" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP訂單編號：</th>
                    <td>
                        <asp:Literal ID="litErpOrderNumber" runat="server" /></td>
                    <th>預計出貨日：</th>
                    <td>
                        <asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" />
                        <asp:Literal ID="litEdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP訂單狀態：</th>
                    <td>
                        <asp:Literal ID="litErpStatus" runat="server" /></td>
                    <th>營業員：</th>
                    <td>
                        <asp:Literal ID="litSalesName" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td>
                        <asp:Literal ID="litCustomerName" runat="server" /></td>
                    <th rowspan="3">表頭折扣：</th>
                    <td class="discount" rowspan="3">
                        <asp:Literal ID="litHeaderDiscountList" runat="server" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="phProjQuoteIndex" Visible="false" runat="server">
                    <tr>
                        <th>專案報價：</th>
                        <td>
                            <ul class="keyword_list">
                                <asp:Literal ID="litProjQuoteIdxes" runat="server" />
                            </ul>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <th>備註：</th>
                    <td>
                        <asp:Literal ID="litRmk" runat="server" /></td>
                </tr>
            </table>

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
                        <td>
                            <asp:Literal ID="litCustomerNumber" runat="server" /></td>
                        <th width="140">地址：</th>
                        <td>
                            <asp:Literal ID="litCustomerAddr" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>客戶名稱：</th>
                        <td>
                            <asp:Literal ID="litCustomerName2" runat="server" /></td>
                        <th>TEL：</th>
                        <td>
                            <asp:Literal ID="litCustomerTel" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>聯絡人：</th>
                        <td>
                            <asp:Literal ID="litCustomerConName" runat="server" /></td>
                        <th>FAX：</th>
                        <td>
                            <asp:Literal ID="litCustomerFax" runat="server" /></td>
                    </tr>
                </table>
            </div>
            <div class="form_title">收貨人資訊</div>
            <div class="formBox">
                <table class="form">
                    <tr>
                        <th width="140">收貨人：</th>
                        <td>
                            <asp:Literal ID="litRcptName" runat="server" /></td>
                        <th width="140">貨運方式：</th>
                        <td>
                            <asp:Literal ID="litFreightWay" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>客戶名稱：</th>
                        <td>
                            <asp:Literal ID="litRcptCusterName" runat="server" /></td>
                        <th>地址：</th>
                        <td>
                            <asp:Literal ID="litRcptAddr" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>TEL：</th>
                        <td>
                            <asp:Literal ID="litRcptTel" runat="server" /></td>
                        <th>FAX：</th>
                        <td>
                            <asp:Literal ID="litRcptFax" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="rondane_box">
            <asp:PlaceHolder ID="phGoodsList" runat="server" />

            <table class="form totel">
                <tr>
                    <th>總金額：</th>
                    <td>
                        <asp:Literal ID="litPTTotalAmtDisp" Text="0" runat="server" />(未稅)</td>
                </tr>
                <tr>
                    <th>稅額：</th>
                    <td>
                        <asp:Literal ID="litTaxAmtDisp" Text="0" runat="server" /></td>
                </tr>
                <tr>
                    <th>折扣金額：</th>
                    <td>
                        <asp:Literal ID="litDctAmtDisp" Text="0" runat="server" /></td>
                </tr>
                <tr>
                    <th>折扣後總金額：</th>
                    <td>
                        <asp:Label ID="lblDctTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />(含稅)</td>
                </tr>
            </table>
            <div class="btnBox">
                <div>
                    <%--待列印 (儲存預計出貨日)--%>
                    <div>
                        <asp:LinkButton ID="btnUpdateEdd" Text="儲存" Visible="false" runat="server" OnClick="btnUpdateEdd_Click" />
                    </div>
                    <%--列印銷貨清單--%>
                    <div>
                        <asp:LinkButton ID="btnPrintSalesSlip" Text="列印銷貨清單" Visible="false" runat="server" OnClick="btnPrintSalesSlip_Click" OnClientClick="return printSalesSlip();" />
                    </div>
                    <%--待列印--%>
                    <div>
                        <asp:LinkButton ID="btnPrintShipping" Text="列印出貨單" Visible="false" runat="server" OnClick="btnPrintShipping_Click" OnClientClick="if(window.confirm('確定列印出貨單？')){return printShipping();}else{return false;}" />
                    </div>
                    <%--已列印--%>
                    <div>
                        <asp:LinkButton ID="btnStockUp" Text="確認備貨" Visible="false" runat="server" OnClick="btnStockUp_Click" />
                    </div>
                    <%--備貨中--%>
                    <div>
                        <asp:LinkButton ID="btnConfirmShipment" Text="確認出貨" Visible="false" runat="server" OnClick="btnConfirmShipment_Click" />
                    </div>
                    <div><a href="javascript:;" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        function printSalesSlip() {
            window.open("../../../popup/dom/print/sales_slip.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }

        function printShipping() {
            window.open("../../../popup/dom/print/shipping.aspx?sid=".concat($.getUrlVar('sid')));
            return true;
        }
    </script>
</asp:Content>

