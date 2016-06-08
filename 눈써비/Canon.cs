using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Diagnostics;
using System.Windows.Threading;

namespace Kinect2FaceHD_NET
{
    class Canon
    {

        CanonAPI APIHandler;
        Camera MainCamera;
        CameraValue[] AvList;
        CameraValue[] TvList;
        CameraValue[] ISOList;
        List<Camera> CamList;
        bool IsInit = false;
       
        ImageBrush bgbrush = new ImageBrush();
        Action<BitmapImage> SetImageAction;
        //System.Windows.Forms.FolderBrowserDialog SaveFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();


        int ErrCount;
        object ErrLock = new object();


        public void start()
        {
            try
            {
               
                APIHandler = new CanonAPI();
                APIHandler.CameraAdded += APIHandler_CameraAdded;
                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;
                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
                // SavePathTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "RemotePhoto");
                SetImageAction = (BitmapImage img) => { bgbrush.ImageSource = img; };
                //SaveFolderBrowser.Description = "Save Images To...";
                RefreshCamera();

            }
            catch (DllNotFoundException) { ReportError("Canon DLLs not found!", true); }
            catch (Exception ex) { ReportError(ex.Message, true); }

        }


        public void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                MainCamera?.Dispose();
                APIHandler?.Dispose();
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }
        public void Stop()
        {

            try
            {
                MainCamera?.Dispose();
                APIHandler?.Dispose();
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsInit == true)
            {
                OpenSession();
               // StarLV();
            }

        }

        public void Continue()
        {
            if (IsInit == true)
            {
                OpenSession();
               // StarLV();
            }
        }



        private void OpenSession()
        {

            MainCamera = CamList[0];
            MainCamera.OpenSession();
            MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
            MainCamera.StateChanged += MainCamera_StateChanged;

            AvList = MainCamera.GetSettingsList(PropertyID.Av);
            TvList = MainCamera.GetSettingsList(PropertyID.Tv);
            ISOList = MainCamera.GetSettingsList(PropertyID.ISO);
            MainCamera.SetSetting(PropertyID.Av, AvValues.GetValue(AvList[0].IntValue));
            MainCamera.SetSetting(PropertyID.Tv, TvValues.GetValue(TvList[0].IntValue));
            MainCamera.SetSetting(PropertyID.ISO, ISOValues.GetValue(ISOList[6].IntValue));



        }




        private void MainCamera_StateChanged(Camera sender, StateEventID eventID, int parameter)
        {
            try { if (eventID == StateEventID.Shutdown) { Application.Current.Dispatcher.Invoke((Action)delegate { CloseSession(); }); } }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }


        private void CloseSession()
        {
            MainCamera.CloseSession();
            //AvCoBox.Items.Clear();
            // TvCoBox.Items.Clear();
            // ISOCoBox.Items.Clear();
            // SettingsGroupBox.IsEnabled = false;
            // LiveViewGroupBox.IsEnabled = false;
            // SessionButton.Content = "Open Session";
            // SessionLabel.Content = "No open session";
        }

        /*
        private void StarLV(Source)
        {
            try
            {
                if (!MainCamera.IsLiveViewOn)
                {
                    LVCanvas.Background = bgbrush;
                    lv.Background = bgbrush;
                    MainCamera.StartLiveView();
                    //StarLVButton.Content = "Stop LV";
                }
                else
                {
                    MainCamera.StopLiveView();
                    //StarLVButton.Content = "Start LV";
                    LVCanvas.Background = Brushes.LightGray;
                }
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }
        */

        private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
            try
            {

                using (WrapStream s = new WrapStream(img))
                {
                    img.Position = 0;
                    BitmapImage EvfImage = new BitmapImage();
                    EvfImage.BeginInit();
                    EvfImage.StreamSource = s;
                    EvfImage.CacheOption = BitmapCacheOption.OnLoad;
                    EvfImage.EndInit();
                    EvfImage.Freeze();
                    Application.Current.Dispatcher.Invoke(SetImageAction, EvfImage);
                }
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }



        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
            ReportError($"SDK Error code: {ex} ({((int)ex).ToString("X")})", false);
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            ReportError(ex.Message, true);
        }

        private void APIHandler_CameraAdded(CanonAPI sender)
        {
            try { Application.Current.Dispatcher.Invoke((Action)delegate { RefreshCamera(); }); }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void ReportError(string message, bool lockdown)
        {
            int errc;
            lock (ErrLock) { errc = ++ErrCount; }

            if (lockdown) EnableUI(false);

            if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            lock (ErrLock) { ErrCount--; }
        }


        private void EnableUI(bool enable)
        {
            if (!Application.Current.Dispatcher.CheckAccess()) Application.Current.Dispatcher.Invoke((Action)delegate { EnableUI(enable); });
            else
            {
                // SettingsGroupBox.IsEnabled = enable;
                // InitGroupBox.IsEnabled = enable;
                // LiveViewGroupBox.IsEnabled = enable;
            }
        }


        private void RefreshCamera()
        {
            Debug.WriteLine("in");
            // CameraListBox.Items.Clear();
            CamList = APIHandler.GetCameraList();
            foreach (Camera cam in CamList)
            {
                Debug.WriteLine(cam.DeviceName);
                IsInit = true;
            }
            //  foreach (Camera cam in CamList) CameraListBox.Items.Add(cam.DeviceName);
            // if (MainCamera?.SessionOpen == true)
            //  CameraListBox.SelectedIndex = CamList.FindIndex(t => t.ID == MainCamera.ID);
            //  else if (CamList.Count > 0) CameraListBox.SelectedIndex = 0;
        }

       
    }
}
