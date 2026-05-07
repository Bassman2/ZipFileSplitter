using System;
using System.Collections.Generic;
using System.Text;

namespace ZipFileSplitterTest;

public static class FileCompare
{
    public static bool FileEquals(string fileName1, string fileName2)
    {
        using var file1 = File.OpenRead(fileName1);
        using var file2 = File.OpenRead(fileName2);
        if (file1.Length != file2.Length)
        {
            return false;
        }
        return StreamsContentsAreEqual(file1, file2);
    }

    private static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * 4;
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            int count1 = stream1.Read(buffer1, 0, bufferSize);
            int count2 = stream2.Read(buffer2, 0, bufferSize);

            if (count1 != count2)
            {
                return false;
            }

            if (count1 == 0)
            {
                return true;
            }

            int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
            for (int i = 0; i < iterations; i++)
            {
                if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                {
                    return false;
                }
            }
        }
    }
}
