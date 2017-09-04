// JavaScript Document
$(document).ready(function () {
    $(".top_right-three li").click(function () {
        $(".top_right-three li img").each(function () {
            $(this).attr("src", "img/sy/tip1.png");
        });
        $(this).children("img").attr("src", "img/sy/tip2.png");

    });
    $(".top-menu li").click(function (e) {
        $(".top-menu li img").each(function () {
            var imgPath = $(this).attr("src");
            var i = imgPath.lastIndexOf(".");

            if (imgPath.substring(i - 1, i) == "2") {
                //$(this).after("<em></em>")
                var newPath = imgPath.substring(0, i - 1) + "1" + imgPath.substring(i);
                $(this).attr("src", newPath);
                $(this).parent("li").removeClass("tip");
                //return false;
            }            
        });

        var imgPath = $(this).children("img").attr("src");

        var i = imgPath.lastIndexOf(".");

        if (imgPath.substring(i - 1, i) == "1") {
            //$(this).children("em").remove();
            var newPath = imgPath.substring(0, i - 1) + "2" + imgPath.substring(i);
            $(this).addClass("tip");
            $(this).children("img").attr("src", newPath);

        }
    });
});