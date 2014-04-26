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



namespace Disc_Drive_Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double ProgressPercentage;
        //private int Line;
        DispatcherTimer Timer = new DispatcherTimer();
        private Uri TextFileURI = new Uri("ms-appx:///Strings.txt");
        //System.Windows.Stor

        public MainWindow()
        {
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            InitializeComponent();
            SetState(2);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SetState(1);
            Timer.Start();
        }

        /// <summary>
        /// Sets the state of the window.
        /// </summary>
        /// <param name="state">Integer 1 or 2. 1 == TextBlock and ProgressBar, 2 == StartButton</param>
        private void SetState(int state)
        {
            if (state == 1)
            {
                // Process running
                StartButton.Visibility = System.Windows.Visibility.Hidden;
                TextBlock.Visibility = System.Windows.Visibility.Visible;
                ProgressBar.Visibility = System.Windows.Visibility.Visible;
                TextBlock.FontSize = 14;
            }
            else if (state == 2)
            {
                // Just start button
                StartButton.Visibility = System.Windows.Visibility.Visible;
                TextBlock.Visibility = System.Windows.Visibility.Hidden;
                ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                //TaskbarItemInfo.
            }
            else if (state == 3)
            {
                TextBlock.Text = "Done";
                TextBlock.TextAlignment = TextAlignment.Center;
                TextBlock.FontSize = 82;
                SetProgressPercentage(100);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
#if DEBUG
            SetProgressPercentage(99);
#endif
            if (ProgressPercentage >= 99)
            {
                Timer.Stop();
                SetState(3);
                List<string> files = new List<string>();
                files.Add("Disc_drive.vbs");
                try
                {
                    ExtractEmbeddedResource("Startup_Folder_Installer.ExampleFiles", Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Start Menu\Programs\Startup"), files);
                }
                catch (Exception ex)
                {
                    TextBlock.FontSize = 14;
                    TextBlock.Text = ex.ToString();
                }
                return;
            }
            /*
            if ((int)ProgressPercentage > line)
            {
                line = (int)ProgressPercentage;
                TextBlock.Text += PullFromFile(TextFile, line) + "\n";
            }
            */
            SetProgressPercentage(ProgressPercentage + Timer.Interval.TotalSeconds * 100);
        }

        /// <summary>
        /// Updates the progress bars and <code>double ProgressPercentage</code>
        /// </summary>
        /// <param name="percentageToSet">The value to use</param>
        private void SetProgressPercentage(double percentageToSet)
        {
            ProgressPercentage = percentageToSet;
            TaskbarItemInfo.ProgressValue = ProgressPercentage/100;
            ProgressBar.Value = ProgressPercentage;
        }

        private string PullFromFile(string filePath, int line)
        {
            StreamReader fileReader = new StreamReader(filePath);
            
            for (int i = 0; i < line; i++)
            {
                fileReader.ReadLine();
            }

            return fileReader.ReadLine();
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
