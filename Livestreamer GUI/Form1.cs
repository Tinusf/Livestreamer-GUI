using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net;
//addet referance system.net.extentions
using System.Web.Script.Serialization;
//TODO:
//idk what more to do with this

namespace Livestreamer_GUI
{
    public partial class Form1 : Form
    {
        bool streamListChanged = false;
        float fontSize = 18f;
        public Form1()
        {
            InitializeComponent();
            createFiles();
            readConfig();
            addListToCB();

            button6.SendToBack();
            button7.SendToBack();
            comboBox4.SendToBack();
            label2.SendToBack();
            checkBox1.SendToBack();
            //one day i will figure out how to make this stuff go behind the panel without these lines. :D
        }

        private void createFiles()
        {
            string config = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/lsguioptions";
            string streamList = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
            FileInfo fi = new FileInfo(config);
            FileInfo fi2 = new FileInfo(streamList);
            if (!fi.Exists)
            {
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.Write("18:False");
                }
                MessageBox.Show("Hey! Thank you for trying out my Livestreamer GUI.\nMake sure you have livestreamer installed, if you don't\nGo to livestreamer.io to download it.");
            }
            if (!fi2.Exists)
            {
                using (StreamWriter sw = fi2.AppendText())
                {
                    sw.Write("TinyTheBoss"); //shameless plug
                    sw.WriteLine("");
                }
            }
        }

        private void readConfig()
        {
            string config = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/lsguioptions";
            using (StreamReader sr = File.OpenText(config))
            {
                string fromFile = sr.ReadLine();
                string[] split = fromFile.Split(':');
                fontSize = float.Parse(split[0]);
                comboBox4.Text = split[0];
                checkBox1.Checked = bool.Parse(split[1]);
            }
        }

        private void fontChange()
        {
            if (comboBox4.Text!="")
            {
                fontSize = float.Parse(comboBox4.Text);
                comboBox2.Font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            }
        }

        private void writeConfig()
        {
            string config = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/lsguioptions";
            File.WriteAllText(config, string.Empty);
            FileInfo fi = new FileInfo(config);
            using (StreamWriter sw = fi.AppendText())
            {
                sw.WriteLine(fontSize +":" + checkBox1.Checked);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string livestreamer = "/C livestreamer " + comboBox3.Text + comboBox2.Text + " " + comboBox1.Text;
            Process.Start("cmd", livestreamer);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData) //enter for å starte
            {
                case Keys.F1: { button1.PerformClick(); break; }
                case Keys.F2: { button20.PerformClick(); break; }
            }
        }
        
        private void button20_Click(object sender, EventArgs e)
        {
            string lschat = comboBox3.Text + comboBox2.Text + "/chat";
            if (comboBox3.Text.Contains("twitch"))
            {
                System.Diagnostics.Process.Start(lschat);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string config = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/livestreamerrc";
            Process.Start("notepad.exe", config);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string lsprofile = comboBox3.Text + comboBox2.Text + "/profile";
            if (comboBox3.Text.Contains("twitch"))
            {
                System.Diagnostics.Process.Start(lsprofile);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://livestreamer.io/");
        }

        
        private void addListToCB()
        {
            comboBox2.Items.Clear();
            int streamCount = 0;
            string streamlist = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
            var streams = File.ReadAllLines(streamlist);
            foreach (var stream in streams)
            {
                if (streamCount ==0)
                {
                    comboBox2.Text = stream;
                }
                comboBox2.Items.Insert(streamCount, stream);
                streamCount++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.Dock = DockStyle.Fill;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            textBox1.Text = "";
            if (streamListChanged)
            {
                addListToCB();
                streamListChanged = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string streamlist = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
            Process.Start("notepad.exe", streamlist);
            streamListChanged = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text!= "")
            {
                string streamlist = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
                FileInfo fi = new FileInfo(streamlist);
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine(textBox1.Text);
                }
                streamListChanged = true;
                textBox1.Text = "";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string streamlist = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
            string[] lines = File.ReadAllLines(streamlist);
            string lineToRemove = comboBox2.Text;
            lines = lines.Where(val => val != lineToRemove).ToArray();
            File.WriteAllLines(streamlist, lines);
            addListToCB();
            if (comboBox2.Items.Count == 0)
            {
                comboBox2.Text = "";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            writeConfig();
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            fontChange();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string username = Microsoft.VisualBasic.Interaction.InputBox("You are now adding the channels that you are following.\nWhat is your twitch name?", "Twith name", "Twitch Name");
            if (username != "")
            {
                //https://github.com/justintv/Twitch-API/blob/master/v3_resources/follows.md#get-usersuserfollowschannels
                //https://api.twitch.tv/kraken/users/tinytheboss/follows/channels?limit=100&sortby=last_broadcast
                //https://msdn.microsoft.com/en-us/library/cc197957%28v=vs.95%29.aspx
                string url = "https://api.twitch.tv/kraken/users/" + username + "/follows/channels?limit=100&sortby=last_broadcast";

                using (WebClient wc = new WebClient())
                {
                    string json = wc.DownloadString(url);
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    RootObject RootObject_ = ser.Deserialize<RootObject>(json);
                    string test = RootObject_.follows[2].channel.display_name;

                    int currentAmtInCb = comboBox2.Items.Count;
                    int totStreams = RootObject_._total;
                    int totStreamsAlreadyInCB = 0;
                    if (totStreams>99)
                    {
                        totStreams = 99;
                    }

                    int streamCount = 0;
                    for (streamCount = 0; streamCount < totStreams; streamCount++)
                    {
                        string stream = RootObject_.follows[streamCount].channel.display_name;

                        if (comboBox2.Items.Contains(stream))
                        {
                            totStreamsAlreadyInCB++;
                        }
                        else
                        {
                            if (streamCount == 0)
                            {
                                comboBox2.Text = stream;
                            }
                            comboBox2.Items.Insert(streamCount + currentAmtInCb - totStreamsAlreadyInCB, stream);

                            string streamlist = Environment.ExpandEnvironmentVariables("%appdata%") + "/livestreamer/streams";
                            FileInfo fi = new FileInfo(streamlist);
                            using (StreamWriter sw = fi.AppendText())
                            {
                                sw.WriteLine(stream);
                            }
                        }
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://livestreamer.io/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tinusf");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Sorted = checkBox1.Checked;
        }
    }

    //created the classes with: http://json2csharp.com/

    public class Links
    {
        public string self { get; set; }
    }

    public class Links2
    {
        public string self { get; set; }
        public string follows { get; set; }
        public string commercial { get; set; }
        public string stream_key { get; set; }
        public string chat { get; set; }
        public string features { get; set; }
        public string subscriptions { get; set; }
        public string editors { get; set; }
        public string videos { get; set; }
        public string teams { get; set; }
    }

    public class Channel
    {
        public Links2 _links { get; set; }
        public object background { get; set; }
        public object banner { get; set; }
        public string broadcaster_language { get; set; }
        public string display_name { get; set; }
        public string game { get; set; }
        public string logo { get; set; }
        public bool? mature { get; set; }
        public string status { get; set; }
        public bool partner { get; set; }
        public string url { get; set; }
        public string video_banner { get; set; }
        public int _id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public object delay { get; set; }
        public int followers { get; set; }
        public string profile_banner { get; set; }
        public string profile_banner_background_color { get; set; }
        public int views { get; set; }
        public string language { get; set; }
    }

    public class Follow
    {
        public string created_at { get; set; }
        public Links _links { get; set; }
        public bool notifications { get; set; }
        public Channel channel { get; set; }
    }

    public class Links3
    {
        public string self { get; set; }
        public string next { get; set; }
    }

    public class RootObject
    {
        public List<Follow> follows { get; set; }
        public int _total { get; set; }
        public Links3 _links { get; set; }
    }
}