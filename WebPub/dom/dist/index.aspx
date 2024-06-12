<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="dom_dist_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" Runat="Server">
    <input type="submit" id="btnPostBack" class="display_none" />
    <div id="center">
    	<!-- InstanceBeginEditable name="content" -->
    	<div class="Maintitle">
            <h1>內銷地區</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        
        <div class="rondane_box">
        	<div class="btnBox"><div>
            	<div><asp:LinkButton ID="btnAdd" Text="新增" runat="server" /></div>
            	<div><asp:LinkButton ID="btnDelete" Text="刪除" runat="server" OnClick="btnDelete_Click" /></div>
            	<div><asp:LinkButton ID="btnUpdateSort" Text="修改排序" runat="server" /></div>
            </div></div>
            
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
    <Common:DataSorting ID="ucDataSorting" runat="server" />
</asp:Content>

