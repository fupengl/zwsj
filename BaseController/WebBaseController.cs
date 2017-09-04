using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ZhiDiExt.BaseController
{
    public abstract class WebBaseController : Controller
    {
        protected string EncodeHtml(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }
    }
}
