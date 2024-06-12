<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="ext_shipping_order_edit" %>

<%@ Reference Control="~/include/client/ext/shipping_order/header_discount_block.ascx" %>
<%@ Reference Control="~/include/client/ext/shipping_order/edit/goods_block.ascx" %>

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
    <asp:HiddenField ID="hidSpecSId" runat="server" />
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
                    <td><asp:Literal ID="litStatus" Text="草稿" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP 訂單編號：</th>
                    <td><asp:Literal ID="litErpOrderNumber" runat="server" /></td>
                    <th>訂單日期：</th>
                    <td><asp:Literal ID="litCdt" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP 訂單狀態：</th>
                    <td><asp:Literal ID="litErpStatus" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td>
                        <asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>預計到港日：</th>
                    <td>
                        <asp:TextBox ID="txtEta" CssClass="checkin" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td>
                        <asp:TextBox ID="txtCdd" CssClass="checkin" runat="server" /></td>
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
                                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/customer.aspx?mr=2', 'customer', 'width=900, height=700, top=100, left=100, scrollbars=1');
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
                    <th>幣別：</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="lblCurrencyCode" CssClass="dev-currency-code" runat="server" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnResetCustomer" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <%--訂單表頭資訊--%>
                <asp:PlaceHolder ID="phOrderHeder" Visible="false" runat="server">
                    <tr>
                        <th>選擇訂單：</th>
                        <td>
                            <a class="choose_btn" href="javascript:;" onclick="extOrderHelper.openWindow(); return false;">選擇</a>
                            <!--有幾個訂單下方就有幾個區塊-->
                            <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <ul class="keyword_list">
                                        <asp:Literal ID="litExtOrderIdxes" runat="server" />
                                    </ul>
                                    <asp:HiddenField ID="hidSeledExtOrders" runat="server" />
                                    <asp:HiddenField ID="hidExistedExtOrders" runat="server" />
                                    <asp:Button ID="btnAddExtOrder" CssClass="display_none" runat="server" OnClick="btnAddExtOrder_Click" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <th>表頭折扣：</th>
                        <td class="discount">
                            <asp:PlaceHolder ID="phHeaderDiscount" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
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

            <%--訂單細部資訊--%>
            <asp:PlaceHolder ID="phOrderDetail" Visible="false" runat="server">
                <div class="form_title">客戶資訊</div>
                <div class="formBox">
                    <table class="form">
                        <tr>
                            <th width="140">客戶編號：</th>
                            <td width="370"><asp:Literal ID="litCustomerNumber" runat="server" /></td>
                            <th width="140">地址：</th>
                            <td><asp:Label ID="lblCustomerAddr" CssClass="dev-custer-addr" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>客戶名稱：</th>
                            <td><asp:Label ID="lblCustomerName" CssClass="dev-custer-name" runat="server" /></td>
                            <th>TEL：</th>
                            <td><asp:Label ID="lblCustomerTel" CssClass="dev-custer-tel" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>聯絡人：</th>
                            <td><asp:Label ID="lblCustomerConName" CssClass="dev-custer-con-name" runat="server" /></td>
                            <th>FAX：</th>
                            <td><asp:Label ID="lblCustomerFax" CssClass="dev-custer-fax" runat="server" /></td>
                        </tr>
                    </table>
                </div>
                <div class="form_title">收貨人資訊</div>
                <div class="formBox">
                    <table class="form">
                        <tr>
                            <td class="same" colspan="4">
                                <label><input type="checkbox" id="chkAsCuster" onchange="setRcptAsCuster();" />同客戶資訊</label></td>
                        </tr>
                        <tr>
                            <th width="140">收貨人：</th>
                            <td width="370">
                                <asp:TextBox ID="txtRcptName" MaxLength="50" CssClass="dev-rcpt-name" runat="server" /></td>
                            <th width="140">貨運方式：</th>
                            <td>
                                <asp:DropDownList ID="lstFreightWayList" runat="server">
                                    <asp:ListItem Text="請選擇" Value="" />
                                </asp:DropDownList></td>
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
            </asp:PlaceHolder>
        </div>

        <%--外銷訂單腳本--%>
        <script type="text/javascript">
            var extOrderHelper = (function () {
                return {
                    openWindow: function () {
                        var customerId = $("#<%=this.hidCustomerId.ClientID%>").val();
                        if (!customerId) {
                            alert('請選擇客戶');
                            return;
                        }

                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/ext/shipping_order/sel_ext_order.aspx?customerId='.concat(customerId), 'ext_order', 'width=900, height=700, top=100, left=100, scrollbars=1');
                    },

                    getExistedExtOrders: function () {
                        return $("#<%=this.hidExistedExtOrders.ClientID%>").val();
                    },

                    removeExistedExtOrders: function (extOrderSId) {
                        var obj = $("#<%=this.hidExistedExtOrders.ClientID%>");
                        var arr = obj.val().split(",");
                        arr = jQuery.grep(arr, function (value) {
                            return value != extOrderSId;
                        });
                        obj.val(arr);

                        $(".dev-ext-order-idx[extordersid='" + extOrderSId + "']").remove();
                    },

                    returnInfo: function (extOrderSIds) {
                        $("#<%=this.hidSeledExtOrders.ClientID%>").val(extOrderSIds);
                        $("#<%=this.btnAddExtOrder.ClientID%>").trigger("click");
                    }
                };
            })();
        </script>

        <%--建立訂單的按鈕操作--%>
        <asp:PlaceHolder ID="phCreateOrderOper" runat="server">
            <div class="rondane_box">
                <%--<div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnCreateGoods" Text="依訂單建立" runat="server" OnClick="btnCreateGoods_Click" />
                        </div>
                        <div><a href="index.aspx" title="回上一頁">回上一頁</a></div>
                    </div>
                </div>--%>

                <div class="btnBox">
                    <div>
                        <div><asp:LinkButton ID="btnCreateGoods" Text="依訂單建立" runat="server" OnClick="btnCreateGoods_Click" /></div>
                        <div><asp:LinkButton ID="btnCreateGoods2" Text="依品項建立" runat="server" OnClick="btnCreateGoods2_Click" /></div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phOrderGoods" Visible="false" runat="server">
            <div class="rondane_box">

                <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:PlaceHolder ID="phGoodsList" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAddExtOrder" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <table class="form totel">
                    <%--外銷出貨單不需要看折扣的金額--%>
                    <tr style="display: none;">
                        <th>總金額：</th>
                        <td>
                            <asp:Label ID="lblTotalAmtDisp" Text="0" runat="server" />
                            <asp:HiddenField ID="hidTotalAmt" runat="server" />
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <th>折扣金額：</th>
                        <td>
                            <asp:Label ID="lblDctAmtDisp" Text="0" runat="server" />
                            <asp:HiddenField ID="hidDctAmt" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th><%--折扣後總金額--%>總金額：</th>
                        <td>
                            <asp:Label ID="lblDctTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />
                            <asp:HiddenField ID="hidDctTotalAmt" runat="server" />
                        </td>
                    </tr>
                </table>
                <div class="btnBox">
                    <div>
                        <div><asp:LinkButton ID="btnToDraft" Text="儲存" runat="server" OnClick="btnToDraft_Click" /></div>
                        <div><asp:LinkButton ID="btnFixedShippingOrder" Text="出貨單確認" runat="server" OnClick="btnFixedShippingOrder_Click" /></div>
                        <div><a href="javascript:void(0);" onclick="return printShipping();" onkeypress="return printShipping();">列印</a></div>
                        <div><asp:LinkButton ID="btnDelete" Text="刪除" runat="server" OnClick="btnDelete_Click" /></div>
                        <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        function printShipping() {
            window.open("../../popup/ext/print/shipping.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }
    </script>

    <%--表頭折扣腳本--%>
    <script type="text/javascript">
        $(function () {
            <%--初始表頭折扣--%>
            $(".dev-header-discount-check :checkbox").change(function () {
                orderItemHelper.calcOrderAmt(true);
            });
        });

        var headerDiscountHelper = (function () {
            return {
                <%--檢查表頭折扣輸入值--%>
                checkDiscount: function (obj) {
                    var value = parseFloat(obj.val());
                    if (isNaN(value)) {
                        //obj.val("100");
                    } else if (value < 0 || value > 100) {
                        obj.val("100");
                    }

                    orderItemHelper.calcOrderAmt(true);
                },
            };
        })();
    </script>

    <%--品項腳本--%>
    <asp:HiddenField ID="hidSeledGoodsItems" runat="server" />
    <script type="text/javascript">
        var orderItemHelper = (function () {

            var actCntrSId;

            return {
                openWindow: function (cntrSId) {

                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return false;
                    }

                    $(".dev-input-add-container", container).hide();

                    var customerId = $("#<%=this.hidCustomerId.ClientID%>").val();
                    var extOrderSId = $("DIV.dev-goods-block-pg-attr", container).attr("ExtOrderSId");

                    if (extOrderSId) {
                        <%--外銷訂單品項--%>
                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/ext/shipping_order/order_goods.aspx?cntrSId='.concat(cntrSId).concat('&customerId=').concat(customerId).concat('&extOrderSId=').concat(extOrderSId), 'goods', 'width=900, height=700, top=100, left=100, scrollbars=1');
                    }
                },

                regCntrSId: function (cntrSId) {
                    actCntrSId = cntrSId;
                },

                getExistedGoodsItems: function (cntrSId) {
                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return "";
                    }
                    return $(".dev-goods-block-existed-goods-items :input[type=hidden]", container).val();
                },

                checkMaxQty: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    //container.removeClass("error");

                    var objQty = $(".dev-goods-qty", container);
                    var qty = parseInt(objQty.val(), 10);
                    if (!isNaN(qty)) {
                        var objMaxQty = $(".dev-goods-max-qty", container);
                        if (objMaxQty) {
                            var maxQty = parseInt(objMaxQty.html(), 10);
                            if (!isNaN(maxQty)) {
                                if (qty > maxQty) {
                                    objQty.val(maxQty);
                                }
                            }
                        }
                    }
                },

                returnInfo: function (cntrSId, goodsItems) {
                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return false;
                    }

                    $(".dev-ac-goods-keyword", container).val("");
                    $(".dev-ac-goods-keyword-container", container).hide();
                    $(".dev-input-add-container", container).show();
                    $(".dev-ac-goods-item-loading", container).show();

                    <%--一般品項傳回「料號-外銷訂單明細系統代號」--%>
                    $("#<%=this.hidSeledGoodsItems.ClientID%>").val(goodsItems);
                    $(".dev-goods-block-search-add", container).trigger("click");
                },

                <%--檢查折扣輸入值--%>
                checkDiscount: function (obj) {
                    var value = parseInt(obj.val(), 10);
                    if (isNaN(value)) {
                        obj.val("100");
                    } else if (value < 0 || value > 100) {
                        obj.val("100");
                    }
                },

                <%--異動單價時, 計算折扣.--%>
                changeUnitPrice: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    var listPrice = parseFloat($("DIV.dev-goods-item-pg-attr", container).attr("ListPrice"));
                    var unitPrice = parseFloat($(".dev-goods-unit-price", container).val());

                    if (isNaN(listPrice) || isNaN(unitPrice)) {
                        $(".dev-goods-paid-amt-disp", container).html("");
                        $(".dev-goods-paid-amt :input[type=hidden]", container).val("")
                    } else {
                        var discount = Math.formatFloat(unitPrice.div(listPrice).mul(100), 2);
                        if (discount < 0 || discount > 100) {
                            discount = 100;
                            $(".dev-goods-discount", container).val(discount);
                            orderItemHelper.changeDiscount(cntrSId);
                        } else {
                            $(".dev-goods-discount", container).val(discount);
                        }
                        $(".dev-goods-discount", container).val(discount);
                    }

                    orderItemHelper.calcGoodsItemAmt(container);
                    orderItemHelper.calcOrderAmt(false);
                },

                <%--異動折扣時, 計算單價.--%>
                changeDiscount: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    var listPrice = parseFloat($("DIV.dev-goods-item-pg-attr", container).attr("ListPrice"));
                    var discount = parseFloat($(".dev-goods-discount", container).val());

                    if (isNaN(listPrice) || isNaN(discount)) {
                        $(".dev-goods-paid-amt-disp", container).html("");
                        $(".dev-goods-paid-amt :input[type=hidden]", container).val("")
                    } else {
                        var unitPrice = Math.formatFloat(discount.div(100).mul(listPrice), 4);
                        $(".dev-goods-unit-price", container).val(unitPrice);
                    }

                    orderItemHelper.calcGoodsItemAmt(container);
                    orderItemHelper.calcOrderAmt(false);
                },

                <%--異動數量時, 計算品項小計.--%>
                changeQty: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    orderItemHelper.calcGoodsItemAmt(container);
                    orderItemHelper.calcOrderAmt(false);
                },

                <%--異動折扣後單價時時, 計算品項小計.--%>
                changeUnitPriceDct: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    orderItemHelper.calcGoodsItemAmt(container);
                    orderItemHelper.calcOrderAmt(false);
                },

                <%--計算指定品項小計.--%>
                calcGoodsItemAmt: function (container, headerDcts, recalcUnitPriceDct) {

                    var goodsInfo = {
                        qty: NaN,
                        unitPrice: NaN,
                        unitPriceDct: NaN,
                        discount: NaN,
                        subtotal: NaN,  //品項小計 (單價)
                        downPrice: 0,   //小計要被扣掉的價錢
                        subtotalDct: 0  //品項小計 (折扣後單價)
                    };

                    //var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return goodsInfo;
                    }

                    goodsInfo.qty = parseInt($(".dev-goods-qty", container).val(), 10);
                    goodsInfo.unitPrice = parseFloat($(".dev-goods-unit-price", container).val());
                    goodsInfo.unitPriceDct = parseFloat($(".dev-goods-unit-price-dct", container).val());
                    goodsInfo.discount = parseFloat($(".dev-goods-discount", container).val());

                    //折扣後單價只關心「單價」和「表頭折扣」
                    //是否重新計算折扣後單價 (因為允許手動更改, 所以只有在異動表頭折扣後才需要重新計算)
                    if (!isNaN(goodsInfo.unitPrice)) {
                        if (recalcUnitPriceDct) {
                            //若要重新計算, 則先讓折扣後單價 = 單價
                            goodsInfo.unitPriceDct = goodsInfo.unitPrice;

                            //單價要被扣掉的價錢
                            if (headerDcts && headerDcts.length) {
                                //所有表頭折扣總和
                                var totalHeaderDct = 0;
                                for (i = 0; i < headerDcts.length; i++) {
                                    totalHeaderDct = totalHeaderDct.add(headerDcts[i]);
                                }
                                var downPrice = Math.formatFloat(goodsInfo.unitPriceDct.mul(totalHeaderDct), 4);
                                goodsInfo.unitPriceDct = Math.formatFloat(goodsInfo.unitPriceDct.sub(downPrice), 4);
                            }

                            //在表頭折扣還沒輸入折扣時, 值會為 NaN.
                            if (isNaN(goodsInfo.unitPriceDct)) {
                                goodsInfo.unitPriceDct = goodsInfo.unitPrice;
                            }

                            $(".dev-goods-unit-price-dct", container).val(goodsInfo.unitPriceDct);
                        }

                        if (isNaN(goodsInfo.unitPriceDct)) {
                            $(".dev-goods-unit-price-dct", container).val(goodsInfo.unitPrice)
                            return goodsInfo;
                        }
                    }

                    if (isNaN(goodsInfo.qty) || isNaN(goodsInfo.unitPrice)) {
                        $(".dev-goods-paid-amt-disp", container).html("");
                        $(".dev-goods-paid-amt :input[type=hidden]", container).val("")
                        return goodsInfo;
                    }

                    <%--折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.--%>
                    //var subtotal = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPrice).mul(accDiv(discount, 100)), 2);
                    //var subtotal = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPrice), 2);
                    goodsInfo.subtotal = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPrice), 2);

                    //品項小計 (折扣後單價)
                    goodsInfo.subtotalDct = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPriceDct), 2);
                    //品項小計要被扣掉的價錢總和
                    goodsInfo.downPrice = Math.formatFloat(goodsInfo.subtotal.sub(goodsInfo.subtotalDct), 2);

                    //用表頭折扣計算後的小計
                    $(".dev-goods-paid-amt-disp", container).html(accounting.formatNumber(goodsInfo.subtotalDct, 2));
                    $(".dev-goods-paid-amt :input[type=hidden]", container).val(goodsInfo.subtotalDct)

                    return goodsInfo;
                },

                <%--計算整張訂單.--%>
                calcOrderAmt: function (recalcUnitPriceDct) {

                    var headerDcts = [];
                    //總金額 = 所有小計總和
                    var totalAmt = 0;
                    //所有小計要被扣掉的價錢總和
                    var totalDownPrice = 0;
                    //所有表頭折扣總和
                    var totalHeaderDct = 0;

                    <%--巡覽所有表頭折扣--%>
                    $(".dev-header-discount-check :checkbox").each(function () {
                        var checked = $(this).is(":checked");
                        if (checked) {
                            var headerDiscount = parseFloat($(this).parent().next("input:text").val());
                            <%--表頭折扣計算公式為「單價 * 表頭折扣」等於「單價要扣掉的金額」, 例如「單價 250 - (單價 250 * 表頭折扣 10% = 25) = 單價 225」--%>
                            var headerDct = headerDiscount.div(100);
                            totalHeaderDct = totalHeaderDct.add(headerDct);
                            headerDcts.push(headerDct);
                        }
                    });

                    <%--巡覽所有品項區塊--%>
                    $(".dev-goods-block").each(function () {

                        var blockSubtotal = 0;

                        $(".dev-goods-item", $(this)).each(function () {

                            var container = $(this);

                            //var qty = parseInt($(".dev-goods-qty", container).val(), 10);
                            //var listPrice = parseFloat($("DIV.dev-goods-item-pg-attr", container).attr("ListPrice"));
                            //var unitPrice = parseFloat($(".dev-goods-unit-price", container).val());
                            //var discount = parseFloat($(".dev-goods-discount", container).val());

                            //品項資訊
                            var goodsInfo = orderItemHelper.calcGoodsItemAmt(container, headerDcts, recalcUnitPriceDct);

                            //用表頭折扣計算後的小計
                            blockSubtotal = blockSubtotal.add(goodsInfo.subtotalDct);

                            //品項表頭折扣後小計
                            //var subtotal_dct = subtotal;
                            //for (i = 0; i < headerDcts.length; i++) {
                            //    subtotal_dct = subtotal.sub(subtotal.mul(headerDcts[i]));
                            //}

                            //所有小計總和
                            totalAmt = totalAmt.add(goodsInfo.subtotal);
                            //所有小計要被扣掉的價錢總和
                            totalDownPrice = totalDownPrice.add(goodsInfo.downPrice);
                        });

                        if (isNaN(blockSubtotal)) {
                            $(".dev-goods-block-subtotal-disp", $(this)).html("");
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val("");
                        } else {
                            $(".dev-goods-block-subtotal-disp", $(this)).html(accounting.formatNumber(blockSubtotal, 2));
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val(blockSubtotal);
                        }
                    });

                    <%--總金額--%>
                    var ptTotalAmt = Math.formatFloat(totalAmt, 2);
                    if (isNaN(ptTotalAmt)) {
                        $("#<%=this.hidTotalAmt.ClientID%>").val("");
                        $("#<%=this.lblTotalAmtDisp.ClientID%>").html("");

                        $("#<%=this.hidDctAmt.ClientID%>").val("");
                        $("#<%=this.lblDctAmtDisp.ClientID%>").html("");

                        $("#<%=this.hidDctTotalAmt.ClientID%>").val("");
                        $("#<%=this.lblDctTotalAmtDisp.ClientID%>").html("");
                    } else {
                        $("#<%=this.hidTotalAmt.ClientID%>").val(ptTotalAmt);
                        $("#<%=this.lblTotalAmtDisp.ClientID%>").html(accounting.formatNumber(ptTotalAmt, 2));

                        <%--折扣金額 = (總金額) * (表頭折扣總和)--%>
                        //var dctAmt = Math.formatFloat(ptTotalAmt.mul(totalHeaderDct), 2);
                        var dctAmt = totalDownPrice;
                        $("#<%=this.hidDctAmt.ClientID%>").val(dctAmt);
                        $("#<%=this.lblDctAmtDisp.ClientID%>").html(accounting.formatNumber(dctAmt, 2));

                        <%--折扣後總金額 = (總金額 - 折扣金額)--%>
                        var dctTotalAmt = Math.formatFloat(ptTotalAmt.sub(dctAmt), 2);
                        $("#<%=this.hidDctTotalAmt.ClientID%>").val(dctTotalAmt);
                        $("#<%=this.lblDctTotalAmtDisp.ClientID%>").html(accounting.formatNumber(dctTotalAmt, 2));
                    }
                },
            };
        })();
    </script>
</asp:Content>

