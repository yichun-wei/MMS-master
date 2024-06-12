<%@ Control Language="C#" AutoEventWireup="true" CodeFile="head_top_fixed.ascx.cs" Inherits="include_client_common_head_top_fixed" %>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta http-equiv="pragma" content="no-cache" />
<meta http-equiv="Cache-Control" content="no-cache" />
<meta http-equiv="Expires" content="0" />

<%
    if (Seec.Marketing.SystemDefine.SystemPhase != Seec.Marketing.SystemPhase.Production)
    {
        Response.Write("<meta name=\"robots\" content=\"noindex,nofollow\" />");
    }
%>

<meta name="keywords" content="士林電機 XS 營銷管理系統" />
<meta name="description" content="士林電機 XS 營銷管理系統" />

<script type="text/javascript" src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/client/js/jquery-1.8.1.min.js"></script>
<script type="text/javascript" src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/common/js/json2.js"></script>
<link type="text/css" rel="stylesheet" href="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/common/css/common.css" />
<script type="text/javascript" src="<%=Seec.Marketing.SystemDefine.WebSiteRoot %>theme/common/js/common.js"></script>

