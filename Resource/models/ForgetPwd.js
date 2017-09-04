define([
  'ko',
  'zd'
], function (ko, app) {
  function ForgetPassWordModel() {
    var self = this;
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
  }

  function check(validateParams, item) {
    var validate = validateParams;
    var b1 = validate.require(item.phone(), '手机号码', item.phoneTip);
    var b2 = validate.validatePhone(item.phone(), ' ', item.phoneTip);
    var b3 = validate.require(item.code(), '验证码', item.codeTip);
    var b4 = validate.require(item.password(), '密码', item.passwordTip);
    var b5 = validate.require(item.repassword(), '确认密码', item.repasswordTip);
    var b6 = validate.validatePassword(item.password(), item.repassword(), item.repasswordTip);
    if (b1 && b2 && b3 && b4 && b5 && b6) {
      return true;
    } else {
      return false;
    }
  };

  function ViewModel() {
    var self = this;
    this.input = new ForgetPassWordModel();
    this.getCodeUrl = 'ValidationCode/GetFindbackPasswordCode';
    this.codeInfo = new app.GetCode(this);
    this.successText = '正在返回登录页面';
    this.resetPassword = function () {
      var pm = new PostViewModel(self.input);
      var valid = app.input.getValidate(self.input);
      if (!valid) {
        return;
      }
      app.request.postAction('FindbackPassword/FindBack', pm, function (data) {
        if (data.Result === 'success') {
          app.util.showModalDialog('#actionSuccessModal');
          setTimeout(function () {
            window.location.href = '/Account/LoginWeb';
          }, 1000);
        } else {
          alert(data.Response);
        }
      });
    }
  }

  app.util.inputFocus();
  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm);
  };
  return vm;
});