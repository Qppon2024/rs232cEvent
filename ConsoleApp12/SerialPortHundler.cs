using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp12
{
    public class SerialPortHundler

    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        SerialPort serialPort;
        PlcProtocolHandler plcProtocolHandler;
        StringBuilder dataBuffer = new StringBuilder(1200);
        public SerialPortHundler()
        {
            plcProtocolHandler = new();
            // シリアルポートの設定
            const string CR = "\r"; //CR
            serialPort = new SerialPort("COM3", 38400, Parity.Even, 7, StopBits.Two);
            serialPort.NewLine = CR;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); // イベントハンドラを登録
            serialPort.Open();
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // データを受信
                int ret = RecievePlcResponse(_cts.Token);
                Console.WriteLine($"受信データ: {ret}");
                // Thread.Sleep(1000); // 1秒待機
                SendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
            }
        }
        public void SendData()
        {
            string message = plcProtocolHandler.BuildReciveCommand();
            serialPort.WriteLine(message);
 
        }
        public void Close()
        {
            _cts.Cancel();
            serialPort.Close();
        }
        private int RecievePlcResponse(CancellationToken token)
        {

            while (!token.IsCancellationRequested)
            {
                if (serialPort.BytesToRead > 0)
                {
                    AppendRacieveData();

                    // 改行を検知
                    if (HasReceivedCompleteResponse(out int bitRead))
                        return bitRead;

                }// else
                 //   await Task.Delay(10); // 100ms待機
            
            }

            return -1;
        }
        // HasReceivedCompleteResponseメソッドは、受信したデータが完全な応答であるかどうかを確認します。
        private bool HasReceivedCompleteResponse(out int bitRead)
        {
            if (dataBuffer.ToString().Contains(serialPort.NewLine))
            {
                string completeData = dataBuffer.ToString();
                Console.WriteLine($"受信データ: {completeData}");
                dataBuffer.Clear(); // バッファをクリア
                
                return plcProtocolHandler.ReadBit(completeData, out bitRead);


            }
            bitRead = 0;
            return false;
        }
        // AppendRacieveDataメソッドは、受信したデータをバッファに追加します。
        private void AppendRacieveData()
        {
            string received = serialPort.ReadExisting();
            dataBuffer.Append(received);
          //  await Task.Delay(10); // 100ms待機
        }
  


    }
}
