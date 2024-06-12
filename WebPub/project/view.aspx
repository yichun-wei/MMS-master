<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="project_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
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
        <div class="Maintitle">
            <h1>CRM專案查詢</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">報價單編號：</th>
                    <td width="370">
                        <asp:Literal ID="litQuoteNumber" runat="server" /></td>
                    <th width="140">報價單日期：</th>
                    <td>
                        <asp:Literal ID="litQuoteDate" runat="server" /></td>
                </tr>
                <tr>
                    <th width="140">客戶編號：</th>
                    <td width="370">
                        <asp:Literal ID="litCustomerId" runat="server" /></td>
                    <th width="140">報價單抬頭：</th>
                    <td>
                        <asp:Literal ID="litQuoteTitle" runat="server" /></td>
                </tr>
                <tr>
                    <th width="140">客戶名稱：</th>
                    <td width="370">
                        <asp:Literal ID="litCustomerName" runat="server" /></td>
                    <th width="140">報價單備註：</th>
                    <td>
                        <asp:Literal ID="litRemark" runat="server" /></td>
                </tr>
            </table>
            <div class="searchBox">
          
        </div>
            <div class="list_box">
            <asp:PlaceHolder ID="phPageList" runat="server" />
            <div class="btn_top"><a href="javascript:void(0)">top</a></div>               
            <div class="btnBox">
                   <div><a href="index.aspx" title="回上一頁">回上一頁</a></div>
<%--                <div><a href="javascript:void(0);" onclick="javascript:window.history.go(-1);" title="回上一頁">回上一頁</a></div>--%>
            </div>
            </div>
            <asp:HiddenField ID="hidSearchConds" runat="server" />
            <asp:HiddenField ID="hidIsAllDomDist" Value="Y" runat="server" />
            <asp:HiddenField ID="hidPriceListId" runat="server" />
        </div>
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

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

