using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ignostic.Studio256.RenderApi.Tools
{
    public partial class TimelineControl : Control
    {
        public TimelineModel Model { get; set; }
        public float TimeScale { get; set; }
        public float TimeOffset { get; set; }
        public SceneItem SelectedItem { get; set; }


        public TimelineControl()
        {
            InitializeComponent();
            Model = new TimelineModel();
            TimeScale = 1;
            DoubleBuffered = true;
            BackColor = Color.FromArgb(32, 32, 32);
        }


        protected override void OnKeyUp(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.Add:
                    Model.Scenes.Add(new SceneItem
                    {
                        Name = "Name",
                        Duration = 10,
                        TimeStart = 0,
                        RowIndex = 0,
                    });
                    Invalidate();
                    break;
            }
        }


        protected override void OnPaint(PaintEventArgs args)
        {
            foreach (var sceneItem in Model.Scenes)
            {
                var rect = CreatePixelRectangle(sceneItem);
                var g = args.Graphics;
                g.FillRectangle(Brushes.Black, rect);
                g.DrawRectangle(Pens.LightGray, rect);
                using (var font = new Font("Arial", 12))
                {
                    var rectf = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                    var stringFormat = StringFormat.GenericDefault;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    g.DrawString(sceneItem.Name, font, Brushes.LightGray, rectf, stringFormat);
                }
            }
        }


        protected override void OnMouseDown(MouseEventArgs args)
        {
            SelectedItem = GetItemAt(args.Location);
            Capture = true;
        }


        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (SelectedItem == null)
                return;

            SelectedItem.TimeStart = Math.Round(PixelsToTime(args.X));
            SelectedItem.RowIndex = PixelsToRow(args.Y);
            Invalidate();
        }


        protected override void OnMouseUp(MouseEventArgs args)
        {
            SelectedItem = null;
            Capture = false;
        }


        private int TimeToPixels(double time)
        {
            return (int)(7.2 * time * TimeScale);
        }


        private double PixelsToTime(int pixel)
        {
            return pixel / (7.2 * TimeScale);
        }

        
        private int RowToPixels(int rowIndex)
        {
            return (int)(36 * rowIndex);
        }


        private int PixelsToRow(int pixel)
        {
            return (int)(pixel / 36);
        }


        private SceneItem GetItemAt(Point pixelPoint)
        {

            return Model.Scenes
                .Where(sceneItem => CreatePixelRectangle(sceneItem).Contains(pixelPoint))
                .FirstOrDefault();
        }


        private Rectangle CreatePixelRectangle(SceneItem sceneItem)
        {
            return new Rectangle
            {
                X = TimeToPixels(sceneItem.TimeStart),
                Y = RowToPixels(sceneItem.RowIndex),
                Width = TimeToPixels(sceneItem.Duration),
                Height = RowToPixels(1)
            };
        }


        private void SetPosition(SceneItem sceneItem, Point pixelPoint)
        {
            sceneItem.TimeStart = PixelsToTime(pixelPoint.X);
            sceneItem.RowIndex = PixelsToRow(pixelPoint.Y);
        }
    }
}
