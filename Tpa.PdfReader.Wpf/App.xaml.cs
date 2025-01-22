using System;
using Tpa.PdfReader.Wpf.ViewModels;
using Tpa.PdfReader.Wpf.Views;

namespace Tpa.PdfReader.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        /// <summary>
        /// Application Entry for Tpa.PdfReader.Wpf
        /// </summary>
        public App()
        {
            var view = new PdfReaderView
            {
                DataContext = Activator.CreateInstance<PdfReaderViewModel>()
            };

            view.Show();
        }

    }
}