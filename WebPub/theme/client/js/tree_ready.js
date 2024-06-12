/*樹狀選單*/
$(document).ready(function(){
	$("#tree").treeview({
			persist: "location",
			collapsed: true,
			unique: true,
			collapsed: true,
			animated: "medium"

		});
	// first example
	$("ul[name=navigation]").each(function(){
		if ($(this).css('display')=="block") return ;
		
		$(this).treeview({
			persist: "location",
			collapsed: true,
			unique: true,
			collapsed: true,
			animated: "medium"

		});
		
		$(this).css('display','block');
	});
});