using GdaTools;
using MetaFilesystem;
using MetaFilesystem.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        private static readonly String FILENAME_ENTRIES = "durchsucher_files.xml";
        private static readonly String DIRECTORYNAME_TEMP = "durchsucher";

        private FilterModel _filterModel = new FilterModel();

        public MainWindow()
        {
            // todo: build UI for this
            var dataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None), FILENAME_ENTRIES);
            var srcDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.None);

            InitializeComponent();
            this.DataContext = _filterModel;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedFile = (sender as DataGrid).SelectedItem as FileEntry;
            if (selectedFile != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Open(selectedFile);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    OpenInExplorer(selectedFile);
                }
            }
        }

        private static void OpenInExplorer(FileEntry selectedFile)
        {
            var src = Path.Combine(selectedFile.Location, selectedFile.Name);
            FileTools.OpenInExplorer(src);
        }

        private void Open(FileEntry selectedFile)
        {
            var src = Path.Combine(selectedFile.Location, selectedFile.Name);
            var dst = Path.Combine(GetTempDataFolder(), selectedFile.Name);
            InitTempDataFolder();
            File.Copy(src, dst, overwrite: true);
            FileTools.OpenInDefaultApplication(dst);
        }

        private void InitTempDataFolder()
        {

            var tempDataFolder = GetTempDataFolder();
            Directory.CreateDirectory(tempDataFolder);
        }

        private String GetTempDataFolder()
        {
            String retValue = Path.Combine(Path.GetTempPath(), DIRECTORYNAME_TEMP);
            return retValue;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DeleteTempDataFolder();
        }

        private void DeleteTempDataFolder()
        {
            if (Directory.Exists(GetTempDataFolder()))
            {
                Directory.Delete(GetTempDataFolder(), recursive:true);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String fileName = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    fileName = openFileDialog.FileName;
                }
                if (null != fileName)
                {
                    _filterModel.LoadFromFile(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String fileName = null;
                var openFileDialog = new SaveFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    fileName = openFileDialog.FileName;
                }
                if (null != fileName)
                {
                    _filterModel.SaveToFile(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String fileName = null;
                var openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog.SelectedPath;
                }
                if (null != fileName)
                {
                    _filterModel.CollectFromDirectory(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void HashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               _filterModel.CalculateHashes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _filterModel.Cancel();
        }
    }
}
