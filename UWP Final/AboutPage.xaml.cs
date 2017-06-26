using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private void ImageControlPage_Click(object sender, RoutedEventArgs e)
        {
            // navigate to next page
            Frame.Navigate(typeof(ImageControlPage));
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
                ApplicationData.Current.LocalSettings.Values.Remove("About Page Data");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("About Page Data"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["About Page Data"] as ApplicationDataCompositeValue;
                    ApplicationData.Current.LocalSettings.Values.Remove("About Page Data");
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
            ApplicationData.Current.LocalSettings.Values["About Page Data"] = composite;
            base.OnNavigatedFrom(e);
        }

        #endregion


        private async void emailButton_Click(object sender, RoutedEventArgs e)
        {
            // verbatim string, the @ means no escape sequences and it won't search
            // for them. its actually faster.
            string address = @"mailto:rdoiron@gmail.com";
            Uri email = new Uri(address);
            await Windows.System.Launcher.LaunchUriAsync(email);
        }

    }
}
