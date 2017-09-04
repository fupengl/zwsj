define([
'ko', 'jquery'
], function (ko, $) {

  return function () {
    if (location.href.indexOf('ZhiDiZhuZhai') > 0) {
      $('#ZhiDiZhuZhaiSearch').addClass('active');
      $('#ZhiDiZhuZhaiSearch').siblings().removeClass('active');
    }
    if (location.href.indexOf('ZhiDiShangYe') > 0) {
      $('#ZhiDiShangYeSearch').addClass('active');
      $('#ZhiDiShangYeSearch').siblings().removeClass('active');
    }
    if (location.href.indexOf('ZhiDiBanGong') > 0) {
      $('#ZhiDiBanGongSearch').addClass('active');
      $('#ZhiDiBanGongSearch').siblings().removeClass('active');
    }
    if (location.href.indexOf('ZhiDiGongYeYongDi') > 0) {
      $('#ZhiDiGongYeYongDiSearch').addClass('active');
      $('#ZhiDiGongYeYongDiSearch').siblings().removeClass('active');
    }

    this.showCity = function () {
      //window.alert(1)
      $('.page-fix-right').css('background', '#fbfbfb');
      $('#showCity').find('img').attr('src', 'Resource/assets/images/sidebar_login_position.png');
      $('#showCityContent').toggle();
    }
    this.hideCity = function () {
      $('.page-fix-right').css('background', '#fff');
      $('#showCity').find('img').attr('src', 'Resource/assets/images/sidebar_notLogin_position.png');
      $('#showCityContent').toggle();
    }
    this.showMember = function () {
      $('.page-fix-right').css('background', '#fbfbfb');
      $('#showMember').find('img').attr('src', 'Resource/assets/images/sidebar_login_member.png');
      $('#showMemberContent').css('display', 'block');
    }
    this.hideMember = function () {
      $('.page-fix-right').css('background', '#fff');
      $('#showMember').find('img').attr('src', 'Resource/assets/images/sidebar_notLogin_member.png');
      $('#showMemberContent').css('display', 'none');
    }

    this.city = function () {
      //window.location.href = 'Account/LoginWeb';
      console.log(1)
    }
    this.memberName = function () {
      //window.location.href = 'Account/LoginWeb';
      console.log(1)
    }
    this.memberData = function () {
      //window.location.href = 'Account/LoginWeb';
      console.log(1)
    }
    this.feedback = function () {
      //window.location.href = 'Account/LoginWeb';
      console.log(1)
    }
    this.institutionIntroduction = function () {
      //window.location.href = 'Account/LoginWeb';
      console.log(1)
    }

  }

});
