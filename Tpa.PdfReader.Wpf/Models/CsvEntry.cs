using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tpa.PdfReader.Wpf.Models
{
    public class CsvEntry : ObservableObject
    {
        public IList<string> Lines { get; set; } = new List<string>();

        public string FullText
        {
            get
            {
                return string.Join(Environment.NewLine, Lines);
            }

            set
            {
                Lines = $"{value}".Split(Environment.NewLine)?.Select(line => line?.Trim() ?? string.Empty)
                    .Where(trimmed => !string.IsNullOrWhiteSpace(trimmed)).ToList() ?? [];

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Lines)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(FullText)));
            }
        }
    }
}
