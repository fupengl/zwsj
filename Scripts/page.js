$(document).ready(function(){
	$(".page span").click(function(){
		$(".page span").each(function(){
			$(this).removeClass("now");	
			
		});
		$(this).addClass("now");
		
	});
	$(".page img").click(function(){
		var dir=$(this).index(".page img");//方向(左右)
		var length=$(".page span").length;
		$(".page span").each(function(){
			if($(this).hasClass("now"))
			{
				var i=$(this).index(".page span");
				if(dir==0)
				{
					if(i!=0)
					{
						$(this).removeClass("now");
						i--;
						$(".page span").eq(i).addClass("now");
					}
				}
				if(dir==1)
				{
					if(i!=length-1) 
					{
						$(this).removeClass("now");	
						i++;
						$(".page span").eq(i).addClass("now");
					}
				}
				return false;
			}			  
		});							  
	});
						   
});
