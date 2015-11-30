using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace LearnCSharp
{
    public class TmodIO
    {
        public  IList<string> fileNames = new List<string>();

        public  IDictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        public readonly string Name;

        public bool InvalidFile
        {
            get;
            internal set;
        }

        internal TmodIO(string name)
        {
            this.Name = name;
            this.InvalidFile = false;
        }

        internal bool HasFile(string fileName)
        {
            return this.files.ContainsKey(fileName);
        }

        internal void AddFile(string fileName, byte[] data)
        {
            if (!this.HasFile(fileName))
            {
                this.fileNames.Add(fileName);
            }
            byte[] array = new byte[data.Length];
            data.CopyTo(array, 0);
            this.files[fileName] = array;
        }

        internal void RemoveFile(string fileName)
        {
            if (this.HasFile(fileName))
            {
                this.fileNames.Remove(fileName);
                this.files.Remove(fileName);
            }
        }

        internal byte[] GetFile(string fileName)
        {
            if (this.HasFile(fileName))
            {
                return this.files[fileName];
            }
            return null;
        }

        internal void Save()
        {
            using (FileStream fileStream = File.Create(this.Name))
            {
                using (DeflateStream deflateStream = new DeflateStream(fileStream, CompressionMode.Compress))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(deflateStream))
                    {
                        binaryWriter.Write((byte)this.fileNames.Count);
                        foreach (string current in this.fileNames)
                        {
                            binaryWriter.Write(current);
                            byte[] array = this.files[current];
                            binaryWriter.Write(array.Length);
                            binaryWriter.Write(array);
                        }
                    }
                }
            }
        }

        internal void Read()
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(this.Name))
                {
                    using (DeflateStream deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(deflateStream))
                        {
                            int num = (int)binaryReader.ReadByte();
                            for (int i = 0; i < num; i++)
                            {
                                this.AddFile(binaryReader.ReadString(), binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            }
                        }
                        deflateStream.Dispose();
                    }
                }
            }
            catch
            {
                this.InvalidFile = true;
            }
        }

        internal bool ValidMod()
        {
            return !this.InvalidFile && this.HasFile("Resources") && ((this.HasFile("Windows") && this.HasFile("Other")) || this.HasFile("All"));
        }
    }
}
