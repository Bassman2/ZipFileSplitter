namespace ZipAnalyser;

public class Analyser
{
    private static ReadOnlySpan<byte> directoryHeader => [(byte)'P', (byte)'K', 1, 2];

    private static ReadOnlySpan<byte> fileHeader => [(byte)'P', (byte)'K', 3, 4];

    private static ReadOnlySpan<byte> endHeader => [(byte)'P', (byte)'K', 5, 6];

    public static int Analyse(FileInfo file)
    {
        return new Analyser().AnalyseInt(file);
    }

    private int AnalyseInt(FileInfo file)
    {
        using var stream = File.OpenRead(file.FullName);
        using var reader = new BinReader(stream);

        while (!reader.IsEnd)
        {
            var chunk = reader.ReadChunk();

            if (chunk.SequenceEqual(directoryHeader))
            {
                DirectoryHeader(reader);
            }
            else if (chunk.SequenceEqual(fileHeader))
            {
                FileHeader(reader);
            }
            else if (chunk.SequenceEqual(endHeader))
            {
                EndHeader(reader);
            }
            else
            {
            }
        }
        return 0;
    }

    private void FileHeader(BinReader reader)
    {
        var version = reader.ReadInt16LittleEndian();
        var flags = reader.ReadInt16LittleEndian();
        var compressionMethod = reader.ReadInt16LittleEndian();
        var lastModTime = reader.ReadInt16LittleEndian();
        var lastModDate = reader.ReadInt16LittleEndian();
        var crc32 = reader.ReadInt32LittleEndian();
        var compressedSize = reader.ReadInt32LittleEndian();
        var uncompressedSize = reader.ReadInt32LittleEndian();
        var fileNameLength = reader.ReadInt16LittleEndian();
        var extraFieldLength = reader.ReadInt16LittleEndian();

        var fileName = reader.ReadString(fileNameLength);


        Console.WriteLine($"File Header");
        Console.WriteLine($"    Version: {version}");
        Console.WriteLine($"    Flags: {flags}");
        Console.WriteLine($"    Compression Method: {compressionMethod}");
        Console.WriteLine($"    Last Mod Time: {lastModTime}");
        Console.WriteLine($"    Last Mod Date: {lastModDate}");
        Console.WriteLine($"    CRC-32: {crc32}");
        Console.WriteLine($"    Compressed Size: {compressedSize}");
        Console.WriteLine($"    Uncompressed Size: {uncompressedSize}");
        Console.WriteLine($"    File Name Length: {fileNameLength}");
        Console.WriteLine($"    Extra Field Length: {extraFieldLength}");
        Console.WriteLine($"    File Name: {fileName}");
        reader.Seek(extraFieldLength, SeekOrigin.Current);
        reader.Seek(compressedSize, SeekOrigin.Current);

    }

    private void DirectoryHeader(BinReader reader)
    {
        var version = reader.ReadByte();
        var madeBy = reader.ReadByte();
        var versionNeeded = reader.ReadInt16LittleEndian();
        var bitFlag = reader.ReadInt16LittleEndian();
        var compression = reader.ReadInt16LittleEndian();
        var time = reader.ReadInt16LittleEndian();
        var date = reader.ReadInt16LittleEndian();
        var crc32 = reader.ReadInt32LittleEndian();
        var compressedSize = reader.ReadInt32LittleEndian();
        var uncompressedSize = reader.ReadInt32LittleEndian();
        var fileNameLength = reader.ReadInt16LittleEndian();
        var extraFieldLength = reader.ReadInt16LittleEndian();
        var fileCommentLength = reader.ReadInt16LittleEndian();
        var diskNumberStart = reader.ReadInt16LittleEndian();
        var internalFileAttributes = reader.ReadInt16LittleEndian();
        var externalFileAttributes = reader.ReadInt32LittleEndian();
        var relativeOffsetOfLocalHeader = reader.ReadInt32LittleEndian();

        var fileName = reader.ReadString(fileNameLength);
        reader.Seek(extraFieldLength, SeekOrigin.Current);
        reader.Seek(fileCommentLength, SeekOrigin.Current);

        Console.WriteLine($"Directory Header");
        Console.WriteLine($"    Version: {version}");
        Console.WriteLine($"    Made By: {madeBy}");
        Console.WriteLine($"    Version Needed: {versionNeeded}");
        Console.WriteLine($"    Bit Flag: {bitFlag}");
        Console.WriteLine($"    Compression: {compression}");
        Console.WriteLine($"    Time: {time}");
        Console.WriteLine($"    Date: {date}");
        Console.WriteLine($"    CRC-32: {crc32}");
        Console.WriteLine($"    Compressed Size: {compressedSize}");
        Console.WriteLine($"    Uncompressed Size: {uncompressedSize}");
        Console.WriteLine($"    File Name Length: {fileNameLength}");
        Console.WriteLine($"    Extra Field Length: {extraFieldLength}");
        Console.WriteLine($"    File Comment Length: {fileCommentLength}");
        Console.WriteLine($"    Disk Number Start: {diskNumberStart}");
        Console.WriteLine($"    Internal File Attributes: {internalFileAttributes}");
        Console.WriteLine($"    External File Attributes: {externalFileAttributes}");
        Console.WriteLine($"    Relative Offset of Local Header: {relativeOffsetOfLocalHeader}");
        Console.WriteLine($"    File Name: {fileName}");
        Console.WriteLine($"    Extra Field Length: {extraFieldLength}");
        Console.WriteLine($"    File Comment Length: {fileCommentLength}");
    }

    private void EndHeader(BinReader reader)
    {
        var diskNumber = reader.ReadInt16LittleEndian();
        var diskWithCentralDirectory = reader.ReadInt16LittleEndian();
        var entriesOnDisk = reader.ReadInt16LittleEndian();
        var totalEntries = reader.ReadInt16LittleEndian();
        var centralDirectorySize = reader.ReadInt32LittleEndian();
        var centralDirectoryOffset = reader.ReadInt32LittleEndian();
        var commentLength = reader.ReadInt16LittleEndian();
        var comment = reader.ReadString(commentLength);

        Console.WriteLine($"End Header");
        Console.WriteLine($"    Disk Number: {diskNumber}");
        Console.WriteLine($"    Disk with Central Directory: {diskWithCentralDirectory}");
        Console.WriteLine($"    Entries on Disk: {entriesOnDisk}");
        Console.WriteLine($"    Total Entries: {totalEntries}");
        Console.WriteLine($"    Central Directory Size: {centralDirectorySize}");
        Console.WriteLine($"    Central Directory Offset: {centralDirectoryOffset}");
        Console.WriteLine($"    Comment Length: {commentLength}");
        Console.WriteLine($"    Comment: {comment}");
    }
}
