define([
'ko',
 'zd',
], function (ko, app) {
  function ViewModel() {

    var self = this;
    this.detailInfo = ko.observableArray();
    this.section1 = ko.observableArray();
    this.section2 =ko.observableArray();
    this.section3 =ko.observableArray();
    this.section4 = ko.observableArray();

    app.request.postAction('WuYeXinXi/TuDiDetailInfo', { WuYeId: 91566 }, function (data) {
      var display = data.Display;
      for (var i = 0; i < display.length; i++) {
        if (display[i].Section == 1) {
          self.section1.push(display[i]);
        } else if (display[i].Section == 2) {
          self.section2.push(display[i]);
        } else if (display[i].Section == 3) {
          self.section3.push(display[i]);
        }else if (display[i].Section == 4) {
          self.section4.push(display[i]);
        }          
      }
      
    });
  }

  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };

  return vm;


});