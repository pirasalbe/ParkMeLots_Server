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
            s.Port = 2000;
            s.NewConnection += ManageUser;
            s.Start();
        }












        static void ManageUser(object sender, Socket Sck)
        {
            bool close = false;
            byte[] msg;
            string req = "", ris = "", userType = "";
            Sck.ReceiveTimeout = 120000;
            Sck.SendTimeout = 120000;

            try
            {
                msg = CServer.Instance.ReceiveData(Sck);
                req = ASCIIEncoding.ASCII.GetString(msg);
                switch (req)
                {
                    case "USR":
                        userType = "NormalUser";
                        break;
                    case "ADM":
                        if (CServer.Instance.Auth(Sck))
                            userType = "AdminUser";
                        break;
                    case "SGN":
                        userType = "Signal";
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
            /*
            if (userType == "NormalUser")
                while (!close)
                    try
                    {
                        data = CServer.Instance.ReceiveData(Sck);
                        req = ASCIIEncoding.ASCII.GetString(data);
                        switch (req)
                        {
                            case "RQST_RGG":
                                via= ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));
                                //pos= ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));
                                req=CDatabase.Instance.Request(via);

                                break;
                                

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
                        CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR:" + e.Message));
                        Sck.Close();
                        Sck.Dispose();
                        close = true;
                    }
            else if (userType == "AdminUser")
                while (!close)
                    try
                    {
                        data = CServer.Instance.ReceiveData(Sck);
                        req = ASCIIEncoding.ASCII.GetString(data);
                        switch (req)
                        {
                            case "NEW_SGN":
                                tipo=
                                pos = ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));

                                break;
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
                        CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR:" + e.Message));
                        Sck.Close();
                        Sck.Dispose();
                        close = true;
                    }

            */
            if(userType== "Signal")
            {
                int codSign;
                string data;
                long dataCreazione, dataFine;
                string pos;
                int key = Convert.ToInt32(ASCIIEncoding.ASCII.GetString(( CServer.Instance.ReceiveData(Sck))));
                if(key==0)
                    key = CDatabase.NewEntry();
                CServer.Instance.SendData(Sck,ASCIIEncoding.ASCII.GetBytes(Convert.ToString(key)));
                while (!close)
                    try
                    {
                        req = ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));
                        switch (req)
                        {
                                case "UPT":
                                //codSign = Convert.ToInt32(CServer.Instance.ReceiveData(Sck));
                                //dataCreazione= Convert.ToInt64(CServer.Instance.ReceiveData(Sck));
                                //dataFine= Convert.ToInt64(CServer.Instance.ReceiveData(Sck));
                                //data = ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));
                                //pos=ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData)
                                Console.WriteLine(ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck)));
                                break;
                            default:
                                CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: UNSUPPORTED COMMAND"));
                                Sck.Close();
                                Sck.Dispose();
                                close = true;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERR: " + e.Message +" con key=" +key);
                        
                        if(!(e as SocketException!=null))
                             CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: " + e.Message));
                        Sck.Close();
                        Sck.Dispose();
                        close = true;

                    }
            }
            return;
        }
    }


}
