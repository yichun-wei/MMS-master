<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="goods.aspx.cs" Inherits="popup_ext_goods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" Runat="Server">
    <asp:Button ID="btnInitPageInfo" runat="server" CssClass="display_none" OnClick="btnInitPageInfo_Click" />
    <div id="popup">
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="200">關鍵字查詢：</th>
                    <td colspan="2">
                        <asp:TextBox ID="txtKeyword" runat="server" /></td>
                </tr>
                <tr>
                    <th>分類查詢：</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="lstCatList_1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCatList_1_SelectedIndexChanged">
                                    <asp:ListItem Text="請選擇" Value="" />
                                </asp:DropDownList>
                                <asp:DropDownList ID="lstCatList_2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCatList_2_SelectedIndexChanged" />
                                <asp:DropDownList ID="lstCatList_3" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <th></th>
                    <td>
                        <asp:Button ID="btnSearch" Text="查詢" runat="server" OnClick="btnSearch_Click" />
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

        <asp:PlaceHolder ID="phMultiAdd" runat="server">
            <div class="btnBox">
                <div>
                    <div><a href="javascript:;" onclick="returnMultiInfo();">新增商品</a></div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />

    <asp:PlaceHolder ID="phSingleModeJS" runat="server">
        <script type="text/javascript">
            $(function () {
                $(".dev-sel-row").each(function () {
                    $(this).click(function (event) {
                        var partno = $(".dev-sel-cell input", $(this)).attr("partno");
                        if (partno) {
                            <%--用 trigger 會慢--%>
                            <%--returnSingleInfo($(".dev-sel-cell input", $(this)).trigger("click"));--%>
                            returnSingleInfo(partno);
                        }
                    });
                });
            });
        </script>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phMultiModeJS" runat="server">
        <script type="text/javascript">
            $(function () {
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
        </script>
    </asp:PlaceHolder>

    <script type="text/javascript">
        function initPageInfo() {
            var existedGoodsItems = opener.orderItemHelper.getExistedGoodsItems("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>");
            $("#<%=this.hidExistedGoodsItems.ClientID%>").val(existedGoodsItems);
            $("#<%=this.btnInitPageInfo.ClientID%>").trigger("click");
        }

        function returnSingleInfo(seledItem) {
            opener.orderItemHelper.returnInfo("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>", seledItem);
            window.close();
        }

        function returnMultiInfo() {
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

