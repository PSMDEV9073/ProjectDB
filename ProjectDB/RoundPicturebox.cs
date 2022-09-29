using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace ProjectDB
{
    public partial class RoundPicturebox : PictureBox
    {
        private int borderSize = 0;
        private Color borderColor = Color.RoyalBlue;
        private Color borderColor2 = Color.HotPink;
        private DashStyle borderLineStyle = DashStyle.Solid;
        private DashCap borderCapStyle = DashCap.Flat;
        private float gradientAngle = 50F;

        public RoundPicturebox()
        {
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        //Properties
        [Category("ProjectDB Code")]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }

            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }

        [Category("ProjectDB Code")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }

            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }

        [Category("ProjectDB Code")]
        public Color BorderColor2
        {
            get
            {
                return borderColor2;
            }

            set
            {
                borderColor2 = value;
                this.Invalidate();
            }
        }

        [Category("ProjectDB Code")]
        public DashStyle BorderLineStyle
        {
            get
            {
                return borderLineStyle;
            }

            set
            {
                borderLineStyle = value;
                this.Invalidate();
            }
        }

        [Category("ProjectDB Code")]
        public DashCap BorderCapStyle
        {
            get
            {
                return borderCapStyle;
            }

            set
            {
                borderCapStyle = value;
                this.Invalidate();
            }
        }

        [Category("ProjectDB Code")]
        public float GradientAngle
        {
            get
            {
                return gradientAngle;
            }

            set
            {
                gradientAngle = value;
                this.Invalidate();
            }
        }

        //Overridden methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            //Fields
            var graph = pe.Graphics;
            var rectContourSmooth = Rectangle.Inflate(this.ClientRectangle, -1, -1);
            var rectBorder = Rectangle.Inflate(rectContourSmooth, -borderSize, -borderSize);
            var smoothSize = borderSize > 0 ? borderSize * 3 : 1;
            using (var borderGColor = new LinearGradientBrush(rectBorder, borderColor, borderColor2, gradientAngle))
            using (var pathRegion = new GraphicsPath())
            using (var penSmooth = new Pen(this.Parent.BackColor, smoothSize))
            using (var penBorder = new Pen(borderGColor, borderSize))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                penBorder.DashStyle = borderLineStyle;
                penBorder.DashCap = borderCapStyle;
                pathRegion.AddEllipse(rectContourSmooth);
                //Set rounded region 
                this.Region = new Region(pathRegion);

                //Drawing
                graph.DrawEllipse(penSmooth, rectContourSmooth);//Draw contour smoothing
                if (borderSize > 0) //Draw border
                    graph.DrawEllipse(penBorder, rectBorder);
            }

        }
    }
}
