using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessor
{
    public class FileItem
    {
        public int FileLength { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }

        public DateTime LastWriteTime { get; set; }
    }
}
