<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_edit.master" AutoEventWireup="true" CodeFile="edit.aspx.cs" Inherits="sys_user_edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../theme/client/css/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <script src="../theme/client/js/jquery.treeview.min.js" type="text/javascript"></script>
    <script src="../theme/client/js/tree_ready.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="popup">
        <div class="searchBox">
            <table class="form1">
                <tr>
                    <th>姓名：</th>
                    <td>
                        <asp:TextBox ID="txtName" MaxLength="20" runat="server" /></td>
                </tr>
                <tr>
                    <th>帳號：</th>
                    <td>
                        <asp:TextBox ID="txtAcct" MaxLength="20" runat="server" /></td>
                </tr>
                <tr>
                    <th>密碼：</th>
                    <td>
                        <asp:TextBox TextMode="Password" ID="txtPwd" autocomplete="off" MaxLength="20" CssClass="disabled_ime_mode" runat="server" />
                        <br />
                        <asp:Literal ID="litPwdNote" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>電子信箱：</th>
                    <td>
                        <asp:TextBox ID="txtEmail" MaxLength="100" runat="server" /></td>
                </tr>
                <tr>
                    <th>部門：</th>
                    <td>
                        <asp:TextBox ID="txtDept" MaxLength="50" runat="server" /></td>
                </tr>
                <tr>
                    <th>內銷地區：</th>
                    <td>
                        <asp:CheckBoxList ID="chklDomDistList" RepeatLayout="Flow" RepeatDirection="Horizontal" RepeatColumns="5" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th>內銷審核：</th>
                    <td>
                        <asp:CheckBoxList ID="chklDomAuditPerms" RepeatLayout="Flow" RepeatDirection="Horizontal" RepeatColumns="5" runat="server">
                            <asp:ListItem Text="營管" Value="1" />
                            <asp:ListItem Text="財務" Value="2" />
                            <asp:ListItem Text="副總" Value="3" />
                            <asp:ListItem Text="倉管" Value="4" />
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <th>外銷審核：</th>
                    <td>
                        <asp:CheckBoxList ID="chklExtAuditPerms" RepeatLayout="Flow" RepeatDirection="Horizontal" RepeatColumns="5" runat="server">
                            <asp:ListItem Text="外銷組" Value="1" />
                        </asp:CheckBoxList>
                    </td>
                </tr>
            </table>
        </div>

        <asp:PlaceHolder ID="phUserRight" runat="server">
            <fieldset class="treeBox">
                <legend>使用者權限</legend>
                <ul id="tree" class="treeview">
                    <asp:Literal ID="litAuthItems" runat="server" />
                </ul>
            </fieldset>
        </asp:PlaceHolder>

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

