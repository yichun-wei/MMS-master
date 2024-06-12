<%@ Page Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="ext_item_Item_Type_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
    <script src="../../theme/common/js/jquery.maxlength-min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
   
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷商品產品別</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>       
           
            <div class="btnBox">
                <div>
                    <div>
                        <asp:HyperLink ID="lnkAdd" Text="新增產品別" runat="server" /></div>
                </div>
            </div>
        <div class="list_box">
              <asp:PlaceHolder ID="phPageList" runat="server" />
        </div> 
        <!-- InstanceEndEditable -->
 </div> 
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        $(function () {
            $(".dev-model").maxlength({
                maxCharacters: 100,
                status: false
            });
        });
    </script>

    <input type="hidden" id="hidModelBuff" name="hidModelBuff" />
    <script type="text/javascript">
        //<![CDATA[
        function ChangingModel() {

            var jDataCount = modelData.length;
            for (var i = 0; i < jDataCount; i++) {
                modelData[i]["NewVal"] = $("#txtModel_" + modelData[i]["SId"]).val();
            }

            var hidden = $("#hidModelBuff");
            hidden.val(JSON2.stringify(modelData));

            document.form[0].submit();
        }
        //]]>
    </script>
</asp:Content>
