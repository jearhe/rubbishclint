using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace 串口接收
{
    public partial class Form1 : Form
    {
        int k = 0;
        string str;

        //添加接收事件开关
        bool isfrist = true;
        //垃圾桶高度
        public int h1=0, h2 = 0, h3 = 0, h4 = 0;
        //下载当前数据
        WebClient wc = new WebClient();
        //开关
        bool isconnet = false;
        //实例化一个SerialPort
        private SerialPort ComDevice = new SerialPort();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //打开串口
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (ComDevice.IsOpen == false)
            {
                if (comboBox1.SelectedIndex==-1)
                {
                    MessageBox.Show("输入串口号");
                }
                else { 
                    //串口号
                    ComDevice.PortName = comboBox1.Text;
                    //波特率
                    ComDevice.BaudRate = Convert.ToInt32("115200");
                    //设置奇偶校验检查协议
                    ComDevice.Parity = Parity.None;
                    //每个字节的标准数据位长度
                    ComDevice.DataBits = 8;
                    //设置每个字节的标准停止位数
                    ComDevice.StopBits = StopBits.One;
                    try
                    {
                        ComDevice.Open();
                        //绑定事件
                        if (isfrist)
                            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    btnOpen.Text = "关闭串口";
                }
            }
            else
            {
                try
                {
                    ComDevice.Close();
                    isfrist = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnOpen.Text = "打开串口";
            }
        }
        /// <summary>
        /// 串口接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            str += ComDevice.ReadExisting();//字符串方式读
            if (str.Length >= 20)
            {
                kk(str);
                str = "";
            }
        }


        /// <summary>
        /// 分析数据中的四个高度
        /// </summary>
        /// <param name="str">接收的字符串</param>
        void kk(string str)
        {
            h1=int.Parse(str.Substring(1,2));
            h2 = int.Parse(str.Substring(6, 2));
            h3 = int.Parse(str.Substring(11, 2));
            h4 = int.Parse(str.Substring(16, 2));
            detail(h1,h2,h3,h4);
            txtShowData.AppendText("h1:"+h1.ToString()+ "h2:"+ h2.ToString()+ "h3:"+ h3.ToString()+ "h4:"+ h4.ToString()+"\n");
        }

        /// <summary>
        /// 向服务器提交参数
        /// </summary>
        /// <param name="a">h1</param>
        /// <param name="b">h2</param>
        /// <param name="c">h3</param>
        /// <param name="d">h4</param>
        void detail(int a,int b,int c,int d)
        {
            if (isconnet)
            {
                str = wc.DownloadString("http://www.overme.xin/Rubbish/postRubbish?h1=" + a + "&h2=" + b + "&h3=" + c + "&h4=" + d + "");
                if (str == "\"1\"")
                {
                    RTB_client.Text +="("+a+" "+b + " " + c + " " + d + ")" + "存入数据库成功\n";
                }
                else
                {
                    RTB_client.Text += "存入数据库失败\n";
                }
            }
        }


        /// <summary>
        /// 清空按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_clear2_Click(object sender, EventArgs e)
        {
            RTB_client.Text = "";
        }

        /// <summary>
        /// 随机生成高度，测试用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            random.Next(0, 24);

            detail(random.Next(0, 20), random.Next(0, 20), random.Next(0, 20), random.Next(0, 20));
        }


        /// <summary>
        /// 清空按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            txtShowData.Text = "";
        }

        /// <summary>
        /// 连接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_lianjie_Click(object sender, EventArgs e)
        {
            if (isconnet)
            {
                isconnet = !isconnet;
                bt_lianjie.Text = "建立连接";
            }
            else
            {
                isconnet = !isconnet;
                bt_lianjie.Text = "断开连接";
            }
               

        }


    }
}
