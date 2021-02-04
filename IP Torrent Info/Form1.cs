using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ByteSizeLib;
using Leaf.xNet;
using Newtonsoft.Json.Linq;

namespace IP_Torrent_Info
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void getLocationDetails(String iip)
        {
            try
            {
                var request = new HttpRequest();
                var response = request.Get("http://ip-api.com/json/"+iip);

                JObject token = JObject.Parse(response.ToString());

                   pictureBox1.Load("https://cdn.ip2location.com/assets/img/flags/"+token.SelectToken("countryCode").ToString().ToLower()+ ".png");
                   labelCountry.Text =  token.SelectToken("country").ToString();
                   labelRegion.Text = token.SelectToken("regionName").ToString();
                   labelCity.Text = token.SelectToken("city").ToString();
                   labelZip.Text = token.SelectToken("zip").ToString();
                   labelTimeZone.Text = token.SelectToken("timezone").ToString();
                   labelIsp.Text = token.SelectToken("isp").ToString();

                

            }
            catch
            {
                
                
            }
        }

        private void getTorrents(String ipAddress)
        {
            getLocationDetails(textBox1.Text);

            String url = "https://api.antitor.com/history/peer/?ip=" +ipAddress+ "&key=3cd6463b477d46b79e9eeec21342e4c7";
            var request1 = new HttpRequest();
            request1.UserAgent = Http.ChromeUserAgent();

            var response1 = request1.Get(url);
            JObject token1 = JObject.Parse(response1.ToString());
            JArray contents = (JArray) token1.SelectToken("contents");

            int id = 1;

            progressBar1.Maximum = contents.Count;
            Cursor.Current = Cursors.WaitCursor;
            foreach (JObject data in contents)
            {


                progressBar1.Value = id;
                id += 1;
                
                var fileSize = ByteSize.FromBytes(double.Parse(data.SelectToken("torrent.size").ToString())).ToString();

                var startTime = DateTime.Parse(data.SelectToken("startDate").ToString()).ToString();
                var endTime = DateTime.Parse(data.SelectToken("endDate").ToString()).ToString();

                String torrentName;

                if (data.SelectToken("name") != null)
                {
                    torrentName = data.SelectToken("name").ToString();
                }

                else
                {
                    torrentName = data.SelectToken("torrent.name").ToString();
                }

                String[] rows = {data.SelectToken("category").ToString(), torrentName, fileSize,startTime,endTime  };
                var listViewItem = new ListViewItem(rows);
                listView1.Items.Add(listViewItem);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            Thread thread = new Thread(()=>getTorrents(textBox1.Text));
            thread.Start();

           
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void buttonDonate_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/paypalme/henryrics");
        }
    }
}
