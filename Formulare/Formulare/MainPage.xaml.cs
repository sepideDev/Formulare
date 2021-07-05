using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;

namespace Formulare
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ShowForm_Btn(object sender, EventArgs e)
        {


            //Navigation.PushAsync(new FormularPage());
            //MainPage = new NavigationPage(new FormularPage());
            await Navigation.PushAsync(new NavigationPage(new Anamnese()));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}
