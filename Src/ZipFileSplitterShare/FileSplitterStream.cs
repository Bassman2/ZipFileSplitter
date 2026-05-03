namespace System.IO.Compression.FileSystem;

public class FileSplitterStream : Stream
{
    private readonly string filePath;
    private readonly long splitSize;
    private readonly FileMode fileMode;
    private readonly FileAccess fileAccess;
    private readonly FileShare fileShare;

    private FileStream fs;
    private int index = 0;
    private long overallLength = 0;
    private long splitLength = 0;

    public FileSplitterStream(string path, long size, ExtentionFormat extentionFormat, FileMode mode, FileAccess access, FileShare share)
    {
        filePath = path;
        splitSize = size;
        fileMode = mode;
        fileAccess = access;
        fileShare = share;

        fs = CreateFile();
    }

    private FileStream CreateFile()
    {
        string path = $"{filePath}.{++index:D3}";
        return new FileStream(path, fileMode, fileAccess, fileShare, bufferSize: 0x1000, useAsync: false);
    }

    public override bool CanRead => fs.CanRead;

    public override bool CanSeek => false;

    public override bool CanWrite => fs.CanWrite;

    public override long Length => overallLength;

    public override long Position
    {
        get => overallLength;
        set => throw new NotSupportedException();
    }

    public override void Flush() => fs.Flush();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    
    public override void SetLength(long value) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        long c = Math.Min(count, splitSize - splitLength);
        if (c > 0)
        {   
            fs.Write(buffer, offset, count);
            splitLength += c;
            overallLength += c;
            count -= (int)c;
        }
        if (count > 0)
        {
            fs.Close();
            fs = CreateFile();
            splitLength = 0;

            fs.Write(buffer, (int)(offset + c), count);
            splitLength += count;
            overallLength += count;
        }
    }
}
