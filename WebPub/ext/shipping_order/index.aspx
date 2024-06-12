<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="ext_shipping_order_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />

    <script type="text/javascript">
        $(function () {
            $("input.dev-date-all").datepicker({

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
                    <th width="140">狀態：</th>
                    <td width="370">
                        <asp:DropDownList ID="lstStatusList" runat="server">
                            <asp:ListItem Text="全部" Value="" />
                        </asp:DropDownList>
                    </td>
                    <th width="140">訂單日期：</th>
                    <td>
                        <asp:TextBox ID="txtBeginCdt" CssClass="checkin dev-date-all" runat="server" />
                        ~
                        <asp:TextBox ID="txtEndCdt" CssClass="checkin dev-date-all" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>關鍵字：</th>
                    <td>
                        <asp:TextBox ID="txtKeyword" runat="server" /><br />
                        <asp:CheckBoxList ID="chklKeywordCols" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Value="ODR_NO" Text="出貨單編號" Selected="true" />
                            <asp:ListItem Value="CUSTOMER_NAME" Text="客戶名稱" Selected="true" />
                            <asp:ListItem Value="RCPT_NAME" Text="收貨人" Selected="true" />
                            <%--TODO: 訂單編號 待改寫--%>
                            <asp:ListItem Value="ODR_NO" Text="訂單編號" Selected="true" />
                            <asp:ListItem Value="ERP_ORDER_NUMBER" Text="ERP 訂單編號" Selected="true" />
                        </asp:CheckBoxList>
                    </td>
                    <th>預計交貨日：</th>
                    <td>
                        <asp:TextBox ID="txtBeginEdd" CssClass="checkin dev-date-all" runat="server" />
                        ~
                        <asp:TextBox ID="txtEndEdd" CssClass="checkin dev-date-all" runat="server" />
                    </td>
                </tr>
            </table>
            <div class="btnBox searchBtn">
                <div>
                    <div>
                        <asp:Button ID="btnSearch" Text="搜尋出貨單" runat="server" OnClick="Page_DoSearch" />
                        <asp:HiddenField ID="hidSearchConds" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <div class="list_box">
            <div class="btnBox">
                <div>
                    <div><asp:HyperLink ID="lnkAdd" Text="新增出貨單" runat="server" /></div>
                </div>
            </div>

            <asp:PlaceHolder ID="phPageList" runat="server" />

            <asp:PlaceHolder ID="phPaging" Visible="false" runat="server">
                <div id="dev_pages">
                    <div>
                        <Paging:CustFirstPaging ID="firstPaging" DisplayText="第一頁" runat="server" />
                        <Paging:CustPrevPaging ID="prevPaging" DisplayText="上一頁" runat="server" />
                        <Paging:CustNumberPaging ID="numberPaging" QuotationStyle="Empty" NextPagesDisplayText="下 {N} 頁" PrevPagesDisplayText="上 {N} 頁" HiddenPrevNextPagesOnInvalid="true" DisplayNumberCount="10" RepeatLayout="Flow" runat="server" />
                        <Paging:CustNextPaging ID="nextPaging" DisplayText="下一頁" runat="server" />
                        <Paging:CustLastPaging ID="lastPaging" DisplayText="最末頁" runat="server" />
                    </div>
                    <span>共
                        <asp:Literal ID="litTotalPageNumber" Text="0" runat="server" />
                        頁/
                        <asp:Literal ID="litSearchResulted" Text="0" runat="server" />
                        筆,&nbsp;每頁顯示
                        <asp:DropDownList ID="listCountOfPage" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="30" Value="30" />
                            <asp:ListItem Text="40" Value="40" />
                            <asp:ListItem Text="50" Value="50" />
                        </asp:DropDownList>
                        筆,
                        移至第&nbsp;<Paging:TextBoxPaging ID="textboxPaging" DisplayShape="HyperLink" DisplayText="" runat="server" />
                        &nbsp;頁
                    </span>
                </div>
                <Paging:ObserverPaging ID="observerPaging" RequestMethod="post" runat="server" />
            </asp:PlaceHolder>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>
</asp:Content>

