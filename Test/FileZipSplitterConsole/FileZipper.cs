using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression.FileSystem;

namespace FileZipSplitterConsole;

public class FileZipper
{

    public int Zip(string input, string output, long splitSize, ExtentionFormat extentionFormat)
    {

        return 0;
    }

    public int Unzip(string input, string output)
    {

        return 0;
    }

    public int Split(string input, string output, long splitSize, ExtentionFormat extentionFormat)
    {
        using var reader = File.OpenRead(input);
        using var writer = new FileSplitterStream(output, splitSize, FileMode.Create, FileAccess.Write, FileShare.None);
        reader.CopyTo(writer);
        return 0;
    }

    public int Merge(string input, string output)
    {

        return 0;
    }
}
