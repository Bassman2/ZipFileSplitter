using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;

namespace System.IO.Compression;

public static partial class ZipFileSplitter
{
    public static System.IO.Compression.ZipArchive OpenRead(string archiveFileName)
        => Open(archiveFileName, ZipArchiveMode.Read, 0, ExtentionFormat.SingleExtention, entryNameEncoding: null);
    public static System.IO.Compression.ZipArchive OpenRead(string archiveFileName, Encoding? entryNameEncoding)
        => Open(archiveFileName, ZipArchiveMode.Read, 0, ExtentionFormat.SingleExtention, entryNameEncoding: entryNameEncoding);

    public static System.IO.Compression.ZipArchive Open(string archiveFileName, ZipArchiveMode mode, long splitSize, ExtentionFormat extentionFormat = ExtentionFormat.MultiExtention)
        => Open(archiveFileName, mode, splitSize, extentionFormat, entryNameEncoding: null);

    public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode, long splitSize, ExtentionFormat extentionFormat = ExtentionFormat.MultiExtention, Encoding? entryNameEncoding = null)
    {
        // Relies on FileStream's ctor for checking of archiveFileName

        FileMode fileMode;
        FileAccess fileAccess;
        FileShare fileShare;

        switch (mode)
        {
        case ZipArchiveMode.Read:
            fileMode = FileMode.Open;
            fileAccess = FileAccess.Read;
            fileShare = FileShare.Read;
            break;

        case ZipArchiveMode.Create:
            fileMode = FileMode.CreateNew;
            fileAccess = FileAccess.Write;
            fileShare = FileShare.None;
            break;

        //case ZipArchiveMode.Update:
        //    fileMode = FileMode.OpenOrCreate;
        //    fileAccess = FileAccess.ReadWrite;
        //    fileShare = FileShare.None;
        //    break;

        default:
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        // Suppress CA2000: fs gets passed to the new ZipArchive, which stores it internally.
        // The stream will then be owned by the archive and be disposed when the archive is disposed.
        // If the ctor completes without throwing, we know fs has been successfully stores in the archive;
        // If the ctor throws, we need to close it here.

        FileSplitterStream fs = new(archiveFileName, splitSize, extentionFormat, fileMode, fileAccess, fileShare);

        try
        {
            return new ZipArchive(fs, mode, leaveOpen: false, entryNameEncoding: entryNameEncoding);
        }
        catch
        {
            fs.Dispose();
            throw;
        }
    }
}
