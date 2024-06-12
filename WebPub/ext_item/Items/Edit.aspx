<%@ Page Language="C#"  MasterPageFile="~/master_page/client.master"  AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="ext_item_Items_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
    <link href="../../theme/client/css/print.css" rel="stylesheet" type="text/css" media="print" />
    <script src="../../theme/common/js/jquery.maxlength-min.js" type="text/javascript"></script>
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
   
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->

        <div class="Maintitle">
            <h1>外銷商品料號：維護</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>  
        <div class="searchBox">
             <table class="ListTable" style="width: 50%"  align="center" > 
            <tr>
                 <td style="width: 40%" align="right" >
                    <asp:Label ID="lblextItem" runat="server" Font-Size="20px" Text="外銷型號："></asp:Label>
                </td>
                 <td align="left">
                    <asp:Label ID="lbltxtextItem" runat="server" Font-Size="20px"></asp:Label>
                </td>
                 </tr>
                 <tr>
                <td style="width: 40%" align="right" >
                    <asp:Label ID="lblItemType" runat="server" Font-Size="20px" Text="產品別："></asp:Label>
                </td>
                <td align="left">
                    <asp:Label ID="lbltxtItemType" runat="server" Font-Size="20px" ></asp:Label>
                   
                </td>                
            </tr>
                    <tr>
                <td style="width: 40%" align="right" >
                    <asp:Label ID="lblErpItem" runat="server" Font-Size="20px" Text="ERP料號："></asp:Label>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtErpItem" runat="server" Font-Size="20px" Width="260px" ></asp:TextBox>
                   
                </td>                
            </tr>
                   <tr>
                 <td style="width: 40%" align="right">
                    <asp:Label ID="lblChkActive" runat="server" Font-Size="20px" Text="有效註記：" ></asp:Label>
                </td>
                       
                 <td  align="left">
                   
                     <asp:UpdatePanel runat="server">
                        <ContentTemplate>
                          <asp:CheckBox ID="CB_ACTIVE_FLAG" runat="server" AutoPostBack="True" />
                        </ContentTemplate>
                      </asp:UpdatePanel>
                </td>                
                             
            </tr>
            </table>      
       <asp:Label ID="lblTxtCount" runat="server"  Visible="False"></asp:Label>
       <asp:Label ID="lblUserSid" runat="server" Visible="False"  ></asp:Label>
       <asp:Label ID="lblErpChanged" runat="server"  Visible="False"></asp:Label>
       <asp:Label ID="lblErpItemId" runat="server"  Visible="False"></asp:Label>        

        </div>     
         <div align="center">
               <table class="ListTable" style="width: 50%" align="center" > 
              <asp:PlaceHolder ID="phPageList" runat="server" />
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

