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
using System.Drawing;
using System.Windows.Interop;
using System.Drawing;
using System.Drawing.Imaging;


namespace Kinect2FaceHD_NET
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {

      
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
        FaceLogin fl = new FaceLogin();
        int count = 0;
        Boolean v_check = false;

        public Login()
        {
            InitializeComponent();
            //fl.getNumber();
           //  fl.Login();
            // camera.Visibility = Visibility.Hidden;
            //Capture();
            //CopyScreen();
           
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
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

        private static void CopyScreen()
        {
           
            var width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            var height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
             var bmpGraphics = Graphics.FromImage(screenBmp);
               
             bmpGraphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
            Bitmap bm3 = new Bitmap(screenBmp);
        
            bm3.Save("C:\\Users\\339-1\\Desktop\\유니폴더\\눈써비\\얼굴인식v3\\kinect-2-face-hd-master - 복사본\\Kinect2FaceHD\\Kinect2FaceHD_NET\\Image\\Login_data\\a4.jpg", ImageFormat.Jpeg);
            bm3.Dispose();


        }

       
       /*     
        public bool Capture()
        {


            // Bitmap screenshot = new System.Drawing.Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight,
            //System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)camera.ActualWidth, (int)camera.ActualHeight, 96d, 96d, PixelFormats.Default);
          
             renderBitmap.Render(camera);
         

          //  BitmapSource screenshot = new System.Drawing.BitmapSource((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight,
             //  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
           // using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(screenshot))
           // {
               // g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
           // }
           
            using (Stream stream = new FileStream("C:\\Users\\339 - 1\\Desktop\\Login_data\\a5.jpg", FileMode.Create, System.IO.FileAccess.Write, FileShare.None))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(stream);
            }

            return true;

        }
        */

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {

            var reference = e.FrameReference.AcquireFrame();
           

            string leftHandState = "-";
            string rightHandState = "-";
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
                                if (v_check == true)
                                {
                                    count++;
                                    if (count == 5)
                                        CopyScreen();
                                    else if (count == 10)
                                        fl.Login();

                                    if (fl.end == false)
                                    {
                                        login_message.Text = "로그인 중";
                                    }
                                    else
                                    {
                                        if (Microsoft.Samples.Kinect.HDFaceBasics.App.check_login == false)
                                            login_message.Text = "로그인 실패";
                                        else
                                        {
                                            login_message.Text = "로그인 성공";
                                            try
                                            {
                                                this.NavigationService.Navigate(new Uri("Pick.xaml", UriKind.Relative));

                                            }
                                            catch (NullReferenceException) { }

                                        }
                                    }
                                }
                              
                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "Open";
                                      //  left_check = true;
                                        //check = true;
                                        break;
                                    case HandState.Closed:
                                        leftHandState = "Closed";
                                        //left_check = false;
                                        //check = true;
                                        break;
                                    case HandState.Lasso:
                                        leftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        //left_check = false;
                                        // leftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                       // left_check = false;
                                        //  leftHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                // LeftHandState.Text = "Left : "+leftHandState;

                                switch (body.HandRightState)
                                {
                                    case HandState.Open:
                                        rightHandState = "Open";
                                       
                                        Microsoft.Samples.Kinect.HDFaceBasics.App.page = 2;
                                        try
                                        {
                                           // this.NavigationService.Navigate(new Uri("UserControl6.xaml", UriKind.Relative));

                                        }
                                        catch (NullReferenceException) { }

                                        //right_check = true;
                                        //check = true;
                                        break;
                                    case HandState.Closed:
                                        rightHandState = "Closed";
                                       // right_check = false;
                                        //check = true;
                                        break;
                                    case HandState.Lasso:
                                        //  CopyScreen();
                                        //  rightHandState = "Lasso";
                                        v_check = true;
                                        break;
                                    case HandState.Unknown:
                                      //  right_check = false;
                                       rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                       // right_check = false;
                                       rightHandState = "Not tracked";
                                        break;

                                    default:
                                        break;
                                }
                                LeftHandState.Text = "Left : " + leftHandState;
                                RightHandState.Text = "Right : " + rightHandState;
                            }
                        }
                    }
                }
            }
        }


    }
}
