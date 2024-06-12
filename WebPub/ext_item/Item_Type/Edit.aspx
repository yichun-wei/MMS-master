<%@ Page Language="C#"  MasterPageFile="~/master_page/client.master"  AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="ext_item_Item_Type_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
    <script src="../../theme/common/js/jquery.maxlength-min.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
   
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷商品產品別：維護</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>  
        <div class="searchBox">
             <table class="ListTable" style="width: 50%"  align="center" > 
            <tr>
                <td style="width: 40%" align="right" >
                    <asp:Label ID="lblItemType" runat="server" Font-Size="20px" Text="產品別："></asp:Label>
                </td>
                <td align="left">
                    <asp:Label ID="lbltxtItemType" runat="server" Font-Size="20px" ></asp:Label>
                   
                </td>                
            </tr>
                   <tr>
                 <td style="width: 40%" align="right">
                    <asp:Label ID="lblChkActive" runat="server" Font-Size="20px" Text="有效註記：" ></asp:Label>
                </td>
                 <td  align="left">
                    <asp:CheckBox ID="CB_ACTIVE_FLAG" runat="server" AutoPostBack="True" />
                </td>                
            </tr>
            </table>
            <table class="form1" style="width: 50%" align="center">
               
               <tr>
                    <th  style="font-size: large">標題序號</th>
                     <th  style="font-size: large; text-align: left;">分類標題名稱</th>
                </tr>
                <tr>
                    <th>標題一：</th>
                    <td>
                        <asp:TextBox ID="txt_1" MaxLength="250" runat="server" /></td>
                        <asp:Label ID="lblUserSid" runat="server" Visible="False"  ></asp:Label>
                </tr>
                                <tr>
                    <th>標題二：</th>
                    <td>
                        <asp:TextBox ID="txt_2" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題三：</th>
                    <td>
                        <asp:TextBox ID="txt_3" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題四：</th>
                    <td>
                        <asp:TextBox ID="txt_4" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題五：</th>
                    <td>
                        <asp:TextBox ID="txt_5" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題六：</th>
                    <td>
                        <asp:TextBox ID="txt_6" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題七：</th>
                    <td>
                        <asp:TextBox ID="txt_7" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題八：</th>
                    <td>
                        <asp:TextBox ID="txt_8" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題九：</th>
                    <td>
                        <asp:TextBox ID="txt_9" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題十：</th>
                    <td>
                        <asp:TextBox ID="txt_10" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題十一：</th>
                    <td>
                        <asp:TextBox ID="txt_11" MaxLength="250" runat="server" /></td>
                </tr>
                                <tr>
                    <th>標題十二：</th>
                    <td>
                        <asp:TextBox ID="txt_12" MaxLength="250" runat="server" /></td>
                </tr>
              
            </table>
                       

        </div>     
            <div class="btnBox">
                <div>
                   
                        <div><asp:LinkButton ID="btnSave" Text="儲存" Visible="false" runat="server" OnClick="btnSave_Click" /></div>
                        <div><a href="View.aspx" title="回上一頁">回上一頁</a></div>
                   
                </div>
            </div>
      
        <!-- InstanceEndEditable -->
 </div> 
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

</asp:Content>

