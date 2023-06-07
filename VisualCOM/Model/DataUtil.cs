using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualCOM.ViewModel;

namespace VisualCOM.Model
{
    public class DataUtil
    {
        public byte head;//帧头
        public byte tail;//帧尾
        public byte separator;//分隔符
        public int channelCount;//通道数

        List<Type> types;
        private int[] typeCount;
        private int setIndex;
        public DataUtil(string head, string tail, string separator, int count)
        {
            try
            {
                //清除字符0x 0X
                string s_head = head.Replace("0x", String.Empty);
                s_head = s_head.Replace("0X", String.Empty);

                string s_tail = tail.Replace("0x", String.Empty);
                s_tail = s_tail.Replace("0X", String.Empty);

                string s_separator = separator.Replace("0x", String.Empty);
                s_separator = s_separator.Replace("0X", String.Empty);

                this.head = System.Convert.ToByte(s_head, 16);
                this.tail = System.Convert.ToByte(s_tail, 16);
                this.separator = System.Convert.ToByte(separator, 16);

                channelCount = count;
                types = new List<Type>(channelCount);
                typeCount = new int[channelCount];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        public void SetType(Type t)
        {
            try
            {
                types.Add(t);
                if (t == typeof(byte))
                {
                    typeCount[setIndex] = 1;
                }
                else if (t == typeof(int))
                {
                    typeCount[setIndex] = 4;
                }
                else if (t == typeof(long))
                {
                    typeCount[setIndex] = 8;
                }
                else if (t == typeof(float))
                {
                    typeCount[setIndex] = 4;
                }
                else if (t == typeof(double))
                {
                    typeCount[setIndex] = 8;
                }
                else
                {
                    return;
                }
                setIndex++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 纯Hex数据格式
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        public double[] GetAnalysisData(byte[] receiveData)
        {
            try
            {
                if (receiveData.Length < 3)
                    return null;
                if (receiveData[0] == head && receiveData[receiveData.Length - 1] == tail)
                {
                    if (receiveData.Length == 3)
                        return new double[] { receiveData[1] };
                    else
                    {
                        //data1是去帧头帧尾
                        byte[] data1 = new byte[receiveData.Length - 2];
                        for (int i = 0; i < data1.Length; i++)
                        {
                            data1[i] = receiveData[i + 1];
                        }
                        //windows平台
                        double[] retData = new double[channelCount];
                        //游标
                        int readIndex = 0;
                        for (int i = 0; i < channelCount; i++, readIndex++)
                        {
                            if (typeCount[i] == 1)
                            {
                                retData[i] = data1[readIndex];
                                readIndex += 1;
                                continue;
                            }
                            else if (typeCount[i] == 4)
                            {
                                if (types[i] == typeof(int))
                                {
                                    retData[i] = BitConverter.ToInt32(data1, readIndex);
                                    readIndex += 4;
                                }
                                else if (types[i] == typeof(float))
                                {
                                    retData[i] = BitConverter.ToSingle(data1, readIndex);
                                    readIndex += 4;
                                }
                                continue;
                            }
                            else if (typeCount[i] == 8)
                            {
                                if (types[i] == typeof(double))
                                {
                                    retData[i] = BitConverter.ToDouble(data1, readIndex);
                                    readIndex += 8;
                                }
                                else if (types[i] == typeof(long))
                                {
                                    retData[i] = BitConverter.ToInt64(data1, readIndex);
                                    readIndex += 8;
                                }
                                continue;
                            }
                        }
                        return retData;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                //Nlog补充异常日志
                Console.WriteLine(e.Message);
                return null;
            }

        }
        /// <summary>
        /// 字符串格式
        /// </summary>
        /// <param name="receiveData"></param>
        /// <returns></returns>
        public double[] GetAnalysisStrData(byte[] receiveData)
        {
            try
            {
                if (receiveData.Length < 4)
                    return null;
                if (receiveData[0] == head && receiveData[receiveData.Length - 2] == tail)
                {
                    if (receiveData.Length == 4)
                        return new double[] { receiveData[1] };
                    else
                    {
                        //data1是去帧头帧尾
                        byte[] data1 = new byte[receiveData.Length - 3];
                        for (int i = 0; i < data1.Length; i++)
                        {
                            data1[i] = receiveData[i + 1];
                        }
                        //windows平台
                        double[] retData = new double[channelCount];
                        //游标
                        int readIndex = 0;
                        for (int i = 0; i < channelCount; i++, readIndex++)
                        {
                            if (typeCount[i] == 1)
                            {
                                retData[i] = data1[readIndex];
                                readIndex += 1;
                                continue;
                            }
                            else if (typeCount[i] == 4)
                            {
                                if (types[i] == typeof(int))
                                {
                                    retData[i] = BitConverter.ToInt32(data1, readIndex);
                                    readIndex += 4;
                                }
                                else if (types[i] == typeof(float))
                                {
                                    retData[i] = BitConverter.ToSingle(data1, readIndex);
                                    readIndex += 4;
                                }
                                continue;
                            }
                            else if (typeCount[i] == 8)
                            {
                                if (types[i] == typeof(double))
                                {
                                    retData[i] = BitConverter.ToDouble(data1, readIndex);
                                    readIndex += 8;
                                }
                                else if (types[i] == typeof(long))
                                {
                                    retData[i] = BitConverter.ToInt64(data1, readIndex);
                                    readIndex += 8;
                                }
                                continue;
                            }
                        }
                        return retData;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                //Nlog补充异常日志
                Console.WriteLine(e.Message);
                return null;
            }

        }
    }
}
