using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConfigurationBitSetting;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;

namespace ConfigurationBitParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string htmlDirectoryPath = @"C:\Program Files (x86)\Microchip\xc8\v1.12\docs\chips";
        string htmlFilePath = null;
        List<string> htmlFiles = new List<string>();
        ChipInfoReader reader;
        List<ConfigBits> bits;
        ObservableCollection<string> listOfChips = new ObservableCollection<string>();
        ObservableCollection<string> listOfConfigBits = new ObservableCollection<string>();
        ObservableCollection<string> listOfConfigValues = new ObservableCollection<string>();
        FolderBrowserDialog folderBrowse;

        BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            folderBrowse = new FolderBrowserDialog();
            this.selectChip.ItemsSource = listOfChips;
            this.configBits.ItemsSource = listOfConfigBits;
            this.configValues.ItemsSource = listOfConfigValues;
            bw.DoWork +=new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            try
            {
                getChips(htmlDirectoryPath);
            }
            catch (Exception e)
            {
                bool correctPath = false;
                while (!correctPath)
                {
                    System.Windows.MessageBox.Show("Unable to find default doc path" +e.Message.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (folderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (folderBrowse.SelectedPath.Contains("chips"))
                        {
                            htmlDirectoryPath = folderBrowse.SelectedPath;
                            getChips(htmlDirectoryPath);
                            correctPath = true;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("You have selected the incorrect path, must end with docs", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (folderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.Cancel || folderBrowse.ShowDialog() == System.Windows.Forms.DialogResult.Abort)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }
        }

        private void bw_DoWork(object sender, EventArgs e)
        {

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        public void getChips(string htmlDirectoryPath)
        {
            foreach (string file in Directory.GetFiles(htmlDirectoryPath))
            {
                htmlFiles.Add(file);
                listOfChips.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }

        }

        private void selectChip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            outputText.Clear();
            listOfConfigBits.Clear();
            listOfConfigValues.Clear();
            htmlFilePath = selectChip.SelectedItem.ToString();
            bits = new List<ConfigBits>();
            reader = new ChipInfoReader(htmlDirectoryPath, bits);

            this.listOfConfigBits.Clear();
            htmlFilePath = htmlFiles[selectChip.SelectedIndex];
            reader.Read(htmlFilePath);
            foreach (ConfigBits bit in bits)
            {
                this.listOfConfigBits.Add(bit.getConfigBitName() + " " + bit.getConfigBitDescription());
            }
        }

        private void configBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listOfConfigValues.Clear();
            int bitSelected = configBits.SelectedIndex;
            for (int i = 0; i < bits[bitSelected].getConfigSettingCount(); i++)
            {
                listOfConfigValues.Add(bits[bitSelected].getConfigSetting(i) + " " + bits[bitSelected].getConfigDescription(i));
            }
        }

        private void setBit_Click(object sender, RoutedEventArgs e)
        {
            if (configValues.SelectedItem != null)
            {
                string pragmaStatement = "#pragma config " + bits[configBits.SelectedIndex].getConfigBitName() + bits[configBits.SelectedIndex].getConfigSetting(configValues.SelectedIndex) + ", ";
                outputText.AppendText(pragmaStatement);
            }
        }

        private void configValues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
        }

        private void clearOutput_Click(object sender, RoutedEventArgs e)
        {
            outputText.Clear();
        }

        /// <summary>
        /// An event handler that opens the selected chip document in the default browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openDoc_Click(object sender, RoutedEventArgs e)
        {
            if (selectChip.SelectedItem != null)
            {
                Process.Start(htmlFilePath); 
            }
        }
    }
}
