using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSizeComp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //文件列表
        List<string> files = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {


            //1.通过注册表获取文件路径：HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\SMS\Setup

            string path;
            string reg = @"SOFTWARE\Microsoft\SMS\Setup";
            using (RegistryKey subKey = Registry.LocalMachine.OpenSubKey(reg))
            {
                if (subKey != null)
                {
                    path = (string)subKey.GetValue("Installation Directory");
                    MessageBox.Show(path);
                }
            }

            //2.获取文件路径下所有文件名以及其文件大小
            //将结果放入dic<string,string>(文件名，文件大小列表)中。数据格式如下：
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("FileName", "C:\\...\\FileName.FR:20K;C:\\...\\FileName.ES:25K;");
            GetFiles(@"C:\Users\v-xiqian\Desktop\new File");
            Dictionary<string, string> dic = ChangeToDic();

            //3.对比同名文件大小
            //对dic对象，逐条进行对比，判断是否有相同大小的文件
            Dictionary<string, string> CompareDic = Compare(dic);

            //4.输出结果至文件.可使用trace输出日志
            //逐条打印对比结果至日志文件
            //如：file a,b,c are of the same size
            foreach (KeyValuePair<string,string> result in CompareDic)
            {
                Console.WriteLine(result.Value.Split(',').Length + "Files " + result.Value + " are the same size!");
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
        /// 获取文件名列表
        /// </summary>
        /// <param name="path"></param>
        public void GetFiles(string path)
        {
            if (!IsDir(path))
            {
                return;
            }
            string[] childs = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in childs)
            {
                files.Add(file);
            }
            foreach (string dir in dirs)
            {
                GetFiles(dir);
            }
        }

        /// <summary>
        /// 将文件列表存入dic对象
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ChangeToDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string path in files)
            {
                FileInfo info = new FileInfo(path);
                string name = info.Name.Split('.')[0];
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

        public Dictionary<string, string> Compare(Dictionary<string, string> dic)
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
    }
}
