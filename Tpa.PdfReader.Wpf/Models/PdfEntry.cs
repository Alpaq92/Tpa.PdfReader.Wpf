using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.IO;

namespace Tpa.PdfReader.Wpf.Models
{
    public class PdfEntry : ObservableObject
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }

        private Stream documentStream;
        public Stream DocumentStream
        {
            get
            {
                if (documentStream == null)
                {
                    documentStream = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
                }

                return documentStream;
            }
        }

        private bool doesFit;
        public bool DoesFit
        {
            get
            {
                return doesFit;
            }

            set
            {
                doesFit = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(DoesFit)));
            }
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
