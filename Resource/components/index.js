define([
    'ko',
    './carousel/carousel'
], function (ko, carousel) {

  return {
    carousel: carousel(),
  }

});