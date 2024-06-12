<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="ext_order_edit" %>

<%@ Register TagPrefix="UC" TagName="GeneralGoods" Src="~/include/client/ext/order/edit/goods_block.ascx" %>
<%@ Register TagPrefix="UC" TagName="ManualGoods" Src="~/include/client/ext/order/edit/manual/goods_block.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷訂單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">訂單編號：</th>
                    <td width="370">
                        <asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th width="140">訂單日期：</th>
                    <td>
                        <asp:Literal ID="litQuotnDate" runat="server" /></td>
                </tr>
                <tr>
                    <th>訂單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td>
                        <asp:Literal ID="litEdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>建單人：</th>
                    <td>
                        <asp:Literal ID="litCName" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td>
                        <asp:Literal ID="litCdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>營業員：</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:Literal ID="litSalesName" runat="server" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnResetCustomer" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <th>幣別：</th>
                    <td>
                        <asp:Label ID="lblCurrencyCode" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:HyperLink ID="lnkSelCuster" CssClass="choose_btn" Text="選擇" NavigateUrl="javascript:;" onclick="customerHelper.openWindow(); return false;" runat="server" />
                                <div class="dev-customer-name"></div>
                                <asp:HiddenField ID="hidCustomerId" runat="server" />
                                <asp:HiddenField ID="hidCustomerName" runat="server" />
                                <asp:HiddenField ID="hidPriceListId" runat="server" />
                                <asp:Button ID="btnResetCustomer" CssClass="display_none" runat="server" OnClick="btnResetCustomer_Click" />
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <%--客戶腳本--%>
                        <script type="text/javascript">
                            $(function () {
                                setCustomerName();
                            });

                            function setCustomerName() {
                                $(".dev-customer-name").text($("#<%=this.hidCustomerName.ClientID%>").val());
                            }
                        </script>

                        <script type="text/javascript">
                            var customerHelper = (function () {
                                return {
                                    openWindow: function () {
                                        var priceListId = $("#<%=this.hidPriceListId.ClientID%>").val();

                                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/customer.aspx?mr=2&priceListId='.concat(priceListId), 'customer', 'width=900, height=700, top=100, left=100, scrollbars=1');
                                    },

                                    returnInfo: function (customerId) {
                                        $("#<%=this.hidCustomerId.ClientID%>").val('');
                                        if (customerId) {
                                            $("#<%=this.hidCustomerId.ClientID%>").val(customerId);
                                            $("#<%=this.btnResetCustomer.ClientID%>").trigger('click');
                                        }
                                    }
                                };
                            })();
                        </script>
                    </td>
                    <th>備註：</th>
                    <td>
                        <asp:Literal ID="litRmk" runat="server" /></td>
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

            <div class="form_title">客戶資訊</div>
            <div class="formBox">
                <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <table class="form">
                            <tr>
                                <th width="140">客戶編號：</th>
                                <td width="370">
                                    <asp:Literal ID="litCustomerNumber" runat="server" /></td>
                                <th width="140">地址：</th>
                                <td>
                                    <asp:Label ID="lblCustomerAddr" CssClass="dev-custer-addr" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>客戶名稱：</th>
                                <td>
                                    <asp:Label ID="lblCustomerName" CssClass="dev-custer-name" runat="server" />
                                </td>
                                <th>TEL：</th>
                                <td>
                                    <asp:Label ID="lblCustomerTel" CssClass="dev-custer-tel" runat="server" /></td>
                            </tr>
                            <tr>
                                <th>聯絡人：</th>
                                <td>
                                    <asp:Label ID="lblCustomerConName" CssClass="dev-custer-con-name" runat="server" /></td>
                                <th>FAX：</th>
                                <td>
                                    <asp:Label ID="lblCustomerFax" CssClass="dev-custer-fax" runat="server" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnResetCustomer" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="form_title">收貨人資訊</div>
            <div class="formBox">
                <table class="form">
                    <tr>
                        <td class="same" colspan="4">
                            <label>
                                <input type="checkbox" id="chkAsCuster" onchange="setRcptAsCuster();" />同客戶資訊</label></td>
                    </tr>
                    <tr>
                        <th width="140">收貨人：</th>
                        <td width="370">
                            <asp:TextBox ID="txtRcptName" MaxLength="50" CssClass="dev-rcpt-name" runat="server" /></td>
                        <th width="140">貨運方式：</th>
                        <td>
                            <asp:DropDownList ID="lstFreightWayList" runat="server">
                                <asp:ListItem Text="請選擇" Value="" />
                            </asp:DropDownList>
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

                <script type="text/javascript">
                    function setRcptAsCuster() {
                        if ($("#chkAsCuster").is(":checked")) {
                            $(".dev-rcpt-custer-name").val($(".dev-custer-name").html());
                            $(".dev-rcpt-name").val($(".dev-custer-con-name").html());
                            $(".dev-rcpt-tel").val($(".dev-custer-tel").html());
                            $(".dev-rcpt-fax").val($(".dev-custer-fax").html());
                            $(".dev-rcpt-addr").val($(".dev-custer-addr").html());
                        }
                    }
                </script>
            </div>
        </div>

        <div class="rondane_box">
            <%--一般品項--%>
            <UC:GeneralGoods ID="ucGeneralGoods" Title="一般訂單" runat="server" />

            <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <%--手動新增--%>
                    <UC:ManualGoods ID="ucManualGoods" Title="手動新增" runat="server" />
                    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <table class="form totel">
                <tr>
                    <th>總金額：</th>
                    <td>
                        <asp:Label ID="lblTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />(未稅)
                        <asp:HiddenField ID="hidTotalAmt" runat="server" />
                    </td>
                </tr>
            </table>
            <div class="btnBox">
                <div>
                    <div><asp:LinkButton ID="btnSaveOnly" Text="儲存" runat="server" OnClick="btnSaveOnly_Click" /></div>
                    <div><asp:LinkButton ID="btnToFormalOrder" Text="轉存為訂單" runat="server" OnClick="btnToFormalOrder_Click" /></div>
                    <div><a href="javascript:void(0);" onclick="return printOrder();" onkeypress="return printOrder();">列印</a></div>
                    <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <%--列印--%>
    <script type="text/javascript">
        function printOrder() {
            window.open("../../popup/ext/print/order.aspx?quotnSId=".concat($.getUrlVar('quotnSId')));
            return false;
        }
    </script>

    <%--品項腳本--%>
    <asp:HiddenField ID="hidSeledGoodsItems" runat="server" />
    <script type="text/javascript">
        var orderItemHelper = (function () {
            return {
                openWindow: function (cntrSId) {

                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return false;
                    }

                    window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/ext/goods_2.aspx?cntrSId='.concat(cntrSId).concat('&single=1'), 'goods', 'width=900, height=700, top=100, left=100, scrollbars=1');
                },

                getExistedGoodsItems: function () {
                    return $("#<%=this.hidExistedGoodsItems.ClientID%>").val();
                },

                returnInfo: function (cntrSId, goodsItems) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return false;
                    }

                    <%--一般品項傳回「料號」--%>
                    $("#<%=this.hidSeledGoodsItems.ClientID%>").val(goodsItems);
                    $(".dev-goods-item-search-mapping", container).trigger("click");
                },

                <%--計算指定品項小計.--%>
                calcGoodsItemAmt: function (container) {
                    //var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return NaN;
                    }

                    var qty = parseInt($(".dev-goods-qty", container).html(), 10);
                    var unitPrice = parseFloat($(".dev-goods-unit-price", container).html());
                    //var discount = parseFloat($(".dev-goods-discount", container).html());

                    //if (isNaN(qty) || isNaN(unitPrice) || isNaN(discount)) {
                    if (isNaN(qty) || isNaN(unitPrice)) {
                        $(".dev-goods-paid-amt-disp", container).html("");
                        $(".dev-goods-paid-amt :input[type=hidden]", container).val("")
                        return NaN;
                    }

                    <%--折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.--%>
                    //var subtotal = Math.formatFloat(qty.mul(unitPrice).mul(accDiv(discount, 100)), 2);
                    var subtotal = Math.formatFloat(qty.mul(unitPrice), 2);
                    $(".dev-goods-paid-amt-disp", container).html(accounting.formatNumber(subtotal, 2));
                    $(".dev-goods-paid-amt :input[type=hidden]", container).val(subtotal)

                    return subtotal;
                },

                <%--計算整張訂單.--%>
                calcOrderAmt: function () {

                    //總金額(未稅) = 所有小計總和
                    var totalAmt = 0;

                    <%--巡覽所有品項區塊--%>
                    $(".dev-goods-block").each(function () {

                        var blockSubtotal = 0;

                        $(".dev-goods-item", $(this)).each(function () {

                            var container = $(this);

                            //品項小計
                            var subtotal = orderItemHelper.calcGoodsItemAmt(container);

                            blockSubtotal = blockSubtotal.add(subtotal);

                            //所有小計總和
                            totalAmt = totalAmt.add(subtotal);
                        });

                        if (isNaN(blockSubtotal)) {
                            $(".dev-goods-block-subtotal-disp", $(this)).html("");
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val("");
                        } else {
                            $(".dev-goods-block-subtotal-disp", $(this)).html(accounting.formatNumber(blockSubtotal, 2));
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val(blockSubtotal);
                        }
                    });

                    <%--總金額(未稅)--%>
                    var ptTotalAmt = Math.formatFloat(totalAmt, 2);
                    if (isNaN(ptTotalAmt)) {
                        $("#<%=this.hidTotalAmt.ClientID%>").val("");
                        $("#<%=this.lblTotalAmtDisp.ClientID%>").html("");
                    } else {
                        $("#<%=this.hidTotalAmt.ClientID%>").val(ptTotalAmt);
                        $("#<%=this.lblTotalAmtDisp.ClientID%>").html(accounting.formatNumber(ptTotalAmt, 2));
                    }
                },
            };
        })();
    </script>
</asp:Content>

