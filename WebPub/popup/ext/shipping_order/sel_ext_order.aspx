<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="sel_ext_order.aspx.cs" Inherits="popup_ext_shipping_order_sel_ext_order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <div id="popup">
        <div class="ListTable_title">選擇訂單</div>

        <asp:Button ID="btnSearch" Text="查詢" runat="server" CssClass="display_none" OnClick="Page_DoSearch" />
        <asp:PlaceHolder ID="phPageList" runat="server" />

        <div class="btnBox">
            <div>
                <div><a href="javascript:;" onclick="returnInfo();">匯入訂單</a></div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidExistedExtOrders" runat="server" />

    <script type="text/javascript">

        $(function () {
            $(".dev-sel-row").each(function () {
                $(this).click(function (event) {
                    if (!$(event.target).is("input:checkbox")) {
                        var sel = $(".dev-sel-cell input", $(this));
                        if (sel) {
                            sel.prop("checked", !sel.is(":checked"));
                        }
                    }
                });
            });
        });

        function getExistedExtOrders() {
            var existedExtOrders = opener.extOrderHelper.getExistedExtOrders();
            $("#<%=this.hidExistedExtOrders.ClientID%>").val(existedExtOrders);
            $("#<%=this.btnSearch.ClientID%>").trigger("click");
        }

        function returnInfo() {
            var seledItems = new Array();
            $(".dev-sel").each(function () {
                var checked = $(this).is(":checked");
                if (checked) {
                    seledItems.push($(this).val());
                }
            });

            opener.extOrderHelper.returnInfo(seledItems);
            window.close();
        }
    </script>
</asp:Content>

