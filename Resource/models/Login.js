define([
  'ko',
  'zd'
], function (ko, app) {

  function PostViewModel(item) {
    this.userName = item.userName();
    this.password = item.password();
  }

  function DataSource(imgPath, title, content) {
    this.imgPath = imgPath;
    this.title = title;
    this.content = content;
  };

  function LoginModel() {
    this.userName = app.input.telRequired();
    this.password = app.input.required();
  }

  function ViewModel() {
    var self = this;
    this.input = new LoginModel();
    var path = '../Resource/assets/images/';
    this.data = [
      new DataSource(path + 'icon_login_info_search.png', '信息查询', '可查询楼盘信息（住宅、办公、商业）和土地招拍挂的相关信息。土地招拍挂涵盖历年经营性土地的招拍挂成交信息；住宅数据主要包括小区基础信息和价格信息，商业及办公主要包含租金、售价信息的查询。'),
      new DataSource(path + 'icon_login_evaluation_.png', '自动评估', '针对普通存量住宅一房一价精确评估和工业厂房的预评。其中普通存量住宅评估，通过输入住宅面积、楼层、朝向等基本信息，实现自动估值；工业厂房的预评通过选择工业片区，输入相关信息，实现自动估值。 '),
      new DataSource(path + 'icon_login_bulletin.png', '公告查阅', '可查阅入线城市智地周刊、土地招拍挂公告及地产沙龙。其中智地周刊我们为客户每周提供房地产的最新动态；土地招拍挂公告根据实际情况不定时发布；地产沙龙为专业客户提供房地产相关的活动信息。 ')
    ];
    this.login = function () {
      var pm = new PostViewModel(self.input);
      var valid = app.input.getValidate(self.input);
      if (!valid) {
        return;
      }
      app.request.postAction('Login/LoginEx', pm, function (data) {
        if (data.Result === 'success') {
          app.cache.set('accountInfo', data.Response);
          location.href = '/ZhiDiZhuZhaiSearch/Index';
        } else {
          alert(data.Response);
        }
      });
    };
  }

  app.util.inputFocus();
  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm);
  };

  return vm;
});