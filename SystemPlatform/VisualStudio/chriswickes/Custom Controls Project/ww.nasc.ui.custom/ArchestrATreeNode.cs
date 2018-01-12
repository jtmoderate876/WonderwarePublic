using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ww.nasc.ui.custom {

    public class ArchestrATreeNode : System.Windows.Forms.TreeNode  {

        #region Declaration(s)
        ArchestrATreeNodeType _atnType = ArchestrATreeNodeType.Unknown;
        List<String> _graphics = new List<string>();
        #endregion

        #region Properties

        public ArchestrATreeNodeType ArchestrATreeNodeType {
            get { return _atnType; }
        }

        public String[] Graphics {
            get { return _graphics.ToArray(); }
        }

        #endregion

        #region Constructor(s)

        public ArchestrATreeNode(ArchestrATreeNodeType type)
            : base() {
            _atnType = type;
        }

        public ArchestrATreeNode(ArchestrATreeNodeType type, String[] Graphics)
            : this(type) {
            if (Graphics != null) {
                try {
                    _graphics.AddRange(Graphics);
                } catch (Exception ex) { 
                }
            }
        }

        public ArchestrATreeNode(ArchestrATreeNodeType type, String Text, Int32 ImageIndex)
            : base(Text, ImageIndex, ImageIndex) {
            _atnType = type;
        }

        public ArchestrATreeNode(ArchestrATreeNodeType type, String[] Graphics, String Text, Int32 ImageIndex)
            : base(Text, ImageIndex, ImageIndex) {
            _atnType = type;
            if (Graphics != null) {
                try {
                    _graphics.AddRange(Graphics);
                } catch (Exception ex) {
                }
            }
        }

        #endregion

        #region Function(s)

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        public override string ToString() {
            try {
                return String.Format("{0};{1};{2}", base.ToString(), this.ArchestrATreeNodeType.ToString(), this.Graphics.ToString());
            } catch (Exception ex) {

            }
            return String.Empty;
        }

        #endregion

    }

}
