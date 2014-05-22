using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

class StringWriterUTF8 : System.IO.StringWriter
{
    public override Encoding Encoding
    {
        get { return Encoding.UTF8; }
    }
}
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBoxUserID.Text = Properties.Settings.Default.UserID;
            textBoxPasscode.Text = Properties.Settings.Default.Passcode;
            textBoxClientID.Text = Properties.Settings.Default.ClientID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxClientID.Text.Length > 0)
            {
                client_id = textBoxClientID.Text;
            }
            else
            {
                string username = textBoxUserID.Text;
                string passcode = textBoxPasscode.Text;


                System.Net.HttpWebRequest webreq = (System.Net.HttpWebRequest)
                    System.Net.WebRequest.Create("https://sync2ch.com/api/auth2");

                //認証の設定
                webreq.Credentials =
                    new System.Net.NetworkCredential(username, passcode);

                //HttpWebResponseの取得
                System.Net.HttpWebResponse webres =
                    (System.Net.HttpWebResponse)webreq.GetResponse();

                string[] values = webres.Headers.GetValues("Sync2ch-Client-ID");
                textBoxClientID.Text = values[0];
                client_id = values[0];
                //受信して表示
                System.IO.Stream st = webres.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(st);
                Console.WriteLine(sr.ReadToEnd());
                //閉じる
                sr.Close();
                st.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.UserID = textBoxUserID.Text;
            Properties.Settings.Default.Passcode = textBoxPasscode.Text;
            Properties.Settings.Default.ClientID = textBoxClientID.Text;
            Properties.Settings.Default.Save();
        }

        int sync_number = 0;
        string client_id = "";
        private void buttonMakeXML_Click(object sender, EventArgs e)
        {
            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement sync2ch_request = CreateRequest(document, sync_number, client_id);

            document.AppendChild(declaration);
            document.AppendChild(sync2ch_request);

            XmlElement thread_group = document.CreateElement("thread_group");
            thread_group.SetAttribute("category", "favorite");
            //thread_group.SetAttribute("category", "open");
            //thread_group.SetAttribute("category", "favorite");
            //thread_group.SetAttribute("category", "history");
            //thread_group.SetAttribute("category", "post_history");

            XmlElement dir = document.CreateElement("dir");
            dir.SetAttribute("name", "フォルダ1");

            thread_group.AppendChild(dir);

            XmlElement th = CreateThread(document);
            dir.AppendChild(th);

            sync2ch_request.AppendChild(thread_group);

            // ファイルに保存する
            //document.Save("sample.xml");
            //textBoxXML.Text = document.OuterXml;
            // StringWriterをつくる
            StringWriter writer = new StringWriter();
            // StringWriterに整形済みXMLを書き出し
            document.Save(writer);
            // 文字列として書き出し
            textBoxXML.Text = writer.ToString();
            // StringWriterを閉じる
            writer.Close();
            sync_number++;
        }
        /// <summary>
        /// 3.2 ルート要素
        ///ルート要素<sync2ch_request>には以下の属性を指定します。

        ///sync_number(必須) ・・・ 同期番号(初回同期時には0）を指定
        ///client_id ・・・ クライアント認証プロセスによって得たクライアントID
        ///client_name ・・・ クライアント名
        ///client_version ・・・ クライアントバージョン
        ///os ・・・ OSを表す文字列
        ///device ・・・ 機種名（スマートフォン等の機種名を指定します。)
        ///sync_rl ・・・ 同期するレス番号リストのカテゴリ名をコンマ区切りで指定します(rlはRes Listの略)。
        /// </summary>
        /// <param name="document"></param>
        /// <param name="syncNumber"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        XmlElement CreateRequest(XmlDocument document, int syncNumber, string clientID)
        {
            XmlElement sync2ch_request = document.CreateElement("sync2ch_request");
            sync2ch_request.SetAttribute("sync_number", syncNumber.ToString());
            sync2ch_request.SetAttribute("client_id", clientID);
            sync2ch_request.SetAttribute("sync_rl", "post,re");
            sync2ch_request.SetAttribute("client_version", "0.0.1");
            sync2ch_request.SetAttribute("client_name", "Thousand Sync");
            sync2ch_request.SetAttribute("os", "Windows 7");
            sync2ch_request.SetAttribute("device", "PC");
            return sync2ch_request;
        }
        /// <summary>
        /// <th> ・・・スレッド 属性: url, title, read, now, count, rt, pt, rl_post (rl_{CATEGORY})</th>
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        XmlElement CreateThread(XmlDocument document)
        {
            XmlElement th = document.CreateElement("th");
            th.SetAttribute("url", "http://toro.2ch.net/read.cgi/test/sampleboard/1389394832/");
            th.SetAttribute("title", "サンプルスレッド");
            th.SetAttribute("read", "32");
            th.SetAttribute("now", "32");
            th.SetAttribute("count", "523");
            return th;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            string username = textBoxUserID.Text;
            string passcode = textBoxPasscode.Text;

            XmlDocument document = new XmlDocument();

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);  // XML宣言
            XmlElement sync2ch_request = CreateRequest(document, sync_number, client_id);
            document.AppendChild(declaration);
            document.AppendChild(sync2ch_request);

            XmlElement thread_group = document.CreateElement("thread_group");
            thread_group.SetAttribute("category", "favorite");
            XmlElement dir = document.CreateElement("dir");
            dir.SetAttribute("name", "フォルダ1");
            thread_group.AppendChild(dir);
            XmlElement th = CreateThread(document);
            dir.AppendChild(th);

            sync2ch_request.AppendChild(thread_group);

            StringWriterUTF8 writer = new StringWriterUTF8();
            document.Save(writer);
            string xmlString = writer.ToString();
            writer.Close();
            byte[] postDataBytes = Encoding.UTF8.GetBytes(xmlString);

            System.Net.HttpWebRequest webreq = (System.Net.HttpWebRequest)
                System.Net.WebRequest.Create("http://sync2ch.com/api/sync2");

            //認証の設定
            webreq.Credentials =
                new System.Net.NetworkCredential(username, passcode);
            
            System.Net.ServicePointManager.Expect100Continue = false;

            webreq.Method = "POST";
            webreq.ContentType = "application/x-www-form-urlencoded";
            webreq.ContentLength = postDataBytes.Length;
            //HttpWebResponseの取得
            Stream reqStream = webreq.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            System.Net.HttpWebResponse webres =
                (System.Net.HttpWebResponse)webreq.GetResponse();

            //受信して表示
            System.IO.Stream st = webres.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(st);
            String result = sr.ReadToEnd();
            textBoxXML.Text = result;
            //閉じる
            sr.Close();
            st.Close();
        }
    }
}
