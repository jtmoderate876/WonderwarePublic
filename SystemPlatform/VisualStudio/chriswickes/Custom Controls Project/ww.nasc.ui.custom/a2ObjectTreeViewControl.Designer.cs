namespace ww.nasc.ui.custom {
    partial class a2ObjectTreeViewControl {
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
            this.tvMain = new System.Windows.Forms.TreeView();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.tsiStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsiExpand = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsiMinus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvMain
            // 
            this.tvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMain.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvMain.ItemHeight = 24;
            this.tvMain.Location = new System.Drawing.Point(0, 0);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(338, 476);
            this.tvMain.TabIndex = 0;
            this.tvMain.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvMain_BeforeCollapse);
            this.tvMain.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvMain_BeforeExpand);
            this.tvMain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tvMain_KeyPress);
            this.tvMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvMain_MouseUp);
            // 
            // ssMain
            // 
            this.ssMain.AllowMerge = false;
            this.ssMain.AutoSize = false;
            this.ssMain.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiStatus,
            this.tsiExpand,
            this.tsiMinus});
            this.ssMain.Location = new System.Drawing.Point(0, 476);
            this.ssMain.Name = "ssMain";
            this.ssMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ssMain.ShowItemToolTips = true;
            this.ssMain.Size = new System.Drawing.Size(338, 27);
            this.ssMain.SizingGrip = false;
            this.ssMain.TabIndex = 1;
            // 
            // tsiStatus
            // 
            this.tsiStatus.Image = global::ww.nasc.ui.custom.Properties.Resources.RefreshSmall;
            this.tsiStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsiStatus.Name = "tsiStatus";
            this.tsiStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsiStatus.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.tsiStatus.Size = new System.Drawing.Size(52, 22);
            this.tsiStatus.Click += new System.EventHandler(this.tsiStatus_Click);
            // 
            // tsiExpand
            // 
            this.tsiExpand.Image = global::ww.nasc.ui.custom.Properties.Resources.PlusCircle;
            this.tsiExpand.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsiExpand.Name = "tsiExpand";
            this.tsiExpand.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.tsiExpand.Size = new System.Drawing.Size(52, 22);
            this.tsiExpand.ToolTipText = "Expand All";
            this.tsiExpand.Click += new System.EventHandler(this.tsiExpand_Click);
            // 
            // tsiMinus
            // 
            this.tsiMinus.Image = global::ww.nasc.ui.custom.Properties.Resources.MinusCircle;
            this.tsiMinus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsiMinus.Name = "tsiMinus";
            this.tsiMinus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsiMinus.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.tsiMinus.Size = new System.Drawing.Size(52, 22);
            this.tsiMinus.ToolTipText = "Collapse All";
            this.tsiMinus.Click += new System.EventHandler(this.tsiMinus_Click);
            // 
            // a2ObjectTreeViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.tvMain);
            this.Name = "a2ObjectTreeViewControl";
            this.Size = new System.Drawing.Size(338, 503);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvMain;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.ToolStripStatusLabel tsiStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsiExpand;
        private System.Windows.Forms.ToolStripStatusLabel tsiMinus;

    }
}
