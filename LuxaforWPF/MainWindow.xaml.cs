using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using LuxaforSharp;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections;

namespace LuxaforWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDeviceList deviceList;
        private IDevice dev;
        private Boolean listenPhone = true;
        private LuxaforSharp.Color prevColor;
        FileSystemWatcher watcher;
        string pathToMonitor;
        string resultFile;
        
        public MainWindow()
        {
            InitializeComponent();
            initStuff();
            
        }

        private void initStuff()
        {
            try
            {
                deviceList = new DeviceList();
                deviceList.Scan();
                dev = deviceList.First();
                listenPhone = true;
                // string test = "";
                //cbDevices.ItemsSource = deviceList;
               // if(deviceList != null && deviceList.ToList().Count > 0)
                 //   cbDevices.ItemsSource = deviceList;
               /*  foreach(var item in deviceList)
                {
                    cbDevices.Items.Add((Device)item);
                }*/

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
            }
            if (Properties.Settings.Default.pathToMonitor != "0" )
                {
                    watcher = new FileSystemWatcher();
                    pathToMonitor = Properties.Settings.Default.pathToMonitor;
                    resultFile = Path.GetDirectoryName(pathToMonitor) + "\\result.txt";
                tbStatusbar.Text = pathToMonitor;
                watcher.Path = Path.GetDirectoryName(pathToMonitor);
                    watcher.Filter = Path.GetFileName(pathToMonitor);
                    watcher.NotifyFilter = NotifyFilters.Attributes | 
                                            NotifyFilters.CreationTime |
                                            NotifyFilters.FileName |
                                            NotifyFilters.LastAccess |
                                            NotifyFilters.LastWrite |
                                            NotifyFilters.Size |
                                            NotifyFilters.Security;

                    watcher.Changed += new FileSystemEventHandler(filesystemWatcher_Changed);
                    watcher.Created += new FileSystemEventHandler(filtersystemWatcer_Created);
                    watcher.EnableRaisingEvents = true;
                }
            try
            {
                string line = readLine();
                string[] rgb = line.Trim().Split(',');
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = System.Windows.Media.Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2]));
                Application.Current.Resources["statusColor"] = brush;
                ecStatus.Fill = (SolidColorBrush)Application.Current.Resources["statusColor"];
                tbStatusbar.Text = "Listening Phone! File : " + pathToMonitor;
                prevColor = new LuxaforSharp.Color(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2]));
                dev.SetColor(LedTarget.All, prevColor);
            }catch(Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }



        private void filtersystemWatcer_Created(object sender, FileSystemEventArgs e)
        {
            //MessageBox.Show("Something new created!");


        }

        private void filesystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {

            try
            {
                string line = readLine();
                if (!String.IsNullOrEmpty(line))
                {
                       // string temp = "";
                        string[] c = null;
                        if (line != null && line.Length > 0 && line.Any(Char.IsDigit))
                        {
                            c = line.Trim().Split(',');

                            if (c != null && c.Length == 5)
                            {
                            prevColor = new LuxaforSharp.Color(Convert.ToByte(c[0]), Convert.ToByte(c[1]), Convert.ToByte(c[2]));
                            dev.SetColor(LedTarget.All, prevColor);
                            dev.Blink(LedTarget.All, prevColor, Convert.ToByte(c[3]), Convert.ToByte(c[4]));

                            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                                {
                                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                        new Action(
                                            delegate ()
                                            {
                                                System.Windows.Media.Color from = System.Windows.Media.Color.FromRgb(prevColor.Red, prevColor.Green, prevColor.Blue);
                                                System.Windows.Media.Color to = Colors.Gray;
                                                var colorAnimation = new ColorAnimation(from, to, TimeSpan.FromSeconds(1));
                                                
                                                Storyboard sb = new Storyboard();
                                                sb.Duration = TimeSpan.FromSeconds(1);
                                                sb.RepeatBehavior = new RepeatBehavior(Convert.ToDouble(c[4])+1);
                                                Storyboard.SetTarget(colorAnimation, ecStatus);
                                                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Fill.Color"));
                                                sb.Children.Add(colorAnimation);
                                                sb.Completed += animCompleted;
                                                sb.Begin();
                                            }
                                            ));
                                }
                          

                            // temp = DateTime.Now.ToString("yyyyMMddHHmmssffff") + " Color Changed to " + prevColor.Red + ", " + prevColor.Green + ", " + prevColor.Blue;                 
                        }
                            else if (c != null && c.Length == 3)
                            {
                               

                            if(this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                            {
                                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(
                                        delegate ()
                                        {
                                            SolidColorBrush brush = new SolidColorBrush();
                                            brush.Color = System.Windows.Media.Color.FromRgb(Convert.ToByte(c[0]), Convert.ToByte(c[1]), Convert.ToByte(c[2]));
                                            Application.Current.Resources["statusColor"] = brush;
                                            ecStatus.Fill = (SolidColorBrush)Application.Current.Resources["statusColor"];
                                        }
                                        ));
                            }
                            prevColor = new LuxaforSharp.Color(Convert.ToByte(c[0]), Convert.ToByte(c[1]), Convert.ToByte(c[2]));
                           // foreach(var item in deviceList)
                          //  {
                                dev.SetColor(LedTarget.All, prevColor);
                          //  }
                            //Device device = (Device)dev;
                           // MessageBox.Show(device.DevicePath);
                           // Device device2 = (Device)deviceList.ElementAt(1);
                           // MessageBox.Show("2nd Device " + device2.DevicePath);
                            //MessageBox.Show(deviceList[1]);
                          //  dev.SetColor(LedTarget.All, prevColor);       
                        }
           
                     }
                    else if (line.ToLower() == "status")
                    {
                       var stream = new FileStream(@Properties.Settings.Default.status, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        string a =DateTime.Now.ToString("yyyyMMddHHmmssffff")+ ": Color : " + prevColor.Red + "," + prevColor.Green + "," + prevColor.Blue + ". Status :";
                        if (listenPhone && dev != null)
                            a += " Listening phone.";
                        if (!listenPhone)
                            a += " Not listening phone.";
                        else if (dev == null)
                            a += "No Luxafor- device detected.";
                        using (var writer = new StreamWriter(stream))
                            writer.WriteLine(a);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
    }
        private string readLine()
        {
            var stream = new FileStream(pathToMonitor, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            reader.Close();
            return line;
        }
        private void animCompleted(object sender, EventArgs e)
        {
           
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromRgb(prevColor.Red, prevColor.Green, prevColor.Blue);
            Application.Current.Resources["statusColor"] = brush;
            ecStatus.Fill = (SolidColorBrush)Application.Current.Resources["statusColor"];
                        
        }

        private void miEnable_Checked(object sender, RoutedEventArgs e)
        {
           
                try
                {
                    deviceList = new DeviceList();
                    deviceList.Scan();
                    dev = deviceList.First();
                }catch(InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                    miEnable.IsChecked = false;
                }
            
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromRgb(0, 204, 0);
            ecStatus.Fill = brush;
            if (dev != null)
            {
                listenPhone = false;
                prevColor = new LuxaforSharp.Color(0, 204, 0);
                dev.SetColor(LedTarget.All, new LuxaforSharp.Color(0, 204, 0));
                tbStatusbar.Text = "Phone listening off. Status: Available";
            }
            else
            {
                MessageBox.Show("Device null");
            }
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromRgb(255, 255, 0);
            ecStatus.Fill = brush;
            if (dev != null)
            {
                listenPhone = false;
                prevColor = new LuxaforSharp.Color(255, 255, 0);
                dev.SetColor(LedTarget.All, prevColor);
                tbStatusbar.Text = "Phone listening off. Status: Away";
            }
            else
            {
                MessageBox.Show("Device null");
            }
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);
            ecStatus.Fill = brush;
            if (dev != null)
            {
                listenPhone = false;
                prevColor = new LuxaforSharp.Color(255, 0, 0);
                
               // System.DrwaSolidBrush solid =new SolidBrush(Color.FromArgb())
                dev.SetColor(LedTarget.All, prevColor);
                tbStatusbar.Text = "Phone listening off. Status: Busy";
            }
            else
            {
                MessageBox.Show("Device null");
            }
        }

        private void btnListenFile_Click(object sender, RoutedEventArgs e)
        {
            // Storyboard sb = (Storyboard)FindResource("sbCall");
            // Storyboard.SetTarget(sb, ecStatus);
            // sb.Begin();
            try
            {
                if (pathToMonitor.Length > 2)
                {
                    listenPhone = true;
                    tbStatusbar.Text = "Listening Phone! File : " + pathToMonitor;
                    string line = readLine();
                    string[] rgb = line.Trim().Split(',');
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = System.Windows.Media.Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2]));
                    Application.Current.Resources["statusColor"] = brush;
                    ecStatus.Fill = (SolidColorBrush)Application.Current.Resources["statusColor"];

                    prevColor = new LuxaforSharp.Color(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2]));
                    dev.SetColor(LedTarget.All, prevColor);
                }
            }catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void miSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //  Gat.Controls.OpenDialogView od = new Gat.Controls.OpenDialogView();
            // Gat.Controls.OpenDialogViewModel vm = (Gat.Controls.OpenDialogViewModel)od.DataContext;
            //  vm.IsDirectoryChooser = true;


            ofd.Filter = "Text files(*.txt)|*.txt|All Files(*.*)|*.*";

            if (ofd.ShowDialog() == true)
            // if(vm.Show() == true)
            {
                try
                {
                    // watcher.EnableRaisingEvents = false;
                    watcher = new FileSystemWatcher();
                    // pathToMonitor = vm.SelectedFolder.Path;
                    pathToMonitor = ofd.FileName;
                    // pathToMonitor = vm.SelectedFolder.Path + "\\set.txt";
                    //resultFile =vm.SelectedFolder.Path+"\\result.txt";
                    resultFile = Path.GetDirectoryName(pathToMonitor) + "\\result.txt";
                    //watcher.Path = pathToMonitor;
                    //   MessageBox.Show(pathToMonitor);
                    //  btnSelectFile.Content = pathToMonitor;
                    tbStatusbar.Text = pathToMonitor;
                    watcher.Path = Path.GetDirectoryName(pathToMonitor);
                    watcher.Filter = Path.GetFileName(pathToMonitor);
                    watcher.NotifyFilter = NotifyFilters.Attributes |
                                           NotifyFilters.CreationTime |
                                           NotifyFilters.FileName |
                                           NotifyFilters.LastAccess |
                                           NotifyFilters.LastWrite |
                                           NotifyFilters.Size |
                                           NotifyFilters.Security;

                    watcher.Changed += new FileSystemEventHandler(filesystemWatcher_Changed);
                    watcher.Created += new FileSystemEventHandler(filtersystemWatcer_Created);
                    watcher.EnableRaisingEvents = true;


                    Properties.Settings.Default.pathToMonitor = pathToMonitor;
                    Properties.Settings.Default.status = resultFile;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();

                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message);
                }
            }
        }

        private void btnOff_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = System.Windows.Media.Colors.Gray;
            Application.Current.Resources["statusColor"] = brush;
            ecStatus.Fill = (SolidColorBrush)Application.Current.Resources["statusColor"];
            if (dev != null)
            {
                dev.SetColor(LedTarget.All, new LuxaforSharp.Color(0, 0, 0));
            }
        }

        /*  private void cbDevices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
          {
            //  MessageBox.Show(cbDevices.SelectedIndex+"");
                  dev = deviceList.ElementAt(cbDevices.SelectedIndex);
                 // dev.SetColor(LedTarget.All, prevColor);

          }*/
    }
}