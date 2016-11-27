using GdaTools;
using MetaFilesystem;
using MetaFilesystem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Durchsucher
{
    public class FilterModel : NotifyPropertyChangeBase
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public List<FileEntry> FilteredEntries { get; set; }
        private FileEntryRepository _fileEntryRepository = new FileEntryRepository();

        private String _currentState = "Ready";

        public String CurrentState
        {
            get { return _currentState; }
            set
            {
                if(value != _currentState)
                {
                    _currentState = value;
                    NotifyPropertyChanged();
                }
            }
        }


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

        internal async void CollectFromDirectory(string srcDir)
        {
            try
            {
                IProgress<String> progress = new Progress<String>((x) => { CurrentState = x; });
                await _fileEntryRepository.LoadFromDirectory(srcDir, progress, _cancellationTokenSource.Token);
                Filter();
            }
            finally
            {
                CurrentState = "Finished scanning";
            }

        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        internal async void CalculateHashes()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                await Task.Run(() =>
                {
                    foreach (var entry in _fileEntryRepository.GetAll())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            CurrentState = entry.Name;
                            if (String.IsNullOrWhiteSpace(entry.Hash))
                            {
                                _fileEntryRepository.CalculateHash(entry);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }, cancellationToken);
            }
            catch (Exception) { }
            finally
            {
                CurrentState = "Finished scanning";
            }
        }
    }
}
