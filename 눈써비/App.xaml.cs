//------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.HDFaceBasics
{
    using Kinect2FaceHD_NET;
    using System.Diagnostics;
    using System.Windows;
    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class App : Application
    {

        public static int facenumber = 0;
        //왼쪽 눈썹의 조절
        public static double left_up = 0;
        public static double left_down = 0;
        public static double left_left = 0;
        public static double left_right = 0;
        public static double left_width = 0;
        public static double left_height = 0;

        //오른쪽 눈썹의 조절
        public static double right_up = 0;
        public static double right_down = 0;
        public static double right_left = 0;
        public static double right_right = 0;
        public static double right_width = 0;
        public static double right_height = 0;
            
        public static bool check_login = false;
        public static int page = 0;
        public static int id_number = 0;

        void App_Startup(object sender, StartupEventArgs e)
        {          
        }
       

        void App_Exit(object sender, ExitEventArgs e)
        {
         
           Debug.WriteLine("간다~");
           
        }


        }
    }
