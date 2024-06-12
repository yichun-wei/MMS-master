<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="customer.aspx.cs" Inherits="popup_customer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="popup">
        <div class="searchBox">
            <div class="btnBox">
                <div>
                    <div>
                        <asp:LinkButton ID="btnUpdateErpCuster" Text="更新資料" runat="server" OnClick="btnUpdateErpCuster_Click" />
                    </div>
                </div>
            </div>
            <table class="form">
                <tr>
                    <th>客戶名稱：</th>
                    <td>
                        <asp:TextBox ID="txtKeyword" runat="server" /><asp:Button ID="btnSearch" Text="查詢" runat="server" OnClick="Page_DoSearch" /><br />
                        <asp:CheckBoxList ID="chklKeywordCols" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Value="CUSTOMER_NUMBER" Text="客戶編號" Selected="true" />
                            <asp:ListItem Value="CUSTOMER_NAME" Text="客戶名稱" Selected="true" />
                        </asp:CheckBoxList>
                        <asp:HiddenField ID="hidSearchConds" runat="server" />
                    </td>
                </tr>
            </table>
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

    <script type="text/javascript">
        $(function () {
            $(".dev-sel-row").each(function () {
                $(this).click(function (event) {
                    <%--用 trigger 會慢--%>
                    <%--returnInfo($(".dev-sel-cell input", $(this)).trigger("click"));--%>
                    returnInfo($(".dev-sel-cell input", $(this)).attr("customerid"));
                });
            });
        });

        function returnInfo(customerId) {
            opener.customerHelper.returnInfo(customerId);
            window.close();
        }
    </script>
</asp:Content>

