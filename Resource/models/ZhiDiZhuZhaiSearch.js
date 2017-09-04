define([
  'ko',
  'zd',
  './map/zhiDiMapListTemplate',
  './map/zhiDiMapPointInfoTemplate',
  './map/zhiDiMap',
  './zdPagination'
], function (ko, app, listTemplate, pointInfoTemplate, action, page) {

  function setContent(item) {
    item.toDetail = 'ZhiDiZhuZhaiDetail?id=' + item.WuYeId;
    item.PhotoUrl = item.PhotoUrl ? item.PhotoUrl : '/Resource/assets/images/img_default_houseMapList.png';
    /*if (item.JiaGeQuJian.indexOf('\n') !== -1) {
      item.JiaGeQuJian = item.JiaGeQuJian.replace('\n/g', '<br/>');
    }*/
    item.JiaGeQuJian = item.JiaGeQuJian && item.JiaGeQuJian.replace('\n/g', '<br/>');
    return item;
  }

  function PostViewModel(item) {
    this.areaId = item.areaId();
    this.keyWord = item.keyWord();
    this.cityId = item.cityId();
  };

  function ViewModel() {
    var self = this;
    var accountInfo = app.cache.get('accountInfo');
    this.areaId = ko.observable();
    this.keyWord = ko.observable();
    this.cityId = ko.observable();
    this.area = ko.observable();
    this.total = ko.observable();
    this.result = ko.observableArray();
    this.areaList = accountInfo.RegionList;
    this.selectedArea = function (record) {
      self.areaId(record.Id);
      self.area(record.Description);
    }
    this.clearFilter = function () {
      self.areaId('');
      self.area('');
      self.keyWord('');
    };
    this.toPosition = function (record) {
      action.toPosition(record.Longitude, record.Latitude);
    };
    this.pagination = new page.Pagination();
    this.searchInfo = function (data) {
      $('body').append(listTemplate.zhuZhaiListTemplate);
      self.result.removeAll();
      for (var i = 0; i < data.length; i++) {
        console.log(setContent(data[i]))
        self.result.push(setContent(data[i]));
      }
    };
    this.search = function () {
      var pm = new PostViewModel(self);
      action.search('WuYeChaXun/ChaXunZhuZhai', pm, self.searchInfo, pointInfoTemplate.zhuZhai);
    };
  }
  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };
  return vm;
});