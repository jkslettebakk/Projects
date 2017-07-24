

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTRemovableFiles
{
    public class MyAppData
    {
        public string MAINPAGETITLE { get; set; }
        public int MAININT { get; set; }
        public string LOGFILENAME { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MyAppData AppData { get; set; }

        public async Task OpenAppFile(string fileName)
        {


            try // Create a local files (open and add text if exist) for writing a short text
            {
                // Create sample file, open if exists
                string fn = fileName + "-Sequensial.txt";
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync(fn,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);
                // Writing to the file
                await Windows.Storage.FileIO.AppendTextAsync(sampleFile, "Swift as a shadow" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                DebugTextBox.Text += ex.Message;
            }

            try // Create a local files (open and add text if exist) for writing a short text
            {
                // Create sample stream file, open if exists
                string fn = fileName + "-Stream.txt";
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync(fn,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);
                var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                using (var outputStream = stream.GetOutputStreamAt(stream.Size))
                {
                    // We'll add more code here in the next step.
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteString("DataWriter has methods to write to various types, such as DataTimeOffset." + Environment.NewLine);
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.

            }
            catch (Exception ex)
            {
                DebugTextBox.Text += ex.Message;
            }

            try // Initial/create OneDrive stream for write/append text
            {
                // Create file Stream towards OneDrive datastore

            }
            catch (Exception ex)
            {
                DebugTextBox.Text += ex.Message;
            }

            //OK, then we are ready to read/write file

            /*            try // Try to open file at USB drive
                        {
                            var removableDevices = KnownFolders.RemovableDevices;
                            var externalDrives = await removableDevices.GetFoldersAsync();
                            var usbDrive = externalDrives.Single(e => e.DisplayName.Contains("USB DISK"));
                            string fn = usbDrive.Name + string.Format("{0}.jpg", fileName);

                            DebugTextBox.Text += "Will try to open:" + fn + " on USB Drive:" + usbDrive.Name;

                            StorageFile appconfig = await usbDrive.CreateFileAsync(
                                fn,
                                CreationCollisionOption.OpenIfExists);

                            using (StreamReader reader = new StreamReader(await appconfig.OpenStreamForReadAsync()))
                            {
                                var data = await reader.ReadToEndAsync();
                                AppData = Newtonsoft.Json.JsonConvert.DeserializeObject<MyAppData>(data);
                            }
                        }
                        catch (Exception ex)
                        {
                            // error
                            DebugTextBox.Text += ex.Message;
                        }
            */
            try
            {
                // Open and read the file

                // Create/open sample file; replace if exists.

                string fn = fileName + ".jpg";
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;

                DebugTextBox.Text += "Will try to open: " + fn + " in " + storageFolder.Path;

                StorageFile appconfig = await storageFolder.CreateFileAsync(fn,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);

                using (StreamReader reader = new StreamReader(await appconfig.OpenStreamForReadAsync()))
                {
                    var data = await reader.ReadToEndAsync();
                    AppData = Newtonsoft.Json.JsonConvert.DeserializeObject<MyAppData>(data);
                }

            }
            catch (Exception ex)
            {

                // Exceptopn
                DebugTextBox.Text += ex.Message;

            }

        }

        StorageFile logFile;
        public async Task InitLog(string fileName)
        {
            logFile = null;
            var currentFolder = string.Format("{0}{1:00}{2:00}{3}",
                fileName,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Year);
            var removableDevices = KnownFolders.RemovableDevices;
            var externalDrives = await removableDevices.GetFoldersAsync();
            var usbDrive = externalDrives.Single(e => e.DisplayName.Contains("USB DISK"));
            var meWattFolder = await usbDrive.CreateFolderAsync(
                currentFolder, CreationCollisionOption.OpenIfExists);
            logFile = await meWattFolder.CreateFileAsync(
                string.Format("{0}.jpg", fileName), CreationCollisionOption.OpenIfExists);
        }

        public async Task<object> WriteLogAsync(string log, object asyncObject = null)
        {
            using (var outputStream = await logFile.OpenStreamForWriteAsync())
            {
                outputStream.Seek(0, SeekOrigin.End);
                DataWriter dataWriter = new DataWriter(outputStream.AsOutputStream());
                dataWriter.WriteString(log);
                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();
                outputStream.Dispose();
            }
            return asyncObject;
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }


       private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            await this.OpenAppFile("MyAppData");

            titleTextBlock.Text = AppData.MAINPAGETITLE;
            DebugTextBox.Text += AppData.MAININT.ToString();

            await InitLog(AppData.LOGFILENAME).ContinueWith(async (antecedent) =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        logButton.IsEnabled = true;
                    });
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            logButton.IsEnabled = false;
            await WriteLogAsync(Environment.NewLine + DebugTextBox.Text).ContinueWith(async (successAsync) =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        DebugTextBox.Text += string.Empty;
                        logButton.IsEnabled = true;
                    });
            });
        
        }

        // The following code snippet shows how to asynchronously check if a file or folder exists in the specific folder. 
        public async Task<bool> IfStorageItemExist(StorageFolder folder, string itemName)
        {
            try
            {
                IStorageItem item = await folder.TryGetItemAsync(itemName);
                return (item != null);
            }
            catch (Exception ex)
            {
                // Should never get here 
                return false;
            }
        }

    }
}