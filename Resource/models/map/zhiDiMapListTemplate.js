define(function () {
  //住宅
  var zhuZhaiListTemplate = '<script type="text/html" id="listTemplate">' +
  '<h4 class="media-heading">' +
    '<a href="" data-bind="text: WuYeMingCheng,click: $parent.toPosition" class="list-title"></a>' +
    '<small>' +
    '<a data-bind="attr: { href: toDetail }">详情</a>' +
    '</small>' +
    '</h4>' +
    '<p class="secondaryTextColor">地址：<span data-bind="text: ZuoLuo"><span></p>' +
    '<p class="secondaryTextColor">行政区域：<span data-bind="text: XiangGuanXinXi"><span></p>' +
    '<p class="secondaryTextColor"><span data-bind="html: JiaGeQuJian"><span></p>' +
    '</script>';

  //商业、办公、工业
  var houseMapListTemplate = '<script type="text/html" id="listTemplate">' +
  '<h4 class="media-heading">' +
    '<a href="" data-bind="text: WuYeMingCheng,click: $parent.toPosition" class="list-title"></a>' +
    '<small>' +
    '<a data-bind="attr: { href: toDetail }">详情</a>' +
    '</small>' +
    '</h4>' +
    '<p class="secondaryTextColor">地址：<span data-bind="text: ZuoLuo"><span></p>' +
    '<p class="secondaryTextColor">行政区域：<span data-bind="text: XiangGuanXinXi"><span></p>' +
     '<p class="secondaryTextColor">租金：<span data-bind="text: ZuJinQujian"><span></p>' +
     '<p class="secondaryTextColor" data-bind="text: ShouJiaQuJian"><span ><span></p>' +
    '</script>';

  //土地招拍挂
  var gongYeYongDiListTemplate = '<script type="text/html" id="listTemplate">' +
  '<h4 class="media-heading">' +
    '<a href="" data-bind="text: WuYeMingCheng,click: $parent.toPosition" class="list-title"></a>' +
    '<small>' +
    '<a data-bind="attr: { href: toDetail }">详情</a>' +
    '</small>' +
    '</h4>' +
    '<p class="secondaryTextColor">地址：<span data-bind="text: ZuoLuo"><span></p>' +
    '<p class="secondaryTextColor">竟得单位：<span data-bind="text: JingDeDanWei"><span></p>' +
     '<p class="secondaryTextColor">土地用途：<span data-bind="text: DiKuaiYongTu"><span></p>' +
    '</script>';
  return {
    zhuZhaiListTemplate: zhuZhaiListTemplate,
    houseMapListTemplate: houseMapListTemplate,
    gongYeYongDiListTemplate: gongYeYongDiListTemplate
  }
});
