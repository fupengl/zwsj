define(['ko', 'underscore', 'jquery','bootstrap'], function (ko, _,$) {
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
  // 输入验证
  function Input() {

  }
  // 默认通过函数
  function pass() {
    return true;
  }
  // 扩展提交验证函数
  function validateEx(submitValidate, val) {
    return submitValidate ? submitValidate(val) : pass();
  }
  // 获取参数
  function options(init, inputValidate, submitValidate) {
    var opt = {};
    if (_.isObject(init)) {
      opt.inputValidate = function (val) {
        return validateEx(init.inputValidate, val) && validateEx(inputValidate, val);
      }
      opt.submitValidate = function (val) {
        return validateEx(init.submitValidate, val) && validateEx(submitValidate, val);
      }
      opt = _.extend({}, init, opt);
    } else {
      opt.init = init;
      opt.inputValidate = inputValidate;
      opt.submitValidate = submitValidate;
    }
    if (opt.noTips === undefined) {
      opt.noTips = true;
    }
    return opt;
  }
  function input(option) {
    var computed, first = true;
    var init = option.init,
      inputValidate = option.inputValidate,
      submitValidate = option.submitValidate;
    //var ignore;
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
          var valid = inputValidate(val);
          var source = value();
          value(val);
          if (valid !== '' && !valid) {
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
    /* 
    实时验证输入内容，用于 ko 的 textInput 或 keydown 事件
    css: { 'has-error': !title.validate() || !title.validateSubmit() }
    */
    computed.validate = ko.pureComputed(function () {
      var valid = inputValidate ? inputValidate(computed()) : pass();
      if (first) {
        first = false;
        return true;
      }
      computed.validateSubmit(valid);
      return valid;
    });
    // 用于提交表单验证，或失去焦点 blur 等事件
    computed.getValidate = function () {
      var valid = submitValidate ? submitValidate(computed()) : pass();
      computed.validateSubmit(valid);
      return valid;
    };
    // 提交表单验证验证结果
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

  function array(option) {
    //var ignore;
    var first = true;
    var init = option.init,
      submitValidate = option.submitValidate;
    init = init || [];
    var observableArray = ko.observableArray(_.clone(init));
    observableArray.init = function (newInit) {
      init = _.isUndefined(newInit) ? observableArray() : newInit;
      observableArray.reset();
    };
    observableArray.reset = function () { observableArray(_.clone(init)); };
    observableArray.validate = ko.pureComputed(function () {
      if (first) {
        first = false;
        return true;
      }
      var valid = submitValidate ? submitValidate(observableArray()) : pass();
      observableArray.validateSubmit(valid);
      return valid;
    });
    observableArray.getValidate = function () {
      var valid = submitValidate ? submitValidate(observableArray()) : pass();
      observableArray.validateSubmit(valid);
      return valid;
    };
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
  Input.fn.options = options;
  Input.fn.pass = pass;
  Input.fn.validateEx = validateEx;
  // 验证所有
  Input.fn.validate = function (model) {
    _.each(model, function (value) {
      _.isFunction(value) && value.getValidate && value.getValidate();
    });
  };
  // 验证所有
  Input.fn.getValidate = function (model) {
    var valid = true;
    _.each(model, function (val) {
      if (_.isFunction(val) && val.getValidate) {
        valid = valid & val.getValidate();
      }
    });
    return valid;
  };
  // 验证失败终止后续
  Input.fn.getValidate2 = function (model) {
    var valid = true;
    _.each(model, function (val) {
      if (!valid && _.isFunction(val) && val.getValidate) {
        valid = valid && val.getValidate();
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
    var option = options(init, pass, submitValidate);
    return array(option);
  };
  Input.fn.custom = function (init, inputValidate, submitValidate) {
    var option = options(init, inputValidate, submitValidate);
    return input(option);
  };
  Input.fn.submit = function (init, submitValidate) {
    var option = options(init, pass, submitValidate);
    return input(option);
  };
  Input.fn.required = function (init) {
    var option = options(init, pass, function (val) {
      return app.validate.required(val, null, null, true);
    });
    return input(option);
  };
  Input.fn.arrayRequired = function (init, submitValidate) {
    var option = options(init, pass, function (val) {
      return val.length && (submitValidate ? submitValidate(val) : true);
    });
    return array(option);
  };
  Input.fn.bool = function (init) {
    var option = options(init, pass, function (val) {
      return _.isBoolean(val);
    });
    if (option.init !== true) {
      option.init = false;
    }
    return input(option);
  };
  Input.fn.number = function (init) {
    var option;
    var validate = function (val) {
      return app.validate.validateNumber(val, option.name, option.title, option.noTips);
    };
    option = options(init, validate, validate);
    return input(option);
  };
  Input.fn.numberRequired = function (init) {
    var option = options(init, function (val) {
      return app.validate.validateNumber(val, option.name, option.title, option.noTips);
    }, function (val) {
      return app.validate.required(val, option.name, option.title, option.noTips) && app.validate.validateNumber(val, option.name, option.title, option.noTips);
    });
    return input(option);
  };
  Input.fn.number2 = function (init, digit) {
    var option;
    var validate = function (val) {
      return app.validate.validateNumber2(val, option.digit, option.name, option.title, option.noTips);
    };
    option = options(init, validate, validate);
    option.digit = digit || option.digit || 2;
    return input(option);
  };
  Input.fn.numberRequired2 = function (init, digit) {
    var option = options(init, function (val) {
      return app.validate.validateNumber2(val, option.digit, option.name, option.title, option.noTips);
    }, function (val) {
      return app.validate.required(val, null, null, true) && app.validate.validateNumber2(val, option.digit, option.name, option.title, option.noTips);
    });
    option.digit = digit || option.digit || 2;
    return input(option);
  };
  Input.fn.tel = function (init) {
    var validate = function (val) {
      return app.validate.pattern(val, /^[\d;,-]+$/, null, null, true);
    };
    var option = options(init, validate, function (val) {
      return validate(val);
    });
    return input(option);
  };
  Input.fn.telRequired = function (init) {
    var option = options(init, function (val) {
      return app.validate.pattern(val, /^[\d;,-]+$/, null, null, true);
    }, function (val) {
      return app.validate.required(val, null, null, true) && app.validate.pattern(val, /^[\d;,-]+$/, null, null, true);
    });
    return input(option);
  };
  app.input = new Input();

  //获取验证码
  function GetCode(vm) {
    var self = this;
    this.disableCss = ko.observable(false);
    this.btnCodeText = ko.observable('获取验证码');
    var i = 60;
    var waitHandle;
    this.getCode = function () {
      self.disableCss(true);
      var valid = vm.input.phone.getValidate();
      if (!valid) {
        return;
      }
      app.request.postAction(vm.getCodeUrl, { Phone: vm.input.phone() }, function (data) {
        if (data.Result === 'success') {
          waitHandle = window.setInterval(function () { wait(self) }, 1000);
          alert(data.Response);
        } else {
          alert(data.Response);
        }
      });
    }

    function wait(self) {
      if (i === 0) {
        self.btnCodeText('获取验证码');
        i = 60;
        self.disableCss(true);
        window.clearInterval(waitHandle);
      } else {
        self.btnCodeText('(' + i + ')秒重试');
      }
      i--;
    }
  };
  app.GetCode = GetCode;

  // 工具  Util
  // ==================
  function Util() { }
  Util.fn = Util.prototype;
  Util.fn.showModalDialog = function (elementId) {
    $(elementId).modal('toggle');
  };
  Util.fn.inputFocus = function () {
    $('input.has-focus')
      .on('focus', function () {
        $(this).parents('.input-group').addClass('input-group-focus');
      })
      .on('blur', function () {
        $(this).parents('.input-group').removeClass('input-group-focus');
      });
    /*$('input').on({
      focus: function () {
        $(this).parents('.input-group').addClass('input-group-focus');
      },
      blur: function () {
        $(this).parents('.input-group').removeClass('input-group-focus');
      }
    });*/
  };
  app.util = new Util();

  // 请求  Request
  // ==================
  function Request() {
    var self = this;
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
    self.ajax = function (options) {
      $.ajax(options);
    }
  }
  app.request = new Request();


  // 存储  Cache
  // ==================
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