using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XGamepadDemo;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using StageConsole;
using SimplePipeClient;

namespace XBoxJoyStickB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XGamepadDemo.XBoxJoyStick _JoyStick = new XGamepadDemo.XBoxJoyStick();
        StageConsole.StageControl _myStage;
        DispatcherTimer _timer;
        public delegate void Updatedro();
        public event Updatedro UpdateDRO1;

        public delegate void UpdatePipe();
        public event UpdatePipe UpdateState;

        bool _bPipeCommClosed = true;
        //protected void OnUpdate(string message)
        //{
        //    if (Update1 != null)
        //    {
        //        Update1();
        //    }
        //}
        private void OnTick(object sender, EventArgs evt)
        {
            try
            {
                _timer.Stop();
                Update();
                _timer.Start();
            }
            catch (Exception e)
            {
               _timer.Stop();
                //                Text = "Error: " + e;
            }
        }

        public void Update()
        {
            UpdateDRO1();
            if (!_bPipeCommClosed)
            {
                UpdateState();
            }
        }
        public void UpdateDRO()
        {
            double X = 0.0, Y = 0.0, Z = 0.0;
            _myStage.GetPos(ref X, ref Y, ref Z);

            X = Math.Round(X, 5);
            Y = Math.Round(Y, 5);
            Z = Math.Round(Z, 5);
            txtBoxXAxis.Text = X.ToString();
            txtBoxYAxis.Text = Y.ToString();
            txtBoxZAxis.Text = Z.ToString();
        }
        public void UpdatePipeState()
        {
            //           this.txtBox.Text = PServer1.ReceivedMessage;
            //           PServer2.SendMessage("Okay", PServer2.clientse);
            string command = "";
            double x = 0;
            double y = 0;
            var thread = new Thread(
              () =>
              {
                  command = SimplePipeClient.SimplePipeClient.ReceiveByteAndRespond();
              });
            thread.Start();
            thread.Join();


            if (command == "end")
            {
                _bPipeCommClosed = true;
            }
            else
            {
                string[] axis = command.Split(',');
                x = Convert.ToDouble(axis[0]);
                y = Convert.ToDouble(axis[1]);

                _myStage.MoveTo(enAxis.X, x);
                _myStage.MoveTo(enAxis.Y, y);
                Thread.Sleep(500);
                txtBoxXAxis.Text = x.ToString();
                txtBoxYAxis.Text = y.ToString();
                Thread.Sleep(500);
                UpdateDRO1();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.UpdateDRO1 += new Updatedro(UpdateDRO);
            this.UpdateState += new UpdatePipe(UpdatePipeState);

            _myStage = new StageControl(@"C:\KMotion433x64\C Programs\SnapAmp\InitSnapAmp3Axis.c");
            _myStage.XCountsPerInch = 14000;
            _myStage.YCountsPerInch = 14000;
            _myStage.ZCountsPerInch = 13220;
            _myStage.SetMotionParams();
//            _myStage.ReverseXAxisDirection(true);
            _myStage.EnableAxis(enAxis.X);
            _myStage.EnableAxis(enAxis.Y);
            _myStage.EnableAxis(enAxis.Z);
            _myStage.ZeroAxis(enAxis.X);
            _myStage.ZeroAxis(enAxis.Y);
            _myStage.ZeroAxis(enAxis.Z);

            _JoyStick.Initialize(0);
            _JoyStick.Stage = _myStage;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Start();
            //            _timer.Stop();
        }

        private void btnPlusX_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _myStage.MoveToRel(enAxis.X, 0.15);
            Thread.Sleep(100);
            _timer.Start();
        }

        private void btnMinusY_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _myStage.MoveToRel(enAxis.Y, -0.15);
            Thread.Sleep(100);
            _timer.Start();
        }

        private void btnPlusY_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _myStage.MoveToRel(enAxis.Y, 0.15);
            Thread.Sleep(100);
            _timer.Start();
        }

        private void btnMinusX_Click(object sender, RoutedEventArgs e)
        {

            _timer.Stop();
            _myStage.MoveToRel(enAxis.X, -0.15);
            Thread.Sleep(100);
            _timer.Start();

        }

        private void txtBoxXAxis_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void txtBoxYAxis_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtBoxZAxis_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnConnectPipe_Click(object sender, RoutedEventArgs e)
        {
            _bPipeCommClosed = false;
//            _timer.Start();
        }

        private void btnDisconnectPipe_Click(object sender, RoutedEventArgs e)
        {
            _bPipeCommClosed = true;
            _timer.Start();
        }
    }
}
