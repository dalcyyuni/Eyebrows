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
using System.Net;

namespace Kinect2FaceHD_NET
{
    /// <summary>
    /// Interaction logic for ButtonPick.xaml
    /// </summary>
    /// 


    public partial class ButtonPick : Page
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
        String imgname;
        String[] imgnames;
        ImageData imagedata = new ImageData();
        int id_number = 0;

        public ButtonPick()
        {
            id_number = Microsoft.Samples.Kinect.HDFaceBasics.App.id_number;
            this.WindowHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.WindowWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            //getName();
            // downloadImage();
            //for (int i = 0; i < 4; i++)
            //  makeMenu(i);
            String stringPath = "C:/Users/339-1/Desktop/menu1.png";
           // Uri imageUri = new Uri(stringPath, UriKind.Absolute);
           // BitmapImage imageBitmap = new BitmapImage(imageUri);
            // imageBitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //button1.Source = imageBitmap;
            //  Debug.WriteLine(button1.Source);

            // BitmapImage b = new BitmapImage();
            // b.BeginInit();
            // b.UriSource = new Uri("C:/Users/339-1/Desktop/menu/Image/menu3.png");
            //b.EndInit();

            // ... Get Image reference from sender.

            // ... Assign Source.
            //  button1.Source = b;
        }





        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


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

        private void makeMenu(int i)
        {
            String stringPath = "C:/Users/339-1/Desktop/menu1.png";
            Uri imageUri = new Uri(stringPath, UriKind.Absolute);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            switch (i)
            {

                case 0:
                    stringPath = "C:/Users/339-1/Desktop/menu/menu1.png";
                    imageUri = new Uri(stringPath, UriKind.Absolute);
                    imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    // myImage.Source = imageBitmap;
                    if (imageBitmap != null)
                        button1.Source = imageBitmap;
                    break;

                case 1:
                    stringPath = "C:/Users/339-1/Desktop/menu/menu2.png";
                    imageUri = new Uri(stringPath, UriKind.Absolute);
                    imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    // myImage.Source = imageBitmap;
                    if (imageBitmap != null)
                        button2.Source = imageBitmap;
                    break;

                case 2:
                    stringPath = "C:/Users/339-1/Desktop/menu/menu3.png";
                    imageUri = new Uri(stringPath, UriKind.Absolute);
                    imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    // myImage.Source = imageBitmap;
                    button3.Source = imageBitmap;

                    break;
                case 3:

                    stringPath = "C:/Users/339-1/Desktop/menu/menu4.png";
                    imageUri = new Uri(stringPath, UriKind.Absolute);
                    imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    // myImage.Source = imageBitmap;
                    if (imageBitmap != null)
                        button4.Source = imageBitmap;
                    break;
            }
        }

        public bool BitmapFromURL(string uri, string fileName)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            bool bImage = response.ContentType.StartsWith("image",
                StringComparison.OrdinalIgnoreCase);

            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                bImage)
            {
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {

                    byte[] buffer = new byte[4096];
                    int bytesRead = 10;
                    while (true)
                    {
                        if (bytesRead > 0)
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                            Debug.WriteLine(bytesRead);


                        }
                        else
                        {
                            Debug.WriteLine("here");

                            outputStream.Close();
                            inputStream.Close();

                            // inputStream.Dispose();
                            // outputStream.Dispose();
                            break;
                        }
                        //outputStream.Dispose();

                    }

                    // inputStream.Dispose();
                    // outputStream.Dispose();





                }



                return true;
            }
            else
            {
                return false;
            }



        }

        private void downloadImage()
        {
            getName();



            string path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));


            imgname = imgname.Trim();
            imgnames = imgname.Split(' ');

            Debug.WriteLine(imgnames.Length);
            for (int i = 0; i < imgnames.Length; i++)
            {
                Debug.WriteLine(imgnames[i]);
                //// if (!imgnames[i].Equals(""))
                //{
                String url = "http://203.252.218.16:8081/test/img/" + imgnames[i];
                Debug.WriteLine(url);
                //String fileName = "C:\\Users\\339-1\\Desktop\\eyebrow\\" +imgnames[i];
                String fileName = "C:/Users/339-1/Desktop/menu/menu" + (i + 1) + ".png";
                Debug.WriteLine(fileName);
                if (!BitmapFromURL(url, fileName))
                {
                    MessageBox.Show("Download Failed: " + url);
                }
                // }
            }

        }


        private void getName()
        {
            HttpWebRequest wRep;
            HttpWebResponse wRes;
            Uri uri;
            string cookie = "";
            string resResult = "";
            try
            {
                uri = new Uri("http://203.252.218.16:8081/test/imgloadProcess.jsp?id_number=3&flag=1");
                wRep = (HttpWebRequest)WebRequest.Create(uri);
                wRep.Method = "GET";
                wRep.ServicePoint.Expect100Continue = false;
                wRep.CookieContainer = new CookieContainer();
                wRep.CookieContainer.SetCookies(uri, cookie);

                using (wRes = (HttpWebResponse)wRep.GetResponse())
                {
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);
                    resResult = readerPost.ReadToEnd();
                    Debug.WriteLine(resResult);
                    if (resResult != null)
                    {
                        imgname = resResult;
                        Debug.WriteLine("--------------");
                        Debug.WriteLine(resResult);
                        Debug.WriteLine(resResult);
                        Debug.WriteLine("--------------");
                    }
                    //if (resResult != null)
                    //{
                    //Int32 number = Int32.Parse(resResult);
                    //return number;
                    //}




                }
            }
            catch (WebException ex) { }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

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
