namespace ww.nasc.ui.custom {
    partial class a2ObjectTreeViewSplitterControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.pbFullScreen = new System.Windows.Forms.PictureBox();
            this.tvMain = new ww.nasc.ui.custom.a2ObjectTreeViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFullScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pbFullScreen);
            this.scMain.Panel1.Controls.Add(this.tvMain);
            this.scMain.Panel1MinSize = 0;
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Resize += new System.EventHandler(this.scMain_Panel2_Resize);
            this.scMain.Size = new System.Drawing.Size(854, 587);
            this.scMain.SplitterDistance = 290;
            this.scMain.TabIndex = 0;
            this.scMain.Paint += new System.Windows.Forms.PaintEventHandler(this.scMain_Paint);
            // 
            // pbFullScreen
            // 
            this.pbFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbFullScreen.Image = global::ww.nasc.ui.custom.Properties.Resources.FullScreen;
            this.pbFullScreen.Location = new System.Drawing.Point(265, 562);
            this.pbFullScreen.Name = "pbFullScreen";
            this.pbFullScreen.Size = new System.Drawing.Size(23, 23);
            this.pbFullScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFullScreen.TabIndex = 1;
            this.pbFullScreen.TabStop = false;
            this.pbFullScreen.Click += new System.EventHandler(this.pbFullScreen_Click);
            // 
            // tvMain
            // 
            this.tvMain.AreaHierarchyStartingPosition = 24;
            this.tvMain.DisplayAppEngines = false;
            this.tvMain.DisplayAppObjects = false;
            this.tvMain.DisplayDIObjects = false;
            this.tvMain.DisplayGraphics = false;
            this.tvMain.DisplayGraphicToolsets = false;
            this.tvMain.DisplayObjectModel = true;
            this.tvMain.DisplayViewEngines = false;
            this.tvMain.DisplayWinPlatforms = false;
            this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvMain.GrUserName = "";
            this.tvMain.GrUserPassword = "";
            this.tvMain.GrUseWindowsAuthentication = true;
            this.tvMain.HideObjectNameStartingWith = "";
            this.tvMain.Location = new System.Drawing.Point(0, 0);
            this.tvMain.Name = "tvMain";
            this.tvMain.NodeSize = ww.nasc.ui.custom.NodeSize.Medium;
            this.tvMain.Size = new System.Drawing.Size(290, 587);
            this.tvMain.TabIndex = 0;
            this.tvMain.TopLevelAreaName = "";
            this.tvMain.TopLevelGraphicToolsetName = "";
            this.tvMain.AppObjectSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedAppObjectHandler(this.tvMain_AppObjectSelected);
            this.tvMain.AreaSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedAreaHandler(this.tvMain_AreaSelected);
            this.tvMain.DIObjectSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedDIObjectHandler(this.tvMain_DIObjectSelected);
            this.tvMain.EngineSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedEngineHandler(this.tvMain_EngineSelected);
            this.tvMain.GraphicSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedGraphicHandler(this.tvMain_GraphicSelected);
            this.tvMain.NodeSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectionHandler(this.tvMain_NodeSelected);
            this.tvMain.PlatformSelected += new ww.nasc.ui.custom.a2ObjectTreeViewControl.SelectedPlatformHandler(this.tvMain_PlatformSelected);
            // 
            // a2ObjectTreeViewSplitterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scMain);
            this.Name = "a2ObjectTreeViewSplitterControl";
            this.Size = new System.Drawing.Size(854, 587);
            this.scMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFullScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scMain;
        private a2ObjectTreeViewControl tvMain;
        private System.Windows.Forms.PictureBox pbFullScreen;
    }
}
