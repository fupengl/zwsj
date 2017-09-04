using System.Collections.Generic;
using PagedList;

namespace ZhiDiExt.Controllers
{
    public class JsonResponse<T>
    {
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }

        public JsonResponse(int totalCount, List<T> data)
        {
            TotalCount = totalCount;
            Data = data;
        } 
    }
}