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
        private string[] videoList = { };
        private int index = 0;
        public string url = "";
        public VideoPlayer(string url)
        {
            InitializeComponent();
            int width = Screen.PrimaryScreen.Bounds.Width;
            int hight = Screen.PrimaryScreen.Bounds.Height;
            this.panel1.Height = hight -80;
            this.panel2.Height = 50;
            this.url = url;
        }

        #region 切换视频
        private void btnChange_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (index < videoList.Length - 1)
                    index++;
                else
                    index = 0;
                string serverUrl = CommonUtil.getServerIpAndPort();
                url = videoList[index];
                if (!string.IsNullOrEmpty(url))
                {
                    string videoUrl = serverUrl + "inspection/videoPlayer.jsp?video_url=" + url;
                    this.webBrowser1.Url = new Uri(videoUrl);
                }
                else
                {
                    MessagePrompt.Show("未找到视频链接,重新打开试一下!");
                }
            }
            catch (Exception ex)
            {
                MessagePrompt.Show("播放时出错,错误信息:" + ex.Message);
            }

        }
        #endregion

        private void VideoPlayer_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(url))
            {
                videoList = url.Split(';');
                if (videoList.Length > 0)
                    url = videoList[0];
                if (!string.IsNullOrEmpty(url))
                {
                    string serverUrl = CommonUtil.getServerIpAndPort();
                    string videoUrl = serverUrl + "inspection/videoPlayer.jsp?video_url=" + url;
                    this.webBrowser1.Url = new Uri(videoUrl);
                }
                else
                {
                    MessagePrompt.Show("未找到视频链接,重新打开试一下!");
                }
            }
            else
            {
                MessagePrompt.Show("未找到视频链接,重新打开试一下!");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
