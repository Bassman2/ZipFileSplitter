using System.Text;
using System.IO.Compression;

namespace System.IO.Compression.FileSystem;

public static partial class ZipFileSplitter
{
    public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        => ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding: null, overwriteFiles: false);

    public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, bool overwriteFiles)
        => ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding: null, overwriteFiles: overwriteFiles);


    public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding)
        => ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, entryNameEncoding: entryNameEncoding, overwriteFiles: false);


    public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding, bool overwriteFiles)
    {
        ArgumentNullException.ThrowIfNull(sourceArchiveFileName);

        using ZipArchive archive = OpenRead(sourceArchiveFileName, entryNameEncoding);
        archive.ExtractToDirectory(destinationDirectoryName, overwriteFiles);
        
    }
}
