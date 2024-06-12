<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="report_index" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>報表查詢</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">報表選項：</th>
                    <td>
                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" DataValueField="CategoryPath" DataTextField="CategoryDesc" AppendDataBoundItems="True">
                            <asp:ListItem Text="-- 請選擇報表分類 --" Value="0" />
                        </asp:DropDownList>


                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnInfo %>"
                            SelectCommand="SELECT [CategoryID], [CategoryPath], [CategoryDesc] FROM [dbo].[XS_REPORT_CATEGORY] WHERE INACTIVE IS NULL ORDER BY [CATEGORYID]" />

                        <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged" DataValueField="ReportPath" DataTextField="ReportDesc" AppendDataBoundItems="True">
                            <asp:ListItem Text="-- 請選擇報表名稱 --" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="btnBox searchBtn">
                <div>
                    <div>
                        <asp:Button ID="btnSearch" Text="搜尋" runat="server" OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="list_box">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="600px" />
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <asp:TextBox ID="TextBox1" runat="server" Height="20px" Style="font-size: small; margin-top: 0px" Width="20px" Visible="False" />
    <asp:TextBox ID="TextBox2" runat="server" Visible="False" Height="20px" Style="margin-bottom: 0px" Width="20px" />
    <asp:TextBox ID="CID" runat="server" Visible="False" Height="20px" Style="margin-bottom: 0px" Width="20px" />
</asp:Content>

