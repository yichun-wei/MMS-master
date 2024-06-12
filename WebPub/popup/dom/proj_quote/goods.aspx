<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="goods.aspx.cs" Inherits="popup_dom_proj_quote_goods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <asp:Button ID="btnInitPageInfo" runat="server" CssClass="display_none" OnClick="btnInitPageInfo_Click" />
    <div id="popup">
        <asp:PlaceHolder ID="phPGOrderConds" Visible="false" runat="server">
            <div class="searchBox">
                <table class="form">
                    <tr>
                        <th width="140">來源：</th>
                        <td>
                            <asp:DropDownList ID="lstSourceList" runat="server">
                                <asp:ListItem Text="專案報價" Value="" />
                            </asp:DropDownList>
                            <asp:Button ID="btnSearch" Text="查詢" runat="server" OnClick="Page_DoSearch" />
                            <asp:HiddenField ID="hidSearchConds" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:PlaceHolder>

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

        <div class="btnBox">
            <div>
                <div><a href="javascript:;" onclick="returnInfo();">新增商品</a></div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidContainPGOrder" Value="N" runat="server" />
    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />

    <script type="text/javascript">

        $(function () {

            $(".dev-sel-all").change(function () {
                var checked = $(this).is(":checked");
                $(".dev-sel").each(function () {
                    $(this).prop("checked", checked);
                });
            });

            $(".dev-sel-row").each(function () {
                $(this).click(function (event) {
                    if (!$(event.target).is("input:checkbox")) {
                        var sel = $(".dev-sel-cell input", $(this));
                        if (sel) {
                            sel.prop("checked", !sel.is(":checked"));
                        }
                    }
                });
            });
        });

        function initPageInfo() {
            var containPGOrder = opener.orderItemHelper.containPGOrder();
            if (containPGOrder) {
                $("#<%=this.hidContainPGOrder.ClientID%>").val(containPGOrder ? "Y" : "N");
            }

            var existedGoodsItems = opener.orderItemHelper.getExistedGoodsItems("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>");
            $("#<%=this.hidExistedGoodsItems.ClientID%>").val(existedGoodsItems);

            $("#<%=this.btnInitPageInfo.ClientID%>").trigger("click");
        }

        function returnInfo() {
            var seledItems = new Array();
            $(".dev-sel").each(function () {
                var checked = $(this).is(":checked");
                if (checked) {
                    seledItems.push($(this).val());
                }
            });

            opener.orderItemHelper.returnInfo("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>", seledItems);
            window.close();
        }
    </script>
</asp:Content>

