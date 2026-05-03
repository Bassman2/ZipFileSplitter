namespace System.IO.Compression.FileSystem;

public class FileSplitterStream : Stream
{
    private readonly string archiveFilePath;
    private readonly long splitSize;
    private readonly ExtentionFormat extentionFormat;
    private readonly FileMode fileMode;
    private readonly FileAccess fileAccess;
    private readonly FileShare fileShare;

    private FileStream? fs;
    private int index = 0;
    private long overallLength = 0;
    private long splitLength = 0;

    public FileSplitterStream(string archiveFile, long size, ExtentionFormat format, FileMode mode, FileAccess access, FileShare share)
    {
        archiveFilePath = archiveFile;
        splitSize = size;
        extentionFormat = format;
        fileMode = mode;
        fileAccess = access;
        fileShare = share;

        if (mode == FileMode.Open)
        {
            if (archiveFilePath.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                extentionFormat = ExtentionFormat.SingleExtention;
            }
            else if (archiveFilePath.EndsWith(".zip.001", StringComparison.InvariantCultureIgnoreCase))
            {
                extentionFormat = ExtentionFormat.MultiExtention;
            }
            else
            {
                throw new ApplicationException($"The file name {archiveFilePath} does not have a valid extension for opening. It should end with either .zip or .zip.001");
            }
        }

        fs = mode switch
        {
            FileMode.Open => OpenFile(),
            FileMode.CreateNew => CreateFile(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };      
    }

    private string CreateFileName()
    {
        return extentionFormat switch
        {
            ExtentionFormat.SingleExtention when index == 0 => Path.ChangeExtension(archiveFilePath, ".zip"),
            ExtentionFormat.SingleExtention when index >= 1 => Path.ChangeExtension(archiveFilePath, $".z{index:D2}"),
            ExtentionFormat.MultiExtention => Path.ChangeExtension(archiveFilePath, $".zip.{index + 1:D3}"),
            _ => throw new InvalidOperationException()
        };
    }

    private FileStream? OpenFile()
    {
        string path = CreateFileName();
        if (!File.Exists(path))
        {
            return null;
        }
        return new FileStream(path, fileMode, fileAccess, fileShare, bufferSize: 0x1000, useAsync: false);
    }

    private FileStream CreateFile()
    {
        string path = CreateFileName();
        return new FileStream(path, fileMode, fileAccess, fileShare, bufferSize: 0x1000, useAsync: false);
    }

    public override bool CanRead => fileMode == FileMode.Open;

    public override bool CanSeek => false;

    public override bool CanWrite => fileMode == FileMode.CreateNew;

    public override long Length => overallLength;

    public override long Position
    {
        get => overallLength;
        set => throw new NotSupportedException();
    }

    public override void Flush() => fs?.Flush();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    
    public override void SetLength(long value) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (fs == null)
        {
            return 0;
        }

        int bytesRead = fs.Read(buffer, offset, count);

        if (bytesRead < count)
        {
            index++;
            fs.Close();
            fs = OpenFile();
            bytesRead += fs?.Read(buffer, offset + bytesRead, count - bytesRead) ?? 0;
        }

        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        long c = Math.Min(count, splitSize - splitLength);
        if (c > 0)
        {   
            fs!.Write(buffer, offset, count);
            splitLength += c;
            overallLength += c;
            count -= (int)c;
        }
        if (count > 0)
        {
            index++;
            fs!.Close();
            fs = CreateFile();
            splitLength = 0;

            fs.Write(buffer, (int)(offset + c), count);
            splitLength += count;
            overallLength += count;
        }
    }
}
