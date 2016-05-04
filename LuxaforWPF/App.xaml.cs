using LuxaforSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LuxaforWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IDeviceList devList = new DeviceList();
            devList.Scan();
            foreach(IDevice device in devList)
            {
                device.Dispose();
            }
            
        }
    }
}
