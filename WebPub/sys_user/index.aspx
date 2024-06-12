<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="sys_user_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <input type="submit" id="btnPostBack" class="display_none" />
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>權限</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">地區：</th>
                    <td width="370">
                        <asp:DropDownList ID="lstDomDistList" runat="server">
                            <asp:ListItem Text="全部" Value="" />
                        </asp:DropDownList>
                    </td>
                    <th width="140">分類：</th>
                    <td>
                        <asp:DropDownList ID="lstMktgRangeList" runat="server">
                            <asp:ListItem Text="全部" Value="" />
                            <asp:ListItem Text="內銷" Value="1" />
                            <asp:ListItem Text="外銷" Value="2" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th>關鍵字：</th>
                    <td>
                        <asp:TextBox ID="txtKeyword" runat="server" />
                        <br />
                        <asp:CheckBoxList ID="chklKeywordCols" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Text="姓名" Value="NAME" Selected="True" />
                            <asp:ListItem Text="帳號" Value="ACCT" Selected="True" />
                        </asp:CheckBoxList>
                    </td>
                    <th></th>
                    <td>
                    </td>
                </tr>
            </table>
            <div class="btnBox searchBtn">
                <div>
                    <div>
                        <asp:Button ID="btnSearch" Text="搜尋" runat="server" OnClick="Page_DoSearch" />
                        <asp:HiddenField ID="hidSearchConds" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="list_box">
            <div class="btnBox">
                <div>
                    <div>
                        <asp:LinkButton ID="btnAdd" Text="新增" runat="server" />
                    </div>
                    <div>
                        <asp:LinkButton ID="btnDelete" Text="刪除" runat="server" OnClick="btnDelete_Click" /></div>
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

