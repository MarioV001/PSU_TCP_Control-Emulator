using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PSU_Library
{
    public class PSUEmulator : IPowerSupply
    {
        readonly int LocalPort = 9899;
        public bool IsInitialized { get; private set; } = false;
        public TcpListener TCP_session = null!;

        private double SimulatedCurrent = 4.1;
        private double SimulatedVoltage = 9.774;

        public void StartPSUEmulator()
        {
            if (IsInitialized == false)
            {
                

                String strHostName = string.Empty;
                //get LAN ip Adress
                IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] addr = ipEntry.AddressList;
                Console.WriteLine("Emulator Starting...");

                //Start the server
                TCP_session = new TcpListener(addr[1], LocalPort);
                TCP_session.Start();

                //Console.WriteLine("Emulator Started On: {0}:{1}", addr[1].ToString(), LocalPort);
                SessionTools.Write("{=Yellow}Emulator Started On: "+ addr[1].ToString() + ":" + LocalPort + "{/}");
                IsInitialized = true;

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null!;

                // Enter the listening loop.
                while (true)
                {
                    TcpClient client = TCP_session.AcceptTcpClient();
                    //Console.WriteLine("({0}) Connected!", client.Client.LocalEndPoint!.ToString());
                    SessionTools.Write("("+client.Client.LocalEndPoint!.ToString() + ") {=Green}Connected!{/} ");

                    data = null!;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;
                    Thread.Sleep(100);
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        
                        //Console.WriteLine("".PadLeft(18) + "Received ({0}) {1}", client.Client.LocalEndPoint!.ToString(), data);
                        SessionTools.Write("".PadLeft(18) + "{=Green}Received{/} (" + client.Client.LocalEndPoint!.ToString() + ") " + "{=Blue}"+data+"{/}");
                        OnReceiveData(data, client);
                    }

                }


             }
        }

        public void OnReceiveData(string Data, TcpClient client)
        {
            switch (Data) //can be simplifed to not re-use the code
            {
                case "CURRent?":
                    SessionTools.WriteServer(SimulatedCurrent.ToString(), client);
                    //Console.WriteLine("".PadLeft(20) + "Response: {0}", SimulatedCurrent.ToString());//print what was sent on the server side
                    SessionTools.Write("".PadLeft(20) + "{=Magenta}Response: {/} " + SimulatedCurrent.ToString() + "{/}");
                    break;
                case "VOLTage?":
                    SessionTools.WriteServer(SimulatedVoltage.ToString(), client);
                    //Console.WriteLine("".PadLeft(20) + "Response: {0}", SimulatedVoltage.ToString());//print what was sent on the server side
                    SessionTools.Write("".PadLeft(20) + "{=Magenta}Response: {/} " + SimulatedVoltage.ToString() + "{/}");
                    break;
                case string b when b.Contains("CURRent "):
                    SimulatedCurrent = SessionTools.GetDoubleFromFunc(Data);
                    SessionTools.Write("".PadLeft(20) + "{=Magenta}New CURRent Set: {/} " + SimulatedCurrent.ToString() + "{/}");
                    break;
                case string c when c.Contains("VOLTage "):
                    SimulatedVoltage = SessionTools.GetDoubleFromFunc(Data);
                    SessionTools.Write("".PadLeft(20) + "{=Magenta}New VOLTage Set: {/} " + SimulatedVoltage.ToString() + "{/}");
                    break;
            }
        }

        
       
        public bool PowerSwitch(bool isOn)
        {
            throw new NotImplementedException();
        }

        public double? SetCurrent(double value)
        {
            throw new NotImplementedException();
        }

        public void SetPower(double currency, double voltage)
        {
            throw new NotImplementedException();
        }

        public double? SetVoltage(double value)
        {
            throw new NotImplementedException();
        }
    }
}
