define(['zd', 'async!BMap'], function (app) {
  // 百度地图API
  var map = new BMap.Map("map");
  var infoWindow;

  //设置地图配置
  function mapConfigure() {
    map.enableScrollWheelZoom(true); //开启百度地图鼠标滚轮缩放功能的

    // 定义一个控件类,即function
    function ZoomControl() {
      // 默认停靠位置和偏移量
      this.defaultAnchor = BMAP_ANCHOR_TOP_LEFT;
      this.defaultOffset = new BMap.Size(10, 10);
    }

    // 通过JavaScript的prototype属性继承于BMap.Control
    ZoomControl.prototype = new BMap.Control();

    // 自定义控件必须实现自己的initialize方法,并且将控件的DOM元素返回
    ZoomControl.prototype.initialize = function (map) {
      var ul = document.createElement('ul');
      ul.setAttribute('class', 'list-inline house-captioned');
      var datas = [
        { 'text': '绿色：新售；', 'calss': 'green' },
        { 'text': '蓝色：存量；', 'calss': 'blue' },
        { 'text': '橙色：推介项目', 'calss': 'orange' }
      ];
      for (var i = 0; i < datas.length; i++) {
        var li = document.createElement('li');
        li.setAttribute('class', datas[i].calss);
        li.innerHTML = datas[i].text;
        ul.appendChild(li);
      }
      map.getContainer().appendChild(ul);
      return ul;
    }
    var myZoomCtrl = new ZoomControl();// 创建控件
    map.addControl(myZoomCtrl);// 添加到地图当中
  };

  //出售化地图为当前城市
  function setCity() {
    var accountInfo = app.cache.get('accountInfo');
    var cityName = accountInfo.City;
    map.centerAndZoom(cityName, 14);
  }

  //初始化地图
  function initialize() {
    mapConfigure();
    setCity();
  }

  //数据查询
  function search(requestUrl, pm, callBack, pointDetailTemplate) {
    app.request.postAction(requestUrl, pm, function (data) {
      if (data.Result === 'success') {
        var datas = data.Response;
        removeMarks();
        callBack(datas);
        if (datas.length > 0) {
          console.log(datas.length)
          var pageCount = datas.length / 10;
          alert(pageCount)
          for (var i = 0; i < datas.length; i++) {
            addMarkers(datas[i], pointDetailTemplate(datas[i]));
          }
          var mapSize;
          if (datas.length === 1) {
            mapSize = 16;
          } else {
            mapSize = 12;
          }
          var point = new BMap.Point(datas[0].Longitude, datas[0].Latitude);
          map.centerAndZoom(point, mapSize);
        }
      } else {
        alert(data.Response);
      }
    });
  };

  function addSimpleMarker(point, iconType, sContent) {
    var mapMark;
    if (iconType === 0) {
      mapMark = '/Resource/assets/images/icon_mark_stock.png';
    } else if (iconType === 1) {
      mapMark = '/Resource/assets/images/icon_mark_newSale.png';
    } else {
      mapMark = '/Resource/assets/images/icon_mark_recommend.png';
    }
    var myIcon = new BMap.Icon(mapMark, new BMap.Size(24, 24));
    var marker = new BMap.Marker(point, { icon: myIcon });
    infoWindow = new BMap.InfoWindow(sContent);  // 创建信息窗口对象
    marker.addEventListener("click", function () {
      this.openInfoWindow(infoWindow);
      //图片加载完毕重绘infowindow
      //document.getElementById('imgDemo').onload = function () {
      //    infoWindow.redraw(); //防止在网速较慢，图片未加载时，生成的信息框高度比图片的总高度小，导致图片部分被隐藏
      //}
    });
    map.addOverlay(marker);
  }

  //添加自定义覆盖物
  function addMarkers(item, sContent) {
    var point = new BMap.Point(item.Longitude, item.Latitude);
    addSimpleMarker(point, item.IconTypeEnum, sContent);
  }

  //删除自定义覆盖物
  function removeMarks() {
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
  };

  //数据列表显示、隐藏
  function mapListToggle() {
    $('#listVisible').click(function () {
      $(this).parent().find('.list').toggle();
      $(this).find('a').toggleClass('mapList-hide');
      console.log($(this).offset().left)
      if ($(this).offset().left === 396) {
        $(this).animate({ 'left': '0' }, 200);
      } else {
        $(this).animate({ 'left': '396px' }, 200);
      }
    });
  }

  function toPosition(x, y) {
    console.log(x, y)
    var point = new BMap.Point(x, y);
    map.centerAndZoom(map.centerAndZoom(point, 16));
    map.openInfoWindow(infoWindow, point);
  };

  mapListToggle();
  initialize();

  return {
    toPosition: toPosition,
    search: search
  };
});