using System.Drawing;
using System.Windows.Forms;
using Ignostic.Studio256.RenderApi;

namespace Jolt.MashRoom
{
    public class InputHandler
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Demo _demo;
        private Point _previousMouseLocation;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public InputHandler()
        {
            _previousMouseLocation = Point.Empty;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public InputHandler Bind(Demo demo)
        {
            _demo = demo;
            _demo.Form.KeyUp += KeyUpHandler;
            _demo.Form.MouseMove += MouseMoveHandler;
            _demo.Form.MouseWheel += MouseWheelHandler;
            _demo.Form.ResizeEnd += (sender, args) => _demo.OutputWasResized = true;
            return this;
        }


        private void KeyUpHandler(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                //case Keys.F5:
                //    swapChain.SetFullscreenState(true, null);
                //    break;
                //case Keys.F4:
                //    swapChain.SetFullscreenState(false, null);
                //    break;
                case Keys.Escape:
                    _demo.Form.Close();
                    break;
                //case Keys.Left:
                //    _demo.Timer.Time -= args.Shift ? 10 : 1;
                //    break;
                //case Keys.Right:
                //    //_demo.Timer.Time = _demo.TimeFrame.LastInterval.EndTime;
                //    _demo.Timer.Time += args.Shift ? 10 : 1;
                //    break;
                //case Keys.F11:
                //    _demo.UseManualCamera = false;
                //    break;
                //case Keys.F12:
                //    _demo.ManualCamera = _demo.Cameras[_demo.CameraIndex].Clone();
                //    _demo.UseManualCamera = true;
                //    break;
                //case Keys.Space:
                //    if (_demo.Timer.IsPlaying)
                //    {
                //        _demo.Timer.StopPlaying();
                //    }
                //    else
                //    {
                //        _demo.Timer.StartPlaying();
                //    }
                //    break;
            }
        }


        private void MouseMoveHandler(object sender, MouseEventArgs args)
        {
            var dx = 0.001F * (args.X - _previousMouseLocation.X);
            var dy = 0.001F * (args.Y - _previousMouseLocation.Y);
            _previousMouseLocation = args.Location;
            
            if (!_demo.UseManualCamera)
                return;

            if (args.Button == MouseButtons.Left)
            {
                _demo.ManualCamera.Rotate(-dx, dy, 0);
            }
            else if (args.Button == MouseButtons.Right)
            {
                _demo.ManualCamera.MoveRelative(dx, 0, dy);
            }
            else if (args.Button == (MouseButtons.Left | MouseButtons.Right))
            {
                _demo.ManualCamera.Rotate(0, 0, 10*dx);
            }
        }

        
        private void MouseWheelHandler(object sender, MouseEventArgs args)
        {
            if (!_demo.UseManualCamera)
                return;

            _demo.ManualCamera.MoveRelative(0, 0.001F * args.Delta, 0);
        }
    }
}
