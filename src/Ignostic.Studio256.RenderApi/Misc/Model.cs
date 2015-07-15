using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using System.IO;
using Jolt;

namespace Ignostic.Studio256.RenderApi
{
    public class Model : IAsset
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Buffer _buffer;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Model()
        {
            Matrix = Matrix.Identity;
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
            Faces = new List<Face>();
            Colors = new List<Vector4>();
            TexCoord0 = new List<Vector4>();
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public string Name { get; set; }
        public Matrix Matrix { get; set; }
        public List<Vector3> Positions { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Face> Faces { get; set; }
        public List<Vector4> Colors { get; set; }
        public List<Vector4> TexCoord0 { get; set; }
        public Buffer Buffer { get { return _buffer; } }
        public Color Color
        {
            set
            {
                foreach (var face in Faces)
                {
                    face.Color = value.ToVector4();
                }
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        // naive and far from optimal implementation
        public void SplitFaces()
        {
            var splitfaces = new List<Face>();
            foreach (var face in Faces)
            {
                var indices = face.Indices;
                switch (indices.Count)
                {
                    case 3:
                        splitfaces.Add(face);
                        break;
                    case 4:
                        splitfaces.Add(new Face(indices[0], indices[1], indices[3])
                        {
                            Color = face.Color
                        });
                        splitfaces.Add(new Face(indices[3], indices[1], indices[2])
                        {
                            Color = face.Color
                        });
                        break;
                    // naive implementation for convex faces
                    default:
                        var newVertex = indices.Select(i => Positions[i]).Sum();
                        var newIndex = Positions.Count;
                        Positions.Add(newVertex);
                        splitfaces = Enumerable
                            .Range(0, indices.Count-1)
                            .Select(i => new Face(indices[i], indices[i+1], newIndex)
                            {
                                Color = face.Color
                            })
                            .ToList();
                        break;
                }
            }
            Faces = splitfaces;
        }

        public void SubDivideFaces3()
        {
            Faces = Faces
                .SelectMany(face => 
                {
                    var indices = face.Indices;
                    var newIndex = Positions.Count;

                    var newVertex = 8F * indices
                        .Select(i => Positions[i])
                        .Average();

                    Positions.Add(newVertex);
                    indices.Add(newIndex);

                    return new[]
                    {
                        new Face(indices[0], indices[1], indices[3]) { Color = face.Color },
                        new Face(indices[1], indices[2], indices[3]) { Color = face.Color },
                        new Face(indices[2], indices[0], indices[3]) { Color = face.Color },
                    };
                })
                .ToList();
        }

        public void SubDivideFaces4()
        {
            Faces = Faces
                .SelectMany(face =>
                {

                    var indices = face.Indices;
                    var v = indices
                        .Select(i => Positions[i])
                        .ToArray();
                    Positions.AddRange(new[]
                    {
                        1 * 0.55F * (v[0] + v[1]),
                        1 * 0.55F * (v[1] + v[2]),
                        1 * 0.55F * (v[2] + v[0]),
                    });
                    indices.Add(Positions.Count - 3);
                    indices.Add(Positions.Count - 2);
                    indices.Add(Positions.Count - 1);

                    //     0
                    //   5   3
                    // 2   4   1
                    return new[]
                    {
                        new Face(indices[0], indices[3], indices[5]) { Color = face.Color },
                        new Face(indices[3], indices[1], indices[4]) { Color = face.Color },
                        new Face(indices[4], indices[2], indices[5]) { Color = face.Color },
                        new Face(indices[5], indices[3], indices[4]) { Color = face.Color },
                    };
                })
                .ToList();
        }

        public void RemoveFaces()
        {
            // normals
            if (Normals.Count == 0)
            {
                CalculateNormals(0);
            }
            else
            {
                Normals = Faces
                    .SelectMany(face => face.Indices)
                    .Select(i => Normals[i])
                    .ToList();
            }

            // positions
            Positions = Faces
                .SelectMany(face => face.Indices)
                .Select(i => Positions[i])
                .ToList();

            // colors
            Colors = Faces
                .Select(face => face.Color)
                .SelectMany(c => new [] { c, c, c })
                .ToList();

            // texcoord 0
            if (TexCoord0.Count == 0)
            {
                TexCoord0 = new List<Vector4>(3 * Faces.Count);
            }
            else
            {
                TexCoord0 = Faces
                    .SelectMany(face => face.Indices)
                    .Select(i => TexCoord0[i])
                    .ToList();
            }
            Faces.Clear();
        }

        public void ReCreateBuffer(Device device)
        {
            Disposer.Dispose(ref _buffer);

            SplitFaces();
            RemoveFaces();
            var vertices = Enumerable
                .Range(0, Positions.Count)
                .Select(i => new Vertex
                {
                    Position = new Vector4(Positions[i], 1),
                    Normal = new Vector4(Normals[i], 0),
                    Color = new Color4(Colors[i]),
                    Custom0 = TexCoord0.Count == 0 ? Vector4.Zero : TexCoord0[i],
                }).ToArray();
            _buffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
        }


        //public void Draw(DeviceContext context, ShaderEnvironment environment)
        //{
        //    var modelBuffer = Buffer;
        //    if (modelBuffer == null)
        //        return;

        //    environment.World = Matrix;
        //    environment.UpdateBuffer(context);
            
        //    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(modelBuffer, Vertex.Size, 0));
        //    context.Draw(Vertices.Count, 0);
        //}


        public static Model Join(string name, IEnumerable<Model> models)
        {
            var newModel = new Model
            {
                Name = name,
            };
            foreach (var model in models)
            {
                newModel.Faces.AddRange(model
                    .Faces
                    .Select(face => new Face(face
                        .Indices
                        .Select(i => i + newModel.Positions.Count)
                        .ToArray())
                        {
                            Color = face.Color
                        }));
                newModel.Positions.AddRange(model.Positions.Select(v => Vector3.TransformCoordinate(v, model.Matrix)));
                newModel.Normals.AddRange(model.Normals.Select(n => Vector3.TransformNormal(n, model.Matrix)));
                newModel.Colors.AddRange(model.Colors);
                newModel.TexCoord0.AddRange(model.TexCoord0);
                newModel.Normals = model.Normals;
            }
            return newModel;
        }


        //public void RoundEdges(float radius, int lod)
        //{
        //    var inset = 0.1F;
        //    var cornerNormals = Enumerable
        //        .Range(0, Vertices.Count)
        //        .Select(i => Faces
        //            .Where(face => face
        //                .Indices
        //                .Select(j => Vertices[j])
        //                .Contains(Vertices[i]))
        //            .Select(face => Faces.IndexOf(face))
        //            .Distinct()
        //            .Select(j => CalculateNormal(j))
        //            .Average())
        //        .ToArray();
                    
        //    throw new NotImplementedException();



        //    //var allVertices = Faces
        //    //    .SelectMany(face => face.Indices)
        //    //    .Select(i => Vertices[i])
        //    //    .Distinct(new SharpDX.
        //    //    IEqualityComparer
        //    var allVertices = Enumerable
        //        .Range(0, Vertices.Count)
        //        .ToDictionary<int, IEnumerable<Face>>(
        //            i => i,
        //            i => Faces
        //                .Where(face => face
        //                    .Indices
        //                    .Select(j => Vertices[j])
        //                    .Contains(Vertices[i])));
                
        //        Faces
        //        .SelectMany(face => face.Indices)
        //        .Distinct()
        //        //.Select(i => Vertices[i])
        //        //.Distinct();

        //    // inset faces
        //    foreach (var face in Faces)
        //    {
        //        var midpoint = face.Indices.Select(i => Vertices[i]).Average();
        //        foreach (var i in face.Indices)
        //        {
        //            Vertices[i] = Vector3.Lerp(midpoint, Vertices[i], 1 - inset);
        //        }
        //    }


        //    //var lines = Faces
        //    //    .Select(face => Enumerable
        //    //        .Range(0, face.Indices.Count)
        //    //        .Select(i => new
        //    //        {
        //    //            I0 = face.Indices[i],
        //    //            I1 = face.Indices[(i + 1) % face.Indices.Count],
        //    //        })
        //    //        .Select(indexLine => new
        //    //        {
        //    //            P0 = Vertices[indexLine.I0],
        //    //            P1 = Vertices[indexLine.I1],
        //    //        })
        //    //        .ToArray())
        //    //    .ToArray();
        //}


        public void RoundEdges(float maxAngle, float insetPercent, int tesselationLevel)
        {
            var tesselationCount = 1 << tesselationLevel;

            // TODO: maxAngle
            foreach (var face in Faces)
            {
                var normal = face.CalculateNormal(this);
                var outerIndices = face.Indices;
                var outerPositions = outerIndices
                    .Select(i => Positions[i])
                    .ToArray();
                var innerPositions = outerPositions
                    .Select(v => v * (1 - insetPercent))
                    .ToArray();
                var innerIndices = Enumerable
                    .Range(Positions.Count, innerPositions.Length)
                    .ToList();
                Positions.AddRange(innerPositions);
                face.Indices = innerIndices;
                //for (int i = 0; i < face.Indices.Count; i++)
                //{
                //    var i0 = face.Indices[i];
                //    var i1 = face.Indices[(i + 1) % face.Indices.Count];
                //}
            }
            
        }


        public Model Clone(string name)
        {
            return Model.Join(name, new[] { this });
        }


        public void ApplyMatrix()
        {
            Positions = Positions
                .Select(v => Vector3.TransformCoordinate(v, Matrix))
                .ToList();
            Normals = Normals
                .Select(v => Vector3.TransformNormal(v, Matrix))
                .ToList();
            Matrix = Matrix.Identity;
        }


        public void AddFace(params Vector3[] vertices)
        {
            Faces.Add(new Face(Enumerable.Range(Positions.Count, vertices.Length))
            {
                //
            });
            Positions.AddRange(vertices);
        }


        public Vector3 CalculateNormal(int faceIndex)
        {
            return Faces[faceIndex].CalculateNormal(this);
        }


        //public Vector3 CalculateNormal(IEnumerable<int> indices)
        //{
        //    var vertices = indices.Select(i => Positions[i]).ToArray();
        //    var u = vertices[1] - vertices[0];
        //    var v = vertices[2] - vertices[0];
        //    var n = Vector3.Normalize(Vector3.Cross(u, v));
        //    return n;
        //}

        
        public void CalculateNormals(float joinAngle)
        {
            // calculate normals
            Normals = Enumerable
                .Range(0, Faces.Count)
                .Select(i => CalculateNormal(i))
                .SelectMany(n => new[] { n, n, n })
                .ToList();

            // TOOO
            if (joinAngle > 0)
            {
                //var allPositions = Faces
                //    .SelectMany(face => face.Indices)
                //    .Select(i => Positions[i])
                //    .ToList();
                
                
                //Enumerable
                //    .Range(0, allPositions.Count)
                //    .Select(i => Enumerable
                //        .Range(0, allPositions.Count)
                //        .Where(j => 
                //            Vector3.Distance(allPositions[i], allPositions[j]) < 0.00001 &&
                //            Math.Acos(Vector3.Dot(Normals[i], Normals[j])) < joinAngle) < joinAngle)
                //    .Select(normals => Vector3.


                //for (int i=0; i < positions.Count; i++)
                //{
                    
                //}
                //var vertices
                throw new NotImplementedException();
            }
            
        }
        
        
        public void ExtrudeFaces(float length)
        {
            // TODO
            throw new NotImplementedException();
            if (Normals.Count == 0)
            {
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        void IDisposable.Dispose()
        {
            Disposer.Dispose(ref _buffer);
        }
    }
}
