using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YYOPInspectionClient
{
    public class Util
    {
        //根据文件名删除fileuploadrecord.txt中对应的一行数据
        public static void deleteDirName(string dirName){
            string path = Application.StartupPath + "\\fileuploadrecord.txt";
            string text = "";
            //用一个读出流去读里面的数据

            using (StreamReader reader = new StreamReader(path, Encoding.Default))
            {
                Console.WriteLine("传过来的值:"+dirName);
                //读一行
                string line = ""; ;
                while ((line=reader.ReadLine())!= null)
                {
                    //Console.WriteLine("输出的值"+line.ToString());
                    //如果这一行里面有abe这三个字符，就不加入到text中，如果没有就加入
                    if (line.ToString().Trim()!= dirName)
                    {
                        text += line + "\r\n";
                    }
                    line = reader.ReadLine();
                }
            }
            //Console.WriteLine("最终文本"+text);
            //定义一个写入流，将值写入到里面去 
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write)) {
                fs.Seek(0,SeekOrigin.Begin);
                fs.SetLength(0);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.Default))
                {
                    writer.Write(text);
                }

           }
        }
    }
}
