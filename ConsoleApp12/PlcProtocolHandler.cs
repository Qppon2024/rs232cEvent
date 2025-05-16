using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp12
{
    public class PlcProtocolHandler
    {
        const string CTRL_READ = "139200";    //D5010
        const string CTRL_WRITE = "139300";    //D5011
        const string PLC_READ = "@00FA000000000010182";
        const string PLC_WRITE = "@00FA000000000010282";

        private string AddFcs(string str)
        {

            int fcs = BuildFcs(str);
            return $"{str}{fcs:X2}*\r";
        }

        private int BuildFcs(string str)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            // FCS計算
            return bytes.Aggregate(0, (current, b) => current ^ b);
        }
        public string BuildSendBit(int ctrlData)
        {
            string message = PLC_WRITE + CTRL_WRITE + $"{ctrlData:X4}";
            return AddFcs(message);;
        }
        public string BuildReciveCommand()
        {
            string message = PLC_READ + CTRL_READ + "0001";

            return AddFcs(message);
        }
        public bool ReadBit(string str,out int bitRead)
        {
            string ss = str.Substring(str.Length - 8, 4);
 
            return int.TryParse(ss, System.Globalization.NumberStyles.HexNumber, null, out bitRead);

        }

    }
}
