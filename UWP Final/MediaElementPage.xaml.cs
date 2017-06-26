using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

/*
 * Created by Ryan Doiron (doir0008@algonquinlive.com)
 * 
 */

namespace UWP_Final
{
    public sealed partial class MediaElementPage : Page
    {
        public MediaElementPage()
        {
            this.InitializeComponent();
        }

        private void FeaturePage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FeaturePage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // we do the below check in case the app is suspended/killed and nav
            // data has been lost
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        #region Navigation To and From with Back Button

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += backButton_Tapped;
            // loading app state

            // are we coming from a suspend and terminate situation?
            // NavigationMode.New means NOT suspend and terminate
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("Media Element Page Data");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Media Element Page Data"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["Media Element Page Data"] as ApplicationDataCompositeValue;
                    ApplicationData.Current.LocalSettings.Values.Remove("Media Element Page Data");
                }
                base.OnNavigatedTo(e);
            }
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= backButton_Tapped;
            // saving application data
            // this object knows how to serialize and de-serialize
            var composite = new ApplicationDataCompositeValue();
            ApplicationData.Current.LocalSettings.Values["Media Element Page Data"] = composite;
            base.OnNavigatedFrom(e);
        }

        #endregion

        // media file picker and play content plus media controls
        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                mediaElement.AreTransportControlsEnabled = true;
                var stream = await file.OpenAsync(FileAccessMode.Read);
                mediaElement.SetSource(stream, file.ContentType);
            }
        }
    }
}
