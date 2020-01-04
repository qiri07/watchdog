using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace VideoMonitor
{
    public partial class Form1 : Form
    {
        public static int countdeads = 0;
        public static int countreboot = 0;
        public static int num = 0;
        public static List<string> liststr = new List<string>();
        public System.Timers.Timer nTimer = new System.Timers.Timer();
        public static int iintval = 0;
        public System.Timers.Timer aTimer = new System.Timers.Timer();       //System.Timers，不是form的  

        public Form1()
        {
            InitializeComponent();
            readConfig();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
            #region 定时器事件  
            aTimer.Elapsed += new ElapsedEventHandler(bSendDingTalkText);
            aTimer.Interval = 60 * 1000;    //配置文件中配置的秒数 1分钟检测一次 
            aTimer.Enabled = true;
            #endregion

            nTimer.Elapsed += new ElapsedEventHandler(bSendDingTalkText);

            toolStripStatusLabel2.Text = "程序启动！";
        }

        private void readConfig()
        {
            AppParam.Init();
            num = AppParam.Numbers;
            liststr = AppParam.Vstr;
        }
        private void bSendDingTalkText(object source, ElapsedEventArgs e)
        {
            try
            {
                for(int i=0;i<num;i++)
                {
                    if (ThreadExitis(liststr[i], false))
                    {
                        continue;
                    }
                    else
                    {
                        Restartnew((object)liststr[i]);
                        int index = this.dataGridView1.Rows.Add();
                        this.dataGridView1.Rows[index].Cells[0].Value = index;
                        this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                        this.dataGridView1.Rows[index].Cells[2].Value = "程序"+liststr[i]+"启动完成！";
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                    } 
                }

                //while (dataGridView1.Rows.Count > 10)
                //{
                //    //dataGridView1.Rows.Clear();
                //    dataGridView1.Rows.RemoveAt(0);
                //    //dataGridView1.Refresh();
                //}


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                toolStripStatusLabel2.Text = ex.ToString();
            }
        }

        public bool ThreadExitis(string threadName, bool kill)
        {
            bool bo = false;
            try {
                    System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
                    foreach (System.Diagnostics.Process process in processList)
                    {

                        if (process.ProcessName.ToLower() == threadName.ToLower())
                        {

                            if (kill)
                            {
                                bo = false;
                                process.Kill(); //结束进程 
                                countdeads++;
                            }
                            else
                            {
                                bo = true;
                            }
                        }
                    }
                }
                catch
                {
                    return bo;
                }

            return bo;
        }

        private void Restartnew(object ProcessName)

        {
            try {

                Thread thtmp = new Thread(new ParameterizedThreadStart(run));

                object appName = ProcessName; //"EasyRTSPLive"

                //Thread.Sleep(1000 * 60);

                thtmp.Start(appName);
                countreboot++;
            }
            catch(Exception e)
            {
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = index;
                this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                this.dataGridView1.Rows[index].Cells[2].Value = "异常Restartnew！" + e.Message;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
            }
        }

        private void run(Object obj)

        {
            try
            {
                Process ps = new Process();

                ps.StartInfo.FileName = obj.ToString();

                ps.Start();
            }
            catch (Exception e)
            {
                //int index = this.dataGridView1.Rows.Add();
                //this.dataGridView1.Rows[index].Cells[0].Value = index;
                //this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                //this.dataGridView1.Rows[index].Cells[2].Value = "异常run！" + e.Message;
                //this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                Console.WriteLine(e.ToString());
            }

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text =DateTime.Now.ToString()+" ";
            toolStripStatusLabel2.Text = "累计重启 "+ countreboot + " 次;累计杀死 "+ countdeads + " 次";
            dataGridView1.Update();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("视频守护进程是否确认关闭?", "视频守护进程退出确认", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
            }
            else
            {
                e.Cancel = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i=0;i<num;i++)
            {
                if (ThreadExitis(liststr[i], false))
                {
                    this.Enabled = false;
                    ThreadExitis(liststr[i], true);
                    Thread.Sleep(5000);
                    Restartnew((object)liststr[i]); //Ncnn_FaceTrackHK
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = index;
                    this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                    this.dataGridView1.Rows[index].Cells[2].Value = "程序"+ liststr[i] + "关闭并重启完成！";
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                    this.Enabled = true;
                }
                else
                {
                    this.Enabled = false;
                    Restartnew((object)liststr[i]); //Ncnn_FaceTrackHK
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = index;
                    this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                    this.dataGridView1.Rows[index].Cells[2].Value = "程序"+ liststr[i] + "启动完成！";
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                    this.Enabled = true;
                }
            }
           
        }

        public enum Intvals
        {
            second=1000,
            minute= 1000 * 60,
            hour= 1000 * 60 * 60,
            day= 1000 * 60 * 60 * 24
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                int irun = 0;
                int ires = 0;
                if((textBox1.Text!=null)&&(int.TryParse(textBox1.Text,out ires))&&(comboBox1.SelectedItem!=null))
                {
                    if(ires>0)
                    {
                        string ss=comboBox1.SelectedItem.ToString();
                        switch(ss)
                        {
                            case "秒":
                                irun=(int)Intvals.second;
                                break;
                            case "分钟":
                                irun = (int)Intvals.minute; 
                                break;
                            case "小时":
                                irun = (int)Intvals.hour;
                                break;
                            case "天":
                                irun = (int)Intvals.day;
                                break;
                            
                            default:
                                irun = (int)Intvals.hour;
                                break;
                        }
                        iintval = ires * irun;
                        int index = this.dataGridView1.Rows.Add();
                        this.dataGridView1.Rows[index].Cells[0].Value = index;
                        this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                        this.dataGridView1.Rows[index].Cells[2].Value = "设置全局定时器 " + ires + ss+" 启动完成！";
                        this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];


                        nTimer.Interval = iintval;    //配置 
                        nTimer.Enabled = true;
                        if(nTimer.Interval<61*1000)
                        {
                            //aTimer.Stop();
                            aTimer.Enabled = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("输入数字要>0");
                        //this.checkBox1.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("输入的值不正确，请输入数字>0");
                    //this.checkBox1.Checked = false;
                }
            }
            else
            {
                if(iintval>0)
                {
                    if(aTimer.Enabled==false)
                    {
                        aTimer.Enabled = true;
                    }
                    //nTimer.Stop();
                    nTimer.Enabled = false;
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = index;
                    this.dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
                    this.dataGridView1.Rows[index].Cells[2].Value = "全局定时器设置取消！";
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[index].Cells[0];
                }
                
            }
        }
    }

}
