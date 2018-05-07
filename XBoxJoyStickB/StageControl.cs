using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMotion_dotNet;
using System.Threading;

namespace StageConsole
{
    public enum enAxis { X, Y, Z };
    public class StageControl
    {
        KM_Controller _KM;
        KM_Axis _XAxis;
        KM_Axis _YAxis;
        KM_Axis _ZAxis;
        KM_MotionParams _MotionParams;
        KM_Interpreter _Interpreter;

        bool _bReverseX;
        bool _bReverseY;
        bool _bReverseZ;

        double _XCountsPerInch = new double();
        double _YCountsPerInch = new double();
        double _ZCountsPerInch = new double();
        public double XCountsPerInch
        {
            get
            {
                return this._XCountsPerInch;
            }
            set
            {
                // Can only be called in this class.
                this._XCountsPerInch = value;
            }
        }
        public double YCountsPerInch
        {
            get
            {
                return this._YCountsPerInch;
            }
            set
            {
                // Can only be called in this class.
                this._YCountsPerInch = value;
            }
        }
        public double ZCountsPerInch
        {
            get
            {
                return this._ZCountsPerInch;
            }
            set
            {
                // Can only be called in this class.
                this._ZCountsPerInch = value;
            }
        }
//        Double JogSpeed = 200;
        public StageControl()
        {
            _XCountsPerInch = 200;
            _YCountsPerInch = 200;
            _ZCountsPerInch = 200;
        }
        public StageControl(String pathFile)
        {
            _KM = new KMotion_dotNet.KM_Controller();
            CompileAndLoadCoff(pathFile);

            _XAxis = _KM.GetAxis(0, "X");
            _YAxis = _KM.GetAxis(1, "Y");
            _ZAxis = _KM.GetAxis(2, "Z");

            _bReverseX = false;
            _bReverseY = false;
            _bReverseZ = false;
        }
        public void SetMotionParams()
        {
            _MotionParams = _KM.CoordMotion.MotionParams;
            _MotionParams.CountsPerInchX = _XCountsPerInch;
            _MotionParams.CountsPerInchY = _YCountsPerInch;
            _MotionParams.CountsPerInchZ = _ZCountsPerInch;
            _KM.CoordMotion.SetTPParams();
            _Interpreter = new KM_Interpreter(_KM.CoordMotion);

        }
        int CompileAndLoadCoff(String pathFile)
        {
            try
            {
                String Result = _KM.CompileAndLoadCoff(1, pathFile, false);
                if (Result != "")
                {
                    MessageBox.Show(Result, "Compile Error");
                }
                else
                {
                    // everything ok, execute the Thread
                    _KM.WriteLine("Execute1");
                }
            }
            catch (DMException ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }

            return 0;
        }

        public int ReverseXAxisDirection(bool X)
        {
            _bReverseX = X;
            return 0;
        }
        public int ReverseYAxisDirection(bool Y)
        {
            _bReverseY = Y;
            return 0;
        }
        public int ReverseZAxisDirection(bool Z)
        {
            _bReverseZ = Z;
            return 0;
        }
        public int ZeroAxis(enAxis Axis)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.ZeroAxis();
                    break;
                case enAxis.Y:
                    _YAxis.ZeroAxis();
                    break;
                case enAxis.Z:
                    _ZAxis.ZeroAxis();
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }
            return 0;

        }

        public int EnableAxis(enAxis Axis)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.Enable();
                    break;
                case enAxis.Y:
                    _YAxis.Enable();
                    break;
                case enAxis.Z:
                    _ZAxis.Enable();
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }
            return 0;

        }
        public int EnableAxis()
        {
            EnableAxis(enAxis.X);
            EnableAxis(enAxis.Y);
            EnableAxis(enAxis.Z);
            return 0;

        }
        public int SetJog(double jog)
        {
            SetJog(enAxis.X, jog);
            SetJog(enAxis.Y, jog);
            SetJog(enAxis.Z, jog);
            return 0;

        }
        public int SetJog(enAxis Axis, double vel)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.Jog(vel);
                    break;
                case enAxis.Y:
                    _YAxis.Jog(vel);
                    break;
                case enAxis.Z:
                    _ZAxis.Jog(vel);
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }
            return 0;

        }
        public int Stop()
        {
            Stop(enAxis.X);
            Stop(enAxis.Y);
            Stop(enAxis.Z);
            return 0;

        }
        public int Stop(enAxis Axis)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.Stop();
                    break;
                case enAxis.Y:
                    _YAxis.Stop();
                    break;
                case enAxis.Z:
                    _ZAxis.Stop();
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }

            return 0;

        }


        public int MoveToRel(double x, double y, double z)
        {
            MoveToRel(enAxis.X, x);
            MoveToRel(enAxis.Y, y);
            MoveToRel(enAxis.Z, z);
            return 0;

        }
        public int MoveTo(double x, double y, double z)
        {
            MoveTo(enAxis.X, x);
            MoveTo(enAxis.Y, y);
            MoveTo(enAxis.Z, z);
            return 0;

        }
        public int Zero(enAxis Axis)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.ZeroAxis();
                    break;
                case enAxis.Y:
                    _YAxis.ZeroAxis();
                    break;
                case enAxis.Z:
                    _ZAxis.ZeroAxis();
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }

            return 0;
        }
        public int MoveTo(enAxis Axis, double dist)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.Velocity = 3000.0;
                    if (_bReverseX)
                    {
                        dist = -dist;
                    }
                    _XAxis.MoveTo(dist * _XCountsPerInch);
                    break;
                case enAxis.Y:
                    _YAxis.Velocity = 3000.0;
                    if (_bReverseY)
                    {
                        dist = -dist;
                    }
                    _YAxis.MoveTo(dist * _YCountsPerInch);
                    break;
                case enAxis.Z:
                    _ZAxis.Velocity = 3000.0;
                    if (_bReverseZ)
                    {
                        dist = -dist;
                    }
                    _ZAxis.MoveTo(dist * _ZCountsPerInch);
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }

            return 0;

        }
        public int MoveToRel(enAxis Axis, double dist)
        {
            switch (Axis)
            {
                case enAxis.X:
                    _XAxis.Velocity = 6000.0;
                    if (_bReverseX)
                    {
                        dist = -dist;
                    }
                    _XAxis.StartRelativeMoveTo(dist*_XCountsPerInch);
                    break;
                case enAxis.Y:
                    _YAxis.Velocity = 6000.0;
                    if (_bReverseY)
                    {
                        dist = -dist;
                    }
                    _YAxis.StartRelativeMoveTo(dist * _YCountsPerInch);
                    break;
                case enAxis.Z:
                    _ZAxis.Velocity = 6000.0;
                    if (_bReverseZ)
                    {
                        dist = -dist;
                    }
                    _ZAxis.StartRelativeMoveTo(dist * _ZCountsPerInch);
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;

            }

            return 0;

        }
        public int GetPos(ref double x, ref double y, ref double z)
        {

            x = _KM.CoordMotion.GetAxisPosition(0);
            y = _KM.CoordMotion.GetAxisPosition(1);
            z = _KM.CoordMotion.GetAxisPosition(2);

            x = x / _XCountsPerInch;
            y = y / _YCountsPerInch;
            z = z / _ZCountsPerInch;
            if (_bReverseX)
            {
                x = -x;

            }

            if (_bReverseY)
            {
                y = -y;

            }

            if (_bReverseZ)
            {
                z = -z;
            }

            return 0;

        }
    }
}
