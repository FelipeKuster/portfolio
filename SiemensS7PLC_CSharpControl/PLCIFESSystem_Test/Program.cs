using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using S7.Net;

namespace PLCIFESSystem_Test
{
    public class Program
    {

        static PLCFactory m_PLCFactory = null;


        public static void Main(string[] args)
        {
            m_PLCFactory = new PLCFactory();
            m_PLCFactory.LoadDefaultPLCs();
            
            Plc slave = GetPLCFactory().GetPLCByIP(PLCFactory.DefaultPlcIpServer1);
            //Plc master = GetPLCFactory().GetPLCByIP(PLCFactory.DefaultPlcIpServer2);



            while (true)
            {
                //bitTest(slave, master);
                //overflowTest(slave, master);
                //DigitalIOTest(slave, master);
            }
        }

        private static void bitTest(Plc slave, Plc master)
        {
            slave.Write("DB1.DBW0", 1);
            master.Write("M30.0", 1);
            Thread.Sleep(1000);
            slave.Write("M0.0", 0);
            master.Write("M30.0", 0);
            Thread.Sleep(1000);
        }

        private static void overflowTest(Plc slave, Plc master)
        {
            int stackOverflowTest = 1;
            int nextStack;
            Thread.Sleep(1500);
            while (Int16.MaxValue > stackOverflowTest)
            {
                Console.WriteLine("Testando estouro de pilha -> Valor atual = " + stackOverflowTest);
                slave.Write("DB1.DBW0", stackOverflowTest);
                Thread.Sleep(1500);
                nextStack = Convert.ToInt32(master.Read("MW20"));
                stackOverflowTest = nextStack;
            }
        }

        public static void DigitalIOTest(Plc slave, Plc master)
        {
            bool statusRotTest;
            bool statusRotMagazine;
            bool statusCilRecuado;
            bool statusCilAvan;
            bool posicionada;
            bool ventosa;

            while(true)
            {
                statusRotTest = Convert.ToBoolean(slave.Read("I4.1"));
                statusRotMagazine = Convert.ToBoolean(slave.Read("I4.0"));
                statusCilRecuado = Convert.ToBoolean(slave.Read("I4.2"));
                statusCilAvan = Convert.ToBoolean(slave.Read("I4.3"));
                posicionada = Convert.ToBoolean(slave.Read("I4.4"));

                string rota, cil, posi;

                if (statusRotTest)
                {
                    rota = "Posição de teste";
                }
                else if(statusRotMagazine)
                {
                    rota = "Posição de Magazine";
                }

                else
                {
                    rota = "Em curso";
                }

                if(statusCilAvan)
                {
                    cil = "avançado";
                }

                else if(statusCilRecuado)
                {
                    cil = "recuado";
                }
                else
                {
                    cil = "em curso";
                }

                if(posicionada)
                {
                    posi = "posicionada";
                }
                else
                {
                    posi = "NÃO posicionada";
                }

                Console.WriteLine("Rotatória: " + rota + "\t Cilíndro: " + cil + "\t Peça: " + posi);
                Thread.Sleep(500);
            }
        }

        public void Atuar(Plc slave)
        {
            bool sensorRotTest;
            bool sensorRotMagazine;
            bool sensorCilRecuado;
            bool sensorCilAvan;
            bool sensorPosicionada;
            bool atuadorRotTestMagazine;
            bool ventosa = false;


            sensorRotTest = Convert.ToBoolean(slave.Read("I4.1"));
            sensorRotMagazine = Convert.ToBoolean(slave.Read("I4.0"));
            sensorCilRecuado = Convert.ToBoolean(slave.Read("I4.2"));
            sensorCilAvan = Convert.ToBoolean(slave.Read("I4.3"));
            sensorPosicionada = Convert.ToBoolean(slave.Read("I4.4"));
            atuadorRotTestMagazine = Convert.ToBoolean(slave.Read("Q.4.5"));

            if((!sensorPosicionada && !sensorRotMagazine) || atuadorRotTestMagazine)
            {
                CilindroAvanca(slave);
            }
        }

        private void CilindroAvanca(Plc atuador)
        {
            atuador.Write("Q4.0", true);
        }

        private void RotTesteMag(Plc atuador)
        {
            atuador.Write("Q.4.1", true);
        }

        private void RotMagTeste(Plc atuador)
        {
            atuador.Write("Q.4.2", true);
        }

        private void VentosaOn(Plc atuador)
        {
            atuador.Write("Q.4.3", true);
        }

        private void VentosaOff(Plc atuador)
        {
            atuador.Write("Q.4.4", true);
        }

        public static PLCFactory GetPLCFactory()
        {
            return m_PLCFactory;
        }
    }
}
