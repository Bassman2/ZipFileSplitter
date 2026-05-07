namespace ZipAnalyser;

public class Analyser
{
    private static ReadOnlySpan<byte> fileHeader => [(byte)'P', (byte)'K', 3, 4];
    
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
            var chunk = reader.ReadChunk();;

            if (chunk.SequenceEqual(fileHeader))
            {
                FileHeader(reader);
            }

        }
        return 0;
    }

    private void FileHeader(BinReader reader)
    {   
        var version = reader.ReadInt16BigEndian();
        var flags = reader.ReadInt16BigEndian();
        var compressionMethod = reader.ReadInt16BigEndian();
        var lastModTime = reader.ReadInt16BigEndian();
        var lastModDate = reader.ReadInt16BigEndian();
        var crc32 = reader.ReadInt32BigEndian();
        var compressedSize = reader.ReadInt32BigEndian();
        var uncompressedSize = reader.ReadInt32BigEndian();
        var fileNameLength = reader.ReadInt16BigEndian();
        var extraFieldLength = reader.ReadInt16BigEndian();
    
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

    }

}
