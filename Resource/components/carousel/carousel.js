define([
    'ko',
    'jquery',
    'css!Resource/components/carousel/carousel.css',
    'Resource/components/carousel/assets/pic_tab'
], function (ko) {

  return function () {
    ko.components.register("app-imgbox", {
      template: { require: "text!Resource/components/carousel/carousel.html", synchronous: true },
      viewModel: { require: "Resource/components/carousel/model.js", synchronous: true }
    })

  }

});