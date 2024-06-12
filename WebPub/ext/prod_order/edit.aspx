<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="ext_prod_order_edit" %>

<%@ Register TagPrefix="UC" TagName="GeneralGoods" Src="~/include/client/ext/prod_order/edit/goods_block.ascx" %>

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
            <h1>外銷生產單</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">訂單編號：</th>
                    <td width="370"><asp:Literal ID="litOdrNo" runat="server" /></td>
                    <th width="140">生產單狀態：</th>
                    <td><asp:Literal ID="litIsProdFixed" runat="server" /></td>
                </tr>
                <tr>
                    <th>建單日期：</th>
                    <td><asp:Literal ID="litCdt" runat="server" /></td>
                    <th>訂單日期：</th>
                    <td><asp:Literal ID="litQuotnDate" runat="server" /></td>
                </tr>
                <tr>
                    <th>生產單號：</th>
                    <td><asp:Literal ID="litProdOdrNo" runat="server" /></td>
                    <th>預計交貨日：</th>
                    <td><asp:TextBox ID="txtEdd" CssClass="checkin" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td><asp:Literal ID="litCustomerName" runat="server" /></td>
                    <th>客戶需求日：</th>
                    <td><asp:Literal ID="litCdd" runat="server" /></td>
                </tr>
            </table>
        </div>

        <div class="rondane_box">
            <%--一般品項--%>
            <UC:GeneralGoods ID="ucGeneralGoods" Title="一般訂單" runat="server" />

            <div class="btnBox">
                <div>
                    <div><asp:LinkButton ID="btnSaveOnly" Text="儲存" runat="server" OnClick="btnSaveOnly_Click" /></div>
                    <div><asp:LinkButton ID="btnFixedProdOrder" Text="確認" runat="server" OnClick="btnFixedProdOrder_Click" /></div>
                    <div id="divPrintProd"><a href="javascript:void(0);" onclick="return printProd();" onkeypress="return printProd();">列印</a></div>
                    <div><asp:LinkButton ID="btnDelete" Text="刪除" runat="server" OnClick="btnDelete_Click" /></div>
                    <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        $(function () {
            $("#<%=this.txtEdd.ClientID%>").change(function () {
                var edd = $(this).val();
                if (edd) {
                    $(".dev-est-fpms-date").each(function () {
                        if (!$(this).val()) {
                            $(this).val(edd);
                        }
                    });
                }
            });
        });
    </script>

    <%--列印--%>
    <script type="text/javascript">
        $(function () {
            var sid = $.getUrlVar('sid');
            if (!sid) {
                $("#divPrintProd").hide();
            }
        });

        function printProd() {
            window.open("../../popup/ext/print/prod.aspx?sid=".concat($.getUrlVar('sid')));
            return false;
        }
    </script>
</asp:Content>

