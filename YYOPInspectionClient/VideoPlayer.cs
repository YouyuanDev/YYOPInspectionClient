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
        public VideoPlayer(string url)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(url))
            {
                string serverUrl = CommonUtil.getServerIpAndPort();
                string videoUrl = serverUrl + "upload/" + url;
                this.webBrowser1.Url = new Uri(videoUrl);
            }
            else {
                MessagePrompt.Show("未找到视频链接,重新打开试一下!");
            }
        }
    }
}
