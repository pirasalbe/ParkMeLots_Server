using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace ServerGarage
{
    public delegate void NewConnectionEventHandler(object sender, Socket Sck);

    class Program
    {
        public static bool DEBUG = true;

        static void Main(string[] args)
        {
            CServer s = CServer.Instance;
            s.Port = 100;
            s.NewConnection += ManageUser;
            s.Start();
        }












        static void ManageUser(object sender, Socket Sck)
        {
            bool close = false;
            byte[] data;
            string req, ris="";

            Sck.ReceiveTimeout = 120000;
            Sck.SendTimeout = 120000;

            try
            {
                data = CServer.Instance.ReceiveData(Sck);
                req = ASCIIEncoding.ASCII.GetString(data);
                switch (req)
                {
                    case "SIN":
                        ris = SingIn();
                        break;
                    case "SUP":
                        SingUp();
                        break;
                    default:
                        CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: UNSUPPORTED COMMAND"));
                        Sck.Close();
                        Sck.Dispose();
                        close = true;
                        break;
                }
            }
            catch (ArgumentException e)
            {
                CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR:" + e.Message));
                Sck.Close();
                Sck.Dispose();
                close = true;
            }

            if (ris!="")
            {
                CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: "+ris));
                Sck.Close();
                Sck.Dispose();
                close = true;
            }
            while (!close)
                try
                {
                    data = CServer.Instance.ReceiveData(Sck);
                    req = ASCIIEncoding.ASCII.GetString(data);
                    switch (req)
                    {
                        case "ALV":
                            break;
                        default:
                            CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: UNSUPPORTED COMMAND"));
                            Sck.Close();
                            Sck.Dispose();
                            close = true;
                            break;
                    }
                }
                catch (ArgumentException e)
                {
                    CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR:"+e.Message));
                    Sck.Close();
                    Sck.Dispose();
                    close = true;
                }

            return;
        }
    }
}
