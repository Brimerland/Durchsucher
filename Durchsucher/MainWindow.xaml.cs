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
            bool refresh = true;
            var dataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None), FILENAME_ENTRIES);
            var srcDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.None);

            InitializeComponent();

            //// load data
            //if (refresh)
            //{
            //    _filterModel.CollectFromDirectory(srcDir);
            //    _filterModel.SaveToFile(dataFile);
            //}
            //else
            //{
            //    //_filterModel.LoadFromFile(dataFile);
            //}
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

        bool isHashing = false;
        private CancelationToken _cancelToken = new CancelationToken();
        private void HashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isHashing)
                {
                    _cancelToken = new CancelationToken();
                    Task.Run(() => { _filterModel.CalculateHashes(_cancelToken); });
                }
                else
                {
                    _cancelToken.Canceled = true;
                }
                isHashing = !isHashing;
                CancelButton.Content = isHashing ? "Cancel" : "CalculateHash";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

    }
}
