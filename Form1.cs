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

        private void LoadSetting()
        {
            var path = Application.StartupPath;
            var ini = new IniFile(path+"\\settings.ini");

            var section = "config";
            CLIENT_ID = ini.GetString(section, "DiscordClientId","");
            SleepImageKey = ini.GetString(section, "SleepImageKey", "");
            PlayingImageKey = ini.GetString(section, "PlayingImageKey", "");
            LastFmAPIKey = ini.GetString(section, "LastFmAPIKey", "");
            LastFmUserName = ini.GetString(section, "LastFmUserName", "");

            if(CLIENT_ID=="" || LastFmAPIKey=="" || LastFmUserName == "")
            {
                MessageBox.Show("settings.iniの設定が不正です。終了します。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                IsExit = true;
                Environment.Exit(0);
            }
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

            client.OnReady += (sender, e) =>
            {
                //Debug.WriteLine("Received Ready from user {0}", e.User.Username);
            };
            client.OnPresenceUpdate += (sender, e) =>
            {
                //Debug.WriteLine("Received Update {0}",e.Presence.Details);
            };
            client.OnJoinRequested += (sender, e) =>
            {
                Debug.WriteLine("##JOINREQUEST {0}", e.User);
                client.Respond(e, true);
            };

            client.RegisterUriScheme(executable: $"rundll32.exe url.dll,FileProtocolHandler \"https://www.google.co.jp/\"");

            client.Initialize();
            client.Subscribe(EventType.JoinRequest);
            client.SetPresence(sleepRPC);
        }

        public Form1()
        {
            LoadSetting();

            //タスクバーから消す
            this.ShowInTaskbar = false;

            //タスクトレイに常駐
            NotifyIcon icon = new NotifyIcon();
            icon.Icon = Resource.rpclogo;
            icon.Visible = true;
            icon.Text = "DiscordRPCTool";
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = "&Exit";
            menuItem.Click += new EventHandler(Close_Click);
            menu.Items.Add(menuItem);
            icon.ContextMenuStrip = menu;

            InitializeComponent();



            InitDiscord();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        ~Form1()
        {
            if (!IsExit)
            {
                client.Dispose();

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            //truck
            var track = GetCurrentTrack();
            if (track.IsNowPlaying)
            {
                if (track.Song != LastSongName)
                {
                    //uri
                    var now = DateTime.Now.ToString();
                    client.RegisterUriScheme(executable: $"rundll32.exe url.dll,FileProtocolHandler {track.URL}".Replace("%","%%").Replace("/music/","/ja/music/"));
                    Debug.WriteLine($"{track.URL}");

                    var h = track.Album == "" ? "" : " - ";
                    var state = $"{track.Artist}{h}{track.Album}";
                    client.SetPresence(new RichPresence()
                    {
                        State = state,
                        Details = $"🎵{track.Song}",
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
                        Secrets = new Secrets()
                        {
                            JoinSecret = "SECRET"+now,
                        },
                        Party = new Party()
                        {
                            ID = "ID"+now,
                            Max = 127,
                            Size = 1,
                        }
                    });
                }
            }
            else
            {
                client.SetPresence(sleepRPC);
            }
            LastSongName = track.Song;
        }

        private Status GetCurrentTrack()
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
            using(var client = new HttpClient())
            {
                var response = client.GetAsync($"http://ws.audioscrobbler.com/2.0/?{new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result}").Result;
                result = response.Content.ReadAsStringAsync().Result;
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

    /// <summary>
    /// Ini ファイルの読み書きを扱うクラスです。
    /// https://knooto.info/csharp-ini/ を参考にさせて頂きました。
    /// </summary>
    public class IniFile
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        /// <summary>
        /// Ini ファイルのファイルパスを取得、設定します。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="filePath">Ini ファイルのファイルパス</param>
        public IniFile(string filePath)
        {
            FilePath = filePath;
        }
        /// <summary>
        /// Ini ファイルから文字列を取得します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">項目名</param>
        /// <param name="defaultValue">値が取得できない場合の初期値</param>
        /// <returns></returns>
        public string GetString(string section, string key, string defaultValue = "")
        {
            var sb = new StringBuilder(1024);
            var r = GetPrivateProfileString(section, key, defaultValue, sb, (uint)sb.Capacity, FilePath);
            return sb.ToString();
        }
        /// <summary>
        /// Ini ファイルに文字列を書き込みます。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">項目名</param>
        /// <param name="value">書き込む値</param>
        /// <returns></returns>
        public bool WriteString(string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, FilePath);
        }
    }
}
