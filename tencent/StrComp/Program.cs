using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StrComp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"C:\Users\v-xiqian\documents\visual studio 2015\Projects\tencent\WindowsFormsApplication1\abc.xml";
            //string filePath = @"C:\Users\v-xiqian\Desktop\Data\AdminUI.UIResources.dll.lcl";
            Dictionary<string, string> dic = loadFileNew(@"C:\Users\v-xiqian\Desktop\Data\AdminUI.UIResources.dll.lcl");

            string logPath = Environment.CurrentDirectory + "\\Result.txt";
            Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(logPath));
            Trace.AutoFlush = true;
            Trace.WriteLine(@"C:\Users\v-xiqian\Desktop\Data\AdminUI.UIResources.dll.lcl");

            Dictionary<string, string> afterDic = filterResult(dic);

            Trace.WriteLine("The result count is " + afterDic.Count);
            Trace.WriteLine("");
            foreach (KeyValuePair<string, string> item in afterDic)
            {
                string[] keys = item.Key.Split('|');
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        Trace.WriteLine(keys[i]);
                    }
                }
                string[] arr = item.Value.Split('|');
                foreach (var res in arr)
                {
                    Trace.WriteLine(res);
                }
                Trace.WriteLine("");
            }

            //findStr(filePath);
        }


        public static Dictionary<string, string> filterResult(Dictionary<string, string> dic)
        {
            Dictionary<string, string> afterDic = new Dictionary<string, string>();
            //排除结果中完全一样的内容
            foreach (KeyValuePair<string, string> item in dic)
            {
                string trueResult = "";

                //item:Adress(Address;Address;Address)|Alla objekt(All Items;All Items)|
                string[] arr = item.Value.Split('|');
                //s:Adress(Address;Address;Address)
                foreach (string s in arr)
                {
                    bool flag = true;
                    string s1 = s.Split('(')[1];
                    //s2:Address;Address;Address
                    string s2 = s1.Substring(0, s1.Length - 1);
                    string[] arr2 = s2.Split(';');
                    foreach (string s3 in arr2)
                    {
                        if (!s3.ToLower().Equals(arr2[0].ToLower()))
                        {
                            flag = false;
                            break;
                        }
                    }

                    //排除字符串仅仅是单词顺序不同的情况
                    bool flag2 = true;
                    foreach (string s3 in arr2)
                    {
                        if (!flag2)
                        {
                            break;
                        }
                        string[] array = s3.Split(' ');
                        foreach (string s4 in arr2)
                        {
                            if (!flag2)
                            {
                                break;
                            }
                            foreach (var str in array)
                            {
                                if (!s4.ToLower().Contains(str.ToLower()))
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!flag && !flag2)
                    {
                        trueResult += s + "|";
                    }
                }
                if (trueResult != "")
                {
                    afterDic.Add(item.Key, trueResult.Substring(0, trueResult.Length - 1));
                }
            }


            /*
             后续处理相同结果合并后，出现如下结果：
                ;Microsoft.ConfigurationManagement.AdminConsole.UIResources.SMS_G_System_SERVICE.resources
                ;Microsoft.ConfigurationManagement.AdminConsole.UIResources.SMS_G_System_SYSTEM_DRIVER.resources
                ;Microsoft.ConfigurationManagement.AdminConsole.UIResources.SMS_G_System_SERVICE.resources
                Beskrivning(Caption;Description)
                Status(State;Status)
             其中Beskrivning(Caption;Description)有其它单独的结果，将类似结果合并
             */

            Dictionary<string, string> tempDic2 = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in afterDic)
            {
                string[] arr = pair.Value.Split('|');
                for (int i = 0; i < arr.Length; i++)
                {
                    tempDic2.Add(pair.Key + Convert.ToChar('a' + i), arr[i]);
                }
            }

            //相同结果合并
            Dictionary<string, string> tempDic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> pair in tempDic2)
            {
                if (!tempDic.ContainsKey(pair.Value))
                {
                    tempDic.Add(pair.Value, pair.Key.Substring(0, pair.Key.Length - 1));
                }
                else
                {
                    tempDic[pair.Value] += ("|" + pair.Key.Substring(0, pair.Key.Length - 1));
                }
            }
            afterDic.Clear();
            foreach (KeyValuePair<string, string> pair in tempDic)
            {
                if (afterDic.ContainsKey(pair.Value))
                {
                    afterDic[pair.Value] += "|" + pair.Key;
                }
                else
                {
                    afterDic.Add(pair.Value, pair.Key);
                }
            }

            return afterDic;
        }
        public static void findStr(string file)
        {

            //XMLData.applyXPath(mapXML,"//mapData/busline[annotName='"+willDeleteName+"']")
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            XmlNamespaceManager xmlnsm = new XmlNamespaceManager(xmldoc.NameTable);
            xmlnsm.AddNamespace("docNS", "http://schemas.microsoft.com/locstudio/2006/6/lcx");


            string test = "//docNS:Item[@ItemId=';Strings']/docNS:Item/docNS:Str/docNS:Tgt/docNS:Val";
            //string test = "//Item[@ItemId=';Strings']/Item/Str/Tgt/Val";
            XmlNodeList list = xmldoc.SelectNodes(test, xmlnsm);
            XmlNodeList list2 = list[0].SelectNodes("/Item/Item");

        }

        /// <summary>
        /// 获取file内容中的字符串列名与列值
        /// </summary>
        /// <param name="file">文件全路径</param>
        /// <returns></returns>
        public static Dictionary<string, string> loadFileNew(string file)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> PreTrans = new Dictionary<string, string>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);
            //第一层节点
            XmlNodeList Level1Nodes = xmldoc.DocumentElement.ChildNodes;
            foreach (XmlElement Level1Element in Level1Nodes)
            {
                //第二层节点
                if (!Level1Element.HasChildNodes)
                {
                    continue;
                }
                XmlNodeList Level2Nodes = Level1Element.ChildNodes;
                foreach (XmlElement Level2Element in Level2Nodes)
                {
                    if (!Level2Element.HasChildNodes)
                    {
                        continue;
                    }
                    //字符串节点
                    if (Level2Element.GetAttribute("ItemId").Equals(";Strings"))
                    {
                        HashSet<string> set = new HashSet<string>();
                        //因为第一次读取到字符串，并不知道后面还有没有与之相同的，故每次无法记录该字符串，需在遍历结束后再次
                        Dictionary<string, string> tempDic = new Dictionary<string, string>();

                        //所有字符串element
                        XmlNodeList WhatYouWantNodes = Level2Element.ChildNodes;
                        foreach (XmlElement WhatYouWantEle in WhatYouWantNodes)
                        {
                            if (!WhatYouWantEle.HasChildNodes)
                            {
                                continue;
                            }
                            XmlNodeList TheLastNodes = WhatYouWantEle.ChildNodes;
                            foreach (XmlElement TheLastEle in TheLastNodes)
                            {
                                if (TheLastEle.HasChildNodes)
                                {
                                    string key = TheLastEle.FirstChild.InnerText;
                                    string value = TheLastEle.LastChild.InnerText;
                                    string resultKey = Level1Element.GetAttribute("ItemId") + "|" + file;
                                    if (value == "")
                                    {
                                        continue;
                                    }
                                    if (!tempDic.ContainsKey(value))
                                    {
                                        tempDic.Add(value, key);
                                    }
                                    else
                                    {
                                        tempDic.Add(value + " ", key);
                                    }

                                    if (set.Contains(value))
                                    {
                                        if (!PreTrans.ContainsKey(resultKey))
                                        {
                                            PreTrans.Add(resultKey, "");
                                        }
                                        if (!tempDic.ContainsKey(value + "  "))
                                        {
                                            PreTrans[resultKey] += (";" + value + ":" + tempDic[value]);
                                            tempDic.Add(value + "  ", key);
                                        }
                                        if (tempDic.ContainsKey(value + " "))
                                        {
                                            PreTrans[resultKey] += (";" + value + ":" + tempDic[value + " "]);
                                            tempDic.Remove(value + " ");
                                        }


                                        if (result.ContainsKey(resultKey))
                                        {
                                            if (!result[resultKey].Split(';').Contains(value))
                                            {
                                                result[resultKey] += (";" + value);
                                            }
                                            continue;
                                        }
                                        result.Add(resultKey, value);
                                    }
                                    if (value != "")
                                    {
                                        set.Add(value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, string> pair in PreTrans)
            {
                if (!result.ContainsKey(pair.Key))
                {
                    continue;
                }
                string[] resultArr = result[pair.Key].Split(';');
                string[] preTransArr = pair.Value.Split(';');

                //Adress, Address:Address
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (string preTransStr in preTransArr)
                {
                    string[] arr = preTransStr.Split(':');
                    if (arr.Length < 2)
                    {
                        continue;
                    }
                    if (dic.ContainsKey(arr[0]))
                    {
                        dic[arr[0]] += ";" + arr[1];
                        continue;
                    }
                    dic.Add(arr[0], arr[1]);
                }
                for (int i = 0; i < resultArr.Length; i++)
                {
                    if (!dic.ContainsKey(resultArr[i]))
                    {
                        continue;
                    }
                    resultArr[i] = dic[resultArr[i]];
                }
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> kv in dic)
                {
                    sb.Append(kv.Key);
                    sb.Append("(");
                    sb.Append(kv.Value);
                    sb.Append(")|");
                }
                result[pair.Key] = sb.ToString().Substring(0, sb.Length - 1);
            }
            return result;
        }

        public static void LoadFile()
        {
            Dictionary<string, string> strDic = new Dictionary<string, string>();
            //C:\Users\v-xiqian\Desktop\Data\AdminUI.UIResources.dll.lcl
            //服务器路径为：
            //\\reddog\Builds\branches\git_configmgr_main_release\5.0.8853.1009\binaries.amd64retail\LocInfo\LCL\sv
            string orgCodePath = @"C:\Users\v-xiqian\Desktop\Data\AdminUI.UIResources.dll.lcl";
            //string orgCodePath = @"\\reddog\Builds\branches\git_configmgr_main_release\5.0.8853.1009\binaries.amd64retail\LocInfo\LCL\sv\AdminUI.UIResources.dll.lcl";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(orgCodePath);
            //获取节点列表 
            XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
            XmlNodeList ele = xmldoc.GetElementsByTagName("Item");

            foreach (XmlElement element in ele)
            {
                string id = element.GetAttribute("ItemId");
                string leaf = element.GetAttribute("Leaf");
                if (leaf != "" && leaf != null)
                {
                    bool flag = Convert.ToBoolean(leaf);
                    if (flag)
                    {
                        XmlNode e = element;
                    }
                }

                if (true)
                {

                }
            }
        }
    }
}
