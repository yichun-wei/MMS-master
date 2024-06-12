<%@ Page Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="ext_item_Items_View " %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" Runat="Server">
        <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷商品料號</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>       
           
            <div class="searchBox">
            <table class="form">
                <tr>
                    <th>外銷商品產品別</th>
                    <td>
                        <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="lstItemType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstItemType_SelectedIndexChanged">
                                    <asp:ListItem Text="[請選擇]" Value="" />
                                </asp:DropDownList>
                                <asp:DropDownList ID="lstCat_1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCat_1_SelectedIndexChanged" Visible="false" />
                                <asp:DropDownList ID="lstCat_2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCat_2_SelectedIndexChanged" Visible="false" />
                                <asp:DropDownList ID="lstCat_3" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCat_3_SelectedIndexChanged" Visible="false" />
                                <asp:DropDownList ID="lstCat_4" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstCat_4_SelectedIndexChanged" Visible="false" />
                                <asp:DropDownList ID="lstCat_5" runat="server" Visible="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>                
            </table>
        </div>
             <div class="btnBox">
                <div>
                   
                        <div><asp:LinkButton ID="btnSearch" Text="查詢" runat="server" OnClick="btnSearch_Click" /></div>
                       <div><asp:HyperLink ID="lnkAdd" Text="新增"  runat="server" /></div>
                         <asp:HiddenField ID="hidSearchConds" runat="server" />
                   
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

    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />

    <asp:PlaceHolder ID="phSingleModeJS" runat="server">
        <script type="text/javascript">
            $(function () {
                $(".dev-sel-row").each(function () {
                    $(this).click(function (event) {
                        var partno = $(".dev-sel-cell input", $(this)).attr("partno");
                        if (partno) {
                            
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
</asp:Content>
