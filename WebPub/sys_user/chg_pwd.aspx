<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="chg_pwd.aspx.cs" Inherits="sys_user_chg_pwd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>修改密碼</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>

        <div id="edBox">
            <div>
                <div class="login_table">
                    <table>
                        <tr>
                            <td colspan="2">
                                <div class="logo">士林電機XS營銷管理系統</div>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <label>原密碼：</label></th>
                            <td>
                                <asp:TextBox TextMode="Password" ID="txtOldPwd" autocomplete="off" MaxLength="20" CssClass="disabled_ime_mode" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>
                                <label>新密碼：</label></th>
                            <td>
                                <asp:TextBox TextMode="Password" ID="txtPwd" autocomplete="off" MaxLength="20" CssClass="disabled_ime_mode" runat="server" /></td>
                        </tr>
                        <tr>
                            <th>
                                <label>確認新密碼：</label></th>
                            <td>
                                <asp:TextBox TextMode="Password" ID="txtPwdConfirm" autocomplete="off" MaxLength="20" CssClass="disabled_ime_mode" runat="server" /></td>
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
                        <div>
                            <asp:Button ID="btnSend" Text="確認" runat="server" OnClick="btnSend_Click" />
                        </div>
                        <div>
                            <input type="reset" value="清除" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- InstanceEndEditable -->
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>
</asp:Content>

