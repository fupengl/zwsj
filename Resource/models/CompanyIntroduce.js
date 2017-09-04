define([
  'ko',
  'zd',
  'commet'
], function (ko, app, request) {

  return function () {
    var self = this;

    var response = {
      'companName': 'xxxxxxxx公司',
      'contactNumber': '400-303394 2738979373',
      'contactEamil': '1129822555@qq.com',
      'contactAddress': 'xxxxx地址',
      'institutionIntroduction': '国际标准组织及国外先进标准组织机构介绍请您介绍一下评分标准可以'
    }

    self.companName = response.companName;
    self.contactNumber = response.contactNumber;
    self.contactEamil = response.contactEamil;
    self.contactAddress = response.contactAddress;
    self.institutionIntroduction = response.institutionIntroduction;

  };
});