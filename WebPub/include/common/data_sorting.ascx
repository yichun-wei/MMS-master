<%@ Control Language="C#" AutoEventWireup="true" CodeFile="data_sorting.ascx.cs" Inherits="include_common_data_sorting" %>

<input type="hidden" id="hidSortingBuff" name="hidSortingBuff" />

<script type="text/javascript">
    //<![CDATA[

    function ChangingSort() {

        var ex = /^\d+$/;
        var jDataCount = sortData.length;
        for (var i = 0; i < jDataCount; i++) {
            sortData[i]["NewVal"] = $("#txtSort_" + sortData[i]["SId"]).val();

            if (!ex.test(sortData[i]["NewVal"])) {
                alert("請輸入整數的排序值");
                return false;
            }
        }

        var hidden = $("#hidSortingBuff");
        hidden.val(JSON2.stringify(sortData));

        document.form[0].submit();
    }

    //]]>
</script>
