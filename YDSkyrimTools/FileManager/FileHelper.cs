using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YDSkyrimTools.RequestCore;
using static YDSkyrimTools.SkyrimModManager.FileHelper;

namespace YDSkyrimTools.SkyrimModManager
{
    public class FileHelper
    {
        public static string ReadFileByStr(string filepath, Encoding EnCoding)
        {
            try
            {
                StreamReader rd = new StreamReader(filepath, Encoding.UTF8);

                StringBuilder sb = new StringBuilder();
                while (!rd.EndOfStream)
                {
                    string dqstr = rd.ReadLine();
                    sb = sb.Append(dqstr + "\r\n");
                }

                rd.Close();
                rd.Dispose();
                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void WriteFile(string targetpath, string text, Encoding encodingtype)
        {
            try
            {
                StreamWriter FileWriter = new StreamWriter(targetpath, false, encodingtype);
                FileWriter.Write(text);
                FileWriter.Close();
                FileWriter.Dispose();
            }
            catch { }

        }
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加
                if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    aimPath += System.IO.Path.DirectorySeparatorChar;
                }
                // 判断目标目录是否存在如果不存在则新建
                if (!System.IO.Directory.Exists(aimPath))
                {
                    System.IO.Directory.CreateDirectory(aimPath);
                }
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                // string[] fileList = Directory.GetFiles（srcPath）；
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (System.IO.Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
                    }
                    // 否则直接Copy文件
                    else
                    {
                        System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static bool CompareFileContent(string filePath1, string filePath2)
        {
            if (File.Exists(filePath1) && File.Exists(filePath2))
            {
                //创建一个哈希算法对象
                try 
                { 

                using (HashAlgorithm hash = HashAlgorithm.Create())
                {
                    using (FileStream file1 = new FileStream(filePath1, FileMode.Open), file2 = new FileStream(filePath2, FileMode.Open))
                    {
                        byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组
                        byte[] hashByte2 = hash.ComputeHash(file2);
                        string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串
                        string str2 = BitConverter.ToString(hashByte2);
                        return (str1 == str2);//比较哈希码
                    }
                }

                }
                catch { }
            }

            return false;
        }

        public static STreeItem GetPathStruct(string FromPath)
        {
            if (!FromPath.EndsWith(@"\"))
            {
                FromPath += @"\";
            }

            if (Directory.Exists(FromPath))
            {
                OneEnum NOneEnum = new OneEnum();
                NOneEnum.STreeItems.MainPath = FromPath;
                NOneEnum.EnumAny(FromPath);
                return NOneEnum.STreeItems;
            }

            return null;
        }

        public class OneEnum
        {
            public STreeItem STreeItems = new STreeItem();

            public void EnumAny(string FromPath)
            {
                foreach (var GetChildFile in Directory.GetFiles(FromPath))
                {
                    STreeItems.Files.Add(new FileItem(GetChildFile));
                }
                foreach (var GetChildDir in Directory.GetDirectories(FromPath))
                {
                    EnumAny(GetChildDir);
                }
            }
        }

    }


    public enum SourceType
    { 
      Null=0, IsFile = 1,IsPath = 2
    }

    public class FileItem : IComparable<FileItem>
    {
        public string FilePath;

        public int CompareTo(FileItem I)
        {
            if (string.Compare(this.FilePath, I.FilePath, StringComparison.Ordinal) > 0) return 1;
            else if (string.Compare(this.FilePath, I.FilePath, StringComparison.Ordinal) < 0) return -1;
            else return 0;
        }

        public FileItem(string FilePath)
        {
            this.FilePath = FilePath;
        }
        public string GetFilePath()
        {
            return this.FilePath;
        }

        public string GetFilePath(string MainPath)
        {
            return (this.FilePath).Substring(MainPath.Length);
        }
    }
    public class STreeItem 
    {
        public string MainPath;
        public List<FileItem> Files = new List<FileItem>();
    }
}
