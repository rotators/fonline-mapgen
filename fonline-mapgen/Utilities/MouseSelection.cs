using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Utilities
{
    public class MouseSelection
    {
        public RectangleF selectionArea;
        public Point clickedPos;
        public Point maxMouseRectPos;
        public Point mouseRectPos;
        public Pen rectPen;
        public bool isDown;
        public bool clicked;
        public bool isRightClickDown;

        public MouseSelection(Pen RectanglePen)
        {
            this.rectPen = RectanglePen;
        }

        public void CalculateSelectionArea()
        {
            this.selectionArea = new RectangleF(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);
            if (this.selectionArea.Width <= 0)
            {
                this.selectionArea.Width = 1;
                this.selectionArea.Height = 1;
                this.clicked = true;
            }
        }

        public void Click(Point point)
        {
            this.clicked = false;
            this.clickedPos = point;
            this.isDown = true;

            this.maxMouseRectPos.X = 0;
            this.maxMouseRectPos.Y = 0;

            this.selectionArea.Width = 1;
            this.selectionArea.Height = 1;
        }

        public void MouseUp()
        {
            this.isDown = false;
            this.clicked = false;

            this.CalculateSelectionArea();

            this.mouseRectPos.X = 0;
            this.mouseRectPos.Y = 0;
        }

        public Rectangle GetRect()
        {
            return new Rectangle(clickedPos.X, clickedPos.Y, mouseRectPos.X - clickedPos.X, mouseRectPos.Y - clickedPos.Y);
        }

        public void UpdateMaxRect()
        {
            if (mouseRectPos.X > maxMouseRectPos.X) maxMouseRectPos.X = mouseRectPos.X;
            if (mouseRectPos.Y > maxMouseRectPos.Y) maxMouseRectPos.Y = mouseRectPos.Y;
        }
    }
}
