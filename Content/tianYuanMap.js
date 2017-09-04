//var map = new BMap.Map("container");
var map = new BMap.Map("map");
var itemsPerPage = 10;

var myCity = new BMap.LocalCity();
myCity.get(cityResolve);
//var point = new BMap.Point(120.621201, 31.299522);
//map.centerAndZoom(point, 12);
map.enableScrollWheelZoom();
map.addControl(new BMap.NavigationControl({ showZoomInfo: true }));
map.addControl(new BMap.OverviewMapControl());
map.addControl(new BMap.MapTypeControl({ mapTypes: [BMAP_NORMAL_MAP, BMAP_HYBRID_MAP] }));

var markers = [];

//定义地图移动事件
map.addEventListener("dragend", function () {
    QueryLocalInSight(getSelectedCity(), getWuYeYongTu(), getKeyword(), getSw(), getNe(), getZoom());
});
function cityResolve(result) {
    var cityName = result.name;
    setCity(cityName);
}
function setCity(cityName) {
    $('#currentCity').val(cityName);
    map.centerAndZoom(cityName, 12);
}
function getSelectedCity() {
    return $('#currentCity').val();
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
    return "ZhuZhai";
}

function getKeyword() {
    return $('#searchKeyword').val();
}

function addMarkers(items) {
    $.each(items, function (index, item) {
        if (findMark(item).length == 0) {
            var point = new BMap.Point(item.X, item.Y);
            addSimpleMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
            addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
            markers.push(item);
        }
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
        showDetail(wuYebianHao);
    });
    map.addOverlay(marker);
}
function moveToMarker(x, y) {
    map.centerAndZoom(new BMap.Point(x, y), map.getZoom());
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
            //            case "[object Marker]": 
            //                var simpleMarker = overlays[i]; 
            //                map.removeOverlay(simpleMarker); 
            //                simpleMarker.dispose(); 
            //                break; 
            default:
                break;
        }
    }
    markers = [];
    map.clearOverlays();
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
    var templateCount = $("#searchResultCountTemplate").html();
    $("#searchResultCount").html(_.template(templateCount, { totalCount: totalcount }));
}
function updateSearchResult(data) {
    var templateResult = $("#searchResultTemplate").html();
    $("#searchResult").html(_.template(templateResult, { items: data }));
    $("tr[id^='item-td-']").hover(function () { $(this).toggleClass('focus-over'); });
}
//function updateScrollbar() {
//    $(".west").mCustomScrollbar("update");
//}
function QueryLocal(city, yongtu, keywords, pageIndex) {
    $.ajax({
        url: window.queryDataUrl,
        data: {
            city: city,
            wuYeYongTu: yongtu,
            keyword: keywords,
            page: pageIndex,
            pageSize: itemsPerPage
        },
        success: function (result) {
            if (pageIndex == 0 && result.TotalCount > itemsPerPage && isEmptyPagination()) {
                initPagination(result.TotalCount);
            }
            updateSearchResultCount(result.TotalCount);
            updateSearchResult(result.Data);
            addMarkers(result.Data);
            //updateScrollbar();
        }
    });
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
    QueryLocal(getSelectedCity(), getWuYeYongTu(), getKeyword(), pageIndex);
    return false;
}
function isEmptyPagination() {
    return $("#Pagination").html().trim() == '';
}
function RemovePagination() {
    $("#Pagination").empty();
}
function IsEmptySearchResult() {
    return $("#searchResult").html().trim() == '';
}
function RemoveSearchResult() {
    $("#searchResultCount").empty();
}
function searchLocal() {
    var keyword = getKeyword();
    if (keyword.length == 0) {
        return;
    }
    RemoveMarks();
    RemovePagination();
    RemoveSearchResult();
    QueryLocal(getSelectedCity(), getWuYeYongTu(), keyword, 0);
}
function showDetail(wuYeBianHao) {
    console.log(wuYeBianHao);
}
