using System.Buffers.Binary;

namespace ZipAnalyser;

public sealed class BinReader : IDisposable
{
    private readonly Stream stream;

    public BinReader(Stream stream)
    {
        this.stream = new BufferedStream(stream, 8 * 1024);
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public Stream BaseStream => stream;

    public bool IsEnd => stream.Position >= stream.Length;

    public Int16 ReadInt16BigEndian()
    {
        Span<byte> buffer = stackalloc byte[2];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    public Int32 ReadInt32BigEndian()
    {
        Span<byte> buffer = stackalloc byte[4];
        stream.ReadExactly(buffer);
        return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    public ReadOnlySpan<byte>  ReadChunk()
    {
        byte[] buffer = new byte[4];
        
        stream.ReadExactly(buffer);
        
        return buffer;
    }

    public string ReadString(int length)
    {
        byte[] buffer = new byte[length];
        stream.ReadExactly(buffer);
        return System.Text.Encoding.UTF8.GetString(buffer);
    }

    public long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }
}
