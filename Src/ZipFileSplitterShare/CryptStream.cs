using System.Security.Cryptography;
using System.Text;

namespace System.IO.Compression;

public class CryptStream : Stream
{
    private readonly Stream stream;
    private readonly CryptMode mode;
    private readonly Aes aes;
    private readonly ICryptoTransform cryptor;
    private readonly CryptoStream cryptoStream;

    public CryptStream(Stream stream, CryptMode mode, string password)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(password);

        this.stream = stream;
        this.mode = mode;

        aes = Aes.Create();
        DeriveKeyAndIV(password, aes);

        if (mode == CryptMode.Encrypt)
        {
            cryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            cryptoStream = new CryptoStream(stream, cryptor, CryptoStreamMode.Write);
        }
        else if (mode == CryptMode.Decrypt)
        {
            cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            cryptoStream = new CryptoStream(stream, cryptor, CryptoStreamMode.Read);
        }
        else
        {
            throw new ArgumentException("Invalid crypt mode.");
        }
    }

    public CryptStream(Stream stream, CryptMode mode, string key, string iv)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);
        if (key.Length != 32)
        {
            throw new ArgumentException("Key must be 32 bytes long.");
        }
        if (iv.Length != 16)
        {
            throw new ArgumentException("IV must be 16 bytes long.");
        }


        this.stream = stream;
        this.mode = mode;

        aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        if (mode == CryptMode.Encrypt)
        {
            cryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            cryptoStream = new CryptoStream(stream, cryptor, CryptoStreamMode.Write);
        }
        else if (mode == CryptMode.Decrypt)
        {

            // decrypt
            cryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            cryptoStream = new CryptoStream(stream, cryptor, CryptoStreamMode.Read);
        }
        else
        {
            throw new ArgumentException("Invalid crypt mode.");
        }
    }

    private static void DeriveKeyAndIV(string password, Aes aes)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = SHA256.HashData(passwordBytes);

        // Use first 32 bytes (256 bits) for the key
        aes.Key = hash;

        // Derive IV by hashing the key
        byte[] ivHash = SHA256.HashData(hash);
        aes.IV = ivHash[..16]; // Use first 16 bytes (128 bits) for IV
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            cryptoStream.Dispose();
            cryptor.Dispose();
            aes.Dispose();
            stream.Dispose();
        }
        base.Dispose(disposing);
    }

    public override bool CanRead => mode == CryptMode.Encrypt;

    public override bool CanWrite => mode == CryptMode.Decrypt;


    public override bool CanSeek => false;


    public override long Length => cryptoStream.Length;

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
        => throw new NotImplementedException();

    public override void Flush()
        => cryptoStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        return cryptoStream.Read(buffer, offset, count);
    }
    
    public override void Write(byte[] buffer, int offset, int count)
    {
        cryptoStream.Write(buffer, offset, count);
    }
}
