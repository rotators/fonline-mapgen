using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FOCommon.Items;

namespace fonline_mapgen
{
    public class EditorData
    {

        private int CurrentMapIdx = -1;
        private List<MapperMap> Maps = new List<MapperMap>();

        public DrawMap.Flags overlayFlags;
        public DrawMap.Flags drawFlags;
        public DrawMap.Flags selectFlags;

        public EditorData(ErrorMsg msgDelegate)
        {
            this.CallErrorMsg = msgDelegate;
        }

        public delegate void ErrorMsg(string message); // MsgBox? Depends on UI.
        ErrorMsg CallErrorMsg = null;

        public List<String> GraphicsPaths = new List<string>();
        public List<string> mapsFiles = new List<string>(); // List of all maps
        public Dictionary<int, ItemProto> itemsPid = new Dictionary<int, ItemProto>();
        public string overlayCritterFormat = "PID=%PID% [%P_ScriptName%@%P_FuncName%]\nBag=%P_ST_BAG_ID%";
        public string overlaySceneryFmt;

        public void AddMap(MapperMap Map)
        {
            Maps.Add(Map);
        }

        public MapperMap CurrentMap
        {
            get
            {
                if (this.CurrentMapIdx < 0 || this.CurrentMapIdx >= this.Maps.Count)
                    return (null);

                return (this.Maps[this.CurrentMapIdx]);
            }
            set
            {
                int idx = this.Maps.IndexOf(value);
                if (idx >= 0)
                    this.CurrentMapIdx = idx;
                else
                    CallErrorMsg("Can't set CurrentMapIndex");
            }
        }

        private DrawMap.Flags UpdateFlag(bool enable, DrawMap.Flags flag, DrawMap.Flags newFlag )
        {
            if (enable)
                flag = flag | newFlag;
            else
                flag = flag & ~newFlag;
            return flag;
        }

        public void UpdateOverlayFlags(bool enable, DrawMap.Flags flag)
        {
            overlayFlags = UpdateFlag(enable, overlayFlags, flag);
        }

        public void UpdateSelectFlags(bool enable, DrawMap.Flags flag)
        {
            selectFlags = UpdateFlag(enable, selectFlags, flag);
        }

        public void UpdateDrawFlags(bool enable, DrawMap.Flags flag)
        {
            drawFlags = UpdateFlag(enable, drawFlags, flag);
        }
    }
}
