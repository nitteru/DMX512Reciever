using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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

namespace DMX512Reciever
{
    public class ComPort
    {
        public string portName { get; set; }
    }
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // シリアルポートオブジェクト
        private SerialPort _serialPort;

        /* XAMLで定義したコントロールのリスト
         *
         * @note
         *   - 本来ならVisualTree等を使って操作するべきところ
         *   - XAMLで記述順に参照できる
         */
        //XAMLで定義したラベルのリスト
        List<Label> _labels = new List<Label>();
        // XAMLで定義した矩形のリスト
        List<Rectangle> _rectangles = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            _labels.Add((Label)sender);
        }

        private void Rectangle_Loaded(object sender, RoutedEventArgs e)
        {
            _rectangles.Add((Rectangle)sender);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private ObservableCollection<ComPort> GetComPorts()
        {
            var results = new ObservableCollection<ComPort>();
            string[] ports = SerialPort.GetPortNames();

            foreach (var m in ports)
                {
                    results.Add(new ComPort()
                    {
                        portName = (string)m,
                    });
                }
            return results;
    }
}
