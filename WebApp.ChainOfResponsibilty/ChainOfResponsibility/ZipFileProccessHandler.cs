using System.IO.Compression;

namespace WebApp.ChainOfResponsibilty.ChainOfResponsibility
{
    public class ZipFileProccessHandler<T> : Processhandler
    {
        public override object handle(object o)
        {
            var excelMemoryStream = o as MemoryStream;

            excelMemoryStream.Position = 0; //memory stream'i yazdırmak istediğin zaman 0'dan başlamasını gerektiğini belirtmelisin

            using (var zipStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    var zipFile = archive.CreateEntry($"{typeof(T).Name}.xlsx");
                    using (var zipEntry = zipFile.Open())
                    {
                        excelMemoryStream.CopyTo(zipEntry);
                    }
                }
                return base.handle(zipStream);
            }
        }
    }
}
