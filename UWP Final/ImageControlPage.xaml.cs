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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

/*
 * Created by Ryan Doiron (doir0008@algonquinlive.com)
 * 
 */

namespace UWP_Final
{
    public sealed partial class ImageControlPage : Page
    {
        public ImageControlPage()
        {
            this.InitializeComponent();
        }

        private void MediaElementPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MediaElementPage));
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
                ApplicationData.Current.LocalSettings.Values.Remove("Image Control Page Data");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Image Control Page Data"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["Image Control Page Data"] as ApplicationDataCompositeValue;
                    ApplicationData.Current.LocalSettings.Values.Remove("Image Control Page Data");
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
            ApplicationData.Current.LocalSettings.Values["Image Control Page Data"] = composite;
            base.OnNavigatedFrom(e);
        }

        #endregion

        // open image file picker and display image on page
        private async void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    image.Source = bitmapImage;
                }
            }
        }

    }
}
