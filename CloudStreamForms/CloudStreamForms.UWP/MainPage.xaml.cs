﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CloudStreamForms;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.System;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using static CloudStreamForms.Main;
using System.Text.RegularExpressions;

namespace CloudStreamForms.UWP
{
    public sealed partial class MainPage
    {
        MainUWP mainUWP;
        public static MainPage mainPage;
        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
           Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            LoadApplication(new CloudStreamForms.App());
            mainPage = this;
            mainUWP = new MainUWP();
            mainUWP.Awake();

        }



    }

    public class MainUWP : CloudStreamForms.App.IPlatformDep
    {

        public static string GetPath(string filename)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            return storageFolder.Path + "\\" + CensorFilename(filename);
        }

        static string CensorFilename(string name)
        {
            name = Regex.Replace(name, @"[^A-Za-z0-9\.]+", String.Empty);
            name.Replace(" ", "");
            name = name.ToLower();
            return name;
        }
        public static async Task CreateFile(string filename, byte[] write, bool autoOpen = true)
        {
            filename = CensorFilename(filename);
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var file = await storageFolder.GetFileAsync(filename);
            var s = await file.OpenStreamForWriteAsync();

            s.Write(write, 0, write.Length);
            s.Close();
            if (autoOpen) {
                var success = await Windows.System.Launcher.LaunchFileAsync(file);
            }
        }

        public static async Task OpenPathsAsVideo(List<string> path, List<string> name, string subtitleLoc = "")
        {

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            bool subEnabled = subtitleLoc != "";

            await CreateFile(CloudStreamForms.App.baseM3u8Name, CloudStreamForms.App.ConvertPathAndNameToM3U8Bytes(path, name, subEnabled, storageFolder.Path + "\\"));
            if (subEnabled) {
                await CreateFile(CloudStreamForms.App.baseSubtitleName, Encoding.UTF8.GetBytes(subtitleLoc));
            }

            //Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(CloudStreamForms.App.baseM3u8Name, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            //#EXTVLCOPT:sub-file=sample2.srt
        }

        public static async void OpenPathAsVideo(string path, string name = "", string subtitleLoc = "")
        {
            await OpenPathsAsVideo(new List<string>() { path }, new List<string>() { name }, subtitleLoc);
            // string vlcPath = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";
            //string args = "-vvv " + path;
            /*
            System.Diagnostics.Process VLC = new System.Diagnostics.Process();
            VLC.StartInfo.FileName = 
            VLC.StartInfo.Arguments = 
            VLC.Start();
            */

            /*
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null) {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            }
            StorageFolder newFolder;

            newFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            await newFolder.CreateFileAsync("testYeet.txt");*/

            /*
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                //  try {
                var result = await ProcessLauncher.RunToCompletionAsync(vlcPath, args, options);
            //    Console.WriteLine(result.ExitCode + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                //   }
                //  catch (System.Exception e) {

                //}
            });*/

            // await StorageApplicationPermissions.FutureAccessList.GetFileAsync("C:\\Program Files\\VideoLAN\\VLC");
            //   await Windows.System.ProcessLauncher.RunToCompletionAsync(vlcPath, args, new Windows.System.ProcessLauncherOptions() { WorkingDirectory = "C:\\Program Files\\VideoLAN\\VLC" });
            /*
            var uriPath = new Uri(path);

            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(uriPath);*/
            //CloudStreamForms.App.OpenBrowser(@"file:///C:/Program Files/VideoLAN/VLC/vlc.exe");
        }
        public void Awake()
        {
            CloudStreamForms.App.platformDep = this;
        }

        public void PlayVlc(string url, string name, string subtitleLoc)
        {
            //MainUWP.OpenPathAsVideo(url,name);
            try {
                MainUWP.OpenPathAsVideo(url, name, subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url);
            }
        }
        public void PlayVlc(List<string> url, List<string> name, string subtitleLoc)
        {
            //MainUWP.OpenPathAsVideo(url,name);
            try {
                MainUWP.OpenPathsAsVideo(url, name, subtitleLoc);

            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url.FirstOrDefault());
            }
        }

        public void ShowToast(string message, double duration)
        {
            Message.ShowMessage(message, duration);
        }

        public string DownloadFile(string file, string fileName, bool mainPath, string extraPath)
        {
            DownloadFile(Encoding.UTF8.GetBytes(file), fileName, mainPath, extraPath);
            return GetPath(fileName);
        }
        public void DownloadFile(byte[] file, string fileName, bool mainPath, string extraPath)
        {
            CreateFile(fileName, file, false);
        }


        public string DownloadUrl(string url, string fileName, bool mainPath, string extraPath)
        {
            try {
                TempThred tempThred = new TempThred();
                tempThred.typeId = 4; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        WebClient webClient = new WebClient();
                        byte[] data = webClient.DownloadData(url);
                        DownloadFile(data, fileName, mainPath, extraPath);
                        //if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Download Thread";
                tempThred.Thread.Start();


                return GetPath(fileName);

            }
            catch (Exception) {
                CloudStreamForms.App.ShowToast("Download Failed");
                return "";
            }
        }

        public string GetBuildNumber()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return v.Major + "." + v.Minor + "." + v.Build;
        }

        public void DeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }

        public void DownloadUpdate(string update)
        {
           // throw new NotImplementedException();
        }
    }

    public static class Message
    {
        private const double LONG_DELAY = 3.5;
        private const double SHORT_DELAY = 2.0;

        public static void LongAlert(string message) =>
          ShowMessage(message, LONG_DELAY);

        public static void ShortAlert(string message) =>
          ShowMessage(message, SHORT_DELAY);

        public static void ShowMessage(string message, double duration)
        {
            var label = new TextBlock {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Windows.UI.Colors.White)));
            style.Setters.Add(new Setter(FrameworkElement.MaxHeightProperty, 1));
            var flyout = new Flyout {
                Content = label,
                Placement = FlyoutPlacementMode.Bottom,

                FlyoutPresenterStyle = style,
            };

            flyout.ShowAt(Window.Current.Content as FrameworkElement);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            timer.Tick += (sender, e) => {
                timer.Stop();
                flyout.Hide();
            };
            timer.Start();
        }
    }
}
