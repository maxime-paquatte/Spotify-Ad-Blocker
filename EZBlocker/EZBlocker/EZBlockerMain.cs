using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace EZBlocker
{
    public partial class Main : Form
    {
        private bool muted;
        private string lastMessage = "";
        private readonly ToolTip artistTooltip = new ToolTip();
        private readonly string volumeMixerPath = Environment.GetEnvironmentVariable("WINDIR") + @"\System32\SndVol.exe";

        private SpotifyHook hook;

        public Main()
        {
            AudioUtils.SetMute(hook.VolumeControl.Control, false);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            InitializeComponent();
        }


        /**
         * Contains the logic for when to mute Spotify
         **/
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            try {
                if (hook.IsRunning())
                {
                    if (hook.IsAdPlaying())
                    {
                        if (MainTimer.Interval != 1000) MainTimer.Interval = 1000;
                        if (!muted) Mute(true);
                        if (!hook.IsPlaying())
                        {
                            AudioUtils.SendNextTrack(hook.Handle == IntPtr.Zero ? Handle : hook.Handle);
                            Thread.Sleep(500);
                        }

                        string artist = hook.GetArtist();
                        string message = Properties.strings.StatusMuting + " " + Truncate(artist);
                        if (lastMessage != message)
                        {
                            lastMessage = message;
                            StatusLabel.Text = message;
                            artistTooltip.SetToolTip(StatusLabel, artist);
                        }
                    }
                    else if (hook.IsPlaying() && !hook.WindowName.Equals("Spotify Free")) // Normal music
                    {
                        if (muted)
                        {
                            Thread.Sleep(500); // Give extra time for ad to change out
                            Mute(false);
                        }
                        if (MainTimer.Interval != 200) MainTimer.Interval = 200;

                        string artist = hook.GetArtist();
                        string message = Properties.strings.StatusPlaying + " " + Truncate(artist);
                        if (lastMessage != message)
                        {
                            lastMessage = message;
                            StatusLabel.Text = message;
                            artistTooltip.SetToolTip(StatusLabel, artist);
                        }
                    }
                    else if (hook.WindowName.Equals("Spotify Free"))
                    {
                        string message = Properties.strings.StatusPaused;
                        if (lastMessage != message)
                        {
                            lastMessage = message;
                            StatusLabel.Text = message;
                            artistTooltip.SetToolTip(StatusLabel, "");
                        }
                    }
                }
                else
                {
                    if (MainTimer.Interval != 1000) MainTimer.Interval = 1000;
                    string message = Properties.strings.StatusNotFound;
                    if (lastMessage != message)
                    {
                        lastMessage = message;
                        StatusLabel.Text = message;
                        artistTooltip.SetToolTip(StatusLabel, "");
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
       
        /**
         * Mutes/Unmutes Spotify.         
         * i: false = unmute, true = mute
         **/
        private void Mute(bool mute)
        {
            AudioUtils.SetMute(hook.VolumeControl.Control, mute);
            muted = AudioUtils.IsMuted(hook.VolumeControl.Control) ?? false;
        }

        private string Truncate(string name)
        {
            if (name.Length > 10)
            {
                return name.Substring(0, 10) + "...";
            }
            return name;
        }


        private void Main_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpdateSettings) // If true, then first launch of latest EZBlocker
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            string spotifyPath = GetSpotifyPath();
            if (spotifyPath != "")
            {
                Properties.Settings.Default.SpotifyPath = spotifyPath;
                Properties.Settings.Default.Save();
            } else
            {
                spotifyPath = Environment.GetEnvironmentVariable("APPDATA") + @"\Spotify\spotify.exe";
            }

            // Start Spotify and give EZBlocker higher priority
            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; // Windows throttles down when minimized to task tray, so make sure EZBlocker runs smoothly
                if (Properties.Settings.Default.StartSpotify && File.Exists(Properties.Settings.Default.SpotifyPath) && Process.GetProcessesByName("spotify").Length < 1)
                {
                    Process.Start(Properties.Settings.Default.SpotifyPath);
                }
            }
            catch (Exception) {}

           
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (startupKey.GetValue("EZBlocker") != null)
            {
                if (startupKey.GetValue("EZBlocker").ToString() == "\"" + Application.ExecutablePath + "\"")
                {
                    StartupCheckbox.Checked = true;
                    this.WindowState = FormWindowState.Minimized;
                }
                else // Reg value exists, but not in right path
                {
                    startupKey.DeleteValue("EZBlocker");
                }
            }
            SpotifyCheckbox.Checked = Properties.Settings.Default.StartSpotify;
           
            // Start Spotify hook
            hook = new SpotifyHook();
            MainTimer.Enabled = true;
        }

        private string GetSpotifyPath()
        {
            foreach (Process p in Process.GetProcessesByName("spotify"))
            {
                if (p.MainWindowTitle.Length > 1)
                {
                    return p.MainModule.FileName;
                }
            }
            return "";
        }

        private void RestoreFromTray()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }
        
        private void Notify(String message)
        {
            NotifyIcon.ShowBalloonTip(5000, "EZBlocker", message, ToolTipIcon.None);
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.ShowInTaskbar && e.Button == MouseButtons.Left)
            {
                RestoreFromTray();
            }
        }

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                Notify(Properties.strings.HiddenNotify);
            }
        }

        private void SkipAdsCheckbox_Click(object sender, EventArgs e)
        {
            if (!MainTimer.Enabled) return; // Still setting up UI
            if (!IsUserAnAdmin())
            {
                MessageBox.Show(Properties.strings.BlockBannersUAC, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                BlockBannersCheckbox.Checked = !BlockBannersCheckbox.Checked;
                return;
            }
        }

        private void StartupCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!MainTimer.Enabled) return; // Still setting up UI
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (StartupCheckbox.Checked)
            {
                startupKey.SetValue("EZBlocker", "\"" + Application.ExecutablePath + "\"");
            }
            else
            {
                startupKey.DeleteValue("EZBlocker");
            }
        }


        private void SpotifyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!MainTimer.Enabled) return; // Still setting up UI
            Properties.Settings.Default.StartSpotify = SpotifyCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void VolumeMixerButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(volumeMixerPath);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.strings.VolumeMixerOpenError, "EZBlocker");
            }
        }

        private void WebsiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.SpotifyPath))
            {
                MessageBox.Show(Properties.strings.ReportProblemMessageBox.Replace("{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()).Replace("{1}", FileVersionInfo.GetVersionInfo(Properties.Settings.Default.SpotifyPath).FileVersion), "EZBlocker");
                Clipboard.SetText(Properties.strings.ReportProblemClipboard.Replace("{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()).Replace("{1}", FileVersionInfo.GetVersionInfo(Properties.Settings.Default.SpotifyPath).FileVersion));
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!MainTimer.Enabled) return; // Still setting up UI
            if (!Properties.Settings.Default.UserEducated)
            {
                var result = MessageBox.Show(Properties.strings.OnExitMessageBox, "EZBlocker",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Warning);

                e.Cancel = (result == DialogResult.No);

                if (result == DialogResult.Yes)
                {
                    Properties.Settings.Default.UserEducated = true;
                    Properties.Settings.Default.Save();
                }
            }
        }

        [DllImport("shell32.dll")]
        public static extern bool IsUserAnAdmin();
    }
}
