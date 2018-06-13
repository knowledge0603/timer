using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;

namespace export
{
    public partial class exportData : Form
    {
        System.IO.DirectoryInfo dirInfo;
        private string filePath = AppDomain.CurrentDomain.BaseDirectory + "Data\\";
        private int _nowSecond = 1;
        #region 初始化
        public exportData()
        {
            InitializeComponent();
            dirInfo = new System.IO.DirectoryInfo(filePath);
        }
        #endregion

        #region  数据导出
        private void button2_Click(object sender, EventArgs e)
        {
            ArrayList businessType = new ArrayList();
            businessType.Clear();

            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (CheckBox ck in ctl.Controls)
                    {
                        if (ck.Checked == true)
                        {
                            businessType.Add(ck.Tag.ToString());
                        }
                    }
                }
            }

            for (int i=0;i< businessType.Count;i++) {
                exportProcess objExportProcess = new exportProcess(businessType[i].ToString());
                ThreadStart startExport = new ThreadStart(objExportProcess.excuteExport);
                Thread thredExport = new Thread(startExport);
                thredExport.Start();
            }
            /*string[] businessTypeArray = { "03", "04", "18", "01", "02", "05", "06", "11", "12", "13", "14", "15", "16", "17", "21", "22", "23", "62", "31", "32", "33", "41", "51", "52", "88", "601", "602", "603", "604", "605", "606", "607", "608", "609", "610", "701", "702" };
            for (int i = 0; i < businessTypeArray.Length; i++)
            {
                exportProcess objExportProcess = new exportProcess(businessTypeArray[i]);
                ThreadStart startExport = new ThreadStart(objExportProcess.excuteExport);
                Thread thredExport = new Thread(startExport);
                thredExport.Start();
            }*/
            
            //使得计时器有效
            timer1.Start();
            timer2.Start();
        }
        #endregion

        #region 导出数据 处理
        public class exportProcess
        {
            private string bussinessType;

            public exportProcess(string bussinessType)
            {
                this.bussinessType = bussinessType;
            }
            public void excuteExport()
            {
                var objExportTask = new Task<outPutExport>(exportTask, new inPutExport() { businessTypeInput = bussinessType});
                objExportTask.Start();
            }
           
        }
        #endregion

        #region  task 调用
        public class inPutExport
        {
            private string businessType;
            public string businessTypeInput { get => businessType; set => businessType = value; }
        }

        public class outPutExport
        {
            private string outPut;
            public string OutPut { get => outPut; set => outPut = value; }
        }

        public static outPutExport exportTask(object para)
        {
            return new outPutExport()
            {
                OutPut = Export.exportData( ((inPutExport)para).businessTypeInput)
            };
        }
        #endregion
        
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (CheckBox ck in ctl.Controls)
                    {
                        ck.Checked = true;
                    }
                }
            }
        }

        private void btnUnCheck_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is GroupBox)
                {
                    foreach (CheckBox ck in ctl.Controls)
                    {
                        if (ck.Checked == true)
                        {
                            ck.Checked = false;
                        }
                        else
                        {
                            ck.Checked = true;
                        }
                    }
                }
            }
        }

        public static int GetFilesCount(System.IO.DirectoryInfo dirInfo)
        {
            int totalFile = 0;
            totalFile += dirInfo.GetFiles().Length;
            foreach (System.IO.DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                totalFile += GetFilesCount(subdir);
            }
            return totalFile;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _nowSecond++;
            int hour = _nowSecond / 3600;
            labelHour.Text = hour.ToString();
            int minute = (_nowSecond /60) ;
            labelMinute.Text = minute.ToString();
            double second = _nowSecond % 60 ;
            labelSecond.Text = second.ToString();
            btnFileCount.Text = GetFilesCount(dirInfo).ToString();
        }
        string temp = "";
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (temp== GetFilesCount(dirInfo).ToString()) {
                timer1.Stop();
            }
            temp = GetFilesCount(dirInfo).ToString();
        }
    }
}
