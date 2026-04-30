using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression.FileSystem
{
    internal class FileSplitterStream : Stream
    {
        private FileStream fs;
        private string path;
        private int index = 1;
        private long overallLength = 0;
        private long splitLength = 0;
        private long splitSize = 0;

        public FileSplitterStream(string path, long splitSize, FileMode mode, FileAccess access, FileShare share)
        {
            this.path = path;
            this.splitSize = splitSize; 
            string filePath = $"{path}.{index:03}";
            fs = new FileStream(filePath, mode, access, share, bufferSize: 0x1000, useAsync: false);
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
                
                index++;
                string filePath = $"{path}.{index:03}";
                fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 0x1000, useAsync: false);
                splitLength = 0;

                fs.Write(buffer, (int)(offset + c), count);
                splitLength += count;
                overallLength += count;
            }
            
        }
    }
}
