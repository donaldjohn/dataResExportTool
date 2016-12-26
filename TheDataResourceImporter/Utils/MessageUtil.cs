﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheDataResourceExporter.Utils
{
    public class MessageUtil
    {
        //设置进度消息的委托
        //public delegate void SetMessageHander(string msg);
        //public static SetMessageHander SetMessage = null;

        public delegate void SetTextboxDetailHander(string msg);
        public static SetTextboxDetailHander setTbDetail = null;

        public delegate void AppendTextboxDetailHander(string msg);
        public static AppendTextboxDetailHander appendTbDetail = null;


        public delegate void updateProgressIndicatorHander(int totalCount, int handledCount, int handledXMLCount, int handledDirCount, string achievePath);
        public static updateProgressIndicatorHander updateProgressIndicator = null;


        public static void DoSetTBDetail(string msg)
        {
            //添加时间标识
            DateTime now = System.DateTime.Now;
            string timeStamp = now.ToLocalTime().ToString() + " " + now.Millisecond;
            //添加消息换行
            msg = Environment.NewLine + timeStamp + Environment.NewLine + msg;

            setTbDetail?.Invoke(msg);
        }


        public static void DoAppendTBDetail(string msg)
        {
            //添加时间标识
            DateTime now = System.DateTime.Now;
            string timeStamp = now.ToLocalTime().ToString() + " " + now.Millisecond;
            //添加消息换行
            msg = Environment.NewLine + timeStamp + Environment.NewLine + msg;

            appendTbDetail?.Invoke(msg);
        }

        public static void DoupdateProgressIndicator(int totalCount, int handledCount, int handledXMLCount, int handledDirCount, string achievePath)
        {
            updateProgressIndicator?.Invoke(totalCount, handledCount, handledXMLCount, handledDirCount, achievePath);
        }

        public static void showMessageBoxWithErrorLog(string message)
        {
            MessageUtil.DoSetTBDetail(message);
            MessageBox.Show(message);
        }
    }
}
