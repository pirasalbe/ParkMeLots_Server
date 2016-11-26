using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace ServerGarage
{
    class CDatabase
    {
        private SignalContext db = new SignalContext();

        List<int> UsedKey;

        #region singleton
        private static CDatabase instance;

        private CDatabase()
        {
            UsedKey = new List<int>();
            var allRecord = db.Signals.Select(Signal => new
            {
                Id = Signal.Id,
            });
            foreach (var s in allRecord)
                UsedKey.Add(s.Id);
            UsedKey.Sort();
        }

        public static CDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CDatabase();
                }
                return instance;
            }
        }
        #endregion singleton

        public int GenKey()
        {
            int ris = 1, c = 0;
            lock (UsedKey)
            {
                bool found = false;
                while (!found && c < UsedKey.Count())
                {
                    if (ris < UsedKey[c])
                        found = true;
                    else if (ris == UsedKey[c])
                    {
                        ris++;
                        c++;
                    }
                    else if (ris > UsedKey[c])
                        c++;
                }
                UsedKey.Insert(c, ris);
            }
            return ris;
        }

        public void UpdateRecord(int key, int codSign, long dataCreazione,long dataFine,double longitude, double latitudine, bool side)
        {
            lock (db)
            {
                var result = db.Signals.SingleOrDefault(b => b.Id == key);
                if (result != null)
                {
                    result.Id = key;
                    result.SignCode = codSign;
                    result.Begin = dataCreazione;
                    result.End = dataFine;
                    result.Longitude = longitude;
                    result.Latitude = latitudine;
                    result.Direction = side;
                }
                Sign newSign = new Sign() { Id = key, SignCode = codSign, Begin = dataCreazione, End = dataFine, };
                db.Signals.Add(newSign);
                db.SaveChanges();
            }
        }
    }


    class SignalContext : DbContext
    {
        public SignalContext() : base("name=DBRoadSign")
        { }

        public DbSet<Sign> Signals { get; set; }
    }

    class Sign
    {
        public Sign()
        { }

        public int Id { get; set; }
        public int SignCode { get; set; }
        public long Begin { get; set; }
        public long End { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool Direction { get; set; }
    }
}
