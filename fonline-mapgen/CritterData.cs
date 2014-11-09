namespace fonline_mapgen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class CritterData
    {
        public Dictionary<int, string> crTypeGraphic = new Dictionary<int, string>();
        public Dictionary<int, int> crProtos = new Dictionary<int, int>(); // crType for critter pids 
    }
}
