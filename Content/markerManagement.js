var wuYeMingCheng = "";
var markerPt;
var wuYeYongTu = "";
var wuYeBianHao = "";

var map = new BMap.Map("container");
var myCity = new BMap.LocalCity();
myCity.get(cityResolve);
//var point = new BMap.Point(120.621201, 31.299522);
//map.centerAndZoom(point, 12);
map.enableScrollWheelZoom();
map.addControl(new BMap.NavigationControl({ showZoomInfo: true }));
map.addControl(new BMap.OverviewMapControl());
map.addControl(new BMap.MapTypeControl({ mapTypes: [BMAP_NORMAL_MAP, BMAP_HYBRID_MAP] }));

var markers = [];
var complexMarkers = [];

var contextMenu = new BMap.ContextMenu();
var txtMenuItem = [
  {
      text: '放大',
      callback: function () { map.zoomIn(); }
  },
  {
      text: '缩小',
      callback: function () { map.zoomOut(); }
  },
  {
      text: '放置到最大级',
      callback: function () { map.setZoom(18); }
  },
  {
      text: '查看全国',
      callback: function () { map.setZoom(4); }
  },
  {
      text: '添加标注',
      callback: function (p) {
          addMarker(p);
//          var marker = new BMap.Marker(p);
//          map.addOverlay(marker);
      }
  }
 ];

for (var i = 0; i < txtMenuItem.length; i++) {
    contextMenu.addItem(new BMap.MenuItem(txtMenuItem[i].text, txtMenuItem[i].callback, 100));
    if (i == 1 || i == 3) {
        contextMenu.addSeparator();
    }
}
map.addContextMenu(contextMenu);

function cityResolve(result) {
    var cityName = result.name;
    map.centerAndZoom(cityName, 12);
}

function moveTo(bianHao, yongTu, pointX, pointY, mingCheng) {
    wuYeBianHao = bianHao;
    wuYeYongTu = yongTu;
    wuYeMingCheng = mingCheng;
    markerPt = null;
    map.clearOverlays();
    if (pointX == 0 || pointY == 0) {
//        var myCity = new BMap.LocalCity();
//        myCity.get(cityResolve);
        searchByKeyWords(wuYeMingCheng);
    } else {

        addMarker(new BMap.Point(pointX, pointY));
        map.centerAndZoom(markerPt, 17);
    }
}

function moveToLouDong(pointX, pointY) {
    map.centerAndZoom(new BMap.Point(pointX, pointY), 17);
}

function getLouDongZuoBiao(wuYeBianHao,url) {
    $.ajax({
        url: url,
        data: {
            wuYeBianHao: wuYeBianHao
        },
        dataType: 'json',
        success: function(result) {
            QuerySuccess(result);
        }
    });
}

function QuerySuccess(result) {
    RemoveComplexMarks();
    RemoveMarks();
    addMarkers(result);
    addComplexMarkers(result);
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
    if (overlays == null) return;
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

function addComplexMarkers(items) {
    //RemoveMarks();
    $.each(items, function(index, item) {
        var point = new BMap.Point(item.Px, item.Py);
        addComplexMarker(point, item.LouDong, item.LouDong);
        markers.push(item);
    });
}

function addComplexMarker(point, title, wuYebianHao) {
       var myCompOverlay = new ComplexCustomOverlay(point, title, title, wuYebianHao);
       map.addOverlay(myCompOverlay);
}

function addMarker(markerPoint) {
    markerPt = markerPoint;
    map.clearOverlays();
    var myMarker = new BMap.Marker(markerPoint);  // 创建标注
    map.addOverlay(myMarker);
}

function addMarkers(items) {
    $.each(items, function(index, item) {
        var point = new BMap.Point(item.Px, item.Py);
        addSimpleMarker(point, item.LouDong, item.LouDong);
        //addComplexMarker(point, item.WuYeMingCheng, item.WuYeBianHao);
        markers.push(item);
    });
}

function addSimpleMarker(point, title, wuYebianHao) {
    var myIcon = new BMap.Icon('url(../../../Content/images/go_home.png")', new BMap.Size(24, 24));
    var marker = new BMap.Marker(point, { icon: myIcon });
    marker.addEventListener("click", function (e) {
        //showDetail(wuYebianHao);
        addComplexMarker(point, title, wuYebianHao);
    });
    map.addOverlay(marker);
}

function searchByKeyWords(keyword) {
    var local = new BMap.LocalSearch(map, {
        renderOptions: { map: map }
    });
    local.search(keyword);
}

function canSaveMarker() {
    return markerPt != undefined;
}

function getPointX() {
    if (canSaveMarker()) {
        return markerPt.lng;
    }
    return 0;
}

function getPointY() {
    
    if (canSaveMarker()) {
        return markerPt.lat;
    }
    return 0;
}

