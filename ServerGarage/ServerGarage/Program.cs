using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Data.Entity;


namespace ServerGarage
{
    public delegate void NewConnectionEventHandler(object sender, Socket Sck);

    class Program
    {
        public static int c = 0;
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
            string req = "", userType = "";
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
                c++;
                string[] allData;
                int codSign;
                Console.WriteLine(c + " host collegati.");
                long dataCreazione, dataFine;
                double longitudine, latitudine;
                int rightSide;
                bool side = false;//0=left, 1=right
                int key = Convert.ToInt32(ASCIIEncoding.ASCII.GetString(( CServer.Instance.ReceiveData(Sck))));
                if(key==0)
                    key = CDatabase.Instance.GenKey();
                CServer.Instance.SendData(Sck,ASCIIEncoding.ASCII.GetBytes(Convert.ToString(key)));
                while (!close)
                    try
                    {
                        
                        req = ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck));
                        Console.WriteLine("UPD Received.");
                        switch (req)
                        {
                            case "UPT":
                                allData = ASCIIEncoding.ASCII.GetString(CServer.Instance.ReceiveData(Sck)).Split(new char[] { ';' });
                                codSign = Convert.ToInt32(allData[1]);
                                dataCreazione = Convert.ToInt64(allData[2]);
                                dataFine = Convert.ToInt64(allData[3]);
                                allData[4]=allData[4].Replace(',', '.');
                                allData[5] = allData[5].Replace(',', '.');
                                longitudine = Convert.ToDouble(allData[4]);
                                latitudine = Convert.ToDouble(allData[5]);
                                rightSide = Convert.ToInt16(allData[6]);
                                if (rightSide != 0)
                                    side = false;
                                else
                                    side = true;
                                CDatabase.Instance.UpdateRecord(key, codSign, dataCreazione, dataFine, longitudine, latitudine, side);
                                //Console.WriteLine("Meme");
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

                        if (e as SocketException == null)
                            try
                            {
                                CServer.Instance.SendData(Sck, ASCIIEncoding.ASCII.GetBytes("ERR: " + e.Message));
                            }
                            catch { }
                        Sck.Close();
                        Sck.Dispose();
                        close = true;

                    }
            }
            c--;
            return;
        }
    }


    

}
