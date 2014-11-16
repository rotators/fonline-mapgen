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

        public bool GetCritterType(int protoId, out string crTypeS)
        {
            int crType = 0;
            crProtos.TryGetValue(protoId, out crType);

            if (!crTypeGraphic.TryGetValue(crType, out crTypeS))
                return false;
            return true;
        }
    }
}
