/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCBLEIoTConnector.Core.Models
{
    public class AppDataEntry
    {
        #region --- Properties ---
        public string moteeui { get; set; }
        public string dir { get; set; }
        public UserData userdata { get; set; }
        public MoteTx motetx { get; set; }
        public GWRX[] gwrx { get; set; }
        #endregion
    }

    public class UserData
    {
        #region --- Properties ---
        public int seqno { get; set; }
        public int port { get; set; }
        public string payload { get; set; }
        #endregion
    }

    public class MoteTx
    {
        #region --- Properties ---
        public float freq { get; set; }
        public string mode { get; set; }
        public string datr { get; set; }
        public string codr { get; set; }
        public bool adr { get; set; }
        #endregion
    }

    public class GWRX
    {
        #region --- Properties ---
        public string eui { get; set; }
        public string time { get; set; }
        public bool timefromgateway { get; set; }
        public int chan { get; set; }
        public int rfch { get; set; }
        public int rssi { get; set; }
        public int lsnr { get; set; }
        #endregion
    }
}
