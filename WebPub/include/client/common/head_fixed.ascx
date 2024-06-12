<%@ Control Language="C#" AutoEventWireup="true" CodeFile="head_fixed.ascx.cs" Inherits="include_client_common_head_fixed" %>
<link href="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/client/css/style.css" rel="stylesheet" type="text/css" />
<link href="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/client/css/dev_cust.css" rel="stylesheet" type="text/css" />
<script src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/client/js/jquery.datepicker.min.js" type="text/javascript"></script>
<script src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/client/js/jquery.js" type="text/javascript"></script>
<script src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/common/js/accounting.min.js" type="text/javascript"></script>

<script type="text/javascript">
    $(function () {
        //datepicker中文檔
        $.datepicker.setDefaults($.datepicker.regional['zh-TW'] = {
            dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
            monthNames: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
            monthNamesShort: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
            prevText: "上個月",
            nextText: "下個月",
            weekHeader: "週",
            showMonthAfterYear: true,
            yearSuffix: "年",
            dateFormat: 'yy-mm-dd'
        });
    })
</script>
