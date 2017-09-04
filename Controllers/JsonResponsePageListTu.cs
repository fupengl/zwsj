using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using PagedList;
using Service.DTOs;

namespace ZhiDiExt.Controllers
{
    public class JsonResponsePageListTu
    {
        public int TotalCount { get; set; }
        public IPagedList<object> Data { get; set; }
        public List<WuYeDto> TotalData { get; set; }

        public JsonResponsePageListTu(int totalCount, IPagedList<object> data, List<WuYeDto> totalData)
        {
            TotalCount = totalCount;
            Data = data;
            TotalData = totalData;
        }
    }
}