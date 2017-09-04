//var map = new BMap.Map("container");
var map = new BMap.Map("map");
var itemsPerPage = 7;
//getCurrentCity();
var cookie = getCookie("CurrentCity");
setCity(cookie);
//var myCity = new BMap.LocalCity();
//myCity.get(cityResolve);
//var point = new BMap.Point(120.621201, 31.299522);
//map.centerAndZoom(point, 12);
map.enableScrollWheelZoom();
map.addControl(new BMap.NavigationControl({ showZoomInfo: true }));
map.addControl(new BMap.OverviewMapControl());
map.addControl(new BMap.MapTypeControl({ mapTypes: [BMAP_NORMAL_MAP, BMAP_HYBRID_MAP] }));


var markers = [];
var complexMarkers = [];

//定义地图移动事件
//map.addEventListener("dragend", function () {
//    QueryLocalInSight(getSelectedCity(), getWuYeYongTu(), getKeyword(), getSw(), getNe(), getZoom());
//});

function getCookie(name) {

    var arg = name + "=";

    var alen = arg.length;

    var clen = document.cookie.length;

    var i = 0;

    while (i < clen) {

        var j = i + alen;
        if (document.cookie.substring(i, j) == arg) {
            return getCookieVal(j);
        }
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
    }

    return "";

}

function getCookieVal(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1) {
        endstr = document.cookie.length;
    }
    return decodeURIComponent(document.cookie.substring(offset, endstr));
}

function getCurrentCity() {
    $.ajax({

        url: '/SetSession/GetCookie',
        data: {
            key: "CurrentCity"
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            setCity(result["CurrentCity"]);
        }
    });
};

function cityResolve(result) {
    var cityName = result.name;
    setCity(cityName);
}
function setCity(cityName) {

    //parent.setCurrentCity(cityName);
    map.centerAndZoom(cityName, 14);

    $("#searchResult").html('');
    $("#searchResultCount").html("共有0条结果");
    $("#Pagination").empty();

}



function getSelectedCity() {
    //return $('#currentCity').val();
    return parent.getCurrentCity();
}
function getSw() {
    return map.getBounds().getSouthWest();
}

function getNe() {
    return map.getBounds().getNorthEast();
}

function getZoom() {
    return map.getZoom();
}

function getWuYeYongTu() {
    var selectedTab = parent.selectedTab();
    switch (selectedTab) {
        case "zhuzhai":
            return "ZhuZhai";
        case "shangye":
            return "ShangYe";
        case "bangong":
            return "BanGong";
        case "gongyeyongdi":
            return "GongYeYongDi";
        default:
            return "";
    }
}
function getBaseUrl() {
    var selectedTab = parent.selectedTab();
    switch (selectedTab) {
        case "zhuzhai":
            return "ZhiDiZhuZhaiDetail";
        case "shangye":
            return "ZhiDiShangYeDetail";
        case "bangong":
            return "ZhiDiBanGongDetail";
        case "gongyeyongdi":
            return "ZhiDiGongYeYongDiDetail";
        default:
            return "";
    }
};

function getPageListTemplate() {
    var selectedTab = parent.selectedTab();
    switch (selectedTab) {
        case "zhuzhai":
            return "searchResultTemplateZhuZhai";
        case "shangye":
            return "searchResultTemplateShangYe";
        case "bangong":
            return "searchResultTemplateShangYe";
        case "gongyeyongdi":
            return "searchResultTemplateTuDi";
        default:
            return "";
    }
}

function getKeyword() {
    return parent.getKeyord();
}
function getDiQu() {
    return parent.getDiQu();
}

function getWuYeLeiXing() {
    return parent.getWuYeLeiXin();
}

function getXiangMuZuoLuo() {
    return parent.getXiangMuZuoLuo();
}

function getDiKuaiBianHao() {
    return parent.getDiKuaiBianHao();
}

function getJinDeDanWei() {
    return parent.getJinDeDanWei();
}

function getDiKuaiYongTu() {
    return parent.getDiKuaiYongTu();
}

function getNianFen() {
    return parent.getNianFen();
}

function getQuYu() {
    return parent.getDiQu();
}

function getZuoLuo() {
    if (typeof (eval(parent.getZuoLuo)) == "function") {
        return parent.getZuoLuo();
    } else
    {return "";}
}

function addMarkers(items) {
    $.each(items, function (index, item) {
//        if (findMark(item).length == 0) {
//            var point = new BMap.Point(item.X, item.Y);
//            addSimpleMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
//            //addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
//            markers.push(item);

        //        }
        var point = new BMap.Point(item.X, item.Y);
        addSimpleMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
        //addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
        markers.push(item);
    });
    var point1 = new BMap.Point(items[0].X, items[0].Y);
    if (items.length == 1) {
        
        map.centerAndZoom(point1, 16);
    } else {
        map.centerAndZoom(point1, 12);
    }
}

function addComplexMarkers(items) {
    //RemoveMarks();
    $.each(items, function (index, item) {
//        if (findMark(item).length == 0) {
//            var point = new BMap.Point(item.X, item.Y);
//            //addSimpleMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
//            addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
//            markers.push(item);

        //        }
        var point = new BMap.Point(item.X, item.Y);
        addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
        markers.push(item);
    });
    
}

function findMark(item) {
    return $.grep(markers, function (e) { return e.WuYeBianHao == item.WuYeBianHao; });
}
function addComplexMarker(point, title, wuYebianHao) {
    var myCompOverlay = new ComplexCustomOverlay(point, title, title, wuYebianHao);
    map.addOverlay(myCompOverlay);
}
function addSimpleMarker(point, title, wuYebianHao) {
    var myIcon = new BMap.Icon('url(../../Content/images/go_home.png")', new BMap.Size(24, 24));
    var marker = new BMap.Marker(point, { icon: myIcon });
    marker.addEventListener("click", function (e) {
        //showDetail(wuYebianHao);
        addComplexMarker(point, title, wuYebianHao);
    });
    map.addOverlay(marker);
}
function moveToMarker(x, y) {
    //map.centerAndZoom(new BMap.Point(x, y), map.getZoom());
    map.centerAndZoom(new BMap.Point(x, y), 16);
}
function highlightMark(x, y) {
    var marker = findMarker(new BMap.Point(x, y));
    if (marker != undefined) {
        marker.hightlight();
    }
}
function RemoveMarks() {
    var overlays = map.getOverlays();
    for (var i = 1; i < overlays.length; i++) {
        switch (overlays[i].toString()) {
            case "[object Overlay]":
                var complexMarker = overlays[i];
                map.removeOverlay(complexMarker);
                complexMarker.dispose();
                break;
            default:
                break;
        }
    }
    markers = [];
    map.clearOverlays();
}

function RemoveComplexMarks() {
    var overlays = map.getOverlays();
    for (var i = 1; i < overlays.length; i++) {
        switch (overlays[i].toString()) {
            case "[object Overlay]":
                var complexMarker = overlays[i];
                map.removeOverlay(complexMarker);
                complexMarker.dispose();
                break;
            default:
                break;
        }
    }
}

function normalshowMark(x, y) {
    var marker = findMarker(new BMap.Point(x, y));
    if (marker != undefined) {
        marker.normalshow();
    }
}
function findMarker(point) {
    var overlays = map.getOverlays();
    for (var i = 1; i < overlays.length; i++) {
        switch (overlays[i].toString()) {
            case "[object Overlay]":
                var marker = overlays[i];
                var markerPoint = marker.markPoint();
                if (markerPoint.lng == point.lng && markerPoint.lat == point.lat) {
                    return marker;
                }
                break;
            default:
                break;
        }
    }
}

function updateSearchResultCount(totalcount) {
    $("#searchResultCount").html("共有" + totalcount + "条结果");
}
function setSearchingStatus() {
    $("#searchResultCount").html("正在查询，请稍后...");
}
function setSearchNoMatchStatus() {
    $("#searchResultCount").html("没有查询到相关信息！");
}
function RemoveSearchResult() {
    $("#searchResultCount").empty();
    $("#searchResult").html('');
}
function RemoveSearchResultHtml() {
    $("#searchResult").html('');
}


function updateSearchResult(data) {

    var template = getPageListTemplate();
    //var templateResult = $("#searchResultTemplate").html();
    var templateResult = $("#" + template).html();
    $("#searchResult").html(_.template(templateResult, { items: data }));
    //$("tr[id^='item-td-']").hover(function () { $(this).toggleClass('focus-over'); });
}

function updateScrollbar() {
    $("#searchleft").mCustomScrollbar("update");
}

function QueryTuDiLocal(city, nianFen, diKuaiBianHao, xiangMuZuoLuo, jinDeDanWei, diKuaiYongTu, pageIndex) {
    $.ajax({
        url: window.queryTuDiLocal,
        data: {
            city: city,
            nianFen: nianFen,
            diKuaiBianHao: diKuaiBianHao,
            xiangMuZuoLuo: xiangMuZuoLuo,
            jinDeDanWei: jinDeDanWei,
            diKuaiYongTu: diKuaiYongTu,
            page: pageIndex,
            pageSize: itemsPerPage
        },
        dataType: 'json',
        success: function (result) {
            QuerySuccess(pageIndex, result);
        }
    });
}

function QuerySuccess(pageIndex, result) {
    if (result.TotalCount <= 0) {
        
        RemoveMarks();
        RemovePagination();
        //RemoveSearchResult(); replace it with function - setSearchNoMatchStatus 
        setSearchNoMatchStatus();
        RemoveSearchResultHtml();
    }
    else  {
        if (pageIndex == 0 && isEmptyPagination()) {
            if (result.TotalCount > itemsPerPage) {
                initPagination(result.TotalCount);
            }
            addMarkers(result.TotalData);
        }
        updateSearchResultCount(result.TotalCount);
        updateSearchResult(result.Data);
        RemoveComplexMarks();
        addComplexMarkers(result.Data);
        updateScrollbar();
    }
};

function QueryLocal(city, yongtu, keywords, diqu, wuyeleixing, pageIndex) {
    setSearchingStatus();
    $.ajax({
        url: window.queryDataUrl,
        data: {
            city: city,
            wuYeYongTu: yongtu,
            keyword: keywords,
            diqu: diqu,
            wuyeleixing: wuyeleixing,
            page: pageIndex,
            pageSize: itemsPerPage
        },
        success: function (result) {
            QuerySuccess(pageIndex,result);
            //QueryLocalComplexMarker(city, yongtu, keywords, diqu, wuyeleixing, pageIndex, result);
        }

    });
}

function QueryLocal2(city, yongtu, keywords, diqu, wuyeleixing, pageIndex, zuoluo) {
    setSearchingStatus();
    $.ajax({
        url: window.queryDataUrl,
        data: {
            city: city,
            wuYeYongTu: yongtu,
            keyword: keywords,
            diqu: diqu,
            wuyeleixing: wuyeleixing,
            page: pageIndex,
            pageSize: itemsPerPage,
            zuoLuo: zuoluo
        },
        success: function (result) {
            QuerySuccess(pageIndex, result);
            //QueryLocalComplexMarker(city, yongtu, keywords, diqu, wuyeleixing, pageIndex, result);
        }

    });
}


function searchLocal2(city, yongtu, keywords, diqu, wuyeleixing, pageIndex, zuoluo) {



        var keyword = getKeyword();
        //        if (keyword.length == 0) {
        //            return;
        //        }
        RemoveMarks();
        RemovePagination();
        RemoveSearchResult();
        QueryLocal2(city, yongtu, keywords, diqu, wuyeleixing, pageIndex, zuoluo);


}

function QueryLocalInSight(city, yongtu, keywords, southWest, northEast, mapZoom) {
    if (southWest == undefined || northEast == undefined) return;

    $.ajax({
        url: window.queryDataInSightUrl,
        data: {
            city: city,
            wuYeYongTu: yongtu,
            keyword: keywords,
            minLat: southWest.lat,
            minLng: southWest.lng,
            maxLat: northEast.lat,
            maxLng: northEast.lng,
            zoom: mapZoom
        },
        success: function (result) {
            addMarkers(result.Data);
        }
    });
}
function initPagination(totRec) {
    // 创建分页
    $("#Pagination").pagination(totRec, {
        num_edge_entries: 1, //边缘页数
        num_display_entries: 2, //主体页数
        callback: pageselectCallback,
        items_per_page: itemsPerPage, //每页显示1项
        prev_text: "前一页",
        next_text: "后一页"
    });
};

function pageselectCallback(pageIndex, jq) {

    if (pageIndex == 0 && IsEmptySearchResult()) return false;
    var selectedTab = parent.selectedTab();
    if (selectedTab == 'gongyeyongdi') {
        QueryTuDiLocal(getSelectedCity(), getNianFen(), getDiKuaiBianHao(), getXiangMuZuoLuo(), getJinDeDanWei(), getDiKuaiYongTu(), pageIndex);
    } else {

        if ($("#hidRequestFrom").val() == 'zwjy') {

            QueryLocal2($("#hidCity").val(), 'ZhuZhai', $("#hidKeyword").val(), $("#hidQuYu").val(), '全部', pageIndex, $("#hidZuoLuo").val());
        }
        else {
            QueryLocal(getSelectedCity(), getWuYeYongTu(), getKeyword(), getDiQu(), getWuYeLeiXing(), pageIndex);
        }
    }

    return false;
}
function isEmptyPagination() {
    return $("#Pagination").html().length == 0 || $("#Pagination").html().trim() == '';
}
function RemovePagination() {
    $("#Pagination").empty();
}
function IsEmptySearchResult() {
    return $("#searchResult").html().length == 0 || $("#searchResult").html().trim() == '';
}

function searchByQuYu() {
    var selectedTab = parent.selectedTab();
    if (selectedTab == 'gongyeyongdi') {
        var nf = getNianFen();
        var xmzl = getXiangMuZuoLuo();
        var bgbh = getDiKuaiBianHao();
        var jddw = getJinDeDanWei();
        var dkyt = getDiKuaiYongTu();
        if (nf.length == 0 && xmzl.length == 0 && bgbh.length == 0 && jddw.length == 0 && dkyt.length == 0) {
            return;
        }
        RemoveMarks();
        RemovePagination();
        RemoveSearchResult();
        QueryTuDiLocal(getSelectedCity(), getNianFen(), getDiKuaiBianHao(), getXiangMuZuoLuo(), getJinDeDanWei(), getDiKuaiYongTu(), 0);
    }
    else {
        
        var keyword = getKeyword();
        if (keyword.length == 0) {
            return;
        }
        RemoveMarks();
        RemovePagination();
        RemoveSearchResult();
        QueryLocal(getSelectedCity(), getWuYeYongTu(), keyword, getDiQu(), getWuYeLeiXing(), 0);
    }
}


function searchLocal() {
    var selectedTab = parent.selectedTab();
    if (selectedTab == 'gongyeyongdi') {
        var nf = getNianFen();
        var xmzl = getXiangMuZuoLuo();
        var bgbh = getDiKuaiBianHao();
        var jddw = getJinDeDanWei();
        var dkyt = getDiKuaiYongTu();
        if (nf.length == 0 && xmzl.length == 0 && bgbh.length == 0 && jddw.length == 0 && dkyt.length == 0) {
            return;
        }
        RemoveMarks();
        RemovePagination();
        RemoveSearchResult();
        QueryTuDiLocal(getSelectedCity(), getNianFen(), getDiKuaiBianHao(), getXiangMuZuoLuo(), getJinDeDanWei(), getDiKuaiYongTu(), 0);
    }
    else {
        var keyword = getKeyword();
//        if (keyword.length == 0) {
//            return;
//        }
        RemoveMarks();
        RemovePagination();
        RemoveSearchResult();

        $("#hidRequestFrom").val("");
        //QueryLocal(getSelectedCity(), getWuYeYongTu(), keyword, getDiQu(), getWuYeLeiXing(), 0);
        if (selectedTab == 'zhuzhai') {
           
            QueryLocal2(getSelectedCity(), getWuYeYongTu(), keyword, getDiQu(), getWuYeLeiXing(), 0, getZuoLuo());
        }
        else {
            QueryLocal(getSelectedCity(), getWuYeYongTu(), keyword, getDiQu(), getWuYeLeiXing(), 0);
        }
    }

}

function showDetail(wuYeBianHao) {
    var href = getBaseUrl() + "?wuYeBianHao=" + wuYeBianHao;
    //window.open(href, '_newtab');
    var open_link = window.open('', '_newtab');
    open_link.location = href;
}
