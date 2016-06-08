using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ClientContract = Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Controls;
using System.Net;

namespace Kinect2FaceHD_NET
{


    class FaceLogin
    {

        private string subscriptionKey = "fe54322d85e14ae1be47080d1fd1af32";

        private ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face> _leftResultCollection = new ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face>();

        private ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face> _rightResultCollection = new ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face>();
        private string _verifyResult;

        public event PropertyChangedEventHandler PropertyChanged;

        public double result = 0;
        public int people_num = 5;
        public Boolean end = false;
        public Boolean hasFace = false;

        public ObservableCollection<Face> LeftResultCollection
        {
            get
            {
                return _leftResultCollection;
            }
        }

        /// <summary>
        /// Gets max image size for UI rendering
        /// </summary>
        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        /// <summary>
        /// Gets face detection results for image on the right
        /// </summary>
        public ObservableCollection<Face> RightResultCollection
        {
            get
            {
                return _rightResultCollection;
            }
        }

        /// <summary>
        /// Gets or sets selected face verification result
        /// </summary>
        public string VerifyResult
        {
            get
            {
                return _verifyResult;
            }

            set
            {
                _verifyResult = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VerifyResult"));
                }
            }
        }


        public void getNumber()
        {

            HttpWebRequest wRep;
            HttpWebResponse wRes;
            Uri uri;
            string cookie = "";
            string resResult = "";
            try
            {
                uri = new Uri("http://203.252.218.16:8081/test/count.jsp");
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
                    if (resResult != null)
                    {
                        Int32 number = Int32.Parse(resResult);
                        people_num = number;
                        Debug.WriteLine("--------------");
                        //Debug.WriteLine(resResult);
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
        public async void HasFace(int i)
        {
            var path = "C:\\Users\\339-1\\Desktop\\유니폴더\\눈써비\\얼굴인식v3\\kinect-2-face-hd-master - 복사본\\Kinect2FaceHD\\Kinect2FaceHD_NET\\Image\\Login_data\\" + i + ".jpg";
            using (var fileStream = File.OpenRead(path))
            {
                try
                {


                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Handle REST API calling error
                    if (faces == null)
                    {
                        //return false;
                        hasFace = false;
                    }
                    hasFace = true;
                   // return true;

                }
                catch (FaceAPIException ex)
                {
                    // MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    //return false;
                }

            }
        }
        public async void Login()
        {


            VerifyResult = string.Empty;

            // User already picked one image

            var path = "C:\\Users\\339-1\\Desktop\\유니폴더\\눈써비\\얼굴인식v3\\kinect-2-face-hd-master - 복사본\\Kinect2FaceHD\\Kinect2FaceHD_NET\\Image\\Login_data\\a4.jpg";
            var imageInfo = Microsoft.ProjectOxford.Face.Controls.UIHelper.GetImageInfoForRendering(path);
            // Clear last time detection results
            LeftResultCollection.Clear();

            //MainWindow.Log("Request: Detecting in {0}", pickedImagePath);
            var sw = Stopwatch.StartNew();
            // Debug.WriteLine(pickedImagePath);
            //  var path = "C:\\Users\\339-1\\Desktop\\ProjectOxford-ClientSDK-master\\ProjectOxford-ClientSDK-master\\Face\\Windows\\Data\\detection1.jpg";
            // path = "C:Users\339-1\Desktop\ProjectOxford-ClientSDK-master\ProjectOxford-ClientSDK-master\Face\Windows\Data\detection1.jpg";
            // Call detection REST API, detect faces inside the image
            using (var fileStream = File.OpenRead(path))
            {
                try
                {


                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Handle REST API calling error
                    if (faces == null)
                    {
                        return;
                    }



                    // Convert detection results into UI binding object for rendering
                    foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        // Detected faces are hosted in result container, will be used in the verification later
                        LeftResultCollection.Add(face);
                    }
                }
                catch (FaceAPIException ex)
                {
                    // MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    return;
                }
            }


            ////////////////////////////


            for (int i = 1; i < people_num; i++)
            {

              
                var pickedImagePath = "C:\\Users\\339-1\\Desktop\\유니폴더\\눈써비\\얼굴인식v3\\kinect-2-face-hd-master - 복사본\\Kinect2FaceHD\\Kinect2FaceHD_NET\\Image\\Login_data\\" + i + ".jpg";

                imageInfo = Microsoft.ProjectOxford.Face.Controls.UIHelper.GetImageInfoForRendering(pickedImagePath);
              
                RightResultCollection.Clear();

                sw = Stopwatch.StartNew();

              
                using (var fileStream = File.OpenRead(pickedImagePath))
                {
                    try
                    {


                        var faceServiceClient = new FaceServiceClient(subscriptionKey);

                        var faces = await faceServiceClient.DetectAsync(fileStream);

                      
                        if (faces == null)
                        {
                            return;
                        }



                        
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                        {
                         
                            RightResultCollection.Add(face);
                        }
                    }
                    catch (FaceAPIException ex)
                    {

                        return;
                    }


                }


                if (LeftResultCollection.Count == 1 && RightResultCollection.Count == 1)
                {
                    VerifyResult = "Verifying...";
                    var faceId1 = LeftResultCollection[0].FaceId;
                    var faceId2 = RightResultCollection[0].FaceId;
                    //Debug.WriteLine("in");


                    // Call verify REST API with two face id
                    try
                    {

                       // Debug.WriteLine(faceId1.ToString());

                        var faceServiceClient = new FaceServiceClient(subscriptionKey);
                        var res = await faceServiceClient.VerifyAsync(Guid.Parse(faceId1), Guid.Parse(faceId2));


                       // Debug.WriteLine(faceId1.ToString());
                        VerifyResult = string.Format("{0} ({1:0.0})", res.IsIdentical ? "Equals" : "Does not equal", res.Confidence);
                        result = res.Confidence;
                        //Debug.WriteLine("result " + result);
                        if (result > 0.5)
                        {
                            Microsoft.Samples.Kinect.HDFaceBasics.App.check_login = true;
                            Microsoft.Samples.Kinect.HDFaceBasics.App.id_number = i;
                            end = true;
                            break;
                        }

                        Debug.WriteLine(VerifyResult);



                    }
                    catch (FaceAPIException ex)
                    {


                        return;
                    }
                }
                else
                {



                }


                if (i == people_num - 1)
                    end = true; 
            }





        }

    }

    }
