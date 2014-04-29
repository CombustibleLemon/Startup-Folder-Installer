using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Xml;



namespace Disc_Drive_Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String FILE_TYPE_1 = "Tool";
        private const String FILE_TYPE_2 = "Prank";
        XmlDocument doc = new XmlDocument();
        private double ProgressPercentage
        {
            get 
            {
                return ProgressBar.Value;
            }
            set
            {
                TaskbarItemInfo.ProgressValue = value/100;
                ProgressBar.Value = value; 
            }
        }
        public int State
        {
            set
            {
                if (value == 0)
                {
                    // Error
                    TextBlock.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    TextBlock.FontSize = 12;
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                }
                else if (value == 1)
                {
                    // Process running
                    TextBlock.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    TextBlock.FontSize = 14;
                }
                else if (value == 2)
                {
                    // Installation choices
                    StartButton.Visibility = System.Windows.Visibility.Visible;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    TextBlock.Visibility = System.Windows.Visibility.Hidden;
                    ProgressBar.Visibility = System.Windows.Visibility.Hidden;

                    GroupBox1.Header = FILE_TYPE_1;
                    GroupBox2.Header = FILE_TYPE_2;
                    
                    //doc.Load()
                }
                else if (value == 3)
                {
                    // Done
                    TextBlock.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    TextBlock.Text = "Done";
                    TextBlock.TextAlignment = TextAlignment.Center;
                    TextBlock.FontSize = 82;
                    ProgressPercentage = 100;
                }
            }
        }
        DispatcherTimer Timer = new DispatcherTimer();

        public MainWindow()
        {
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            InitializeComponent();
            State = 2;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            State = 1;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
#if DEBUG
            ProgressPercentage = 85;
#endif
            if (ProgressPercentage >= 99)
            {
                Timer.Stop();
                State = 3;
                List<string> files = new List<string>();
                files.Add("Disc_drive.vbs");
                try
                {
                    ExtractEmbeddedResource("Startup_Folder_Installer.ExampleFiles", Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Start Menu\Programs\Startup"), files);
                }
                catch (Exception ex)
                {
                    State = 0;
                    TextBlock.Text = ex.ToString();
                }
                return;
            }
            ProgressPercentage += Timer.Interval.TotalSeconds * 10000;
        }

        public static void ExtractEmbeddedResource(string resourceLocation, string outputDir, List<string> files)
        {
            foreach (string file in files)
            {
                using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
            }
        }
    }
}