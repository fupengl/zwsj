using System;

namespace ZhiDiExt.Models
{
    public class DocumentUploadModel
    {
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AreaPath { get; set; }
    }
}