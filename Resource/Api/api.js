define([
    'commet'
], function (app) {

  var status = false;
  var url = '/Api/Mobile/';
  return {

    //LoginEx1: function (parmes) {
    //  return app.ajaxGet('Api/Mobile/Login/LoginEx', parmes);
    //},

    LoginEx: function (parmes) {
      return app.ajaxPost(url + 'Login/LoginEx', parmes)
    },

    getStatus: function () {
      return status
    },

    changeStatus: function (val) {
      status = val;
    }

  }

});