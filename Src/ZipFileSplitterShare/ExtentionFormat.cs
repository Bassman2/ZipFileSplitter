namespace System.IO.Compression.FileSystem;

public enum ExtentionFormat
{
    /// <summary>
    /// .zip, .z01, .z02, ...
    /// </summary>
    SingleExtention,

    /// <summary>
    /// .zip.001, .zip.002, ...
    /// </summary>
    MultiExtention,
}
