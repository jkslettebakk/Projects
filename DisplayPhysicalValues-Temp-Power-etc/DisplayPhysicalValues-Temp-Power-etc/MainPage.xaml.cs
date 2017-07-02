using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DisplayPhysicalValues_Temp_Power_etc
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string IODataFile = @"D:\tmp\IODataLog.txt"; //Pointing to default catalogue

        public MainPage()
        {
            this.InitializeComponent();

            InitializIoT();

            ReadLoggedDataFromFile();

        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
 
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {

        }

        private bool InitializIoT()
        {
            // Initialize graphic, Temperature/Power
            // Handle error if failing

            try
            {
                // Initialise graphics

                // Initialise IoT temperature, power and other IO functions

                return true;

            }

            catch (InvalidCastException e)
            {
                return false;
            }

        }

        private void ReadLoggedDataFromFile()
        {
            // read data from file if it exists, else create file for datastore
            using (StreamWriter sw = File.AppendText(IODataFile))
            {
                    Random random = new Random();
                    int randomNumber = random.Next(0, 1000);

                    sw.Write("Jann" + ",");
                    sw.Write("Slettebakk" + ",");
                    sw.WriteLine("Jobb " + randomNumber.ToString());
            }
        }

    }
}
