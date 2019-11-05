using Microsoft.Win32;
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
    /// <summary>
    /// 1.对比文件路径相同的文件，C:\SCCM_8634.1000_Main_RETAIL\cd.latest\SMSSETUP\BIN\X64\ja\SetupWPF_CDLatest.resources.dll（路径中包含\ja\）
    ///     如果文件大小相同,会比较其内容，如果也相同，会打印出来
    /// 2.对比文件中包含相关语言信息的文件,//C:\\aaa\\new File\\COMANAGEMENTSETTINGS.ZH-CN.RESX
    /// </summary>
    class Program
    {
        static List<string> files = new List<string>();

        //相关文件列表
        static List<string> relatedFiles = new List<string>();

        static void Main(string[] args)
        {
            string logPath = Environment.CurrentDirectory + "\\CompareResult.log";
            Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(logPath));
            Trace.AutoFlush = true;
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("COMPARE FILES");
            Trace.WriteLine("");

            //1，读取到相关文件目录的根
            HashSet<string> set = Common.GetRoots();

            //2.获取每个目录下的所有文件(相关)
            foreach (var path in set)
            {
                Common.fileListDir.Clear();
                Common.fileListName.Clear();
                Trace.WriteLine(path + "    " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                Common.GetRelatedFiles(path);
                //C:\\aaa\\new File\\COMANAGEMENTSETTINGS.ZH-CN.RESX
                List<string> fileListOfName = Common.FilesOfName();
                //C:\\aaa\\new File\\COMANAGEMENTSETTINGS.lan.RESX.zh-cn
                List<string> newFileListOfName = new List<string>();
                List<string> fileListOfDir = Common.FilesOfDir();

                for (int i = 0; i < fileListOfName.Count; i++)
                {
                    string[] arr = fileListOfName[i].Split('.');
                    for (int j = 0; j < arr.Length; j++)
                    {
                        Match ma = Regex.Match(arr[j].ToLower(), Common.reg2Name);
                        if (ma.Success)
                        {
                            arr[j] = Regex.Replace(arr[j].ToLower(), Common.reg2Name, "lan");
                            string newName = "";
                            foreach (var item in arr)
                            {
                                newName += "." + item;
                            }
                            newName = newName.Substring(1, newName.Length - 1);
                            newFileListOfName.Add(newName + "." + ma.Value);
                            break;
                        }

                    }
                }

                //在各个根路径下分开进行比较
                //文件列表转换为dic（两种：文件名包含语言信息；路径包含语言信息）
                //最终效果为（文件名）：
                /*
                    1. RESOURCES.lan.RESX,
                    C:\aaa\new File\RESOURCES.JA.RESX|57277;
                    C:\aaa\new File\new1\RESOURCES.NL.RESX|52850
                    2. COMANAGEMENTSETTINGS.lan.RESX, 
                    C:\aaa\new File\COMANAGEMENTSETTINGS.es.RESX|2353791;
                    C:\aaa\new File\COMANAGEMENTSETTINGS.ZH-CN.RESX|2353791;
                    C:\aaa\new File\new1\COMANAGEMENTSETTINGS.ZH-CN.RESX|2353791
                */
                Dictionary<string, string> NameDic = ChangeToDicOfName(newFileListOfName, fileListOfName);

                //转换效果为（路径）
                /*
                    新建文, 
                    C:\aaa\new File\new1\new2\new3\NEW\new\新建文.txt|9;——对应英文
                    C:\aaa\new File\new1\new2\new3\NEW\new\cs\新建文.txt|9;
                    C:\aaa\new File\new1\new2\new3\NEW\new\es\新建文.txt|9;
                    C:\aaa\new File\new1\new2\new3\NEW\new\zh-cn\新建文.txt|0 
                */
                Dictionary<string, string> DirDic = ChangeToDic(fileListOfDir);
                //3.对比
                Dictionary<string, string> CompareDirDic = CompareDir(DirDic);
                //过滤掉结果中本身就是同一个文件的部分
                //请注意：这里有exception,在循环中修改了集合
                //foreach (var item in CompareDirDic)
                //{
                //    bool b = true;
                //    string[] p = item.Value.Split(',');
                //    string[] p0 = p[0].Split('\\');
                //    string p0Tail = (p0[p0.Length - 2] + p0[p0.Length - 1]).ToLower();
                //    for (int i = 0; i < p.Length; i++)
                //    {
                //        string[] pi = p[i].Split('\\');
                //        string piTail = (pi[pi.Length - 2] + pi[pi.Length - 1]).ToLower();
                //        if (!p0Tail.Equals(piTail))
                //        {
                //            b = false;
                //            break;
                //        }
                //    }
                //    if (b)
                //    {
                //        CompareDirDic.Remove(item.Key);
                //    }
                //}
                Dictionary<string, string> CompareNameDic = CompareName(NameDic);

                //4.输出结果至文件.可使用trace输出日志
                //逐条打印对比结果至日志文件
                Trace.WriteLine("The Total results count of the path " + path + " is:" + (CompareDirDic.Count + CompareNameDic.Count));
                Trace.WriteLine("");

                foreach (KeyValuePair<string, string> result in CompareDirDic)
                {
                    Trace.WriteLine(result.Value.Split(',').Length + " Files named " + result.Key.Split('(')[0] + " are the same! ==>");

                    int i = result.Key.IndexOf('(');
                    int j = result.Key.IndexOf(')');
                    string s = result.Key.Substring(i + 1, j - i - 1);
                    string[] res = result.Value.Split(',');
                    foreach (var item in res)
                    {
                        Trace.WriteLine(item);
                    }
                    Trace.WriteLine("");
                }
                if (CompareNameDic.Count > 0 && CompareDirDic.Count > 0)
                {
                    Trace.WriteLine("------------------------------------------------------------------------------------------------------------------");
                }
                foreach (KeyValuePair<string, string> result in CompareNameDic)
                {
                    Trace.WriteLine(result.Value.Split(',').Length + " Files named " + result.Key.Split('(')[0] + " are the same! ==>");

                    int i = result.Key.IndexOf('(');
                    int j = result.Key.IndexOf(')');
                    string s = result.Key.Substring(i + 1, j - i - 1);

                    string[] res = result.Value.Split(',');
                    foreach (var item in res)
                    {
                        Trace.WriteLine(item);
                    }
                    Trace.WriteLine("");
                }
            }

            Process.Start("notepad.exe", logPath);
            Console.WriteLine("The result is saved in " + logPath);
            Console.WriteLine("Press any key to exit!");
        }

        private static Dictionary<string, string> ChangeToDicOfName(List<string> files, List<string> realFiles)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int i = 0; i < files.Count; i++)
            {
                string path = files[i];
                string realPath = realFiles[i];
                FileInfo info = new FileInfo(path);
                FileInfo realInfo = new FileInfo(realPath);
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

                long length = realInfo.Length;
                if (dic.ContainsKey(name))
                {
                    dic[name] = dic[name] + ";" + realPath + "|" + length;
                }
                else
                {
                    dic.Add(name, realPath + "|" + length);
                }
            }
            return dic;
        }

        private static Dictionary<string, string> CompareName(Dictionary<string, string> nameDic)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in nameDic)
            {
                List<string> fileName = new List<string>();
                List<long> size = new List<long>();
                List<string> pathList = new List<string>();
                string[] arr = kv.Value.Split(';');
                foreach (var item in arr)
                {
                    string[] s = item.Split('|');
                    fileName.Add(s[0]);
                    size.Add(long.Parse(s[1]));
                    pathList.Add(s[0]);
                }
                //防止同一个结果比较多次，造成结果冗余
                List<int> li = new List<int>();
                for (int i = 0; i < size.Count; i++)
                {
                    if (li.Contains(i))
                    {
                        continue;
                    }
                    string key = fileName[i].Substring(fileName[i].LastIndexOf("\\") + 1).Split('.')[0] + "(" + size[i] + ")";
                    if (!result.ContainsKey(key))
                    {
                        for (int j = i + 1; j < size.Count; j++)
                        {
                            if (li.Contains(j))
                            {
                                continue;
                            }

                            //同一个语言的文件，放在了不同的路径下，这里用的是文件名相同方式排除掉
                            if (new FileInfo(pathList[i]).Name.ToLower().Equals(new FileInfo(pathList[j]).Name.ToLower()))
                            {
                                continue;
                                li.Add(j);
                            }
                            if (size[i] == size[j] && Common.FileCompare(pathList[i], pathList[j]))
                            {
                                li.Add(j);
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

        /*
         private static Dictionary<string, string> CompareDir(Dictionary<string, string> dirDic)
         {
             Dictionary<string, string> result = new Dictionary<string, string>();

             foreach (KeyValuePair<string, string> kv in dirDic)
             {
                 List<string> fileName = new List<string>();
                 List<long> size = new List<long>();
                 List<string> pathList = new List<string>();
                 List<Match> matchs = new List<Match>();
                 string[] arr = kv.Value.Split(';');
                 foreach (var item in arr)
                 {
                     string[] s = item.Split('|');
                     fileName.Add(s[0]);
                     size.Add(long.Parse(s[1]));
                     pathList.Add(s[0]);
                     Match m = Regex.Match(new FileInfo(item).Directory.FullName.ToLower(), Common.reg2Dir);
                     matchs.Add(m);
                 }

                 for (int i = 0; i < size.Count; i++)
                 {
                     int index = fileName[i].LastIndexOf("\\");
                     string name = fileName[i].Substring(index + 1);
                     string key = name.Substring(0, name.LastIndexOf(".")) + "(" + size[i] + ")";
                     //string key = fileName[i].Substring(fileName[i].LastIndexOf("\\") + 1).Split('.')[0] + "(" + size[i] + ")";
                     if (!result.ContainsKey(key))
                     {
                         for (int j = i + 1; j < size.Count; j++)
                         {
                             //去掉本身就是同一个文件的情况，不用比较，依然用正则对比，如\CHS\
                             if (matchs[i].Success && matchs[j].Success && matchs[i].Value.Substring(matchs[i].Value.LastIndexOf("\\") + 1).Equals(matchs[i].Value.Substring(matchs[i].Value.LastIndexOf("\\") + 1)))
                             {
                                 continue;
                             }
                             if (size[i] == size[j] && Common.FileCompare(pathList[i], pathList[j]))
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
         */
        private static Dictionary<string, string> CompareDir(Dictionary<string, string> dirDic)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in dirDic)
            {
                List<string> fileName = new List<string>();
                List<long> size = new List<long>();
                List<string> pathList = new List<string>();
                //List<Match> matchs = new List<Match>();
                string[] arr = kv.Value.Split(';');
                foreach (var item in arr)
                {
                    string[] s = item.Split('|');
                    fileName.Add(s[0]);
                    size.Add(long.Parse(s[1]));
                    pathList.Add(s[0]);
                    //Match m = Regex.Match(item, Common.reg2Dir);
                    //matchs.Add(m);
                }

                List<int> li = new List<int>();
                for (int i = 0; i < size.Count; i++)
                {
                    if (li.Contains(i))
                    {
                        continue;
                    }
                    int index = fileName[i].LastIndexOf("\\");

                    string name = fileName[i].Substring(index + 1);
                    string key = name.Substring(0, name.LastIndexOf(".")) + "(" + size[i] + ")";
                    Match mi = Regex.Match(new FileInfo(pathList[i]).Directory.FullName.ToLower(), Common.reg2Dir);
                    if (!result.ContainsKey(key))
                    {
                        for (int j = i + 1; j < size.Count; j++)
                        {
                            if (li.Contains(j))
                            {
                                continue;
                            }
                            //去掉本身就是同一个文件的情况，不用比较，依然用正则对比，如\CHS\
                            if (size[i] == size[j])
                            {
                                Match mj = Regex.Match(new FileInfo(pathList[j]).Directory.FullName.ToLower(), Common.reg2Dir);
                                if (mi.Success && mj.Success && mi.Value.ToLower().Substring(mi.Value.LastIndexOf("\\") + 1).Equals(mj.Value.ToLower().Substring(mj.Value.LastIndexOf("\\") + 1)))
                                {
                                    li.Add(j);
                                    continue;
                                }
                                if (!mi.Success && !mj.Success)
                                {
                                    li.Add(j);
                                    continue;
                                }
                                if (Common.FileCompare(pathList[i], pathList[j]))
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
            }
            return result;
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
        /// 获取文件名列表
        /// </summary>
        /// <param name="path"></param>
        public static void GetAllChildFiles(string path)
        {
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
                foreach (string file in childs)
                {
                    files.Add(file);
                }
                foreach (string dir in dirs)
                {
                    GetAllChildFiles(dir);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("This file is not compared:" + path);
            }

        }

        /// <summary>
        /// 将文件列表存入dic对象
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ChangeToDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string path in files)
            //foreach (string path in relatedFiles)
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
        /// 将文件列表存入dic对象
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ChangeToDic(List<string> files)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string path in files)
            //foreach (string path in relatedFiles)
            {
                FileInfo info = new FileInfo(path);
                string name = "";
                int index = info.Name.LastIndexOf(".");
                if (index > 1)
                {
                    name = info.Name.Substring(0, info.Name.LastIndexOf("."));
                    if (!name.EndsWith("resources"))
                    {
                        name += ".resources";
                    }
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

        public static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);
            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        public static object GetRegistryValue(string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }
}
