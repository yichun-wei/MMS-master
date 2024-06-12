<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client_popup.master" AutoEventWireup="true" CodeFile="order_goods.aspx.cs" Inherits="popup_ext_shipping_order_order_goods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    <asp:Button ID="btnInitPageInfo" runat="server" CssClass="display_none" OnClick="btnInitPageInfo_Click" />
    <div id="popup">
        <asp:PlaceHolder ID="phPageList" runat="server" />

        <div class="btnBox">
            <div>
                <div><a href="javascript:;" onclick="returnMultiInfo();">新增商品</a></div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />

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
    </script>

    <script type="text/javascript">
        function initPageInfo() {
            var existedGoodsItems = opener.orderItemHelper.getExistedGoodsItems("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>");
            $("#<%=this.hidExistedGoodsItems.ClientID%>").val(existedGoodsItems);
            $("#<%=this.btnInitPageInfo.ClientID%>").trigger("click");
        }

        function returnMultiInfo() {
            var seledItems = new Array();
            $(".dev-sel").each(function () {
                var checked = $(this).is(":checked");
                if (checked) {
                    seledItems.push($(this).val());
                }
            });

            opener.orderItemHelper.returnInfo("<%=EzCoding.ConvertLib.ToStr(this.ContainerSId)%>", seledItems);
            window.close();
        }
    </script>
</asp:Content>

