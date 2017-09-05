define([
'ko',
 'zd',
], function (ko, app) {
  function ViewModel() {

    var self = this;
    this.detailInfo = ko.observableArray();
    this.section1 = ko.observableArray();
    this.section2 = ko.observableArray();
    this.section3 = ko.observableArray();
    this.section4 = ko.observableArray();

    app.request.postAction('ZhuZhaiPingGu/GetLouDongAndWuYeLeiXing', { WuYeId: '192540' }, function (data) {
      console.log(data)

    });
  }

  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };

  return vm;


});