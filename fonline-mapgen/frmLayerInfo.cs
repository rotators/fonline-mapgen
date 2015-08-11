using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FOCommon.Maps;

namespace fonline_mapgen
{
    public partial class frmLayerInfo : Form
    {
        /// <summary>
        /// Parent form which calls this
        /// </summary>
        public Action refresh;

        /// <summary>
        /// Update copy buffer to map
        /// </summary>
        public Action updateCopyBufferToMap;

        /// <summary>
        /// Current public selection buffer
        /// </summary>
        public List<Selection> copyBuffer;

        /// <summary>
        /// Current map
        /// </summary>
        public MapperMap currentMap;

        public frmLayerInfo()
        {
            InitializeComponent();
        }


        public List<object> getSelectedItems()
        {
            return null;
        }

        private void frmDebugInfo_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void lstBuffer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteItem(lstBuffer, currentMap.Selection);
            }
        }

        private void lstMainBuffer_DragDrop(object sender, DragEventArgs e)
        {
            DragDropItems(lstBuffer, lstMainBuffer, copyBuffer);
        }

        private void lstBuffer_DragDrop(object sender, DragEventArgs e)
        {
            DragDropItems(lstMainBuffer, lstBuffer, currentMap.Selection);
            updateCopyBufferToMap();
        }

        private void lstBuffer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void lstMainBuffer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void lstMainBuffer_ItemDrag(object sender, ItemDragEventArgs e)
        {
            lstMainBuffer.DoDragDrop(lstMainBuffer.SelectedItems, DragDropEffects.Copy);
        }

        private void lstBuffer_ItemDrag(object sender, ItemDragEventArgs e)
        {
            lstBuffer.DoDragDrop(lstBuffer.SelectedItems, DragDropEffects.Copy);
        }

        /// <summary>
        /// Drag and drop between lists
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="buffer"></param>
        private void DragDropItems(ListView from, ListView to, List<Selection> buffer)
        {
            foreach (ListViewItem item in from.SelectedItems)
            {
                var cloned = (ListViewItem)item.Clone();
                Selection sel = (Selection)((ListViewItem)item).Tag;
                sel.Added = false;
                buffer.Add(sel);
            }

            // Update list boxes according to the buffer
            if (refresh != null)
            {
                refresh();
            }
        }

        private void DeleteItem(ListView from, List<Selection> buffer)
        {
            foreach (var item in from.SelectedItems)
            {
                // Lazy object conversion
                Selection sel = (Selection)((ListViewItem)item).Tag;
                currentMap.RemoveSelection(sel);
                buffer.Remove(sel);
            }

            // Update list boxes according to the buffer
            if (refresh != null)
            {
                refresh();
            }
        }

        private void lstMainBuffer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteItem(lstMainBuffer, copyBuffer);
            }
        }
    }
}
