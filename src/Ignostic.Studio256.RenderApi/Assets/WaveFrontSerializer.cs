using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX;
using System.Globalization;

namespace Ignostic.Studio256.RenderApi
{
    public class WaveFrontSerializer
    {
        public List<Model> Deserialize(string path)
        {
            var models = new List<Model>();
            var model = null as Model;
            var culture = CultureInfo.InvariantCulture;
            var indexOffset = 0;
            foreach (var line in File.ReadAllLines(path, Encoding.ASCII))
            {
                var split = line.Split(' ');
                var prefix = split[0];
                var args = split.Skip(1);
                switch (prefix)
                {
                    case "o":
                        models.Add(model = new Model());
                        indexOffset = models.SelectMany(m => m.Positions).Count();
                        break;
                    case "v":
                        model.Positions.Add(new Vector3(args.Select(s => float.Parse(s, culture)).ToArray()));
                        break;
                    case "f":
                        model.Faces.Add(new Face(args.Select(s => int.Parse(s, culture) - indexOffset - 1).ToArray()));
                        break;
                    case "vt":
                        model.TexCoord0.Add(new Vector4(args.Select(s => float.Parse(s, culture)).Concat(new[] { 0F, 0F }).ToArray()));
                        break;
                }
            }
            return models;
        }
    }
}
