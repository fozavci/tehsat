using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Tehsat
{
    public static class FileUtil
    {
        public async static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
        => await js.InvokeAsync<object>(
        "saveAsFile",
        filename,
        Convert.ToBase64String(data));
        public static byte[] CreateZip(String fileName, Dictionary<String, String> files)
        {
            // create the archive directory
            String archivePath = Path.Combine(Path.GetTempPath(), fileName);
            Directory.CreateDirectory(archivePath);

            // archive file path
            String archiveFile = Path.Combine(Path.GetTempPath(),  fileName + ".zip");

            // write the files to the folder
            foreach (var f in files)
            {
                //File.Create(Path.Combine(archivePath, f.Key));
                File.WriteAllBytes(Path.Combine(archivePath, f.Key), Encoding.UTF8.GetBytes(f.Value));
            }

            // create the archive
            ZipFile.CreateFromDirectory(archivePath, archiveFile);

            // load the archive
            byte[] archive = File.ReadAllBytes(archiveFile);

            // delete the files & the folder
            foreach (var f in files)
            {
                File.Delete(Path.Combine(archivePath, f.Key));
            }
            Directory.Delete(archivePath);
            File.Delete(archiveFile);

            return archive;
        }
    }
       
}

