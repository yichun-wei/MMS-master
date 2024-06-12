<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="dom_pg_order_edit" %>

<%@ Reference Control="~/include/client/dom/pg_order/edit/goods_block.ascx" %>

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
            <h1>內銷備貨單</h1>
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
                    <th width="140">備貨單日期：</th>
                    <td>
                        <asp:Literal ID="litCdt" runat="server" /></td>
                </tr>
                <tr>
                    <th>備貨單單號：</th>
                    <td>
                        <asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th>預計出貨日：</th>
                    <td>
                        <asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>備貨單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" Text="未使用" runat="server" /></td>
                    <th>備註：</th>
                    <td>
                        <asp:TextBox ID="txtRmk" TextMode="MultiLine" Rows="3" runat="server" /></td>
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
                                <asp:Button ID="btnResetCustomer" CssClass="display_none" runat="server" OnClick="btnResetCustomer_Click" />
                            </ContentTemplate>
                        </asp:UpdatePanel>

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
                </tr>
                <asp:PlaceHolder ID="phProjQuoteIndex" Visible="false" runat="server">
                    <tr>
                        <th>專案報價：</th>
                        <td>
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
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </div>

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
        <asp:PlaceHolder ID="phCreateOrderOper" runat="server">
            <div class="rondane_box">
                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnCreateGoods" Text="建立備貨單品項" runat="server" OnClick="btnCreateGoods_Click" />
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

                <div class="btnBox">
                    <div>
                        <div>
                            <asp:LinkButton ID="btnSave" Text="儲存備貨單" runat="server" OnClick="btnSave_Click" />
                        </div>
                        <div><a href="index.aspx" title="回上一頁">回上一頁</a></div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <asp:HiddenField ID="hidSeledGoodsItems" runat="server" />
    <script type="text/javascript">
        var orderItemHelper = (function () {
            return {
                openWindow: function (cntrSId) {

                    var container = $(".dev-goods-block-".concat(cntrSId));
                    if (!container) {
                        return false;
                    }

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
                    return false;
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

                    container.removeClass("error");

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

                    <%--一般品項傳回「料號」, 專案報價品項傳回「報價單明細項次」--%>
                    $("#<%=this.hidSeledGoodsItems.ClientID%>").val(goodsItems);
                    $(".dev-goods-block-add", container).trigger("click");
                }
            };
        })();
    </script>
</asp:Content>

