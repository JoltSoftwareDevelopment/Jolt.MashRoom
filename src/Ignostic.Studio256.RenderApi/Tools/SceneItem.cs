using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Studio256.RenderApi.Tools
{
    [Serializable]
    public class SceneItem
    {
        public string Name { get; set; }
        public double TimeStart { get; set; }
        public double Duration { get; set; }
        public int RowIndex { get; set; }
    }
}
