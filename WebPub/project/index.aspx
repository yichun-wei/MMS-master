<%@ Page Title="" Language="C#" MasterPageFile="~/master_page/client.master" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="project_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phHtmlHead" Runat="Server">
    <script type="text/javascript">
        $(function () {
            $("input.dev-date-all").datepicker({

            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phHtmlBody" Runat="Server">
    <div id="center">
        <div class="Maintitle">
            <h1>CRM專案查詢</h1>
            <Client:Breadcrumbs ID="ucBreadcrumbs" runat="server" />
        </div>
        <div class="searchBox">
            <table class="form">
                <tr>
                    <th width="140">地區：</th>
                    <td width="370">
                        <asp:DropDownList ID="lstDomDistList" runat="server">
                            <asp:ListItem Text="全部" Value="" />
                        </asp:DropDownList>
                        <asp:HiddenField ID="hidIsAllDomDist" Value="Y" runat="server" />
                    </td>
                    <th>專案編號：</th>
                    <td>
                        <asp:TextBox ID="txtKeyword" runat="server" /><br />  
                    </td>
                </tr>                
                <tr>
                    <th width="140">狀態：</th>
                    <td width="370">
                        <asp:DropDownList ID="lstProjQuoteCancelStatuList" runat="server"></asp:DropDownList>
                    </td>
                    <%--2017-05-25 CRM專案取消 客戶名稱--%>
                    <th>客戶名稱：</th>
                    <td>
                        <asp:TextBox ID="txtDEALER_ERP_NAME" runat="server" /><br />                       
                    </td>
                </tr>                
                <tr>
                    <th width="140">報價單日期：</th>
                    <td width="370">
                        <asp:TextBox ID="txtBeginEdd" CssClass="checkin dev-date-all" runat="server" />
                        ~
                        <asp:TextBox ID="txtEndEdd" CssClass="checkin dev-date-all" runat="server" />
                    </td>
                    <th></th>
                    <td>
                                               
                    </td>
                </tr>
            </table>            
            <div class="btnBox searchBtn">                
                <div>                                       
                    <div>
                        <asp:Button ID="btnSearch" Text="搜尋專案" runat="server" OnClick="Page_DoSearch"/>
                        <asp:HiddenField ID="hidSearchConds" runat="server" />
                    </div>
                </div>
            </div>
            <div class="list_box">
            <div class="btnBox">
                <div>
                    <div><asp:LinkButton ID="btnUpdateCRMQuote" Text="專案報價更新" runat="server"  OnClick="btnUpdateCRMQuote_Click"/></div> 
                    <div>                        
                        <asp:Button ID="btnCancel_PROJ_QUOTE" runat="server" OnClick="btnCancel_PROJ_QUOTE_Click" OnClientClick="getSeledProjQuotes()" Text="取消報價單"/>                            
                        <asp:HiddenField ID="hidSeledProjQuotes" runat="server" />
                    </div>
                </div>
                <div>
                    <div>
                        <div>
                            <asp:Button ID="btnValid_PROJ_QUOTE" runat="server" OnClick="btnValid_PROJ_QUOTE_Click" OnClientClick="getSeledValidProjQuotes()" Text="有效報價單"/>    
                        </div>             
                    </div>
                </div>
            </div>
            <asp:PlaceHolder ID="phPageList" runat="server" />
            <div class="btn_top"><a href="javascript:void(0)">top</a></div>
            <asp:PlaceHolder ID="phPaging" Visible="false" runat="server">
                <div id="dev_pages">
                    <div>
                        <Paging:CustFirstPaging ID="firstPaging" DisplayText="第一頁" runat="server" />
                        <Paging:CustPrevPaging ID="prevPaging" DisplayText="上一頁" runat="server" />
                        <Paging:CustNumberPaging ID="numberPaging" QuotationStyle="Empty" NextPagesDisplayText="下 {N} 頁" PrevPagesDisplayText="上 {N} 頁" HiddenPrevNextPagesOnInvalid="true" DisplayNumberCount="10" RepeatLayout="Flow" runat="server" />
                        <Paging:CustNextPaging ID="nextPaging" DisplayText="下一頁" runat="server" />
                        <Paging:CustLastPaging ID="lastPaging" DisplayText="最末頁" runat="server" />
                    </div>
                    <span>共
                        <asp:Literal ID="litTotalPageNumber" Text="0" runat="server" />
                        頁/
                        <asp:Literal ID="litSearchResulted" Text="0" runat="server" />
                        筆,&nbsp;每頁顯示
                        <asp:DropDownList ID="listCountOfPage" AutoPostBack="true" runat="server">
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="30" Value="30" />
                            <asp:ListItem Text="40" Value="40" />
                            <asp:ListItem Text="50" Value="50" />
                        </asp:DropDownList>
                        筆,
                        移至第&nbsp;<Paging:TextBoxPaging ID="textboxPaging" DisplayShape="HyperLink" DisplayText="" runat="server" />
                        &nbsp;頁
                    </span>
                </div>
                <Paging:ObserverPaging ID="observerPaging" RequestMethod="post" runat="server" />
            </asp:PlaceHolder>            
            </div>
        </div>
    </div>
    <div id="fox_top"><a href="javascript:void(0)">top</a></div>

    <script type="text/javascript">
        //2017-05-24 CRM專案取消全選
        $(function () {
            $(".dev-sel-all").change(function () {
                var checked = $(this).is(":checked");
                $(".dev-sel").each(function () {
                    $(this).prop("checked", checked);
                });
            });
        });

        var Count = 0, sum = 0;
        function getSeledProjQuotes() {
            var SeledProjQuotes = new Array();    
            $(".dev-sel").each(function () {
                var checked = $(this).is(":checked");
                if (checked) {                    
                    SeledProjQuotes.push($(this).val());
                    sum++;
                }
            });
            
            if (Count == 0) {
                if (sum > 0)
                {
                    confirm("確定取消這幾筆報價單?");
                }
                else { alert("請選擇欲取消之報價單!!!");}
            }
            
            Count++;
            $("#<%=this.hidSeledProjQuotes.ClientID%>").val(SeledProjQuotes);
            $("#<%=this.btnCancel_PROJ_QUOTE.ClientID%>").trigger("click");
        }

        function getSeledValidProjQuotes()
        {            
            var SeledProjQuotes = new Array();
            $(".dev-sel").each(function ()
            {
                var checked = $(this).is(":checked");
                if (checked) {
                    SeledProjQuotes.push($(this).val());
                    sum++;
                }
            });
            if (Count == 0) {
                
                if (sum > 0) {
                    confirm("確定回復這幾筆報價單?");
                }
                else { alert("請選擇欲回復之報價單!!!"); }
            }
            Count++;
            $("#<%=this.hidSeledProjQuotes.ClientID%>").val(SeledProjQuotes);
            $("#<%=this.btnValid_PROJ_QUOTE.ClientID%>").trigger("click");            
        }
    </script>

</asp:Content>

