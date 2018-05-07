using System;
using System.Collections.Generic;
using System.Text;
using XGamePad;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using Microsoft.DirectX.XInput;
using StageConsole;

namespace XGamepadDemo
{
    class XBoxJoyStick
    {
        StageControl myStage;
        public StageControl Stage
        {
            get { return myStage; }
            set { myStage = value; }
        }

        Gamepad gamepad;
        DispatcherTimer timer;
        string Text;
        public void Initialize(int devIndex)
        {

            double initRightThumbX = 0.0, initRightThumbY = 0.0;
            double initLeftThumbX = 0.0, initLeftThumbY = 0.0;
//            InitStage();
            OpenGamepad(devIndex);
            GetThumbXY(GamepadButtons.RightThumb, 0, ref initRightThumbX, ref initRightThumbY);
            GetThumbXY(GamepadButtons.LeftThumb, 0, ref initLeftThumbX, ref initLeftThumbY);
            gamepad.SetThumbRadius(GamepadButtons.RightThumb, initRightThumbX, initRightThumbY);
            gamepad.SetThumbRadius(GamepadButtons.LeftThumb, initLeftThumbX, initLeftThumbY);

        }

        public void OpenGamepad(int devIndex)
        {
            Text = "Accessing gamepad device " + devIndex + "\r";
            gamepad = new Gamepad(devIndex);

            gamepad.OnAButtonPress += new Gamepad.ButtonHandler(this.OnAButtonPress);
            gamepad.OnAButtonRelease += new Gamepad.ButtonHandler(this.OnAButtonRelease);
            gamepad.OnBackButtonPress += new Gamepad.ButtonHandler(this.OnBackButtonPress);
            gamepad.OnBackButtonRelease += new Gamepad.ButtonHandler(this.OnBackButtonRelease);
            gamepad.OnBButtonPress += new Gamepad.ButtonHandler(this.OnBButtonPress);
            gamepad.OnBButtonRelease += new Gamepad.ButtonHandler(this.OnBButtonRelease);
            gamepad.OnDPadChange += new Gamepad.DPadHandler(this.OnDPadChange);
            gamepad.OnDPadPress += new Gamepad.DPadHandler(this.OnDPadPress);
            gamepad.OnDPadRelease += new Gamepad.DPadHandler(this.OnDPadRelease);
            gamepad.OnLeftThumbUpdate += new Gamepad.ThumbHandler(this.OnLeftThumbUpdate);
            gamepad.OnLeftShoulderButtonPress += new Gamepad.ButtonHandler(this.OnLeftShoulderButtonPress);
            gamepad.OnLeftShoulderButtonRelease += new Gamepad.ButtonHandler(this.OnLeftShoulderButtonRelease);
            gamepad.OnLeftThumbButtonPress += new Gamepad.ButtonHandler(this.OnLeftThumbButtonPress);
            gamepad.OnLeftThumbButtonRelease += new Gamepad.ButtonHandler(this.OnLeftThumbButtonRelease);
            gamepad.OnLeftTriggerChange += new Gamepad.TriggerHandler(this.OnLeftTriggerChange);
            gamepad.OnLeftTriggerPress += new Gamepad.TriggerHandler(this.OnLeftTriggerPress);
            gamepad.OnLeftTriggerRelease += new Gamepad.TriggerHandler(this.OnLeftTriggerRelease);
            gamepad.OnRightThumbUpdate += new Gamepad.ThumbHandler(this.OnRightThumbUpdate);
            gamepad.OnRightShoulderButtonPress += new Gamepad.ButtonHandler(this.OnRightShoulderButtonPress);
            gamepad.OnRightShoulderButtonRelease += new Gamepad.ButtonHandler(this.OnRightShoulderButtonRelease);
            gamepad.OnRightThumbButtonPress += new Gamepad.ButtonHandler(this.OnRightThumbButtonPress);
            gamepad.OnRightThumbButtonRelease += new Gamepad.ButtonHandler(this.OnRightThumbButtonRelease);
            gamepad.OnRightTriggerChange += new Gamepad.TriggerHandler(this.OnRightTriggerChange);
            gamepad.OnRightTriggerPress += new Gamepad.TriggerHandler(this.OnRightTriggerPress);
            gamepad.OnRightTriggerRelease += new Gamepad.TriggerHandler(this.OnRightTriggerRelease);
            gamepad.OnStartButtonPress += new Gamepad.ButtonHandler(this.OnStartButtonPress);
            gamepad.OnStartButtonRelease += new Gamepad.ButtonHandler(this.OnStartButtonRelease);
            gamepad.OnXButtonPress += new Gamepad.ButtonHandler(this.OnXButtonPress);
            gamepad.OnXButtonRelease += new Gamepad.ButtonHandler(this.OnXButtonRelease);
            gamepad.OnYButtonPress += new Gamepad.ButtonHandler(this.OnYButtonPress);
            gamepad.OnYButtonRelease += new Gamepad.ButtonHandler(this.OnYButtonRelease);
            gamepad.OnThumbBackInsideRadius += new Gamepad.ThumbHandler(this.OnThumbBackInsideRadius);

            gamepad.OnConnect += new Gamepad.ConnectionHandler(this.OnConnect);
            gamepad.OnDisconnect += new Gamepad.ConnectionHandler(this.OnDisconnect);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += new EventHandler(OnTick);
            timer.Start();
        }
        public void Disable()
        {
            timer.Stop();
        }
        public void Enable()
        {
            timer.Start();
        }
        private void OnTick(object sender, EventArgs evt)
        {
            try

            {
                gamepad.Update();
            }
            catch (Win32Exception e)
            {
                timer.Stop();
                Text = "Error: " + e;
            }
        }
        private void OnConnect(object sender, GamepadEventArgs evt)
        {
            Text = "Device " + evt.DeviceID + " is connected";
        }

        private void OnDisconnect(object sender, GamepadEventArgs evt)
        {
            Text = "Device " + evt.DeviceID + " is not connected.";
        }

        private void OnAButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.A) Text = "Expected A button, got " + evt.Button;
        }
        private void OnAButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.A)
            {
                myStage.Stop();
                Text = "Expected A button, got " + evt.Button;
            }
        }
        private void OnBButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.B)
            {
                myStage.Zero(enAxis.Z);
                Text = "Expected B button, got " + evt.Button;
            }
        }
        private void OnBButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.B)
            {

                Text = "Expected B button, got " + evt.Button;
            }
        }
        private void OnXButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.X)
            {
                myStage.Zero(enAxis.X);
                Text = "Expected X button, got " + evt.Button;

            }
        }
        private void OnXButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.X) Text = "Expected X button, got " + evt.Button;
        }
        private void OnYButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Y)
            {
                myStage.Zero(enAxis.Y);
                Text = "Expected Y button, got " + evt.Button;
            }

        }
        private void OnYButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Y) Text = "Expected Y button, got " + evt.Button;
        }
        private void OnStartButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Start) Text = "Expected Start button, got " + evt.Button;
        }
        private void OnStartButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Start) Text = "Expected Start button, got " + evt.Button;
        }
        private void OnBackButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Back) Text = "Expected Back button, got " + evt.Button;
        }
        private void OnBackButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.Back) Text = "Expected Back button, got " + evt.Button;
        }
        private void OnLeftThumbButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.LeftThumb);
        }
        private void OnLeftThumbButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.LeftThumb);
        }
        private void OnRightThumbButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.RightThumb);
        }
        private void OnRightThumbButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.RightThumb) ;
        }
        private void OnLeftShoulderButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.LeftShoulder) ;
        }
        private void OnLeftShoulderButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.LeftShoulder) ;
        }
        private void OnRightShoulderButtonPress(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.RightShoulder) ;
        }
        private void OnRightShoulderButtonRelease(object sender, GamepadButtonEventArgs evt)
        {
            if (evt.Button == GamepadButtons.RightShoulder) ;
        }

        private void OnDPadChange(object sender, GamepadDPadEventArgs evt)
        {
            // Reset
            //DPadDownHighlight.Visible = false;
            //DPadDownLeftHighlight.Visible = false;
            //DPadDownRightHighlight.Visible = false;
            //DPadLeftHighlight.Visible = false;
            //DPadLeftHighlight.Visible = false;
            //DPadRightHighlight.Visible = false;
            //DPadUpHighlight.Visible = false;
            //DPadUpLeftHighlight.Visible = false;
            //DPadUpRightHighlight.Visible = false;

            bool DownIsDown = (evt.Buttons & GamepadButtons.DPadDown) != 0;
            bool LeftIsDown = (evt.Buttons & GamepadButtons.DPadLeft) != 0;
            bool RightIsDown = (evt.Buttons & GamepadButtons.DPadRight) != 0;
            bool UpIsDown = (evt.Buttons & GamepadButtons.DPadUp) != 0;

            //if (DownIsDown) DPadDownHighlight.Visible = true;

            //if (LeftIsDown) DPadLeftHighlight.Visible = true;

            //if (RightIsDown) DPadRightHighlight.Visible = true;

            //if (UpIsDown) DPadUpHighlight.Visible = true;

            //if (DownIsDown && LeftIsDown)
            //{
            //    DPadDownHighlight.Visible = false;
            //    DPadLeftHighlight.Visible = false;
            //    DPadDownLeftHighlight.Visible = true;
            //}

            //if (DownIsDown && RightIsDown)
            //{
            //    DPadDownHighlight.Visible = false;
            //    DPadRightHighlight.Visible = false;
            //    DPadDownRightHighlight.Visible = true;
            //}

            //if (UpIsDown && LeftIsDown)
            //{
            //    DPadUpHighlight.Visible = false;
            //    DPadLeftHighlight.Visible = false;
            //    DPadUpLeftHighlight.Visible = true;
            //}

            //if (UpIsDown && RightIsDown)
            //{
            //    DPadUpHighlight.Visible = false;
            //    DPadRightHighlight.Visible = false;
            //    DPadUpRightHighlight.Visible = true;
            //}
        }

        private void OnDPadPress(object sender, GamepadDPadEventArgs evt)
        {
            Text = "DPad pressed";
            bool DownIsDown = (evt.Buttons & GamepadButtons.DPadDown) != 0;
            bool LeftIsDown = (evt.Buttons & GamepadButtons.DPadLeft) != 0;
            bool RightIsDown = (evt.Buttons & GamepadButtons.DPadRight) != 0;
            bool UpIsDown = (evt.Buttons & GamepadButtons.DPadUp) != 0;
//            double x=0.0, y = 0.0, z = 0.0;
            double deltaz = 2000/ myStage.ZCountsPerInch;
            if (UpIsDown || DownIsDown)
            {

                if (UpIsDown)
                {
//                    myStage.GetPos(ref x, ref y, ref z);
                    myStage.MoveToRel(0, 0, deltaz);
                }
                if (DownIsDown)
                {
//                    myStage.GetPos(ref x, ref y, ref z);
                    myStage.MoveToRel(0, 0, -deltaz);
                }
            }

            Thread.Sleep(50);


        }


        private void OnDPadRelease(object sender, GamepadDPadEventArgs evt)
        {
            Text = "DPad released";
            myStage.Stop();
        }

        private void OnLeftTriggerPress(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Left)
            {
                Text = "OnLeftTriggerPress";
            }
        }
        private void OnLeftTriggerChange(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Left)
            {
                Text = "OnLeftTriggerChange";
            }
        }
        private void OnLeftTriggerRelease(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Left)
            {
                Text = "OnLeftTriggerRelease";
            }
        }

        private void OnRightTriggerPress(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Right) Text = "OnRightTriggerPress";
        }
        private void OnRightTriggerChange(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Right) Text = "OnRightTriggerChange";
        }
        private void OnRightTriggerRelease(object sender, GamepadTriggerEventArgs evt)
        {
            if (evt.Trigger == GamepadTriggers.Right) Text = "OnRightTriggerRelease";
        }

        private void OnLeftThumbUpdate(object sender, GamepadThumbEventArgs evt)
        {
            if (evt.Thumb == GamepadThumbs.Left)
            {
 //               double x=0.0, y = 0.0, z = 0.0;
                double leftThumbCurrentRadius = 0.0;
                double leftThumbRadius = 0.0;
                double leftThumbX = 0.0;
                double leftThumbY = 0.0;
                double deltaRadius = 0.0, deltax = 0.0, deltay = 0.0, deltaz=0.05;
                double magnitude = 0.0;
                double deltapos = 100;
                Gamepad gp = (Gamepad)sender;

                gamepad.GetThumbRadius(GamepadButtons.LeftThumb, ref leftThumbRadius);
                gamepad.GetThumbXY(GamepadButtons.LeftThumb, ref leftThumbX, ref leftThumbY);

                leftThumbCurrentRadius = gamepad.CalculateMagnitude((double)evt.XPosition, (double)evt.YPosition);

                deltaRadius = gamepad.CalculatePosDifference(leftThumbRadius, leftThumbCurrentRadius, 5000);

                if (deltaRadius <= 0.0)
                {
                    return;
                }

                deltax = gamepad.CalculatePosDifference((double)evt.XPosition, leftThumbX, 5000);
                deltay = gamepad.CalculatePosDifference((double)evt.YPosition, leftThumbX, 5000);
                magnitude = gamepad.CalculateMagnitude(deltax, deltay);

                deltax = deltax / magnitude;
                deltay = deltay / magnitude;

                if (Math.Abs(deltax) > 0.0)
                {
                    return;
                }

                deltax = deltax * deltapos;
                deltay = deltay * deltapos;
                
                if (Math.Abs(deltax) >= 0.5)
                {
                    deltay = 0.0;
                }

                if (Math.Abs(deltay) >= 0.5)
                {
                    deltax = 0.0;
                }
                
                if (deltay > 0)
                {
                    deltaz = -50.05;
                    gp._ZdirectionDown = true;
                }

                if (deltay < 0)
                {
                    deltaz = 50.05;
                    gp._ZdirectionDown = false;
                }

                deltaz = deltaz / myStage.ZCountsPerInch;
                if (!gp._Move3Axis)
                {
                    myStage.MoveToRel(0, 0, deltaz);
                }
                Text = "(" + evt.XPosition + "," + evt.YPosition + ")";

            }
        }

        private void OnThumbBackInsideRadius(object sender, GamepadThumbEventArgs evt)
        {
            if (evt.Thumb == GamepadThumbs.Right)
            {
                myStage.Stop();
                Text = "(" + evt.XPosition + "," + evt.YPosition + ")";

            }
        }

        private void OnRightThumbUpdate(object sender, GamepadThumbEventArgs evt)
        {
            if (evt.Thumb == GamepadThumbs.Right)
            {

                double rightThumbCurrentRadius = 0.0;
                double rightThumbRadius = 0.0;
                double rightThumbX = 0.0;
                double rightThumbY = 0.0;
                double deltaRadius = 0.0, deltax = 0.0, deltay = 0.0, deltaz=0.05;
                double magnitude = 0.0;
                double deltapos = 2000;
                gamepad.GetThumbRadius(GamepadButtons.RightThumb, ref rightThumbRadius);
                gamepad.GetThumbXY(GamepadButtons.RightThumb, ref rightThumbX, ref rightThumbY);
                Gamepad gp = (Gamepad)sender;
                if (gp._ZdirectionDown)
                {
                    deltaz = -deltaz;
                }
                rightThumbCurrentRadius = gamepad.CalculateMagnitude((double)evt.XPosition, (double)evt.YPosition);

                deltaRadius = gamepad.CalculatePosDifference(rightThumbRadius, rightThumbCurrentRadius, 4000.0);

                if (deltaRadius <= 0.0)
                {
                    return;
                }

                deltax = gamepad.CalculatePosDifference((double)evt.XPosition, rightThumbX, 4000);
                deltay = gamepad.CalculatePosDifference((double)evt.YPosition, rightThumbX, 4000);


                magnitude = gamepad.CalculateMagnitude(deltax, deltay);

                deltax = deltax / magnitude;
                deltay = deltay / magnitude;

                if (Math.Abs(deltax) >= 0.9)
                {
                    deltay = 0.0;
                }

                if (Math.Abs(deltay) >= 0.9)
                {
                    deltax = 0.0;
                }

                deltax = deltax * deltapos;
                deltay = deltay * deltapos;

                deltax = deltax / myStage.XCountsPerInch;
                deltay = deltay / myStage.YCountsPerInch;
                deltaz = deltaz / myStage.ZCountsPerInch;

                if (gp._Move3Axis)
                {
                    myStage.MoveToRel(deltax, deltay, deltaz);
                }
                else
                {
                    myStage.MoveToRel(deltax , deltay , 0);
                }
                Text = "(" + evt.XPosition + "," + evt.YPosition + ")";
            }
        }
        private void GetThumbXY(GamepadButtons buttonType, int deviceID, ref double ThumbX, ref double ThumbY)
        {

            XInputState gpState;
            XInputMethods.GetState(deviceID, out gpState);
            XInputGamepad gp = gpState.Gamepad;

            if (buttonType == GamepadButtons.RightThumb)
            {
                ThumbX = (double)gp.RightThumbX;
                ThumbY = (double)gp.RightThumbY;
            }
            if (buttonType == GamepadButtons.LeftThumb)
            {
                ThumbX = (double)gp.LeftThumbX;
                ThumbY = (double)gp.LeftThumbY;
            }

        }

    }
}
