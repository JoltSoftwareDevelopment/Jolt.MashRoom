using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;

namespace Ignostic.Studio256.RenderApi.Tools
{
    public class TimelineModel
    {
        public List<SceneItem> Scenes { get; private set; }
        public string Path { get; set; }


        public TimelineModel()
        {
            Scenes = new List<SceneItem>();
        }


        public void Load(string path)
        {
            Path = path;
            var serializer = new JavaScriptSerializer();
            var json = File.ReadAllText(path, Encoding.UTF8);
            Scenes = serializer.Deserialize<List<SceneItem>>(json);
        }


        public void Save()
        {
            SaveAs(Path);
        }


        public void SaveAs(string path)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(Scenes);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
