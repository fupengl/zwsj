(function (require) {
  require.config({
    baseUrl: './',
    paths: {
      'jquery': '/Scripts/jquery-3.1.0.min',
      'bootstrap': '/Scripts/bootstrap.min',
      'ko': '/Scripts/knockout-3.4.1',
      'underscore': '/Scripts/underscore.min',
      'underscoreString': '/Scripts/underscore.string.min'
    },
    shim: {
      bootstrap: {
        deps: [
            'jquery',
            'css!/Content/bootstrap.min.css'
        ]
      }
    },
    map: {
      '*': {
        'css': '/Scripts/css.min.js'
      }
    }
  });

  require.onError = function (err) {
    console.log(err);
  };

  require(['bootstrap', 'ko', 'underscore', 'underscoreString'], function (ko, app) {
    //ko.applyBindings(app);
  });
})(require)