using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using System.Diagnostics;

namespace Ignostic.Studio256.RenderApi
{
    // todo: Refactor
    public interface ICamera
    {
        Matrix CreateViewMatrix();
        Matrix CreateProjectionMatrix(Vector2 resolution);
    }


    public class NoCamera : ICamera
    {
        public Matrix CreateViewMatrix()
        {
            return Matrix.Identity;
        }

        public Matrix CreateProjectionMatrix(Vector2 resolution)
        {
            return Matrix.Identity;
        }
    }

    
    public class Camera : ICamera
    {
        public float ViewAngle = 1;
        public Vector3 Position = Vector3.UnitY;
        public Vector3 Target = Vector3.Zero;
        public Vector3 Up = Vector3.UnitZ;
        public float ZNear = 0.01F;
        public float ZFar = 100F;
        public bool IsViewEnabled = true;
        public bool IsProjectionEnabled = true;


        public Vector3 Direction
        {
            get { return Vector3.Normalize(Target - Position); }
            set { Target = Position + value; }
        }


        public Matrix CreateViewMatrix()
        {
            return IsViewEnabled
                ? Matrix.LookAtLH(Position, Target, Up)
                : Matrix.Identity;
        }

        
        public Matrix CreateProjectionMatrix(Vector2 resolution)
        {
            return IsProjectionEnabled
                ? Matrix.PerspectiveFovLH(ViewAngle, resolution.X / resolution.Y, ZNear, ZFar)
                : Matrix.Identity;
        }


        public void LookAt(Vector3 position, Vector3 target)
        {
            LookAt(position, target, Vector3.UnitZ);
        }

        
        public void LookAt(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;

            var yAxis = Vector3.Normalize(target - position);
            var xAxis = Vector3.Normalize(Vector3.Cross(yAxis, up));
            var zAxis = Vector3.Normalize(Vector3.Cross(xAxis, yAxis));

            Up = zAxis;
        }


        public void MoveRelative(float dx, float dy, float dz)
        {
            var delta = 
                dx * Vector3.Cross(Direction, Up) +
                dy * Direction +
                dz * Up;
            Position += delta;
            Target += delta;
        }


        public void Rotate(float yaw, float pitch, float roll)
        {
            var camera = this;

            // calculate current axis
            var yAxis = camera.Direction;
            var xAxis = Vector3.Normalize(Vector3.Cross(yAxis, camera.Up));
            var zAxis = Vector3.Normalize(Vector3.Cross(xAxis, yAxis));

            // roll: rotate around yAxis
            xAxis = Vector3.TransformNormal(xAxis, Matrix.RotationAxis(yAxis, roll));
            zAxis = Vector3.TransformNormal(zAxis, Matrix.RotationAxis(yAxis, roll));

            // pitch: rotate around xAxis
            yAxis = Vector3.TransformNormal(yAxis, Matrix.RotationAxis(xAxis, pitch));
            zAxis = Vector3.TransformNormal(zAxis, Matrix.RotationAxis(xAxis, pitch));

            // yaw: rotate around zAxis
            xAxis = Vector3.TransformNormal(xAxis, Matrix.RotationAxis(zAxis, yaw));
            yAxis = Vector3.TransformNormal(yAxis, Matrix.RotationAxis(zAxis, yaw));


            // set new camera orientation
            camera.Up = zAxis;
            camera.Direction = yAxis;
        }


        public Camera Clone()
        {
            throw new NotImplementedException();
            return new Camera
            {
                ViewAngle = this.ViewAngle,
                Position = this.Position,
                Target = this.Target,
                Up = this.Up,
            };
        }
    }
}
