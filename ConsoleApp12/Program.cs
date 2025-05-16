using System.IO.Ports;
using System.Text;
namespace ConsoleApp12
{
    internal class Program
    {
        static SerialPort serialPort;
        static PlcProtocolHandler plcProtocolHandler ;
        static StringBuilder dataBuffer = new StringBuilder();
        static void Main(string[] args)
        {
            plcProtocolHandler = new ();
            // シリアルポートの設定
            const string CR = "\r"; //CR
            serialPort = new SerialPort("COM3", 38400, Parity.Even, 7, StopBits.Two);
            serialPort.NewLine = CR;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); // イベントハンドラを登録
            serialPort.Open();
            //RecievePlcResponse();
            sendData();
            Console.WriteLine("シリアルポートを開きました。データ受信を待っています...");
            Console.WriteLine("終了するにはEnterキーを押してください。");
            Console.ReadLine();

            serialPort.Close(); Console.WriteLine("Hello, World!");
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // データを受信
                int ret=RecievePlcResponse();
                Console.WriteLine($"受信データ: {ret}");
               // Thread.Sleep(1000); // 1秒待機
                sendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
            }
        }
        private static void sendData()
        {
            string message = plcProtocolHandler.BuildReciveCommand();
            serialPort.WriteLine(message);
        }
        private static int RecievePlcResponse()
        {

            while (true)
            {
                if (serialPort.BytesToRead > 0)
                {
                    AppendRacieveData();

                    // 改行を検知
                    if (HasReceivedCompleteResponse(out int bitRead)) return bitRead;

                }
            }
        }
        // HasReceivedCompleteResponseメソッドは、受信したデータが完全な応答であるかどうかを確認します。
        private static bool HasReceivedCompleteResponse(out int bitRead)
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
        private static void AppendRacieveData()
        {
            string received = serialPort.ReadExisting();
            dataBuffer.Append(received);
        }
        // PLCからの応答を受信した後、ビットを取得します。





    }
}
