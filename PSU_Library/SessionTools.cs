
using System.Net.Sockets;
using System.Text;

namespace PSU_Library
{
    public static class SessionTools
    {
        public static string WriteAndFlush(string WRITE, TcpClient _tcpClient,bool Formated = true)
        {
            if (Formated == true)
            {
                NetworkStream stream = _tcpClient.GetStream();
                Byte[] data = Encoding.ASCII.GetBytes(WRITE);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                if (_tcpClient.Connected == false)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            else
            {

            }
            return "";
        }
        public static double ReadDoubleAsync(TcpClient _tcpClient)
        {
            try
            {
                byte[] responseInBytes = new byte[4096];
                var reader = new BinaryReader(_tcpClient.GetStream());

                int bytes = reader.Read(responseInBytes, 0, responseInBytes.Count());
                return Convert.ToDouble(Encoding.UTF8.GetString(responseInBytes));
            }
            catch
            {
                Console.WriteLine("ERROR: disconnected on request.");
            }
            return 0.0;
        }
        /// <summary>
        /// 
        ///Consol manual Syntax hilighter
        /// </summary>
        /// <param name="msg"></param>
        public static void Write(string msg)//text Syntax
        {
            msg = msg + "\r\n";//add new line to end
            string[] ss = msg.Split('{', '}');
            ConsoleColor c;
            foreach (var s in ss)
                if (s.StartsWith("/"))
                    Console.ResetColor();
                else if (s.StartsWith("=") && Enum.TryParse(s.Substring(1), out c))
                    Console.ForegroundColor = c;
                else
                    Console.Write(s);
        }
        public static double GetDoubleFromFunc(string stFunction)
        {
            string[] Ssplit = stFunction.Split(" ");
            return Convert.ToDouble(Ssplit[1]);
        }
        ///
        ///Server_Side
        ///
        public static string WriteServer(string WRITE, TcpClient client)
        {

            NetworkStream stream = client.GetStream();
            Byte[] data = Encoding.ASCII.GetBytes(WRITE);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            return "";
        }
        
    }
}
