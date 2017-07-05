using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace FileProcessor
{
    public class ZipFileService
    {
        public async Task<FileItemCollection> ExtractToStreamCollection(Stream zipStream)
        {
            var fileItemCollection = new FileItemCollection();
            using (var reader = new StreamReader(zipStream))
            {
                var s = reader.ReadToEnd();
                var zstream = reader.BaseStream;
                string collectionName = "collection-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                using (ZipArchive zip = new ZipArchive(zstream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        using (var streamRdr = new StreamReader(entry.Open()))
                        {
                            var strm = streamRdr.BaseStream;
                            byte[] buffer = new byte[entry.Length];
                            int read;
                            var ms = new MemoryStream();
                            while ((read = strm.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }

                            fileItemCollection.Add(
                                new FileItem()
                                {
                                    Content = ms.ToArray(),
                                    FileLength = buffer.Length,
                                    FileName = entry.Name,
                                    LastWriteTime = entry.LastWriteTime.DateTime
                                });
                        }
                        collectionName = entry.FullName.Split('/')[0];
                    }
                    fileItemCollection.CollectionName = collectionName;
                }
            }
            return fileItemCollection;
        }
    }
}
