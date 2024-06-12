
//將網頁導向指定的網址。
//url : 指定的網址。
function Redirect(url) {
    document.location = url;
}

//Pop-Up 視窗。
//url : 指定的網址。
//name : 指定的視窗名稱。
//width : 指定的寛。
//height : 指定的高。
function WindowOpen(url, name, width, height) {
    if (width == undefined) { width = 800; }
    if (height == undefined) { height = 500; }
    window.open(url, name, 'height=' + height + ',width=' + width + ',status=yes,scrollbars=yes,resizable=yes,toolbar=no,menubar=no,location=no', true);
}

var imageDialog;
function ShowImageDialog(path) {
    imageDialog = window.showModelessDialog(path, window, "dialogwidth=500px;dialogheight=500px;resizable=yes");
}

function MM_showHideLayers() {
    var i, p, v, obj, args = MM_showHideLayers.arguments;
    for (i = 0; i < (args.length - 2); i += 3) if ((obj = MM_findObj(args[i])) != null) {
        v = args[i + 2];
        if (obj.style) { obj = obj.style; v = (v == 'show') ? 'visible' : (v == 'hide') ? 'hidden' : v; }
        obj.visibility = v;
    }
}

function EnterClick(e, clientId) {
    var key = window.event ? e.keyCode : e.which;
    if (key == 13) {
        DoClick(clientId);
        return false;
    }
}

function DoClick(clientId) {
    var o = document.getElementById(clientId);
    if (document.all && typeof (document.all) == "object") {
        o.click();
    }
    else {
        var e = document.createEvent('MouseEvents');
        e.initEvent('click', true, true);
        o.dispatchEvent(e);
    }
}

//ASP.NET TreeView CheckBox 全選。
function TreeView_OnTreeNodeChecked() {
    var obj = window.event.srcElement;
    var treeNodeFound = false;
    var checkedState;
    if (obj.tagName == "INPUT" && obj.type == "checkbox") {
        var treeNode = obj;
        checkedState = treeNode.checked;
        do {
            obj = obj.parentElement;
        } while (obj.tagName != "TABLE")

        var parentTreeLevel = obj.rows[0].cells.length;
        var parentTreeNode = obj.rows[0].cells[0];
        var tables = obj.parentElement.getElementsByTagName("TABLE");
        var numTables = tables.length;
        if (numTables >= 1) {
            for (i = 0; i < numTables; i++) {
                if (tables[i] == obj) {
                    treeNodeFound = true;
                    i++;
                    if (i == numTables) {
                        return;
                    }
                }

                if (treeNodeFound == true) {
                    var childTreeLevel = tables[i].rows[0].cells.length;
                    if (childTreeLevel > parentTreeLevel) {
                        var cell = tables[i].rows[0].cells[childTreeLevel - 1];
                        var inputs = cell.getElementsByTagName("INPUT");
                        inputs[0].checked = checkedState;
                    }
                    else {
                        return;
                    }
                }
            }
        }
    }
}

//同一個表單下的 CheckBox 全選。
function SelectAllCheckBox(chk) {
    var frm = document.forms[0];
    var objLen = frm.length;
    for (var i = 0; i < objLen; i++) {
        if (frm.elements[i].type == "checkbox") {
            frm.elements[i].checked = chk.checked;
        }
    }
}

//同一個表單下指定名稱的 CheckBox 全選。
function SelectAllCheckBox(chk, name) {
    var frm = document.forms[0];
    var objLen = frm.length;
    for (var i = 0; i < objLen; i++) {
        if (frm.elements[i].type == "checkbox" && frm.elements[i].name == name) {
            frm.elements[i].checked = chk.checked;
        }
    }
}

function ShowError(errMsg, focusField) {
    if (errMsg != null && errMsg != "") {
        alert(errMsg);
    }
    if (focusField != null) {
        focusField.focus();
    }
    return false;
}

function Trim(txt) {

    if (txt == "") { return txt; }

    var txtLen = txt.length;

    for (var i = 1; i <= txtLen; i++) {
        if (txt.substring(0, 1) == " " || txt.substring(0, 1) == "　") {
            txt = txt.substring(1, txt.length);
        }
        else {
            break;
        }
    }

    for (var i = 1; i <= txtLen; i++) {
        if (txt.substring(txt.length - 1, txt.length) == " " || txt.substring(txt.length - 1, txt.length) == "　") {
            txt = txt.substring(0, txt.length - 1);
        }
        else {
            break;
        }
    }

    return txt;
}

function CopyToClipboard(txt, alertPrompt) {
    if (window.clipboardData) {
        window.clipboardData.clearData();
        window.clipboardData.setData("Text", txt);
    } else if (navigator.userAgent.indexOf("Opera") != -1) {
        window.location = txt;
    } else if (window.netscape) {
        try {
            netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
        } catch (e) {
            alert("被瀏覽器拒絕！\n請在瀏覽器地址欄輸入'about:config'並確認\n然後將 'signed.applets.codebase_principal_support' 設置為 'true'");
        }
        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
        if (!clip) return; var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
        if (!trans) return;
        trans.addDataFlavor('text/unicode');
        var str = new Object(); var len = new Object();
        var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
        var copytext = txt; str.data = copytext;
        trans.setTransferData("text/unicode", str, copytext.length * 2);
        var clipid = Components.interfaces.nsIClipboard;
        if (!clip) return false;
        clip.setData(trans, null, clipid.kGlobalClipboard);
    } alert(alertPrompt);
}

//僅允許數字輸入
function ValidateNumber(e, val) {
    if (!/^\d+$/.test(val)) {
        var newVal = /^\d+/.exec(e.value);
        if (newVal != null) {
            e.value = newVal;
        }
        else {
            e.value = "";
        }
    }
    return false;
}

//僅允許數字和小數輸入
function ValidateFloat(e, val) {
    if (!/^\d+[.]?\d*$/.test(val)) {
        var newVal = /^\d+[.]?\d*/.exec(e.value);
        if (newVal != null) {
            e.value = newVal;
        }
        else {
            e.value = "";
        }
    }
    return false;
}

//僅允許數字和小數 (第一位) 輸入
function ValidateFloat1(e, val) {
    if (!/^\d+[.]?[0-9]?$/.test(val)) {
        var newVal = /\d+[.]?[0-9]?/.exec(e.value);
        if (newVal != null) {
            e.value = newVal;
        }
        else {
            e.value = "";
        }
    }

    return false;
}

//僅允許數字和小數 (第二位) 輸入
function ValidateFloat1(e, val) {
    if (!/^\d+[.]?[0-9]?[0-9]?$/.test(val)) {
        var newVal = /\d+[.]?[0-9]?[0-9]?/.exec(e.value);
        if (newVal != null) {
            e.value = newVal;
        }
        else {
            e.value = "";
        }
    }

    return false;
}

//僅允許數字和小數 (第四位) 輸入
function ValidateFloat4(e, val) {
    if (!/^\d+[.]?[0-9]?[0-9]?[0-9]?[0-9]?$/.test(val)) {
        var newVal = /\d+[.]?[0-9]?[0-9]?[0-9]?[0-9]?/.exec(e.value);
        if (newVal != null) {
            e.value = newVal;
        }
        else {
            e.value = "";
        }
    }

    return false;
}

/*============Ajax:AutoCompleteExtender==================*/
function ACClientPopulated(source, eventArgs) {
    if (source._currentPrefix != null) {
        var list = source.get_completionList();
        var search = source._currentPrefix.toLowerCase();
        for (var i = 0; i < list.childNodes.length; i++) {
            var text = list.childNodes[i].innerHTML;
            var index = text.toLowerCase().indexOf(search);
            if (index != -1) {
                var value = text.substring(0, index);
                value += '<span class="auto_complete_list_item_hilite_text">';
                value += text.substr(index, search.length);
                value += '</span>';
                value += text.substring(index + search.length);
                list.childNodes[i].innerHTML = value;
            }
        }
    }
}

// 讓捲軸用動畫的方式移動到 0 的位置
function GogoTop(scrollTop) {
    var $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $('html') : $('body')) : $('html,body');
    $body.animate({
        scrollTop: scrollTop ? scrollTop : 0
    }, 600);

    return false;
}

/*============浮點數四則運算==================*/
//除法函數，用來得到精確的除法結果
//說明：javascript的除法結果會有誤差，在兩個浮點數相除的時候會比較明顯。這個函數返回較為 精確的除法結果。
//調用：accDiv(arg1,arg2)
//返回值：arg1除以arg2的精確結果
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
    try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""))
        r2 = Number(arg2.toString().replace(".", ""))
        return (r1 / r2) * pow(10, t2 - t1);
    }
}

//給Number類型增加一個div方法，調用起來更加方便。
Number.prototype.div = function (arg) {
    return accDiv(this, arg);
}

//乘法函數，用來得到精確的乘法結果
//說明：javascript的乘法結果會有誤差，在兩個浮點數相乘的時候會比較明顯。這個函數返回較為精確的乘法結果。
//調用：accMul(arg1,arg2)
//返回值：arg1乘以 arg2的精確結果
function accMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try { m += s1.split(".")[1].length } catch (e) { }
    try { m += s2.split(".")[1].length } catch (e) { }
    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m)
}

// 給Number類型增加一個mul方法，調用起來更加方便。
Number.prototype.mul = function (arg) {
    return accMul(arg, this);
}

//加法函數，用來得到精確的加法結果
//說明：javascript的加法結果會有誤差，在兩個浮點數相加的時候會比較明顯。這個函數返回較為精確的加法結果。
//調用：accAdd(arg1,arg2)
// 返回值：arg1加上arg2的精確結果
function accAdd(arg1, arg2) {
    var r1, r2, m;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2))
    return (arg1 * m + arg2 * m) / m
}

//給Number類型增加一個add方法，調用起來更加方便。
Number.prototype.add = function (arg) {
    return accAdd(arg, this);
}

//減法函數，用來得到精確的減法結果
//說明：javascript的減法結果會有誤差，在兩個浮點數相減的時候會比較明顯。這個函數返回較為精確的減法結果。
//調用：accSub(arg1,arg2)
// 返回值：arg1減掉arg2的精確結果
function accSub(arg1, arg2) {
    var r1, r2, m;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2));
    return (arg1 * m - arg2 * m) / m;
}

//給Number類型增加一個sub方法，調用起來更加方便。
Number.prototype.sub = function (arg) {
    return accSub(this, arg);
}

//解決浮點數的計算誤差
//說明：若直接計算會是「0.1 + 0.2 = 0.30000000000000004」
//調用：Math.formatFloat(01 + 0.2, 1) 結果為「0.3」
Math.formatFloat = function (f, digit) {
    var m = Math.pow(10, digit);
    return parseInt(f * m, 10) / m;
}

//僅允許單行輸入
function ValidateSingleLine(e) {
    var key = window.event ? e.keyCode : e.which;
    if (key == 13) {
        //e.value = "";
        e.returnValue = false;
        //alert('tt');
    }

    return false;
}

//取得 get 參數值. 例「$.getUrlVar('sid');」.
$(function () {
    $.extend({
        getUrlVars: function () {
            var vars = [],
                hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        },
        getUrlVar: function (name) {
            return $.getUrlVars()[name];
        }
    });
});