using System.Globalization;
using System.Net.Sockets;

namespace PSU_Library
{
   
    public class ItechITM3100 : IPowerSupply, IDisposable
    {
        public bool IsInitialized { get; private set; } = false;
        public string PSUServerIP { get; private set; } = "192.168.0.255";
        public int PSUServerPort { get; private set; } = 7777;

        public double MaxCurrent { get; private set; } = 5.5;//defined
        public double MaxVoltage { get; private set; } = 12.0;

        public TcpClient TCP_session = null!;

        public async void SetUpConn(string IPAdress, int PORT, double maxCurrent, double maxVoltage,bool EnableAutoResponse=false)
        {
            if (TCP_session !=null && TCP_session.Connected)
                Console.WriteLine("Allready Connected to {0}:{1}", PSUServerIP, PSUServerPort);
            else{
                MaxCurrent = maxCurrent;//can be used to over-ride different models
                MaxVoltage = maxVoltage;//can be used to over-ride different models
                PSUServerIP =IPAdress;
                PSUServerPort=PORT;

                Console.WriteLine("Connecting...");
                try
                {
                    TCP_session = new TcpClient();
                    //Console.Clear();
                    await TCP_session.ConnectAsync(PSUServerIP, PSUServerPort);

                    SessionTools.Write("{=Green}Connected To:{/} " + PSUServerIP + ":" + PSUServerPort);
                    IsInitialized = true;

                    if(EnableAutoResponse==true)//this will enable to Get/Update any response from the server/PSU asynchronously 
                    {
                        Task TTask = Task.Run(() => GetStreamData(TCP_session));
                        await TTask;
                        //Task.Run(async () => await InitializeDevice(TCP_session)); //(dos NOT! await correctly)
                    }                
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static async Task GetStreamData(TcpClient clt)///SD
        {
            using (var reader = new StreamReader(clt.GetStream()))
            {
                char[] buffer = new char[256];
                while ((await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string sChar = new string(buffer);
                    SessionTools.Write("".PadLeft(18) + "{=Green}Received{/} {=Blue}" + sChar + "{/}");
                }
            }

        }
        
        /// <summary>
        /// 
        ///
        /// </summary>
        /// <param name="value">Current amount to set</param>
        /// <returns>The Current that has been set</returns>
        public double? SetCurrent(double value)
        {
            var settedValue = GetCurrent();
            if (settedValue is null)
                return null;
            if (value == settedValue.Value)
                return value;

            if (value > MaxCurrent)//make sure we dont over-current the MAX limit
                value = MaxCurrent;
            value = Math.Max(value, 0);

            try
            {
                var curr = value.ToString("0.00", CultureInfo.InvariantCulture);
                Thread.Sleep(100);//solve problem with not able to compute fast enough
                SessionTools.WriteAndFlush("CURRent " + curr, TCP_session);
                return GetCurrent();//make sure we get live update
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        /// <summary>
        /// 
        ///
        /// </summary>
        /// <param name="value">Voltage amount to set</param>
        /// <returns>The Voltage that has been set</returns>
        public double? SetVoltage(double value)
        {
            var settedValue = GetVoltage();
            if (settedValue is null)
                return null;
            if (value == settedValue.Value)
                return value;

            if (value > MaxVoltage)//make sure we dont over-current the MAX limit
                value = MaxVoltage;
            value = Math.Max(value, 0);

            try
            {
                var Volts = value.ToString("0.0", CultureInfo.InvariantCulture);
                Thread.Sleep(100);//solve problem with not able to compute fast enough
                SessionTools.WriteAndFlush("VOLTage " + Volts, TCP_session);
                return GetVoltage();//make sure we get live update
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 
        ///
        /// </summary>
        /// <param name="isOn">Sets the Turn On/Off State</param>
        /// <returns>returns power status</returns>
        public bool PowerSwitch(bool isOn)
        {
            var state = isOn ? "ON" : "OFF";
            try
            {
                SessionTools.WriteAndFlush("OUTPut " + state, TCP_session);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 
        ///
        /// </summary>
        /// <param name="value">Current amount to set</param>
        /// <returns>The Current that has been set</returns>
        public void SetPower(double currency, double voltage)
        {
            voltage = Math.Min(voltage, MaxVoltage);
            currency = Math.Min(currency, MaxCurrent);

            try
            {
                var volt = voltage.ToString("0.0", CultureInfo.InvariantCulture);
                var curr = currency.ToString("0.00", CultureInfo.InvariantCulture);
                SessionTools.WriteAndFlush("APPLy " + volt + ", " + curr, TCP_session);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        //                            //
        //=======Get Functions========//
        //                            //
        public double? GetCurrent()
        {
            try
            {
                                          //LIST:STEP:CURRent? <NR1>
                                          //BATTery:CHARge:CURRent?     - (charging Current)
                SessionTools.WriteAndFlush("CURRent?", TCP_session);
                return SessionTools.ReadDoubleAsync(TCP_session);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0.0;
        }
        public double? GetVoltage()
        {
            try
            {
                                          //LIST:STEP:VOLTage? <NR1>
                                          //BATTery:CHARge:VOLTage?     - (charging voltage)
                SessionTools.WriteAndFlush("VOLTage?", TCP_session);
                return SessionTools.ReadDoubleAsync(TCP_session);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0.0;
        }
        public double? GetTime()
        {
            try
            {
                SessionTools.WriteAndFlush("FETCh:TIME?", TCP_session);
                return SessionTools.ReadDoubleAsync(TCP_session);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0.0;
        }

        public void Dispose()
        {
            if (TCP_session.Connected)
            {
                PowerSwitch(false);//turn off the PSU on dispose
                TCP_session.Close();//close connection
                TCP_session.Dispose();//dispose memmory
            }
        }
    }
}