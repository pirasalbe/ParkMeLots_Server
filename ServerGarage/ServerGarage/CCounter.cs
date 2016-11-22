using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerGarage
{
    class CCounter
    {
        private int mCounter;
        private bool mStop;
        public CCounter()
        {
            mCounter = 0;
            mStop = false;
        }

        public int Counter
        {
            get { return mCounter; }
        }

        public void Start()
        {
            Thread t = new Thread(Go);
            t.Start();
        }

        public void Stop()
        {
            mStop = true;
        }

        public void Reset()
        {
            mCounter = 0;
        }
        private void Go()
        {
            while (!mStop)
            {
                Thread.Sleep(1000);
                mCounter += 1;
            }
        }
    }
}
