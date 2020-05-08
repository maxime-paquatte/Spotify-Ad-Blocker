namespace EZBlocker
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.websiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VolumeMixerButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.StartupCheckbox = new System.Windows.Forms.CheckBox();
            this.SpotifyCheckbox = new System.Windows.Forms.CheckBox();
            this.mediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.LocalTrackPathTb = new System.Windows.Forms.TextBox();
            this.LocalTrackPathBtn = new System.Windows.Forms.Button();
            this.LocalTrackPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.LocalTracksCountLbl = new System.Windows.Forms.Label();
            this.NotifyIconContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTimer
            // 
            this.MainTimer.Interval = 600;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenu;
            resources.ApplyResources(this.NotifyIcon, "NotifyIcon");
            this.NotifyIcon.BalloonTipClicked += new System.EventHandler(this.NotifyIcon_BalloonTipClicked);
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // NotifyIconContextMenu
            // 
            this.NotifyIconContextMenu.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.NotifyIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.websiteToolStripMenuItem,
            this.undoPatchToolStripMenuItem,
            this.separatorToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.NotifyIconContextMenu.Name = "NotifyIconContextMenu";
            resources.ApplyResources(this.NotifyIconContextMenu, "NotifyIconContextMenu");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Text = global::EZBlocker.Properties.strings.ToolStripOpen;
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // websiteToolStripMenuItem
            // 
            this.websiteToolStripMenuItem.Name = "websiteToolStripMenuItem";
            resources.ApplyResources(this.websiteToolStripMenuItem, "websiteToolStripMenuItem");
            this.websiteToolStripMenuItem.Text = global::EZBlocker.Properties.strings.ToolStripWebsite;
            // 
            // undoPatchToolStripMenuItem
            // 
            this.undoPatchToolStripMenuItem.Name = "undoPatchToolStripMenuItem";
            resources.ApplyResources(this.undoPatchToolStripMenuItem, "undoPatchToolStripMenuItem");
            this.undoPatchToolStripMenuItem.Text = global::EZBlocker.Properties.strings.ToolStripRemovePatch;
            // 
            // separatorToolStripMenuItem
            // 
            this.separatorToolStripMenuItem.Name = "separatorToolStripMenuItem";
            resources.ApplyResources(this.separatorToolStripMenuItem, "separatorToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Text = global::EZBlocker.Properties.strings.ToolStripExit;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // VolumeMixerButton
            // 
            resources.ApplyResources(this.VolumeMixerButton, "VolumeMixerButton");
            this.VolumeMixerButton.Name = "VolumeMixerButton";
            this.VolumeMixerButton.Text = global::EZBlocker.Properties.strings.VolumeMixerButtonText;
            this.VolumeMixerButton.UseVisualStyleBackColor = true;
            this.VolumeMixerButton.Click += new System.EventHandler(this.VolumeMixerButton_Click);
            // 
            // StatusLabel
            // 
            resources.ApplyResources(this.StatusLabel, "StatusLabel");
            this.StatusLabel.Name = "StatusLabel";
            // 
            // StartupCheckbox
            // 
            resources.ApplyResources(this.StartupCheckbox, "StartupCheckbox");
            this.StartupCheckbox.Name = "StartupCheckbox";
            this.StartupCheckbox.Text = global::EZBlocker.Properties.strings.StartupCheckboxText;
            this.StartupCheckbox.UseVisualStyleBackColor = true;
            this.StartupCheckbox.CheckedChanged += new System.EventHandler(this.StartupCheckbox_CheckedChanged);
            // 
            // SpotifyCheckbox
            // 
            resources.ApplyResources(this.SpotifyCheckbox, "SpotifyCheckbox");
            this.SpotifyCheckbox.Name = "SpotifyCheckbox";
            this.SpotifyCheckbox.Text = global::EZBlocker.Properties.strings.SpotifyCheckboxText;
            this.SpotifyCheckbox.UseVisualStyleBackColor = true;
            this.SpotifyCheckbox.CheckedChanged += new System.EventHandler(this.SpotifyCheckbox_CheckedChanged);
            // 
            // mediaPlayer
            // 
            resources.ApplyResources(this.mediaPlayer, "mediaPlayer");
            this.mediaPlayer.Name = "mediaPlayer";
            this.mediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("mediaPlayer.OcxState")));
            this.mediaPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.MediaPlayer_PlayStateChange);
            // 
            // LocalTrackPathTb
            // 
            resources.ApplyResources(this.LocalTrackPathTb, "LocalTrackPathTb");
            this.LocalTrackPathTb.Name = "LocalTrackPathTb";
            // 
            // LocalTrackPathBtn
            // 
            resources.ApplyResources(this.LocalTrackPathBtn, "LocalTrackPathBtn");
            this.LocalTrackPathBtn.Name = "LocalTrackPathBtn";
            this.LocalTrackPathBtn.UseVisualStyleBackColor = true;
            this.LocalTrackPathBtn.Click += new System.EventHandler(this.LocalTrackPathBtn_Click);
            // 
            // LocalTracksCountLbl
            // 
            resources.ApplyResources(this.LocalTracksCountLbl, "LocalTracksCountLbl");
            this.LocalTracksCountLbl.Name = "LocalTracksCountLbl";
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LocalTracksCountLbl);
            this.Controls.Add(this.LocalTrackPathBtn);
            this.Controls.Add(this.LocalTrackPathTb);
            this.Controls.Add(this.mediaPlayer);
            this.Controls.Add(this.SpotifyCheckbox);
            this.Controls.Add(this.StartupCheckbox);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.VolumeMixerButton);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.NotifyIconContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mediaPlayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Button VolumeMixerButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.CheckBox StartupCheckbox;
        private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem websiteToolStripMenuItem;
        private System.Windows.Forms.CheckBox SpotifyCheckbox;
        private System.Windows.Forms.ToolStripMenuItem undoPatchToolStripMenuItem;
        private AxWMPLib.AxWindowsMediaPlayer mediaPlayer;
        private System.Windows.Forms.TextBox LocalTrackPathTb;
        private System.Windows.Forms.Button LocalTrackPathBtn;
        private System.Windows.Forms.FolderBrowserDialog LocalTrackPathDialog;
        private System.Windows.Forms.Label LocalTracksCountLbl;
    }
}

