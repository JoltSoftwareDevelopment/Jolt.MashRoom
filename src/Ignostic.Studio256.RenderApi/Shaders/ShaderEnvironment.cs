using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using System.Diagnostics;

namespace Ignostic.Studio256.RenderApi
{
    public class ShaderEnvironment : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Buffer Buffer;
        public Matrix[] Matrices;
        public Vector3 LightPosition;
        public float LerpTime;
        public float BeatTime;
        public float AbsoluteTime;
        public float ScreenAspectRatio;
        public float Lead;
        public Vector2 Resolution;
        public float Nisse0;
        public float Nisse1;
        public float Nisse2;
        public float Nisse3;

        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public ShaderEnvironment()
        {
            Matrices = new Matrix[16];
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void UpdateBuffer(DeviceContext context, Matrix world, ICamera camera)
        {
            var view = camera.CreateViewMatrix();
            var projection = camera.CreateProjectionMatrix(Resolution);
            Matrices[0] = Matrix.Transpose(world);
            Matrices[1] = Matrix.Transpose(view);
            Matrices[2] = Matrix.Transpose(projection);
            Matrices[3] = Matrix.Transpose(world * view);
            Matrices[4] = Matrix.Transpose(world * view * projection);
            Matrices[5] = Matrix.Transpose(view * projection);
            Matrices[6] = Matrix.Invert(world);
            Matrices[7] = Matrix.Invert(world * view);
            Matrices[8] = Matrix.Transpose(Matrix.Identity * Matrix.Scaling(LightPosition));
            Matrices[9] = new Matrix(new float[] 
            {
                LerpTime, AbsoluteTime, Resolution.X, Resolution.Y,
                BeatTime, Lead,0,0,
                Nisse0, Nisse1, Nisse2, Nisse3,
                0,0,0,0,
            });
            if (Buffer == null)
            {
                Buffer = new Buffer(context.Device, Matrices.Length * Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }

            context.UpdateSubresource(Matrices, Buffer);
        }

        
        void IDisposable.Dispose()
        {
            if (Buffer != null)
            {
                Buffer.Dispose();
                Buffer = null;
            }
        }
    }
}
