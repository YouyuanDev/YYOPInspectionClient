using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public partial class VideoPlayer : Form
    {
        private string []videoList={};
        private int index = 0;
        public string url = "";
        public VideoPlayer(string url)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(url))
            {
                videoList = url.Split(';');
                if (videoList.Length > 0)
                    url = videoList[0];
                if (!string.IsNullOrEmpty(url)) {
                    string serverUrl = CommonUtil.getServerIpAndPort();
                    string videoUrl = serverUrl + "upload/videos/" + url;
                    this.webBrowser1.Url = new Uri(videoUrl);
                }
                else
                {
                    MessagePrompt.Show("未找到视频链接,重新打开试一下!");
                }
            }
            else {
                MessagePrompt.Show("未找到视频链接,重新打开试一下!");
            }
        }

        #region 切换视频
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (index < videoList.Length)
                index++;
            else 
                index = 0;
            string serverUrl = CommonUtil.getServerIpAndPort();
            url = videoList[index];
            if (!string.IsNullOrEmpty(url)) {
                string videoUrl = serverUrl + "upload/videos/" + url;
                this.webBrowser1.Url = new Uri(videoUrl);
            }
            else
            {
                MessagePrompt.Show("未找到视频链接,重新打开试一下!");
            }

        } 
        #endregion
    }
}
