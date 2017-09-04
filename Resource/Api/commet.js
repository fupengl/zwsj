define([
    'jquery'
], function ($) {

  var defauleHeader = {};

  var debug = true;

  var loginData = window.localStorage.getItem('loginInfo') || false;

  return {

    config: {
      debug: debug,
      loginData: loginData
    },
    ajaxPost: function (path, url, parmes, successCallBack, err) {
      return $.ajax({
        url: path || '/Api/webReq/' + url,
        datatype: 'json',
        method: 'POST',
        data: parmes,
        header: defauleHeader,
        success: successCallBack,
        error: err
      });
    }
  }

});