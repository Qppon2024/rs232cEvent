using System.IO.Ports;
using System.Text;
namespace ConsoleApp12
{
    public class Program
    {
  
        static void Main(string[] args)
        {
 
            //RecievePlcResponse();
            SerialPortHundler serialPortHundler = new();
            serialPortHundler.SendData();
            Console.WriteLine("シリアルポートを開きました。データ受信を待っています...");
            Console.WriteLine("終了するにはEnterキーを押してください。");
            Console.ReadLine();

            serialPortHundler.Close(); Console.WriteLine("Hello, World!");
        }


    }
}
