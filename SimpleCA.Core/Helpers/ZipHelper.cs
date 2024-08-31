using System.IO.Compression;

namespace SimpleCA.Core.Helpers
{
    public static class ZipHelper
    {
        public static byte[] ZipFiles(Dictionary<string, byte[]> files)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var f in files)
                    {
                        zip.CompressFile(f.Value, f.Key);
                    }
                }

                return ms.ToArray();
            }
        }

        private static void CompressFile(this ZipArchive zip, byte[] file, string name)
        {
            var entry = zip.CreateEntry(name);
            using (var zipStream = entry.Open())
            {
                zipStream.Write(file, 0, file.Length);
            }
        }
    }
}
