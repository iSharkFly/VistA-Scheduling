using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.Globalization;
using VR.PrintPreview;

namespace PrintPreviewDemo {
    public partial class Form1 : Form {

        SampleDocument sample;
        AdditionalText additionalText;
        List<AdditionalText> globalAdditionalTextList;

        public Form1() {
            InitializeComponent();
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            sample = new SampleDocument();
            additionalText = new AdditionalText("Page $pagenumber");
            propertyGrid1.SelectedObject = additionalText;

            //Additional text can be stored in a global list and then 
            //assigned through the AdditionalTextList property of the EnhancedPrintPreviewDialog
            globalAdditionalTextList = new List<AdditionalText>();
            globalAdditionalTextList.Add(new AdditionalText("Draft", new Font("Arial", 120), Brushes.Red, TextPosition.WaterMark, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Top Right", new Font("Courier New", 10), Brushes.Blue, TextPosition.VTopRight,0,0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Top Left", new Font("Comic sans MS", 16), Brushes.Red, TextPosition.VTopLeft, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Bottom Left", new Font("Microsoft Sans Serif", 16), Brushes.Green, TextPosition.VBottomLeft, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Bottom Right", new Font("Thaoma", 14), Brushes.Brown, TextPosition.VBottomRight, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Middle Right", new Font("Times New Roma", 18), Brushes.Violet, TextPosition.VMiddleRight, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Vertical Middle Left", new Font("Comic sans MS", 16), Brushes.YellowGreen, TextPosition.VMiddleLeft, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Horizontal Top Right", new Font("Courier New", 10), Brushes.Blue, TextPosition.HTopRight, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Horizontal Top Left", new Font("Comic sans MS", 10), Brushes.Red, TextPosition.HTopLeft, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Horizontal Top Center", new Font("Times New Roma", 10), Brushes.Violet, TextPosition.HTopCenter, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Horizontal Bottom Right", new Font("Courier New", 10), Brushes.Blue, TextPosition.HBottomRight, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Horizontal Bottom Left", new Font("Comic sans MS", 10), Brushes.Red, TextPosition.HBottomLeft, 0, 0));
            globalAdditionalTextList.Add(new AdditionalText("Page $pagenumber")); //uses default font and postion (bottom center)
        }

        private void btnClassic_Click(object sender, EventArgs e) {
            MessageBox.Show("If you press the \"print button\" 10 pages will be sent soon to your default printer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            PrintPreviewDialog ClassicPreview = new PrintPreviewDialog();
            ClassicPreview.Document = sample.PrintDocument;
            ClassicPreview.ShowDialog();
        }

        private void btnNew_Click(object sender, EventArgs e) {
            EnhancedPrintPreviewDialog NewPreview = new EnhancedPrintPreviewDialog();
            NewPreview.Document = sample.PrintDocument;
            NewPreview.ShowDialog();
        }

        private void btnTest_Click(object sender, EventArgs e) {
            EnhancedPrintPreviewDialog NewPreview = new EnhancedPrintPreviewDialog();
            NewPreview.Document = sample.PrintDocument;
            NewPreview.ShowPrinterSettingsButton = chkShowPrinterSettings.Checked;
            NewPreview.ShowPageSettingsButton = chkShowPageSettings.Checked;
            NewPreview.ShowPrinterSettingsBeforePrint = chkPrinterSettingBeforePrint.Checked;
            if (!NewPreview.ShowPrinterSettingsBeforePrint)
                MessageBox.Show("If you press the \"print button\" 10 pages will be sent soon to your default printer", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            NewPreview.AdditionalTextList.Add(additionalText);
            NewPreview.ShowDialog();
        }


        private void btnNewAdditionalText_Click(object sender, EventArgs e) {
            EnhancedPrintPreviewDialog NewPreview = new EnhancedPrintPreviewDialog();
            NewPreview.Document = sample.PrintDocument;
            NewPreview.AdditionalTextList = globalAdditionalTextList;
            NewPreview.ShowDialog();
        }
    }
}