using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileProcessor
{
    public class StreamCollection : Collection<Stream>, IDisposable
    {
        public void Dispose()
        {
            foreach (Stream stream in this)
            {
                stream.Dispose();
            }
        }
    }

    public class FileItemCollection : Collection<FileItem>
    {
        public string CollectionName { get; set; }
    }
}
