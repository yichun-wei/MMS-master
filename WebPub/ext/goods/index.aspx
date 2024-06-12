<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="ext_goods_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
    <script src="../../theme/common/js/jquery.maxlength-min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <input type="submit" id="btnPostBack" class="display_none" />
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷商品</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">關鍵字查詢：</th>
                    <td width="370">
                        <asp:TextBox ID="txtKeyword" runat="server" /></td>
                    <th width="140">狀態：</th>
                    <td>
                        <asp:DropDownList ID="lstModelStatus" runat="server">
                            <asp:ListItem Text="全部" Value="" />
                            <asp:ListItem Text="已建立型號" Value="Y" />
                            <asp:ListItem Text="未建立型號" Value="N" />
                        </asp:DropDownList>
                    </td>
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
            </table>
            <div class="btnBox searchBtn">
                <div>
                    <div>
                        <asp:Button ID="btnSearch" Text="查詢" runat="server" OnClick="btnSearch_Click" />
                        <asp:HiddenField ID="hidSearchConds" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="list_box">
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
                    <div>
                        <asp:Button ID="btnUpdateModel" Text="儲存修改" runat="server" />
                    </div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        $(function () {
            $(".dev-model").maxlength({
                maxCharacters: 100,
                status: false
            });
        });
    </script>

    <input type="hidden" id="hidModelBuff" name="hidModelBuff" />
    <script type="text/javascript">
        //<![CDATA[
        function ChangingModel() {

            var jDataCount = modelData.length;
            for (var i = 0; i < jDataCount; i++) {
                modelData[i]["NewVal"] = $("#txtModel_" + modelData[i]["SId"]).val();
            }

            var hidden = $("#hidModelBuff");
            hidden.val(JSON2.stringify(modelData));

            document.form[0].submit();
        }
        //]]>
    </script>
</asp:Content>

