    
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;
    using System.Windows.Shapes;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Runtime.InteropServices;
    using System.Diagnostics;   
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Navigation;

    

namespace Kinect2FaceHD_NET
{
    public partial class MainWindow : Page
    {
        private KinectSensor _sensor = KinectSensor.GetDefault();

        private BodyFrameSource _bodySource = null;

        private BodyFrameReader _bodyReader = null;

        private HighDefinitionFaceFrameSource _faceSource = null;

        private HighDefinitionFaceFrameReader _faceReader = null;

        private FaceAlignment _faceAlignment = null;

        private FaceModel _faceModel = null;

        private List<Ellipse> _points = new List<Ellipse>();


        //
        private double getY;
        private double getX;

        private double getX1;
        private double getY1;
        
        private double width;
        private double height;

        private double pre_getX;
        private double pre_getY;

        private double pre_getX1;
        private double pre_getY1;

        private ColorFrameReader reader = null;
        private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private readonly WriteableBitmap _bmp = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgr32, null);
        Byte[] _frameData = null;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        private Boolean check = true;
        private Boolean left_check = false;
        private Boolean right_check = false;

        private double lefteyebrows_left=0;
        private double lefteyebrows_right = 0;
        private double lefteyebrows_up= 0;
        private double lefteyebrows_down = 0;
        private int nextCounter = 0;

    
  



        public MainWindow()
        {
            this.WindowHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.WindowWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_faceModel != null)
            {
                _faceModel.Dispose();
                _faceModel = null;
            }

            GC.SuppressFinalize(this);
        }


        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {

            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {

                    colorimage.Source = frame.ToBitmap();
                    
                }
            }
            
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

                                // Draw hands and thumbs
                               canvas.DrawHand(handRight, _sensor.CoordinateMapper);
                               canvas.DrawHand(handLeft, _sensor.CoordinateMapper);
                                //canvas.DrawThumb(thumbRight, _sensor.CoordinateMapper);
                                //canvas.DrawThumb(thumbLeft, _sensor.CoordinateMapper);

                                // Find the hand states
                                check = true;
                                string leftHandState = "-";
                                string rightHandState = "-";

                                //왼손
                                CameraSpacePoint vertice1 = handLeft.Position;
                                DepthSpacePoint point_left = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice1);
                            // LeftHandPosition.Text = "LeftHandPosition: X "+point_left.X + " Y "+point_left.Y;
                                /*
                                //button3
                                if (point_left.X > 250 && point_left.X < 310 &&
                                    point_left.Y > 170 && point_left.Y < 220 && left_check == true)
                                {

                                    button3.SetValue(Canvas.WidthProperty, (double)330);
                                    button3.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_up -= 3;
                                }
                                else
                                {
                                    button3.SetValue(Canvas.WidthProperty, (double)291);
                                    button3.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                
                                //button1
                                if (point_left.X > 160 && point_left.X < 220 &&
                                    point_left.Y > 235 && point_left.Y < 260 && left_check == true)
                                {
                                    button1.SetValue(Canvas.WidthProperty, (double)330);
                                    button1.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_left -= 1;
                                }
                                else {
                                    button1.SetValue(Canvas.WidthProperty, (double)291);
                                    button1.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                
                                //button4
                                if (point_left.X > 160 && point_left.X < 220 &&
                                    point_left.Y > 220 && point_left.Y < 270 && left_check == true)
                                {
                                    button4.SetValue(Canvas.WidthProperty, (double)330);
                                    button4.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_down += 1;
                                }
                                else
                                {
                                    button4.SetValue(Canvas.WidthProperty, (double)291);
                                    button4.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                
                                //button2

                                if (point_left.X > 180 && point_left.X < 220 &&
                                    point_left.Y > 180 && point_left.Y < 220 && left_check == true)
                                {
                                    button2.SetValue(Canvas.WidthProperty, (double)330);
                                    button2.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_right += 1;
                                }
                                else
                                {
                                    button2.SetValue(Canvas.WidthProperty, (double)291);
                                    button2.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                */
                                //오른손
                                CameraSpacePoint vertice2 = handRight.Position;
                                DepthSpacePoint point_right = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice2);
                                RightHandPosition.Text = "RightHandPosition: X "+point_right.X + " Y "+point_right.Y;//

                                if (point_right.X > 250 && point_right.X < 310 && right_check == true)
                                {

                                    if (point_right.Y > 180 && point_right.Y < 220)
                                    {
                                        button3.SetValue(Canvas.WidthProperty, (double)330);
                                        button3.SetValue(Canvas.HeightProperty, (double)200);
                                        lefteyebrows_up -= 1;

                                        button1.SetValue(Canvas.WidthProperty, (double)291);
                                        button1.SetValue(Canvas.HeightProperty, (double)168);
                                        button2.SetValue(Canvas.WidthProperty, (double)291);
                                        button2.SetValue(Canvas.HeightProperty, (double)168);
                                        button4.SetValue(Canvas.WidthProperty, (double)291);
                                        button4.SetValue(Canvas.HeightProperty, (double)168);

                                    }
                                    else if (point_right.Y > 280 && point_right.Y < 300)
                                    {
                                        button1.SetValue(Canvas.WidthProperty, (double)330);
                                        button1.SetValue(Canvas.HeightProperty, (double)200);
                                        lefteyebrows_left -= 1;



                                        button3.SetValue(Canvas.WidthProperty, (double)291);
                                        button3.SetValue(Canvas.HeightProperty, (double)168);
                                        button2.SetValue(Canvas.WidthProperty, (double)291);
                                        button2.SetValue(Canvas.HeightProperty, (double)168);
                                        button4.SetValue(Canvas.WidthProperty, (double)291);
                                        button4.SetValue(Canvas.HeightProperty, (double)168);
                                    }
                                    else if (point_right.Y > 225 && point_right.Y < 250)
                                    {
                                        button4.SetValue(Canvas.WidthProperty, (double)330);
                                        button4.SetValue(Canvas.HeightProperty, (double)200);
                                        lefteyebrows_down += 1;


                                        button1.SetValue(Canvas.WidthProperty, (double)291);
                                        button1.SetValue(Canvas.HeightProperty, (double)168);
                                        button2.SetValue(Canvas.WidthProperty, (double)291);
                                        button2.SetValue(Canvas.HeightProperty, (double)168);
                                        button3.SetValue(Canvas.WidthProperty, (double)291);
                                        button3.SetValue(Canvas.HeightProperty, (double)168);
                                    }
                                    else if (point_right.Y > 255 && point_right.Y < 275)
                                    {
                                        button2.SetValue(Canvas.WidthProperty, (double)330);
                                        button2.SetValue(Canvas.HeightProperty, (double)200);
                                        lefteyebrows_right += 1;


                                        button1.SetValue(Canvas.WidthProperty, (double)291);
                                        button1.SetValue(Canvas.HeightProperty, (double)168);
                                        button3.SetValue(Canvas.WidthProperty, (double)291);
                                        button3.SetValue(Canvas.HeightProperty, (double)168);
                                        button4.SetValue(Canvas.WidthProperty, (double)291);
                                        button4.SetValue(Canvas.HeightProperty, (double)168);
                                    }

                                }
                                else if (right_check == false)
                                {
                                    button1.SetValue(Canvas.WidthProperty, (double)291);
                                    button1.SetValue(Canvas.HeightProperty, (double)168);
                                    button3.SetValue(Canvas.WidthProperty, (double)291);
                                    button3.SetValue(Canvas.HeightProperty, (double)168);
                                    button4.SetValue(Canvas.WidthProperty, (double)291);
                                    button4.SetValue(Canvas.HeightProperty, (double)168);
                                    button2.SetValue(Canvas.WidthProperty, (double)291);
                                    button2.SetValue(Canvas.HeightProperty, (double)168);
                                }
                            
                                /*
                                //button3
                                  if (point_right.X > 250 && point_right.X < 310 &&
                                    point_right.Y > 180 && point_right.Y < 220 && right_check == true)
                                {

                                    button3.SetValue(Canvas.WidthProperty, (double)330);
                                    button3.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_up -= 1;
                                }
                                else
                                {
                                    button3.SetValue(Canvas.WidthProperty, (double)291);
                                    button3.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                
                             // button1
                              if (point_right.X > 250 && point_right.X < 310 &&
                                  point_right.Y > 280 && point_right.Y < 300 && right_check == true)
                              {
                                  button1.SetValue(Canvas.WidthProperty, (double)330);
                                  button1.SetValue(Canvas.HeightProperty, (double)200);
                                  lefteyebrows_left -= 1;
                              }
                              else {
                                  button1.SetValue(Canvas.WidthProperty, (double)291);
                                  button1.SetValue(Canvas.HeightProperty, (double)168);
                              }
                              
                                //button4
                                if (point_right.X > 270 && point_right.X < 310 &&
                                    point_right.Y > 225 && point_right.Y < 250 && right_check == true)
                                {
                                    button4.SetValue(Canvas.WidthProperty, (double)330);
                                    button4.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_down += 1;
                                }
                                else
                                {
                                    button4.SetValue(Canvas.WidthProperty, (double)291);
                                    button4.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                
                                //button2

                                if (point_right.X > 250 && point_right.X < 310 &&
                                    point_right.Y > 255 && point_right.Y < 275 && right_check == true)
                                {
                                    button2.SetValue(Canvas.WidthProperty, (double)330);
                                    button2.SetValue(Canvas.HeightProperty, (double)200);
                                    lefteyebrows_right += 1;
                                }
                                else
                                {
                                    button2.SetValue(Canvas.WidthProperty, (double)291);
                                    button2.SetValue(Canvas.HeightProperty, (double)168);
                                }
                                */

                                //next
                                if (point_right.X > 250 && point_right.X<300 
                                    && point_right.Y > 100 && point_right.Y<180)
                                {
                                    Microsoft.Samples.Kinect.HDFaceBasics.App.left_down = lefteyebrows_down;
                                    Microsoft.Samples.Kinect.HDFaceBasics.App.left_up = lefteyebrows_up;
                                    Microsoft.Samples.Kinect.HDFaceBasics.App.left_left = lefteyebrows_left;
                                    Microsoft.Samples.Kinect.HDFaceBasics.App.left_right = lefteyebrows_right;

                                    button_next.SetValue(Canvas.WidthProperty, (double)430);
                                    button_next.SetValue(Canvas.HeightProperty, (double)400);
                                    nextCounter++;

                                    if (nextCounter == 10)
                                    {
                                        try
                                        {
                                            this.NavigationService.Navigate(new Uri("UserControl1.xaml", UriKind.Relative));

                                        }
                                        catch (NullReferenceException) { }
                                    }

                                }
                                
                                
                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "Open";
                                        left_check = true;
                                        //check = true;
                                        break;
                                    case HandState.Closed:
                                        leftHandState = "Closed";
                                        left_check = false;
                                        //check = true;
                                        break;
                                    case HandState.Lasso:
                                        leftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        left_check = false;
                                        // leftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        left_check = false;
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
                                        right_check = true;
                                        //check = true;
                                        break;
                                    case HandState.Closed:
                                        rightHandState = "Closed";
                                        right_check = false;
                                        //check = true;
                                        break;
                                    case HandState.Lasso:
                                        rightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        right_check = false;
                                        // rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        right_check = false;
                                        //  rightHandState = "Not tracked";
                                        break;

                                    default:
                                        break;
                                }
                                LeftHandState.Text = "Left : " + leftHandState;
                                RightHandState.Text = "Right : "+rightHandState;
                               // hi.Text = leftHandState;
                            }
                        }
                    }
                }
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
                    UpdateFacePoints();
                }
            }

            
        }


         private void ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference;
            try
            {
                var frame = reference.AcquireFrame();
                if (frame == null) return;
                using (frame)
                {
                    FrameDescription desc = frame.FrameDescription;
                    var size = desc.Width * desc.Height;

                    if (_frameData == null)
                    {
                      //  _bmp = new WriteableBitmap(desc.Width, desc.Height, 96, 96, PixelFormats.Bgr32, null);
                        _frameData = new byte[size * bytesPerPixel];
                  
                    }

                    frame.CopyConvertedFrameDataToArray(_frameData,ColorImageFormat.Bgra);
                    _bmp.WritePixels(new Int32Rect(0,0,desc.Width,desc.Height),
                        _frameData,
                        desc.Width * bytesPerPixel,
                        0);
                }

                colorimage.Source = _bmp;
            }
            catch { }
        }




        private void UpdateFacePoints()
        {
            if (_faceModel == null) return;

            var vertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);

            if (vertices.Count > 0)
            {
                if (_points.Count == 0)
                {
                    for (int index = 0; index < vertices.Count; index++)
                    {
                        Ellipse ellipse = new Ellipse
                        {
                            Width = 5.0,
                            Height = 5.0,
                            Fill = new SolidColorBrush(Colors.Blue)
                        };

                        _points.Add(ellipse);
                    }

                  //  foreach (Ellipse ellipse in _points)
                   // {
                     //  canvas.Children.Add(ellipse);
                    //}
                }

                for (int index = 0; index < vertices.Count; index++)
                {
                    CameraSpacePoint vertice = vertices[index];
                    //Debug.WriteLine(vertice.X+" "+vertice.Y+" "+vertice.Z);
                    DepthSpacePoint point = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(vertice);

                    if (float.IsInfinity(point.X) || float.IsInfinity(point.Y)) return;

                    Ellipse ellipse = _points[index];
                    point.X = point.X * 10-1937;
                    point.Y = point.Y * 10-1634;
    
                   Canvas.SetLeft(ellipse, point.X);
                   Canvas.SetTop(ellipse, point.Y);

                    if (left.Source != null) {
                                  
                        if (index == 200 && check==true)
                        {

                          
                          
                         getY = Math.Round(point.Y-132)+lefteyebrows_up+lefteyebrows_down;
                         getX = Math.Round(point.X-50)+lefteyebrows_left+lefteyebrows_right;

                            if (Math.Abs(getX - pre_getX) > 6 || Math.Abs(getY - pre_getY) > 6)
                            {
                                left.SetValue(Canvas.TopProperty, getY);
                                left.SetValue(Canvas.LeftProperty, getX);
                                getX1 = getX;
                                getY1 = getY;
                                pre_getX = getX;
                                pre_getY = getY;
                            }
                        
                        }

                        if (index == 804 && check==true)
                        {
                            getY = Math.Round(point.Y-120);
                            getX = Math.Round(point.X-50);
                            if (Math.Abs(getX - pre_getX1) > 6 || Math.Abs(getY - pre_getY1) > 6)
                            {
                                right.SetValue(Canvas.TopProperty, getY);
                                right.SetValue(Canvas.LeftProperty, getX);
                                pre_getX1 = getX;
                                pre_getY1 = getY;
                            }
                        }

                        if (index == 345 && check == true)
                        {
                            width =  Math.Abs(getX - getX1)+150
                                
                                
                                
                                
                                ;
                            height =  Math.Abs(getY - getY1)+150;
                            if (width > 100 && height > 30)
                            {
                                double pre_width = right.Width;
                                double pre_height = right.Height;
                                if (Math.Abs(pre_width - width) > 3 || Math.Abs(pre_height - height) > 3)
                                {
                                    right.SetValue(Canvas.WidthProperty, width);
                                    right.SetValue(Canvas.HeightProperty, height);
                                    left.SetValue(Canvas.WidthProperty, width-2);
                                    left.SetValue(Canvas.HeightProperty, height);
                                }
                            }
                        }

                   }
                }

                int sizeInBytes = Marshal.SizeOf(typeof(PointF));
                IntPtr pointsPtr = IntPtr.Zero;
               

            }
        }


      


    }
}