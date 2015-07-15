using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using SharpDX;

namespace Ignostic.Studio256.RenderApi
{
    public class ModelManager : AssetManager<Model>
    {
        public ModelManager ()
        {
            // todo
            RootPath = @"resources\models";
        }

        public override Model Load(string name)
        {
            var models = GetList(name);
            if (models.Count > 0)
                return models.First();

            var path = string.Format(@"{0}\{1}.obj", RootPath, name);
            var serializer = new WaveFrontSerializer();
            var model = Model.Join(name, serializer.Deserialize(path));
            model.Color = new Color(128, 128, 128, 255);
            
            Add(model);
            return model;
        }

        public void LoadAll()
        {
            var names = Directory
                .GetFiles(RootPath, "*.obj")
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToArray();
            foreach (var name in names)
            {
                Load(name);
            }
        }

        public Model Clone(string name, string newName="")
        {
            var model = Load(name).Clone(newName);
            Add(model);
            return model;
        }

    }
}
