using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoreLinq;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Tpa.PdfReader.Wpf.Models;

namespace Tpa.PdfReader.Wpf.ViewModels
{
    public partial class PdfReaderViewModel : ObservableObject
    {
        public PdfReaderViewModel()
        {
            PropertyChanged += PropertyChangedCallback;
            Csv = new CsvEntry();
        }

        public ObservableCollection<PdfEntry> PdfDocuments { get; set; } = new ObservableCollection<PdfEntry>();

        private CsvEntry csv;
        public CsvEntry Csv
        {
            get
            {
                return csv;
            }
            set
            {
                if (csv != null)
                {
                    csv.PropertyChanged -= PropertyChangedCallback;
                }                

                csv = value;

                if (csv != null)
                {
                    csv.PropertyChanged += PropertyChangedCallback;
                }

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Csv)));
            }
        }

        private PdfEntry selectedPdf;
        public PdfEntry SelectedPdf
        {
            get
            {
                return selectedPdf;
            }
            set
            {
                selectedPdf = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedPdf)));
            }
        }

        private ICommand loadCsv;
        public ICommand LoadCsv
        {
            get
            {
                if (loadCsv == null)
                {
                    loadCsv = new RelayCommand(() =>
                    {
                        lock (this)
                        {
                            var dialog = new Microsoft.Win32.OpenFileDialog
                            {
                                DefaultExt = ".csv",
                                Filter = "Comma separated values (.csv)|*.csv"
                            };

                            if (dialog.ShowDialog() ?? false)
                            {
                                if (File.Exists(dialog.FileName))
                                {
                                    Csv.Lines = File.ReadAllLines(dialog.FileName).Select(line => line.Trim() ?? string.Empty)
                                        .Where(trimmed => !string.IsNullOrWhiteSpace(trimmed)).ToList() ?? [];
                                    OnPropertyChanged(nameof(Csv));
                                }
                            }
                        }
                    });
                }

                return loadCsv;
            }
        }

        private ICommand openPdf;
        public ICommand OpenPdf
        {
            get
            {
                if (openPdf == null)
                {
                    openPdf = new RelayCommand(() =>
                    {
                        lock (this)
                        {
                            var dialog = new Microsoft.Win32.OpenFileDialog
                            {
                                DefaultExt = ".pdf",
                                Filter = "Portable Document Format (.pdf)|*.pdf"
                            };

                            if (dialog.ShowDialog() ?? false)
                            {
                                if (File.Exists(dialog.FileName))
                                {
                                    if (!PdfDocuments.Any(dodcument => dodcument.FullPath.Equals(dialog.FileName, StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        PdfDocuments.Add(new PdfEntry
                                        {
                                            FileName = Path.GetFileName(dialog.FileName),
                                            FullPath = dialog.FileName
                                        });
                                        OnPropertyChanged(nameof(PdfDocuments));
                                    }
                                }
                            }
                        }
                    });
                }

                return openPdf;
            }
        }

        private ICommand searchPdf;
        public ICommand SearchPdf
        {
            get
            {
                if (searchPdf == null)
                {
                    searchPdf = new RelayCommand(() =>
                    {
                        lock (this)
                        {
                            if (!Csv.Lines.Any())
                            {
                                return;
                            }

                            foreach (var document in PdfDocuments)
                            {
                                var pdf = new PdfDocumentView();
                                pdf.Load(document.FullPath);

                                var extracted = string.Empty;
                                for (var i = 0; i < pdf.PageCount; i++)
                                {
                                    var lines = new TextLines();
                                    extracted += pdf.ExtractText(i, out lines);
                                }

                                document.DoesFit = Csv.Lines.All(line => extracted.Contains(line, StringComparison.InvariantCultureIgnoreCase));
                            }

                            OnPropertyChanged(new PropertyChangedEventArgs(nameof(PdfDocuments)));
                        }
                    });
                }

                return searchPdf;
            }
        }

        private void PropertyChangedCallback(object? sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName?.Equals(nameof(Csv)) ?? false) || sender is CsvEntry)
            {
                PdfDocuments?.ForEach(document => document.DoesFit = false);
            }
        }
    }
}