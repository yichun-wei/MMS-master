<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="dom_order_edit" %>

<%@ Reference Control="~/include/client/dom/order/header_discount_block.ascx" %>
<%@ Reference Control="~/include/client/dom/order/edit/goods_block.ascx" %>

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
            <h1>內銷訂單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">地區：</th>
                    <td width="370">
                        <asp:DropDownList ID="lstDomDistList" runat="server">
                            <asp:ListItem Text="請選擇" Value="" />
                        </asp:DropDownList>

                        <script type="text/javascript">
                            $(function () {
                                $("#<%=this.lstDomDistList.ClientID%>").change(function () {
                                    $(".dev-customer-name").text("");
                                    $("#<%=this.hidCustomerId.ClientID%>").val("");
                                });
                            });
                        </script>
                    </td>
                    <th width="140">倉庫：</th>
                    <td>
                        <asp:DropDownList ID="lstWhseList" runat="server">
                            <asp:ListItem Text="請選擇" Value="" />
                        </asp:DropDownList>

                        <script type="text/javascript">
                            $(function () {
                                $("#<%=this.lstWhseList.ClientID%>").change(function () {
                                    orderItemHelper.checkAllOnHand();
                                });
                            });
                        </script>
                    </td>
                </tr>

                <tr>
                    <th>訂單編號：</th>
                    <td>
                        <asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th>ERP 交貨編號：</th>
                    <td>
                        <asp:Literal ID="litErpShipNumber" runat="server" /></td>
                </tr>
                <tr>
                    <th>訂單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" Text="草稿" runat="server" /></td>
                    <th>訂單日期：</th>
                    <td>
                        <asp:Literal ID="litCdt" runat="server" /></td>
                </tr>
                <tr>
                    <th>ERP 訂單編號：</th>
                    <td>
                        <asp:Literal ID="litErpOrderNumber" runat="server" /></td>
                    <th>預計出貨日：</th>
                    <td>
                        <%--<asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" />--%>
                        <asp:Literal ID="litEdd" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>ERP 訂單狀態：</th>
                    <td>
                        <asp:Literal ID="litErpStatus" runat="server" /></td>
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
                                        var dist = $("#<%=this.lstDomDistList.ClientID%>").find(":selected").val();
                                        if (!dist) {
                                            alert('請選擇地區');
                                            return;
                                        }

                                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/customer.aspx?mr=1&dist='.concat(dist), 'customer', 'width=900, height=700, top=100, left=100, scrollbars=1');
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
                	<th>產品：</th>
                    <td>
                        <asp:DropDownList ID="lstProdTypeList" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th rowspan="2">專案報價：</th>
                    <td rowspan="2">
                        <asp:PlaceHolder ID="phProjQuoteIndex" Visible="false" runat="server">
                            <a class="choose_btn" href="javascript:;" onclick="projQuoteHelper.openWindow(); return false;">選擇</a>
                            <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <ul class="keyword_list">
                                        <asp:Literal ID="litProjQuoteIdxes" runat="server" />
                                    </ul>
                                    <asp:HiddenField ID="hidSeledProjQuotes" runat="server" />
                                    <asp:HiddenField ID="hidExistedProjQuotes" runat="server" />
                                    <asp:Button ID="btnAddProjQuote" CssClass="display_none" runat="server" OnClick="btnAddProjQuote_Click" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:PlaceHolder>
                    </td>
                    <th rowspan="2">表頭折扣：</th>
                    <td rowspan="2" class="discount">
                        <asp:PlaceHolder ID="phHeaderDiscount" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>備註：</th>
                    <td>
                        <asp:TextBox ID="txtRmk" TextMode="MultiLine" Rows="3" runat="server" />
                    </td>
                </tr>
            </table>

            <%--訂單細部資訊--%>
            <asp:PlaceHolder ID="phOrderDetail" Visible="false" runat="server">
                <div class="form_title">客戶資訊</div>
                <div class="formBox">
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

        <%--專案報價腳本--%>
        <script type="text/javascript">
            var projQuoteHelper = (function () {
                return {
                    openWindow: function () {
                        var customerId = $("#<%=this.hidCustomerId.ClientID%>").val();
                        if (!customerId) {
                            alert('請選擇客戶');
                            return;
                        }

                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/dom/proj_quote/index.aspx?customerId='.concat(customerId), 'proj_quote', 'width=900, height=700, top=100, left=100, scrollbars=1');
                    },

                    getExistedProjQuotes: function () {
                        return $("#<%=this.hidExistedProjQuotes.ClientID%>").val();
                    },

                    removeExistedProjQuotes: function (quoteNo) {
                        var obj = $("#<%=this.hidExistedProjQuotes.ClientID%>");
                        var arr = obj.val().split(",");
                        arr = jQuery.grep(arr, function (value) {
                            return value != quoteNo;
                        });
                        obj.val(arr);

                        $(".dev-proj-quote-idx[quoteno='" + quoteNo + "']").remove();
                    },

                    returnInfo: function (quoteNos) {
                        $("#<%=this.hidSeledProjQuotes.ClientID%>").val(quoteNos);
                        $("#<%=this.btnAddProjQuote.ClientID%>").trigger("click");
                    }
                };
            })();
        </script>

        <%--建立訂單的按鈕操作--%>
        <asp:PlaceHolder ID="phCreateOrderOper" Visible="false" runat="server">
            <div class="rondane_box">
                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnCreateOrder" Text="建立訂單" runat="server" OnClick="btnCreateOrder_Click" />
                        </div>
                        <div><a href="index.aspx" title="回上一頁">回上一頁</a></div>
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
                        <asp:AsyncPostBackTrigger ControlID="btnAddProjQuote" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <table class="form totel">
                    <tr>
                        <th>總金額：</th>
                        <td>
                            <asp:Label ID="lblPTTotalAmtDisp" Text="0" runat="server" />(未稅)
                            <asp:HiddenField ID="hidPTTotalAmt" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>稅額：</th>
                        <td>
                            <asp:Label ID="lblTaxAmtDisp" Text="0" runat="server" />
                            <asp:HiddenField ID="hidTaxAmt" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>折扣金額：</th>
                        <td>
                            <asp:Label ID="lblDctAmtDisp" Text="0" runat="server" />(未稅)
                            <asp:HiddenField ID="hidDctAmt" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>折扣後總金額：</th>
                        <td>
                            <asp:Label ID="lblDctTotalAmtDisp" Text="0" CssClass="total_price" runat="server" />(含稅)
                            <asp:HiddenField ID="hidDctTotalAmt" runat="server" />
                        </td>
                    </tr>
                </table>
                <div class="btn_top"><a href="javascript:void(0)">top</a></div>

                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnToDraft" Text="儲存" Visible="false" runat="server" OnClick="btnToDraft_Click" />
                        </div>
                        <div>
                            <asp:LinkButton ID="btnCreatePGOrder" Text="建立備貨單" runat="server" OnClick="btnCreatePGOrder_Click" /></div>
                        <div>
                            <asp:LinkButton ID="btnToBizMgtDept" Text="提交審核" runat="server" OnClick="btnToBizMgtDept_Click" />
                        </div>
                        <div>
                            <asp:LinkButton ID="btnDelete" Text="刪除" runat="server" OnClick="btnDelete_Click" />
                        </div>
                        <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <%--表頭折扣腳本--%>
    <script type="text/javascript">
        $(function () {
            <%--初始表頭折扣--%>
            $(".dev-header-discount-check :checkbox").change(function () {
                orderItemHelper.calcOrderAmt();
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

                    orderItemHelper.calcOrderAmt();
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
                    var quoteNo = $("DIV.dev-goods-block-pg-attr", container).attr("QuoteNumber");

                    if (quoteNo) {
                        <%--專案報價品項--%>
                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/dom/proj_quote/goods.aspx?cntrSId='.concat(cntrSId).concat('&customerId=').concat(customerId).concat('&quoteNo=').concat(quoteNo), 'goods', 'width=900, height=700, top=100, left=100, scrollbars=1');
                    } else {
                        <%--一般品項--%>
                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/dom/goods.aspx?cntrSId='.concat(cntrSId).concat('&customerId=').concat(customerId), 'goods', 'width=900, height=700, top=100, left=100, scrollbars=1');
                    }
                },

                containPGOrder: function () {
                    return true;
                },

                regCntrSId: function (cntrSId) {
                    actCntrSId = cntrSId;
                },

                showInputAdd: function (cntrSId) {
                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return;
                    }
                    $(".dev-input-add-container", container).show();
                    $(".dev-ac-goods-keyword", container).focus();
                },

                setAutoCompleteContextKey: function () {
                    var container = $(".dev-goods-block-".concat(actCntrSId));
                    if (!container) {
                        return false;
                    }

                    var customerId = $("#<%=this.hidCustomerId.ClientID%>").val();
                    var quoteNo = $("DIV.dev-goods-block-pg-attr", container).attr("QuoteNumber");

                    if (quoteNo) {
                        <%--專案報價品項--%>
                        var contextKey = {
                            Source: 2,
                            CustomerId: customerId,
                            QuoteNo: quoteNo
                        };
                        $find($(".dev-goods-block-auto-complete-id :input[type=hidden]", container).val()).set_contextKey(JSON2.stringify(contextKey));
                    } else {
                        <%--一般品項--%>
                        var contextKey = {
                            Source: 1
                        };
                        $find($(".dev-goods-block-auto-complete-id :input[type=hidden]", container).val()).set_contextKey(JSON2.stringify(contextKey));
                    }
                },

                onAutoCompleteGoodsSeled: function (source, eventArgs) {
                    var container = $(".dev-goods-block-".concat(actCntrSId));
                    if (!container) {
                        return false;
                    }

                    var goodsItem = eventArgs.get_value().GoodsItem;
                    orderItemHelper.returnInfo(actCntrSId, goodsItem);
                },

                getExistedGoodsItems: function (cntrSId) {
                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return "";
                    }
                    return $(".dev-goods-block-existed-goods-items :input[type=hidden]", container).val();
                },

                checkOnHand: function (cntrSId, container) {
                    if (!container) {
                        container = $(".dev-goods-item-".concat(cntrSId));
                        if (!container) {
                            return;
                        }
                    }

                    var whse = $("#<%=this.lstWhseList.ClientID%>").find(":selected").val();
                    if (!whse) {
                        container.addClass("error");
                        return;
                    }

                    var objQty = $(".dev-goods-qty", container);
                    var qty = parseInt(objQty.val(), 10);
                    if (!isNaN(qty)) {
                        var onhand = parseInt($("SPAN[whse='" + whse + "']", container).attr("onhand"), 10);
                        if (!isNaN(onhand)) {
                            if (qty > onhand) {
                                container.addClass("error");
                            } else {
                                container.removeClass("error");
                            }
                        }else{
                            container.addClass("error");
                        }
                    } else {
                        container.addClass("error");
                    }
                },

                checkAllOnHand: function () {
                    $(".dev-goods-block").each(function () {
                        $(".dev-goods-item", $(this)).each(function () {
                            var container = $(this);
                            orderItemHelper.checkOnHand("", container);
                        });
                    });
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

                    <%--一般品項傳回「料號」或「料號[#]備貨單明細系統代號」, 專案報價品項傳回「報價單明細項次」或「報價單明細項次[#]備貨單明細系統代號」--%>
                    $("#<%=this.hidSeledGoodsItems.ClientID%>").val(goodsItems);
                    $(".dev-goods-block-search-add", container).trigger("click");
                },

                <%--檢查折扣輸入值--%>
                checkDiscount: function (obj, minVal) {
                    var value = parseFloat(obj.val());
                    if (isNaN(value)) {
                        obj.val("100");
                    } else if (value < minVal) {
                        obj.val(minVal);
                    } else if (value > 100) {
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
                    orderItemHelper.calcOrderAmt();
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
                    orderItemHelper.calcOrderAmt();
                },

                <%--異動數量時, 計算品項小計.--%>
                changeQty: function (cntrSId) {
                    var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return;
                    }

                    orderItemHelper.calcGoodsItemAmt(container);
                    orderItemHelper.calcOrderAmt();
                },

                <%--計算指定品項小計.--%>
                calcGoodsItemAmt: function (container, headerDcts) {

                    var goodsInfo = {
                        qty: NaN,
                        unitPrice: NaN,
                        discount: NaN,
                        subtotal: NaN,  //原始單價
                        downPrice: 0,   //小計要被扣掉的價錢
                        subtotalDct: 0  //「小計」-「小計要被扣掉的價錢」= 依表頭折扣計算後的小計
                    };

                    //var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return goodsInfo;
                    }

                    goodsInfo.qty = parseInt($(".dev-goods-qty", container).val(), 10);
                    goodsInfo.unitPrice = parseFloat($(".dev-goods-unit-price", container).val());
                    goodsInfo.discount = parseFloat($(".dev-goods-discount", container).val());

                    //if (isNaN(goodsInfo.qty) || isNaN(goodsInfo.unitPrice) || isNaN(goodsInfo.discount)) {
                    if (isNaN(goodsInfo.qty) || isNaN(goodsInfo.unitPrice)) {
                        $(".dev-goods-paid-amt-disp", container).html("");
                        $(".dev-goods-paid-amt :input[type=hidden]", container).val("")
                        return goodsInfo;
                    }

                    <%--折扣是用來看的該單價是牌價的幾折, 並不是「數量 * 單價 * 折扣 = 小計」, 小計只要「數量 * 單價」就好了.--%>
                    //goodsInfo.subtotal = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPrice).mul(accDiv(goodsInfo.discount, 100)), 2);
                    goodsInfo.subtotal = Math.formatFloat(goodsInfo.qty.mul(goodsInfo.unitPrice), 2);

                    //小計要被扣掉的價錢
                    if (headerDcts && headerDcts.length) {
                        var totalHeaderDct = 0;
                        for (i = 0; i < headerDcts.length; i++) {
                            totalHeaderDct = totalHeaderDct.add(headerDcts[i]);
                        }
                        goodsInfo.downPrice = Math.formatFloat(goodsInfo.subtotal.mul(totalHeaderDct), 2);
                    }
                    goodsInfo.subtotalDct = Math.formatFloat(goodsInfo.subtotal.sub(goodsInfo.downPrice), 2); //「小計」-「小計要被扣掉的價錢」= 依表頭折扣計算後的小計

                    //用小計
                    $(".dev-goods-paid-amt-disp", container).html(accounting.formatNumber(goodsInfo.subtotal, 2));
                    $(".dev-goods-paid-amt :input[type=hidden]", container).val(goodsInfo.subtotal)

                    return goodsInfo;
                },

                <%--計算整張訂單.--%>
                calcOrderAmt: function () {

                    var headerDcts = [];
                    //總金額(未稅) = 所有小計總和
                    var totalAmt = 0;
                    //所有小計要被扣掉的價錢總和
                    var totalDownPrice = 0;
                    //所有小計總和 (表頭折扣後)
                    var totalAmt_dct = 0;
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
                            var goodsInfo = orderItemHelper.calcGoodsItemAmt(container, headerDcts);

                            //用小計
                            blockSubtotal = blockSubtotal.add(goodsInfo.subtotal);

                            //品項表頭折扣後小計
                            //var subtotal_dct = subtotal;
                            //for (i = 0; i < headerDcts.length; i++) {
                            //    subtotal_dct = subtotal.sub(subtotal.mul(headerDcts[i]));
                            //}

                            //所有小計總和
                            totalAmt = totalAmt.add(goodsInfo.subtotal);
                            //所有小計要被扣掉的價錢總和
                            totalDownPrice = totalDownPrice.add(goodsInfo.downPrice);
                            //所有小計總和 (表頭折扣後)
                            totalAmt_dct = totalAmt_dct.add(goodsInfo.subtotalDct); //「小計」-「小計要被扣掉的價錢」= 依表頭折扣計算後的小計
                        });

                        if (isNaN(blockSubtotal)) {
                            $(".dev-goods-block-subtotal-disp", $(this)).html("");
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val("");
                        } else {
                            $(".dev-goods-block-subtotal-disp", $(this)).html(accounting.formatNumber(blockSubtotal, 2));
                            $(".dev-goods-block-subtotal :input[type=hidden]", $(this)).val(blockSubtotal);
                        }
                    });

                    <%--人民幣稅率「0.17」.--%>
                    var tax = 0.17;

                    <%--總金額(未稅)--%>
                    var ptTotalAmt = Math.formatFloat(totalAmt, 2);
                    if (isNaN(ptTotalAmt)) {
                        $("#<%=this.hidPTTotalAmt.ClientID%>").val("");
                        $("#<%=this.lblPTTotalAmtDisp.ClientID%>").html("");

                        $("#<%=this.hidTaxAmt.ClientID%>").val("");
                        $("#<%=this.lblTaxAmtDisp.ClientID%>").html("");

                        $("#<%=this.hidDctAmt.ClientID%>").val("");
                        $("#<%=this.lblDctAmtDisp.ClientID%>").html("");

                        $("#<%=this.hidDctTotalAmt.ClientID%>").val("");
                        $("#<%=this.lblDctTotalAmtDisp.ClientID%>").html("");
                    } else {
                        $("#<%=this.hidPTTotalAmt.ClientID%>").val(ptTotalAmt);
                        $("#<%=this.lblPTTotalAmtDisp.ClientID%>").html(accounting.formatNumber(ptTotalAmt, 2));

                        <%--稅額--%>
                        var taxAmt = Math.formatFloat(totalAmt.mul(tax), 2);
                        $("#<%=this.hidTaxAmt.ClientID%>").val(taxAmt);
                        $("#<%=this.lblTaxAmtDisp.ClientID%>").html(taxAmt);
                        $("#<%=this.lblTaxAmtDisp.ClientID%>").html(accounting.formatNumber(taxAmt, 2));

                        <%--折扣金額 = (總金額(未稅) + 稅額) * (表頭折扣總和)--%>
                        //var dctAmt = Math.formatFloat(ptTotalAmt.add(taxAmt).mul(totalHeaderDct), 2);
                        <%--折扣金額 = 所有小計要被扣掉的價錢總和 [by fan]--%>
                        var dctAmt = Math.formatFloat(totalDownPrice, 2);
                        $("#<%=this.hidDctAmt.ClientID%>").val(dctAmt);
                        $("#<%=this.lblDctAmtDisp.ClientID%>").html(accounting.formatNumber(dctAmt, 2));

                        <%--折扣後總金額(含稅) = (總金額(未稅) + 稅額 - 折扣金額)--%>
                        //var dctTotalAmt = Math.formatFloat(ptTotalAmt.add(taxAmt).sub(dctAmt), 2);
                        <%--折扣後總金額(含稅) = 所有小計總和 (表頭折扣後) * 1.17  [by fan]--%>
                        var dctTotalAmt = Math.formatFloat(totalAmt_dct.mul((1).add(tax)), 2);
                        $("#<%=this.hidDctTotalAmt.ClientID%>").val(dctTotalAmt);
                        $("#<%=this.lblDctTotalAmtDisp.ClientID%>").html(accounting.formatNumber(dctTotalAmt, 2));
                    }
                },
            };
        })();
    </script>
</asp:Content>

