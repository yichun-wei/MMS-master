<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="ext_quotn_edit" %>

<%@ Register TagPrefix="UC" TagName="GeneralGoods" Src="~/include/client/ext/quotn/edit/goods_block.ascx" %>
<%@ Register TagPrefix="UC" TagName="ManualGoods" Src="~/include/client/ext/quotn/edit/manual/goods_block.ascx" %>

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
            <h1>外銷報價單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">報價單編號：</th>
                    <td width="370">
                        <asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th width="140">報價單日期：</th>
                    <td>
                        <asp:TextBox ID="txtQuotnDate" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>報價單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" Text="草稿" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td>
                        <asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>建單人：</th>
                    <td>
                        <asp:Literal ID="litCName" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td>
                        <asp:TextBox ID="txtCdd" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td>
                        <table>
                            <tr>
                                <th>
                                    <asp:RadioButton ID="rbtCusterBySearch" GroupName="CusterBy" CssClass="dev-custer-by-search" onclick="customerHelper.onCusterBySearch();" runat="server" /></th>
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
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <asp:RadioButton ID="rbtCusterByManual" GroupName="CusterBy" CssClass="dev-custer-by-manual" onclick="customerHelper.onCusterByManual();" Checked="true" runat="server" /></th>
                                <td>
                                    <asp:TextBox ID="txtInputCustomerName" MaxLength="50" runat="server" /></td>
                            </tr>
                        </table>

                        <script type="text/javascript">
                            $(function () {
                                setCustomerName();
                            });

                            function setCustomerName() {
                                $(".dev-customer-name").text($("#<%=this.hidCustomerName.ClientID%>").val());
                            }

                            var customerHelper = (function () {
                                return {
                                    openWindow: function () {
                                        var priceListId = customerHelper.onCusterBySearch();
                                        if (!priceListId) {
                                            return;
                                        }

                                        window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/customer.aspx?mr=2&priceListId='.concat(priceListId), 'customer', 'width=900, height=700, top=100, left=100, scrollbars=1');
                                    },

                                    returnInfo: function (customerId) {
                                        $("#<%=this.hidCustomerId.ClientID%>").val('');
                                        if (customerId) {
                                            $("#<%=this.hidCustomerId.ClientID%>").val(customerId);
                                            $("#<%=this.btnResetCustomer.ClientID%>").trigger('click');
                                        }
                                    },

                                    onCusterBySearch: function () {
                                        var objCurrencyBook = $("#<%=this.lstCurrencyBook.ClientID %>");

                                        //var priceListId = $("#<%=this.hidPriceListId.ClientID%>").val();
                                        //if (priceListId) {
                                        //    objCurrencyBook.val(priceListId);
                                        //} else {
                                        //    priceListId = objCurrencyBook.val();
                                        //    if (!priceListId) {
                                        //        customerHelper.onCusterByManual();
                                        //        alert('請選擇幣別');
                                        //        return 0;
                                        //    }
                                        //}

                                        var priceListId = objCurrencyBook.val();
                                        if (!priceListId) {
                                            customerHelper.onCusterByManual();
                                            alert('請選擇幣別');
                                            return 0;
                                        }

                                        //將核選切換為「選擇」
                                        $("#<%=this.rbtCusterBySearch.ClientID %>").prop("checked", true);

                                        //當客戶為選擇時, 不允許選擇幣別.
                                        //objCurrencyBook.val("");
                                        //objCurrencyBook.attr("disabled", "disabled");

                                        return priceListId;
                                    },

                                    onCusterByManual: function () {
                                        //將核選切換為「手動」
                                        $("#<%=this.rbtCusterByManual.ClientID %>").prop("checked", true);

                                        //當客戶為手動時, 允許選擇幣別.
                                        //var objCurrencyBook = $("#<%=this.lstCurrencyBook.ClientID %>");
                                        //objCurrencyBook.removeAttr("disabled");

                                        <%--切換為手動輸入時, 則清掉選擇客戶的相關設定.--%>
                                        $("#<%=this.hidPriceListId.ClientID%>").val("");
                                        $("#<%=this.hidCustomerId.ClientID%>").val("");
                                        $("#<%=this.hidCustomerName.ClientID%>").val("");
                                        $(".dev-customer-name").html("");
                                    },

                                    <%--手動輸入客戶名稱時(僅在編輯時同步用)--%>
                                    onInputCustomerName: function (obj) {
                                        $("#<%=this.hidCustomerName.ClientID%>").val(obj.val());
                                        $(".dev-customer-name").html(obj.val());
                                        $("#<%=this.lblCustomerName.ClientID%>").html(obj.val());
                                    }
                                };
                            })();
                        </script>
                    </td>
                    <th>幣別：</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="lstCurrencyBook" CssClass="dev-currency-book" runat="server">
                                    <asp:ListItem Text="請選擇" Value="" />
                                </asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnResetCustomer" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <th>備註：</th>
                    <td>
                        <asp:TextBox ID="txtRmk" TextMode="MultiLine" Rows="3" runat="server" /></td>
                </tr>
            </table>

            <%--連動幣別--%>
            <script type="text/javascript">
                function resetCurrencyCode() {
                    var currencyCode = $("#<%=this.lstCurrencyBook.ClientID%>").find(":selected").text();
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
                            <td width="370">
                                <asp:TextBox ID="txtCustomerNumber" runat="server" /></td>
                            <th width="140">地址：</th>
                            <td>
                                <asp:TextBox ID="txtCustomerAddr" MaxLength="240" CssClass="dev-custer-addr" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>客戶名稱：</th>
                            <td>
                                <asp:Label ID="lblCustomerName" CssClass="dev-custer-name" runat="server" /></td>
                            <th>TEL：</th>
                            <td>
                                <asp:TextBox ID="txtCustomerTel" MaxLength="25" CssClass="dev-custer-tel" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>聯絡人：</th>
                            <td>
                                <asp:TextBox ID="txtCustomerConName" MaxLength="50" CssClass="dev-custer-con-name" runat="server" /></td>
                            <th>FAX：</th>
                            <td>
                                <asp:TextBox ID="txtCustomerFax" MaxLength="25" CssClass="dev-custer-fax" runat="server" /></td>
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
                                $(".dev-rcpt-name").val($(".dev-custer-con-name").val());
                                $(".dev-rcpt-tel").val($(".dev-custer-tel").val());
                                $(".dev-rcpt-fax").val($(".dev-custer-fax").val());
                                $(".dev-rcpt-addr").val($(".dev-custer-addr").val());
                            }
                        }
                    </script>
                </div>
            </asp:PlaceHolder>
        </div>

        <%--建立訂單的按鈕操作--%>
        <asp:PlaceHolder ID="phCreateOrderOper" runat="server">
            <div class="rondane_box">
                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnCreateGoods" Text="建立報價單品項" runat="server" OnClick="btnCreateGoods_Click" />
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
                        <div class="btnBox min_btn2">
                            <div><asp:LinkButton ID="btnRemoveItems" Text="批次刪除" runat="server" OnClick="btnRemoveItems_Click" /></div>
                        </div>

                        <%--一般品項--%>
                        <UC:GeneralGoods ID="ucGeneralGoods" Title="一般訂單" runat="server" OnAddComplete="ucGeneralGoods_AddComplete" />

                        <%--手動新增--%>
                        <UC:ManualGoods ID="ucManualGoods" Title="手動新增" runat="server" />
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
                        <div>
                            <asp:LinkButton ID="btnCopyOrder" Text="複製" runat="server" OnClick="btnCopyOrder_Click" />
                        </div>
                        <div>
                            <asp:LinkButton ID="btnToOrder" Text="提交" runat="server" OnClick="btnToOrder_Click" />
                        </div>
                        <div>
                            <asp:LinkButton ID="btnToDraft" Text="儲存" runat="server" OnClick="btnToDraft_Click" />
                        </div>
                        <div id="divPrintQuotn"><a href="javascript:void(0);" onclick="return printQuotn();" onkeypress="return printQuotn();">列印</a></div>
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

    <%--列印--%>
    <script type="text/javascript">
        $(function () {
            var quotnSId = $.getUrlVar('sid');
            if (!quotnSId) {
                $("#divPrintQuotn").hide();
            }
        });

        function printQuotn() {
            window.open("../../popup/ext/print/quotn.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }
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

                    //2016.4.25 加上resizable=1 ，for IE browser 視窗
                    window.open('<%=Seec.Marketing.SystemDefine.WebSiteRoot %>popup/ext/goods_2.aspx?cntrSId='.concat(cntrSId), 'goods', 'width=900, height=700, top=100, left=100, scrollbars=1 ,resizable=1');
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

                    //var customerId = $("#<%=this.hidCustomerId.ClientID%>").val();

                    var contextKey = {
                        ReqErpItem: true
                    };
                    $find($(".dev-goods-block-auto-complete-id :input[type=hidden]", container).val()).set_contextKey(JSON2.stringify(contextKey));
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

                    <%--一般品項「搜尋新增」傳回「型號[#]料號」; 一般品項「手動新增」傳回「料號」--%>
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
                calcGoodsItemAmt: function (container) {
                    //var container = $(".dev-goods-item-".concat(cntrSId));
                    if (!container) {
                        return NaN;
                    }

                    var qty = parseInt($(".dev-goods-qty", container).val(), 10);
                    var unitPrice = parseFloat($(".dev-goods-unit-price", container).val());
                    //var discount = parseFloat($(".dev-goods-discount", container).val());

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

