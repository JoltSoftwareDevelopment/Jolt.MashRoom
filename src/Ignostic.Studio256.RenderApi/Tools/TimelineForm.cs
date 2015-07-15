using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ignostic.Studio256.RenderApi.Tools
{
    public partial class TimelineForm : Form
    {
        public TimelineForm()
        {
            InitializeComponent();
        }


        public TimelineModel Model
        {
            get { return timelineControl1.Model; }
        }
    }
}
