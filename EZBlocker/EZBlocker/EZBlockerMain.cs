using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace EZBlocker
{
    public enum LocalTrackState
    {
        Stop,
        Play,
        End
    }

    public partial class Main : Form
    {
        private readonly string volumeMixerPath = Environment.GetEnvironmentVariable("WINDIR") + @"\System32\SndVol.exe";
        private readonly ISpotifyHook hook;

        private readonly Random random = new Random();
        private LocalTrackState localTrackState;
        private string[] tracks;

        public Main()
        {
            // Start Spotify hook
            hook = new SpotifyHook();//new SpotifyHookMoq(this);// 

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            
            InitializeComponent();
        }

        private void LoadLocalTracks()
        {
            var p = Properties.Settings.Default.LocalTracksPath;
            LocalTrackPathTb.Text = Properties.Settings.Default.LocalTracksPath;
            if (!string.IsNullOrEmpty(p) && Directory.Exists(p))
                tracks = Directory.GetFiles(Properties.Settings.Default.LocalTracksPath);
            else tracks = new string[0];
            LocalTracksCountLbl.Text = "(" + tracks.Length + ")";
        }

        private void PlayLocalTrack()
        {
            if (tracks.Length > 0)
            {
                var t = tracks[random.Next(tracks.Length)];
                mediaPlayer.URL = t;
                mediaPlayer.Ctlcontrols.play();
                localTrackState = LocalTrackState.Play;
            }
            else
                localTrackState = LocalTrackState.End;
        }
       

        /**
         * Contains the logic for when to mute Spotify
         **/
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            var s = hook.Check();
            if (s != SpotifyState.NonHooked)
            {
                if (MainTimer.Interval != 200) MainTimer.Interval = 200;

                if (s == SpotifyState.PlayingAd && localTrackState == LocalTrackState.Stop)
                {
                    if (!hook.IsMuted) hook.Mute(true);
                    PlayLocalTrack();

                    if (!hook.IsPlaying()) //if Spotify is not playing Ad, play next
                    {
                        hook.NextTrack();
                        Thread.Sleep(500);
                    }

                }
                else if (s == SpotifyState.Playing && localTrackState == LocalTrackState.Play) // end of ad, pause spotify while local trac end
                {
                    hook.PlayPause();
                }
                else if (s == SpotifyState.Pause && localTrackState == LocalTrackState.End) // local track end, resume spotify
                {
                    hook.PlayPause();
                    localTrackState = LocalTrackState.Stop;
                }
                else if (s == SpotifyState.Playing) // Normal music
                {
                    if (hook.IsMuted)
                    {
                        Thread.Sleep(500); // Give extra time for ad to change out
                        hook.Mute(false);
                    }

                    StatusLabel.Text = Properties.strings.StatusPlaying;
                }
                else if (s == SpotifyState.Pause) // Normal music
                    StatusLabel.Text = Properties.strings.StatusPaused;
                else
                    StatusLabel.Text = Properties.strings.StatusUnknown;
            }
            else
            {
                if (MainTimer.Interval != 1000) MainTimer.Interval = 1000;
                StatusLabel.Text = Properties.strings.StatusNotFound;
            }
        }


        private void Main_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpdateSettings) // If true, then first launch of latest EZBlocker
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            
            LoadLocalTracks();

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
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void MediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8)//MediaEnded
                localTrackState = LocalTrackState.End;
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

            Properties.Settings.Default.Save();
        }
        
        private void LocalTrackPathBtn_Click(object sender, EventArgs e)
        {
            var r = LocalTrackPathDialog.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                Properties.Settings.Default.LocalTracksPath = LocalTrackPathDialog.SelectedPath;
                LoadLocalTracks();
            }
        }
    }
}
