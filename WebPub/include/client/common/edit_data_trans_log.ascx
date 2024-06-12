<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit_data_trans_log.ascx.cs" Inherits="include_client_common_edit_data_trans_log" %>
<div class="rondane_box">
    <div class="ListTable_title1">異動紀錄</div>
    <table class="form1">
        <tr>
            <th>建立人：</th>
            <td>
                <asp:Literal ID="litCreatorName" runat="server" /></td>
        </tr>
        <tr>
            <th>建立時間：</th>
            <td>
                <asp:Literal ID="litCreateDT" runat="server" /></td>
        </tr>
        <tr>
            <th>最後修改人：</th>
            <td>
                <asp:Literal ID="litLastModifierName" runat="server">&nbsp;</asp:Literal></td>
        </tr>
        <tr>
            <th>最後修改時間：</th>
            <td>
                <asp:Literal ID="litLastModifyDT" runat="server">&nbsp;</asp:Literal></td>
        </tr>
    </table>
</div>
