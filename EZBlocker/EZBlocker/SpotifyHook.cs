using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace EZBlocker
{
    class SpotifyHook
    {
        public bool Hooked{ get; private set; }
        public Process Spotify { get; private set; }
        private HashSet<int> Children;
        public AudioUtils.VolumeControl VolumeControl { get; private set; }
        public string WindowName { get; private set; }
        public IntPtr Handle { get; private set; }

        private float peak = 0f;
        private float lastPeak = 0f;


        public bool Check()
        {
            if (!Hooked) //if never or no more hooked, start hook
                Hooked = HookSpotify();
            else if (!IsRunning()) // if hooked and spotify no more running, clean hook
                CleanHook();
            else // else inspect spotify process
            {
                WindowName = Spotify.HasExited ? string.Empty : Spotify.MainWindowTitle;
                Handle = Spotify.HasExited ? IntPtr.Zero : Spotify.MainWindowHandle;
                if (VolumeControl == null)
                {
                    VolumeControl = AudioUtils.GetVolumeControl(Children);
                }
                else
                {
                    lastPeak = peak;
                    peak = AudioUtils.GetPeakVolume(VolumeControl.Control);
                }
                return true;
            }

            return false;
        }

        public bool IsPlaying()
        {
            return peak > 0 && lastPeak > 0;
        }

        public bool IsAdPlaying()
        {
            if ((WindowName.Equals("Advertisement") || !WindowName.Contains(" - ")) && !WindowName.Equals("") && !WindowName.Equals("Drag") && IsPlaying())
            {
                Debug.WriteLine("Ad: " + lastPeak + " " + peak);
                return true;
            }
            return false;
        }

        public bool IsRunning()
        {
            if (Spotify == null)
                return false;

            Spotify.Refresh();
            return !Spotify.HasExited;
        }

        public string GetArtist()
        {
            if (!IsPlaying()) return "";

            if (WindowName.Contains(" - "))
                return WindowName.Split(new[] { " - " }, StringSplitOptions.None)[0];
            return WindowName;
        }


        private bool HookSpotify()
        {
            Children = new HashSet<int>();

            // Try hooking through window title
            foreach (Process p in Process.GetProcessesByName("spotify"))
            {
                Children.Add(p.Id);
                Spotify = p;
                if (p.MainWindowTitle.Length > 1)
                {
                    return true;
                }
            }

            // Try hooking through audio device
            VolumeControl = AudioUtils.GetVolumeControl(Children);
            if (VolumeControl != null)
            {
                Spotify = Process.GetProcessById(VolumeControl.ProcessId);
                return true;
            }

            return false;
        }


        public void CleanHook()
        {
            Spotify = null;
            WindowName = "";
            Handle = IntPtr.Zero;
            if (VolumeControl != null) 
                Marshal.ReleaseComObject(VolumeControl.Control);
            VolumeControl = null;
            Hooked = false;
        }


        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }
}
