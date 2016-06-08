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
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace Kinect2FaceHD_NET
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
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



        public MainPage()
        {
            InitializeComponent();
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
                                        //rightHandState = "Open";
                                        Microsoft.Samples.Kinect.HDFaceBasics.App.page = 2;
                                       // try
                                      //  {
                                          //  this.NavigationService.Navigate(new Uri("UserControl6.xaml", UriKind.Relative));

                                        //}
                                       // catch (NullReferenceException) { }

                                        //right_check = true;
                                        //check = true;
                                        break;
                                    case HandState.Closed:
                                        //rightHandState = "Closed";
                                        // right_check = false;
                                        //check = true;
                                        break;
                                    case HandState.Lasso:
                                        //  rightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        //  right_check = false;
                                      //  rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        // right_check = false;
                                       // rightHandState = "Not tracked";
                                        break;

                                    default:
                                        break;
                                }
                                LeftHandState.Text = "Left : " + leftHandState;
                                RightHandState.Text = "Right : " + rightHandState;

                                CameraSpacePoint vertice2 = handRight.Position;
                                DepthSpacePoint point_right = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice2);
                                RightHandPosition.Text = "RightHandPosition: X " + point_right.X + " Y " + point_right.Y;//
                                if (point_right.X > 200 && point_right.X < 250)
                                {
                                    if (point_right.Y > 290 && point_right.Y < 320)
                                    {
                                       
                                        //button_login.SetValue(Canvas.WidthProperty, (double)320);
                                        //button_login.SetValue(Canvas.HeightProperty, (double)320);
                                       
                                        BlurEffect be = new BlurEffect();
                                        be.Radius = 0;
                                        button_login.SetValue(Image.EffectProperty, be);
                                        //be.Radius = 20;
                                        //button_register.SetValue(Image.EffectProperty, be);
                                        //    button_login.SetValue(Image.EffectProperty.)
                                        //button_register.SetValue(Canvas.WidthProperty, (double)300);
                                        //button_register.SetValue(Canvas.HeightProperty, (double)300);
                                    }

                                    else if (point_right.Y >230 && point_right.Y < 300)
                                    {
                                        // button_register.SetValue(Canvas.WidthProperty, (double)320);
                                        // button_register.SetValue(Canvas.HeightProperty, (double)320);
                                       // BlurEffect be = new BlurEffect();
                                        //be.Radius = 0;
                                        //button_register.SetValue(Image.EffectProperty, be);
                                        //be.Radius = 20;
                                        //button_login.SetValue(Image.EffectProperty, be);
                                        // button_register.BitmapEffect.SetValue(BitmapEffect.bl);
                                        //= myBlurEffect;
                                        // button_login.SetValue(Canvas.WidthProperty, (double)300);
                                        // button_login.SetValue(Canvas.HeightProperty, (double)300);
                                    }

                                }
                                else
                                {

                                    BlurEffect be = new BlurEffect();
                                    be.Radius = 20;
                                    button_login.SetValue(Image.EffectProperty, be);
                                    button_register.SetValue(Image.EffectProperty, be);

                                }
                                CameraSpacePoint vertice1 = handLeft.Position;
                                DepthSpacePoint point_left = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice1);
                                LeftHandPosition.Text = "LeftHandPosition: X "+point_left.X + " Y "+point_left.Y;
                            }
                        }
                    }
                }
            }
        }
    }
}
