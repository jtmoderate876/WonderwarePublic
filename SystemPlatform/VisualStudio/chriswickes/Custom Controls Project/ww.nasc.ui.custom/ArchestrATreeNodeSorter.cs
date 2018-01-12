using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ww.nasc.ui.custom {

    internal class ArchestrATreeNodeSorter : System.Collections.IComparer {

        public int Compare(ArchestrATreeNode x, ArchestrATreeNode y) {
            int result = 0;

            if (x == null && y == null) {
                result = 0;
            }

            if (x == null && y != null) {
                result = -1;
            }

            if (x != null && y == null) {
                result = 1;
            }
            if (x != null && y != null) {
                    if (x.ArchestrATreeNodeType == y.ArchestrATreeNodeType) {
                        result = String.Compare(x.Text, y.Text, false);
                    } else {
                        switch (x.ArchestrATreeNodeType) {
                            case ArchestrATreeNodeType.Area:
                                result = -1;
                                break;
                            case ArchestrATreeNodeType.Folder:
                                if (y.ArchestrATreeNodeType == ArchestrATreeNodeType.Area) {
                                    result = 1;
                                } else {
                                    result = -1;
                                }
                                break;
                            case ArchestrATreeNodeType.Graphic:
                                result = 1;
                                break;
                            default:
                                switch (y.ArchestrATreeNodeType) {
                                    case ArchestrATreeNodeType.Area:
                                        result = 1;
                                        break;
                                    case ArchestrATreeNodeType.Folder:
                                        result = 1;
                                        break;
                                    case ArchestrATreeNodeType.Graphic:
                                        result = -1;
                                        break;
                                    default:
                                        result = String.Compare(x.Text, y.Text, false);
                                        break;
                                }
                                break;
                        }
                    }
            }
            return result;
        }

        public int Compare(object x, object y) {
            int result = 0;

            if (x == null && y == null) {
                result = 0;
            }

            if (x == null && y != null) {
                result = -1;
            }

            if (x != null && y == null) {
                result = 1;
            }
            if (x != null && y != null) {
                if (x.GetType() == y.GetType() && x.GetType() == typeof(ArchestrATreeNode)) {
                    ArchestrATreeNode x1 = (ArchestrATreeNode)x;
                    ArchestrATreeNode y1 = (ArchestrATreeNode)y;

                    if (x1.ArchestrATreeNodeType == y1.ArchestrATreeNodeType) {
                        result = String.Compare(x1.Text, y1.Text, false);
                    } else {
                        switch (x1.ArchestrATreeNodeType) {
                            case ArchestrATreeNodeType.Area:
                                result = -1;
                                break;
                            case ArchestrATreeNodeType.Folder:
                                if (y1.ArchestrATreeNodeType == ArchestrATreeNodeType.Area) {
                                    result = 1;
                                } else {
                                    result = -1;
                                }
                                break;
                            case ArchestrATreeNodeType.Graphic:
                                result = 1;
                                break;
                            default:
                                switch (y1.ArchestrATreeNodeType) {
                                    case ArchestrATreeNodeType.Area:
                                        result = 1;
                                        break;
                                    case ArchestrATreeNodeType.Folder:
                                        result = 1;
                                        break;
                                    case ArchestrATreeNodeType.Graphic:
                                        result = -1;
                                        break;
                                    default:
                                        result = String.Compare(x1.Text, y1.Text, false);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return result;
        }

    }

}
