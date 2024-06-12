<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="view.aspx.cs" Inherits="dom_pg_order_view" %>

<%@ Reference Control="~/include/client/dom/pg_order/view/goods_block.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
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
                        <asp:Literal ID="litDomDistName" runat="server" /></td>
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
                        <asp:Literal ID="litEdd" runat="server" /></td>
                </tr>
                <tr>
                    <th>備貨單狀態：</th>
                    <td>
                        <asp:Literal ID="litStatus" runat="server" /></td>
                    <th>備註：</th>
                    <td>
                        <asp:Literal ID="litRmk" runat="server" /></td>
                </tr>
                <tr>
                    <th>客戶名稱：</th>
                    <td>
                        <asp:Literal ID="litCustomerName" runat="server" /></td>
                </tr>
                <asp:PlaceHolder ID="phProjQuoteIndex" Visible="false" runat="server">
                    <tr>
                        <th>專案報價：</th>
                        <td>
                            <ul class="keyword_list">
                                <asp:Literal ID="litProjQuoteIdxes" runat="server" />
                            </ul>
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </div>
        <div class="rondane_box">
            <asp:PlaceHolder ID="phGoodsList" runat="server" />

            <div class="btnBox">
                <div>
                    <div>
                        <asp:LinkButton ID="btnDelete" Text="刪除" Visible="false" runat="server" OnClick="btnDelete_Click" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnCancel" Text="取消" Visible="false" runat="server" OnClick="btnCancel_Click" />
                    </div>
                    <div><a href="index.aspx">回到備貨單列表</a></div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>
</asp:Content>

