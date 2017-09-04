define(['ko', 'underscore', 'jquery', 'bootstrap'], function (ko, _, $) {
  //验证
  var app = {};

  _.isNullOrEmpty = function (str) {
    return _.isUndefined(str) || _.isNull(str) || str === '';
  };

  // 消息弹窗  Message popups
  // ==================
  function Messager() {

  }
  Messager.fn = Messager.prototype;
  Messager.fn.alert = function (title, info, icon, callback) {
    callback = callback || icon;
    alert(info);
    if (callback)
      callback();
  }
  Messager.fn.confirm = function (title, info, icon, callback) {
    callback = callback || icon;
    if (confirm(info))
      if (callback)
        callback({ index: 1 });
  }
  Messager.fn.prompt = function (title, info, icon, callback) {
    callback = callback || icon;
    var r = prompt(title, info);
    if (r) {
      callback(r);
    }
  }
  Messager.fn.toast = function (info, callback) {
    this.alert('', info, callback);
  }
  Messager.fn.unopened = function () {
    app.messager.toast('该模块未开放');
  }
  app.messager = new Messager();

  // 验证规则  Validate
  // ==================
  function Validate() { };
  Validate.fn = Validate.prototype;
  Validate.fn.toast = function (msg) {
    app.messager.toast(msg);
  };
  Validate.fn.require = Validate.fn.required = function (value, name, info, noTips) {
    if (_.isNullOrEmpty(value)) {
      name = name || '输入内容';
      !noTips && this.toast(info || (name + '不能为空'));
      return false;
    }
    return true;
  };
  Validate.fn.pattern = function (value, pattern, name, info, noTips) {
    if (_.isString(pattern)) {
      pattern = new RegExp(pattern);
    }
    if (!pattern.test(value)) {
      !noTips && this.toast(info || (name + '输入格式不正确'));
      return false;
    }
    return true;
  };
  Validate.fn.validateNumber = function (value, name, info, noTips) {
    return this.pattern(value, '^\\d*$', name, info, noTips);
  }
  Validate.fn.validateNumber2 = function (value, digit, name, info, noTips) {
    return this.pattern(value, '^\\d*\\.?\\d{0,' + (digit || 2) + '}$', name, info, noTips);
  }
  Validate.fn.validatePassword = function (password, confirmPassword, noTips) {
    var self = this;
    return this.required(password, '密码', null, noTips) &&
      this.required(confirmPassword, '再次输入密码', null, noTips) &&
      function () {
        if (password !== confirmPassword) {
          !noTips && self.toast('两次输入的密码不相同');
          return false;
        }
        return true;
      }();
  };
  Validate.fn.validatePhone = function (phone, name, info, noTips) {
    name = name || '手机号码';
    /*if (!this.required(phone, name, info, noTips)) {
      return false;
    }*/
    //var isPass = phone.match(/^(((13[0-9]{1})|159|153|171)+\d{8})$/);
    var pattern = /^1[34578]\d{9}$/;
    return this.pattern(phone, pattern, name, info, noTips);
  }
  Validate.fn.validateEMail = function (email, name, info, noTips) {
    name = name || '邮箱';
    if (!this.required(email, name)) {
      return false;
    }
    var pattern = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
    return this.pattern(email, pattern, name, info, noTips);
  }
  app.validate = new Validate();

  // 输入验证  Input
  // ==================
  function Input() {

  }
  function pass() {
    return true;
  }
  function input(init, inputValidate, submitValidate) {
    var computed;
    var first = true;
    if (_.isUndefined(init))
      init = '';
    if (inputValidate === pass) {
      computed = ko.observable(init);
    } else {
      var value = ko.observable(init);
      computed = ko.pureComputed({
        read: function () {
          return value();
        },
        write: function (val) {
          var valid = true;
          if (first) {
            first = false;
          } else {
            valid = inputValidate(val);
          }
          var source = value();
          value(val);
          if (source !== '' && !valid) {
            value(source);
          }
        }
      });
    }
    computed.init = function (newInit) {
      init = _.isUndefined(newInit) ? computed() : newInit;
      computed.reset();
    };
    computed.reset = function () { computed(init); };
    computed.getValidate = function () {
      var valid = submitValidate ? submitValidate(computed()) : pass();
      computed.validateSubmit(valid);
      return valid;
    };
    computed.validate = ko.pureComputed(function () {
      var valid = submitValidate ? submitValidate(computed()) : pass();
      //var valid = (submitValidate ? _.bind(submitValidate, computed) : pass)();
      if (first) {
        first = false;
        return true;
      }
      computed.validateSubmit(valid);
      return valid;
    });
    computed.validateSubmit = ko.observable(true);
    /*computed.ignore = function () {
      if (arguments.length === 0) {
        return ignore;
      } else {
        ignore = arguments[0] === true;
        return this;
      }
    };*/
    return computed;
  }
  function array(init, submitValidate) {
    //var ignore;
    init = init || [];
    var observableArray = ko.observableArray(_.clone(init));
    observableArray.init = function (newInit) {
      init = _.isUndefined(newInit) ? observableArray() : newInit;
      observableArray.reset();
    };
    observableArray.reset = function () { observableArray(_.clone(init)); };
    observableArray.getValidate = function () {
      var valid = submitValidate ? submitValidate(observableArray()) : pass();
      observableArray.validateSubmit(valid);
      return valid;
    };
    observableArray.validate = ko.pureComputed(function () {
      var valid = submitValidate ? submitValidate(observableArray()) : pass();
      observableArray.validateSubmit(valid);
      return valid;
    });
    observableArray.validateSubmit = ko.observable(true);
    /*observableArray.ignore = function () {
      if (arguments.length === 0) {
        return ignore;
      } else {
        ignore = arguments[0] === true;
        return this;
      }
    };*/
    return observableArray;
  }
  Input.fn = Input.prototype;
  Input.fn.validate = function (model) {
    _.each(model, function (value) {
      _.isFunction(value) && value.getValidate && value.getValidate();
    });
  };
  Input.fn.getValidate = function (model) {
    var valid = true;
    _.each(model, function (val) {
      if (_.isFunction(val) && val.getValidate) {
        valid = valid & val.getValidate();
      }
    });
    return valid;
  };
  Input.fn.reset = function (model) {
    _.each(model, function (value) {
      _.isFunction(value) && value.reset && value.reset();
    });
  };
  Input.fn.init = function (model) {
    _.each(model, function (value) {
      _.isFunction(value) && value.init && value.init();
    });
  };
  Input.fn.array = function (init, submitValidate) {
    return array(init, submitValidate);
  };
  Input.fn.custom = function (init, inputValidate, submitValidate) {
    return input(init, inputValidate || pass, submitValidate);
  };
  Input.fn.submit = function (init, submitValidate) {
    return input(init, pass, submitValidate);
  };
  Input.fn.required = function (init, submitValidate) {
    return input(init, pass, function (val) {
      return app.validate.required(val, null, null, true) && (submitValidate ? submitValidate(val) : true);
    });
  };
  Input.fn.phone = function (init, submitValidate) {
    return input(init, pass, function () {
      return app.validate.validatePhone(val, null, null, true) && (submitValidate ? submitValidate(val) : true);
    });
  };
  Input.fn.arrayRequired = function (init, submitValidate) {
    return array(init, function () {
      return val.length && (submitValidate ? submitValidate(val) : true);
    });
  };
  Input.fn.bool = function (init) {
    return input(init || false, pass);
  };
  Input.fn.text = function (init) {
    return input(init, pass);
  };
  Input.fn.number = function (init, submitValidate) {
    return input(init, function (val) {
      return _.isUndefined(val) || app.validate.validateNumber(val, null, null, true);
    }, submitValidate);
  };
  Input.fn.number2 = function (init, digit, submitValidate) {
    return input(init, function (val) {
      return app.validate.validateNumber2(val, digit || 2, null, null, true);
    }, submitValidate);
  };
  Input.fn.tel = function (init, submitValidate) {
    return input(init, function (val) {
      return app.validate.pattern(val, /^[\d;,-]+$/, null, null, true);
    }, submitValidate);
  };
  Input.fn.telRequired = function (init, submitValidate) {
    return input(init, pass, function (val) {
      return app.validate.required(val, null, null, true) && app.validate.validatePhone(val, null, null, true);
    });
  };
  app.input = new Input();

  // 工具  Util
  // ==================
  function Util() { }
  Util.fn = Util.prototype;
  Util.fn.showModalDialog = function (elementId) {
    $(elementId).modal('toggle');
  };
  app.util = new Util();

  // 请求  Request
  // ==================
  function Request() {
    var self = this;
    self.ajax = function (options) {
      $.ajax(options);
    }
    self.postAction = function (prefix, action, data, callback) {
      if (_.isObject(action)) {
        callback = data;
        data = action;
        action = prefix;
        prefix = '/Api/webReq/';
      }
      var options = {
        url: prefix + action,
        data: data,
        type: 'POST',
        success: callback
      };
      self.ajax(options);
    };
  }
  app.request = new Request();


  //获取验证码
  function GetCode(validateParams, vm) {
    var validate = validateParams;
    var i = 60;
    var waitHandle;
    this.getCode = function () {
      if (validate.require(vm.phone(), '手机号码', vm.phoneTip) && validate.validatePhone(vm.phone(), ' ', vm.phoneTip)) {
        vm.disableCss(true);
        // 页面数字
        request.ajaxPost(null, 'ValidationCode/GetCode', { Phone: vm.phone() }, function (data) {
          if (data.Result === 'success') {
            waitHandle = window.setInterval(function () { wait(vm) }, 1000);
            alert(data.Response);
          } else {
            alert(data.Response);
          }
        });
      }
    }

    function wait(vm) {
      if (i === 0) {
        vm.btnCodeText('获取验证码');
        i = 60;
        vm.disableCss(true);
        window.clearInterval(waitHandle);
      } else {
        vm.btnCodeText('(' + i + ')秒重试');
      }
      i--;
    }
  };
  app.GetCode = GetCode;

  //输入框获取焦点
  function inputFocus() {
    $('input').on({
      focus: function () {
        $(this).parents('.input-group').addClass('input-group-focus');
      },
      blur: function () {
        $(this).parents('.input-group').removeClass('input-group-focus');
      }
    });
  };
  app.inputFocus = inputFocus;

  //sessionStorage
  function Cache() {
  }
  Cache.fn = Cache.prototype;
  Cache.fn.set = function (key, data) {
    localStorage[key] = JSON.stringify(data);
  }
  Cache.fn.get = function (key) {
    var val = localStorage[key];
    if (_.isUndefined(val) || _.isNull(val)) {
      return undefined;
    }
    return JSON.parse(val);
  }
  app.cache = new Cache();

  return app;
});