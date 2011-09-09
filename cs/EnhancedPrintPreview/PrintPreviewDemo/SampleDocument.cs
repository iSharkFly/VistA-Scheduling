using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;

namespace PrintPreviewDemo {
    class SampleDocument {

        const int LineHeight = 30;
        const int LinesToPrint = 300;


        PrintDocument pdoc;
        int Lines = 0;

        public PrintDocument PrintDocument { get { return pdoc; } }

        public SampleDocument() {
            pdoc = new PrintDocument();
            pdoc.BeginPrint += new PrintEventHandler(pdoc_BeginPrint);
            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
        }

        void pdoc_BeginPrint(object sender, PrintEventArgs e) {
            Lines = 0;
        }

        void pdoc_PrintPage(object sender, PrintPageEventArgs e) {
            int CurrentY = e.MarginBounds.Top;
            Font f = new Font("Arial", 12);
            Rectangle r;
            while (CurrentY < e.MarginBounds.Bottom - LineHeight && Lines <= LinesToPrint) {
                r = new Rectangle(e.MarginBounds.Left, CurrentY, e.MarginBounds.Width, LineHeight);
                e.Graphics.DrawRectangle(Pens.Black, r);
                e.Graphics.DrawString("Row " + Lines.ToString(), f, Brushes.Black, r);
                CurrentY += LineHeight;
                Lines++;
            }
            e.HasMorePages = (Lines < LinesToPrint);
        }


    }
}
