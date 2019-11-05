using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static List<Array> data = new List<Array>
        {
            //LCID(16),LCID(10),文件路径语言缩写,语言缩写
            new string[] { "405", "1029", "cs", "CSY" },
            new string[] { "409", "1033", "en", "ENU" },
            new string[] { "804", "2052", "zh-chs", "CHS" },
            new string[] { "407", "1031", "de", "DEU" },
            new string[] { "40C", "1036", "fr", "FRA" },
            new string[] { "411", "1041", "ja", "JPN" },
            new string[] { "419", "1049", "ru", "RUS" },
            new string[] { "404", "1028", "zh-tw", "CHT" },
            new string[] { "410", "1040", "it", "ITA" },
            new string[] { "413", "1043", "nl", "NLD" },
            new string[] { "412", "1042", "ko", "KOR" },
            new string[] { "415", "1045", "pl", "PLK" },
            new string[] { "41F", "1055", "tr", "TRK" },
            new string[] { "41D", "1053", "zh-cn", "SVE" },
            new string[] { "40E", "1038", "hu", "HUN" },
            new string[] { "816", "2070", "pt", "PTG" },
            new string[] { "C0A", "3082", "es", "ESN" },
            new string[] { "416", "1046", "pt-br", "PTB" },
            new string[] { "40B", "1035", "fi", "FIN" },
            new string[] { "408", "1032", "el", "ELL" },
            new string[] { "406", "1030", "da", "DAN" },
            new string[] { "414", "1044", "no", "NOR" }
        };
        
        static string reg2Name = "";
        static string reg2Dir = "";

        static HashSet<string> set = new HashSet<string>();

        static List<string> filterList = new List<string> { @"c:\program files", @"c:\program files (x86)", @"c:\windows", @"c:\tools", @"c:\users", @"c:\sulphurclient", @"c:\sccmcontentlib", @"c:\package2", @"c:\novascripts", @"c:\inetpub" };

        static void Main(string[] args)
        {
            string logPath = Environment.CurrentDirectory + "\\CompareResult.log";
            Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(logPath));
            Trace.AutoFlush = true;

            string pre = ".*";
            foreach (Array item in data)
            {
                foreach (var str in item)
                {
                    string regex = pre + str;
                    reg2Name += regex + "$|";
                }
                reg2Dir += pre + @"\\" + item.GetValue(2) + "$|";
            }
            reg2Name = reg2Name.Substring(0, reg2Name.Length - 1).ToLower();
            reg2Dir = reg2Dir.Substring(0, reg2Dir.Length - 1).ToLower();

            GetFiles(@"C:\", true);

            foreach (var item in set)
            {
                if (!filterList.Contains(item))
                {
                    Trace.WriteLine(item);
                }
            }

        }

        /// <summary>
        /// 获取文件名列表
        /// </summary>
        /// <param name="path"></param>
        public static void GetFiles(string path, bool flag)
        {
            //跳出条件
            string[] strs = path.Split('\\');
            string root = (strs[0] + @"\" + strs[1]).ToLower();
            if (filterList.Contains(root))
            {
                return;
            }

            if (!flag)
            {
                return;
            }
            if (!IsDir(path))
            {
                return;
            }

            string[] childs;
            string[] dirs;
            try
            {
                childs = Directory.GetFiles(path);
                dirs = Directory.GetDirectories(path);

                bool flag1 = path.Equals(@"C:\");
                if (!flag1)
                {
                    foreach (string file in childs)
                    {
                        if (!flag)
                        {
                            break;
                        }
                        FileInfo f = new FileInfo(file);
                        string[] a = f.Name.ToLower().Split('.');

                        foreach (var item in a)
                        {
                            if (Regex.IsMatch(item, reg2Name))
                            {
                                string[] arr = file.Split('\\');
                                set.Add(arr[0] + @"\" + arr[1]);
                                Trace.WriteLine("file name: " + file);
                                flag = false;
                                filterList.Add(root);
                                break;
                            }
                        }
                        
                        if (Regex.IsMatch(f.Directory.FullName.ToLower(), reg2Dir))
                        {
                            string[] arr = file.Split('\\');
                            set.Add(arr[0] + @"\" + arr[1]);
                            Trace.WriteLine("dir name: " + file);
                            flag = false;
                            filterList.Add(root);
                            break;
                        }                        
                    }
                }

                foreach (string dir in dirs)
                {
                    FileInfo d = new FileInfo(dir);
     
                    //排除隐藏文件
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (d.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        GetFiles(dir, flag);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        /// <summary>
        /// 判断目标是文件夹还是目录(目录包括磁盘)
        /// </summary>
        /// <param name="filepath">文件名</param>
        /// <returns></returns>
        public static bool IsDir(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            if ((fi.Attributes & FileAttributes.Directory) != 0)
                return true;
            else
            {
                return false;
            }
        }
    }
}
