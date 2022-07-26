using PSU_Library;


namespace PSU_Consol // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private static ItechITM3100 PSURef { get; set; } = null!;
        private static PSUEmulator EmulatorRef { get; set; } = null!;
        static void Main(string[] args)
        {
            string Readinput = "";
            int Stage = 0;
            PSURef = new ItechITM3100();

            while (Stage != -1)//
            {
                //Console.Clear();
                if (Readinput!.ToLower().Contains("psuconnect"))
                {
                    if (Readinput!.ToLower().Contains(":"))
                    {
                        string[] TempStr = Readinput.Remove(0,11).Split(":");
                        if (TempStr[0].Length > 10)
                        {
                            PSURef.SetUpConn(TempStr[0], Convert.ToInt32(TempStr[1]), 3.0, 12.0);//we have set the MAX Voltage for the unit
                            Stage = 1;
                        }
                        else SessionTools.Write("\n{=Red}==IP Adress Not valid!=={/}\n");
                    }else SessionTools.Write("\n{=Red}==IP:Port Argument Missing!=={/}\n");

                }
                else if (Readinput!.ToLower().Contains("start emulator"))
                {
                    EmulatorRef = new PSUEmulator();
                    EmulatorRef.StartPSUEmulator();
                    Stage = 2;

                }
                else if (Readinput!.ToLower().Contains("getvoltage"))
                {
                    double? GetVolt = PSURef.GetVoltage();
                    SessionTools.Write("".PadLeft(18) + "{=Green}Running Voltage:{/} {=Magenta}" + GetVolt + "{/}");
                }
                else if (Readinput!.ToLower().Contains("getcurrent"))
                {
                    double? GetCurr = PSURef.GetCurrent();
                    SessionTools.Write("".PadLeft(18) + "{=Green}Running Current:{/} {=Magenta}" + GetCurr + "{/}");
                }
                //                                                          //Set Commands
                else if (Readinput!.ToLower().Contains("setcurrent"))
                {
                    if (IsCorrectDouble(Readinput) == true)
                    {
                        var cResult = PSURef.SetCurrent(SessionTools.GetDoubleFromFunc(Readinput));
                        SessionTools.Write("".PadLeft(18) + "{=Green}Current is now set to:{/} {=Magenta}" + cResult + "{/}");
                    }
                    else SessionTools.Write("\n{=Red}==Invalid Amount Entered=={/}\n");
                    
                }
                else if (Readinput!.ToLower().Contains("setvoltage"))
                {
                    if (IsCorrectDouble(Readinput) == true)
                    {
                        var vResult = PSURef.SetVoltage(SessionTools.GetDoubleFromFunc(Readinput));
                        SessionTools.Write("".PadLeft(18) + "{=Green}Voltage is now set to:{/} {=Magenta}" + vResult + "{/}");
                    }else SessionTools.Write("\n{=Red}==Invalid Amount Entered=={/}\n");
                }
                //if (EmulatorRef.IsInitialized == true) Stage = 1;
                if (Stage == 0)
                {
                    Console.WriteLine("[1] To Connect To PSU Type: (PSUConnect IP:PORT)");
                    Console.WriteLine("[2] To start simulator tpye: (Start Emulator)");
                    Console.WriteLine();
                    Console.WriteLine("--------------------");
                    Console.WriteLine("----PSU Commands----");
                    Console.WriteLine("--------------------");
                    Console.WriteLine("(1) SetVoltage x.x");
                    Console.WriteLine("(2) SetCurrent x.x");
                    Console.WriteLine("(3) GetVoltage");
                    Console.WriteLine("(4) GetCurrent");
                    Console.WriteLine();
                    
                }
                Readinput = Console.ReadLine()!;
            }
                
        }
        public static bool IsCorrectDouble(string StrRef)//some Checking for typrewrite
        {
            if (StrRef == null) return false;
            if (StrRef.Contains(" ")==false) return false;
            if (StrRef.Contains(".") == false) return false;

            //now check the decimals
            string[] Ssplit = StrRef.Split(" ");
            string[] DecimalSplit = Ssplit[1].Split(".");
            if (DecimalSplit[0].Length==0) return false;
            if (DecimalSplit[1].Length==0) return false;
            return true;
        }
    }
}
