<%@ Control Language="C#" AutoEventWireup="true" CodeFile="header_discount_block.ascx.cs" Inherits="include_client_dom_order_header_discount_block" %>
<div>
    <%--<Ajax:FilteredTextBoxExtender TargetControlID="txtDiscount" FilterType="Numbers" runat="server" />--%>
    <asp:CheckBox ID="chkSel" CssClass="dev-header-discount-check" runat="server" /><asp:TextBox ID="txtDiscount" MaxLength="6" onkeyup="ValidateFloat1(this,value); headerDiscountHelper.checkDiscount($(this));" runat="server" />%
    <asp:HiddenField ID="hidDiscountId" runat="server" />
</div>
