using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace fonline_mapgen
{
    public partial class frmMapTree : Form
    {
        public frmMapTree(MapperMap map)
        {
            InitializeComponent();
            var header = treeView1.Nodes.Add("Map Header Data");
            this.Text = map.Name;

            Type type = typeof(FOCommon.Maps.MapHeader);
            foreach(var field in type.GetProperties())
            {
                 header.Nodes.Add(field.Name + " = " + field.GetValue(map.Header, null));
            }

            var tilesNode = treeView1.Nodes.Add("Tiles");
            List<string> addedTiles = new List<string>();
            foreach (var tile in map.Tiles)
            {
                if (addedTiles.Contains(tile.Path)) continue;
                addedTiles.Add(tile.Path);
                var tilePathNode = tilesNode.Nodes.Add(tile.Path);
                tilePathNode.Nodes.Add(map.Tiles.Count(x => x.Path == tile.Path).ToString() + " occurances");
            }

            var objectsNode = treeView1.Nodes.Add("Objects");
            var crNode = objectsNode.Nodes.Add("Critters");
            var scNode = objectsNode.Nodes.Add("Scenery");
            var itNode = objectsNode.Nodes.Add("Items");

            foreach (var obj in map.Objects)
            {
                //string objType = "Unknown";
                TreeNode parent = objectsNode;
                
                if (obj.MapObjType == FOCommon.Maps.MapObjectType.Critter)
                {
                    //objType = "Critter";
                    parent = crNode;
                }
                if (obj.MapObjType == FOCommon.Maps.MapObjectType.Item)             
                {
                    //objType = "Item";
                    parent = itNode;
                }
                if (obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery)
                {
                    //objType = "Scenery";
                    parent = scNode;
                }

                var node = parent.Nodes.Add("ProtoId " + obj.ProtoId + " [" + obj.MapX + "," + obj.MapY + "]");

                foreach (var kvp in obj.Properties)
                {
                    node.Nodes.Add(kvp.Key + " = " + kvp.Value);
                }
                if (obj.MapObjType == FOCommon.Maps.MapObjectType.Critter)
                {
                    foreach (var kvp in obj.CritterParams)
                    {
                        node.Nodes.Add(kvp.Key + " = " + kvp.Value);
                    }
                }
            }
            
        }
    }
}
