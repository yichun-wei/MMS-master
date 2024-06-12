<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="zh-TW" lang="zh-TW">
<head runat="server">
    <Client:HeadTopFixed ID="ucHeadTopFixed" runat="server" />
    <title></title>
    <Client:HeadFixed ID="ucHeadFixed" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <Ajax:ToolkitScriptManager runat="server" />
        <div id="loginBox">
            <div>
                <div class="login_table">
                    <table>
                        <tr>
                            <td colspan="2">
                                <div class="logo">士林電機 XS 營銷管理系統</div>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <label>帳號：</label></th>
                            <td>
                                <asp:TextBox ID="txtAcct" MaxLength="50" TabIndex="1" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>
                                <label>密碼：</label></th>
                            <td>
                                <asp:TextBox ID="txtPwd" TextMode="Password" autocomplete="off" MaxLength="20" TabIndex="2" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>
                                <label>驗證碼：</label></th>
                            <td class="randomcode">
                                <asp:UpdatePanel UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <div class="randomcode">
                                            <div>
                                                <asp:TextBox ID="txtVerifyCode" CssClass="txt" MaxLength="4" TabIndex="3" runat="server" />
                                            </div>
                                            <div>
                                                <asp:Image ID="imgVerifyCode" ImageUrl="~/common/verify_code.aspx" Width="90" Height="30" runat="server" />
                                            </div>
                                            <div class="btn">
                                                <input type="button" value="更換認證碼" class="refresh" onclick="resetVerifyCode();">
                                                <asp:Button ID="btnRegetVerifyCode" CssClass="display_none" runat="server" OnClick="btnRegetVerifyCode_Click" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <script type="text/javascript">
                                    function resetVerifyCode() {
                                        DoClick('<%=this.btnRegetVerifyCode.ClientID%>');
                                    }
                                </script>
                            </td>
                        </tr>
                    </table>
                    <div class="btnBox">
                        <asp:Button ID="btnLogin" Text="登入" CssClass="btn" TabIndex="4" runat="server" OnClick="btnLogin_Click" />
                    </div>
                </div>
            </div>
        </div>
        <Client:PageFooter ID="ucPageFooter" runat="server" />
    </form>
</body>
</html>
