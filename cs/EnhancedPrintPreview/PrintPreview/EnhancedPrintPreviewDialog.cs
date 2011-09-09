// Code is under the CPOL license. Original by Vincenzo Rossi.
// Taken from http://www.codeproject.com/KB/printing/EnhancedPrintPreviewDlg.aspx

// Mods by Sam Habiel. Mods to remain under CPOL.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;

namespace VR.PrintPreview {

    public partial class EnhancedPrintPreviewDialog : Form {

        #region PRIVATE FIELDS

        PrintDocument mDocument;
        PageSetupDialog mPageSetupDialog;
        PrintDialog mPrintDialog;
        int mVisibilePages = 1;
        int mTotalPages = 0;
        bool mShowPageSettingsButton = true;
        bool mShowPrinterSettingsButton = false;
        bool mShowPrinterSettingsBeforePrint = true;
        List<AdditionalText> mAdditionalTextList;

        #endregion

        public EnhancedPrintPreviewDialog() {
            InitializeComponent();
            printPreviewControl1.StartPageChanged += new EventHandler(printPreviewControl1_StartPageChanged);
            ShowPrinterSettingsButton = false;
        }

        #region PROPERTIES

        public List<AdditionalText> AdditionalTextList {
            get {
                if (mAdditionalTextList == null) mAdditionalTextList = new List<AdditionalText>();
                return mAdditionalTextList;
            }
            set { mAdditionalTextList = value; }
        }

        public PageSetupDialog PageSetupDialog {
            get {
                if (mPageSetupDialog == null) mPageSetupDialog = new PageSetupDialog();
                return mPageSetupDialog;
            }
            set { mPageSetupDialog = value; }
        }

        public PrintDialog PrintDialog {
            get {
                if (mPrintDialog == null) mPrintDialog = new PrintDialog();
                return mPrintDialog;
            }
            set { mPrintDialog = value; }
        }

        public bool ShowPrinterSettingsButton {
            get { return mShowPrinterSettingsButton; }
            set {
                mShowPrinterSettingsButton = value;
                tsBtnPrinterSettings.Visible = value;
            }
        }

        public bool ShowPageSettingsButton {
            get { return mShowPageSettingsButton; }
            set {
                mShowPageSettingsButton = value;
                tsBtnPageSettings.Visible = value;
            }
        }

        public bool ShowPrinterSettingsBeforePrint {
            get { return mShowPrinterSettingsBeforePrint; }
            set { mShowPrinterSettingsBeforePrint = value; }
        }

        public PrintPreviewControl PrintPreviewControl { get { return printPreviewControl1; } }

        public PrintDocument Document {
            get { return mDocument; }
            set {
                SwitchPrintDocumentHandlers(mDocument, false);
                mDocument = value;
                SwitchPrintDocumentHandlers(mDocument, true);
                printPreviewControl1.Document = mDocument;
            }
        }

        public bool UseAntiAlias {
            get { return printPreviewControl1.UseAntiAlias; }
            set { printPreviewControl1.UseAntiAlias = value; }
        }

        #endregion


        #region DOCUMENT EVENT HANDLERS


        void mDocument_BeginPrint(object sender, PrintEventArgs e) {
            mTotalPages = 0;
        }

        void mDocument_EndPrint(object sender, PrintEventArgs e) {
            tsLblTotalPages.Text = " / " + mTotalPages.ToString();
        }

        void mDocument_PrintPage(object sender, PrintPageEventArgs e) {
            mTotalPages++;
            if (mAdditionalTextList == null) return;

            SizeF measure;
            PointF point = new PointF();
            Brush brush;
            StringFormat format = new StringFormat();
            string text;

            foreach (AdditionalText at in mAdditionalTextList) {
                text = at.Text.Replace("$pagenumber", mTotalPages.ToString());
                measure = e.Graphics.MeasureString(text, at.Font);
                brush = at.Brush;
                format = new StringFormat();

                switch (at.Position) {
                    case TextPosition.HTopLeft:
                        point.Y = e.MarginBounds.Top - measure.Height;
                        point.X = e.MarginBounds.Left;
                        break;
                    case TextPosition.HTopCenter:
                        point.Y = e.MarginBounds.Top - measure.Height;
                        point.X = e.MarginBounds.Left + (e.MarginBounds.Width - measure.Width) / 2;
                        break;
                    case TextPosition.HTopRight:
                        point.Y = e.MarginBounds.Top - measure.Height;
                        point.X = e.MarginBounds.Right - measure.Width;
                        break;
                    case TextPosition.HBottomLeft:
                        point.Y = e.MarginBounds.Bottom;
                        point.X = e.MarginBounds.Left;
                        break;
                    case TextPosition.HBottomCenter:
                        point.Y = e.MarginBounds.Bottom;
                        point.X = e.MarginBounds.Left + (e.MarginBounds.Width - measure.Width) / 2;
                        break;
                    case TextPosition.HBottomRight:
                        point.Y = e.MarginBounds.Bottom;
                        point.X = e.MarginBounds.Right - measure.Width;
                        break;
                    case TextPosition.VTopLeft:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.Y = e.MarginBounds.Top;
                        point.X = e.MarginBounds.Left - measure.Height;
                        break;
                    case TextPosition.VMiddleLeft:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.X = e.MarginBounds.Left - measure.Height;
                        point.Y = e.MarginBounds.Top + (e.MarginBounds.Height - measure.Width) / 2;

                        break;
                    case TextPosition.VBottomLeft:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.X = e.MarginBounds.Left - measure.Height;
                        point.Y = e.MarginBounds.Bottom - measure.Width;
                        break;
                    case TextPosition.VTopRight:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.Y = e.MarginBounds.Top;
                        point.X = e.MarginBounds.Right;
                        break;
                    case TextPosition.VMiddleRight:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.Y = e.MarginBounds.Top + (e.MarginBounds.Height - measure.Width) / 2;
                        point.X = e.MarginBounds.Right;
                        break;
                    case TextPosition.VBottomRight:
                        format.FormatFlags = StringFormatFlags.DirectionVertical;
                        point.Y = e.MarginBounds.Bottom - measure.Width;
                        point.X = e.MarginBounds.Right;
                        break;
                    case TextPosition.WaterMark:
                        point.Y = e.MarginBounds.Top + (e.MarginBounds.Height - measure.Height) / 2;
                        point.X = e.MarginBounds.Left + (e.MarginBounds.Width - measure.Width) / 2;
                        point.X += at.OffsetX;
                        point.Y += at.OffsetY;
                        int TranslationX = (int)(point.X + measure.Width / 2);
                        int TranslationY = (int)(point.Y + measure.Height / 2);
                        e.Graphics.TranslateTransform(TranslationX, TranslationY);
                        e.Graphics.RotateTransform(-55f);
                        point.X = -measure.Width / 2;
                        point.Y = -measure.Height / 2;
                        if (at.Brush is SolidBrush && ((SolidBrush)at.Brush).Color.A == 255)
                            brush = new SolidBrush(Color.FromArgb(100, ((SolidBrush)at.Brush).Color));

                        e.Graphics.DrawString(text, at.Font, brush, point, format);
                        e.Graphics.RotateTransform(55f);
                        e.Graphics.TranslateTransform(-TranslationX, -TranslationY);

                        break;
                    default:
                        break;
                }


                if (at.Position != TextPosition.WaterMark) {
                    point.X += at.OffsetX;
                    point.Y += at.OffsetY;
                    e.Graphics.DrawString(text, at.Font, brush, point, format);
                }
            }
        }

        #endregion


        #region TOOLBAR EVENT HANDLERS


        private void tsTxtCurrentPage_Leave(object sender, EventArgs e) {
            int startPage;
            if (Int32.TryParse(tsTxtCurrentPage.Text, out startPage)) {
                try {
                    startPage--;
                    if (startPage < 0) startPage = 0;
                    if (startPage > mTotalPages - 1) startPage = mTotalPages - mVisibilePages;
                    printPreviewControl1.StartPage = startPage;
                } catch { }
            }
        }

        private void NumOfPages_Click(object sender, EventArgs e) {
            ToolStripMenuItem mnuitem = (ToolStripMenuItem)sender;
            tsDDownPages.Image = mnuitem.Image;
            mVisibilePages = Int32.Parse((string)mnuitem.Tag);
            switch (mVisibilePages) {
                case 1:
                    printPreviewControl1.Rows = 1;
                    printPreviewControl1.Columns = 1;
                    break;
                case 2:
                    printPreviewControl1.Rows = 1;
                    printPreviewControl1.Columns = 2;
                    break;
                case 4:
                    printPreviewControl1.Rows = 2;
                    printPreviewControl1.Columns = 2;
                    break;
                case 6:
                    printPreviewControl1.Rows = 2;
                    printPreviewControl1.Columns = 3;
                    break;
                case 8:
                    printPreviewControl1.Rows = 2;
                    printPreviewControl1.Columns = 4;
                    break;
            }
            tsBtnZoom_Click(null, null);
        }

        // Manages the next and previous button 
        private void Navigate_Click(object sender, EventArgs e) {
            ToolStripButton btn = (ToolStripButton)sender;
            int startPage = printPreviewControl1.StartPage;
            try {
                if (btn.Name == "tsBtnNext") {
                    startPage += mVisibilePages;
                }
                else {
                    startPage -= mVisibilePages;
                }
                if (startPage < 0) startPage = 0;
                if (startPage > mTotalPages - 1) startPage = mTotalPages - mVisibilePages;
                printPreviewControl1.StartPage = startPage;

            } catch { }
        }

        void printPreviewControl1_StartPageChanged(object sender, EventArgs e) {
            int tmp = printPreviewControl1.StartPage + 1;
            tsTxtCurrentPage.Text = tmp.ToString();
        }

        private void tsComboZoom_Leave(object sender, EventArgs e) {
            if (tsComboZoom.SelectedIndex == 0) {
                printPreviewControl1.AutoZoom = true;
                return;
            }
            string sZoomVal = tsComboZoom.Text.Replace("%", "");
            double zoomval;
            if (double.TryParse(sZoomVal, out zoomval)) {
                try {
                    printPreviewControl1.Zoom = zoomval / 100;
                } catch { }
                zoomval = (printPreviewControl1.Zoom * 100);
                tsComboZoom.Text = zoomval.ToString() + "%";
            }
        }

        private void tsBtnZoom_Click(object sender, EventArgs e) {
            tsComboZoom.SelectedIndex = 0;
            tsComboZoom_Leave(null, null);
        }

        private void tsComboZoom_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13) {
                tsComboZoom_Leave(null, null);
            }
        }

        private void tsBtnPrint_Click(object sender, EventArgs e) {
            PrintDialog.Document = this.mDocument;
            if (mShowPrinterSettingsBeforePrint) {
                if (PrintDialog.ShowDialog() == DialogResult.OK) {
                    try {
                        mDocument.Print();
                    } catch { }
                }
            }
            else {
                try {
                    mDocument.Print();
                } catch { }
            }
        }

        private void tsBtnPageSettings_Click(object sender, EventArgs e) {
            PageSetupDialog.Document = this.mDocument;
            if (PageSetupDialog.ShowDialog() == DialogResult.OK) {
                printPreviewControl1.InvalidatePreview();
            }
        }

        private void tsBtnPrinterSettings_Click(object sender, EventArgs e) {
            PrintDialog.Document = this.mDocument;
            PrintDialog.ShowDialog();
        }

        #endregion


        private void RadPrintPreview_FormClosing(object sender, FormClosingEventArgs e) {
            SwitchPrintDocumentHandlers(mDocument, false);
        }

        private void SwitchPrintDocumentHandlers(PrintDocument Document, bool Attach) {
            if (Document == null) return;
            if (Attach) {
                mDocument.BeginPrint += new PrintEventHandler(mDocument_BeginPrint);
                mDocument.PrintPage += new PrintPageEventHandler(mDocument_PrintPage);
                mDocument.EndPrint += new PrintEventHandler(mDocument_EndPrint);
            }
            else {
                mDocument.BeginPrint -= new PrintEventHandler(mDocument_BeginPrint);
                mDocument.PrintPage -= new PrintPageEventHandler(mDocument_PrintPage);
                mDocument.EndPrint -= new PrintEventHandler(mDocument_EndPrint);
            }
        }

        private void landcapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Document.DefaultPageSettings.Landscape == true) return;

            this.Document.DefaultPageSettings.Landscape = true;
            tsBtnLandScapePortrait.Text = tsBtnLandScapePortrait.DropDownItems[0].Text;
            tsBtnLandScapePortrait.DropDownItems[0].Enabled = false;  //disable LS
            tsBtnLandScapePortrait.DropDownItems[1].Enabled = true;   //enable POR
            this.printPreviewControl1.InvalidatePreview();
        }

        private void portraitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Document.DefaultPageSettings.Landscape == false) return;

            this.Document.DefaultPageSettings.Landscape = false;
            tsBtnLandScapePortrait.Text = tsBtnLandScapePortrait.DropDownItems[1].Text;
            tsBtnLandScapePortrait.DropDownItems[1].Enabled = false;  //disable POR
            tsBtnLandScapePortrait.DropDownItems[0].Enabled = true;   //Enable LS
            this.printPreviewControl1.InvalidatePreview();
        }


    }


}