<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="dom_order_view" %>

<%@ Reference Control="~/include/client/dom/order/view/goods_block.ascx" %>

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
            <h1>內銷訂單</h1>
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
                	<th>產品：</th>
                    <td>
                        <asp:Literal ID="litProdType" runat="server" /></td>
                </tr>
                <tr>
                    <th>專案報價：</th>
                    <td>
                        <asp:PlaceHolder ID="phProjQuoteIndex" Visible="false" runat="server">
                            <ul class="keyword_list">
                                <asp:Literal ID="litProjQuoteIdxes" runat="server" />
                            </ul>
                        </asp:PlaceHolder>
                    </td>
                    <th rowspan="2">表頭折扣：</th>
                    <td rowspan="2" class="discount">
                        <asp:Literal ID="litHeaderDiscountList" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>備註：</th>
                    <td>
                        <asp:TextBox ID="txtRmk" TextMode="MultiLine" Rows="3" runat="server" />
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

            <%--收貨人資訊 - 檢視--%>
            <asp:PlaceHolder ID="phRcptView" runat="server">
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
            </asp:PlaceHolder>

            <%--收貨人資訊 - 編輯--%>
            <asp:PlaceHolder ID="phRcptEdit" runat="server">
                <div class="form_title">收貨人資訊</div>
                <div class="formBox">
                    <table class="form">
                        <tr>
                            <th width="140">收貨人：</th>
                            <td width="370">
                                <asp:TextBox ID="txtRcptName" MaxLength="50" CssClass="dev-rcpt-name" runat="server" /></td>
                            <th width="140">貨運方式：</th>
                            <td>
                                <asp:DropDownList ID="lstFreightWayList" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <th>客戶名稱：</th>
                            <td>
                                <asp:TextBox ID="txtRcptCusterName" MaxLength="50" CssClass="dev-rcpt-custer-name" runat="server" /></td>
                            <th>地址：</th>
                            <td>
                                <asp:TextBox ID="txtRcptAddr" MaxLength="240" CssClass="dev-rcpt-addr" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>TEL：</th>
                            <td>
                                <asp:TextBox ID="txtRcptTel" MaxLength="35" CssClass="dev-rcpt-tel" runat="server" /></td>
                            <th>FAX：</th>
                            <td>
                                <asp:TextBox ID="txtRcptFax" MaxLength="35" CssClass="dev-rcpt-fax" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </asp:PlaceHolder>
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
                        <asp:Literal ID="litDctAmtDisp" Text="0" runat="server" />(未稅)</td>
                </tr>
                <tr>
                    <th>折扣後總金額：</th>
                    <td>
                        <asp:Label ID="lblDctTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />(含稅)</td>
                </tr>
            </table>
            <div class="btnBox">
                <div>
                    <div>
                        <asp:LinkButton ID="btnUpdateInfo" Text="儲存" Visible="false" runat="server" OnClick="btnUpdateInfo_Click" />
                    </div>
                    <%--營管部待審核--%>
                    <div>
                        <asp:LinkButton ID="btnToDraft" Text="退回重改" Visible="false" runat="server" OnClick="btnToDraft_Click" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnToErp" Text="審核通過" Visible="false" runat="server" OnClick="btnToErp_Click" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnCancel" Text="取消" Visible="false" runat="server" OnClick="btnCancel_Click" />
                    </div>
                    <%--財務待審核--%>
                    <div>
                        <asp:LinkButton ID="btnPrintSalesSlip" Text="列印銷貨清單" Visible="false" runat="server" OnClick="btnPrintSalesSlip_Click" OnClientClick="return printSalesSlip();" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnPrintShipping" Text="列印出貨單" Visible="false" runat="server" OnClick="btnPrintShipping_Click" OnClientClick="return printShipping();" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnToShipping" Text="財務審核通過" runat="server" Visible="false" OnClick="btnToShipping_Click" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnToUnpaid" Text="未付款待簽核" runat="server" Visible="false" OnClick="btnToUnpaid_Click" />
                    </div>
                    <%--未付款待簽核 (to 副總審核)--%>
                    <div>
                        <asp:LinkButton ID="btnToShipping2" Text="審核通過" Visible="false" runat="server" OnClick="btnToShipping2_Click" />
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
            window.open("../../popup/dom/print/sales_slip.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }

        function printShipping() {
            window.open("../../popup/dom/print/shipping.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }
    </script>
</asp:Content>

