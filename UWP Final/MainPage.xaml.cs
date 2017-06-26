using System;
using System.Collections.Generic;
using System.IO;
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

/*
 * Created by Ryan Doiron (doir0008@algonquinlive.com)
 * 
 */

namespace UWP_Final
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region page navigation

        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void ImageControlPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImageControlPage));
        }

        private void MediaElementPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MediaElementPage));
        }

        private void FeaturePage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FeaturePage));
        }

        #endregion

        // popup msg for cartman button
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog respectMyAuthority = new ContentDialog
            {
                Title = "Respect My Authoritah!",
                Content = "Hey! I am a Cop and you will respect My authoritah!",
                PrimaryButtonText = "Ok"
            };
            ContentDialogResult result = await respectMyAuthority.ShowAsync();
        }
    }
}
