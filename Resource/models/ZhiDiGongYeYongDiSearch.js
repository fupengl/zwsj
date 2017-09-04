define([
  'ko',
  'zd',
  './map/zhiDiMapListTemplate',
  './map/zhiDiMapPointInfoTemplate',
  './map/zhiDiMap',
  './zdPagination'
], function (ko, app, listTemplate, pointInfoTemplate, action, page) {
  function getYear(item) {
    var date = new Date();
    var maxYear = date.getFullYear() + 1;
    var minYear = maxYear - 17;
    _.each(_.range(minYear, maxYear), function (val) {
      item.yearList.push(val);
    });
  };

  function setContent(item) {
    this.toDetail = 'ZhiDiZhuZhaiDetail?id=' + item.WuYeId;
    if (!item.PhotoUrl) {
      this.PhotoUrl = '/Resource/assets/images/img_default_houseMapList.png';
    }
  }

  function PostViewModel(item) {
    this.areaId = item.areaId();
    this.keyWord = item.keyWord();
    this.nianFen = item.nianFen();
    this.cityId = item.cityId();
  };

  function ViewModel() {
    var self = this;
    var accountInfo = app.cache.get('accountInfo');
    this.areaId = ko.observable();
    this.keyWord = ko.observable();
    this.nianFen = ko.observable();
    this.yearList = ko.observableArray();
    this.cityId = ko.observable();
    this.area = ko.observable();
    this.total = ko.observable();
    this.result = ko.observableArray();
    this.areaList = accountInfo.RegionList;
    this.selectedArea = function (record) {
      self.areaId(record.Id);
      self.area(record.Description);
    }
    this.selectedYear = function (record) {
      self.nianFen(record);
    };
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
      $('body').append(listTemplate.gongYeYongDiListTemplate);
      self.result.removeAll();
      for (var i = 0; i < data.length; i++) {
        setContent(data[i]);
        self.result.push(data[i]);
      }
    };
    this.search = function () {
      var pm = new PostViewModel(self);
      action.search('WuYeChaXun/ChaXunTuDi', pm, self.searchInfo, pointInfoTemplate.gongYeYongDi);
    };
    getYear(this);
  }
  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };
  return vm;
});