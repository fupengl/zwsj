define(['ko'], function (ko) {
  function PaginationModel(searchFunc, showPageCount) {
    //viewModel本身。用来防止直接使用this的时候作用域混乱
    var self = this;
    self.showPageCount = showPageCount || 10;
    var half = parseInt(self.showPageCount / 2);
    self.prePageCount = half;
    self.nextPageCount = self.showPageCount % 2 ? half : half - 1;
    self.pageIndex = ko.observable(1);//当前第几页
    self.pageCount = ko.observable(1);//分页总数
    self.allPages = ko.observableArray();//输出哪些页面
    self.refresh = function () {
      //限制请求页码在该数据页码范围内
      if (self.pageIndex() < 1)
        self.pageIndex(1);
      if (self.pageIndex() > self.pageCount()) {
        self.pageIndex(self.pageCount());
      }
      //console.log(self.pageIndex() === self.pageCount());
      searchFunc();
    };

    //请求第一页数据
    self.first = function () {
      self.pageIndex(1);
      self.refresh();
    };
    //请求下一页数据
    self.next = function () {
      self.pageIndex(self.pageIndex() + 1);
      self.refresh();
    };
    //请求先前一页数据
    self.previous = function () {
      self.pageIndex(self.pageIndex() - 1);
      self.refresh();
    };
    //请求最后一页数据
    self.last = function () {
      self.pageIndex(self.pageCount());
      self.refresh();
    };
    self.goToPageIndex = ko.pureComputed(function () {
      return self.pageIndex();
    });
    self.goTo = function (r, e) {
      var pageIndex = parseInt($(e.target).prev().val());
      if (isNaN(pageIndex)) return;
      if (pageIndex <= 0) return;
      self.gotoPage(pageIndex);
    };
    //跳转到某页
    self.gotoPage = function (data, event) {
      self.pageIndex(data);
      self.refresh();
    };
    self.clear = function () {
      self.allPages.removeAll();
      self.pageIndex(1);
    };
    self.loadPages = function (data) {
      self.pageCount(data.totalPage);
      var index = self.pageIndex();
      var total = data.totalPage;
      if (data.totalPage > 0 && data.totalPage < index) {
        self.pageIndex(data.totalPage);
        searchFunc();
        return;
      }

      // 加载新的数据前，先移除原先的数据
      self.allPages.removeAll();
      var startIndex, endIndex;
      if (index <= self.prePageCount) {
        startIndex = 1;
        endIndex = total > self.showPageCount ? self.showPageCount : total;
      } else if (index > total - self.nextPageCount) {
        startIndex = total - self.showPageCount + 1;
        if (startIndex <= 0) startIndex = 1;
        endIndex = total;
      } else {
        startIndex = index - self.prePageCount;
        endIndex = startIndex + self.showPageCount - 1;
      }

      for (var i = startIndex; i <= endIndex; i++) {
        //装填页码
        self.allPages.push(i);
      }
    };

  };
  return { Pagination: PaginationModel }
});