using System.Collections.Generic;
using PagedList;
using Service.DTOs;

namespace ZhiDiExt.Controllers
{
    public class JsonResponsePageList
    {
        public int TotalCount { get; set; }
        public IPagedList<object> Data { get; set; }
        public List<WeYeWebDto> TotalData { get; set; }

        public JsonResponsePageList(int totalCount, IPagedList<object> data, List<WeYeWebDto> totalData)
        {
            TotalCount = totalCount;
            Data = data;
            TotalData = totalData;
        }
    }
}