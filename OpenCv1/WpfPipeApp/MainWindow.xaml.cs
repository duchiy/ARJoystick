using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SimplePipeClient;
using System.ComponentModel;
namespace WpfPipeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _bPipeCommClosed = true;
        DispatcherTimer _timer;
        public delegate void UpdateUI();
        public event UpdateUI UpdateUi;
        SimplePipeClient.SimplePipeClient PipeClient;
        string Ms;
        public MainWindow()
        {
            InitializeComponent();
            PipeClient = new SimplePipeClient.SimplePipeClient();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 600);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Start();
            this.UpdateUi += new UpdateUI(UpdatePipe);
            this.Closing += new CancelEventHandler(Window1_Closing);
        }
        private void OnTick(object sender, EventArgs evt)
        {
            try
            {
                _timer.Stop();
                UpdateAll();
                _timer.Start();
            }
            catch (Exception e)
            {

                //                Text = "Error: " + e;
            }
        }
        public void UpdateAll()
        {
            if (!_bPipeCommClosed)
            {
                UpdatePipe();
            }
        }
        public void UpdatePipe()
        {
            //           this.txtBox.Text = PServer1.ReceivedMessage;
            //           PServer2.SendMessage("Okay", PServer2.clientse);
            string command = "";
            SimplePipeClient.SimplePipeClient.ReceiveByteAndRespond(ref command);
            txtBox.Text = command;
            if (command == "end")
            {
                _bPipeCommClosed = true;
            }

        }
        void Window1_Closing(object sender, CancelEventArgs e)
        {
           System.Windows.Application.Current.Shutdown();
        }
        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
//            txtBox.Text = "Dude";
        }

        private void btnConnectPipe_Click(object sender, RoutedEventArgs e)
        {
            _bPipeCommClosed = false;
        }

        private void btnDisconnectPipe_Click(object sender, RoutedEventArgs e)
        {
            _bPipeCommClosed = true;
        }
    }
}
