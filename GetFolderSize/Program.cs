using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace GetFolderSize
{
    class Program
    {
        const string outputFile = "FolderSize.csv";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (args.Length > 0)
            {
                foreach (string targetDir in args)
                {
                    if (Directory.Exists(targetDir))
                    {
                        TargetInfo ti = new TargetInfo();
                        GetSubDirectoryInfo(new DirectoryInfo(targetDir), ti);

                        Console.WriteLine("=============");
                        Console.WriteLine(
                            "フォルダーサイズ : {0}\r\n" +
                            "ファイル数       : {1}\r\n" +
                            "フォルダー数     : {2}\r\n" +
                            "アクセス不可     : {3}", ti.TotalSize, ti.FileLength, ti.DirectoryLength, string.Join(";", ti.DenyList));
                        using (StreamWriter sw = new StreamWriter(outputFile, true, Encoding.GetEncoding("Shift_JIS")))
                        {
                            sw.WriteLine(string.Format(
                                "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                    Path.GetFileName(targetDir), 
                                    ti.TotalSize,
                                    ti.FileLength, 
                                    ti.DirectoryLength,
                                    string.Join(";", ti.DenyList)));
                        }
                    }
                }
            }
            Console.WriteLine("完了");
            Console.ReadLine();
        }

        private static void GetSubDirectoryInfo(DirectoryInfo dirInfo, TargetInfo ti)
        {
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                ti.TotalSize += fileInfo.Length;
                ti.FileLength++;
            }
            foreach (DirectoryInfo directoryInfo in dirInfo.GetDirectories())
            {
                try
                {
                    ti.DirectoryLength++;
                    GetSubDirectoryInfo(directoryInfo, ti);
                }
                catch
                {
                    ti.DenyList.Add(directoryInfo.FullName);
                    Console.WriteLine(directoryInfo.FullName);
                }
            }
        }
    }

    class TargetInfo
    {
        public long TotalSize { get; set; }
        public long FileLength { get; set; }
        public long DirectoryLength { get; set; }
        public List<string> DenyList { get; set; }
        public TargetInfo()
        {
            this.DenyList = new List<string>();
        }
    }
}
