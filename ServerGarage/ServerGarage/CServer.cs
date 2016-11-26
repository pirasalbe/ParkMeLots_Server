using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ServerGarage
{
    class CServer
    {
        private bool DEBUG = Program.DEBUG;

        public event NewConnectionEventHandler NewConnection;

        public bool IsStopped = false;
        private Thread mThreadListener;
        private Socket mListener;
        private static int mPort;

        #region singleton
        private static CServer instance;

        private CServer()
        {
        }

        public static CServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CServer();
                }
                return instance;
            }
        }
        #endregion singleton

        #region Events
        private void OnNewConnection(object Sck)
        {
            NewConnectionEventHandler handler = NewConnection;
            if (handler != null)
                handler(this, Sck as Socket);
        }

        #endregion Events

        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, mPort);
            mListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mListener.Bind(localEndPoint);
            mListener.Listen(mPort);
            mThreadListener = new Thread(new ThreadStart(StartAcceptUsersConnection));
            mThreadListener.Start();
        }

        private void StartAcceptUsersConnection()
        {

            SocketAsyncEventArgs asyncConnection;
            bool IncomingConnection = false;
            Thread ConnectionManager;
            if (DEBUG)
                CIO.DebugOut("Start waiting connection...");
            while (!IsStopped)
            {
                IncomingConnection = false;
                asyncConnection = new SocketAsyncEventArgs();
                asyncConnection.Completed += (object sender, SocketAsyncEventArgs e) => { IncomingConnection = true; };
                mListener.AcceptAsync(asyncConnection);
                if (DEBUG)
                    CIO.DebugOut("Waiting connection...");
                while (!IncomingConnection && !IsStopped)
                {
                    Thread.Sleep(1000);
                }
                if (IncomingConnection && !IsStopped)
                {
                    if (DEBUG)
                        CIO.DebugOut("Established connection!");
                    ConnectionManager = new Thread(new ParameterizedThreadStart(OnNewConnection));
                    ConnectionManager.Start(asyncConnection.AcceptSocket);
                }
                asyncConnection.Dispose();
            }
            //CloseAllConnection();
            if (DEBUG)
                CIO.WriteLine("Chiuse tutte le connessioni con gli users");
        }

        public string DoCommand(string Command)
        {
            try
            {
                switch (Command)
                {
                    case "Stop":
                        IsStopped = true;
                        break;
                    default:
                        return "Didn't find.";
                }
                return "Command exexuted.";
            }
            catch
            {
                return "Error.";
            }
        }

        public byte[] ReceiveData(Socket Receiving)
        {
            byte[] data = new byte[4];
            Receiving.Receive(data);
            data = new byte[BitConverter.ToInt32(data, 0)];
            Receiving.Receive(data);
            return data;
        }

        public void SendData(Socket Dispatcher, byte[] data)
        {
            Dispatcher.Send(BitConverter.GetBytes(data.Length));
            Dispatcher.Send(data);
        }

        public bool Auth(Socket Sck)
        {
            return true;
        }
    }
}