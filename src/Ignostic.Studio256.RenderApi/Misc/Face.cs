using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Ignostic.Studio256.RenderApi
{
    public class Face
    {
        public List<int> Indices { get; set; }
        public Vector4 Color { get; set; }
        public Vector3 Normal { get; set; }

        
        public Face(IEnumerable<int> indices)
        {
            Indices = new List<int>(indices);
        }

        
        public Face(params int[] indices)
            : this((IEnumerable<int>)indices)
        {
        }


        public Vector3 CalculateNormal(Model model)
        {
            //var vertices = Indices.Select(i => model.Positions[i]).ToArray();
            var u = model.Positions[Indices[1]] - model.Positions[Indices[0]];
            var v = model.Positions[Indices[2]] - model.Positions[Indices[0]];
            Normal = Vector3.Normalize(Vector3.Cross(u, v));
            return Normal;
        }

    }
}
