using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ww.nasc.ui.custom {
    public partial class a2ObjectTreeViewSplitterControl : UserControl {

        #region Declaration(s)
        private Boolean _fullScreen = false;
        private Int32 _lastSplitterDistance = 0;
        #endregion

        #region Event Delagates
        private delegate void addNodes(List<ArchestrATreeNode> Nodes);
        public delegate void RefreshCompleteHandler(object Sender, EventArgs e);
        public delegate void SelectedAppObjectHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectedAreaHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectedDIObjectHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectedEngineHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectedGraphicHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectionHandler(object Sender, SelectionEventArgs e);
        public delegate void SelectedPlatformHandler(object Sender, SelectionEventArgs e);
        public delegate void ClientAreaSizeChangedHandler(object Sender, EventArgs e);
        #endregion

        #region Constructor

        public a2ObjectTreeViewSplitterControl() {
            InitializeComponent();
            this.MyInit();
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [Category("Behavior")]
        [Description("Controls the staring position from which the AreaHierarchy XML file is parsed")]
        public Int32 AreaHierarchyStartingPosition {
            get { return tvMain.AreaHierarchyStartingPosition; }
            set { tvMain.AreaHierarchyStartingPosition = value; }
        }

        [Browsable(false)]
        [Description("Identifies the height of the \"client area\"")]
        public Int32 ClientHeight {
            get { return scMain.Panel2.Height; }
        }

        [Browsable(false)]
        [Description("Identifies the width of the \"client area\"")]
        public Int32 ClientWidth {
            get { return scMain.Panel2.Width; }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Application Engine objects are displayed in the tree view")]
        public Boolean DisplayAppEngines {
            get { return tvMain.DisplayAppEngines; }
            set {
                tvMain.DisplayAppEngines = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether application objects are displayed in the tree view.  If true, valid galaxy database information must be provided")]
        public Boolean DisplayAppObjects {
            get { return tvMain.DisplayAppObjects; }
            set {
                tvMain.DisplayAppObjects = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Device Integration objects are displayed in the tree view")]
        public Boolean DisplayDIObjects {
            get { return tvMain.DisplayDIObjects; }
            set {
                tvMain.DisplayDIObjects = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether graphic objects are displayed in the tree view.  If true, valid galaxy database information must be provided")]
        public Boolean DisplayGraphics {
            get { return tvMain.DisplayGraphics; }
            set {
                tvMain.DisplayGraphics = value; ;
            }
        }

        [CategoryAttribute("Behavior")]
        [Description("Controls whether or not graphic toolsets are shown or not.  TopLevelGraphicToolsetName property must be set to a valid value if this property is set to true")]
        public Boolean DisplayGraphicToolsets {
            get { return tvMain.DisplayGraphicToolsets; }
            set { tvMain.DisplayGraphicToolsets = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Controls whether or not the deployed model elements are shown.  If true (default), valid galaxy database information must be provided if object and/or graphic elements are to be shown")]
        public Boolean DisplayObjectModel {
            get { return tvMain.DisplayObjectModel; }
            set { tvMain.DisplayObjectModel = value; }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Application Engine objects are displayed in the tree view")]
        public Boolean DisplayViewEngines {
            get { return tvMain.DisplayViewEngines; }
            set {
                tvMain.DisplayViewEngines = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether WinPlatform objects are displayed in the tree view")]
        public Boolean DisplayWinPlatforms {
            get { return tvMain.DisplayWinPlatforms; }
            set { tvMain.DisplayWinPlatforms = value; }
        }

        [Browsable(false)]
        [Description("If truee, the vertical slider is positioned to the far left.  If false, it is positioned to the right of the left boarder 290 pixels.")]
        public Boolean FullScreenClientArea {
            get { return _fullScreen; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the name of the galaxy from which information will be extracted for object and graphic data.")]
        public String GalaxyName {
            get { return tvMain.GrDbName; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the SQL database server name from which information will be extracted for object and graphic data.")]
        public String GrServerName {
            get { return tvMain.GrServerName; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the user name that should be utilized to access the galaxy SQL database.  This is NOT a galaxy user but a SQL user!")]
        public String GrUserName {
            get { return tvMain.GrUserName; }
            set { tvMain.GrUserName = value; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [PasswordPropertyText(true)]
        [Description("Defines the user password that should be utilized to access the galaxy SQL database.  This is NOT a galaxy user but a SQL user!")]
        public String GrUserPassword {
            get { return tvMain.GrUserPassword; }
            set { tvMain.GrUserPassword = value; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines whether Windows Authentication shoudl be used when accessing the database.  If set to false, the user name and password properties MUST be set.")]
        public Boolean GrUseWindowsAuthentication {
            get { return tvMain.GrUseWindowsAuthentication; }
            set { tvMain.GrUseWindowsAuthentication = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Used to define a series of string characters for which if matched, the tree view will not include")]
        public String HideObjectNameStartingWith {
            get { return tvMain.HideObjectNameStartingWith; }
            set {
                tvMain.HideObjectNameStartingWith = value;
            }
        }

        [Browsable(true), Category("Appearance"), Description("Specifies the size of the individual nodes of the TreeView")]
        public NodeSize NodeSize {
            get { return tvMain.NodeSize; }
            set { tvMain.NodeSize = value; }
        }

        [Browsable(false)]
        public String SelectedAppEngine {
            get { return tvMain.SelectedAppEngine; }
        }

        [Browsable(false)]
        public String SelectedAppObject {
            get { return tvMain.SelectedAppObject; }
        }

        [Browsable(false)]
        public String SelectedArea {
            get { return tvMain.SelectedArea; }
        }

        [Browsable(false)]
        public String SelectedDIObject {
            get { return tvMain.SelectedDIObject; }
        }

        [Browsable(false)]
        public String SelectedGraphic {
            get { return tvMain.SelectedGraphic; }
        }

        [Browsable(false)]
        public String SelectedNode {
            get { return tvMain.SelectedNode; }
        }

        [Browsable(false)]
        public String SelectedPlatform {
            get { return tvMain.SelectedPlatform; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Identifies the top level area that should be displayed in the tree view based on the deployed area model")]
        public String TopLevelAreaName {
            get { return tvMain.TopLevelAreaName; }
            set { tvMain.TopLevelAreaName = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Identifies the top level graphic toolset that should be displayed in the tree view.  DisplayGraphicToolsets must be set to true and valid database information provided as well")]
        public String TopLevelGraphicToolsetName {
            get { return tvMain.TopLevelGraphicToolsetName; }
            set { tvMain.TopLevelGraphicToolsetName = value; }
        }

        [Browsable(false)]
        [Description("Identifies the total height of the entire control")]
        public Int32 TotalHeight {
            get { return this.Height; }
        }

        [Browsable(false)]
        [Description("Identifies the total width of the entire control")]
        public Int32 TotalWidth {
            get { return this.Width; }
        }

        [Browsable(false)]
        public RefreshStatus TreeViewRefreshStatus {
            get { return tvMain.TreeViewRefreshStatus; }
        }

        #endregion

        #region Method(s)

        private void MyInit() {
            _lastSplitterDistance = scMain.SplitterDistance;
            pbFullScreen.Location = new Point(scMain.SplitterDistance - (pbFullScreen.Width + 5), this.Height - (pbFullScreen.Height + 2));
            pbFullScreen.Visible = true;
        }

        protected virtual void OnClientAreaChanged(EventArgs e) {
            if (ClientAreaSizeChanged != null) {
                ClientAreaSizeChanged(this, e);
            }
        }

        protected virtual void OnNodeSelected(SelectionEventArgs e) {
            if (NodeSelected != null)
                NodeSelected(this, e);
        }

        protected virtual void OnRefreshComplete(EventArgs e) {
            if (RefreshComplete != null) RefreshComplete(this, e);
        }

        protected virtual void OnSelectedEngine(SelectionEventArgs e) {
            if (EngineSelected != null)
                EngineSelected(this, e);
        }

        protected virtual void OnSelectedAppObject(SelectionEventArgs e) {
            if (AppObjectSelected != null)
                AppObjectSelected(this, e);
        }

        protected virtual void OnSelectedArea(SelectionEventArgs e) {
            if (AreaSelected != null)
                AreaSelected(this, e);
        }

        protected virtual void OnSelectedDIObject(SelectionEventArgs e) {
            if (DIObjectSelected != null)
                DIObjectSelected(this, e);
        }

        protected virtual void OnSelectedGraphic(SelectionEventArgs e) {
            if (GraphicSelected != null)
                GraphicSelected(this, e);
        }

        protected virtual void OnSelectedPlatform(SelectionEventArgs e) {
            if (PlatformSelected != null)
                PlatformSelected(this, e);
        }


        public void CollapseAll() {
            tvMain.CollapseAll();
        }

        public void ExpandAll() {
            tvMain.ExpandAll();
        }

        public void MoveFullScreen(Boolean GoFullScreen) {
            if (GoFullScreen) {
                _lastSplitterDistance = scMain.SplitterDistance;
                scMain.SplitterDistance = 0;
            } else {
                scMain.SplitterDistance = _lastSplitterDistance;
            }
        }

        public void RefreshTreeView() {
            if (String.IsNullOrEmpty(this.GalaxyName) || String.IsNullOrEmpty(this.GrServerName)) {
                tvMain.SetStatusInfo(RefreshStatus.Warning, "Unable to refresh...no galaxy deployed");
            } else {
                tvMain.RefreshTreeView();
            }
        }

        #endregion

        #region Event(s)

        [Category("Property Changed")]
        [Description("Identifies the selection of an Application Object from within the control")]
        public event SelectedAppObjectHandler AppObjectSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of an Area within the control")]
        public event SelectedAreaHandler AreaSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of a DI Object within the control")]
        public event SelectedDIObjectHandler DIObjectSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of an Application or View Engine within the control")]
        public event SelectedEngineHandler EngineSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of a graphic object within the control")]
        public event SelectedGraphicHandler GraphicSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of any type of node within the control")]
        public event SelectionHandler NodeSelected;

        [Category("Property Changed")]
        [Description("Identifies the selection of a WinPlatform object within the control")]
        public event SelectedPlatformHandler PlatformSelected;

        [Category("Action")]
        [Description("Identifies that the refresh operation has completed")]
        public event RefreshCompleteHandler RefreshComplete;

        [Category("Action")]
        [Description("Identifies that the client area size has changed")]
        public event ClientAreaSizeChangedHandler ClientAreaSizeChanged;

        #endregion

        #region Event Handler(s)

        private void pbFullScreen_Click(object sender, EventArgs e) {
            if (!this.FullScreenClientArea) {
                this.MoveFullScreen(true);
            }
        }

        private void scMain_Panel2_Resize(object sender, EventArgs e) {
            if (scMain.SplitterDistance > 1) {
                _fullScreen = false;
            } else {
                _fullScreen = true;
            }

            // Move full screen button (if there is enough room to see it)
            if (scMain.SplitterDistance > 170) {
                pbFullScreen.Visible = true;
                pbFullScreen.Location = new Point(scMain.SplitterDistance - (pbFullScreen.Width + 5), this.Height - (pbFullScreen.Height + 2));
            } else {
                pbFullScreen.Visible = false;
            }

            // Raise event
            this.OnClientAreaChanged(EventArgs.Empty);
        }

        private void tvMain_AppObjectSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedAppObject(e);
        }

        private void tvMain_AreaSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedArea(e);
        }

        private void tvMain_DIObjectSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedDIObject(e);
        }

        private void tvMain_EngineSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedEngine(e);
        }

        private void tvMain_GraphicSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedGraphic(e);
        }

        private void tvMain_NodeSelected(object Sender, SelectionEventArgs e) {
            this.OnNodeSelected(e);
        }

        private void tvMain_PlatformSelected(object Sender, SelectionEventArgs e) {
            this.OnSelectedPlatform(e);
        }

        #endregion

        private void scMain_Paint(object sender, PaintEventArgs e) {
            pbFullScreen.Height = (tvMain.StatusBarHeight - 2);
            pbFullScreen.Width = pbFullScreen.Height;
              _lastSplitterDistance = scMain.SplitterDistance;
            pbFullScreen.Location = new Point(scMain.SplitterDistance - (pbFullScreen.Width + 5), this.Height - (pbFullScreen.Height + 2));
            pbFullScreen.Visible = true;
      }

    }
}
