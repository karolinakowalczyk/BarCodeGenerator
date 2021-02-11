using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BarCodeGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("Barcode.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document pdfdoc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(pdfdoc, fs);
            pdfdoc.Open();
            PdfContentByte cb = writer.DirectContent;

            BarcodeEAN ean13 = new BarcodeEAN();
            ean13.ChecksumText = true;
            ean13.CodeType = BarcodeEAN.EAN13;

            if (textBox1.Text.Length == 12)
            {
                int checkSum = calculateChecksum(textBox1.Text);
                ean13.Code = textBox1.Text + checkSum.ToString();
                iTextSharp.text.Image barcodeImage = ean13.CreateImageWithBarcode(cb, null, null);
                pdfdoc.Add(barcodeImage);
                pdfdoc.Close();
                MessageBox.Show("PDF generated correct!");
            }
            else if (textBox1.Text.Length == 13)
            {
                if (CheckCode(textBox1.Text))
                {
                    ean13.Code = textBox1.Text;
                    iTextSharp.text.Image barcodeImage = ean13.CreateImageWithBarcode(cb, null, null);
                    pdfdoc.Add(barcodeImage);
                    pdfdoc.Close();
                    MessageBox.Show("PDF generated correct!");
                }
                else
                {
                    MessageBox.Show("Control digit is incorrect!");
                    fs.Close();
                }
            }
            else 
            {
                MessageBox.Show("Length of correct sum should be 12 or 13!");
                fs.Close();
            }
        }
        
        public static int calculateChecksum(string code)
        {
            if (code == null || code.Length != 12)
                throw new ArgumentException("Code length should be 12 (without a check digit)");

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int v;
                if (!int.TryParse(code[i].ToString(), out v))
                    throw new ArgumentException("Invalid character!");
                sum += (i % 2 == 0 ? v : v * 3);
            }
            int check = 10 - (sum % 10);
            if (check == 10)
            {
                check = 0;
            }
            return check;
        }

        private bool CheckCode(string code)
        {
            if (code == null || code.Length != 13)
                return false;

            int res;
            foreach (char c in code)
                if (!int.TryParse(c.ToString(), out res))
                    return false;

            char check = (char)('0' + calculateChecksum(code.Substring(0, 12)));
            return code[12] == check;
        }
    }

    //9876732174262
    //123456789012-8
    //9876732174263 (zle)
    //923456789012-0
    //777634895435-2

}
