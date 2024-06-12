<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>後端管理</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <asp:PlaceHolder ID="phDomOrderSummary" runat="server">
            <fieldset class="treeBox">
                <legend>內銷</legend>
                <ul class="d_list d_list1">
                    <%--備貨單-未使用--%>
                    <asp:PlaceHolder ID="phPGOrderUnusedCnt" runat="server">
                        <li class="d_l1">
                            <asp:HyperLink ID="lnkPGOrderUnusedCnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--營管待審核--%>
                    <asp:PlaceHolder ID="phDomOrder_Status_1_Cnt" runat="server">
                        <li class="d_l2">
                            <asp:HyperLink ID="lnkDomOrder_Status_1_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--財務待審核--%>
                    <asp:PlaceHolder ID="phDomOrder_Status_3_Cnt" runat="server">
                        <li class="d_l3">
                            <asp:HyperLink ID="lnkDomOrder_Status_3_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--未付款待審核--%>
                    <asp:PlaceHolder ID="phDomOrder_Status_4_Cnt" runat="server">
                        <li class="d_l4">
                            <asp:HyperLink ID="lnkDomOrder_Status_4_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--備貨中--%>
                    <asp:PlaceHolder ID="phDomOrder_Status_7_Cnt" runat="server">
                        <li class="d_l5">
                            <asp:HyperLink ID="lnkDomOrder_Status_7_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                </ul>
            </fieldset>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phExtOrderSummary" runat="server">
            <fieldset class="treeBox">
                <legend>外銷</legend>
                <ul class="d_list d_list2">
                    <%--報價單草稿--%>
                    <asp:PlaceHolder ID="phExtQuonDraftCnt" runat="server">
                        <li class="d_l1"><asp:HyperLink ID="lnkExtQuonDraftCnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--待轉為訂單--%>
                    <asp:PlaceHolder ID="phExtOrder_Status_1_Cnt" runat="server">
                        <li class="d_l2"><asp:HyperLink ID="lnkExtOrder_Status_1_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--正式訂單未排程--%>
                    <asp:PlaceHolder ID="phExtOrder_Status_3_Cnt" runat="server">
                        <li class="d_l3"><asp:HyperLink ID="lnkExtOrder_Status_3_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--出貨單已確認(未出貨)--%>
                    <asp:PlaceHolder ID="phExtShippingOrder_Status_1_Cnt" runat="server">
                        <li class="d_l5"><asp:HyperLink ID="lnkExtShippingOrder_Status_1_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                    <%--出貨單已出貨(未上傳)--%>
                    <asp:PlaceHolder ID="phExtShippingOrder_Status_2_Cnt" runat="server">
                        <li class="d_l6"><asp:HyperLink ID="lnkExtShippingOrder_Status_2_Cnt" runat="server" /></li>
                    </asp:PlaceHolder>
                </ul>
            </fieldset>
        </asp:PlaceHolder>
        <ul class="last_info">
            <li>最後登入時間：<asp:Literal ID="litLastLoginDT" runat="server" /></li>
            <li>最後登入IP：<asp:Literal ID="litLastLoginIP" runat="server" /></li>
        </ul>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>
</asp:Content>

