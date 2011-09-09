using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace VR.PrintPreview {
    public enum TextPosition {
        HTopLeft, HTopCenter, HTopRight,
        HBottomLeft, HBottomCenter, HBottomRight,
        VTopLeft, VMiddleLeft, VBottomLeft,
        VTopRight, VMiddleRight, VBottomRight,
        WaterMark
    }

    public class AdditionalText {
        private string text;
        private Font font;
        private Brush brush;
        private TextPosition position;
        private int offsetX = 0;
        private int offsetY = 0;

        public string Text {
            get { return text; }
            set { text = value; }
        }

        public Font Font {
            get { return font; }
            set { font = value; }
        }

        [Browsable(false)]
        public Brush Brush {
            get { return brush; }
            set { brush = value; }
        }

        public TextPosition Position {
            get { return position; }
            set { position = value; }
        }

        public int OffsetX {
            get { return offsetX; }
            set { offsetX = value; }
        }

        public int OffsetY {
            get { return offsetY; }
            set { offsetY = value; }
        }

        public Color Color {
            get { return (brush is SolidBrush) ? ((SolidBrush)brush).Color : Color.Black; }
            set { brush = new SolidBrush(value); }
        }


        public AdditionalText(string text, Font font, Brush brush, TextPosition position, int offsetX, int offsetY) {
            this.text = text;
            this.font = (font != null) ? font : new Font("Arial", 12f);
            this.brush = (brush != null) ? brush : Brushes.Gray;
            this.position = position;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public AdditionalText(string text, TextPosition position) : this(text, null, null, position, 0, 0) { }

        public AdditionalText(string text, TextPosition position, int offsetX, int offsetY) : this(text, null, null, position, offsetX, offsetY) { }

        public AdditionalText(string text) : this(text, null, null, TextPosition.HBottomCenter, 0, 0) { }

    }
}
