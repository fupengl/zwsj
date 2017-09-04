define([
  'ko',
  'zd',
  'commet'
], function (ko, app, request) {

  function checkModifyPassword(validateParams, item) {
    var validate = validateParams;
    var b1 = validate.require(item.phone(), '登录账号', item.phoneTip);
    var b2 = validate.require(item.password(), '密码', item.passwordTip);
    var b3 = validate.require(item.oldPassword(), '旧密码', item.oldPasswordTip);
    if (b1 && b2 && b3) {
      return true;
    } else {
      return false;
    }
  };

  function checkModifyIndustry(validateParams, item) {
    var validate = validateParams;
    var b1 = validate.require(item.phone(), '登录账号', item.phoneTip);
    var b2 = validate.require(item.job(), '所属行业', item.jobTip);

    if (b1 && b2) {
      return true;
    } else {
      return false;
    }
  };

  function ViewModel() {
    var validate = new app.Validate();
    var self = this;
    self.currentPhone = ko.observable('111');
    self.currentJob = ko.observable('333');
    self.currentPassword = ko.observable('11441');

    self.phone = ko.observable('111');
    self.job = ko.observable('job');
    self.password = ko.observable('1111');
    self.oldPassword = ko.observable();
    self.phoneTip = ko.observable();
    self.jobTip = ko.observable();
    self.passwordTip = ko.observable();
    self.oldPasswordTip = ko.observable();
    self.jobList = ['金融', '地产', '政府', '中介', '其他'];

    self.selectedJob = function (record) {
      self.job(record);
    };
    self.exit = function () {

    }

    self.modifyIndustry = function () {
      if (checkModifyIndustry(validate, self)) {
        request.ajaxPost(null, 'FindbackPassword/ModifyJob', { UserName: self.phone(), Job: self.job() }, function (data) {
          console.log(data)
          if (data.Result === 'success') {
            console.log(data)
          } else {
            console.log(data)
          }
        });
      }
    }

    self.modifyPassword = function () {
      if (checkModifyPassword(validate,self)) {
        request.ajaxPost(null, 'FindbackPassword/ModifyPassword', { UserName: self.phone(), Password: self.password(), OldPassword: self.oldPassword() }, function (data) {
          console.log(data)
          if (data.Result === 'success') {
            console.log(data)
          } else {
            console.log(data)
          }
        });

      }


    }

    //self.savePassword=function() {

    //}
  }


  return ViewModel;

});