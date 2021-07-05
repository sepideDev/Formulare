using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;
//using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using Android.Graphics.Pdf;
//using Syncfusion.Pdf.Parsing;
//using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using System.Reflection;

namespace Formulare
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Anamnese : ContentPage
    {
        public Anamnese()
        {

            InitializeComponent();
        }



        private async void button_Clicked(object sender, EventArgs e)
        {

            MemoryStream stream = new MemoryStream();
            //Create a new PDF document
            using (PdfDocument document = new PdfDocument())
            {
                //Add page to the PDF document.
                PdfPage page = document.Pages.Add();

                //Create graphics instance.
                PdfGraphics graphics = page.Graphics;

                Stream imageStream = null;

                if (Device.RuntimePlatform == Device.Android)
                {
                    byte[] data;
                    data = await Xamarin.Forms.DependencyService.Get<ISaveWindowsPhone>().CaptureAsync();

                    imageStream = new MemoryStream(data);

                }
                else
                {
                    imageStream = new MemoryStream(DependencyService.Get<ISave>().CaptureAsync().Result);
                }

                //Load the image
                PdfBitmap image = new PdfBitmap(imageStream);

                //Draw the image
                graphics.DrawImage(image, 0, 0, page.GetClientSize().Width, page.GetClientSize().Height);

                //Save the document into memory stream
                document.Save(stream);

            }
            stream.Position = 0;
            if (Device.RuntimePlatform == Device.Android)
            {
                Xamarin.Forms.DependencyService.Get<ISaveWindowsPhone>().Save("XAMLtoPDF.pdf", "application/pdf", stream);
            }
            else
            {
                Xamarin.Forms.DependencyService.Get<ISave>().Save("XAMLtoPDF.pdf", "application/pdf", stream);
            }

        }

      
    }
}