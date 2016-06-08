using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Windows.Shapes;

namespace Kinect2FaceHD_NET
{
    /// <summary>
    /// Interaction logic for Draw.xaml
    /// </summary>
    public partial class Draw : Page
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

        int ErrCount;
        object ErrLock = new object();
        //canon


        //

        private KinectSensor _sensor = KinectSensor.GetDefault();

        private BodyFrameSource _bodySource = null;

        private BodyFrameReader _bodyReader = null;

        private HighDefinitionFaceFrameSource _faceSource = null;

        private HighDefinitionFaceFrameReader _faceReader = null;

        private FaceAlignment _faceAlignment = null;

        private FaceModel _faceModel = null;

        private List<Ellipse> _points = new List<Ellipse>();
        private ColorFrameReader reader = null;
        private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private readonly WriteableBitmap _bmp = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgr32, null);
        Byte[] _frameData = null;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;


        public Draw()
        {
            this.WindowHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.WindowWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            try
            {
                InitializeComponent();
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



        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                MainCamera?.Dispose();
                APIHandler?.Dispose();
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsInit == true)
            {
                OpenSession();
                StarLV();
            }

            //
            _sensor = KinectSensor.GetDefault();
            _sensor.Open();
            //
            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            //
            //  this.reader = _sensor.ColorFrameSource.OpenReader();
            // reader.FrameArrived += ColorFrameArrived;
            if (_sensor != null)
            {
                _bodySource = _sensor.BodyFrameSource;
                _bodyReader = _bodySource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                _faceSource = new HighDefinitionFaceFrameSource(_sensor);

                _faceReader = _faceSource.OpenReader();
                _faceReader.FrameArrived += FaceReader_FrameArrived;

                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();

                _sensor.Open();
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
            try { if (eventID == StateEventID.Shutdown) { Dispatcher.Invoke((Action)delegate { CloseSession(); }); } }
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


        private void StarLV()
        {
            try
            {
                if (!MainCamera.IsLiveViewOn)
                {
                    LVCanvas.Background = bgbrush;
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
            try { Dispatcher.Invoke((Action)delegate { RefreshCamera(); }); }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void ReportError(string message, bool lockdown)
        {
            int errc;
            lock (ErrLock) { errc = ++ErrCount; }

            if (lockdown) EnableUI(false);

            if (errc < 4)
            {
                Debug.WriteLine(message);
                //MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (errc == 4)
            {
                Debug.WriteLine(message);
                //MessageBox.Show("Many errors happened!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            lock (ErrLock) { ErrCount--; }
        }


        private void EnableUI(bool enable)
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.Invoke((Action)delegate { EnableUI(enable); });
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body[] bodies = new Body[frame.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

                    Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();

                    if (!_faceSource.IsTrackingIdValid)
                    {
                        if (body != null)
                        {
                            _faceSource.TrackingId = body.TrackingId;
                        }
                    }
                }
            }
        }


        private void FaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null && frame.IsFaceTracked)
                {
                    frame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    //UpdateFacePoints();
                }
            }


        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {

            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                // Find the joints
                                Joint handRight = body.Joints[JointType.HandRight];
                                Joint thumbRight = body.Joints[JointType.ThumbRight];

                                Joint handLeft = body.Joints[JointType.HandLeft];
                                Joint thumbLeft = body.Joints[JointType.ThumbLeft];

                            }
                        }
                    }
                }
            }
        }


    }
}
