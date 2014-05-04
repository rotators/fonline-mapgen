using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DATLib;
using System.IO;

using FOCommon.Maps;
using FOCommon.Graphic;
using FOCommon.Parsers;
using FOCommon.Items;
using System.Runtime.Serialization.Formatters.Binary;

namespace fonline_mapgen
{
    public partial class frmMain : Form
    {
        public List<String> GraphicsPaths = new List<string>();
        //public Dictionary<String, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();

        public Dictionary<String, FalloutFRM> Frms = new Dictionary<string, FalloutFRM>();
        List<ItemProto> items = new List<ItemProto>();

        FOCommon.Maps.FOHexMap hexmap;
        FOCommon.Parsers.FOMapParser parser;

        ItemProtoParser protoParser = new ItemProtoParser();

        public FOCommon.Maps.FOMap map;

        /*
        bool drawTiles = true;
        bool drawRoofs = true;
        bool drawCritters = true;
        bool drawItems = true;
        bool drawScenery = true;
        bool drawSceneryWalls = true;
        */
        DrawMap.Flags drawFlags;

        public frmMain()
        {
            InitializeComponent();

            // update drawFlags via form events
            menuViewTiles.Checked =
            menuViewRoofs.Checked =
            //TODO: menuViewCritters.Checked =
            menuViewItems.Checked =
            menuViewScenery.Checked =
            menuViewSceneryWalls.Checked =
                true;
        }

        private void Form1_Load( object sender, EventArgs e )
        {

        }

        private void LoadMap( string fileName )
        {
            parser = new FOCommon.Parsers.FOMapParser( fileName );
            parser.Parse();
            map = parser.Map;
            hexmap = new FOHexMap( new Size( map.Header.MaxHexX, map.Header.MaxHexY ) );
            this.Text = "Mapper Experiment - " + fileName;
        }

        public void WriteLog( string str )
        {
            File.AppendAllText( "./debug.log", str + Environment.NewLine );
        }

        public bool LoadDat( string DatPath, Color Transparency )
        {
            DatReaderError status;
            DAT loadedDat = DATReader.ReadDat( DatPath, out status );
            if( status.Error != DatError.Success )
            {
                MessageBox.Show( "Error loading " + DatPath + ": " + Environment.NewLine + status.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return false;
            }

            List<DATFile> files = new List<DATFile>();
            foreach( string path in this.GraphicsPaths )
            {
                files.AddRange( loadedDat.GetFilesByPattern( path ) );
            }

            foreach( DATFile file in files )
            {
                string ext = Path.GetExtension( file.FileName ).ToLower();
                if( !(ext == ".frm" || ext == ".png") )
                    continue;

                if( ext == ".frm" )
                {
                    var frm = FalloutFRMLoader.LoadFRM( file.GetData(), Transparency );
                    frm.FileName = file.Path.ToLower();
                    Frms[frm.FileName] = frm;
                }
                else
                {
                    System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter( typeof( Bitmap ) );
                    Bitmap bitmap = (Bitmap)tc.ConvertFrom( file.GetData() );
                }
            }

            loadedDat.Close();

            return true;
        }

        private void panel1_Paint( object sender, PaintEventArgs e )
        {
            if( Frms.Count == 0 )
                return;
            if( hexmap == null )
                return;

            DrawMap.OnGraphics( e.Graphics, map, hexmap, items, Frms, this.drawFlags );
        }

        private void panel1_MouseMove( object sender, MouseEventArgs e )
        {
            if( hexmap == null )
                return;
            var hex = hexmap.GetHex( new PointF( e.X, e.Y + 6.0f ) );
            lblMouseCoords.Text = string.Format( "Mouse Coords: {0},{1} - Hex: {2},{3}", e.X, e.Y, hex.X, hex.Y );

            //

            if( map.Objects.Count( x => x.MapX == hex.X && x.MapY == hex.Y ) == 0 )
                lblProtos.Text = "Proto: ";
            foreach( var obj in map.Objects.FindAll( x => x.MapX == hex.X && x.MapY == hex.Y ) )
            {
                if( !(obj.MapObjType == FOCommon.Maps.MapObjectType.Item ||
                      obj.MapObjType == FOCommon.Maps.MapObjectType.Scenery) )
                    continue;
                //MessageBox.Show(""+obj.ProtoId);
                //if()

                ItemProto prot = items.Where( x => x.ProtoId == obj.ProtoId ).FirstOrDefault();
                if( prot == null )
                    continue;

                lblProtos.Text = "Proto: " + (obj.ProtoId);
                lblProtos.Text += string.Format( " ({0} - {1})", prot.Name, prot.PicMap );
            }
        }

        private void btnLoadMap_Click( object sender, EventArgs e )
        {
            LoadMap( (string)cmbMaps.SelectedItem );
            pnlViewPort.Invalidate();
            pnlViewPort.Refresh();
        }

        private void headerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if( this.map == null )
            {
                MessageBox.Show( "Map not loaded!" );
                return;
            }

            frmHeaderEditor formHeaderEditor = new frmHeaderEditor( this.map.Header );
            formHeaderEditor.ShowDialog();
        }

        private void frmMain_Paint( object sender, PaintEventArgs e )
        {
            if( Frms.Count != 0 )
                return;

            GraphicsPaths.Add( "art\\tiles" );
            GraphicsPaths.Add( "art\\misc" );
            GraphicsPaths.Add( "art\\walls" );
            GraphicsPaths.Add( "art\\door" );
            GraphicsPaths.Add( "art\\scenery" );

            Stream stream;
            BinaryFormatter formatter = new BinaryFormatter();

            if( File.Exists( "./graphics.dat" ) )
            {
                stream = File.OpenRead( "./graphics.dat" );
                Frms = (Dictionary<String, FalloutFRM>)formatter.Deserialize( stream );
            }
            else
            {
                foreach( string dataFile in UGLY.DataFiles )
                {
                    LoadDat( dataFile, Color.FromArgb( 11, 0, 11 ) );
                }

                stream = File.Create( "./graphics.dat" );
                formatter.Serialize( stream, Frms );
            }

            if( File.Exists( "./items.dat" ) )
            {
                stream = File.OpenRead( "./items.dat" );
                items = (List<ItemProto>)formatter.Deserialize( stream );
            }
            else
            {
                MSGParser FOObj = new MSGParser( UGLY.ServerDir + @"text\engl\FOOBJ.MSG" );
                FOObj.Parse();

                protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\door.fopro", "1.0", FOObj, items, null );
                protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\misc.fopro", "1.0", FOObj, items, null );
                protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\generic.fopro", "1.0", FOObj, items, null );
                protoParser.LoadProtosFromFile( UGLY.ServerDir + @"proto\items\wall.fopro", "1.0", FOObj, items, null );

                stream = File.Create( "./items.dat" );
                formatter.Serialize( stream, items );
            }

            cmbMaps.Items.AddRange( Directory.GetFiles( UGLY.ServerDir + @"maps\", "*.fomap" ) );

            //string fileName = UGLY.ServerDir+@"maps\hq_camp.fomap";
            //LoadMap(UGLY.ServerDir + @"maps\den.fomap");
        }

        #region Menu functions

        private void menuFileOpen_Click( object sender, EventArgs e )
        {
            if( openMapDialog.ShowDialog( this ) == DialogResult.OK && File.Exists( openMapDialog.FileName ) )
            {
                menuFileExport.Enabled = true;
                LoadMap( openMapDialog.FileName );
                panel1.Refresh();
            }
        }

        private void menuFileExportImage_Click( object sender, EventArgs e )
        {
            SaveFileDialog save = new SaveFileDialog();
            if( save.ShowDialog( this ) == DialogResult.OK )
            {
                Bitmap bmp = new Bitmap( panel1.ClientRectangle.Width, panel1.ClientRectangle.Height );
                panel1.DrawToBitmap( bmp, panel1.ClientRectangle );
                bmp.Save( save.FileName );
            }
        }

        private void UpdateDrawFlags( object sender, DrawMap.Flags flag )
        {
            if( ((ToolStripMenuItem)sender).Checked )
                this.drawFlags = this.drawFlags | flag;
            else
                this.drawFlags = this.drawFlags & ~flag;

            panel1.Refresh();
        }

        private void menuViewTiles_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Tiles );
        }

        private void menuViewRoofs_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Roofs );
        }

        private void menuViewCritters_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Critters );
        }

        private void menuViewItems_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Items );
        }

        private void menuViewScenery_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.Scenery );
        }

        private void menuViewSceneryWalls_CheckedChanged( object sender, EventArgs e )
        {
            this.UpdateDrawFlags( sender, DrawMap.Flags.SceneryWalls );
        }

        #endregion // Menu functions
    }
}
