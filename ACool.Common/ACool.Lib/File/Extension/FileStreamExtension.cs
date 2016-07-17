using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class FileExtension
    {
        public static FileStream ConvertToFileStream(this string filepath)
        {
            if (File.Exists(filepath))
            {
                return new FileStream(filepath, FileMode.Open);
            }

            return null;
        }

        public static byte [] ConvertToByteArray(this string filepath)
        {
            FileStream stream = ConvertToFileStream(filepath);

            if (stream != null)
            {
                return stream.ToByteArray((int)stream.Length);
            }

            return null;
        }

        public static Stream ToStream(this byte[] myByteArray)
        {
            MemoryStream stream = new MemoryStream(myByteArray);

            return stream;
        }

        public static byte[] ToByteArray(this Stream InputStream, int length)
        {
            BinaryReader br = new BinaryReader(InputStream);
            br.BaseStream.Seek(0, SeekOrigin.Begin);

            return br.ReadBytes(length);
        }
    }
}
