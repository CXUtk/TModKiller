using System;
using System.IO;
using System.Windows.Forms;


namespace LearnCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            string path = "";
            OpenFileDialog selfiles = new OpenFileDialog();
            selfiles.FileName = "Open Tmodloader File";
            selfiles.Filter = "tmodloader Files (*.tmod)|*.tmod";
            DialogResult da = selfiles.ShowDialog();
            if (da == DialogResult.OK)
            {
                path = selfiles.FileName;
            }
            else
            {
                Console.WriteLine("No file selected!");
                return;
            }
            try
            {
                string str;
                string fileName;
                //string path = Directory.GetCurrentDirectory() + "\\ExampleMod.tmod"; Console.WriteLine(path);
                TmodIO tmod = new TmodIO(path);
                tmod.Read();
                if (tmod.InvalidFile)
                {
                    throw new FileLoadException();
                }
                if (!tmod.HasFile("All"))
                {
                    byte[] buffer = tmod.GetFile("Windows");
                    byte[] buffer2 = tmod.GetFile("Other");
                    byte[] buffer3 = tmod.GetFile("Resources");


                    using (MemoryStream mem = new MemoryStream(buffer3))
                    {
                        using (BinaryReader br = new BinaryReader(mem))
                        {
                            mem.Seek(br.ReadInt32(), SeekOrigin.Current);
                            str = br.ReadString();
                            #region DEbug！
                            fileName = string.Format("\\{0}.dll", str);
                            File.WriteAllBytes(Directory.GetCurrentDirectory() + fileName, buffer);
                            Console.WriteLine(string.Format("Extracting \\{0}.dll", str));
                            fileName = string.Format("\\{0}.dll", str + "2");
                            File.WriteAllBytes(Directory.GetCurrentDirectory() + fileName, buffer2);
                            Console.WriteLine(string.Format("Extracting \\{0}.dll", str + "2"));
                            //fileName =string.Format("{0}",str);
                            #endregion
                            DirectoryInfo dir = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + str);
                            for (string str2 = br.ReadString(); str2 != "end"; str2 = br.ReadString())
                            {
                                byte[] buf = br.ReadBytes(br.ReadInt32());
                                if (!Directory.Exists(str2))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(str2));

                                }

                                using (FileStream fl = File.Create(Directory.GetCurrentDirectory() + "\\" + str2))
                                {
                                    using (MemoryStream stream2 = new MemoryStream(buf))
                                    {
                                        Console.WriteLine(string.Format("Extracting \\{0}", Directory.GetCurrentDirectory() + "\\" + str2));
                                        fl.Write(stream2.ToArray(), 0, stream2.ToArray().Length);
                                    }

                                }

                            }
                            //mem.Dispose();
                        }

                    }


                }
                else
                {
                    throw new FileLoadException();
                }
                Console.WriteLine("Done!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }

        }

    }
}