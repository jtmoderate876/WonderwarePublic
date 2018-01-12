using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ww.nasc.ui.custom {

    public enum ArchestrATreeNodeType {
        Unknown = 0,
        Area = 1,
        Platform = 2,
        Engine = 3,
        IO = 4,
        AppObject = 5,
        Graphic = 6,
        Folder = 7,
        RIO = 8
    }

    public enum NodeSize {
        XSmall = 8,
        Small = 12,
        Medium = 14,
        Large = 16,
        XLarge = 24
    }

    public enum RefreshStatus {
        Unknown = 0,
        Success = 1,
        Warning = 2,
        Refreshing = 3
    }

}
