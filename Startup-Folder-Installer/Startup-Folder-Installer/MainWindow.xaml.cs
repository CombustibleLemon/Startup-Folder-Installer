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



namespace Startup_Folder_Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int State
        {
            set
            {
                if (value == 0)
                {
                    // Installation choices
                    StartButton.Visibility = System.Windows.Visibility.Visible;
                    GroupBox1.Visibility = System.Windows.Visibility.Visible;
                    GroupBox2.Visibility = System.Windows.Visibility.Visible;

                    ScrollViewer.Visibility = System.Windows.Visibility.Hidden;
                    ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                }
                else if (value == 2)
                {
                    // Error
                    ScrollViewer.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    ScrollViewer.FontSize = 12;
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                }
                else if (value == 1)
                {
                    // Process running
                    ScrollViewer.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    ScrollViewer.FontSize = 14;
                }
                else if (value == 3)
                {
                    // Done
                    ScrollViewer.Visibility = System.Windows.Visibility.Visible;
                    ProgressBar.Visibility = System.Windows.Visibility.Visible;

                    StartButton.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox1.Visibility = System.Windows.Visibility.Hidden;
                    GroupBox2.Visibility = System.Windows.Visibility.Hidden;

                    ScrollViewer.Content = "Done";
                    ScrollViewer.FontSize = 82;
                    ProgressPercentage = 100;
                }
            }
        }
        private double ProgressPercentage
        {
            get
            {
                return ProgressBar.Value;
            }
            set
            {
                TaskbarItemInfo.ProgressValue = value / 100;
                ProgressBar.Value = value;
            }
        }
        private const string CONTENTS_FILE = @"Startup_Folder_Installer.Assets.ExampleFiles.Contents.xml";
        private const string SPLASH_FILE = @"Startup_Folder_Installer.Assets.SplashStrings.txt";
        private List<Grid> grids = new List<Grid>();
        private Stream contentsFileStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(CONTENTS_FILE);
        private XmlDocument doc = new XmlDocument();
        private DispatcherTimer Timer = new DispatcherTimer();
        private List<string> splash;

        public MainWindow()
        {
            InitializeComponent();

            // Stupid coding, but whatever
            grids.Add(GroupBox1Grid);
            grids.Add(GroupBox2Grid);

            // Timers
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            // For the XML
            doc.Load(contentsFileStream);
            XMLtoCheckboxes();
            
            // Makes the groupboxes and install button visible
            State = 0;

            // Splash strings yippidy doo dah
            splash = Helpers.DanFile.TextFile(SPLASH_FILE);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            ProgressPercentage = 85;
#endif
            State = 1;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ProgressPercentage >= 99)
            {
                Timer.Stop();
                State = 3;
                List<string> files = new List<string>();

                foreach (Grid grid in grids)
                {
                    foreach (CheckBox box in grid.Children)
                    {
                        files.Add
                            (
                                Helpers.DanXML.FindNode
                                    (
                                        Helpers.DanXML.FindInnerText(doc.ChildNodes[1],
                                        (string)box.Content).ParentNode,
                                        new List<string>(){ "filename" }
                                    ).InnerText
                            );
                    }
                }

                try
                {
                    Helpers.DanFile.ExtractEmbeddedResource
                        (
                            "Startup_Folder_Installer.Assets.ExampleFiles",
                            Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Start Menu\Programs\Startup"),
                            files
                        );
                }
                catch (Exception ex)
                {
#if DEBUG
                    State = 2;
                    ScrollViewer.Content+= ex.ToString();
#endif
                }
                return;
            }

            try
            {
                ScrollViewer.Content += ("\n" + splash[(int)ProgressPercentage]);
            }
            catch (IndexOutOfRangeException)
            {
#if DEBUG
                    ScrollViewer.Content += ("\n" + SPLASH_FILE + " shorter than 100");
#endif
            }

            ProgressPercentage++;
        }

        private void XMLtoCheckboxes()
        {
            XmlNode node = Helpers.DanXML.FindNode(doc.ChildNodes[1].ParentNode, new List<string>() { "files" });

            foreach (XmlNode nodeChild in node.ChildNodes)
            {
                string usage = Helpers.DanXML.FindAttribute(nodeChild, "usage");
                CheckBox check = new CheckBox();
                check.Content = Helpers.DanXML.FindNode(nodeChild, new List<string>(){ "name" }).InnerText;

                foreach (Grid grid in grids)
                {
                    GroupBox box = (GroupBox)grid.Parent;
                    string header = (string)box.Header;

                    if (header.ToLower() == usage.ToLower())
                    {
                        grid.Children.Add(check);
                    }
                }
            }
        }

    }
}
