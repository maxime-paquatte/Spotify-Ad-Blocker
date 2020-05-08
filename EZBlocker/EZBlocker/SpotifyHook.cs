using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EZBlocker
{
    public enum SpotifyState
    {
        NonHooked = 0,
        Pause,
        Playing,
        PlayingAd,
        Unknown = 99,
    }

    internal interface ISpotifyHook
    {
        bool IsPlaying();
        SpotifyState Check();
        void Mute(bool mute);
        bool IsMuted { get; }

        void NextTrack();
        void PlayPause();
    }

    class SpotifyHookMoq : ISpotifyHook
    {
        public SpotifyState State { get; set; }
        public bool IsMuted { get; set; }

        public SpotifyHookMoq(Form f)
        {
            f.DoubleClick += F_DoubleClick;
            f.KeyPress += F_KeyPress;
        }

        private void F_DoubleClick(object sender, EventArgs e)
        {
            State = State == SpotifyState.PlayingAd ? SpotifyState.Playing : SpotifyState.PlayingAd;
        }

        private void F_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar ==  'd')
            {
                State = State == SpotifyState.PlayingAd ? SpotifyState.Playing : SpotifyState.PlayingAd;
            }
        }

        public bool IsPlaying()
        {
            return State == SpotifyState.PlayingAd || State == SpotifyState.Playing;
        }

        public SpotifyState Check()
        {
            return State;
        }

        public void Mute(bool mute)
        {
            IsMuted = mute;
        }

        public void NextTrack()
        {
            
        }

        public void PlayPause()
        {
            State = State == SpotifyState.Pause ? SpotifyState.Playing : SpotifyState.Pause;
        }


        public void KeyPress(Keys c)
        {

        }
    }

    class SpotifyHook : ISpotifyHook
    {
        public bool Hooked{ get; private set; }
        public Process Spotify { get; private set; }
        private HashSet<int> children;
        private AudioUtils.VolumeControl volumeControl;
        private IntPtr handle;

        private float peak = 0f;
        private float lastPeak = 0f;


        public SpotifyState Check()
        {
            if (!Hooked) //if never or no more hooked, start hook
                Hooked = HookSpotify();
            else if (!IsRunning()) // if hooked and spotify no more running, clean hook
                CleanHook();
            else // else inspect spotify process
            {
                if (volumeControl != null)
                {
                    lastPeak = peak;
                    peak = AudioUtils.GetPeakVolume(volumeControl.Control);
                }
                else volumeControl = AudioUtils.GetVolumeControl(children);

                if (Spotify.HasExited)
                {
                    handle = IntPtr.Zero;
                    return SpotifyState.NonHooked;
                }

                handle = Spotify.MainWindowHandle;
                var windowName = Spotify.MainWindowTitle;

                if (windowName.Equals("Spotify Free"))
                    return SpotifyState.Pause;
                if ((windowName.Equals("Advertisement") || windowName.Equals("Spotify")) && IsPlaying())
                    return SpotifyState.PlayingAd;
                if (IsPlaying())
                    return SpotifyState.Playing;

            }

            return SpotifyState.NonHooked;
        }

        public bool IsPlaying()
        {
            return peak > 0 && lastPeak > 0;
        }


        public bool IsRunning()
        {
            if (Spotify == null)
                return false;

            Spotify.Refresh();
            return !Spotify.HasExited;
        }

        private bool HookSpotify()
        {
            children = new HashSet<int>();

            // Try hooking through window title
            foreach (Process p in Process.GetProcessesByName("spotify"))
            {
                children.Add(p.Id);
                Spotify = p;
                if (p.MainWindowTitle.Length > 1)
                {
                    return true;
                }
            }

            // Try hooking through audio device
            volumeControl = AudioUtils.GetVolumeControl(children);
            if (volumeControl != null)
            {
                Spotify = Process.GetProcessById(volumeControl.ProcessId);
                return true;
            }

            return false;
        }

        public void Mute(bool mute)
        {
            AudioUtils.SetMute(volumeControl.Control, mute);
        }

        public bool IsMuted => AudioUtils.IsMuted(volumeControl?.Control) ?? false;
       

        public void CleanHook()
        {
            Spotify = null;
            handle = IntPtr.Zero;
            if (volumeControl != null) 
                Marshal.ReleaseComObject(volumeControl.Control);
            volumeControl = null;
            Hooked = false;
        }


        public void NextTrack()
        {
            AudioUtils.SendNextTrack(handle);
        }

        public void PlayPause()
        {
            AudioUtils.SendPlayPause(handle);
        }


        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }
}
