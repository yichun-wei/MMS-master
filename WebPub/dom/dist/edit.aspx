<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_edit.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="dom_dist_edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <asp:HiddenField ID="hidSpecSId" runat="server" />
    <div id="popup">
        <div class="searchBox">
            <table class="form1">
                <tr>
                    <th>代碼：</th>
                    <td>
                        <Ajax:FilteredTextBoxExtender TargetControlID="txtCode" FilterType="Numbers,LowercaseLetters,UppercaseLetters" runat="server" />
                        <asp:TextBox ID="txtCode" MaxLength="4" runat="server" /></td>
                </tr>
                <tr>
                    <th>地區：</th>
                    <td>
                        <asp:TextBox ID="txtName" MaxLength="20" runat="server" /></td>
                </tr>
                <tr>
                    <th>公司名稱：</th>
                    <td>
                        <asp:TextBox ID="txtCompName" MaxLength="100" runat="server" /></td>
                </tr>
                <tr>
                    <th>TEL：</th>
                    <td>
                        <asp:TextBox ID="txtTel" MaxLength="25" runat="server" /></td>
                </tr>
                <tr>
                    <th>FAX：</th>
                    <td>
                        <asp:TextBox ID="txtFax" MaxLength="25" runat="server" /></td>
                </tr>
                <tr>
                    <th>地址：</th>
                    <td>
                        <asp:TextBox ID="txtAddr" MaxLength="240" runat="server" /></td>
                </tr>
                <tr>
                    <th>排序：</th>
                    <td>
                        <asp:TextBox ID="txtSort" Text="5000" Width="50" MaxLength="4" CssClass="txt_number" runat="server" onkeyup="ValidateNumber(this,value);" /></td>
                </tr>
            </table>
        </div>

        <Client:EditDataTransLog ID="ucEditDataTransLog" Visible="false" runat="server" />

        <div class="btnBox">
            <div>
                <div>
                    <asp:Button ID="btnSend" Text="送出" runat="server" OnClick="btnSend_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

