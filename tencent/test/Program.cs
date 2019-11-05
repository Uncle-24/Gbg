using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlProcessTest
{
    public class Program
    {
        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="xmlFilePath">XML文件路径</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDoc(string xmlFilePath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            return xmlDoc;
        }
        /// <summary>
        /// 根据指定的XPath表达式获取XML结点列表
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="xpathExpr"></param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodes(XmlDocument xmlDoc, string xpathExpr)
        {
            if (xmlDoc == null)
                return null;
            return xmlDoc.SelectNodes(xpathExpr);
        }
        public static string GetXmlNodeInfo(XmlNode node, string type = "xml")
        {
            if (node == null)
                return "Empty node or error node";
            string xmlNodeInfo = null;
            switch (type)
            {
                case "text":
                    xmlNodeInfo = node.InnerText;
                    break;
                default:
                    xmlNodeInfo = node.InnerXml;
                    break;
            }
            return xmlNodeInfo;
        }
        public static void Main(string[] args)
        {
            var xmlDoc = LoadXmlDoc("XMLFile1.xml");
            var rootExpr = "/bookstore";   //  根节点对应的XPath表达式
            var rootNode = GetXmlNodes(xmlDoc, rootExpr);   // 
            Console.WriteLine("XPath表达式为 /bookstore，根节点bookstore的所有子节点XML内容如下：");
            Console.WriteLine(GetXmlNodeInfo(rootNode[0]));
            Console.WriteLine();
            var allBooksExpr = "/bookstore/book"; // 根节点bookstore的子元素的所有子节点
            var bookNodes = GetXmlNodes(xmlDoc, allBooksExpr);
            Console.WriteLine("XPath表达式为 bookstore/book，book节点共有：" + bookNodes.Count);
            Console.WriteLine();
            var anyBookExpr = "//book"; // 选取所有book子元素，而不管它们在文档中的位置
            var anyBookNodes = GetXmlNodes(xmlDoc, anyBookExpr);
            Console.WriteLine("XPath表达式为 //book，book节点共有：" + anyBookNodes.Count);
            Console.WriteLine(anyBookNodes[0].InnerXml);
            Console.WriteLine(anyBookNodes[0].OuterXml);
            Console.WriteLine();
            var categoryExpr = "//@category";   // 选取名为category的所有属性
            var allCategoryNodes = GetXmlNodes(xmlDoc, categoryExpr);
            Console.WriteLine("XPath表达式为 //@category，category节点共有：" + allCategoryNodes.Count);
            Console.WriteLine(allCategoryNodes[0].InnerText);
            Console.WriteLine(allCategoryNodes[0].InnerXml);
            Console.WriteLine();
            var titleWithLangExpr = "//title[@lang]";   // 选取所有带有lang属性的title节点
            var titleWithLangNodes = GetXmlNodes(xmlDoc, titleWithLangExpr);
            Console.WriteLine("XPath表达式为 //title[@lang]，带lang属性的title节点共有：" + titleWithLangNodes.Count);
            Console.WriteLine(GetXmlNodeInfo(titleWithLangNodes[0]));
            var englishTitleExpr = "//title[@lang='en']";   // 选取所有lang属性值为en的title节点
            var englishTitleNodes = GetXmlNodes(xmlDoc, englishTitleExpr);
            Console.WriteLine("XPath表达式为 //title[@lang='en']，lang属性值为en的title节点共有：" + englishTitleNodes.Count);
            Console.WriteLine(GetXmlNodeInfo(englishTitleNodes[0]));
            Console.WriteLine();
            // 使用索引的XPath查询
            var indexExpr = "/bookstore/book[1]";   // 取bookstore子元素的第一个book元素
            var firstBookNode = GetXmlNodes(xmlDoc, indexExpr);
            Console.WriteLine("XPath表达式为 /bookstore/book[1]，节点数为：" + firstBookNode.Count);
            Console.WriteLine(GetXmlNodeInfo(firstBookNode[0]));
            Console.WriteLine();

            var indexExpr2 = "/bookstore/book[last()]"; // 取bookstore子元素的最后一个book元素
            var lastBookNode = GetXmlNodes(xmlDoc, indexExpr2);
            Console.WriteLine("XPath表达式为 /bookstore/book[last()]，节点数为：" + lastBookNode.Count);
            Console.WriteLine(GetXmlNodeInfo(lastBookNode[0]));
            Console.WriteLine();
            var indexExpr3 = "/bookstore/book[last()-1]"; // 取bookstore子元素的倒数第二个book元素
            var nextByLastBookNode = GetXmlNodes(xmlDoc, indexExpr3);
            Console.WriteLine("XPath表达式为 /bookstore/book[last()-1]，节点数为：" + lastBookNode.Count);
            Console.WriteLine(GetXmlNodeInfo(nextByLastBookNode[0]));
            Console.WriteLine();
            var indexExpr4 = "/bookstore/book[position()<3]"; // 取bookstore的前两个book子元素
            var firstTwoBookNodes = GetXmlNodes(xmlDoc, indexExpr4);
            Console.WriteLine("XPath表达式为 /bookstore/book[position()<3]，节点数为：" + firstTwoBookNodes.Count);
            Console.WriteLine(GetXmlNodeInfo(firstTwoBookNodes[0]));
            Console.WriteLine();
            // 带属性值过滤条件的XPath表达式
            var fileterExpr = "/bookstore/book[price>35.00]";   // 选取bookstore的所有price属性值大于35.00的book元素
            var bookGt35Nodes = GetXmlNodes(xmlDoc, fileterExpr);
            Console.WriteLine("XPath表达式为 /bookstore/book[price>35.00]，节点数为：" + bookGt35Nodes.Count);
            Console.WriteLine(GetXmlNodeInfo(bookGt35Nodes[0]));
            // 通配符
            // @*                匹配任何属性节点
            // node()             匹配任何类型的节点
            // /bookstore/*   选取 bookstore 元素的所有子元素
            // //*                   选取文档的所有元素
            // //title[@*]        选取所有带有属性的 title 元素
            var allTitleWithAttrExpr = "//title[@*]";
            var allTitleWithAttrNodes = GetXmlNodes(xmlDoc, allTitleWithAttrExpr);
            Console.WriteLine("XPath表达式为 title[@*]，节点数为：" + allTitleWithAttrNodes.Count);
            Console.WriteLine(GetXmlNodeInfo(allTitleWithAttrNodes[0]));
            Console.WriteLine();
            // |        或
            var titleAndPriceExpr = "//book/title | //book/price";
            var titleAndPriceNodes = GetXmlNodes(xmlDoc, titleAndPriceExpr);
            Console.WriteLine("XPath表达式为 //book/title | //book/price，节点数为：" + titleAndPriceNodes.Count);
            Console.WriteLine(GetXmlNodeInfo(titleAndPriceNodes[0]));
            // text()  选取文本
            var titleTextExpr = "//title/text()";
            var titleTextNodes = GetXmlNodes(xmlDoc, titleTextExpr);
            Console.WriteLine("XPath表达式为 //title/text()，节点数为：" + titleTextNodes.Count);
            Console.WriteLine(titleTextNodes[0].Value); // 文本节点的值
            Console.ReadKey();
        }
    }
}