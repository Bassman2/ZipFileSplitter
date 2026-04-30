using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression.FileSystem;

public static class ZipFileSplitter
{

    //public static ZipArchive OpenRead(string archiveFileName, long splitSize) => Open(archiveFileName, splitSize, ZipArchiveMode.Read);

    //public static ZipArchive Open(string archiveFileName, long splitSize, ZipArchiveMode mode) => Open(archiveFileName, splitSize, mode, entryNameEncoding: null);

    public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode = ZipArchiveMode.Read, long splitSize = 0, Encoding? entryNameEncoding = null)
    {
        // Relies on FileStream's ctor for checking of archiveFileName

        FileMode fileMode;
        FileAccess access;
        FileShare fileShare;

        switch (mode)
        {
        case ZipArchiveMode.Read:
            fileMode = FileMode.Open;
            access = FileAccess.Read;
            fileShare = FileShare.Read;
            break;

        case ZipArchiveMode.Create:
            fileMode = FileMode.CreateNew;
            access = FileAccess.Write;
            fileShare = FileShare.None;
            break;

        case ZipArchiveMode.Update:
            fileMode = FileMode.OpenOrCreate;
            access = FileAccess.ReadWrite;
            fileShare = FileShare.None;
            break;

        default:
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        // Suppress CA2000: fs gets passed to the new ZipArchive, which stores it internally.
        // The stream will then be owned by the archive and be disposed when the archive is disposed.
        // If the ctor completes without throwing, we know fs has been successfully stores in the archive;
        // If the ctor throws, we need to close it here.

        FileSplitterStream fs = new(archiveFileName, splitSize, fileMode, access, fileShare);

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
