using System;
using System.Text;
using System.IO.Ports;
using System.Windows;
using DMX512Reciever.Model;

namespace DMX512Reciever.Common
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Serial_func : Window
    {
        SerialPort serialPort = new SerialPort();

        private byte[] dmxBuffer = new byte[513];
        public byte[] DMXBuffer
        {
            get { return dmxBuffer; }
        }

        private bool rcvStartCode = false;
        private int dmx512Counter = 0;

        private string portName = "";
        private int baudRate = 0;
        private int dataBits = 0;
        private string stopBits = "";
        private int writeTimeout = 0;
        private int readTimeout = 0;
        private string parity = "";
        private string handShake = "";
        private string encoding = "";
        private string newLine = "";

        private string rcvBuffer = "";

        public void init(string pName)
        {
            portName = pName;
            //baudRate = 115200;
            baudRate = 250000;
            dataBits = 8;
            stopBits = "Two";
            writeTimeout = 2000;
            readTimeout = 2000;
            parity = "None";
            handShake = "None";
            encoding = "UTF-8";
            newLine = "\r"; // PC \n, Jig \r

            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = dataBits;
            serialPort.NewLine = newLine;
            serialPort.ReadTimeout = readTimeout;
            serialPort.WriteTimeout = writeTimeout;

            if (stopBits == "None")
            {
                serialPort.StopBits = StopBits.None;
            }
            else if (stopBits == "One")
            {
                serialPort.StopBits = StopBits.One;
            }
            else if (stopBits == "Two")
            {
                serialPort.StopBits = StopBits.Two;
            }
            else if (stopBits == "OnePintFive")
            {
                serialPort.StopBits = StopBits.OnePointFive;
            }

            if (parity == "None")
            {
                serialPort.Parity = Parity.None;
            }
            else if (parity == "Even")
            {
                serialPort.Parity = Parity.Even;
            }
            else if (parity == "Odd")
            {
                serialPort.Parity = Parity.Odd;
            }
            else if (parity == "Space")
            {
                serialPort.Parity = Parity.Space;
            }
            else if (parity == "Mark")
            {
                serialPort.Parity = Parity.Mark;
            }

            if (handShake == "None")
            {
                serialPort.Handshake = Handshake.None;
            }
            else if (handShake == "RequestToSend")
            {
                serialPort.Handshake = Handshake.RequestToSend;
            }
            else if (handShake == "RequestToSendXOnXOff")
            {
                serialPort.Handshake = Handshake.RequestToSendXOnXOff;
            }
            else if (handShake == "XOnXOff")
            {
                serialPort.Handshake = Handshake.XOnXOff;
            }

            if (encoding == "UTF-8")
            {
                serialPort.Encoding = Encoding.UTF8;
            }
            else if (encoding == "ASCII")
            {
                serialPort.Encoding = Encoding.ASCII;
            }

            serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
            serialPort.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(ErrRecieve);
        }

        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            try
            {
                while (serialPort.BytesToRead > 0 && rcvStartCode == true)
                {
                    dmxBuffer[dmx512Counter] = (byte)serialPort.ReadByte();
                    if (++dmx512Counter > 512)
                    {
                        dmx512Counter = 0;
                        rcvStartCode = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rcvBuffer = ex.Message;
            }
            //Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(setRcvData), recieved_data);
        }

        private void ErrRecieve(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            if (e.EventType == SerialError.Frame)
            {
                rcvStartCode = true;
                dmx512Counter = 0;
            }
        }

        public bool serialOpen()
        {
            if (serialPort.IsOpen)
            {
                MessageBox.Show(portName + " はすでに開かれています。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public void serialClose()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public bool sendCommand(string cmdString, ref string response)
        {
            DateTime startDt = new DateTime();
            TimeSpan ts = new TimeSpan();

            bool loopFlag = false;

            if (serialPort.IsOpen == false)
            {
                return false;
            }

            try
            {
                startDt = DateTime.Now;
                loopFlag = true;
                rcvBuffer = "";
                serialPort.Write(cmdString);

                while (rcvBuffer == "" && loopFlag)
                {
                    ts = DateTime.Now - startDt;
                    if(ts.Seconds >= 2)
                    {
                        loopFlag = false;
                    }
                }

                response = rcvBuffer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public string makeBCC(string cmdString)
        {
            // 文字列からBCCを作る
            string bccStr = "";
            byte[] byteArray = Encoding.ASCII.GetBytes(cmdString);
            byte bccByte = 0;

            foreach (byte byteChar in byteArray)
            {
                bccByte = (byte)(bccByte ^ byteChar);
            }

            bccStr = bccByte.ToString("X2");

            return bccStr;
        }

        public int checkBCC(string commandLine)
        {
            // BCCをチェックする
            // チェックフラグ
            int bccOK = 0;
            // BCC計算用文字列
            string bccStr = "";
            // 計算したBCC
            string bcc = "";
            // コマンド上のBCC
            string cmdBcc = "";

            if (commandLine == "")
            {
                return 0;
            }

            // BCCとBCCまでを分けて抽出
            // コマンドの種類に応じて文字数を決めて抜き取り、BCCの計算
            switch (commandLine.Substring(1, 1))
            {
                case "T": // Test
                    bccStr = commandLine.Substring(0, 2);
                    cmdBcc = commandLine.Substring(2, 2);
                    bcc = makeBCC(bccStr);
                    break;
                case "I": // GetInfo
                    int idx = commandLine.IndexOf(",");
                    bccStr = commandLine.Substring(0, idx + 4);
                    cmdBcc = commandLine.Substring(idx + 4, 2);
                    bcc = makeBCC(bccStr);
                    break;
                case "D": // SetDMX512
                    bccStr = commandLine.Substring(0, 2);
                    cmdBcc = commandLine.Substring(2, 2);
                    bcc = makeBCC(bccStr);
                    break;
                case "S": // WriteSerial
                    bccStr = commandLine.Substring(0, 2);
                    cmdBcc = commandLine.Substring(2, 2);
                    bcc = makeBCC(bccStr);
                    break;
                case "C": // WriteAddress
                    bccStr = commandLine.Substring(0, 2);
                    cmdBcc = commandLine.Substring(2, 2);
                    bcc = makeBCC(bccStr);
                    break;
                default:
                    break;
            }

            // BCCを比較
            if (cmdBcc == bcc)
            {
                bccOK = 1;
            }
            else
            {
                bccOK = 0;
            }

            return bccOK;
        }
    }
}
