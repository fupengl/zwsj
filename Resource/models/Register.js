define([
  'ko',
  'zd'
], function (ko, app) {

  function RegisterModel() {
    var self = this;
    this.city = app.input.required();
    this.job = app.input.required();
    this.phone = app.input.telRequired();
    this.code = app.input.required();
    this.password = app.input.required();
    this.confirmPassword = app.input.required({
      submitValidate: function (val) {
        return self.password() === val;
      }
    });
  }

  function PostViewModel(item) {
    this.phone = item.phone();
    this.code = item.code();
    this.password = item.password();
    this.city = item.city();
    this.job = item.job();
  }

  function DataSource(code, cityInfo) {
    this.code = code;
    this.cityInfo = cityInfo;
  }

  function getCity(cityList) {
    app.request.postAction('Login/GetCities', {}, function (data) {
      if (data.Result === 'success') {
        var codes = _.pluck(data.Response, 'Code');
        var codeList = _.uniq(codes);
        _.each(codeList, function (j) {
          var cityInfo = _.filter(data.Response, function (val) {
            return val.Code === j;
          });
          cityList.push(new DataSource(j, cityInfo));
        });
      }
    });
  }

  function ViewModel() {
    var self = this;
    this.input = new RegisterModel();
    this.cityList = ko.observableArray();
    this.getCodeUrl = 'ValidationCode/GetCode';
    this.codeInfo = new app.GetCode(this);
    this.successText = '正在返回登录页...';
    this.inputFocus = ko.observable(false);
    this.jobList = ['金融', '地产', '政府', '中介', '其他'];
    this.selectedCity = function (record) {
      var util = app.util;
      self.input.city(record.Description);
      util.showModalDialog('#selectedCityModal');
    };
    this.cityListMore = function () {
      var $element = $(arguments[1].target);
      $element.parent().toggleClass('less');
      $element.parent().siblings('.city-list').toggleClass('overflow-ellipsis');
      $element.parents('.city-list-wrap').toggleClass('bg-more');
    };
    this.selectedJob = function (record) {
      self.input.job(record);
    };
    this.register = function () {
      var valid = app.input.getValidate(self.input);
      if (!valid) {
        return;
      }
      var pm = new PostViewModel(self.input);
      app.request.postAction('UserRegist/Regist', pm, function (data) {
        if (data.Result === 'success') {
          app.util.showModalDialog('#actionSuccessModal');
          setTimeout(function () {
            location.href = '/ZhiDiZhuZhaiSearch/Index';
          }, 1000);
        } else {
          alert(data.Response);
        }
      });
    };
    getCity(self.cityList);
  }

  app.util.inputFocus();
  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm);
  };
  return vm;
});