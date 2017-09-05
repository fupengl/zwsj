define([
'ko',
'echarts',
 'zd',
'../components/index'

], function (ko, ec, app) {

  function ViewModel() {
    var self = this;
    this.detailInfo = ko.observableArray();
    this.chart = '';
    this.data = ko.observableArray();
    this.x = ko.observableArray();
    var myChart = ec.init(document.getElementById('main'));

    self.data = [
      'https://baijia.baidu.com/s?id=1577586759667075090&wfr=pc&fr=idx_lst',
       'https://baijia.baidu.com/s?id=1577586759667075090&wfr=pc&fr=idx_lst',
        'https://baijia.baidu.com/s?id=1577586759667075090&wfr=pc&fr=idx_lst',
         'https://baijia.baidu.com/s?id=1577586759667075090&wfr=pc&fr=idx_lst',
    ];

    var xData = [],
        label = [],
        series = [];
    app.request.postAction('JiaGeZouShi/GetShangYeData', { WuYeId: '192540' }, function (data) {
      if (data.Result === 'success') {
        xData = data.Response[0].XAxisName;
        _.each(data.Response, function (item, i) {
          label.push(item.WuYeLeiXing);
          var temp = {
            name: item.WuYeLeiXing,
            type: 'line',
            stack: '',
            data: []
          };
          _.each(item.YAxisName, function (item1, j) {
            temp.data.push(item1);
          });
          series.push(temp);
        });
        var option = {
          title: {
            text: '折线图堆叠'
          },
          tooltip: {
            trigger: 'axis'
          },
          chart: {
            marginRight: '220'
          },
          legend: {
            align: "right",//程度标的目标地位
            verticalAlign: "top", //垂直标的目标地位
            right: 0, //间隔x轴的间隔
            y: 100,//间隔Y轴的间隔
            data: label
          },
          grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
          },
          toolbox: {
            feature: {
              saveAsImage: {}
            }
          },
          xAxis: {
            type: 'category',
            boundaryGap: false,
            data: xData
          },
          yAxis: {
            type: 'value'
          },
          series: series
        };
        myChart.setOption(option);
      } else {

        alert(data.Response);
      }
    });

    app.request.postAction('WuYeXinXi/ShangYeDetailInfo', { WuYeId: '192540' }, function (data) {

      self.chart = data.Chart;
      console.log(data);
      for (var i = 0; i < data.Display.length; i++) {
        self.detailInfo.push(data.Display[i]);
      }
      //_.each(data.Photo, function (item, i) {
      //  app.request.postAction('WuYeZhaoPian/GetZhaoPianUrl', { PhotoId: item, Width: '200', Height: '200' }, function (data1) {
      //    if (data1.Result.success) {
      //      self.data.push(Response);
      //    } else {

      //    }
      //  });
      //});
    });

  }

  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };

  return vm;
});
