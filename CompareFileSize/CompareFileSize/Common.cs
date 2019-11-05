using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompareFile
{
    class Common
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

        /// <summary>
        ///[alp]?[\d+]?405$|[alp]?[\d+]?1029$|^cs$|^csy$|[alp]?[\d+]?409$|[alp]?[\d+]?1033$|^en$|^enu$|[alp]?[\d+]?804$|[alp]?[\d+]?2052$|^zh-chs$|^chs$|[alp]?[\d+]?407$|[alp]?[\d+]?1031$|^de$|^deu$|[alp]?[\d+]?40c$|[alp]?[\d+]?1036$|^fr$|^fra$|[alp]?[\d+]?411$|[alp]?[\d+]?1041$|^ja$|^jpn$|[alp]?[\d+]?419$|[alp]?[\d+]?1049$|^ru$|^rus$|[alp]?[\d+]?404$|[alp]?[\d+]?1028$|^zh-tw$|^cht$|[alp]?[\d+]?410$|[alp]?[\d+]?1040$|^it$|^ita$|[alp]?[\d+]?413$|[alp]?[\d+]?1043$|^nl$|^nld$|[alp]?[\d+]?412$|[alp]?[\d+]?1042$|^ko$|^kor$|[alp]?[\d+]?415$|[alp]?[\d+]?1045$|^pl$|^plk$|[alp]?[\d+]?41f$|[alp]?[\d+]?1055$|^tr$|^trk$|[alp]?[\d+]?41d$|[alp]?[\d+]?1053$|^zh-cn$|^sve$|[alp]?[\d+]?40e$|[alp]?[\d+]?1038$|^hu$|^hun$|[alp]?[\d+]?816$|[alp]?[\d+]?2070$|^pt$|^ptg$|[alp]?[\d+]?c0a$|[alp]?[\d+]?3082$|^es$|^esn$|[alp]?[\d+]?416$|[alp]?[\d+]?1046$|^pt-br$|^ptb$|[alp]?[\d+]?40b$|[alp]?[\d+]?1035$|^fi$|^fin$|[alp]?[\d+]?408$|[alp]?[\d+]?1032$|^el$|^ell$|[alp]?[\d+]?406$|[alp]?[\d+]?1030$|^da$|^dan$|[alp]?[\d+]?414$|[alp]?[\d+]?1044$|^no$|^nor$
        /// </summary>
        public static string reg2Name = "";


        /// <summary>
        /// .*\\cs$|.*\\en$|.*\\zh-chs$|.*\\de$|.*\\fr$|.*\\ja$|.*\\ru$|.*\\zh-tw$|.*\\it$|.*\\nl$|.*\\ko$|.*\\pl$|.*\\tr$|.*\\zh-cn$|.*\\hu$|.*\\pt$|.*\\es$|.*\\pt-br$|.*\\fi$|.*\\el$|.*\\da$|.*\\no$
        /// </summary>
        public static string reg2Dir = "";

        static HashSet<string> set = new HashSet<string>();

        static List<string> filterList = new List<string> { @"c:\program files", @"c:\program files (x86)", @"c:\windows", @"c:\tools", @"c:\users", @"c:\sulphurclient", @"c:\sccmcontentlib", @"c:\package2", @"c:\novascripts", @"c:\inetpub", @"c:\windows.old" };

        public static List<string> fileListDir = new List<string>();
        public static List<string> fileListName = new List<string>();
        public static HashSet<string> GetRoots()
        {
            string pre = ".*";
            foreach (Array item in data)
            {
                reg2Name += "[ALP]?[\\d+]?" + item.GetValue(0) + "$|";
                reg2Name += "[ALP]?[\\d+]?" + item.GetValue(1) + "$|";
                reg2Name += "^" + item.GetValue(2) + "$|";
                reg2Name += "^" + item.GetValue(3) + "$|";

                reg2Dir += pre + @"\\" + item.GetValue(2) + "$|";
            }
            reg2Name = reg2Name.Substring(0, reg2Name.Length - 1).ToLower();
            reg2Dir = reg2Dir.Substring(0, reg2Dir.Length - 1).ToLower();
            GetFiles(@"C:\", true);
            return set;
        }

        /// <summary>
        /// 获取文件名列表
        /// </summary>
        /// <param name="path"></param>
        public static void GetRelatedFiles(string path)
        {
            //跳出条件
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

                bool enFlag = false;
                foreach (var dir in dirs)
                {
                    //英语相关的文件
                    if (Regex.IsMatch(dir.ToLower(), reg2Dir))
                    {
                        enFlag = true;
                        string[] ens = Directory.GetFiles(path);
                        foreach (var item in ens)
                        {
                            fileListDir.Add(item);
                        }
                        break;
                    }
                    continue;
                }
                if (!enFlag)
                {
                    foreach (string file in childs)
                    {
                        FileInfo f = new FileInfo(file);
                        //路径中包含
                        if (!file.EndsWith(".resx") && Regex.IsMatch(f.Directory.FullName.ToLower(), reg2Dir))
                        {
                            fileListDir.Add(file);
                            continue;
                        }
                        //文件名中包含
                        string[] a = f.Name.ToLower().Split('.');
                        foreach (var item in a)
                        {
                            //排除掉dll与xml
                            if (file.EndsWith(".dll") || file.EndsWith(".xml"))
                            {
                                break;
                            }
                            if (Regex.IsMatch(item, reg2Name))
                            {
                                fileListName.Add(file);
                                continue;
                            }
                        }
                    }
                }

                foreach (string dir in dirs)
                {
                    FileInfo d = new FileInfo(dir);
                    //排除隐藏文件
                    if ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (d.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        GetRelatedFiles(dir);
                    }
                }
            }

            catch (Exception ex)
            {
                return;
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
                                //Trace.WriteLine("file name: " + file);
                                flag = false;
                                filterList.Add(root);
                                break;
                            }
                        }

                        if (Regex.IsMatch(f.Directory.FullName.ToLower(), reg2Dir))
                        {
                            string[] arr = file.Split('\\');
                            set.Add(arr[0] + @"\" + arr[1]);
                            //Trace.WriteLine("dir name: " + file);
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

        /// <summary>
        /// 返回相关的所有文件名列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static List<string> FilesOfDir()
        {
            return fileListDir;
        }

        /// <summary>
        /// 返回相关的所有文件名列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static List<string> FilesOfName()
        {
            return fileListName;
        }

        /// <summary>
        /// 将文件列表存入dic对象
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ChangeToDic(List<string> files)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string path in files)
            {
                FileInfo info = new FileInfo(path);
                string name = "";
                int index = info.Name.LastIndexOf(".");
                if (index > 1)
                {
                    name = info.Name.Substring(0, info.Name.LastIndexOf("."));
                }
                else
                {
                    name = info.Name;
                }

                long length = info.Length;
                if (dic.ContainsKey(name))
                {
                    dic[name] = dic[name] + ";" + path + "|" + length;
                }
                else
                {
                    dic.Add(name, path + "|" + length);
                }
            }

            return dic;
        }
        /// <summary>
        /// 过滤掉完全相同的文件，只留其一
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<string, string> FilterDic(Dictionary<string, string> dic)
        {
            Dictionary<string, string> resultDic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> item in dic)
            {
                string[] arr = item.Value.Split(';');
                try
                {
                    //相同的文件名，只留下一个，其余全部删掉,使用set，存入时可以去重复
                    HashSet<int> indexs = new HashSet<int>();
                    for (int i = 0; i <= arr.Length - 2; i++)
                    {
                        string curName = new FileInfo(arr[i].Split('|')[0]).Name.ToLower();
                        long curLength = long.Parse(arr[i].Split('|')[1]);
                        for (int j = i + 1; j <= arr.Length - 1; j++)
                        {
                            if (new FileInfo(arr[j].Split('|')[0]).Name.ToLower().Equals(curName) && curLength == long.Parse(arr[j].Split('|')[1]))
                            {
                                indexs.Add(j);
                            }
                        }
                    }

                    string value = "";
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (!indexs.Contains(i))
                        {
                            value += arr[i] + ";";
                        }
                    }
                    value = value.Substring(0, value.Length - 1);
                    resultDic.Add(item.Key, value);

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message + ":" + item.Value);
                }

            }

            return resultDic;
        }

        /// <summary>
        /// 比较，名字完全相同的文件，比较其大小，相同的保存结果
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Compare(Dictionary<string, string> dic)
        {

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in dic)
            {
                List<string> fileName = new List<string>();
                List<long> size = new List<long>();
                string[] arr = kv.Value.Split(';');
                foreach (var item in arr)
                {
                    string[] s = item.Split('|');
                    fileName.Add(s[0]);
                    size.Add(long.Parse(s[1]));
                }

                for (int i = 0; i < size.Count; i++)
                {
                    string key = fileName[i].Substring(fileName[i].LastIndexOf("\\") + 1).Split('.')[0] + "(" + size[i] + ")";
                    if (!result.ContainsKey(key))
                    {
                        for (int j = i + 1; j < size.Count; j++)
                        {
                            if (size[i] == size[j])
                            {
                                if (!result.ContainsKey(key))
                                {
                                    result.Add(key, fileName[i] + "," + fileName[j]);
                                }
                                else
                                {
                                    result[key] = result[key] + "," + fileName[j];
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 比较两个文件内容是否完全相同，参数为两个文件路径
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool FileCompare(string file1, string file2)
        {
            if (file1 == file2) { return true; }
            int file1byte = 0;
            int file2byte = 0;
            try
            {
                using (FileStream fs2 = new FileStream(file2, FileMode.Open,FileAccess.Read))
                {
                    using (FileStream fs1 = new FileStream(file1, FileMode.Open,FileAccess.Read))
                    {
                        if (fs1.Length != fs2.Length)
                        {
                            return false;
                        }// 逐一比较两个文件的每一个字节，直到发现不相符或已到达文件尾端为止。
                        do
                        {// 从每一个文件读取一个字节。
                            file1byte = fs1.ReadByte();
                            file2byte = fs2.ReadByte();
                            if (file1byte != file2byte)
                            {
                                return false;
                            }
                        } while ((file1byte != -1));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("error occur " + ex.Message);
                return false;
            }

        }
    }
}
