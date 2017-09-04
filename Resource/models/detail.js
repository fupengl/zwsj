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
    this.photoIds = ko.observableArray();
    this.x = ko.observableArray();
    var myChart = ec.init(document.getElementById('main'));
    app.request.postAction('JiaGeZouShi/GetZhuZhaiData', { WuYeId: '192540' }, function (data) {
      console.log(data)
      if (data.Result === 'success') {
       self.x(data.Response[0].XAxisName);     
       self.y = data.YAxisName;
      } else {
        alert(data.Response);
      }
    });
    console.log(self.x())
    option = {
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
        data: ['邮件营销', '联盟广告', '视频广告', '直接访问', '搜索引擎']
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
        data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
      },
      yAxis: {
        type: 'value'
      },
      series: [
          {
            name: '搜索引擎',
            type: 'line',
            stack: '总量',
            data: [820, 932, 901, 934, 1290, 1330, 1320]
          },
          {
            name: '搜索引擎',
            type: 'line',
            stack: '总量',
            data: [820, 932, 901, 934, 1290, 1330, 1320]
          }
      ]
    };
    myChart.setOption(option);
    this.data = '';
    //self.data = [
    //    'https://haitaoad.nosdn.127.net/ad.bid.material_d071e5202e23434797d1be4a36f1aae0?imageView&thumbnail=240x240&quality=100',
    //    'http://imgsize.ph.126.net/?imgurl=http://imglf1.nosdn.127.net/img/NnFaNTJ4UklQYmJWYXJiNzVqT3JVUlRPTGRGSWtEVUxSMWk2Yk1BdkQ2cDFOMXZXa0k1NWdBPT0.jpg?imageView_240x150x1.jpg&enlarge=true&0001',
    //    'https://haitaoad.nosdn.127.net/ad.bid.material_d071e5202e23434797d1be4a36f1aae0?imageView&thumbnail=240x240&quality=100',
    //    'http://imgsize.ph.126.net/?imgurl=http://imglf1.nosdn.127.net/img/NnFaNTJ4UklQYmJWYXJiNzVqT3JVUlRPTGRGSWtEVUxSMWk2Yk1BdkQ2cDFOMXZXa0k1NWdBPT0.jpg?imageView_240x150x1.jpg&enlarge=true&0001',
    //    'https://haitaoad.nosdn.127.net/ad.bid.material_d071e5202e23434797d1be4a36f1aae0?imageView&thumbnail=240x240&quality=100',
    //     'http://imgsize.ph.126.net/?imgurl=http://imglf1.nosdn.127.net/img/NnFaNTJ4UklQYmJWYXJiNzVqT3JVUlRPTGRGSWtEVUxSMWk2Yk1BdkQ2cDFOMXZXa0k1NWdBPT0.jpg?imageView_240x150x1.jpg&enlarge=true&0001',
    //          'https://haitaoad.nosdn.127.net/ad.bid.material_d071e5202e23434797d1be4a36f1aae0?imageView&thumbnail=240x240&quality=100',
    //];

    app.request.postAction('WuYeXinXi/ZhuZhaiDetailInfo', { WuYeId: '192540' }, function (data) {
      self.chart = data.Chart;
      console.log(data);
      for (var i = 0; i < data.Display.length; i++) {
        self.detailInfo.push(data.Display[i]);
      }
    });


    app.request.postAction('WuYeZhaoPian/GetZhaoPianUrl', { PhotoId: ['2637'], Width: '200', Height: '200' }, function (data) {
      console.log(data);

    });
  }

  var vm = new ViewModel();
  vm.initialize = function () {
    ko.applyBindings(vm, document.getElementById('container'));
  };

  return vm;
});
