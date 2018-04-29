using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using IfcXplorer.CustomControls;
using IfcXplorer.Properties;
using IfcXplorer.ViewModels;
using Xbim.Ifc;

using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ZipFile = System.IO.Compression.ZipFile;

namespace IfcXplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string DefaultTitle = "IFC Xplorer";
        public static RoutedCommand FontSizeIncrease = new RoutedCommand("Increase Font Size", typeof(MainWindow));
        public static RoutedCommand FontSizeDecrease = new RoutedCommand("Decrease Font Size", typeof(MainWindow));
        public static RoutedCommand FontSizeReset = new RoutedCommand("Reset Font Size", typeof(MainWindow));
        public static RoutedCommand ToggleTheme = new RoutedCommand("Toggle Theme", typeof(MainWindow));
        public static RoutedCommand TextSearch = new RoutedCommand("Text Search", typeof(MainWindow));
        private readonly TextSearcher _textSearcher;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();
            _textSearcher = new TextSearcher(IfcReader);
            FontSizeIncrease.InputGestures.Add(new KeyGesture(Key.OemPlus, ModifierKeys.Control));
            FontSizeDecrease.InputGestures.Add(new KeyGesture(Key.OemMinus, ModifierKeys.Control));
            FontSizeReset.InputGestures.Add(new KeyGesture(Key.D0, ModifierKeys.Control));
            TextSearch.InputGestures.Add(new KeyGesture(Key.F3));
            Cursor = Cursors.AppStarting;
        }

        private void InitializeSettings()
        {
            ThemeSwitchButton.IsChecked = Settings.Default.NightMode;

            //Flip the theme to combat issue where cursors get overriden
            SetTheme(!Settings.Default.NightMode);
            SetTheme(Settings.Default.NightMode);

            IfcReader.FontSize = Settings.Default.FontSize;
            FontSizeLabel.Content = Settings.Default.FontSize.ToString();
        }

        #region Open File
        /// <summary>
        /// Command binding for Open button and menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Ifc Files|*.ifc;*.ifczip|COBie Files|*.COBie;*.COBieZip"
            };
            // Filter files by extension 
            dlg.FileOk += Dlg_OpenAnyFile;
            dlg.ShowDialog(this);
        }

        /// <summary>
        /// Listener for the OpenFileDialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dlg_OpenAnyFile(object sender, CancelEventArgs e)
        {
            if (sender is OpenFileDialog dlg) LoadAnyFile(dlg.FileName);
        }

        /// <summary>
        /// Loads files from a filestream or zipstream and
        /// outputs the text to the textbox
        /// </summary>
        /// <param name="file"></param>
        internal void LoadAnyFile(string file)
        {
            string ifc;
            if (file.ToLower().EndsWith("ifczip") || file.ToLower().EndsWith("cobiezip"))
            {
                //Load zip file
                using (var zip = ZipFile.OpenRead(file))
                {
                    using (var sr = new StreamReader(zip.Entries[0].Open()))
                    {
                        ifc = sr.ReadToEnd();
                        sr.Close();
                    }
                    CompressedSize.Text = (zip.Entries[0].CompressedLength / 1000) + "Kb";
                    UncompressedSize.Text = (zip.Entries[0].Length / 1000) + "Kb";
                    zip.Dispose();
                }
            }
            else
            {
                //Load uncompressed files.
                ifc = File.ReadAllText(file);
                IfcReader.Document = null;
                CompressedSize.Text = "n/a";
                UncompressedSize.Text = "n/a";
            }
 
            Title = DefaultTitle + " - " + Path.GetFileName(file);
            IfcReader.Document = new TextDocument { UndoStack = { SizeLimit = 0 } };

            //Output the loaded text to the window
            IfcReader.Document = new TextDocument(ifc);
            IfcReader.Padding = new Thickness(10, 0, 0, 0);

            NumberOfLines.Text = IfcReader.GetNumberOfLines().ToString();
            NumberOfInstances.Text = IfcReader.GetNumberOfInstances().ToString();

            using (var store = IfcStore.Open(file))
            {
                var containment = TreeViewBuilder.ContainmentView(store.ReferencingModel);
                SpatialTree.ItemsSource = containment;

            }
        }

        #endregion

        #region Theme
        private void ThemeSwitch(object sender, RoutedEventArgs e)
        {
            var btn = (IconToggleButton)e.OriginalSource;
            Settings.Default.NightMode = btn.IsChecked == true;
            SetTheme(Settings.Default.NightMode);
        }

        private void SetTheme(bool isDarkMode)
        {
            using (new OverrideCursor(Cursors.Wait))
            {
                IfcReader.SetTheme(isDarkMode);

                var text = (SolidColorBrush)Application.Current.Resources[isDarkMode ? "LightText" : "DarkText"];
                var readerText =
                    (SolidColorBrush)Application.Current.Resources[isDarkMode ? "LightTextReader" : "DarkText"];
                var readerBackground =
                    (SolidColorBrush)Application.Current.Resources[isDarkMode ? "DarkBackground" : "LightBackground"];
                var panelsBackground =
                    (SolidColorBrush)Application.Current.Resources[
                        isDarkMode ? "DarkPanelsBackground" : "LightPanelsBackground"];
                var border = (SolidColorBrush)Application.Current.Resources[isDarkMode ? "BorderDark" : "BorderLight"];

                BrowserHeading.Foreground = text;
                BrowserUnderline.Fill = text;
                SpatialTree.Foreground = text;
                IfcReader.Foreground = readerText;
                StatusBar.Foreground = text;
                SearchText.Foreground = readerText;
                FontSizeLabel.Foreground = text;
                //Backgrounds
                Background = panelsBackground;
                RightPanel.Background = panelsBackground;
                StatusBar.Background = panelsBackground;
                SideBar.Background = panelsBackground;
                ToolBar.Background = panelsBackground;
                IfcReader.Background = readerBackground;
                SearchText.Background = readerBackground;
                SearchPanel.Background = panelsBackground;
                //Buttons
                OpenButton.Foreground = text;
                ThemeSwitchButton.Foreground = text;
                //Borders
                GridSplitter.BorderBrush = border;
                ToolbarBorder.Stroke = border;
                StatusBar.BorderBrush = border;
                SearchText.BorderBrush = border;
            }
        }
        #endregion

        #region Event Handlers
        private void SpatialTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IfcReader.SelectLine(((CustomXbimViewModel)SpatialTree.SelectedItem).EntityLabel.ToString());
        }
        private void Search(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text)) return;

            _textSearcher.SearchForward(SearchText.Text);
        }
        private void ChangeFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            switch (((RoutedCommand)e.Command).Name)
            {
                case "Increase Font Size":
                    IfcReader.FontSize += 1;
                    break;
                case "Decrease Font Size":
                    if ((int)IfcReader.FontSize != 1)
                        IfcReader.FontSize -= 1;
                    break;
                case "Reset Font Size":
                    IfcReader.FontSize = 14;
                    break;
            }
            Settings.Default.FontSize = (int)IfcReader.FontSize;
            FontSizeLabel.Content = Settings.Default.FontSize.ToString();
        }
        private void OnExit(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }

        #endregion
    }
}
