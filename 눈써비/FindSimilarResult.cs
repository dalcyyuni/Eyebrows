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

namespace Kinect2FaceHD_NET
{
    public class FindSimilarResult : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Similar faces collection
        /// </summary>
        private ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face> _faces;

        /// <summary>
        /// Query face
        /// </summary>
        private Microsoft.ProjectOxford.Face.Controls.Face _queryFace;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets similar faces collection
        /// </summary>
        public ObservableCollection<Microsoft.ProjectOxford.Face.Controls.Face> Faces
        {
            get
            {
                return _faces;
            }

            set
            {
                _faces = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Faces"));
                }
            }
        }

        /// <summary>
        /// Gets or sets query face
        /// </summary>
        public Microsoft.ProjectOxford.Face.Controls.Face QueryFace
        {
            get
            {
                return _queryFace;
            }

            set
            {
                _queryFace = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("QueryFace"));
                }
            }
        }

        #endregion Properties
    }

        //#endregion Nested Types
    }

