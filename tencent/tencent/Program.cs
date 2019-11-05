using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tencent
{
    class Program
    {
        static void Main(string[] args)
        {

            comb(3, 2);

        }

        /// <summary>
        /// tencent 面试题
        /// 0    1    2    3    4    5    6    7    8    9
        //1    1    1    1    1    1    1    1    1    1    
        //0    10   0    0    0    0    0    0    0    0    
        //9    0    0    0    0    0    0    0    0    0
        //9    0    0    0    0    0    0    0    0    1
        //8    1    0    0    0    0    0    0    1    0
        //7    2    1    0    0    0    0    1    0    0    
        //6    2    1    0    0    0    1    0    0    0
        /// </summary>
        private static void tencentQuesten()
        {
            int len = 10;
            int[] top = new int[len];
            int[] bottom = new int[len];

            bool success = false;
            for (int i = 0; i < len; i++)
            {
                top[i] = i;
            }

            while (!success)
            {
                bool reB = true;
                for (int i = 0; i < len; i++)
                {
                    int frequecy = getFrequecy(bottom, i);
                    if (bottom[i] != frequecy)
                    {
                        bottom[i] = frequecy; reB = false;
                    }
                }
                success = reB;
            }
        }

        /// <summary>
        /// 腾讯面试题笨办法
        /// </summary>
        private static void tencetQue()
        {
            bool bo = isSuccess(new int[] { 6, 2, 1, 0, 0, 0, 1, 0, 0, 0 });
            Console.WriteLine("6,2,1,0,0,0,1,0,0,0 is the success result!!!");
            List<Array> list = new List<Array>();
            for (int i0 = 0; i0 < 10; i0++)
            {
                for (int i1 = 0; i1 < 10; i1++)
                {
                    for (int i2 = 0; i2 < 10; i2++)
                    {
                        for (int i3 = 0; i3 < 10; i3++)
                        {
                            for (int i4 = 0; i4 < 10; i4++)
                            {
                                for (int i5 = 0; i5 < 10; i5++)
                                {
                                    for (int i6 = 0; i6 < 10; i6++)
                                    {
                                        for (int i7 = 0; i7 < 10; i7++)
                                        {
                                            for (int i8 = 0; i8 < 10; i8++)
                                            {
                                                for (int i9 = 0; i9 < 2; i9++)
                                                {
                                                    int sum = 0;
                                                    int[] arr = new int[] { i0, i1, i2, i3, i4, i5, i6, i7, i8, i9 };
                                                    foreach (var item in arr)
                                                    {
                                                        sum += item;
                                                    }
                                                    if (sum == 10)
                                                    {
                                                        if (isSuccess(arr))
                                                        {
                                                            list.Add(arr);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int getFrequecy(int[] bottom, int num)
        {
            int count = 0; for (int i = 0; i < 10; i++) { if (bottom[i] == num) count++; }
            return count;    //cout即对应 frequecy
        }

        private static void setNextBottom()
        {
            throw new NotImplementedException();
        }

        public static bool isSuccess(int[] arr)
        {
            for (int num = 0; num < 10; num++)
            {
                int count = 0;
                for (int i = 0; i < 10; i++)
                {
                    //if (i == num)
                    //{
                    //    count++;
                    //}
                    if (arr[i] == num)
                    {
                        count++;
                    }
                }
                if (arr[num] != count)
                {
                    return false;
                }
            }
            return true;
        }


        //0123456789
        //abcdefghij
        //9876543210
        //0abcdefghi
        //1000000009 
        private static List<int> GetResult()
        {
            List<int> intResult = new List<int>();
            int count = 0;
            Dictionary<int, int> digitSummary = new Dictionary<int, int>();
            int[] intArray = null;

            for (int i = 0; i < 1999999999; i++)
            {
                intArray = GetIntArray(i);
                for (int j = 0; j < intArray.Length; j++)
                {
                    count += intArray[j];
                    if (digitSummary.Keys.Contains(intArray[j]))
                    {
                        digitSummary[intArray[j]]++;
                    }
                    else
                    {
                        digitSummary.Add(intArray[j], 1);
                    }
                }

                if (count != 10)
                    continue;
                bool digitSummaryVerificationPass = true;
                for (int j = 0; j < 10; j++)
                {

                    if (digitSummary.Keys.Contains(j) && digitSummary[j] != intArray[j])
                    {
                        digitSummaryVerificationPass = false;
                        break;
                    }
                }
                if (!digitSummaryVerificationPass)
                    continue;

                intResult.Add(i);
            }
            return intResult;
        }

        private static int[] GetIntArray(int srcInt)
        {
            int[] intArray = new int[10];
            for (int i = 9; i >= 0; i--)
            {
                intArray[i] = srcInt % 10;
                srcInt = srcInt / 10;
            }

            return intArray;
        }

        private static void ZuHe()
        {
            //问题描述：找出从自然数1、2、……、n中任取r个数的所有组合。例如n = 5，r = 3的所有组合为：⑴5、4、3 ⑵5、4、2 ⑶5、4、1
            //        ⑷5、3、2 ⑸5、3、1 ⑹5、2、1
            //        ⑺4、3、2 ⑻4、3、1 ⑼4、2、1
            //        ⑽3、2、1

            //分析所列的10个组合，可以采用这样的递归思想来考虑求组合函数的算法。设函数为void comb(int m, int k）为找出从自然数1、2、……、m中任取k个数的所有组合。当组合的第一个数字选定时，其后的数字是从余下的m - 1个数中取k - 1数的组合。这就将求m 个数中取k个数的组合问题转化成求m - 1个数中取k - 1个数的组合问题。设函数引入工作数组a[ ]存放求出的组合的数字，约定函数将确定的k个数字组合的第一个数字放在a[k]中，当一个组合求出后，才将a[ ]中的一个组合输出。第一个数可以是m、m - 1、……、k，函数将确定组合的第一个数字放入数组后，有两种可能的选择，因还未去顶组合的其余元素，继续递归去确定；或因已确定了组合的全部元素，输出这个组合。细节见以下程序中的函数comb。
        }


        private static void comb(int m, int k)
        {
            int[] a = new int[100];
            a[0] = 3;
            int i, j;
            for (i = m; i >= k; i--)
            {
                a[k] = i;
                if (k > 1)
                {
                    comb(i - 1, k - 1);
                }
                else
                {
                    for (j = a[0]; j > 0; j--)
                    {
                        Console.Write(a[j]);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
