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
                int ret = RecievePlcResponse();
                Console.WriteLine($"受信データ: {ret}");
                // Thread.Sleep(1000); // 1秒待機
                _= SendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
            }
        }
        public async Task SendData()
        {
            string message = plcProtocolHandler.BuildReciveCommand();
            serialPort.WriteLine(message);
            await Task.Delay(10); // 100ms待機
        }
        public void Close()
        {
            serialPort.Close();
        }
        private int RecievePlcResponse()
        {

            while (true)
            {
                if (serialPort.BytesToRead > 0)
                {
                    _ =  AppendRacieveData();

                    // 改行を検知
                    if (HasReceivedCompleteResponse(out int bitRead)) return bitRead;

                }
            }
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
        private async Task AppendRacieveData()
        {
            string received = serialPort.ReadExisting();
            dataBuffer.Append(received);
            await Task.Delay(10); // 100ms待機
        }
  


    }
}
