using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FOCommon.Items;

namespace fonline_mapgen
{
    public class EditorData
    {
        public EditorData(ErrorMsg msgDelegate)
        {
            this.CallErrorMsg = msgDelegate;
        }

        public delegate void ErrorMsg(string message); // MsgBox? Depends on UI.
        ErrorMsg CallErrorMsg = null;
        
        public List<string> mapsFiles = new List<string>(); // List of all maps
        public Dictionary<int, ItemProto> itemsPid = new Dictionary<int, ItemProto>();

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

        public void UpdateSelectFlags(bool enable, DrawMap.Flags flag)
        {
            if (enable)
                this.selectFlags = this.selectFlags | flag;
            else
                this.selectFlags = this.selectFlags & ~flag;
        }

        public void UpdateDrawFlags(bool enable, DrawMap.Flags flag)
        {
            if (enable)
                this.drawFlags = this.drawFlags | flag;
            else
                this.drawFlags = this.drawFlags & ~flag;
        }

        private int CurrentMapIdx = -1;
        private List<MapperMap> Maps = new List<MapperMap>();

        public DrawMap.Flags drawFlags;
        public DrawMap.Flags selectFlags;
    }
}
