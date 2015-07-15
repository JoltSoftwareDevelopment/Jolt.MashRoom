using System;
using System.Collections.Generic;
using SharpDX;

namespace Jolt.MashRoom.Effects.ProceduralModels
{
    class Cylinder
    {
        public float radius = 0.5f;
        public float height = 2.0f;

        public int segments = 10;

        public int heightSegments = 4;

        private readonly bool _weirdness;

        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<Vector2> TexCoords = new List<Vector2>();
        public List<int> Indices = new List<int>();

        public Cylinder(float radius, float height, int radialSegments, int heightSegments, bool weirdness)
        {
            this.radius = radius;
            this.height = height;
            segments = radialSegments;
            this.heightSegments = heightSegments;
            _weirdness = weirdness;
            CreateCylinder();
        }

        public void AddTriangle(int index0, int index1, int index2)
        {
            Indices.Add(index0);
            Indices.Add(index1);
            Indices.Add(index2);
        }

        public void CreateCylinder()
        {
            float heightInc = height / heightSegments;

            for (var tmp = 0; tmp < heightSegments; tmp++)
            {
                var q = (float)(tmp + 1.0) / heightSegments;
            }

            for (int i = 0; i <= heightSegments; i++)
            {
                Vector3 centrePos = Vector3.Up * heightInc * i;

                float v = (float)i / heightSegments;

                BuildRing(segments, centrePos, radius, v, i > 0, i);
            }
        }

        private void BuildRing(int segmentCount, Vector3 centre, float radius, float v, bool buildTriangles, int heightSeg)
        {
            float angleInc = (MathUtil.TwoPi) / segmentCount;

            var finalRad = 1.0f;
            if (_weirdness)
            {
                float minVal = 0.3f;

                var q = (float)(heightSeg + 1.0) / (heightSegments / 4.0);
                float radOffset = (float)Math.Sin((MathUtil.TwoPi * q));

                radOffset = 0.5f * (1 + radOffset);

                finalRad = ((1 - minVal) * radOffset) + (minVal);
            }
            for (int i = 0; i <= segmentCount; i++)
            {
                float angle = angleInc * i;

                Vector3 unitPosition = Vector3.Zero;
                unitPosition.X = (float)Math.Cos(angle) * (radius * (finalRad));
                unitPosition.Z = (float)Math.Sin(angle) * (radius * (finalRad));

                Vertices.Add(centre + unitPosition);
                Normals.Add(unitPosition);
                TexCoords.Add(new Vector2((float)i / segmentCount, v));

                if (i > 0 && buildTriangles)
                {
                    int baseIndex = Vertices.Count - 1;

                    int vertsPerRow = segmentCount + 1;

                    int index0 = baseIndex;
                    int index1 = baseIndex - 1;
                    int index2 = baseIndex - vertsPerRow;
                    int index3 = baseIndex - vertsPerRow - 1;

                    AddTriangle(index0, index2, index1);
                    AddTriangle(index2, index3, index1);
                }
            }
        }
    }
}
