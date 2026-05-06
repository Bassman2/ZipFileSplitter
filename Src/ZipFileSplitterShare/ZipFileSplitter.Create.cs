using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Text;

namespace System.IO.Compression;

public static partial class ZipFileSplitter
{
    public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, long splitSize, ExtentionFormat extentionFormat)
        => CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, splitSize, extentionFormat, compressionLevel: CompressionLevel.Optimal, includeBaseDirectory: false, entryNameEncoding: null);

    public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, long splitSize, ExtentionFormat extentionFormat, CompressionLevel compressionLevel)
       => CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, splitSize, extentionFormat, compressionLevel, includeBaseDirectory: false, entryNameEncoding: null);

    public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, long splitSize, ExtentionFormat extentionFormat, CompressionLevel compressionLevel, bool includeBaseDirectory)
        => CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, splitSize, extentionFormat, compressionLevel, includeBaseDirectory, entryNameEncoding: null);

    public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, long splitSize, ExtentionFormat extentionFormat, CompressionLevel compressionLevel, bool includeBaseDirectory, System.Text.Encoding? entryNameEncoding)
    {
        sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);
        destinationArchiveFileName = Path.GetFullPath(destinationArchiveFileName);

        using ZipArchive archive = Open(destinationArchiveFileName, ZipArchiveMode.Create, splitSize, extentionFormat, entryNameEncoding);
               
        DirectoryInfo di = new(sourceDirectoryName);
        if (includeBaseDirectory && di.Parent != null)
        {
            di = di.Parent;
        }

        foreach (var fse in di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
        {
            if (fse is FileInfo file)
            {
                archive.CreateEntryFromFile(file.FullName, file.Name, compressionLevel);
            }
            else if (fse is DirectoryInfo dir)
            {
                archive.CreateEntry(dir.Name + "/");
            }
            else
            {
                throw new IOException($"Unsupported file for zipping. {fse.FullName}");
            }
        }

        // If no entries create an empty root directory entry:
        if (includeBaseDirectory)
        {
            archive.CreateEntry(di.Name + "/");
        }
    }
}
