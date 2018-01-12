using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ww.nasc.ui.custom {
    public partial class SelectionEventArgs : EventArgs {

        #region Declaration(s)
        String _selectedNode = String.Empty;
        ArchestrATreeNodeType _selectedNodeType = ArchestrATreeNodeType.Unknown;
        ArchestrATreeNode _tn = null;
        #endregion

        #region Properties

        public ArchestrATreeNode ArchestrATreeNode {
            get { return _tn; }
        }

        public String SelectedNode {
            get { return _selectedNode; }
        }

        public ArchestrATreeNodeType SelectedNodeType {
            get { return _selectedNodeType; }
        }

        #endregion

        #region Constructor(s)

        public SelectionEventArgs(String SelectedNode, ArchestrATreeNodeType SelectedType, ArchestrATreeNode ArchestrATreeNode) {
            this._selectedNode = SelectedNode;
            this._selectedNodeType = SelectedType;
            this._tn = ArchestrATreeNode;
        }

        #endregion

        #region Function(s)

        public override bool Equals(object obj) {
            bool result = false;
            
            if (obj != null && obj is string) {
                if (String.Compare((string)obj, this.ToString()) == 0) {
                    result = true;
                }
            }
            return result;
       }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        public override string ToString() {
            return _selectedNode;
        }

        #endregion

    }
}
