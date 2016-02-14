using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Durchsucher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model myModel = new Model();

        public MainWindow()
        {
            // todo: build UI for this
            bool refresh = true;
            var dataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None), "durchsucher_files.xml");
            var srcDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.None);

            InitializeComponent();

            // load data
            {
                XmlSerializer s = new XmlSerializer(typeof(List<FileEntry>));

                if (refresh)
                {
                    myModel.AllEntries = CollectFileEntries(srcDir, null);

                    var sw = new StringWriter();
                    s.Serialize(sw, myModel.AllEntries);
                    File.WriteAllText(dataFile, sw.ToString());
                }
                else
                {
                    var sr = new StringReader(File.ReadAllText(dataFile));
                    myModel.AllEntries = (List<FileEntry>)s.Deserialize(sr);
                }
            }

            myModel.FilteredEntries = myModel.AllEntries;
            this.DataContext = myModel;
        }

        List<FileEntry> CollectFileEntries(string directory, List<FileEntry> fillMe)
        {
            var comparison = new Comparison<string>((a, b) =>
            {
                return a.ToLowerInvariant().CompareTo(b.ToLowerInvariant());
            });

            System.Diagnostics.Debug.WriteLine(directory);
            var result = fillMe != null ? fillMe : new List<FileEntry>();

            try
            {
                var filenames = new List<string>(System.IO.Directory.GetFiles(directory));
                filenames.Sort(comparison);
                foreach (var fileName in filenames)
                    result.Add(new FileEntry() { Name = System.IO.Path.GetFileName(fileName), Location = directory });

            }
            catch
            { }

            try
            {
                var dirnames = new List<string>(System.IO.Directory.GetDirectories(directory));
                dirnames.Sort(comparison);
                foreach (var dirName in dirnames)
                    CollectFileEntries(System.IO.Path.Combine(directory, dirName), result);
            }
            catch
            { }

            return result;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedSong = (sender as DataGrid).SelectedItem as FileEntry;
            if (selectedSong != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    var src = System.IO.Path.Combine(selectedSong.Location, selectedSong.Name);
                    var dst = System.IO.Path.Combine("v:/inbox/tempMP3", selectedSong.Name);
                    File.WriteAllBytes(dst, File.ReadAllBytes(src));
                    ShellExecute(0, "open", dst, "", "", 5);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    var src = System.IO.Path.Combine(selectedSong.Location, selectedSong.Name);
                    ShellExecute(0, "open", "explorer.exe", string.Format("/select,\"{0}\"", src), "", 5);
                }
            }
        }

        [DllImport("shell32.dll", EntryPoint = "ShellExecute")]
        public static extern long ShellExecute(int hwnd, string cmd, string file, string param1, string param2, int swmode);


        void Filter()
        {
            bool invert = cbInvert.IsChecked == true;
            var text = tbFilter.Text.ToLower();
            myModel.Filter((s) => invert != (s.Name.ToLower().Contains(text) || s.Location.ToLower().Contains(text)));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void cbInvert_Checked(object sender, RoutedEventArgs e)
        {
            Filter();
        }
    }

    public class FileEntry
    {
        public String Name { get; set; }
        public int Size { get; set; }
        public String Location { get; set; }
    }

    public class Model : INotifyPropertyChanged
    {
        public List<FileEntry> AllEntries = new List<FileEntry>();
        public List<FileEntry> FilteredEntries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Filter(Predicate<FileEntry> pred)
        {
            FilteredEntries = AllEntries.FindAll(pred);

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(FilteredEntries)));
        }
    }
}
