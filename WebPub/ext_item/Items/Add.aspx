<%@ Page Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="Add.aspx.cs" Inherits="ext_item_Items_Add " %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" runat="Server">
    
    <div id="center">
        <!-- InstanceBeginEditable name="content" -->
        <div class="Maintitle">
            <h1>外銷商品料號：新增</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>

        <div class="searchBox">
            <table class="ListTable" style="width: 50%"  align="center">
                <tr>
                    <td>外銷商品產品別</td>
                    <td>
                       
                                <asp:DropDownList ID="lstItemType" runat="server"  Width="260px" OnSelectedIndexChanged="lstItemType_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Text="[請選擇]" Value="" />
                                </asp:DropDownList>

                          
                    </td>
                </tr>
                <tr>
                     <td>外銷型號</td>
               
                 <td align="left">
                    <asp:TextBox ID="txtExtItem" runat="server"  Width="260px"></asp:TextBox>
                </td>
                 </tr>
               
                  <tr>
                       <td>ERP料號</td>
                
                <td align="left">
                    <asp:TextBox ID="txtErpItem" runat="server" Width="260px" ></asp:TextBox>
                   
                </td>                
            </tr>
                   <tr>
                        <td>有效註記</td>
                   <td  align="left">
                    <asp:CheckBox ID="CB_ACTIVE_FLAG"  runat="server" AutoPostBack="True" Checked="True" />
                </td>                
            </tr>
            </table>
       <asp:Label ID="lblTxtCount" runat="server"  Visible="False"></asp:Label>
       <asp:Label ID="lblErpItemId" runat="server"  Visible="False"></asp:Label>
       <asp:Label ID="lblUserSid" runat="server" Visible="False"  ></asp:Label>
        </div>
        <div align="center">
               <table class="ListTable" style="width: 50%" align="center" > 
              <asp:PlaceHolder ID="phPageList" runat="server" />
              </table>
        </div> 
        <div class="btnBox">
            <div>

                <div>
                    <asp:LinkButton ID="btnSave" Text="儲存" Visible="false" runat="server" OnClick="btnSave_Click" /></div>
                <div><a href="View.aspx" title="回上一頁">回上一頁</a></div>
                <asp:HiddenField ID="hidSearchConds" runat="server" />

            </div>
        </div>

    </div>

    <asp:HiddenField ID="hidExistedGoodsItems" runat="server" />
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

</asp:Content>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Add.aspx.cs" Inherits="ext_item_Items_Add"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>外銷商品產品別明細維護 | 士林電機 XS 營銷管理系統</title>
    <link href="/theme/client/css/style.css" rel="stylesheet" type="text/css" />
    <link href="/theme/client/css/dev_cust.css" rel="stylesheet" type="text/css" />
    <link type="text/css" rel="stylesheet" href="/theme/common/css/common.css" />
    <script src="jquery-tablepage-1.0.js"></script>
    <script type="text/javascript" src="/theme/client/js/jquery-1.4.2.min.js"></script>
    <script type="text/javascript">
        function ShowValue() 
        {
            var mums = document.getElementById('FROMTAG').getElementsByTagName('input').length;
            var j = 0;
            for (i = 1; i <= mums; i=i+1)
            {
            var v = document.getElementById('txt_'+i).value;
            if (v == '') {
                j++;
                if (mums == j) {
                    alert('明細內容請至少輸入一項');
                        window.event.returnValue = false;
                    }
                }
                else
                    return true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="header" >
        <div>
            <div class="logo"><a href="http://mmstest.xseec.cn/index.aspx">士林電機營銷系統</a></div>
            <div class="mem">魏儀淳 (12842)，您好<a href="http://mmstest.xseec.cn/logout.aspx">登出</a></div>
        </div>
    </div>

    <div id="Menu">
        <ul class="top_menu"><li><a title="內銷訂單" href="/dom/order/index.aspx">內銷訂單</a></li>
            <li><a title="內銷備貨單" href="/dom/pg_order/index.aspx">內銷備貨單</a></li>
            <li><a title="外銷報價單" href="/ext/quotn/index.aspx">外銷報價單</a></li>
            <li><a title="外銷訂單" href="/ext/order/index.aspx">外銷訂單</a></li>
            <li><a title="外銷生產單" href="/ext/prod_order/index.aspx">外銷生產單</a></li>
            <li><a title="內銷出貨單" href="/dom/order/shipping/index.aspx">內銷出貨單</a></li>
            <li><a title="外銷出貨單" href="/ext/shipping_order/index.aspx">外銷出貨單</a></li>
            <li><a title="後端管理" href="javascript:void(0)">後端管理</a><ul><li><a title="權限" href="/sys_user/index.aspx">權限</a></li>
            <li><a title="內銷地區" href="/dom/dist/index.aspx">內銷地區</a></li>
            <li><a title="內銷貨運方式" href="/dom/freight_way/index.aspx">內銷貨運方式</a></li>
            <li><a title="外銷商品" href="/ext/goods/index.aspx">外銷商品</a></li>
            <li><a title="外銷貨運方式" href="/ext/freight_way/index.aspx">外銷貨運方式</a></li>
            <li><a title="外銷商品產品別維護" href="/NewWindow.aspx">外銷商品產品別維護</a></li>
            <li><a title="外銷商品明細維護" href="/Ext_Item_Details.aspx">外銷商品產品別明細維護</a></li>
            </ul></li>
            <li><a title="修改密碼" href="/sys_user/chg_pwd.aspx">修改密碼</a></li>
            <li><a title="報表查詢" href="/report/index.aspx">報表查詢</a></li>
        </ul>
    </div>

    <div class="Maintitle" style="padding: 0px 50px 0px 50px;  margin: 0px">
        <h1>外銷商品產品別明細維護</h1>
        <ul class="breadcrumbs">
            <li>現在位置：</li>
            <li><a href='http://mmstest.xseec.cn/index.aspx'>首頁</a></li><li>></li>
            <li class='last'><a href='/ext/quotn/index.aspx'>外銷商品產品別明細維護</a></li><li>></li>
            <li class='last'><a href='/ext/quotn/index.aspx'>新增商品產品別明細</a></li>
        </ul>
    </div>
    <div align="center"  style="padding: 0px 30px 0px 30px;  margin: 0px">
            <div style="padding: 0px 310px 0px 310px;  margin: 0px">
            <table style="width: 100%" class="ListTable">
                <tr>
                    <td style="width: 50%; text-align: right;">
                        <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="20px" Text="產品別："></asp:Label>
                    </td>
                    <td style="width: 50%; text-align: left;" >
                        <asp:DropDownList ID="ddl_item_type" runat="server" DataSourceID="SqlDataSource1" DataTextField="EXPORT_ITEM_TYPE" DataValueField="EXPORT_ITEM_TYPE" AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="ddl_item_type_SelectedIndexChanged">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="lbl_EXPORT_ITEM_TYPE" runat="server" Visible="False"></asp:Label>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:SEEC_MKTG_TSTConnectionString %>" SelectCommand="SELECT [EXPORT_ITEM_TYPE] FROM [EXT_ITEM_TYPE]"></asp:SqlDataSource>
                    </td>                
                </tr>
                
                <tr>
                    <td style="width: 50%; text-align: right;" >
                        <asp:Label ID="Label17" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="20px" Text="外銷型號："></asp:Label>
                    </td>
                    <td style="width: 50%; text-align: left;">
                        <asp:TextBox ID="txt_EXPORT_ITEM" runat="server" Width="250px"></asp:TextBox>
                    </td>                
                </tr>
                <tr>
                    <td style="width: 50%; text-align: right;">
                        <asp:Label ID="Label18" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="20px" Text="ERP料號："></asp:Label>
                    </td>
                    <td style="width: 50%; text-align: left;">
                        <asp:TextBox ID="txt_ERP_ITEM" runat="server" Width="250px" MaxLength="16"></asp:TextBox>
                        <asp:Label ID="lbl_ERP_ITEM" runat="server" Visible="False"></asp:Label>
                        
                    </td>                
                </tr>
                <tr>
                    <td  style="width: 50%; text-align: right;">
                    <asp:Label ID="Label19" runat="server" Font-Bold="True" Font-Names="標楷體" Font-Size="20px" Text="有效註記："></asp:Label>
                    </td>
                    <td  style="width: 50%; text-align: left;">
                        <asp:CheckBox ID="CB_ACTIVE_FLAG" runat="server" />
                    </td>               
                </tr>
            </table>
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            <asp:Label ID="lbl_CATEGORY1" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY2" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY3" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY4" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY5" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY6" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY7" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY8" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY9" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY10" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY11" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lbl_CATEGORY12" runat="server" Visible="False"></asp:Label>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
    </div>
    </div>
    <div class="btnBox" style="text-align: center">
        <div>
            <div>
                <asp:Button ID="btn_Add_Item" runat="server" Text="儲存" OnClientClick="ShowValue();" OnClick="btn_Add_Item_Click"/>&nbsp;&nbsp;&nbsp;
            </div>
            <div>&nbsp;&nbsp;&nbsp;&nbsp;</div>
<div>
    <asp:Button ID="btn_Cancel" runat="server" Text="回上一頁" OnClick="btn_Cancel_Click" />
</div>
</div>
    </div>
    </form>
</body>
</html>--%>
