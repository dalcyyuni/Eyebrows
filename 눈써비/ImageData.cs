using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect2FaceHD_NET
{
    class ImageData
    {
        string path;
        public string imagePath
        {
            get { return this.path; }
        }

        public void setImagePath(string path)
        {
            this.path = path;
        }

    }
}
