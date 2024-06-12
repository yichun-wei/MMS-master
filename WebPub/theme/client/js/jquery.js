$(function(){	
	/**
	 *	自動把字串多出來的字元變成... 用法:class="truncate" length="10"
	 */
	$('.truncate').each(function(){
		var _len=$(this).attr("length");
		var _txt=$(this).text();
		var _txt2=_txt.substring(0,_len);
		if (_txt!=_txt2)
			$(this).text(_txt2+'...');
	});
	
});

// gototop 效果
$(function() {
	$('div.btn_top > a').click(function() {
		// 讓捲軸用動畫的方式移動到 0 的位置
		// 感謝網友 sam 修正 Opera 問題
		var $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $('html') : $('body')) : $('html,body');
		$body.animate({
			scrollTop: 0
		}, 600);

		return false;
	});
});

// gototop 效果
$(function() {
	$('div#fox_top > a').click(function() {
		// 讓捲軸用動畫的方式移動到 0 的位置
		// 感謝網友 sam 修正 Opera 問題
		var $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $('html') : $('body')) : $('html,body');
		$body.animate({
			scrollTop: 0
		}, 600);

		return false;
	});
});