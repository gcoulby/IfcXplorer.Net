using System.IO;
using System.Windows;

namespace IfcXplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var args = e.Args;
            var mainWindow = new MainWindow();
            mainWindow.Show();
            foreach (var arg in e.Args)
            {
                mainWindow.Title += " " + arg + " ";
                if (File.Exists(arg))
                {
                    mainWindow.LoadAnyFile(arg);
                }
            }

        }
    }
}
