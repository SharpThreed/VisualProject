﻿using CommunityToolkit.Mvvm.Messaging;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualCOM.ContentHelp;
using VisualCOM.Model;

namespace VisualCOM.View
{
    /// <summary>
    /// ChartsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChartsPage : UserControl
    {
        private SerialPortModel serialPortModel;
        //外部
        public static double[] LiveData = new double[] { };
        public static double[] xs = new double[] { };
        /// <summary>
        /// 数据点位
        /// </summary>
        public static int LiveDataLength = 100;
        /// <summary>
        /// 是否默认解析
        /// </summary>
        private bool isDefultFormat = true;

        List<ScatterPlotListRe<double>> _list = new List<ScatterPlotListRe<double>>();

        public List<ChartArgs> ReceiveChartArgs { get; set; }
        /// <summary>
        ///添加的通道数
        /// </summary>
        private List<string> _operateName = new List<string>();
        /// <summary>
        /// 解析格式
        /// </summary>
        private DataUtil _dataUtil;
        public ChartsPage()
        {
            InitializeComponent();

            //绘图参数使
            WeakReferenceMessenger.Default.Register<List<ChartArgs>>(this, (r, m) =>
            {
                ReceiveChartArgs = m;
            });

            //通道解析格式选择(目前仅支持单解析)
            WeakReferenceMessenger.Default.Register<DataUtil>(this, (r, m) =>
            {
                _dataUtil = m;
                isDefultFormat = false;
            });

            //意外关闭串口
            WeakReferenceMessenger.Default.Register<InfoMessage>(this, (r, m) =>
            {

                if (m.MessageTitle == MessageHead.MainVMToClose)
                {
                    if (serialPortModel != null)
                        serialPortModel.ClosePort();
                }
            });

        }


        private void UpdateDefultData(object currentValue, DateTime now)
        {
            _list[0].Add(now.ToOADate(), System.Convert.ToDouble(currentValue));
            if (_list[0].GetXs().Count > LiveDataLength)
            {
                _list[0].GetXs().RemoveAt(0);
                _list[0].GetYs().RemoveAt(0);
            }

            Dispatcher.Invoke(() =>
            {
                s_plot.Plot.AxisAuto();
                s_plot.Refresh();
            });
        }
        private void UpdateData(double[] currentValue, DateTime now)
        {
            for (int i = 0; i < _operateName.Count; i++)
            {
                _list[i].Add(now.ToOADate(), currentValue[i]);
                if (_list[i].GetXs().Count > LiveDataLength)
                {
                    _list[i].GetXs().RemoveAt(0);
                    _list[i].GetYs().RemoveAt(0);
                }
            }
            Dispatcher.Invoke(() =>
            {
                s_plot.Plot.AxisAuto();
                s_plot.Refresh();
            });
        }

        private void AddSource(string chooseName)
        {

            ScatterPlotListRe<double> scatterPlot = new ScatterPlotListRe<double>();
            int index = System.Convert.ToInt32(chooseName.Remove(0, 2));
            foreach (var args in ReceiveChartArgs)
            {
                if (args.ChannelName.Equals(chooseName))
                {
                    switch (args.ChartType)
                    {
                        case "折线":
                            {
                                //样式设置
                                scatterPlot.Smooth = false;
                                scatterPlot.Color = args.GetColor();
                                scatterPlot.LineStyle = args.GetLineShape();
                                scatterPlot.LineWidth = args.LineSize;
                                scatterPlot.MarkerSize = args.PointSize;
                                scatterPlot.MarkerShape = args.GetPointShape();

                                xs = scatterPlot.GetXs().ToArray();
                                LiveData = scatterPlot.GetYs().ToArray();
                                s_plot.Plot.Add(scatterPlot);
                                scatterPlot.AddRange(xs, LiveData);

                                s_plot.Plot.XAxis.DateTimeFormat(true);
                                s_plot.Plot.AxisAuto();

                                s_plot.Plot.Style(args.GetStyle());

                                s_plot.Refresh();

                                break;
                            }
                        case "曲线":
                            {
                                //样式设置
                                scatterPlot.Smooth = true;
                                scatterPlot.Color = args.GetColor();
                                scatterPlot.LineStyle = args.GetLineShape();
                                scatterPlot.LineWidth = args.LineSize;
                                scatterPlot.MarkerSize = args.PointSize;
                                scatterPlot.MarkerShape = args.GetPointShape();

                                xs = scatterPlot.GetXs().ToArray();
                                LiveData = scatterPlot.GetYs().ToArray();

                                s_plot.Plot.Add(scatterPlot);
                                scatterPlot.AddRange(xs, LiveData);

                                s_plot.Plot.XAxis.DateTimeFormat(true);

                                s_plot.Plot.AxisAuto();

                                s_plot.Plot.Style(args.GetStyle());
                                s_plot.Refresh();

                                break;
                            }
                        case "散点":
                            {
                                //样式设置
                                scatterPlot.LineStyle = LineStyle.None;
                                scatterPlot.Color = args.GetColor();
                                scatterPlot.MarkerSize = args.PointSize;

                                xs = scatterPlot.GetXs().ToArray();
                                LiveData = scatterPlot.GetYs().ToArray();

                                s_plot.Plot.Add(scatterPlot);
                                scatterPlot.AddRange(xs, LiveData);

                                s_plot.Plot.XAxis.DateTimeFormat(true);
                                s_plot.Plot.AxisAuto();

                                s_plot.Plot.Style(args.GetStyle());
                                s_plot.Refresh();

                                break;
                            }
                        case "阶梯":
                            {

                                //样式设置
                                scatterPlot.Color = args.GetColor();
                                scatterPlot.LineStyle = args.GetLineShape();
                                scatterPlot.LineWidth = args.LineSize;
                                scatterPlot.StepDisplay = true;
                                scatterPlot.MarkerSize = args.PointSize;
                                scatterPlot.MarkerShape = args.GetPointShape();

                                scatterPlot.AddRange(xs, LiveData);

                                s_plot.Plot.Add(scatterPlot);
                                s_plot.Plot.XAxis.DateTimeFormat(true);

                                s_plot.Plot.AxisAuto();

                                s_plot.Plot.Style(args.GetStyle());
                                s_plot.Refresh();

                                break;
                            }
                        default: break;
                    }

                    _list.Insert(index - 1, scatterPlot);
                }
            }

        }

        private void RemoveSource(string chooseName)
        {
            int index = System.Convert.ToInt32(chooseName.Remove(0, 2));
            _list.RemoveAt(index - 1);
        }

        private void btBegin_Click(object sender, RoutedEventArgs e)
        {
            if (_operateName.Count == 0)
            {
                WeakReferenceMessenger.Default.Send<InfoMessage>(new InfoMessage() { MessageTitle = MessageHead.NoAddChart });
                return;
            }
            if (serialPortModel == null)
            {
                serialPortModel = ComPortHelp.serialPortModel;

                serialPortModel.DataReceived += SerialPortModel_DataReceived;

                //接收到有一个字节就触发
                //收到多少字节才触发事件
                serialPortModel.ReceivedBytesThreshold = 1;

                ///双使能后每次开关触发复位信息
                serialPortModel.DtrEnable = true;
                serialPortModel.RtsEnable = true;


                serialPortModel.OpenPort();

            }
        }

        private void SerialPortModel_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                //默认以字符解析
                if (isDefultFormat)
                {
                    if (serialPortModel.BytesToRead > 0)
                    {

                        //获得字节流
                        int count = serialPortModel.BytesToRead;
                        byte[] readBuffer = new byte[count];
                        serialPortModel.Read(readBuffer, 0, readBuffer.Length);
                        string data = Encoding.ASCII.GetString(readBuffer);
                        UpdateDefultData(data, DateTime.Now);

                    }
                }
                else
                {
                    if (serialPortModel.BytesToRead > 0)
                    {

                        int count = serialPortModel.BytesToRead;
                        byte[] readBuffer = new byte[count];
                        serialPortModel.Read(readBuffer, 0, readBuffer.Length);

                        double[] resData = _dataUtil.GetAnalysisData(readBuffer);
                        UpdateData(resData, DateTime.Now);
                    }
                }
            }
            catch (Exception mes)
            {
                Console.WriteLine(mes.Message);
                //!默认解析出错发出提示
            }



        }
        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortModel != null && serialPortModel.IsOpen)
            {
                serialPortModel?.ClosePort();
                serialPortModel = null;
            }
        }
        private void btClearData_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _list)
            {
                item.Clear();
            }
            s_plot.Refresh();
        }

        private void btAddChart_Click(object sender, RoutedEventArgs e)
        {
            string chooseName = (string)channelComBox.SelectedValue;
            //判断是否多次添加
            foreach (var item in _operateName)
            {
                if (item.Equals(chooseName))
                {
                    WeakReferenceMessenger.Default.Send<InfoMessage>(new InfoMessage() { MessageTitle = MessageHead.RepeatAddChart });
                    return;
                }
            }
            AddSource(chooseName);
            _operateName.Add(chooseName);
        }

        private void btRemoveChart_Click(object sender, RoutedEventArgs e)
        {
            string chooseName = (string)channelComBox.SelectedValue;
            //判断是否多次移除
            foreach (var item in _operateName)
            {
                if (item.Equals(chooseName))
                {
                    RemoveSource(chooseName);
                    _operateName.Remove(chooseName);
                    return;
                }
            }
            WeakReferenceMessenger.Default.Send<InfoMessage>(new InfoMessage() { MessageTitle = MessageHead.RepeatRemoveChart });

        }


    }
}
