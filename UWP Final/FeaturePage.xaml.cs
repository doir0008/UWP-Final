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
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

/*
 * Created by Ryan Doiron (doir0008@algonquinlive.com)
 * 
 */

namespace UWP_Final
{

    public sealed partial class FeaturePage : Page
    {

        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;

        // MS emotion service setup
        const string APIKEY = "e046a01f50c342389a07143131b97aad";
        EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);
        Emotion[] emotionResult;

        public FeaturePage()
        {
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedAspectRatio = new Size(200, 200);
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
                ApplicationData.Current.LocalSettings.Values.Remove("Feature Page Data");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Feature Page Data"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["Feature Page Data"] as ApplicationDataCompositeValue;
                    ApplicationData.Current.LocalSettings.Values.Remove("Feature Page Data");
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
            ApplicationData.Current.LocalSettings.Values["Feature Page Data"] = composite;
            base.OnNavigatedFrom(e);
        }

        #endregion

        private async void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            // camera code to handle capture, storage and display of photo
            try
            {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (photo == null)
                {
                    return;
                }
                else
                {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
                    image.Source = bitmapSource;
                }
            }
            catch (Exception)
            {
                textBlock.Text = "Error taking the picture";
                //throw;
            }

            // emotion code, grabs elements returned from emotionservice and displays results
            try
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());
                if (emotionResult != null)
                {
                    EmotionScores scores = emotionResult[0].Scores;
                    textBlock.Text = "According to a robot, your emotions are: \n\n" +
                        "Happiness: " + scores.Happiness + "\n" +
                        "Sadness: " + scores.Sadness + "\n" +
                        "Fear: " + scores.Fear + "\n" +
                        "Neutral: " + scores.Neutral;

                    if (scores.Neutral > 0.5)
                    {
                        textBlock.Text += "\n\nRobot says: \"Ah, I see we are more similar than I had thought, human.\"";
                    }
                    else
                    {
                        textBlock.Text += "\n\nRobot says: \"Your emotions will be your undoing, foolish bag of mostly water.\"";
                    }
                }
            }
            catch (Exception)
            {
                textBlock.Text = "Error! Emotions not found. Bleep bloop.";
                //throw;
            }
        }
    }
}
