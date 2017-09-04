define(['underscore'], function (_) {
  //住宅
  var zhuZhai = _.template('<div class="media mapMark-info">' +
  '<a class="media-left" href="/ZhiDiZhuZhaiDetai?id=<%= WuYeId %>">' +
      '<img class="media-object" src="<%= PhotoUrl %>" alt="媒体对象">' +
  '</a>' +
  '<div class="media-body">' +
      '<h4 class="media-heading"><%= WuYeMingCheng %></h4>' +
      '<p>地址：<%= ZuoLuo %></p>' +
      '<p><%= JiaGeQuJian %></p>' +
  '</div>' +
'</div>');

  //商业
  var shangYe = _.template('<div class="media mapMark-info">' +
  '<a class="media-left" href="/ZhiDiShangYeDetail?id=<%= WuYeId %>">' +
      '<img class="media-object" src="<%= PhotoUrl %>" alt="媒体对象">' +
  '</a>' +
  '<div class="media-body">' +
      '<h4 class="media-heading"><%= WuYeMingCheng %></h4>' +
      '<p>地址：<%= ZuoLuo %></p>' +
      '<p>租金：<%= ZuJinQujian %></p>' +
      '<p>售价：<%= ShouJiaQuJian %></p>' +
  '</div>' +
'</div>');

  //办公
  var banGong = _.template('<div class="media mapMark-info">' +
  '<a class="media-left" href="/ZhiDiShangYeDetail?id=<%= WuYeId %>">' +
      '<img class="media-object" src="<%= PhotoUrl %>" alt="媒体对象">' +
  '</a>' +
  '<div class="media-body">' +
      '<h4 class="media-heading"><%= WuYeMingCheng %></h4>' +
      '<p>地址：<%= ZuoLuo %></p>' +
      '<p>租金：<%= ZuJinQujian %></p>' +
  '</div>' +
'</div>');

  //办公
  var gongYeYongDi = _.template('<div class="media mapMark-info">' +
  '<a class="media-left" href="/ZhiDiShangYeDetail?id=<%= WuYeId %>">' +
      '<img class="media-object" src="<%= PhotoUrl %>" alt="媒体对象">' +
  '</a>' +
  '<div class="media-body">' +
      '<h4 class="media-heading"><%= WuYeMingCheng %></h4>' +
      '<p>地址：<%= ZuoLuo %></p>' +
  '</div>' +
'</div>');

  return {
    zhuZhai: zhuZhai,
    shangYe: shangYe,
    banGong: banGong,
    gongYeYongDi: gongYeYongDi
  }
});
