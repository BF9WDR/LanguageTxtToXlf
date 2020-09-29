using LanguageTxtToXlf.Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanguageTxtToXlf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string sourceFilePath { get; set; }

        private TextBlockLogger Logger;
        public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>
        {
           {"de", "A1031"},
           {"en", "A1033"},
           {"es", "A1034"},
           {"fr", "A1036"},
           {"hu", "A1038"},
           {"it", "A1040"},
           {"pt", "A1046"},
           {"gsw","A2055"},
           {"en-gb", "A2057"},
           {"de-2", "A3079"}
        };
        private string resultFilePath = null;
        public MainWindow()
        {         
            InitializeComponent();
            TargetLanguageCombBox.ItemsSource = Languages.Keys;
            Logger = new TextBlockLogger(this.LogTextBlock);
            Logger.Log("Helloszia");
        }

        private void SelectSourceFIleButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //Get the path of specified file
                sourceFilePath = openFileDialog.FileName;
                SourceFileLabel.Content = sourceFilePath;
            } else
            {
                SourceFileLabel.Content = "";
            }
            
            

        }

        private async void RunButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (TargetLanguageCombBox.SelectedItem == null || !File.Exists(sourceFilePath))
            {
                MessageBox.Show("Kérlek, válassz egy létező bemeneti fájlt és nyelvet!");
                return;
            }
            string languageCode;
            if (Languages.TryGetValue((string)TargetLanguageCombBox.SelectedItem, out languageCode));
            var logic = new Logic.TxtToXlfLogic(sourceFilePath, languageCode, Logger);
            Task.Run(() =>logic.Process());           
        }

        private void TargetLanguageCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
