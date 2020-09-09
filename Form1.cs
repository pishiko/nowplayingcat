using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Runtime.InteropServices;

namespace DiscordRPCTool
{
    enum CATIMAGE
    {
        black=0,
        headphone=1,
        dance=2
    }

    public partial class Form1 : Form
    {
        //settings
        string CLIENT_ID;
        string SleepImageKey;
        string PlayingImageKey;
        string LastFmAPIKey;
        string LastFmUserName;

        string SleepState = "zzz...";
        string SleepDetails = "in a dream";
        string SleepImageText = "zzz...";
        string PlayingImageText = "nyan";

        public DiscordRpcClient client;
        public string LastSongName = "";
        private RichPresence sleepRPC;
        private bool IsExit = false;

        private bool LoadSetting()
        {
            var path = Application.StartupPath;

            CLIENT_ID = "707972898274541629";
            SleepImageKey = "cat_zzz";
            LastFmAPIKey = Properties.Settings.Default.LastFmAPIKey;
            LastFmUserName = Properties.Settings.Default.LastFmUserName;

            switch (Properties.Settings.Default.CatImageIndex)
            {
                case (int)CATIMAGE.headphone:
                    PlayingImageKey = "cat_music";
                    break;

                case (int)CATIMAGE.dance:
                    PlayingImageKey = "cat_dance";
                    break;

                default:
                    PlayingImageKey = "cat_dev";
                    break;
            }

            if (LastFmAPIKey == "" || LastFmUserName == "")
            {

                var result = MessageBox.Show("Last.fm APIキー，ユーザーネームの設定をしてください。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                if(result == DialogResult.OK)
                {
                    if (FormSetting == null || FormSetting.IsDisposed)
                    {
                        FormSetting = new Form2();
                        FormSetting.Show();
                    }
                    else
                    {
                        FormSetting.Activate();
                    }
                    return false;

                }
                else
                {
                    IsExit = true;
                    Environment.Exit(0);

                }
            }
            return true;
        }

        private void InitDiscord()
        {
            sleepRPC = new RichPresence()
            {
                State = SleepState,
                Details = SleepDetails,
                Assets = new Assets()
                {
                    LargeImageKey = SleepImageKey, // Larger Image Asset Key
                    LargeImageText = SleepImageText, // Large Image Tooltip

                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.Now,
                },
                //Secrets = new Secrets()
                //{
                //    SpectateSecret = "nyan",
                //}
            };

            client = new DiscordRpcClient(CLIENT_ID);

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //client.OnReady += (sender, e) =>
            //{
            //    //Debug.WriteLine("Received Ready from user {0}", e.User.Username);
            //};
            //client.OnPresenceUpdate += (sender, e) =>
            //{
            //    //Debug.WriteLine("Received Update {0}",e.Presence.Details);
            //};
            //client.OnJoinRequested += (sender, e) =>
            //{
            //    Debug.WriteLine("##JOINREQUEST {0}", e.User);
            //    client.Respond(e, true);
            //};

            //client.RegisterUriScheme(executable: $"rundll32.exe url.dll,FileProtocolHandler \"https://www.google.co.jp/\"");

            client.Initialize();
            //client.Subscribe(EventType.JoinRequest);
            client.SetPresence(sleepRPC);
        }

        private Form2 FormSetting = null;
        public Form1()
        {
            var isLoaded = LoadSetting();
            //タスクバーから消す
            this.ShowInTaskbar = false;

            //タスクトレイに常駐
            NotifyIcon icon = new NotifyIcon();
            icon.Icon = Resource.rpclogo;
            icon.Visible = true;
            icon.Text = "Discord_Lastfm";
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem menuItemExit = new ToolStripMenuItem();
            menuItemExit.Text = "&Exit";
            menuItemExit.Click += new EventHandler(Close_Click);
            menu.Items.Add(menuItemExit);

            ToolStripMenuItem menuItemSetting = new ToolStripMenuItem();
            menuItemSetting.Text = "&Setting";
            menuItemSetting.Click += new EventHandler(Setting_Click);
            menu.Items.Add(menuItemSetting);

            icon.ContextMenuStrip = menu;

            InitializeComponent();


            if (isLoaded)
            {
                InitDiscord();
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Enabled = true;

            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        ~Form1()
        {
            if (!IsExit && client!=null)
            {
                client.Dispose();

            }
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            if (FormSetting == null || FormSetting.IsDisposed)
            {
                FormSetting = new Form2();
                FormSetting.Show();
            }
            else
            {
                FormSetting.Activate();
            }
        }

        public static void SetStartUp()
        {
            //Runキーを開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);
            //値の名前に製品名、値のデータに実行ファイルのパスを指定し、書き込む
            regkey.SetValue(Application.ProductName, Application.ExecutablePath);
            //閉じる
            regkey.Close();
        }

        public static void RemoveStartUp()
        {
            //Runキーを開く
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);

            regkey.DeleteValue(Application.ProductName,false);

            //閉じる
            regkey.Close();
        }
            private async void timer1_Tick(object sender, EventArgs e)
        {
            
            //truck
            var track = await GetCurrentTrack();
            if (track.IsNowPlaying)
            {
                if (track.Song != LastSongName)
                {
                    //uri
                    var now = DateTime.Now.ToString();
                    //client.RegisterUriScheme(executable: $"rundll32.exe url.dll,FileProtocolHandler {track.URL}".Replace("%","%%").Replace("/music/","/ja/music/"));
                    Debug.WriteLine($"{track.URL}");

                    var h = track.Album == "" ? "" : " - ";
                    var state = $"{track.Artist}{h}{track.Album}";
                    client.SetPresence(new RichPresence()
                    {
                        State = CutString(128,state),
                        Details = CutString(128,$"🎵{track.Song}"),
                        Assets = new Assets()
                        {
                            LargeImageKey = PlayingImageKey, // Larger Image Asset Key
                            LargeImageText = PlayingImageText, // Large Image Tooltip
                                                     //SmallImageKey = "cat_dev", // Small Image Asset Key
                                                     //SmallText = "foo smallImageText", // Small Image Tooltip
                        },
                        //Timestamps = new Timestamps()
                        //,
                        //    Start = track.UTime,
                        //},
                        //Secrets = new Secrets()
                        //{
                        //    JoinSecret = "SECRET"+now,
                        //},
                        Party = new Party()
                        {
                            ID = "ID"+now,
                            Max = 1,
                            Size = 1,
                        }
                    });
                }
            }
            else
            {
                //client.SetPresence(sleepRPC);
                client.ClearPresence();
            }
            LastSongName = track.Song;
        }

        private async Task<Status> GetCurrentTrack()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "method","user.getrecenttracks"},
                {"user",LastFmUserName },
                {"api_key",LastFmAPIKey },
                {"limit","1" },
                {"extended","1" },
                {"format","json" }
            };

            string result;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"http://ws.audioscrobbler.com/2.0/?{new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result}");
                    //.Result;
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException)
            {
                var st = new Status();
                st.IsNowPlaying = false;
                return st;
            }
            dynamic json = JObject.Parse(result.Replace("#text","text").Replace("@attr","attr"));
            //Debug.WriteLine(json);
            var ret = new Status();
            try
            {
                var track = json.recenttracks.track[0];
                ret.IsNowPlaying = track.attr==null?false:true;
                ret.Song = track.name;
                ret.Artist = track.artist.name;
                ret.Album = track.album.text;
                ret.UTime = DateTime.Now;
                ret.URL = track.url;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                Debug.WriteLine("#RuntimeBinderException");
                ret.IsNowPlaying = false;
            }
            return ret;
        }

        private string CutString(int size,string text)
        {
            Encoding e = Encoding.GetEncoding("UTF-8");
            return new String(text.TakeWhile((c, i) => e.GetByteCount(text.Substring(0, i + 1)) <= size).ToArray());
            
        }
    }

    public class Status
    {
        public bool IsNowPlaying { get; set; }
        public string Song { get; set; }
        public string Artist { get; set; }
        public DateTime UTime { get; set; }
        public string Album { get; set; }
        public string URL { get; set; }
    }

}
