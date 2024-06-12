<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="popup_dom_proj_quote_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="popup">
        <div class="ListTable_title">選擇專案報價</div>

        <asp:Button ID="btnSearch" Text="查詢" runat="server" CssClass="display_none" OnClick="Page_DoSearch" />
        <asp:HiddenField ID="hidSearchConds" runat="server" />

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
                <div><a href="javascript:;" onclick="returnInfo();">匯入報價單</a></div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidExistedProjQuotes" runat="server" />

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

        function getExistedProjQuotes() {
            var existedProjQuotes = opener.projQuoteHelper.getExistedProjQuotes();
            $("#<%=this.hidExistedProjQuotes.ClientID%>").val(existedProjQuotes);
            $("#<%=this.btnSearch.ClientID%>").trigger("click");
        }

        function returnInfo() {
            var seledItems = new Array();
            $(".dev-sel").each(function () {
                var checked = $(this).is(":checked");
                if (checked) {
                    seledItems.push($(this).val());
                }
            });

            opener.projQuoteHelper.returnInfo(seledItems);
            window.close();
        }
    </script>
</asp:Content>

