using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace ww.nasc.ui.custom {

    public partial class a2ObjectTreeViewControl : UserControl {

        #region Declaration(s)
        private Boolean _displayAppEngines = false;
        private Boolean _displayDIOs = false;
        private Boolean _displayGraphics = false;
        private Boolean _displayGraphicToolset = false;
        private Boolean _displayObjectModel = true;
        private Boolean _displayObjects = false;
        private Boolean _displayViewEngines = false;
        private Boolean _displayWinPlatform = false;
        private String _grDBName = String.Empty;
        private Boolean _grDbUseSSPI = true;
        private String _grPassword = String.Empty;
        private String _grServerName = String.Empty;
        private String _grUserName = String.Empty;
        private String _hideObjectNameStartingWith = String.Empty;
        private String _selectedArea = String.Empty;
        private String _selectedAppObject = String.Empty;
        private String _selectedDIObject = String.Empty;
        private String _selectedEngine = String.Empty;
        private String _selectedGraphic = String.Empty;
        private String _selectedNode = String.Empty;
        private String _selectedPlatform = String.Empty;
        private String _topLevelAreaName = String.Empty;
        private String _topLevelGraphicToolset = String.Empty;
        private Int32 _offset = 24;
        private NodeSize _nodeSize = NodeSize.Medium;
        private RefreshStatus _rs = RefreshStatus.Unknown;
        private Boolean _refreshing = false;
        private Boolean _expandDebounce = false;
        private Boolean _colDebounce = false;
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
        #endregion

        #region Constructor(s)

        public a2ObjectTreeViewControl() {
            InitializeComponent();
            SetStatusInfo(RefreshStatus.Success, "Ready...");
        }

        #endregion

        #region Properties

        [Browsable(false)]
        [Category("Behavior")]
        [Description("Controls the staring position from which the AreaHierarchy XML file is parsed")]
        public Int32 AreaHierarchyStartingPosition {
            get { return _offset; }
            set {
                if (value < 0) {
                    _offset = 0;
                } else {
                    _offset = value;
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Application Engine objects are displayed in the tree view")]
        public Boolean DisplayAppEngines {
            get { return _displayAppEngines; }
            set {
                _displayAppEngines = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether application objects are displayed in the tree view.  If true, valid galaxy database information must be provided")]
        public Boolean DisplayAppObjects {
            get { return _displayObjects; }
            set {
                _displayObjects = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Device Integration objects are displayed in the tree view")]
        public Boolean DisplayDIObjects {
            get { return _displayDIOs; }
            set {
                _displayDIOs = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether graphic objects are displayed in the tree view.  If true, valid galaxy database information must be provided")]
        public Boolean DisplayGraphics {
            get { return _displayGraphics; }
            set {
                _displayGraphics = value; ;
            }
        }

        [CategoryAttribute("Behavior")]
        [Description("Controls whether or not graphic toolsets are shown or not.  TopLevelGraphicToolsetName property must be set to a valid value if this property is set to true")]
        public Boolean DisplayGraphicToolsets {
            get { return _displayGraphicToolset; }
            set { _displayGraphicToolset = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Controls whether or not the deployed model elements are shown.  If true (default), valid galaxy database information must be provided if object and/or graphic elements are to be shown")]
        public Boolean DisplayObjectModel {
            get { return _displayObjectModel; }
            set { _displayObjectModel = value; }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether Application Engine objects are displayed in the tree view")]
        public Boolean DisplayViewEngines {
            get { return _displayViewEngines; }
            set {
                _displayViewEngines = value;
            }
        }

        [CategoryAttribute("Appearance")]
        [Description("Specifies whether WinPlatform objects are displayed in the tree view")]
        public Boolean DisplayWinPlatforms {
            get { return _displayWinPlatform; }
            set {
                _displayWinPlatform = value;
            }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the SQL database name of the galaxy from which information will be extracted for object and graphic data.")]
        public String GrDbName {
            get {
                String result = String.Empty;
                Microsoft.Win32.RegistryKey regKey = null;
                try {
                    regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\ArchestrA\\Framework\\ClusterInformation", false);

                    if (regKey == null) {
                        // Try getting it out of Wow6432 Node structure
                        regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\ArchestrA\\Framework\\ClusterInformation", false);
                    }

                    if (regKey != null) {
                        Object o = regKey.GetValue("ClusterName");
                        if (o != null) {
                            result = (String)o;
                        }
                    } else {
                        this.SetStatusInfo(RefreshStatus.Warning, "Unable to retrieve the galaxy name.  Is a Platform object deployed to this node?");
                    }
                } catch (Exception ex) {
                    this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered attempting to retrieve the galaxy name.  Is a Platform object deployed to this node?");
                }
                return result;
            }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the SQL database server name from which information will be extracted for object and graphic data.")]
        public String GrServerName {
            get {
                String result = String.Empty;
                Microsoft.Win32.RegistryKey regKey = null;
                try {
                    regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\ArchestrA\\Framework\\ClusterInformation", false);

                    if (regKey == null) {
                        // Try getting it out of Wow6432 Node structure
                        regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\ArchestrA\\Framework\\ClusterInformation", false);
                    }

                    if (regKey != null) {
                        Object o = regKey.GetValue("GalaxyPlatformName");
                        if (o != null) {
                            result = (String)o;
                        }
                    } else {
                        this.SetStatusInfo(RefreshStatus.Warning, "Unable to retrieve the galaxy repository server name.  Is a Platform object deployed to this node?");
                    }
                } catch (Exception ex) {
                    this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered attempting to retrieve the galaxy repository's server name.  Is a Platform object deployed to this node?");
                }
                return result;
            }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines the user name that should be utilized to access the galaxy SQL database.  This is NOT a galaxy user but a SQL user!")]
        public String GrUserName {
            get { return _grUserName; }
            set { _grUserName = value; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [PasswordPropertyText(true)]
        [Description("Defines the user password that should be utilized to access the galaxy SQL database.  This is NOT a galaxy user but a SQL user!")]
        public String GrUserPassword {
            get { return _grPassword; }
            set { _grPassword = value; }
        }

        [CategoryAttribute("Galaxy Database Info")]
        [Description("Defines whether Windows Authentication shoudl be used when accessing the database.  If set to false, the user name and password properties MUST be set.")]
        public Boolean GrUseWindowsAuthentication {
            get { return _grDbUseSSPI; }
            set { _grDbUseSSPI = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Used to define a series of string characters for which if matched, the tree view will not include")]
        public String HideObjectNameStartingWith {
            get { return _hideObjectNameStartingWith; }
            set {
                _hideObjectNameStartingWith = value;
            }
        }

        [Browsable(true), Category("Appearance"), Description("Specifies the size of the individual nodes of the TreeView")]
        public NodeSize NodeSize {
            get {
                return _nodeSize;
            }
            set {
                _nodeSize = value;
                try {
                    this.LoadImageList(value);
                } catch (Exception ex) {
                    this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered attempting to load the standard galaxy icons.  Is a Platform object deployed to this node?");
                }
            }
        }

        [Browsable(false)]
        public String SelectedAppEngine {
            get { return _selectedEngine; }
        }

        [Browsable(false)]
        public String SelectedAppObject {
            get { return _selectedAppObject; }
        }

        [Browsable(false)]
        public String SelectedArea {
            get { return _selectedArea; }
        }

        [Browsable(false)]
        public String SelectedDIObject {
            get { return _selectedDIObject; }
        }

        [Browsable(false)]
        public String SelectedGraphic {
            get { return _selectedGraphic; }
        }

        [Browsable(false)]
        public String SelectedNode {
            get { return _selectedNode; }
        }

        [Browsable(false)]
        public String SelectedPlatform {
            get { return _selectedPlatform; }
        }

        [Browsable(false)]
        [Description("Identifies the total height of the status bar")]
        public Int32 StatusBarHeight {
            get { return ssMain.Height; }
        }

        [Browsable(false)]
        [Description("Identifies the total width of the status bar")]
        public Int32 StatusBarWidth {
            get { return ssMain.Width; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Identifies the top level area that should be displayed in the tree view based on the deployed area model")]
        public String TopLevelAreaName {
            get { return _topLevelAreaName; }
            set { _topLevelAreaName = value; }
        }

        [CategoryAttribute("Behavior")]
        [Description("Identifies the top level graphic toolset that should be displayed in the tree view.  DisplayGraphicToolsets must be set to true and valid database information provided as well")]
        public String TopLevelGraphicToolsetName {
            get { return _topLevelGraphicToolset; }
            set { _topLevelGraphicToolset = value; }
        }

        [Browsable(false)]
        public RefreshStatus TreeViewRefreshStatus {
            get { return _rs; }
        }

        #endregion

        #region Method(s)

        #region Private Method(s)

        private void AddTheNodes(List<ArchestrATreeNode> Nodes) {
            if (Nodes == null) return;
            

            tvMain.BeginUpdate();

            if (tvMain.Nodes.Count > 0) {
                tvMain.Nodes.Clear();
            }
            
            tvMain.TreeViewNodeSorter = new ArchestrATreeNodeSorter();
            tvMain.Sort();
            //this.FinishSort(Nodes);
            tvMain.Nodes.AddRange(Nodes.ToArray());
            tvMain.EndUpdate();
        }

        private void LoadImageList(NodeSize NewNodeSize) {
            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth24Bit;
            il.TransparentColor = Color.Fuchsia;

            switch (NewNodeSize) {
                case NodeSize.XSmall:
                    this.tvMain.Font = new Font("Arial", 8f, FontStyle.Bold);
                    il.ImageSize = new Size(12, 12);
                    this.tvMain.ItemHeight = 16;
                    this.tvMain.Indent = 16;
                    break;
                case NodeSize.Small:
                    this.tvMain.Font = new Font("Arial", 12f, FontStyle.Bold);
                    il.ImageSize = new Size(16, 16);
                    this.tvMain.ItemHeight = 20;
                    this.tvMain.Indent = 20;
                    break;
                case NodeSize.Medium:
                    this.tvMain.Font = new Font("Arial", 14f, FontStyle.Bold);
                    il.ImageSize = new Size(24, 24);
                    this.tvMain.ItemHeight = 28;
                    this.tvMain.Indent = 28;
                    break;
                case NodeSize.Large:
                    this.tvMain.Font = new Font("Arial", 16f, FontStyle.Bold);
                    il.ImageSize = new Size(28, 28);
                    this.tvMain.ItemHeight = 32;
                    this.tvMain.Indent = 32;
                    break;
                case NodeSize.XLarge:
                    this.tvMain.Font = new Font("Arial", 24f, FontStyle.Bold);
                    il.ImageSize = new Size(32, 32);
                    this.tvMain.ItemHeight = 36;
                    this.tvMain.Indent = 36;
                    break;
            }

            il = AddImageListIcons(il);
            this.tvMain.ImageList = il;
        }

        private void RaiseNodeSelectedEvent(ArchestrATreeNode atn) {
            switch (atn.ImageIndex) {
                case 0:     // Area
                    _selectedNode = atn.Text;
                    _selectedArea = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Area, (ArchestrATreeNode)atn));
                    this.OnSelectedArea(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Area, (ArchestrATreeNode)atn));
                    break;
                case 1:     // AppEngine
                case 8:     // ViewEngine
                    _selectedNode = atn.Text;
                    _selectedEngine = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Engine, (ArchestrATreeNode)atn));
                    this.OnSelectedEngine(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Engine, (ArchestrATreeNode)atn));
                    break;
                case 2:     // GR 
                    _selectedNode = atn.Text;
                    _selectedPlatform = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Platform, (ArchestrATreeNode)atn));
                    this.OnSelectedPlatform(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Platform, (ArchestrATreeNode)atn));
                    break;
                case 3:     // Platform
                    _selectedNode = atn.Text;
                    _selectedPlatform = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Platform, (ArchestrATreeNode)atn));
                    this.OnSelectedPlatform(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Platform, (ArchestrATreeNode)atn));
                    break;
                case 4:     // App Object
                    _selectedNode = atn.Text;
                    _selectedAppObject = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.AppObject, (ArchestrATreeNode)atn));
                    this.OnSelectedAppObject(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.AppObject, (ArchestrATreeNode)atn));
                    break;
                case 5:     // Graphic
                    ArchestrATreeNode tnParent = (ArchestrATreeNode)atn.Parent;

                    if (tnParent != null && tnParent.ArchestrATreeNodeType != ArchestrATreeNodeType.Folder) {
                        _selectedNode = String.Format("{0}.{1}", tnParent.Text, atn.Text);
                    } else {
                        _selectedNode = atn.Text;
                    }

                    _selectedGraphic = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Graphic, (ArchestrATreeNode)atn));
                    this.OnSelectedGraphic(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.Graphic, (ArchestrATreeNode)atn));
                    break;
                case 6:     // DI Object
                case 9:
                    _selectedNode = atn.Text;
                    _selectedDIObject = _selectedNode;
                    OnNodeSelected(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.IO, (ArchestrATreeNode)atn));
                    this.OnSelectedDIObject(new SelectionEventArgs(_selectedNode, ArchestrATreeNodeType.IO, (ArchestrATreeNode)atn));
                    break;
                default:
                    break;
            }
        }

        internal void SetStatusInfo(RefreshStatus Status, String Message) {
            switch (Status) {
                case RefreshStatus.Unknown:
                    tsiStatus.Image = Properties.Resources.WarningSmall;
                    _rs = Status;
                    break;
                case RefreshStatus.Success:
                    tsiStatus.Image = Properties.Resources.CheckMarkSmall;
                    _rs = Status;
                    break;
                case RefreshStatus.Warning:
                    tsiStatus.Image = Properties.Resources.WarningSmall;
                    _rs = Status;
                    break;
                case RefreshStatus.Refreshing:
                    tsiStatus.Image = Properties.Resources.RefreshSmall;
                    _rs = Status;
                    break;
                default:
                    tsiStatus.Image = Properties.Resources.WarningSmall;
                    _rs = Status;
                    break;
            }

            tsiStatus.ToolTipText = Message;
            Application.DoEvents();     // Force the UI to update
        }

        #endregion

        #region Protected Method(s)

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

        #endregion

        #region Public Method(s)

        public void CollapseAll() {
            tvMain.CollapseAll();
            _colDebounce = false;
            _expandDebounce = false;
            tvMain.SelectedNode = null;
        }

        public void ExpandAll() {
            tvMain.ExpandAll();
            _colDebounce = false;
            _expandDebounce = false;
            tvMain.SelectedNode = null;
        }

        public void RefreshTreeView() {
            this.SetStatusInfo(RefreshStatus.Refreshing, "Refreshing...");
            _refreshing = true;

            // First make sure valid information exists to retrieve the desired data
            if (System.String.IsNullOrEmpty(this.GrDbName) || System.String.IsNullOrEmpty(this.GrServerName) || (!this.GrUseWindowsAuthentication && System.String.IsNullOrEmpty(this.GrUserName))) {
                this.SetStatusInfo(RefreshStatus.Warning, "Unable to refresh...no/invalid information about the galaxy");
                _refreshing = false;
            }

            if (this.TreeViewRefreshStatus == RefreshStatus.Warning) return;

            this.LoadImageList(this.NodeSize);

            // Some kind of problem was encountered...no sense continuing so bail
            if (this.TreeViewRefreshStatus != RefreshStatus.Refreshing) {
                _refreshing = false;
                return;
            }

            // Peform the refresh on a seperate thread so the UI isn't locked while accessing external resources
            BackgroundWorker _bw = new BackgroundWorker();
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bw_RunWorkerCompleted);
            _bw.RunWorkerAsync();
        }

        #endregion

        #endregion

        #region Function(s)

        #region Private Function(s)

        private ImageList AddImageListIcons(ImageList ImageList) {
            ImageList il = ImageList;

            if (il != null) {
                try {
                    Object o = null;
                    o = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ArchestrA\\Framework").GetValue("RootPath");

                    if (o == null) {
                        o = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\ArchestrA\\Framework").GetValue("RootPath");
                    }

                    if (o != null) {
                        String arg = (String)o;

                        //String arg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ArchestrA\\Framework").GetValue("RootPath").ToString();
                        String filename1 = String.Format("{0}\\Icons\\area_pd.w2k.bmp", arg);
                        String filename2 = String.Format("{0}\\Icons\\appengine_pd.w2k.bmp", arg);
                        String filename3 = String.Format("{0}\\Icons\\GRNode_pd.w2k.bmp", arg);
                        String filename4 = String.Format("{0}\\Icons\\ntplatform_pd.w2k.bmp", arg);
                        String filename5 = String.Format("{0}\\Icons\\appobject.w2k.bmp", arg);
                        String filename6 = String.Format("{0}\\Icons\\symbol.w2k.bmp", arg);
                        String filename7 = String.Format("{0}\\Icons\\DDESuiteLinkClient_pd.w2k.bmp", arg);
                        String filename8 = String.Format("{0}\\Icons\\viewengine_pd.w2k.bmp", arg);

                        il.Images.Add("AreaIcon", Image.FromFile(filename1));
                        il.Images.Add("EngineIcon", Image.FromFile(filename2));
                        il.Images.Add("GRIcon", Image.FromFile(filename3));
                        il.Images.Add("WinPlatformIcon", Image.FromFile(filename4));
                        il.Images.Add("ObjectIcon", Image.FromFile(filename5));
                        il.Images.Add("GraphicIcon", Image.FromFile(filename6));
                        il.Images.Add("DIIcon", Image.FromFile(filename7));
                        il.Images.Add("OpenFolder", global::ww.nasc.ui.custom.Properties.Resources.OpenFolderSmall2);
                        il.Images.Add("ViewEngineIcon", Image.FromFile(filename8));
                        il.Images.Add("RDI", global::ww.nasc.ui.custom.Properties.Resources.RDI);
                    } else {
                        this.SetStatusInfo(RefreshStatus.Warning, "Unable to load the list of galaxy grpahic icons.");
                    }
                } catch (Exception ex) {
                    this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while attempting to load the list of galaxy grpahic icons.");
                }
            } else {
                // simply return an blank image list
                il = new ImageList();
            }

            return il;
        }

        private ArchestrATreeNode GetAppEngineTreeNode(String EngineName) {
            ArchestrATreeNode atn = null;
            if (this.DisplayAppEngines && !this.IsHideMatch(EngineName)) {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, EngineName, 1);
                String[] graphicNames = this.GetGraphicNames(EngineName);

                if (graphicNames.Length == 0) {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, EngineName, 1);
                } else {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, graphicNames, EngineName, 1);
                    if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                }
            }
            return atn;
        }

        private ArchestrATreeNode GetAppObjectTreeNodeByDataRow(DataRow dr) {
            ArchestrATreeNode tn = null;
            try {
                if (!this.IsHideMatch(dr[0].ToString())) {
                    String[] graphicNames = this.GetGraphicNames(dr[0].ToString());

                    if (graphicNames.Length == 0) {
                        tn = new ArchestrATreeNode(ArchestrATreeNodeType.AppObject, dr[0].ToString(), 4);
                    } else {
                        tn = new ArchestrATreeNode(ArchestrATreeNodeType.AppObject, graphicNames, dr[0].ToString(), 4);
                        if (this.DisplayGraphics) tn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                    }

                    List<ArchestrATreeNode> children = this.GetAppObjectTreeNodesByAppObject(tn.Text);
                    if (children != null) tn.Nodes.AddRange(children.ToArray());
                }
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while analyzing the data from the galaxy related to the Application Objects and assigning to the proper hierarchy location(s)");
            }

            return tn;
        }

        private List<ArchestrATreeNode> GetAppObjectTreeNodesByAppObject(String ObjectName) {
            List<ArchestrATreeNode> nodes = new List<ArchestrATreeNode>();

            SqlConnection sqlConn = null;
            if (!this.GetSqlConnection(out sqlConn)) {
                return nodes;
            }

            try {
                SqlDataAdapter da = new SqlDataAdapter("SELECT g.tag_name FROM gObject g INNER JOIN gobject g2 ON g.contained_by_gobject_id = g2.gobject_id WHERE g2.tag_name = @Parent AND g.contained_by_gobject_id = g2.gobject_id ORDER BY g.tag_name", sqlConn);
                SqlParameter p1 = new SqlParameter("@Parent", ObjectName);
                da.SelectCommand.Parameters.Add(p1);
                DataSet ds = new DataSet("objects");
                da.Fill(ds);

                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
                    foreach (DataRow dr in ds.Tables[0].Rows) {
                        ArchestrATreeNode atn = this.GetAppObjectTreeNodeByDataRow(dr);
                        if (atn != null) nodes.Add(atn);
                    }
                }

                da.Dispose();
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while retrieving application object data from other application objects");
            }

            return nodes;
        }

        private List<ArchestrATreeNode> GetAppObjectTreeNodesByArea(String AreaName) {
            List<ArchestrATreeNode> nodes = new List<ArchestrATreeNode>();

            if (this.DisplayAppObjects) {
                SqlConnection sqlConn = null;
                if (!this.GetSqlConnection(out sqlConn)) {
                    return nodes;
                }

                try {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT g.tag_name FROM gObject g INNER JOIN gobject g2 ON g.area_gobject_id = g2.gobject_id WHERE g2.tag_name = @AreaName AND g.area_gobject_id = g.hosted_by_gobject_id AND g.contained_by_gobject_id = 0 ORDER BY g.tag_name", sqlConn);
                    SqlParameter p1 = new SqlParameter("@AreaName", AreaName);
                    da.SelectCommand.Parameters.Add(p1);
                    DataSet ds = new DataSet("objects");
                    da.Fill(ds);

                    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
                        foreach (DataRow dr in ds.Tables[0].Rows) {
                            ArchestrATreeNode atn = this.GetAppObjectTreeNodeByDataRow(dr);
                            if (atn != null) nodes.Add(atn);
                        }
                    }

                    da.Dispose();
                } catch (Exception ex) {
                    // Only update the tree node status if it is already not in a warning state...
                    if (this.TreeViewRefreshStatus != RefreshStatus.Warning) {
                        this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while retrieving application object data from area(s)");
                    }
                }
            }
            return nodes;
        }

        private ArchestrATreeNode GetAreaTreeNode(String AreaName) {
            ArchestrATreeNode atn = null;
            String[] graphicNames = this.GetGraphicNames(AreaName);

            if (graphicNames.Length == 0) {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Area, AreaName, 0);
            } else {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Area, graphicNames, AreaName, 0);
                if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
            }

            return atn;
        }

        private ArchestrATreeNode GetDIObjectTreeNode(String DIOName) {
            ArchestrATreeNode atn = null;

            if (this.DisplayDIObjects) {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, DIOName, 6);
                String[] graphicNames = this.GetGraphicNames(DIOName);

                if (graphicNames.Length == 0) {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, DIOName, 6);
                } else {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, graphicNames, DIOName, 6);
                    if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                }
            }

            return atn;
        }

        private ArchestrATreeNode GetRDIObjectTreeNode(String DIOName) {
            ArchestrATreeNode atn = null;

            if (this.DisplayDIObjects) {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, DIOName, 9);
                String[] graphicNames = this.GetGraphicNames(DIOName);

                if (graphicNames.Length == 0) {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, DIOName, 9);
                } else {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.IO, graphicNames, DIOName, 9);
                    if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                }
            }

            return atn;
        }

        private String GetFilterExpression() {
            StringBuilder sbFilter = new StringBuilder();

            try {
                sbFilter.Append("//Area[@IsArea='1']");
                if (this.DisplayAppEngines) { sbFilter.Append(" | //Area[@Category='3']"); }
                if (this.DisplayDIObjects) { sbFilter.Append(" | //Area[@Category='11']"); }
                if (this.DisplayDIObjects) { sbFilter.Append(" | //Area[@Category='24']"); } // Redundant DI Objects
                if (this.DisplayWinPlatforms) { sbFilter.Append(" | //Area[@Category='1']"); }
                if (this.DisplayViewEngines) { sbFilter.Append(" | //Area[@Category='4']"); }
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while building the filter expression for the node types to be shown");
            }
            return sbFilter.ToString();
        }

        private String[] GetGraphicNames(String ByName) {
            List<String> graphics = new List<string>();
            List<TreeNode> nodes = new List<TreeNode>();

            SqlConnection sqlConn = null;
            if (this.GetSqlConnection(out sqlConn)) {
                try {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT primitive_name AS Graphic FROM internal_visual_element_description_view a INNER JOIN internal_common_obj b on a.gobject_id = b.gobject_id WHERE b.tag_name = @Parent AND a.package_type <> 'x' ORDER BY primitive_name", sqlConn);
                    SqlParameter p1 = new SqlParameter("@Parent", ByName);
                    da.SelectCommand.Parameters.Add(p1);
                    DataSet ds = new DataSet("objects");
                    da.Fill(ds);

                    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
                        foreach (DataRow dr in ds.Tables[0].Rows) {
                            graphics.Add(dr[0].ToString());
                        }
                    }
                    da.Dispose();
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while retrieving graphics from the galaxy");
                }
            }

            return graphics.ToArray();
        }

        private ArchestrATreeNode[] GetGraphicsNodes(String[] Names) {
            List<ArchestrATreeNode> nodes = new List<ArchestrATreeNode>();

            foreach (String name in Names) {
                if (!this.IsHideMatch(name)) {
                    ArchestrATreeNode atn = new ArchestrATreeNode(ArchestrATreeNodeType.Graphic, name, 5);
                    nodes.Add(atn);
                }

            }
            return nodes.ToArray();
        }

        private List<ArchestrATreeNode> GetGraphicSymbols(Int32 ParentFolderID) {
            List<ArchestrATreeNode> results = new List<ArchestrATreeNode>();
            if (TreeViewRefreshStatus == RefreshStatus.Warning) return results;
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT O.tag_name, F.Folder_id ");
            sb.AppendLine("FROM gObject O ");
            sb.AppendLine("LEFT JOIN folder_gObject_link fgl ON O.gObject_id = fgl.gObject_id ");
            sb.AppendLine("LEFT JOIN Folder F on fgl.folder_id = F.folder_id ");
            sb.AppendLine("LEFT OUTER JOIN instance I on O.gObject_id = I.gObject_ID ");
            sb.AppendLine("WHERE O.Is_Hidden = 0 ");
            sb.AppendLine("AND (I.mx_platform_id = 0 OR I.mx_platform_id IS NULL) ");
            sb.AppendLine("AND F.Folder_id IS NOT NULL ");
            sb.AppendLine("AND O.tag_name NOT LIKE '$%' ");
            sb.AppendFormat("AND F.folder_id IN ({0})", ParentFolderID);
            sb.AppendLine();
            sb.AppendLine("ORDER BY O.tag_name ");

            SqlConnection sqlConn = null;
            if (this.GetSqlConnection(out sqlConn)) {
                try {
                    SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), sqlConn);
                    DataTable dt = new DataTable("g");
                    da.Fill(dt);

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {
                        foreach (DataRow dr in dt.Rows) {
                            results.Add(new ArchestrATreeNode(ArchestrATreeNodeType.Graphic, (String)dr[0], 5));
                        }
                    }
                    dt.Dispose();
                    da.Dispose();
                } catch (Exception ex) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to retrieve the graphic toolset symbols.");
                }
            } else {
                if (TreeViewRefreshStatus != RefreshStatus.Warning) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to establish a connection to the galaxy database.");
                }
            }
            return results;
        }

        private List<ArchestrATreeNode> GetGraphicToolsetNodes(Int32 ParentFolderID) {
            List<ArchestrATreeNode> results = new List<ArchestrATreeNode>();
            if (TreeViewRefreshStatus == RefreshStatus.Warning) return results;
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT f1.folder_id, CAST(f1.folder_name AS VARCHAR(MAX)) AS folder_name, CAST(f1.folder_name AS VARCHAR(MAX)) AS fullname, f1.has_objects, f1.has_folders, f1.parent_folder_id, f2.folder_name as Parent,0 as level ");
            sb.AppendLine("FROM folder f1 LEFT JOIN folder f2 ON f1.parent_folder_id = f2.folder_id ");
            sb.AppendLine("WHERE f1.folder_type = 2 ");
            sb.AppendFormat("and f2.folder_id = {0}", ParentFolderID);
            sb.AppendLine();
            sb.AppendLine("order by f1.folder_name");

            SqlConnection sqlConn = null;
            if (this.GetSqlConnection(out sqlConn)) {
                try {
                    SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), sqlConn);
                    DataTable dt = new DataTable("Next");
                    da.Fill(dt);

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {
                        foreach (DataRow dr in dt.Rows) {
                            ArchestrATreeNode atn = null;
                            if ((Boolean)dr[3] || (Boolean)dr[4]) { // Has other folder and/or graphics in it
                                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Folder, (String)dr[1], 7);
                            }

                            if ((Boolean)dr[4]) {   // Subfolders exist...
                                List<ArchestrATreeNode> atnF = this.GetGraphicToolsetNodes((Int32)dr[0]);
                                if (atnF != null && atnF.Count > 0) {
                                    atn.Nodes.AddRange(atnF.ToArray());
                                }
                            }

                            if ((Boolean)dr[3]) {  // Graphics exist...
                                List<ArchestrATreeNode> atnG = this.GetGraphicSymbols((Int32)dr[0]);
                                if (atnG != null && atnG.Count > 0) {
                                    atn.Nodes.AddRange(atnG.ToArray());
                                }
                            }

                            if (atn != null) results.Add(atn);
                        }
                    }
                } catch (Exception ex) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to retrieve the graphic toolsets.");
                }
            } else {
                if (TreeViewRefreshStatus != RefreshStatus.Warning) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to establish a connection to the galaxy database.");
                }
            }

            return results;
        }

        private List<ArchestrATreeNode> GetGraphicToolsetTopNodes(String ToolsetName) {
            Boolean nameSearch = false;
            List<ArchestrATreeNode> results = new List<ArchestrATreeNode>();
            if (TreeViewRefreshStatus == RefreshStatus.Warning) return results;
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT f1.folder_id, CAST(f1.folder_name AS VARCHAR(MAX)) AS folder_name, CAST(f1.folder_name AS VARCHAR(MAX)) AS fullname, f1.has_objects, f1.has_folders, f1.parent_folder_id, f2.folder_name as Parent,0 as level ");
            sb.AppendLine("FROM folder f1 LEFT JOIN folder f2 ON f1.parent_folder_id = f2.folder_id ");
            sb.AppendLine("WHERE f1.folder_type = 2 ");

            if (!String.IsNullOrEmpty(ToolsetName)) {
                sb.AppendFormat("and f1.folder_name = '{0}'", ToolsetName);
                sb.AppendLine();
                nameSearch = true;
            } else {
                sb.AppendLine("and f1.parent_folder_id = 0");
                nameSearch = false;
            }

            sb.AppendLine("order by f1.folder_name");

            SqlConnection sqlConn = null;
            if (this.GetSqlConnection(out sqlConn)) {
                try {
                    SqlDataAdapter topDA = new SqlDataAdapter(sb.ToString(), sqlConn);
                    DataTable topDT = new DataTable("Top");
                    topDA.Fill(topDT);

                    if (topDT != null && topDT.Rows != null && topDT.Rows.Count > 0) {
                        foreach (DataRow dr in topDT.Rows) {
                            ArchestrATreeNode atn = null;
                            if ((Boolean)dr[3] || (Boolean)dr[4]) { // Has other folder and/or graphics in it
                                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Folder, (String)dr[1], 7);
                            }

                            if ((Boolean)dr[4]) {   // Subfolders exist...
                                List<ArchestrATreeNode> fNodes = GetGraphicToolsetNodes((Int32)dr[0]);
                                if (fNodes != null && fNodes.Count > 0) {
                                    atn.Nodes.AddRange(fNodes.ToArray());
                                }
                            }

                            if ((Boolean)dr[3]) {  // Graphics exist...
                                List<ArchestrATreeNode> gNodes = GetGraphicSymbols((Int32)dr[0]);
                                if (gNodes != null && gNodes.Count > 0) {
                                    atn.Nodes.AddRange(gNodes.ToArray());
                                }
                            }
                            if (atn != null) results.Add(atn);
                        }
                    } else {
                        if (nameSearch) SetStatusInfo(RefreshStatus.Warning, String.Format("Unable to load the specified graphic toolset, {0}", ToolsetName));
                    }
                } catch (Exception ex) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to retrieve the top level graphic toolsets.");
                }
            } else {
                if (TreeViewRefreshStatus != RefreshStatus.Warning) {
                    SetStatusInfo(RefreshStatus.Warning, "An unexpected problem was encountered while attempting to establish a connection to the galaxy database.");
                }
            }

            return results;
        }

        private TreeNode GetParentTreeNode(String Parent, TreeNode node) {
            TreeNode result = null;

            if (String.Compare(Parent, node.Text, true) == 0) {
                result = node;
            } else {
                if (node.Nodes != null && node.Nodes.Count > 0) {
                    foreach (TreeNode tn in node.Nodes) {
                        result = this.GetParentTreeNode(Parent, tn);
                    }
                }
            }

            return result;
        }

        private ArchestrATreeNode GetPlatformTreeNode(String PlatformName, String PlatformID) {
            ArchestrATreeNode atn = null;

            if (this.DisplayWinPlatforms) {
                String[] graphicNames = this.GetGraphicNames(PlatformName);

                if (graphicNames.Length == 0) {
                    if (PlatformID == "1") {
                        atn = new ArchestrATreeNode(ArchestrATreeNodeType.Platform, PlatformName, 2);
                    } else {
                        atn = new ArchestrATreeNode(ArchestrATreeNodeType.Platform, PlatformName, 3);
                    }
                } else {
                    if (PlatformID == "1") {
                        atn = new ArchestrATreeNode(ArchestrATreeNodeType.Platform, graphicNames, PlatformName, 2);
                        if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                    } else {
                        atn = new ArchestrATreeNode(ArchestrATreeNodeType.Platform, graphicNames, PlatformName, 3);
                        if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                    }
                }
            }

            return atn;
        }

        private Boolean GetSqlConnection(out SqlConnection sqlConn) {
            String strConn = String.Empty;
            sqlConn = null;
            Boolean result = false;

            try {
                if (!String.IsNullOrEmpty(this.GrServerName) && !String.IsNullOrEmpty(this.GrDbName)) {
                    if (this.GrUseWindowsAuthentication) {
                        strConn = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", this.GrServerName, this.GrDbName);
                    } else {
                        if (!String.IsNullOrEmpty(this.GrUserName) && !String.IsNullOrEmpty(this._grPassword)) {
                            strConn = String.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", this.GrServerName, this.GrDbName, this.GrUserName, this._grPassword);
                        } else {
                            this.SetStatusInfo(RefreshStatus.Warning, "Invalid galaxy database authentication information provided!");
                            sqlConn = null;
                            result = false;
                        }
                    }
                } else {
                    this.SetStatusInfo(RefreshStatus.Warning, "Invalid galaxy database information provided!");
                    sqlConn = null;
                    result = false;
                }

                if (!String.IsNullOrEmpty(strConn)) {
                    sqlConn = new SqlConnection(strConn);
                    result = true;
                } else {
                    // Just making sure...not necessary really
                    sqlConn = null;
                    result = false;
                }
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "Invalid galaxy database information provided!");
                sqlConn = null;
                result = false;
            }
            return result;
        }

        private ArchestrATreeNode GetTopLevelAreaMatch(string Name, ArchestrATreeNode node) {
            ArchestrATreeNode matchedNode = null;
            if (string.IsNullOrEmpty(Name)) {
                matchedNode = node;
            }

            if (string.Compare(node.Text, Name, true) == 0) {
                matchedNode = node;
            }

            if (matchedNode == null) {
                foreach (ArchestrATreeNode atn in node.Nodes) {
                    if (atn.ArchestrATreeNodeType == ArchestrATreeNodeType.Area) {
                        if (string.Compare(atn.Text, Name, true) == 0) {
                            matchedNode = atn;
                            break;
                        }

                        foreach (ArchestrATreeNode newATN in atn.Nodes) {
                            ArchestrATreeNode topLevelAreaMatch = this.GetTopLevelAreaMatch(Name, newATN);
                            if (topLevelAreaMatch != null) {
                                matchedNode = topLevelAreaMatch;
                            }
                        }
                    }
                }
            }
            return matchedNode;
        }

        private System.IO.MemoryStream GetValidHierarchyData() {
            MemoryStream xmlStream = null;
            Microsoft.Win32.RegistryKey rk = null;
            try {
                Boolean success = false;
                rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\ArchestrA\\Framework");

                Object o = rk.GetValue("RootPath");

                if (o == null) {
                    rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\ArchestrA\\Framework");
                    o = rk.GetValue("RootPath");
                }

                if (o == null) {
                    this.SetStatusInfo(RefreshStatus.Warning, "Unable to locate area model data.  A platform must be deployed to this node before the control can contain navigation data.");
                    return xmlStream;
                }

                String aaRootPath = (String)o;
                String FullFileName = string.Format("{0}\\GlobalDataCache\\AreaHierarchy.xml", aaRootPath);
                FileStream fs; // = new FileStream(FullFileName, FileMode.Open, FileAccess.Read);
                Byte[] streamBuffer = new Byte[102400]; //Dim streamBuffer[102400] As System.Byte; 'Holds 100Kb.

                MemoryStream ms;
                BinaryReader streamReader;
                Int32 length;


                // Load the file.
                fs = new System.IO.FileStream(FullFileName, FileMode.Open, FileAccess.Read);

                // Transfer the file to a byte array buffer.
                fs.Read(streamBuffer, (int)0, (int)fs.Length);
                length = (int)fs.Length; // Needed to cast a long into an integer 
                fs.Close();

                // Load the memory stream with the buffer.
                ms = new System.IO.MemoryStream(streamBuffer, (int)0, length);

                // Create the reader for the memory stream.
                streamReader = new System.IO.BinaryReader(ms);

                //Locate the beginning of the xml code within the file.
                //The file has some binary data as a header that prevents
                //the .Net xml classes to load it.

                Int32 offset = this.AreaHierarchyStartingPosition;
                Char[] headerBuffer = new Char[5];
                String valueRead; //Holds the string representation of the buffer

                // Locate the beginning of the xml code.
                Int32 itChecker = 0;

                while (true) {
                    itChecker++;
                    ms.Seek(offset, SeekOrigin.Begin);
                    streamReader.Read(headerBuffer, 0, 4);
                    valueRead = new System.String(headerBuffer);

                    if (valueRead.ToUpper().StartsWith("<ROO")) {
                        success = true;
                        break;
                    }

                    if (itChecker == 1000) break;

                    offset++;
                }


                if (success) {
                    // Transfer the xml code from the memory (file) stream
                    // to a new memory (xml) stream.
                    // The following code uses the streamBuffer and length
                    // variables declared above.
                    length = (int)ms.Length - (int)offset;
                    ms.Seek(offset, SeekOrigin.Begin);
                    ms.Read(streamBuffer, (int)0, (int)length);
                    xmlStream = new MemoryStream(streamBuffer, (int)0, (int)length);
                    ms.Close();
                } else {
                    this.SetStatusInfo(RefreshStatus.Warning, "Error parsing area hierarchy file...");
                }

            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "An unexpected error was encountered while trying to prase the area hierarchy file...");
            }

            return xmlStream;
        }

        private ArchestrATreeNode GetViewEngineTreeNode(String EngineName) {
            ArchestrATreeNode atn = null;

            if (this.DisplayViewEngines && !this.IsHideMatch(EngineName)) {
                atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, EngineName, 8);
                String[] graphicNames = this.GetGraphicNames(EngineName);

                if (graphicNames.Length == 0) {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, EngineName, 8);
                } else {
                    atn = new ArchestrATreeNode(ArchestrATreeNodeType.Engine, graphicNames, EngineName, 8);
                    if (this.DisplayGraphics) atn.Nodes.AddRange(this.GetGraphicsNodes(graphicNames));
                }
            }

            return atn;
        }

        private Boolean IsHideMatch(String Reference) {
            Boolean result = true;

            if (String.IsNullOrEmpty(HideObjectNameStartingWith)) {
                result = false;
            } else {
                if (Reference.ToUpper().StartsWith(HideObjectNameStartingWith.ToUpper())) {
                    result = true;
                } else {
                    result = false;
                }
            }
            return result;
        }

        private List<ArchestrATreeNode> RefreshObjectModelData() {
            List<ArchestrATreeNode> nodes = new List<ArchestrATreeNode>();
            Boolean result = true;

            if (this.TreeViewRefreshStatus == RefreshStatus.Warning) {
                return nodes;
            } else {
                this.SetStatusInfo(RefreshStatus.Refreshing, "Loading/Refreshing List...");
            }

            // Declare XML variables
            XPathNavigator nav = null;
            XPathDocument docNav = null;
            XPathNodeIterator nodeIter = null;

            // Load the Plant Model XMl file into XPath
            try {
                docNav = new System.Xml.XPath.XPathDocument(this.GetValidHierarchyData());
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "Error loading Area Hierarchy file data...");
                nodes = null;
            }

            if (nodes == null) return new List<ArchestrATreeNode>();     // Bail if a problem was encountered

            try {
                nav = docNav.CreateNavigator();

                // Configure filter string based on config
                String strFilter = String.Empty;
                strFilter = this.GetFilterExpression();

                if (this.TreeViewRefreshStatus != RefreshStatus.Warning) {
                    // Select the node(s) and place the results in an iterator.
                    nodeIter = nav.Select(strFilter);
                }
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "Error filtering records.  Invalid Area Hierarchy?");
                nodes = null;
            }

            if (nodes == null) return new List<ArchestrATreeNode>();     // Bail if a problem was encountered

            // Iterate through the results showing the element value.
            try {
                while (nodeIter.MoveNext()) {
                    String ParentName;
                    String AreaName = nodeIter.Current.GetAttribute("Name", "");
                    String AreaCategory = nodeIter.Current.GetAttribute("Category", "");
                    String AreaPlatformID = nodeIter.Current.GetAttribute("PlatformID", "");
                    ArchestrATreeNode tn = null;

                    if (!this.IsHideMatch(AreaName)) {
                        switch (AreaCategory) {
                            case "13":  // Area
                                tn = this.GetAreaTreeNode(AreaName);

                                List<ArchestrATreeNode> children = this.GetAppObjectTreeNodesByArea(AreaName);
                                if (children != null && children.Count > 0) tn.Nodes.AddRange(children.ToArray());

                                break;
                            case "1":   // Platform
                                tn = this.GetPlatformTreeNode(AreaName, AreaPlatformID);
                                break;
                            case "3":   // AppEngine
                                tn = this.GetAppEngineTreeNode(AreaName);
                                break;
                            case "4":   // ViewEngine
                                tn = this.GetViewEngineTreeNode(AreaName);
                                break;
                            case "11":  // DI Object
                                tn = this.GetDIObjectTreeNode(AreaName);
                                break;
                            case "24":  // RDI Object
                                tn = this.GetRDIObjectTreeNode(AreaName);
                                break;
                            default:    // Weird...this is something else
                                break;
                        }
                    }

                    if (tn != null) {
                        nav = nodeIter.Current;
                        nav.MoveToParent();                     //Get the Area's Parent
                        ParentName = nav.GetAttribute("Name", "");

                        if (ParentName == String.Empty) {
                            nodes.Add(tn);                      // This is a top level node
                        } else {
                            foreach (TreeNode tnI in nodes) {   // This is not a top-level node...find the right parent and add it.
                                TreeNode tnParent = this.GetParentTreeNode(ParentName, tnI);

                                if (tnParent != null) {
                                    tnParent.Nodes.Add(tn);
                                    break;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, String.Format("An unexpected error was encountered while attempting to load/refersh.\n\r{0}", ex.Message));
                result = false;
            }


            if (result) {
                if (this.TreeViewRefreshStatus != RefreshStatus.Warning) {
                    this.SetStatusInfo(RefreshStatus.Success, "Load/Refresh Complete");
                }
            } else {
                return new List<ArchestrATreeNode>();     // Bail if a problem was encountered
            }

            // Locate node matching top level area name property (if exists)
            if (!String.IsNullOrEmpty(TopLevelAreaName)) {
                ArchestrATreeNode mNode = null;
                foreach (ArchestrATreeNode atn in nodes) {
                    mNode = this.GetTopLevelAreaMatch(TopLevelAreaName, atn);
                    if (mNode != null) {
                        break;
                    }
                }

                if (mNode != null) {
                    nodes.Clear();
                    nodes.Add(mNode);
                }
            }
            //nodes.Sort();
            return nodes;
        }

        private List<ArchestrATreeNode> RefreshPlantModelPicker() {
            List<ArchestrATreeNode> allNodes = new List<ArchestrATreeNode>();
            if (this.DisplayObjectModel) allNodes.AddRange(this.RefreshObjectModelData());
            if (this.DisplayGraphicToolsets) allNodes.AddRange(GetGraphicToolsetTopNodes(this.TopLevelGraphicToolsetName));
            return allNodes;
        }

        #endregion

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

        #endregion

        #region Event Handler(s)

        private void _bw_DoWork(object sender, DoWorkEventArgs e) {
            List<ArchestrATreeNode> nodes = RefreshPlantModelPicker();
            e.Result = nodes;
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            // This is necessary due to the creation of UI elements on a different thread than the UI.  This marshals things onto the UI thread
            try {
                List<ArchestrATreeNode> nodes = (List<ArchestrATreeNode>)e.Result;

                // Clean it up?
                List<ArchestrATreeNode> finishNodes = new List<ArchestrATreeNode>();
                foreach (ArchestrATreeNode n in nodes) {
                    if (n.ArchestrATreeNodeType == ArchestrATreeNodeType.Folder) {
                        if (n.Nodes != null && n.Nodes.Count > 0) {
                            if (n.Parent == null) {
                                finishNodes.Add(n);
                            }
                        }
                    } else {
                        finishNodes.Add(n);
                    }
                }
                tvMain.Invoke(new addNodes(AddTheNodes), new object[] { finishNodes });
                this.OnRefreshComplete(EventArgs.Empty);
                _refreshing = false;
            } catch (Exception ex) {
                this.SetStatusInfo(RefreshStatus.Warning, "An error was unexpectedly encountered while trying to finalize the refreshing of the TreeView.");
            }
        }

        private void tsiStatus_Click(object sender, EventArgs e) {
            if (!_refreshing) {
                this.RefreshTreeView();
            }
        }

        private void tsiExpand_Click(object sender, EventArgs e) {
            this.ExpandAll();
        }

        private void tsiMinus_Click(object sender, EventArgs e) {
            this.CollapseAll();
        }

        private void tvMain_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            if (e.Action == TreeViewAction.Collapse) {
                _expandDebounce = true;
            }
        }

        private void tvMain_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            if (e.Action == TreeViewAction.Expand) {
                _expandDebounce = true;
            }
        }

        private void tvMain_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (tvMain.SelectedNode != null) {
                    try {
                        ArchestrATreeNode atn = (ArchestrATreeNode)tvMain.SelectedNode;
                        if (atn != null) {
                            //tvMain_AfterSelect(this, new TreeViewEventArgs(atn, TreeViewAction.ByMouse));
                            RaiseNodeSelectedEvent(atn);
                        }
                    } catch {

                    }
                }
            }
        }

        private void tvMain_MouseUp(object sender, MouseEventArgs e) {
            if (!_expandDebounce) {
                TreeViewHitTestInfo tvi = tvMain.HitTest(e.Location.X, e.Location.Y);
                if (tvi != null && tvi.Node != null) {
                    RaiseNodeSelectedEvent((ArchestrATreeNode)tvi.Node);
                }
            } else {
                _expandDebounce = false;
            }
        }

        #endregion

    }

}
