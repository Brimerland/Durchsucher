using GdaTools;
using MetaFilesystem;
using MetaFilesystem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durchsucher
{
    public class FilterModel : NotifyPropertyChangeBase
    {
        public List<FileEntry> FilteredEntries { get; set; }
        private FileEntryRepository _fileEntryRepository = new FileEntryRepository();

        private String _filterText;

        public String FilterText
        {
            get { return _filterText ?? ""; }
            set
            {
                _filterText = value;
                Filter();
                NotifyPropertyChanged();
            }
        }

        private bool _isInverted;

        public bool IsInverted
        {
            get { return _isInverted; }
            set
            {
                if(value != _isInverted)
                {
                    _isInverted = value;
                    Filter();
                    NotifyPropertyChanged();
                }
            }
        }



        private void Filter()
        {
            Filter((s) => IsInverted != s.Name.ToLower().Contains(FilterText) || s.Location.ToLower().Contains(FilterText));
        }

        private void Filter(Predicate<FileEntry> pred)
        {
            FilteredEntries = _fileEntryRepository.GetByFilter(pred);
            NotifyPropertyChanged(nameof(FilteredEntries));
        }

        internal void LoadFromFile(string dataFile)
        {
            _fileEntryRepository.LoadFromFile(dataFile);
            Filter();
        }

        internal void SaveToFile(string dataFile)
        {
            _fileEntryRepository.SaveToFile(dataFile);
        }

        internal void CollectFromDirectory(string srcDir)
        {
            _fileEntryRepository.LoadFromDirectory(srcDir);
            Filter();
        }
    }
}
