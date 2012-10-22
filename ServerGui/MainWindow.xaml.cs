using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ServerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Server.Server MyServer { get; private set; }
        public bool IsRunning = false;

        public static readonly DependencyProperty ServiceRateProperty =
            DependencyProperty.Register("ServiceRate", typeof(String), typeof(MainWindow),
                                        new PropertyMetadata(default(String)));

        public String ServiceRate
        {
            get { return (String)GetValue(ServiceRateProperty); }
            set { SetValue(ServiceRateProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            var loc = Assembly.GetExecutingAssembly().Location;
            for (int i = 0; i < 4; i++)
                loc = loc.Substring(0, loc.LastIndexOf('\\'));
            RootDirectory.Text = loc + "\\HTML";
        }

        private void SelectFolderClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog(this.GetIWin32Window());
            if (result == System.Windows.Forms.DialogResult.OK)
                RootDirectory.Text = dialog.SelectedPath;
        }

        private void StartServerClick(object sender, RoutedEventArgs e)
        {
            int port;
            var success = Int32.TryParse(Port.Text, out port);
            if (success)
            {
                IsRunning = true;
                ControlButtons();
                MyServer = new Server.Server(RootDirectory.Text, port);
                new Thread(() =>
                               {
                                   while (IsRunning)
                                   {
                                       Dispatcher.Invoke(() => ServiceRateLabel.Content =
                                           MyServer.GetServiceRate());
                                       Thread.Sleep(500);
                                   }
                               }).Start();
            }
        }

        private void StopServerClick(object sender, RoutedEventArgs e)
        {
            IsRunning = false;
            MyServer.Stop();
            ControlButtons();
        }

        private void ControlButtons()
        {
            StartButton.IsEnabled = StopButton.IsEnabled;
            StopButton.IsEnabled = !StartButton.IsEnabled;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (IsRunning)
                StopServerClick(null, null);
        }
    }
}
