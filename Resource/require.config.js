var path = '../Resource/models/';
(function (require) {
  "use strict";
  require.config({
    baseUrl: '/',
    paths: {
      'module': 'Resource/models',
      'zd': 'Resource/zd',
      'jquery': 'Scripts/jquery-3.1.0.min',
      'bootstrap': 'Scripts/bootstrap.min',
      'ko': 'Scripts/knockout-3.4.1',
      'underscore': 'Scripts/underscore.min',
      'underscoreString': 'Scripts/underscore.string.min',
      //'domReady': '/Scripts/domReady',
      'text': 'Scripts/text',
      'async': 'Scripts/async',
      'BMap': 'http://api.map.baidu.com/api?key=&v=1.4&services=true',
      'echarts': '/Scripts/ECharts/echarts',
      'echart-line': '/Scripts/ECharts/chart/line',
    },
    waitSeconds: 0,
    shim: {
      bootstrap: {
        deps: [
            'jquery'
        ]
      },
      'BMap': {
        deps: ['jquery'],
        exports: 'BMap'
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
})(require);