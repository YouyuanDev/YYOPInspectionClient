using Keyence.AutoID.SDK;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System;

namespace ReaderMaintenance
{
    public partial class Form1 : Form
    {
        private ReaderSearcher m_searcher = new ReaderSearcher();
        private Dictionary<string, ReaderAccessor> m_resisterdReaders = new Dictionary<string, ReaderAccessor>();
        List<NicSearchResult> m_nicList = new List<NicSearchResult>();

        #region FORM_CONTROL
        public Form1()
        {
            InitializeComponent();
            m_nicList = m_searcher.ListUpNic();
            if (m_nicList != null)
            {
                for (int i = 0; i < m_nicList.Count; i++)
                {
                    nicComboBox.Items.Add(m_nicList[i].NicName + "/" + m_nicList[i].NicIpAddr + "/" + m_nicList[i].NicIpv4Mask);
                }
            }
            nicComboBox.SelectedIndex = 0;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_searcher.Dispose();
            liveviewForm1.Dispose();
            foreach (ReaderAccessor reader in m_resisterdReaders.Values)
            {
                reader.Dispose();
            }
        }
        private void nicComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_searcher.SelectedNicSearchResult = m_nicList[nicComboBox.SelectedIndex];
        }
        #endregion
        #region SEARCH_PROCESS
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            if (!m_searcher.IsSearching)
            {
                removeUnusedReaders();
                //Start searching readers
                m_searcher.Start((res) => {
                    BeginInvoke(new delegateSearchResult(appendSearchResult), res);
                });
                searchUIControl(true);
            }
        }
        private void appendSearchResult(ReaderSearchResult res)
        {
            if (res.IpAddress == "")
            {
                searchUIControl(false);
                return;
            }
            string key = getKeyFromSearchResult(res);
            foreach (string oldkey in SearchedReaders_ChkLstBx.Items)   
            {
                // Ignore registerd reader.
                if (key == oldkey) return;
            }
            SearchedReaders_ChkLstBx.Items.Add(key);
            LiveViewReaderList.Items.Add(key);
            LiveViewReaderList.SelectedIndex = 0;
            label1.Text = SearchedReaders_ChkLstBx.Items.Count + " Readers Found.";
        }
        private void removeUnusedReaders()
        {
            for (int i = SearchedReaders_ChkLstBx.Items.Count - 1; i >= 0; i--)
            {
                String key = SearchedReaders_ChkLstBx.Items[i].ToString();
                if (!SearchedReaders_ChkLstBx.GetItemChecked(i))
                {
                    SearchedReaders_ChkLstBx.Items.Remove(key);
                    LiveViewReaderList.Items.Remove(key);
                }
                if (SearchedReaders_ChkLstBx.Items.Count == 0) break;
            }
        }
        //Delegate method for searching process
        private delegate void delegateSearchResult(ReaderSearchResult res);
        private string getKeyFromSearchResult(ReaderSearchResult res)
        {
            return res.IpAddress + "/" + res.ReaderModel + "/" + res.ReaderName;
        }
        private ReaderSearchResult getSearchResultFromKey(string key)
        {
            String[] readerInfo = key.Split('/');
            if (readerInfo.Length == 3)
            {
                return new ReaderSearchResult(readerInfo[1], readerInfo[2], readerInfo[0]);
            }
            return new ReaderSearchResult();
        }
        #endregion
        #region MAINTEANCE
        private void maintainSelect_CheckedChanged(object sender, EventArgs e)
        {
            setLiveview(maintainSelect.Checked);
        }
        private void setLiveview(bool enable)
        {
            enable = enable && (LiveViewReaderList.SelectedItem != null);
            if (enable)
            {
                string key = LiveViewReaderList.SelectedItem.ToString();
                ReaderSearchResult res = getSearchResultFromKey(key);
                liveviewForm1.EndReceive();
                liveviewForm1.IpAddress = res.IpAddress;
                liveviewForm1.BeginReceive();
                label3.Text = key;
            }else
            {
                liveviewForm1.EndReceive();
                label3.Text = "Stopped.";
            }
            maintenanceUIControl(enable);
        }
        #endregion
        #region READER_SELECTION
        private void SearchedReaders_ChkLstBx_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string key = SearchedReaders_ChkLstBx.Items[e.Index].ToString();

            if (e.NewValue == CheckState.Checked)
            {          
                e.NewValue = resisterReaders(key) ? CheckState.Checked : CheckState.Unchecked;              
            }
            else
            {
                e.NewValue = removeReaders(key) ? CheckState.Unchecked : CheckState.Checked;
            }
        }
        private bool resisterReaders(string key)
        {
            if (m_resisterdReaders.ContainsKey(key)) return false;

            ReaderSearchResult result = getSearchResultFromKey(key);
            m_resisterdReaders.Add(key, new ReaderAccessor(result.IpAddress));
            return true;
        }
        private bool removeReaders(string key)
        {
            if (m_resisterdReaders.ContainsKey(key))
            {
                m_resisterdReaders[key].Dispose();
                m_resisterdReaders.Remove(key);
                return true;
            }
            return false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < SearchedReaders_ChkLstBx.Items.Count; i++)
            {
                SearchedReaders_ChkLstBx.SetItemChecked(i, AllReadersSelectBtn.Checked);
            }
        }
        #endregion
        #region COMMAND_CONTROL
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                sendCommandToAllReaders();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            sendCommandToAllReaders();
        }
        private void sendCommandToAllReaders()
        {
            string cmd = commandTxt.Text;
            foreach (ReaderAccessor reader in m_resisterdReaders.Values)
            {
                reader.Connect();
                string resp = reader.ExecCommand(cmd);
                commandResponseText.AppendText("[" + reader.IpAddress + "][" + DateTime.Now + "]" + resp + "\r\n");
            }
            if (!commandTxt.Items.Contains(commandTxt.Text)) commandTxt.Items.Add(commandTxt.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            commandResponseText.Text = "";
        }
        #endregion
        #region FILE_CONTROL
        private void download_Click(object sender, EventArgs e)
        {
            string curdir = System.IO.Directory.GetCurrentDirectory();
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder.";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog.SelectedPath = curdir;
            folderBrowserDialog.ShowNewFolderButton = true;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                string todayDate = System.DateTime.Now.ToShortDateString().Replace('/', '_');
                string loggingFolder = folderBrowserDialog.SelectedPath + "\\" + todayDate;
                downloadPath.Text = loggingFolder;

                int readerCounter = 0;
                label2.Text = "--";
                foreach (ReaderAccessor reader in m_resisterdReaders.Values)
                {
                    readerCounter++;
                    string readerLoggingFolder = loggingFolder + "\\" + reader.IpAddress.Replace('.', '_');
                    Directory.CreateDirectory(readerLoggingFolder);
                    reader.OpenFtp();
                    getAllFilesInSrFolder(reader, "CONFIG", readerLoggingFolder);
                    getAllFilesInSrFolder(reader, "LUA", readerLoggingFolder);
                    reader.CloseFtp();
                    label2.Text = "Finished:"+readerCounter.ToString() + "/" + m_resisterdReaders.Count.ToString();
                    this.Refresh();
                }           
            }
        }
        private void getAllFilesInSrFolder(ReaderAccessor reader,string srcFolder,string dstFolder)
        {
            List<string> fileList = reader.GetFileList(srcFolder); 
            if (fileList != null)
            {
                foreach (string fileName in fileList)
                {
                    reader.GetFile(srcFolder+"\\" + fileName, dstFolder + "\\" + fileName);
                }
            }
        }

        private void upload_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a CONFIG file.";
            openFileDialog1.Multiselect = true;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "(*.PTC*,*.LUA)|*.PTC;*.LUA";

            if (LiveViewReaderList.SelectedItem == null) return;

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string key = LiveViewReaderList.SelectedItem.ToString();
                ReaderAccessor reader = new ReaderAccessor(getSearchResultFromKey(key).IpAddress);
                reader.OpenFtp();

                foreach (string filePath in openFileDialog1.FileNames)
                {
                    if(Path.GetExtension(filePath)==".PTC")
                    {
                        reader.PutFile((filePath), @"CONFIG\" + Path.GetFileName(filePath));
                    }
                    else if (Path.GetExtension(filePath) == ".LUA")
                    {
                        reader.PutFile((filePath), @"LUA\" + Path.GetFileName(filePath));
                    }
                }
                reader.CloseFtp();
            }          
        }
        #endregion
        #region UICONTROL
        private void searchUIControl(bool enable)
        {
            if (enable)
            {
                Search_Btn.Enabled = false;
                AllReadersSelectBtn.Enabled = false;
                SearchedReaders_ChkLstBx.Enabled = false;
                maintainSelect.Enabled = false;
                LiveViewReaderList.Enabled = false;
                downloadBtn.Enabled = false;
            }
            else
            {
                Search_Btn.Enabled = true;
                AllReadersSelectBtn.Enabled = true;
                SearchedReaders_ChkLstBx.Enabled = true;
                maintainSelect.Enabled = true;
                LiveViewReaderList.Enabled = true;
                downloadBtn.Enabled = true;
            }
        }
        private void maintenanceUIControl(bool enable)
        {
            if (enable)
            {
                Search_Btn.Enabled = false;
                LiveViewReaderList.Enabled = false;
                uploadBtn.Enabled = true;
            }
            else
            {
                Search_Btn.Enabled = true;
                LiveViewReaderList.Enabled = true;
                uploadBtn.Enabled = false;
            }
        }
        #endregion
    }
}
