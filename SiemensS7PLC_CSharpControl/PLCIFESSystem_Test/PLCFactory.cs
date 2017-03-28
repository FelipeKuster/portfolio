using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using S7.Net;

namespace PLCIFESSystem_Test
{
    public class PLCFactory
    {
        private List<Plc> m_PLCConnectedList;
        const int maxAttempts = 5;
        public const string DefaultPlcIpServer1 = "192.168.0.9";  //Escravo
        public const string DefaultPlcIpServer2 = "192.168.0.11"; 


        public PLCFactory()
        {
            this.m_PLCConnectedList = new List<Plc>();
            Console.WriteLine("PLC Manager iniciado com sucesso.");
            Console.WriteLine("Criando instâncias de PLC...");
        }

        public void AddPLC(string ip, short rack, short slot)
        {
            int attempsToConnect = 0;
            Plc myPlc = new Plc(CpuType.S7300, ip, rack, slot);

            while ((!myPlc.IsConnected) && !(attempsToConnect >= maxAttempts))
            {
                connect(myPlc);
                attempsToConnect++;
            }

            if (!this.m_PLCConnectedList.Contains(myPlc) && myPlc.IsConnected)
            {
                m_PLCConnectedList.Add(myPlc);
                Console.WriteLine("PLC " + myPlc.IP + " cached");
            }
            else
            {
                Console.WriteLine("A conexão fracassou... encerrando o programa.");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        public void LoadDefaultPLCs()
        {
            this.AddPLC(DefaultPlcIpServer1,0,2);
            //this.AddPLC(DefaultPlcIpServer2, 0, 2);
        }

        public Plc GetPLCByIP(string ip)
        {
            return this.m_PLCConnectedList.FirstOrDefault(x => x.IP == ip);
        }

        private void connect(Plc myPlc)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Conectando PLC em " + myPlc.IP + " Rack " + myPlc.Rack + " Slot " + myPlc.Slot + "...");
            myPlc.Open();
            if (myPlc.LastErrorCode == ErrorCode.NoError)
            {
                Console.WriteLine("PLC conectado com sucesso");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erro ao conectar: " + myPlc.LastErrorCode.ToString());
                Console.WriteLine("Tentando novamente");
                Thread.Sleep(1000);
            }
        }
    }
}
